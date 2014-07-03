$rangerSpots = 
   "-96 45 5";
$numRangerSpots = getRecordCount($rangerSpots);

function chooseRangerSpot(%i) {
   %idx = %i $= "" ? getRandom(0, $numRangerSpots-1) : %i;
   %pos = getRecord($rangerSpots, %idx);
   %pos = chooseGroundPos(%pos, 10);
   return %idx SPC %pos;
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

function Ranger::onAdd(%this, %obj) {
   Rangers.add(%obj);

   subscribe(%obj, Monster, Attack);
   subscribe(%obj, Monster, Swim);
   subscribe(%obj, Monster, Bubble);
   subscribe(%obj, Tourist, Scared);
   subscribe(%obj, Tourist, AskHelp);
}

function Ranger::onTouristAskHelp(%this, %obj, %tourist) {
   if(%tourist.helpLocation !$= "") {
      %obj.setMoveSpeed(0.5);
      %obj.setPathDestination(%tourist.helpLocation);
   }
}
