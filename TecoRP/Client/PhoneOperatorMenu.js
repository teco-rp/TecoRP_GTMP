//-------------JOB-LEVELS-------------
var operator_menu = null;


API.onServerEventTrigger.connect(function (name, args) {
    if (name === "open_operator_menu") {
        operator_menu = API.createMenu(args[0], "Bu hat satış noktasında yapabilecekleriniz:", 100, 100, 6);
        operator_menu.ResetKey(menuControl.Back);
        operator_menu.AddItem(API.createMenuItem("Fiyatlandırma", args[1]));
        
        operator_menu.AddItem(API.createMenuItem("Hat Satın Al", ""));        
        operator_menu.AddItem(API.createMenuItem("Dakika yükle", ""));        
        operator_menu.AddItem(API.createMenuItem("İnternet yükle", ""));

        operator_menu.Visible = true;
        API.showCursor(true);


        operator_menu.OnItemSelect.connect(function (sender, item, index) {
            switch (index) {
                case 1:
                    API.triggerServerEvent("BuySimCard", args[0]);
                    break;
                case 2:
                    API.triggerServerEvent("BuyCalling", args[0], API.getUserInput("25", 4));
                    break;
                case 3:
                    API.triggerServerEvent("BuyInternet", args[0], API.getUserInput("1000", 4));
                    break;
                default:
            }
            operator_menu.Visible = false;
            API.showCursor(false);
            operator_menu = null;
        });
    }
});



API.onKeyDown.connect(function (Player, args) {
    if (operator_menu !== null && operator_menu.Visible == true && args.KeyCode == Keys.Escape) {
        operator_menu.Visible = false;
        API.showCursor(false);
        operator_menu.Clear();
    }

});