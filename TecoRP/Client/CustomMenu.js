//-------------CUSTOM-RETURN-MENU-------------
var custom_menu = API.createMenu("MENU", "AÇIKLAMA", 100, 100, 6);
custom_menu.Visible = false;
custom_menu.ResetKey(menuControl.Back);

var IDList;
var retunName = "";
var customArg = null;
API.onServerEventTrigger.connect(function (name, args) {
    if (name == "open_custom_menu") {
        API.sendChatMessage(args[3]+","+args[4]+","+args[5]+","+args[6]+","+args[7]);
        IDList = args[3];
        customArg = args[5];
        retunName = args[7];
        API.setMenuTitle(custom_menu, args[4]);
        API.setMenuSubtitle(custom_menu, args[6]);
        for (var i = 0; i < args[0]; i++) {

            API.sendChatMessage("sector for "+i);
            custom_menu.AddItem(API.createMenuItem(args[1][i], args[2].length == undefined ? "" : args[2][i]));
        }
        API.showCursor(true);
        custom_menu.Visible = true;
    }
});
custom_menu.OnItemSelect.connect(function (sender, item, index) {
    API.triggerServerEvent(retunName, IDList[index], customArg);
    API.sendChatMessage(retunName + "," + IDList[index]);
    custom_menu.Visible = false;
    custom_menu.Clear();
    API.showCursor(false);


});

//TODO: [Deprecated] Check after test
//API.onUpdate.connect(function () {
//    API.drawMenu(custom_menu);
//});

API.onKeyDown.connect(function (Player, args) {
    if (custom_menu.Visible == true && args.KeyCode == Keys.Escape) {
        custom_menu.Visible = false;
        API.showCursor(false);
        custom_menu.Clear();
    }
});