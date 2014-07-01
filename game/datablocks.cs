datablock CameraData(Observer) {};

datablock PlayerData(Monster) {
   shapeFile = "art/monster.dae";
   mass = 90;
   runSurfaceAngle = 85;
   swimForce = 90 * 5;
   maxUnderwaterForwardSpeed = 25;
   maxUnderwaterBackwardSpeed = 25;
   maxUnderwaterSideSpeed = 25;
   groundImpactMinSpeed = 100;
};

datablock PlayerData(Tourist : Monster) {
   shapeFile = "art/tourist.dae";
   mass = 90;
   runSurfaceAngle = 85;
   swimForce = 90 * 5;
   runForce = 90 * 10;
   maxForwardSpeed = 10;
   maxBackwardSpeed = 10;
   maxSideSpeed = 10;
};

datablock PlayerData(Ranger : Monster) {
   runForce = 90 * 10;
   maxForwardSpeed = 10;
   maxBackwardSpeed = 10;
   maxSideSpeed = 10;
};
