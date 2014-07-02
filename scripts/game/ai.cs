exec("scripts/stateMachine/main.cs");
// Convenience function!
function makeSM(%type, %obj) {
   %parent = %type @ SM;
   eval("%sm = new ScriptObject(\"\" : " @ %parent @ Template @ ");");
   %sm.superclass = StateMachine;
   %sm.class = %parent;
   %sm.state = null;
   %sm.owner = %obj;
   return %sm;
}

exec("scripts/events/main.cs");
// Events relevant to the monster's actions.
eventQueue(Monster);
event(Monster, Attack);
event(Monster, Swim);
event(Monster, Bubble);

new ScriptObject(TouristSMTemplate) {
   //class = StateMachine;
   transition[null, ready] = ready;
};

function Tourist::onAdd(%this, %obj) {
   // Subscribe to monster messages.
   subscribe(%obj, Monster, Attack);
   subscribe(%obj, Monster, Swim);
   subscribe(%obj, Monster, Bubble);

   // Make a state machine!
   %obj.sm = makeSM(Tourist, %obj);
   %obj.sm.onEvent(ready);

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

function TouristSM::enterReady(%this) {
   %obj = %this.owner;
}

function Tourist::onMonsterAttack(%this, %obj, %pos) {
   //error(%obj SPC %pos);
}

function Tourist::onMonsterSwim(%this, %obj, %pos) {
   %p1 = getWords(%obj.getPosition(), 0, 1) SPC 0;
   %p2 = getWords(%pos, 0, 1) SPC 0;
   %d = VectorLen(VectorSub(%p1, %p2));
   if(%d < 10) {
      %obj.setAimObject(TheMonster);
   }
}

function Tourist::onMonsterBubble(%this, %obj, %pos) {
   //error(%obj SPC %pos);
}

function Monster::onAdd(%this, %obj) {
   $Monster::swimNoiseMs = 500;
   schedule($Monster::swimNoiseMs, %this, makeNoise, %this, %obj);
}

function makeNoise(%this, %obj) {
   schedule($Monster::swimNoiseMs, %this, makeNoise, %this, %obj);
   if(VectorLen(%obj.getVelocity()) > 4) {
      postEvent(Monster, Swim, %obj.getPosition());
   }
}
