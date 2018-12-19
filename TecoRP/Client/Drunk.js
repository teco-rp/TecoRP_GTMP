/// <reference path="../types-gt-mp/Declarations.d.ts" />

API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName == "set_drunk") {
        setDrunk();
    }
    else if (eventName == "set_undrunk") {
        setUndrunk();
    }
    
});

function setDrunk() {
    API.setPlayerIsDrunk(API.getLocalPlayer(), true);
}
function setUndrunk() {
    API.setPlayerIsDrunk(API.getLocalPlayer(), false);
}

