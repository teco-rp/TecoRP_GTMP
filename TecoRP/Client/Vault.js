//--------------Vault------------

var vault_menu = API.createMenu("Olusum Kasa", "Oluşum Kasası", 0, 0, 6);
vault_menu.Visible = false;
vault_menu.ResetKey(menuControl.Back);


var IDlist = null;
API.onServerEventTrigger.connect(function (name, args) {

    if (name == "vault_open") {

        IDlist = args[3];
        for (var i = 0; i < args[0]; i++) {
            vault_menu.AddItem(API.createMenuItem(args[1][i], args[2][i]));
        }
        API.showCursor(true);
        vault_menu.Visible = true;
    }
});

vault_menu.OnItemSelect.connect(function (sender, item, index) {
    API.triggerServerEvent("retun_vault", IDlist[index]);
    vault_menu.Visible = false;
    API.showCursor(false);
    vault_menu.Clear();
});

API.onKeyDown.connect(function (Player, args) {
    if (args.KeyCode == Keys.Escape && vault_menu.Visible == true) {
        vault_menu.Visible = false;
        API.showCursor(false);
        vault_menu.Clear();
    }
});
