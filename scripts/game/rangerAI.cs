new ScriptObject(RangerSMTemplate) {
   transition[null, relax] = relaxed;

   // Relaxed state: loiter near an umbrella.
   transition[relaxed, newSpot] = walking;
   transition[relaxed, monsterNoise] = alert;
   transition[relaxed, touristScared] = alert;
   transition[relaxed, touristAskHelp] = assisting;

   // We'll relax eventually. Or attack.
   transition[alert, timeOut] = relaxed;
   transition[alert, monsterNoise] = pursuing;
   transition[alert, touristAskHelp] = assisting;

   // Help tourists by following them.
   transition[assisting, monsterNoise] = paused;
   transition[assisting, reachDestination] = alert;
   transition[assisting, attackFar] = paused;

   transitition[pursuing, timeOut] = alert;

   // Heard the monster while assisting.
   transition[paused, monsterNoise] = pursuing;
   transition[paused, timeOut] = assisting;

   // Attacks!
   transition[_, attackNear] = pursuing;
   transition[_, attackFar] = alert;
};

function RangerSM::enterRelaxed(%this) {
   %obj = %this.owner;
   %obj.say("Must have been rats.");
   %obj.setMoveSpeed(0.2);
}

function RangerSM::enterAlert(%this) {
   %obj = %this.owner;
   if(%obj.getShapeName() $= "") {
      %obj.say("Hmm?");
   }
   %ang = getRandom() * 2 * 3.14159;
   %dir = VectorScale(mSin(%ang) SPC mSin(%ang) SPC 0, 1000);
   %look = VectorAdd(%obj.getPosition(), %dir);
   %obj.setAimLocation(%look);
   %obj.timeOut(getRandom(5000, 10000));
}

function RangerSM::leaveAlert(%this) {
   %obj = %this.owner;
   %obj.say("All quiet now.");
   %obj.stopTimeOut();
   while(!%obj.setPathDestination(chooseRangerSpot(%obj.spot))) {}
}

function RangerSM::enterAssisting(%this) {
   %obj = %this.owner;
   %obj.threshold = 2;
   %obj.say("Monster? Where?");
   %obj.setMoveSpeed(0.5);
   while(!%obj.setPathDestination(chooseGroundPos(%obj.assisting.helpLocation, 3))) {}
}

function RangerSM::enterPaused(%this) {
   %obj = %this.owner;
   %obj.say("What was that?");
   %obj.timeOut(getRandom(3000, 7000));
   %obj.stop();
}

function RangerSM::leavePaused(%this) {
   %this.owner.stopTimeOut();
}

function RangerSM::enterPursuing(%this) {
   %obj = %this.owner;
   %obj.say("I have you now!");
   %obj.setAimObject(TheMonster);
   %obj.stop();
   %obj.timeOut(getRandom(5000, 10000));
}

function RangerSM::onAttackNear(%this) {
   if(strstr("alert pursuing paused assisting", %this.state) != -1) {
      %obj = %this.owner;
      if(!%obj.eaten) {
         %l = new SpotLight() {
            innerAngle = 45;
            outerAngle = 90;
            range = 50;
            color = "White";
            castShadows = false;
            brightness = 2;
         };
         %trans = %obj.getEyeTransform();
         %pos = VectorAdd(getWords(%trans, 0, 2), "0 0 2");
         %rot = getWords(%trans, 3, 6);
         %l.setTransform(%pos SPC %rot);
         GameGroup.add(%l);
         %l.schedule(100, delete);
         schedule(500, 0, endGame, true);
      }
   }
}

function Ranger::onTouristAskHelp(%this, %obj, %tourist) {
   %d = VectorLen(VectorSub(%tourist.getPosition(), %obj.getPosition()));
   if(%d < 10 && %tourist.helpLocation !$= "") {
      %obj.assisting = %tourist;
      %obj.onEvent(touristAskHelp);
   }
}

function Ranger::onTouristScared(%this, %obj, %scared) {
   %d = VectorLen(VectorSub(%obj.getPosition(), %scared.getPosition()));
   if(%d < 10) {
      %obj.assisting = %scared;
      %obj.onEvent(touristScared);
   }
}

function Ranger::onAdd(%this, %obj) {
   Rangers.add(%obj);

   subscribe(%obj, Monster, Attack);
   subscribe(%obj, Monster, Swim);
   subscribe(%obj, Monster, Bubble);
   subscribe(%obj, Tourist, Scared);
   subscribe(%obj, Tourist, AskHelp);

   // Make a state machine!
   %obj.sm = makeSM(Ranger, %obj);
   %obj.onEvent(relax);

   %obj.threshold = 3;
}

function findNearestRanger(%pos) {
   %best = -1;
   %bestDist = 10000;
   foreach(%r in Rangers) {
      // This should use path distance instead :/
      %d = VectorLen(VectorSub(%pos, %r.getPosition()));
      if(%d < %bestDist) {
         %bestDist = %d;
         %best = %r;
      }
   }
   return %best;
}

$rangerSpots =
   "176 188 5" NL
   "154 5.5 7.3" NL
   "-14 -227 5" NL
   "-212 -35 5" NL
   "130 -99 5" NL
   "-96 45 5";
$numRangerSpots = getRecordCount($rangerSpots);

function chooseRangerSpot(%idx, %radius) {
   return chooseGroundPos(getRecord($rangerSpots, %idx), %radius);
}
