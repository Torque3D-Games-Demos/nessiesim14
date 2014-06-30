singleton Material(BlankWhite) {
   diffuseColor[0] = "White";
};

singleton Material(GroundMaterial) {
   mapTo = "GroundMaterial";
   diffuseColor[0] = "White";
};

singleton Material(WoodMaterial) {
   mapTo = "WoodMaterial";
   diffuseColor[0] = "White";
};

singleton Material(LeafMaterial) {
   mapTo = "LeafMaterial";
   diffuseColor[0] = "White";
};

singleton Material(MonsterMaterial) {
   mapTo = "MonsterMaterial";
   diffuseColor[0] = "0 0 0 0";
   castShadows = false;
   translucent = true;
};
