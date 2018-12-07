//--------------Building------------

var building_menu = API.createMenu("Bina", "Katlar  | (~y~Z~s~) Zil | (~y~K~s~) Kilit", 0, 0, 6);
building_menu.Visible = false;
building_menu.ResetKey(menuControl.Back);



var BuildingId = null;
API.onServerEventTrigger.connect(function (name, args) {

    if (name == "building_open" && building_menu.Visible == false) {

        API.setMenuTitle(building_menu, args[4]);
        BuildingId = args[3];
        for (var i = 0; i < args[0]; i++) {
            building_menu.AddItem(API.createMenuItem(args[1][i], args[2][i]));
        }
        API.showCursor(true);
        building_menu.Visible = true;
    }
});

building_menu.OnItemSelect.connect(function (sender, item, index) {
    API.triggerServerEvent("return_building", BuildingId, index);
    building_menu.Visible = false;
    API.showCursor(false);
    building_menu.Clear();
});

API.onKeyDown.connect(function (Player, args) {
    if (args.KeyCode == Keys.Escape && building_menu.Visible == true) {
        building_menu.Visible = false;
        API.showCursor(false);
        building_menu.Clear();
    }
    if (args.KeyCode == Keys.B && building_menu.Visible == true) {
        API.triggerServerEvent("return_building_buy", BuildingId, building_menu.CurrentSelection);
        building_menu.Visible = false;
        building_menu.Clear();
        API.showCursor(false);
    }
    if (args.KeyCode == Keys.X && building_menu.Visible == true) {
        API.sendChatMessage("Evi satılığa çıkarmak istediğiniz fiyatı giriniz.");
        API.triggerServerEvent("return_building_sell", BuildingId, building_menu.CurrentSelection, API.getUserInput("$", 9));
        building_menu.Visible = false;
        building_menu.Clear();
        API.showCursor(false);
    }
    if (args.KeyCode == Keys.K && building_menu.Visible == true) {
        API.triggerServerEvent("return_building_lock", BuildingId, building_menu.CurrentSelection);
        building_menu.Visible = false;
        building_menu.Clear();
        API.showCursor(false);
    }
    if (args.KeyCode == Keys.Z && building_menu.Visible == true) {
        API.triggerServerEvent("return_building_ring", BuildingId, building_menu.CurrentSelection);
    }
});
