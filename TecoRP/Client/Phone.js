//-----------------------PHONE-----------------------------
var phone_menu = API.createMenu("Hat Yok", "Telefon", 0, 0, 6);
var phonebook_menu = API.createMenu("Telefon Defteri", "Telefon", 0, 0, 6);
var phone_gps_menu = API.createMenu("GPS", "Haritalar", 0, 0, 6);
var phone_downloadapp_menu = API.createMenu("STORE", "Uygulamalar", 0, 0, 6);
var phone_emlakci_menu = API.createMenu("Emlak Uygulaması", "En uygun evler", 0, 0, 6);
var model_phone;
phone_menu.Visible = false;
phonebook_menu.Visible = false;
phone_gps_menu.Visible = false;
phone_downloadapp_menu.Visible = false;
phone_emlakci_menu.Visible = false;
phone_menu.ResetKey(menuControl.Back);
phonebook_menu.ResetKey(menuControl.Back);
phone_gps_menu.ResetKey(menuControl.Back);
phone_downloadapp_menu.ResetKey(menuControl.Back);
phone_emlakci_menu.ResetKey(menuControl.Back);

API.onServerEventTrigger.connect(function (name, args) {

    if (name == "phone_open" && phone_menu.Visible == false) {
        API.setMenuTitle(phone_menu, args[3]);
        API.setMenuSubtitle(phone_menu, "Telefon | " + args[4]);
        model_phone = args[5];

        phone_menu.AddItem(API.createMenuItem("ARA", "Tuş takımını açar."));
        phone_menu.AddItem(API.createMenuItem("SMS", "Mesajlaşma."));
        phone_menu.AddItem(API.createMenuItem("REHBER", "Kayıtlı arkadaşlarınız."));

        if (args[3] == "Vodacell") {
            API.setMenuBannerRectangle(phone_menu, 200, 255, 30, 25);
            API.setMenuBannerRectangle(phonebook_menu, 200, 255, 30, 25);
            API.setMenuBannerRectangle(phone_gps_menu, 200, 255, 30, 25);
            API.setMenuBannerRectangle(phone_downloadapp_menu, 200, 255, 30, 25);
            API.setMenuBannerRectangle(phone_emlakci_menu, 200, 255, 30, 25);
            phone_menu.AddItem(API.createMenuItem("UÇAK MODU", "Uçak modunu açar."));

        } else if (args[3] == "LosTelecom") {
            API.setMenuBannerRectangle(phone_menu, 200, 15, 255, 45);
            API.setMenuBannerRectangle(phonebook_menu, 200, 15, 255, 45);
            API.setMenuBannerRectangle(phone_downloadapp_menu, 200, 15, 255, 45);
            API.setMenuBannerRectangle(phone_emlakci_menu, 200, 15, 255, 45);
            phone_menu.AddItem(API.createMenuItem("UÇAK MODU", "Uçak modunu açar."));
        } else if (args[3] == "HAT YOK") {
            API.setMenuBannerRectangle(phone_menu, 150, 50, 50, 50);
            API.setMenuBannerRectangle(phonebook_menu, 150, 50, 50, 50);
            API.setMenuBannerRectangle(phone_gps_menu, 150, 50, 50, 50);
            API.setMenuBannerRectangle(phone_downloadapp_menu, 150, 50, 50, 50);
            API.setMenuBannerRectangle(phone_emlakci_menu, 150, 50, 50, 50);
            phone_menu.AddItem(API.createMenuItem("UÇAK MODU", "Uçak modunu açar."));
        } else {
            API.setMenuBannerRectangle(phone_menu, 150, 50, 50, 50);
            API.setMenuBannerRectangle(phonebook_menu, 150, 50, 50, 50);
            API.setMenuBannerRectangle(phone_gps_menu, 150, 50, 50, 50);
            API.setMenuBannerRectangle(phone_downloadapp_menu, 150, 50, 50, 50);
            API.setMenuBannerRectangle(phone_emlakci_menu, 150, 50, 50, 50);
            phone_menu.AddItem(API.createMenuItem("*UÇAK MODU", "Uçak modunu kapatır."));
        }
        phone_menu.AddItem(API.createMenuItem("UYGULAMA MAĞAZASI", "Uçak modunu açıp kapatır."));
        for (var i = 0; i < args[0]; i++) {
            phone_menu.AddItem(API.createMenuItem(args[1][i], ""));
        }
        phone_menu.Visible = true;
        API.showCursor(false);
    }

    if (name == "phone_contacts") {
        phonebook_menu.AddItem(API.createMenuItem("YENİ EKLE", ""));
        for (var i = 0; i < args[0]; i++) {
            phonebook_menu.AddItem(API.createMenuItem(args[1][i], args[2][i]));
        }
        //API.showCursor(true);
        phonebook_menu.Visible = true;
    }

    if (name == "phone_gps_open") {
        phone_gps_menu.AddItem(API.createMenuItem("En Yakın Banka", ""));
        phone_gps_menu.AddItem(API.createMenuItem("En Yakın Benzinlik", ""));
        phone_gps_menu.AddItem(API.createMenuItem("En Yakın Alışveriş", ""));
        phone_gps_menu.AddItem(API.createMenuItem("En Yakın Telefoncu", ""));
        phone_gps_menu.AddItem(API.createMenuItem("En Yakın Araç Satıcısı", ""));
        phone_gps_menu.AddItem(API.createMenuItem("En Yakın Meslek", ""));
        for (var i = 0; i < args[0]; i++) {
            phone_gps_menu.AddItem(API.createMenuItem(args[1][i], " "));
        }
        //API.showCursor(true);
        phone_gps_menu.Visible = true;
    }

    if (name == "phone_downloadscreen") {
        for (var i = 0; i < args[0]; i++) {
            phone_downloadapp_menu.AddItem(API.createMenuItem(args[1][i], args[2][i]));
        }
        //API.showCursor(true);
        phone_downloadapp_menu.Visible = true;
    }

    if (name == "phone_emlakci_open") {
        phone_emlakci_menu.Clear();
        phone_emlakci_menu.AddItem(API.createMenuItem("En Yakın Satılık Evler", ""));
        phone_emlakci_menu.AddItem(API.createMenuItem("En Ucuz Satılık Evler", ""));
        phone_emlakci_menu.AddItem(API.createMenuItem("En Pahalı Satılık Evler", ""));
        for (var i = 0; i < args[0]; i++) {
            phone_emlakci_menu.AddItem(API.createMenuItem(args[1][i], args[2][i]));
        }
        //API.showCursor(true);
        phone_emlakci_menu.Visible = true;
    }

});

