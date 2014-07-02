exec("scripts/navigation/main.cs");
exec("scripts/stateMachine/main.cs");
exec("scripts/events/main.cs");

// Convenience function for state machines.
function makeSM(%type, %obj) {
   %parent = %type @ SM;
   eval("%sm = new ScriptObject(\"\" : " @ %parent @ Template @ ");");
   %sm.superclass = StateMachine;
   %sm.class = %parent;
   %sm.state = null;
   %sm.owner = %obj;
   return %sm;
}

// And allow onEvent to be called on AIPlayers.
function AIPlayer::onEvent(%obj, %event) {
   if(isObject(%obj.sm)) {
      %obj.sm.onEvent(%event);
   }
}

// Events relevant to the monster's actions.
eventQueue(Monster);
event(Monster, Attack);
event(Monster, Swim);
event(Monster, Bubble);

// Events that come from tourists.
eventQueue(Tourist);
event(Tourist, Scared);

function Tourist::onAdd(%this, %obj) {
   // Subscribe to messages.
   subscribe(%obj, Monster, Attack);
   subscribe(%obj, Monster, Swim);
   subscribe(%obj, Monster, Bubble);
   subscribe(%obj, Tourist, Scared);

   // Make a state machine!
   %obj.sm = makeSM(Tourist, %obj);
   %obj.onEvent(relax);

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

new ScriptObject(TouristSMTemplate) {
   transition[null, relax] = relaxed;
   transition[relaxed, newSpot] = walking;
   transition[relaxed, swimNoise] = scared;
   transition[relaxed, touristScared] = inquiring;
   transition[walking, swimNoise] = scared;
   transition[scared, swimNoise] = fleeing;
   transition[_, attackNear] = fleeing;
   transition[_, attackFar] = scared;
};

function TouristSM::enterRelaxed(%this) {
   %obj = %this.owner;
   %obj.setShapeName(" Relaxed ");
}

function TouristSM::enterScared(%this) {
   %obj = %this.owner;
   %obj.becomeScared = schedule(getRandom(0, 500), %this, _Tourist_becomeScared, %obj);
}

function TouristSM::leaveScared(%this) {
   %obj = %this.owner;
   if(%obj.becomeScared !$= "") {
      cancel(%obj.becomeScared);
   }
}

function _Tourist_becomeScared(%obj) {
   %obj.setShapeName(" Scared ");
   %obj.setAimObject(TheMonster);
   %obj.becomeScared = "";
   postEvent(Tourist, Scared, %obj);
}

function TouristSM::enterInquiring(%this) {
   %obj = %this.owner;
   %obj.setShapeName(" R U OK? ");
   %obj.setAimObject(%obj.inquring);
}

function TouristSM::enterFleeing(%this) {
   %obj = %this.owner;
   %obj.setShapeName(" Fleeing ");
}

function Tourist::onMonsterAttack(%this, %obj, %pos) {
   //error(%obj SPC %pos);
}

function Tourist::onTouristScared(%this, %obj, %scared) {
   if(%scared == %obj) return;
   %d = VectorLen(VectorSub(%obj.getPosition(), %scared.getPosition()));
   if(%d < 20) {
      %obj.inquiring = %scared;
      %obj.onEvent(touristScared);
   }
}

function Tourist::onMonsterSwim(%this, %obj, %pos) {
   %p1 = getWords(%obj.getPosition(), 0, 1) SPC 0;
   %p2 = getWords(TheMonster.getPosition(), 0, 1) SPC 0;
   %d = VectorLen(VectorSub(%p1, %p2));
   if(%d < 10) {
      if(%obj.swimCount $= "") {
         %obj.swimCount = 0;
      }
      %obj.swimCount++;
      if(%obj.swimCount > 4) {
         %obj.onEvent(swimNoise);
         %obj.swimCount = 0;
      }
   }
}

function Tourist::onMonsterBubble(%this, %obj, %pos) {
   //error(%obj SPC %pos);
}

function Monster::onAdd(%this, %obj) {
   $Monster::swimNoiseMs = 500;
   $Monster::swimNoiseSpeed = 0;
   schedule($Monster::swimNoiseMs, %this, makeNoise, %this, %obj);
}

function makeNoise(%this, %obj) {
   schedule($Monster::swimNoiseMs, %this, makeNoise, %this, %obj);
   if(VectorLen(%obj.getVelocity()) > $Monster::swimNoiseSpeed) {
      postEvent(Monster, Swim, %obj.getPosition());
   }
}
