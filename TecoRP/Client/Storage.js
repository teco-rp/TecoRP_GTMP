
var storage_menu = API.createMenu("Depolama", "Depolanmış Eşyalarınız", 0, 0, 4);
storage_menu.Visible = false;
storage_menu.ResetKey(menuControl.Back);

var requestType = null;
var ID = null;

API.onServerEventTrigger.connect(function (name, args) {

    if (name == "storage_open" && storage_menu.Visible == false) {
        requestType = args[4];
        ID = args[5];
        API.setMenuTitle(storage_menu, args[3]);

        for (var i = 0; i < args[0]; i++) {
            storage_menu.AddItem(API.createMenuItem(args[1][i], args[2][i]));
        }
        
        storage_menu.Visible = true;
        API.showCursor(true);
    }
});

storage_menu.OnItemSelect.connect(function (sender, item, index) {
    API.triggerServerEvent("storage_slot_selected", requestType, ID, index);
    storage_menu.Visible = false;
    API.showCursor(false);
    storage_menu.Clear();
});

API.onKeyDown.connect(function (Player, args) {
    if (args.KeyCode == Keys.Escape && storage_menu.Visible == true) {
        storage_menu.Visible = false;
        API.showCursor(false);
        storage_menu.Clear();
    }
});
