function eventQueue(%namespace) {
   %manager = %namespace @ Events;
   %m = new EventManager(%manager) {
      queue = %manager;
   };
}

function event(%namespace, %action, %classes) {
   %classes = %classes $= "" ? AIPlayer : %classes;
   %manager = %namespace @ Events;
   %event = %namespace @ %action;
   %method = on @ %event;
   eval(%manager @ ".registerEvent(" @ %event @ ");");
   foreach$(%class in %classes) {
      eval(
"function " @ %class @ "::" @ %method @ "(%this, %data) {" @
   "if(%this.getDataBlock().isMethod(" @ %method @ ")) {" @
      "%this.getDataBlock()." @ %method @ "(%this, %data);" @
   "}" @
"}"
      );
   }
}

function subscribe(%obj, %namespace, %action) {
   %manager = %namespace @ Events;
   %event = %namespace @ %action;
   %manager.subscribe(%obj, %event);
}

function postEvent(%namespace, %action, %data) {
   %manager = %namespace @ Events;
   %event = %namespace @ %action;
   %manager.postEvent(%event, %data);
}
