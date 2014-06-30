datablock CameraData(Observer) {};

datablock PlayerData(MonsterData) {
   shapeFile = "art/monster.dae";
   mass = 90;
   runForce = 0;
   swimForce = 0;
   maxUnderwaterForwardSpeed = 15;
   maxUnderwaterBackwardSpeed = 15;
   maxUnderwaterSideSpeed = 15;
   groundImpactMinSpeed = 100;
};
