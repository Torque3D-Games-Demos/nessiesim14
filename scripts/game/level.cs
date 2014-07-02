new SimGroup(GameGroup) {
   new LevelInfo(TheLevelInfo) {
      canvasClearColor = "Black";
      fogColor = "Black";
      visibleDistance = 1000;
      fogDensity = 0.01;
      fogDensityOffset = 10;
   };
   new GroundPlane(TheGround) {
      material = Black;
   };
   new WaterPlane(TheSwamp) {
      position = "0 0 3";
      rippleTex = "art/ripple.dds";
      specularColor = "Gray";
      specularPower = 250;
      rippleDir[0] = "0 1";
      rippleSpeed[0] = 0.005;
      rippleTexScale[0] = "40 40";
      rippleDir[1] = "1 1";
      rippleSpeed[1] = 0.006;
      rippleTexScale[1] = "40 40";
      waterFogDensity = 1;
      overallRippleMagnitude = 1;
      clarity = 0;
      reflectivity = 0;
   };
   new Sun(TheSun) {
      azimuth = 230;
      elevation = 45;
      color = "0.5 0.5 0.5";
      ambient = "0.1 0.2 0.1";
      castShadows = true;
      attenuationRatio = "0 1 1";
      shadowType = "PSSM";
      texSize = "1024";
      overDarkFactor = "4000 3000 2000 1000";
      shadowDistance = "300";
      shadowSoftness = "0.15";
      numSplits = "4";
      logWeight = "0.91";
      fadeStartDistance = "280";
      lastSplitTerrainOnly = "0";
   };
   new TSStatic(Level) {
      shapeName = "art/level.dae";
      position = "0 0 3";
      collisionType = "Visible Mesh";
      allowPlayerStep = true;
   };
   new TSStatic(LevelDetails) {
      shapeName = "art/level_details.dae";
      position = "0 0 3";
      collisionType = "None";
   };
   new NavMesh(Nav) {
      cellSize = 1;
      cellHeight = 0.5;
      tileSize = 20;
      position = "0 0 8";
      scale = "29 29 5";
      fileName = "./navmesh";
      alwaysRender = $prefs::debug;
      actorClimb = 0.5;
   };
};
