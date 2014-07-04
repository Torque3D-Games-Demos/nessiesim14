new ScriptObject(TouristSMTemplate) {
   transition[null, relax] = relaxed;

   // Relaxed state: loiter near an umbrella.
   transition[relaxed, newSpot] = walking;
   transition[relaxed, monsterNoise] = scared;
   transition[relaxed, touristScared] = inquiring;

   // Walking state: go long-distance to a different umbrella.
   transition[walking, monsterNoise] = scared;
   transition[walking, reachDestination] = relaxed;

   // Get bored of this eventually.
   transition[inquiring, timeOut] = relaxed;
   transition[inquiring, monsterNoise] = scared;

   // Once scared, we can escalate or de-escalate.
   transition[scared, monsterNoise] = getHelp;
   transition[scared, timeOut] = relaxed;

   // Seek help from a Ranger.
   transition[getHelp, noHelp] = fleeing;
   transition[getHelp, reachDestination] = returnToTheScene;

   // Attacks!
   transition[_, attackNear] = getHelp;
   transition[_, attackFar] = scared;
};

function TouristSM::enterRelaxed(%this) {
   %obj = %this.owner;
   %obj.setShapeName(" Relaxed ");
   %obj.wander = schedule(getRandom(5000, 10000), %obj, Tourist__wander, %obj);
}

function Tourist__wander(%obj) {
   %spot = chooseTouristSpot(%obj.spot);
   %obj.setMoveSpeed(0.2);
   %obj.setPathDestination(getWords(%spot, 1, 3));
}

function Tourist::onReachPathDestination(%this, %obj) {
   switch$(%obj.sm.state) {
      case relaxed:
         %obj.wander = schedule(getRandom(5000, 10000), %obj, Tourist__wander, %obj);

      case getHelp:
         postEvent(Tourist, AskHelp, %obj);
   }
   %obj.onEvent(reachDestination);
}

function TouristSM::leaveRelaxed(%this) {
   cancel(%this.owner.wander);
}

function TouristSM::enterScared(%this) {
   %obj = %this.owner;
   %obj.becomeScared = schedule(getRandom(0, 500), %this, Tourist__becomeScared, %obj);
   %obj.timeOut(getRandom(5000, 10000));
}

function TouristSM::leaveScared(%this) {
   %obj = %this.owner;
   %obj.stopTimeOut();
   %obj.clearAim();
   if(%obj.becomeScared !$= "") {
      cancel(%obj.becomeScared);
   }
}

function Tourist__becomeScared(%obj) {
   %obj.setShapeName(" Scared ");
   %obj.clearPathDestination();
   %obj.setAimObject(TheMonster);
   %obj.becomeScared = "";
   postEvent(Tourist, Scared, %obj);
}

function TouristSM::enterInquiring(%this) {
   %obj = %this.owner;
   %obj.setShapeName(" R U OK? ");
   %obj.setAimObject(%obj.inquiring);
   %obj.timeOut(getRandom(3000, 7000));
}

function TouristSM::leaveEnquiring(%this) {
   %obj.stopTimeOut();
   %obj.clearAim();
}

function TouristSM::enterGetHelp(%this) {
   %obj = %this.owner;
   %obj.threshold = 2;
   %r = findNearestRanger(%obj.getPosition());
   if(isObject(%r)) {
      %obj.helpLocation = chooseGroundPos(%obj.getPosition(), 3);
      %obj.setShapeName(" Help! ");
      %obj.setMoveSpeed(1);
      %pos = chooseGroundPos(%r.getPosition(), 3);
      %obj.setPathDestination(%pos);
   }
}

function TouristSM::enterFleeing(%this) {
   %obj = %this.owner;
   %obj.fetchLocation = %obj.getPosition();
   %obj.setShapeName(" Fleeing ");
   %obj.setMoveSpeed(1);
   // Temporary
   %obj.setPathDestination("45 -235 5");
}

function TouristSM::enterReturnToTheScene(%this) {
   %obj = %this.owner;
   %obj.setMoveSpeed(0.5);
   %obj.setPathDestination(%obj.helpLocation);
}

