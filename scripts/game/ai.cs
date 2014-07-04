exec("scripts/navigation/main.cs");
exec("scripts/stateMachine/main.cs");
exec("scripts/events/main.cs");
exec("./touristAI.cs");
exec("./rangerAI.cs");

function delete(%obj) {
   %obj.delete();
}

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
   %obj._timeout = schedule(%time, %obj, AIPlayer__timeOut, %obj);
}

function AIPlayer__timeOut(%obj) {
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
event(Monster, Swim,   "Player AIPlayer");
event(Monster, Bubble, "Player AIPlayer");
event(Monster, Attack, "Player AIPlayer");

// Events that come from tourists.
eventQueue(Tourist);
event(Tourist, Scared);
event(Tourist, Eaten);
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

$Monster::swimNoiseMs = 500;
$Monster::swimNoiseSpeed = 3;
function Monster__makeNoise(%this, %obj) {
   schedule($Monster::swimNoiseMs, %this, Monster__makeNoise, %this, %obj);
   if(VectorLen(%obj.getVelocity()) > $Monster::swimNoiseSpeed) {
      postEvent(Monster, Swim, %obj.getPosition());
   }
}

function Monster::onAdd(%this, %obj) {
   subscribe(%obj, Monster, Attack);
   subscribe(%obj, Monster, Bubble);
   subscribe(%obj, Monster, Swim);
   schedule($Monster::swimNoiseMs, %this, Monster__makeNoise, %this, %obj);
}

function Monster::onMonsterAttack(%this, %obj, %pos) {
   %p = new ParticleEmitterNode() {
      datablock = DefaultNode;
      emitter = AttackRippleEmitter;
      active = true;
      position = getWords(%obj.getPosition(), 0, 1) SPC 3.2;
   };
   %p.schedule(1000, delete, %p);
   GameGroup.add(%p);

   %p = new ParticleEmitterNode() {
      datablock = DefaultNode;
      emitter = AttackSplashEmitter;
      active = true;
      position = getWords(%obj.getPosition(), 0, 1) SPC 2;
   };
   %p.schedule(500, delete, %p);
   GameGroup.add(%p);

   %p = new ParticleEmitterNode() {
      datablock = DefaultNode;
      emitter = AttackJetEmitter;
      active = true;
      position = getWords(%obj.getPosition(), 0, 1) SPC 2.5;
   };
   %p.schedule(300, delete, %p);
   GameGroup.add(%p);
}

function Monster::onMonsterBubble(%this, %obj, %pos) {
   %p = new ParticleEmitterNode() {
      datablock = DefaultNode;
      emitter = BubbleRippleEmitter;
      active = true;
      position = getWords(%obj.getPosition(), 0, 1) SPC 3.2;
   };
   %p.schedule(1000, delete, %p);
   GameGroup.add(%p);

   %p = new ParticleEmitterNode() {
      datablock = DefaultNode;
      emitter = BubbleEmitter;
      active = true;
      position = getWords(%obj.getPosition(), 0, 1) SPC 2.5;
   };
   %p.schedule(700, delete, %p);
   GameGroup.add(%p);
}

function Monster::onMonsterSwim(%this, %obj, %pos) {
   %p = new ParticleEmitterNode() {
      datablock = DefaultNode;
      emitter = WakeEmitter;
      active = true;
      position = getWords(%obj.getPosition(), 0, 1) SPC 3.2;
   };
   %p.schedule(3000, delete, %p);
   GameGroup.add(%p);
}
