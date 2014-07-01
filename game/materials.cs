singleton Material(BlankWhite) {
   diffuseColor[0] = "White";
};

singleton Material(Black) {
   diffuseColor[0] = "Black";
};

singleton Material(Invisible) {
   mapTo = "Invisible";
   diffuseColor[0] = "0 0 0 0";
   castShadows = false;
   translucent = true;
};

singleton Material(GroundMaterial) {
   mapTo = "GroundMaterial";
   diffuseColor[0] = "LightGreen";
};

singleton Material(WoodMaterial) {
   mapTo = "WoodMaterial";
   diffuseColor[0] = "BurlyWood";
};

singleton Material(LeafMaterial) {
   mapTo = "LeafMaterial";
   diffuseColor[0] = "Khaki";
};

singleton Material(MonsterMaterial : Invisible) {
   mapTo = "MonsterMaterial";
   //diffuseColor[0] = "0 0 0 1";
};

singleton Material(TouristMaterial) {
   mapTo = "TouristMaterial";
   diffuseColor[0] = "LightSalmon";
};

singleton Material(WaterMaterial : Invisible) {
   mapTo = "WaterMaterial";
   //diffuseColor[0] = "0.3 0.3 0.3";
};

singleton Material(ParasolRed) {
   mapTo = "ParasolRed";
   diffuseColor[0] = "Red";
   doubleSided = true;
};

singleton Material(ParasolWhite) {
   mapTo = "ParasolWhite";
   diffuseColor[0] = "White";
   doubleSided = true;
};

singleton Material(ParasolStem) {
   mapTo = "ParasolStem";
   diffuseColor[0] = "Gray";
};
