//-------------------BANKA--------------------------
var bank_menu = API.createMenu("Banka/ATM", "Hesap No", -50, 0, 4);
bank_menu.Visible = false;
bank_menu.ResetKey(menuControl.Back);

var model_phone;
API.onServerEventTrigger.connect(function (name, args) {
    if (name == "bank_open" && bank_menu.Visible == false && !API.isCursorShown()) {

        API.setMenuTitle(bank_menu, args[3]);
        API.setMenuSubtitle(bank_menu, args[4]);
        model_phone = args[5];
        for (var i = 0; i < args[0]; i++) {          

            bank_menu.AddItem(API.createMenuItem(args[1][i], args[2][i]));
        }


        API.showCursor(true);
        bank_menu.Visible = true;
    }
});

bank_menu.OnItemSelect.connect(function (sender, item, index) {

    bank_menu.Visible = false;
    API.showCursor(false);
    if (item.Text.toLowerCase() == "para çek") { API.triggerServerEvent("bank_withdraw", API.getUserInput("0", 5)); } else
        if (item.Text.toLowerCase() == "para yatır" || item.Text.toLowerCase() == "para yatir") { API.triggerServerEvent("bank_deposit", API.getUserInput("0", 5)); } else
            if (item.Text.toLowerCase() == "hesap aç") { API.triggerServerEvent("bank_createaccount"); } else
                if (item.Text == "VERGİ SORGULA") { API.triggerServerEvent("vehicle_tax", API.getUserInput("LS-", 7)); } else
                    if (item.Text == "VERGİ ÖDE") { API.triggerServerEvent("vehicle_tax_pay", API.getUserInput("LS-", 7)); } else
                        if (item.Text.toLowerCase() == "havale yap") {
                            var bankAccount = API.getUserInput("LS-", 9);
                            API.triggerServerEvent("bank_transfer", bankAccount, API.getUserInput("0", 9));
                        }
                        else {
                            API.triggerServerEvent("return_bank", item.Text, model_phone, API.getUserInput("512", 4));
                        }
    bank_menu.Clear();

});


API.onKeyDown.connect(function (Player, args) {
    if (args.KeyCode == Keys.Escape && bank_menu.Visible == true) {
        bank_menu.Visible = false;
        bank_menu.Clear();
        API.showCursor(false);
    }
});