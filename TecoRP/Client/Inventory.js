//--------------INVENTORY------------
var inventory_menu = API.createMenu("Envanter", "Eşyalarınız", 0, 0, 4);
var player_selection_menu = API.createMenu("Eşya Sat", "Satmak istediğiniz oyuncuyu seçin", 0, 0, 4);
API.setMenuBannerRectangle(inventory_menu, 200, 255, 10, 30);
player_selection_menu.Visible = false;
inventory_menu.Visible = false;
player_selection_menu.ResetKey(menuControl.Back);
inventory_menu.ResetKey(menuControl.Back);

var selectionType = null;
var objectId = null;
var playerId = null;
var idList = null;
var itemIndex = null;
API.onServerEventTrigger.connect(function (name, args) {

    if (name == "inventory_open" && inventory_menu.Visible == false) {
        API.setMenuSubtitle(inventory_menu, args[3]);
        playerId = args[4];
        for (var i = 0; i < args[0]; i++) {
            inventory_menu.AddItem(API.createMenuItem(args[1][i], args[2][i]));
        }
        API.showCursor(true);
        inventory_menu.Visible = true;
    }
    if (name == "inventory_open_selection") {
        selectionType = args[4];
        objectId = args[5];
        API.setMenuSubtitle(inventory_menu, args[3]);
        for (var i = 0; i < args[0]; i++) {
            inventory_menu.AddItem(API.createMenuItem(args[1][i], args[2][i]));
        }
        API.showCursor(true);
        inventory_menu.Visible = true;
    }
    if (name == "sell_item_player_selection") {
        idList = args[2];
        itemIndex = args[3];
        for (var i = 0; i < args[0]; i++) {
            player_selection_menu.AddItem(API.createMenuItem(args[1][i], ""));
        }
        API.showCursor(true);
        player_selection_menu.Visible = true;

    }
});

inventory_menu.OnItemSelect.connect(function (sender, item, index) {

    if (selectionType == null) {
        API.triggerServerEvent("iventory_item_selected", index);
        inventory_menu.Visible = false;
        API.showCursor(false);
        inventory_menu.Clear();
    } else {
        API.triggerServerEvent("inventory_item_selection_selected", selectionType, objectId, index);
        inventory_menu.Visible = false;
        API.showCursor(false);
        inventory_menu.Clear();
        selectionType = null;
    }
});
player_selection_menu.OnItemSelect.connect(function (sender, item, index) {
    API.sendChatMessage("Satmak istediğiniz fiyatı giriniz.");
    API.triggerServerEvent("return_sell_item_player_selection", idList[index], itemIndex, API.getUserInput("$", 9));
    player_selection_menu.Visible = false;
    API.showCursor(false);
    player_selection_menu.Clear();
});
API.onKeyDown.connect(function (Player, args) {
    if (args.KeyCode == Keys.Escape && inventory_menu.Visible == true) {
        selectionType = null;
        objectId = null;
        inventory_menu.Visible = false;
        API.showCursor(false);
        inventory_menu.Clear();
    }
    //if (args.KeyCode == Keys.B && inventory_menu.Visible == true) {
    //    API.triggerServerEvent("key_B", inventory_menu.CurrentSelection)
    //    inventory_menu.Visible = false;
    //    API.showCursor(false);
    //    inventory_menu.Clear();
    //}
    if (args.KeyCode == Keys.B && inventory_menu.Visible == true) {
        API.triggerServerEvent("request_sell_item", inventory_menu.CurrentSelection)
        inventory_menu.Visible = false;
        API.showCursor(false);
        inventory_menu.Clear();
    }
    if (args.KeyCode == Keys.X && inventory_menu.Visible == true) {
        API.triggerServerEvent("key_X", inventory_menu.CurrentSelection.toString())
        inventory_menu.Visible = false;
        API.showCursor(false);
        inventory_menu.Clear();
    }
    if (args.KeyCode == Keys.N) {
        API.triggerServerEvent("key_N")
    }

});


API.onUpdate.connect(function () {
    API.drawMenu(inventory_menu);
    API.drawMenu(player_selection_menu);
});




//----------------GIVE---ITEM-------------------
var players_menu = API.createMenu("Envanter", "Eşyayı vermek istediğiniz kişiyi seçin |", 0, 0, 4);
API.setMenuBannerRectangle(players_menu, 200, 255, 10, 30);
players_menu.Visible = false;
players_menu.ResetKey(menuControl.Back);
API.onServerEventTrigger.connect(function (name, args) {

    if (name == "select_players_list" && players_menu.Visible == false) {
        API.setMenuSubtitle(players_menu, args[3]);
        for (var i = 0; i < args[0]; i++) {
            players_menu.AddItem(API.createMenuItem(args[1][i], args[2]));
        }
        API.showCursor(true);
        players_menu.Visible = true;
    }
});

players_menu.OnItemSelect.connect(function (sender, item, index) {
    API.triggerServerEvent("retun_players_list", item.Text, item.Description);
    players_menu.Visible = false
    API.showCursor(false);
    players_menu.Clear();
});

API.onKeyDown.connect(function (Player, args) {
    if (args.KeyCode == Keys.Escape && players_menu.Visible == true) {
        players_menu.Visible = false;
        API.showCursor(false);
        players_menu.Clear();
    }
});
API.onUpdate.connect(function () {
    API.drawMenu(players_menu);
});