function Tourist::onCollision(%this, %obj, %col) {
   if(%obj.sm.state $= getHelp && %col.getDataBlock() $= Ranger) {
      %obj.clearPathDestination();
      postEvent(Tourist, AskHelp, %obj);
      %obj.onEvent(reachDestination);
   }
}

function Tourist::onMonsterAttack(%this, %obj, %pos) {
   //error(%obj SPC %pos);
}

function Tourist::onTouristScared(%this, %obj, %scared) {
   if(%scared == %obj) return;
   %d = VectorLen(VectorSub(%obj.getPosition(), %scared.getPosition()));
   if(%d < 10) {
      %obj.inquiring = %scared;
      %obj.onEvent(touristScared);
   }
}

function Tourist::onMonsterSwim(%this, %obj, %pos) {
   %p1 = getWords(%obj.getPosition(), 0, 1) SPC 0;
   %p2 = getWords(TheMonster.getPosition(), 0, 1) SPC 0;
   %d = VectorLen(VectorSub(%p1, %p2));
   if(%d < 10) {
      %obj.increaseDetection(1);
   }
}

function Tourist::onDetectionChange(%this, %obj, %val) {
   if(%val == %obj.threshold) {
      %obj.onEvent(monsterNoise);
      %obj.resetDetection();
   }
}

function Tourist::onMonsterBubble(%this, %obj, %pos) {
   %p1 = getWords(%obj.getPosition(), 0, 1) SPC 0;
   %p2 = getWords(%pos, 0, 1) SPC 0;
   %d = VectorLen(VectorSub(%p1, %p2));
   if(%d < 10) {
      %obj.increaseDetection(3);
   } else if(%d < 25) {
      %obj.increaseDetection(2);
   }
}

function Tourist::onMonsterAttack(%this, %obj, %pos) {
   %p1 = getWords(%obj.getPosition(), 0, 1) SPC 0;
   %p2 = getWords(%pos, 0, 1) SPC 0;
   %d = VectorLen(VectorSub(%p1, %p2));
   if(%d < 5) {
      postEvent(Tourist, Eaten, %obj.getPosition());
      %obj.schedule(750, delete, %obj);
   } else if(%d < 30) {
      %obj.onEvent(attackNear);
   } else if(%d < 50) {
      %obj.onEvent(attackFar);
   }
}

$touristSpots =
   "13 151 3" NL
   "-71 108 2.5" NL
   "-42 75 2.5" NL
   "9 80 3";
$numTouristSpots = getRecordCount($touristSpots);

function chooseTouristSpot(%i) {
   %idx = %i $= "" ? getRandom(0, $numTouristSpots-1) : %i;
   %pos = getRecord($touristSpots, %idx);
   %pos = chooseGroundPos(%pos, 10);
   return %idx SPC %pos;
}

function Tourist::onAdd(%this, %obj) {
   // Subscribe to messages.
   subscribe(%obj, Monster, Attack);
   subscribe(%obj, Monster, Swim);
   subscribe(%obj, Monster, Bubble);
   subscribe(%obj, Tourist, Scared);

   // Make a state machine!
   %obj.sm = makeSM(Tourist, %obj);
   %obj.onEvent(relax);

   %obj.threshold = 4;

   // Set initial look direction based on slope, or randomly.
   %pos = %obj.getPosition();
   %end = VectorSub(%pos, "0 0 10");
   %ray = ContainerRayCast(%pos, %end, $TypeMasks::StaticObjectType);
   if(isObject(getWord(%ray, 0))) {
      %norm = getWords(%ray, 4, 5) SPC 0;
      if(VectorLen(%norm) > 0.001) {
      } else {
         %norm = VectorNormalize(%norm);
      }
   }
   if(%norm $= "") {
      %ang = getRandom() * 2 * 3.14159;
      %norm = mSin(%ang) SPC mSin(%ang) SPC 0;
   }
   %dir = VectorScale(%norm, 1000);
   %look = VectorAdd(%pos, %dir);
   %obj.setAimLocation(%look);
}
