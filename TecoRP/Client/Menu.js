let menu = null;

API.onServerEventTrigger.connect(function (name, args) {
    if (name == "my_vehicles") {
        menu = API.createMenu("Araçlarım", 0, 0, 6);
        let item0 = API.createMenuItem(args[0], "");
        let item1 = API.createMenuItem(args[1], "");
        let item2 = API.createMenuItem(args[2], "");
        let item3 = API.createMenuItem(args[3], "");
        let item4 = API.createMenuItem(args[4], "");

        item0.Activated.connect(function (menu, item) {
            API.triggerServerEvent("find_vehicle", args[0]);
            menu.Visible = false;
        });
        item1.Activated.connect(function (menu, item) {
            API.triggerServerEvent("find_vehicle", args[1]);
            menu.Visible = false;
        });
        item2.Activated.connect(function (menu, item) {
            API.triggerServerEvent("find_vehicle", args[2]);
            menu.Visible = false;
        });
        item3.Activated.connect(function (menu, item) {
            API.triggerServerEvent("find_vehicle", args[3]);
            menu.Visible = false;
        });
        item4.Activated.connect(function (menu, item) {
            API.triggerServerEvent("find_vehicle", args[4]);
            menu.Visible = false;
        });

        if (args[0] != "") { menu.AddItem(item0); }
        if (args[0] != "") { menu.AddItem(item1); }
        if (args[0] != "") { menu.AddItem(item2); }
        if (args[0] != "") { menu.AddItem(item3); }
        if (args[0] != "") { menu.AddItem(item4); }

        menu.Visible = true;
    }

    if (name == "set_ui_color") {
        API.setUiColor(args[0], args[1], args[2]);
    }
    

});


//API.onUpdate.connect(function () {
//    if (menu != null) {
//        menuPool.ProcessMenus();
//    }
//});
