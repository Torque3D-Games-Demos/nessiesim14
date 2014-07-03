exec("scripts/navigation/main.cs");
exec("scripts/stateMachine/main.cs");
exec("scripts/events/main.cs");
exec("./touristAI.cs");
exec("./rangerAI.cs");

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

function AIPlayer::increaseDetection(%obj, %amount) {
   if(%obj._detection $= "") {
      %obj._detection = 0;
   }
   %obj._detection++;
   %obj.getDataBlock().onDetectionChange(%obj, %obj._detection);
}

function AIPlayer::resetDetection(%obj) {
   %obj._detection = 0;
}

function AIPlayer::timeOut(%obj, %time) {
   error(timeout SPC %obj SPC %time);
   %obj._timeout = schedule(%time, %obj, AIPlayer__timeOut, %obj);
}

function AIPlayer__timeOut(%obj) {
   error("timeout!!" SPC %obj);
   %obj._timeout = "";
   %obj.onEvent(timeOut);
}

function AIPlayer::stopTimeOut(%obj) {
   if(%obj._timeout !$= "") {
      cancel(%obj._timeout);
   }
   %obj._timeout = "";
}

// Events relevant to the monster's actions.
eventQueue(Monster);
event(Monster, Attack);
event(Monster, Swim);
event(Monster, Bubble);

// Events that come from tourists.
eventQueue(Tourist);
event(Tourist, Scared);
event(Tourist, AskHelp);

function chooseGroundPos(%airPos, %radius) {
   if(%radius $= "") {
      %radius = 0;
   }
   %count = 0;
   while(%count < 10) {
      %count++;
      if(%radius > 0) {
         %angle = getRandom() * 2 * 3.14159;
         %diff = mSin(%angle) * %radius SPC mCos(%angle) * %radius SPC 0;
         %newPos = VectorAdd(%airPos, %diff);
      } else {
         %newPos = %airPos;
      }
      %start = getWords(%newPos, 0, 1) SPC getWord(%newPos, 2) + 5;
      %end = getWords(%newPos, 0, 1) SPC 0;
      %ray = ContainerRayCast(%start, %end, $TypeMasks::StaticObjectType);
      %obj = getWord(%ray, 0);
      if(isObject(%obj)) {
         return VectorAdd(getWords(%ray, 1, 3), "0 0 0.1");
      }
   }
   return %airPos;
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
