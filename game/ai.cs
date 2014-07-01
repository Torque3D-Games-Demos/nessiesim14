function Tourist::onAdd(%this, %obj) {
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
