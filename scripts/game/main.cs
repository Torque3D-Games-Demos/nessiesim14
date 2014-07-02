exec("scripts/metrics/main.cs");
exec("scripts/profiling/main.cs");
exec("scripts/console/main.cs");

exec("./playGui.gui");
exec("./materials.cs");
exec("./datablocks.cs");
exec("./ai.cs");

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
   TheCamera.setTransform("-98.0393 162.685 20 1 0 0 0");
   TheCamera.scopeToClient(%this);
   GameGroup.add(TheCamera);

   new Player(TheMonster) {
      datablock = Monster;
   };
   TheMonster.setTransform("-93.4728 141.151 0 1 0 0 0");
   GameGroup.add(TheMonster);

   %this.setControlObject(TheMonster);
   %this.setCameraObject(TheCamera);
   TheCamera.setTrackObject(TheMonster);
   %this.setFirstPerson(false);

   //PlayGui.noCursor = true;
   Canvas.setContent(PlayGui);
   activateDirectInput();

   setupControls();

   if($prefs::graphics $= High) {
      // Not working :(
      MLAAFx.enable();
   }
}

//-----------------------------------------------------------------------------
function setupControls() {
   new ActionMap(MoveMap);
   MoveMap.bind("keyboard", "w", forwards);
   MoveMap.bind("keyboard", "s", backwards);
   MoveMap.bind("keyboard", "a", left);
   MoveMap.bind("keyboard", "d", right);
   MoveMap.push();

   $MovementHz = 50;
   $MovementMillis = 1000 / $MovementHz;
   schedule($MovementMillis, 0, movementSchedule);
}

function movementSchedule() {
   schedule($MovementMillis, 0, movementSchedule);
   updateMovement();
   updateCamera();
}

function updateMovement() {
   %axes = getCameraAxes();
   %front = getField(%axes, 0);
   %right = getField(%axes, 1);
   %moveDir = VectorAdd(VectorScale(%right, $right - $left),
                        VectorScale(%front, $forward - $backward));
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
function onStart() {
   exec("./level.cs");

   %numTourists = 10;
   for(%i = 0; %i < %numTourists; %i++) {
      %spot = chooseTouristSpot();
      GameGroup.add(new AIPlayer() {
         datablock = Tourist;
         spot = getWord(%spot, 0);
         position = getWords(%spot, 1, 3);
      });
   }
}

//-----------------------------------------------------------------------------
function onEnd() {
   MoveMap.delete();
   GameGroup.delete();
}
