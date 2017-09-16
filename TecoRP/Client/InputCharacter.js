API.onServerEventTrigger.connect(function (name, args) {

    if (name == "set_character_name") {
        API.triggerServerEvent("return_character_name", API.getUserInput("isim soyisim", 40), args[0]);
    }

    if (name == "set_character_sex") {
        API.triggerServerEvent("return_character_sex", API.getUserInput("kadın", 5), args[0]);
    }

    if (name == "snow_all") {
        API.setSnowEnabled(true, true, true, true);
    }
    if (name == "get_input") {
        API.triggerServerEvent(args[0], API.getUserInput("", 160));
    }

});

