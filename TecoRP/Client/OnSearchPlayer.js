//-----------------SEARCH ON PLAYER--------------------
var inventory_menu = API.createMenu("Envanter", "Eşyalar", 0, 0, 3);
API.setMenuBannerRectangle(inventory_menu, 200, 255, 10, 30);
inventory_menu.Visible = false;
inventory_menu.ResetKey(menuControl.Back);
var inventoryOwner = null;
API.onServerEventTrigger.connect(function (name, args) {

    if (name == "inventory_search") {
        API.setMenuSubtitle(inventory_menu, args[3]);
        inventoryOwner = args[4];
        for (var i = 0; i < args[0]; i++) {
            inventory_menu.AddItem(API.createMenuItem(args[1][i], args[2][i]));
        }
        API.showCursor(true);
        inventory_menu.Visible = true;
    }
});

API.onKeyDown.connect(function (Player, args) {
    if (inventory_menu.Visible == true && (args.KeyCode == Keys.Escape || args.KeyCode == Keys.Back)) {
        inventory_menu.Visible = false;
        API.showCursor(false);
        inventory_menu.Clear();
    }
    if (inventory_menu.Visible == true && (args.KeyCode == Keys.Delete)) {   
        API.triggerServerEvent("delete_from_inventory", inventoryOwner, inventory_menu.CurrentSelection);
        inventory_menu.Visible = false;
        API.showCursor(false);
        inventory_menu.Clear();
    }
});

//-----------------------------------------------------