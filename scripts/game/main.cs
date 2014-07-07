exec("scripts/console/main.cs");
exec("scripts/metrics/main.cs");
exec("scripts/profiling/main.cs");

exec("./materials.cs");
exec("./datablocks.cs");
exec("./ai.cs");

exec("./playGui.gui");
exec("./helpGui.gui");
exec("./endGameGui.gui");

$pref::PSSM::smallestVisiblePixelSize = 10;
$pref::Shadows::filterMode = "SoftShadowHighQuality";

//-----------------------------------------------------------------------------
// Called when all datablocks have been transmitted.
function GameConnection::onEnterGame(%this) {
   new Camera(TheCamera) {
      datablock = Observer;
      camVel = "0 0 0";
      camForce = "0 0 0";
   };
   TheCamera.setTransform("-98.0393 162.685 15 1 0 0 0");
   TheCamera.scopeToClient(%this);
   GameGroup.add(TheCamera);

   new Player(TheMonster) {
      datablock = Monster;
   };
   TheMonster.setTransform("-93.4728 141.151 0 1 0 0 0");
   GameGroup.add(TheMonster);

   %this.setControlObject(TheMonster);
   %this.setCameraObject(TheCamera);
   TheCamera.setTrackObject(TheMonster, "0 0 3");
   %this.setFirstPerson(false);

   //PlayGui.noCursor = true;
   Canvas.setContent(PlayGui);
   $enableDirectInput = true;
   activateDirectInput();

   setupControls();

   if($prefs::graphics $= High) {
      // Not working :(
      SSAOPostFx.enable();
   }
}

//-----------------------------------------------------------------------------
function setupControls() {
   new ActionMap(MoveMap);
   MoveMap.push();

   MoveMap.bind("keyboard", "w", forwards);
   MoveMap.bind("keyboard", "s", backwards);
   MoveMap.bind("keyboard", "a", left);
   MoveMap.bind("keyboard", "d", right);
   MoveMap.bind("keyboard", "space", attack);
   MoveMap.bind("keyboard", "lcontrol", bubble);
   MoveMap.bind("keyboard", "lshift", creep);

   MoveMap.bind("keyboard", "h", toggleHelp);
   MoveMap.bind("keyboard", "p", endGame);

   MoveMap.bind("gamepad", "btn_a", attack);
   MoveMap.bind("gamepad", "btn_b", bubble);
   MoveMap.bind("gamepad", "btn_x", creep);
   MoveMap.bind("gamepad", "thumblx", "D", "-0.23 0.23", right);
   MoveMap.bind("gamepad", "thumbly", "D", "-0.23 0.23", forwards);

   $MovementHz = 50;
   $MovementMillis = 1000 / $MovementHz;
   $moveSchedule = schedule($MovementMillis, 0, movementSchedule);
}

function movementSchedule() {
   $moveSchedule = schedule($MovementMillis, 0, movementSchedule);
   updateMovement();
   updateCamera();
}

function updateMovement() {
   %axes = getCameraAxes();
   %front = getField(%axes, 0);
   %right = getField(%axes, 1);
   %moveDir = VectorAdd(VectorScale(%right, $right - $left),
                        VectorScale(%front, $forward - $backward));
   %moveDir = VectorScale(%moveDir, $moveSpeed);
   $mvRightAction = getWord(%moveDir, 0);
   $mvForwardAction = getWord(%moveDir, 1);
}

function updateCamera() {
   %diff = getWords(VectorSub(TheMonster.getPosition(), TheCamera.getPosition()), 0, 1) SPC 0;
   %dist = VectorLen(%diff);
   %diff = VectorNormalize(%diff);
   if(%dist < 18) {
      TheCamera.camForce = VectorScale(%diff, -1 * (18 - %dist));
   } else if(%dist > 22) {
      TheCamera.camForce = VectorScale(%diff, %dist - 22);
   }
   TheCamera.camForce = VectorAdd(TheCamera.camForce, VectorScale(TheCamera.camVel, -1));
   %position = VectorAdd(TheCamera.getPosition(), VectorScale(TheCamera.camVel, 1 / $MovementHz));
   TheCamera.setTransform(%position SPC getWords(TheCamera.getTransform(), 3, 6));
   TheCamera.camVel = VectorAdd(TheCamera.camVel, VectorScale(TheCamera.camForce, 1/50));
}

function forwards(%val)  { $forward  = %val; updateMovement(); }
function backwards(%val) { $backward = %val; updateMovement(); }
function left(%val)      { $left     = %val; updateMovement(); }
function right(%val)     { $right    = %val; updateMovement(); }

$lastAttack = getSimTime();
$attackCooldown = 1000;
function attack(%val) {
   if(%val) {
      %now = getSimTime();
      if(%now - $lastAttack > $attackCooldown) {
         $lastAttack = %now;
         %pos2 = getWords(TheMonster.getPosition(), 0, 1);
         %start = %pos2 SPC 10;
         %end = %pos2 SPC 3;
         %ray = ContainerRayCast(%start, %end, $TypeMasks::StaticShapeObjectType);
         if(!isObject(getWord(%ray, 0))) {
            postEvent(Monster, Attack, TheMonster.getPosition());
         }
      }
   }
}

$lastBubble = getSimTime();
$bubbleCooldown = 500;
function bubble(%val) {
   if(%val) {
      %now = getSimTime();
      if(%now - $lastBubble > $bubbleCooldown) {
         $lastAttack = %now;
         postEvent(Monster, Bubble, TheMonster.getPosition());
      }
   }
}

$moveSpeed = 1;
function creep(%val) {
   $moveSpeed = %val ? 0.1 : 1;
}

function getCameraAxes() {
   %front = VectorSub(TheMonster.getPosition(), TheCamera.getPosition());
   // Let's assume that wasn't a vertical vector!
   %front = VectorNormalize(%front);
   %right = VectorCross(%front, "0 0 1");
   %front = VectorCross("0 0 1", %right);
   %front = VectorNormalize(%front);
   %right = VectorNormalize(%right);
   return %front TAB %right;
}

//-----------------------------------------------------------------------------
function toggleHelp(%val) {
   if(%val) {
      %help = PlayGui->Help;
      %help.visible = !%help.visible;
   }
}

//-----------------------------------------------------------------------------
function onStart() {
   exec("./level.cs");

   %numTourists = 30;
   for(%i = 0; %i < %numTourists; %i++) {
      %spot = chooseTouristSpot(%i);
      GameGroup.add(new AIPlayer() {
         datablock = Tourist;
         spot = getWord(%spot, 0);
         position = getWords(%spot, 1, 3);
      });
   }

   GameGroup.add(new SimSet(Rangers));
   for(%i = 0; %i < $numRangerSpots; %i++) {
      %pos = chooseGroundPos(getRecord($rangerSpots, %i));
      GameGroup.add(new AIPlayer() {
         datablock = Ranger;
         spot = %i;
         position = %pos;
      });
   }

   $say::stuff = true;
}

//-----------------------------------------------------------------------------
new ActionMap(EndMap);
EndMap.bind(keyboard, enter, resetGame);

function endGame(%val) {
   if(%val) {
      cancel($moveSchedule);
      GameGroup.delete();
      Canvas.setContent(EndGameGui);
      MoveMap.pop();
      MoveMap.delete();
      EndMap.push();
   }
}

function resetGame(%val) {
   if(%val) {
      EndMap.pop();
      commandToServer('reset');
   }
}

function serverCmdReset(%client) {
   $say::stuff = false;
   onStart();
   %client.onEnterGame();
}

function onEnd() {
   MoveMap.delete();
   EndMap.delete();
   GameGroup.delete();
}
