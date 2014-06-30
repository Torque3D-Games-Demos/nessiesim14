exec("./playGui.gui");
exec("./materials.cs");
exec("./datablocks.cs");

//-----------------------------------------------------------------------------
// Called when all datablocks have been transmitted.
function GameConnection::onEnterGame(%this) {
   new Camera(TheCamera) {
      datablock = Observer;
      camVel = "0 0 0";
      camForce = "0 0 0";
   };
   TheCamera.setTransform("0 0 25 1 0 0 0");
   TheCamera.scopeToClient(%this);
   GameGroup.add(TheCamera);

   new Player(TheMonster) {
      datablock = MonsterData;
      position = "0 20 0";
   };
   GameGroup.add(TheMonster);

   %this.setControlObject(TheMonster);
   %this.setCameraObject(TheCamera);
   TheCamera.setTrackObject(TheMonster);
   %this.setFirstPerson(false);

   PlayGui.noCursor = true;
   Canvas.setContent(PlayGui);
   activateDirectInput();

   setupControls();
}

//-----------------------------------------------------------------------------
function setupControls() {
   new ActionMap(MoveMap);
   MoveMap.bind("keyboard", "w", forwards);
   MoveMap.bind("keyboard", "s", backwards);
   MoveMap.bind("keyboard", "a", left);
   MoveMap.bind("keyboard", "d", right);
   MoveMap.push();

   schedule(10, 0, movementSchedule);
}

function movementSchedule() {
   schedule(10, 0, movementSchedule);
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
   %position = VectorAdd(TheCamera.getPosition(), VectorScale(TheCamera.camVel, 0.01));
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
   new SimGroup(GameGroup) {
      new LevelInfo(TheLevelInfo) {
         canvasClearColor = "Black";
         fogColor = "Black";
         visibleDistance = 1000;
         fogDensity = 0.01;
         fogDensityOffset = 10;
      };
      new GroundPlane(TheGround) {
         material = GroundMaterial;
      };
      new WaterPlane(TheSwamp) {
         position = "0 0 3.1";
         viscosity = 0;
         rippleTex = "art/ripple.dds";
         specularColor = "Gray";
         specularPower = 250;
         rippleDir[0] = "0 1";
         rippleSpeed[0] = 0.005;
         rippleTexScale[0] = "40 40";
         rippleDir[1] = "1 1";
         rippleSpeed[1] = 0.006;
         rippleTexScale[1] = "40 40";
         overallRippleMagnitude = 1;
         clarity = 0;
         reflectivity = 0;
      };
      new Sun(TheSun) {
         azimuth = 230;
         elevation = 45;
         color = "0.3 0.3 0.3";
         ambient = "0 0.1 0";
         castShadows = true;
      };
      new TSStatic(Level) {
         shapeName = "art/level.dae";
         position = "0 0 3";
         collisionType = "Visible Mesh";
      };
      new TSStatic(LevelDetails) {
         shapeName = "art/level_details.dae";
         position = "0 0 3";
         collisionType = "None";
      };
   };
}

//-----------------------------------------------------------------------------
function onEnd() {
   MoveMap.delete();
   GameGroup.delete();
}
