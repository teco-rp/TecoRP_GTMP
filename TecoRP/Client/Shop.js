var shop_menu = API.createMenu("Alısveris", "Satışta Olan Eşyalar", 0, 0, 6);
shop_menu.Visible = false;
shop_menu.ResetKey(menuControl.Back);
var shopId = null;

API.onServerEventTrigger.connect(function (name, args) {

    if (name == "shop_open" && shop_menu.Visible == false) {

        shopId = args[3];
        for (var i = 0; i < args[0]; i++) {
            
            shop_menu.AddItem(API.createMenuItem(args[1][i], args[2][i]));
        }
        API.showCursor(true);
        shop_menu.Visible = true;
    }
});

shop_menu.OnItemSelect.connect(function (sender, item, index) {
    API.triggerServerEvent("shop_item_selected", shopId,index);
    shop_menu.Visible = false;
    API.showCursor(false);
    shop_menu.Clear();

});

API.onKeyDown.connect(function (Player, args) {
    if (args.KeyCode == Keys.Escape && shop_menu.Visible == true) {
        shop_menu.Visible = false;
        API.showCursor(false);
        shop_menu.Clear();
 
    }
});
