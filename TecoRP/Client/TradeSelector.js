//--------------Trade------------

var player_selector_menu = API.createMenu("OYUNCULAR", "Etrafınızdaki oyunculardan birini seçin.", 100, 100, 6);
player_selector_menu.Visible = false;
player_selector_menu.ResetKey(menuControl.Back);

var sellType = null;
var sellingObjID = null;
var IDs = null;
var returnName = null;
API.onServerEventTrigger.connect(function (name, args) {

    if (name == "sell_player_select") {
        sellType = args[3];
        sellingObjID = args[4];
        IDs = args[2];
        returnName = args[5];
        for (var i = 0; i < args[0]; i++) {
            player_selector_menu.AddItem(API.createMenuItem(args[1][i], "İşlem yapmak istediğiniz oyuncuyu seçin."));
        }
        API.showCursor(true);
        player_selector_menu.Visible = true;
    }

});

player_selector_menu.OnItemSelect.connect(function (sender, item, index) {
    API.triggerServerEvent(returnName, sellType, sellingObjID, IDs[index], returnName== "return_sell_player_select" ?  API.getUserInput("$", 10) : " ");
});

API.onKeyDown.connect(function (Player, args) {
    if (args.KeyCode == Keys.Escape && player_selector_menu.Visible == true) {
        player_selector_menu.Visible = false;
        API.showCursor(false);
        player_selector_menu.Clear();
    }

});
API.onUpdate.connect(function () {
    API.drawMenu(player_selector_menu);
});
