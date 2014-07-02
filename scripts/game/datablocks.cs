datablock CameraData(Observer) {};

datablock PlayerData(Monster) {
   shapeFile = "art/monster.dae";
   mass = 90;
   swimForce = 90 * 5;
   maxUnderwaterForwardSpeed = 25;
   maxUnderwaterBackwardSpeed = 25;
   maxUnderwaterSideSpeed = 25;
   groundImpactMinSpeed = 100;
};

datablock PlayerData(Tourist) {
   shapeFile = "art/tourist.dae";
   mass = 90;
   runSurfaceAngle = 85;
   runForce = 90 * 12;
   maxForwardSpeed = 8;
   maxBackwardSpeed = 8;
   maxSideSpeed = 8;
   swimForce = 90 * 1;
   maxUnderwaterForwardSpeed = 3;
   maxUnderwaterBackwardSpeed = 3;
   maxUnderwaterSideSpeed = 3;
};

datablock PlayerData(Ranger : Tourist) {
};
