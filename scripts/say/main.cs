function AIPlayer::say(%obj, %phrase) {
   %this = %obj.getDataBlock();
   if(%this.isMethod(say)) {
      %this.say(%obj, %phrase);
   }
}

function PlayerData::say(%this, %obj, %phrase) {
   if(!$say::stuff) return;
   cancel(%obj._unsayS);
   %obj._unsayS = %obj.schedule(3000, _unsay);
   %obj.setShapeName("" SPC %phrase SPC "");
}

function AIPlayer::_unsay(%obj) {
   %obj.setShapeName("");
   %obj._unsayS = "";
}
