var player = API.getLocalPlayer();
var isInVehicle = API.isPlayerInAnyVehicle(player);

var CTRL = false;


API.onKeyDown.connect(function (Player, args) {
    //API.sendChatMessage("key: " + args.number);
    if (args.KeyCode == Keys.ControlKey) {
        CTRL = true;
    }

    if (args.KeyCode == Keys.Y && !API.isChatOpen() && !isInVehicle) {
        API.triggerServerEvent("key_Y");
    }
    if (args.KeyCode == Keys.L && !API.isChatOpen()) {
        API.triggerServerEvent("key_L");
    }
    if (args.KeyCode == Keys.F && !API.isChatOpen() && !isInVehicle) {
        API.triggerServerEvent("key_F");
    }
    if (args.KeyCode == Keys.I && !API.isChatOpen() && !API.isCursorShown()) {
        API.triggerServerEvent("key_I");
    }
    if (args.KeyCode == Keys.E && !API.isChatOpen() && !isInVehicle && !API.isCursorShown()) {
        API.triggerServerEvent("Key_E");
    }
    //if (args.KeyCode == Keys.E && !API.isChatOpen() && !isInVehicle && !API.isCursorShown()) {
    //    API.triggerServerEvent("key_B");
    //}

    if (CTRL && args.KeyCode == Keys.D1) {
        API.triggerServerEvent("CTRL_1");
    }
    if (CTRL && args.KeyCode == Keys.D2) {
        API.triggerServerEvent("CTRL_2");
    }
    if (CTRL && args.KeyCode == Keys.D3) {
        API.triggerServerEvent("CTRL_3");
    }
    if (CTRL && args.KeyCode == Keys.D4) {
        API.triggerServerEvent("CTRL_4");
    }
    if (CTRL && args.KeyCode == Keys.D5) {
        API.triggerServerEvent("CTRL_5");
    }
});


API.onKeyUp.connect(function (Player, args) {

    if (args.KeyCode == Keys.ControlKey) {
        CTRL = false;
    }
});

