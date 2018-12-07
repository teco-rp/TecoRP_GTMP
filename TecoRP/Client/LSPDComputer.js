//--------------LSPD------------

var lspd_menu = API.createMenu("LSPD", "LSPD Veritabanı", 100, 100, 6);
lspd_menu.Visible = false;
lspd_menu.ResetKey(menuControl.Back);


var commmandType = null;
var socialClubID = null;
API.onServerEventTrigger.connect(function (name, args) {

    if (name == "lspd_guilty_list_open") {

        commmandType = args[3];
        socialClubID = args[4]; 
        for (var i = 0; i < args[0]; i++) {
            lspd_menu.AddItem(API.createMenuItem(args[1][i], args[2][i]));
        }
        API.showCursor(true);
        lspd_menu.Visible = true;
    }
    else
        if (name == "lspd_main_menu") {
            lspd_menu.AddItem(API.createMenuItem("SUÇLULAR", "Tüm suçluların listesi"));
            lspd_menu.AddItem(API.createMenuItem("PARMAK İZİ SORGULA", "Parmakizindne suçlu bulmak için"));
            lspd_menu.AddItem(API.createMenuItem("SUÇ EKLE", "Yeni bir suç eklemek için"));
            API.showCursor(true);
            lspd_menu.Visible = true;
        }
});

lspd_menu.OnItemSelect.connect(function (sender, item, index) {

    if (item.Text == "SUÇLULAR") {
        API.triggerServerEvent("lspd_guilty_list_request");
        lspd_menu.Visible = false;
        API.showCursor(false);
        lspd_menu.Clear();
    } else
        if (item.Text == "PARMAK İZİ SORGULA") {
            API.triggerServerEvent("lspd_fingerprint_request", API.getUserInput("", 8));
            lspd_menu.Visible = false;
            API.showCursor(false);
            lspd_menu.Clear();
        } else
            if (item.Text == "SUÇ EKLE") {
                API.sendChatMessage("Suçlunun parmak izini giriniz.");
                API.triggerServerEvent("lspd_crime_add_request", API.getUserInput("", 8));
                lspd_menu.Visible = false;
                API.showCursor(false);
                lspd_menu.Clear();
            }
            else {
                API.triggerServerEvent("lspd_computer_return", commmandType, index, socialClubID);
                commmandType = null;
                socialClubID = null;
                lspd_menu.Visible = false;
                API.showCursor(false);
                lspd_menu.Clear();
            }
});

API.onKeyDown.connect(function (Player, args) {
    if (args.KeyCode == Keys.Escape && lspd_menu.Visible == true) {
        lspd_menu.Visible = false;
        API.showCursor(false);
        lspd_menu.Clear();
    }

});