phone_emlakci_menu.OnItemSelect.connect(function (sender, item, index) {
    API.triggerServerEvent("return_emlakci_selected", item.Text, model_phone);
    //phone_emlakci_menu.Visible = false;
    //phone_emlakci_menu.Clear();
    //API.showCursor(false);


});

phone_menu.OnItemSelect.connect(function (sender, item, index) {
    if (item.Text == "ARA") {
        API.triggerServerEvent("return_phone_call", API.getUserInput("0", 10), model_phone);
        phone_menu.Visible = false;
        API.showCursor(false);
    }
    else if (item.Text == "SMS") {
        var _number = API.getUserInput("0", 10);
        API.triggerServerEvent("return_phone_sms", _number, API.getUserInput("Mesajınız.", 160), model_phone);
        phone_menu.Visible = false;
        API.showCursor(false);
    } else
    if (item.Text == "Radyo") {
        API.triggerServerEvent("return_phone_radio_frequence", API.getUserInput("-1", 5));
    } else
        if (item.Text == "Reklamcılık") {
        API.sendChatMessage("Reklam metnini giriniz. ((Adınız ve telefon numaranız otomatik eklenecek.))");
        API.triggerServerEvent("return_phone_advertisement", API.getUserInput(" ",160));
    }
    else {
        API.triggerServerEvent("return_phone", item.Text, model_phone, model_phone);
        phone_menu.Visible = false
        API.showCursor(false);
    }
    phone_menu.Clear();
});

phonebook_menu.OnItemSelect.connect(function (sender, item, index) {
    if (item.Text == "YENİ EKLE") {
        var _number = API.getUserInput("0", 10);
        API.triggerServerEvent("phone_contacts_add", _number, API.getUserInput("kişi", 50));
    }
    else {
        API.triggerServerEvent("return_phone_call", item.Description)
    }
    API.showCursor(false);
    phonebook_menu.Visible = false;
    phonebook_menu.Clear();

});

phone_gps_menu.OnItemSelect.connect(function (sender, item, index) {

    API.triggerServerEvent("phone_gps_point", item.Text, model_phone);
    API.showCursor(false);
    phone_gps_menu.Visible = false;
    phone_gps_menu.Clear();

});

phone_downloadapp_menu.OnItemSelect.connect(function (sender, item, index) {
    API.triggerServerEvent("return_phone_download", item.Text, model_phone);
    API.showCursor(false);
    phone_downloadapp_menu.Visible = false;
    phone_downloadapp_menu.Clear();
});



API.onKeyDown.connect(function (Player, args) {
    if ((args.KeyCode == Keys.Escape || args.KeyCode == Keys.Back)) {
        API.triggerServerEvent("anim_stop");
        phone_menu.Visible = false;
        phonebook_menu.Visible = false;
        phone_gps_menu.Visible = false;
        phone_downloadapp_menu.Visible = false;
        phone_emlakci_menu.Visible = false;
        phone_menu.Clear();
        phonebook_menu.Clear();
        phone_gps_menu.Clear();
        phone_downloadapp_menu.Clear();
        phone_emlakci_menu.Clear();
        API.showCursor(false);
    }
});


//------------------PHONEBOOK-----------------------