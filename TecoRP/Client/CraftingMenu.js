//--------------CraftingMenu------------
var crafting_menu = API.createMenu("URETIM", "Üretebileceğiniz eşyalar:", 100, 100, 6);
crafting_menu.Visible = false;
crafting_menu.ResetKey(menuControl.Back);
var craftingTableModel = null; API.onServerEventTrigger.connect(function (name, args) {
    if (name == "crafting_open") {
        craftingTableModel = args[3];
        for (var i = 0; i < args[0]; i++)
        {
            crafting_menu.AddItem(API.createMenuItem(args[1][i], args[2][i]));
        }
        API.showCursor(true); crafting_menu.Visible = true;
    }
}); crafting_menu.OnItemSelect.connect(function (sender, item, index) {
    API.triggerServerEvent("return_crafting", craftingTableModel, index)
});
API.onKeyDown.connect(function (Player, args) {
    if (args.KeyCode == Keys.Escape && crafting_menu.Visible == true) {
        crafting_menu.Visible = false; API.showCursor(false); crafting_menu.Clear();
    }
    if (args.KeyCode == Keys.U && crafting_menu.Visible == false && !API.isPlayerInAnyVehicle(API.getLocalPlayer())) {
        API.triggerServerEvent("crafting_open_key");
    }
});
