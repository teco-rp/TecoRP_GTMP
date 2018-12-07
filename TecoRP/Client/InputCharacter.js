API.onServerEventTrigger.connect(function (name, args) {

    if (name == "set_character_name") {
        API.triggerServerEvent("return_character_name", API.getUserInput("isim soyisim", 40), args[0]);
    }

    if (name == "set_character_sex") {
        var selectionMenu = API.createMenu("Cinsiyet", "Karakterinizin cinsiyetini seçiniz.", 100, 100, 6);
        selectionMenu.AddItem(API.createMenuItem("Kadın", ""));
        selectionMenu.AddItem(API.createMenuItem("Erkek", ""));
        selectionMenu.Visible = true;

        selectionMenu.OnItemSelect.connect(function (sender, item, index) {
            API.triggerServerEvent("return_character_sex", index == 0 ? "Kadın" : "Erkek", args[0]);
            selectionMenu.Visible = false;
        });
    }

    if (name == "snow_all") {
        API.setSnowEnabled(true, true, true, true);
    }
    if (name == "get_input") {
        API.triggerServerEvent(args[0], API.getUserInput("", 160));
    }

});

