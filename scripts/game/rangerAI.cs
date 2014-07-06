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

   transition[assisting, monsterNoise] = paused;
   transition[assisting, reachDestination] = alert;
   transition[assisting, attackFar] = paused;

   transition[paused, monsterNoise] = pursuing;
   transition[paused, timeOut] = assisting;

   // Attacks!
   transition[_, attackNear] = pursuing;
   transition[_, attackFar] = alert;
};

function RangerSM::enterRelaxed(%this) {
   %obj = %this.owner;
   %obj.setShapeName(" Relaxed ");
}

function RangerSM::enterAlert(%this) {
   %obj = %this.owner;
   %obj.setShapeName(" Alert ");
   %ang = getRandom() * 2 * 3.14159;
   %dir = VectorScale(mSin(%ang) SPC mSin(%ang) SPC 0, 1000);
   %look = VectorAdd(%obj.getPosition(), %dir);
   %obj.setAimLocation(%look);
   %obj.timeOut(getRandom(5000, 10000));
}

function RangerSM::leaveAlert(%this) {
   %this.owner.stopTimeOut();
}

function RangerSM::enterAssisting(%this) {
   %obj = %this.owner;
   %obj.threshold = 2;
   %obj.setShapeName(" Helping ");
   %obj.setMoveSpeed(0.5);
   while(!%obj.setPathDestination(chooseGroundPos(%obj.assisting.helpLocation, 3))) {}
}

function RangerSM::enterPaused(%this) {
   %obj = %this.owner;
   %obj.setShapeName(" Pausing ");
   %obj.timeOut(getRandom(3000, 7000));
}

function RangerSM::leavePaused(%this) {
   %this.owner.stopTimeOut();
}

function Ranger::onReachPathDestination(%this, %obj) {
   %obj.onEvent(reachDestination);
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

new SimSet(Rangers);
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
   "-96 45 5";
$numRangerSpots = getRecordCount($rangerSpots);

function chooseRangerSpot(%i) {
   %idx = %i $= "" ? getRandom(0, $numRangerSpots-1) : %i;
   %pos = getRecord($rangerSpots, %idx);
   %pos = chooseGroundPos(%pos, 10);
   return %idx SPC %pos;
}
