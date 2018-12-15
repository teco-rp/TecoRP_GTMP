/// <reference path="types-gt-mp/index.d.ts" />


var help_main_menu = null;

API.onKeyDown.connect(function (sender, args) {

    switch (args.KeyCode) {
        case Keys.F2:
            callHelpMenu();
            break;
    }
});

function callHelpMenu() {
    if (help_main_menu == null) {
        help_main_menu = API.createMenu("YARDIM", "Oyun içi yardım menüsü", 100, 0, 3);
        //help_main_menu.SetBannerType(new Sprite("shopui_title_conveniencestore", "shopui_title_conveniencestore", new Point(0, 0), new Size(100, 100)));

        help_main_menu.AddItem(API.createMenuItem("Kısayollar", "Oyun içerisinde kullanabileceğiniz kısayol tuşlarıdır."));
        API.addSubMenu(help_main_menu, callVehicleActionsMenu(), "Araçlar", "Araçlar hakkında bilmeniz gerekenler.");
        API.addSubMenu(help_main_menu, callAnimationHelpMenu(), "Animasyonlar", "Oyun içerisindeki bazı temel animasyonlar.");

        help_main_menu.AddItem(API.createMenuItem("Evler", "Evler hakkında bilgiler."));
        help_main_menu.AddItem(API.createMenuItem("Envanter", "Evler hakkında bilgiler."));



        //help_main_menu.OnItemSelect.connect(function (sender,selected,index) {
        //    switch (index) {
        //        case 0:
        //            break;
        //        case 1:
        //            help_main_menu.Visible = false;
        //            callVehicleActionsMenu();
        //            break;
        //        case 2:
        //            break;
        //        case 3:
        //            break;
        //        case 4:
        //            help_main_menu.Visible = false;
        //            API.triggerServerEvent("key_I");
        //            break;
        //    }
        //});
    }
    help_main_menu.Visible = true;
    API.showCursor(false);
}

function callShortcutsMenu() {

}

function callVehicleActionsMenu() {

    var menu_vehicle_actions = API.createMenu("Araclar", "Araçlarla ilgili işlemler ve kısayolları", 100, 0, 3);
    menu_vehicle_actions.AddItem(API.createMenuItem("Motor", "Morotu çalıştırmak için ~y~T~w~ tuşunu kullanabilirsiniz. ((/motor))"));
    menu_vehicle_actions.AddItem(API.createMenuItem("Kilit", "Aracınızın kilitlemek/kilidini açmak için ~y~L~w~ tuşunu kullanabilirsiniz. ((/kilit))"));
    menu_vehicle_actions.AddItem(API.createMenuItem("Bagaj", "Aracınızın bagajını açmak için ~y~/bagaj~w~ komutunu kullanabilirsiniz."));
    menu_vehicle_actions.AddItem(API.createMenuItem("Bagaja Bak", "Aracınızın bagajının içine bakmak için ~y~/bagajabak~w~ komutunu kullanabilirsiniz."));
    menu_vehicle_actions.AddItem(API.createMenuItem("Torpido Gözü", "Aracınızın torpido gözündeki eşyalara göz atmak için ~y~/torpido~w~ komutunu kullanabilirsiniz."));
    menu_vehicle_actions.AddItem(API.createMenuItem("Kaput", "Aracınızın kaputunu açmak için ~y~/kaput~w~ komutunu kullanabilirsiniz."));
    menu_vehicle_actions.AddItem(API.createMenuItem("Araçlarım", "Tüm araçlarınızı gösteren listeyi açar. Ayrıca bu listeden araçlarınızın yerini öğrenebilirsiniz. ((~y~/araclarim~w~))"));
    menu_vehicle_actions.AddItem(API.createMenuItem("Park Et", "Aracınızın tüm özellikleriyle birlikte konumunu kaydetmeye yarar. Ayrıca ~y~/park~w~ komutu da kullanılabilir."));
    menu_vehicle_actions.AddItem(API.createMenuItem("Emniyet Kemeri", "Emniyet kemerini takıp çıkarmak için ~y~/ekemer~w~ komutunu kullanabilirsiniz."));

    menu_vehicle_actions.OnItemSelect.connect(function (sender, selected, index) {

        switch (index) {
            case 0:
                API.triggerServerEvent("key_Y");
                break;
            case 1:
                API.triggerServerEvent("key_L");
                break;
            case 2:
                API.triggerServerEvent("VehicleBaggage");
                break;
            case 3:
                API.triggerServerEvent("LookToBaggage");
                break;
            case 4:
                API.triggerServerEvent("LookToTorpedo");
                break;
            case 5:
                API.triggerServerEvent("VehicleHood");
                break;
            case 6:
                API.triggerServerEvent("MyVehicles");
                break;
            case 7:
                API.triggerServerEvent("ParkVehicle");
                break;
            case 8:
                API.triggerServerEvent("Belt");
                break;
        }
        menu_vehicle_actions.Visible = false;
    });

    //menu_vehicle_actions.Visible = true;

    return menu_vehicle_actions;
}


function callAnimationHelpMenu() {
    var anim_menu = API.createMenu("Animasyonlar", "Anmiasyonların listesi", 100, 0, 3);

    anim_menu.AddItem(API.createMenuItem("Eller Yukarı", "~y~CTRL + 1~w~ kombinasyonu ile kolayca yapabileceğiniz animasyondur."));
    anim_menu.AddItem(API.createMenuItem("Yaslanarak Otur", "~y~CTRL + 2~w~ kombinasyonu ile de yapabilirsiniz."));
    anim_menu.AddItem(API.createMenuItem("Eğilerek Otur", "~y~CTRL + 3~w~ kombinasyonu ile de yapabilirsiniz."));
    anim_menu.AddItem(API.createMenuItem("Geri Yaslan", "~y~CTRL + 4~w~ kombinasyonu ile de yapabilirsiniz."));

    anim_menu.OnItemSelect.connect(function (sender, selected, index) { if (index < 4) { API.triggerServerEvent("CTRL_" + (index + 1)); } });

    var sit_menu = API.createListItem("Otur", "", getListByRange(1, 6), 0);
    sit_menu.Activated.connect(function (sender, index) { API.triggerServerEvent("Sit", sit_menu.CurrentItem()); });
    anim_menu.AddItem(sit_menu);

    var dance_menu = API.createListItem("Dans", "Ayrıca ~y~/dans~w~ komutu kullanılarak da yapılabilir.", getListByRange(1, 5), 0);
    dance_menu.Activated.connect(function (sender, index) { API.triggerServerEvent("Dance", sit_menu.CurrentItem()); });
    anim_menu.AddItem(dance_menu);

    let anim_list_menu = API.createListItem("Anim", "Ayrıca ~y~/anim~w~ komutu kullanılarak da yapılabilir.", getAnimsList(), 0);
    anim_list_menu.Activated.connect(function (sender, selected) { API.triggerServerEvent("Anim", anim_list_menu.CurrentItem()); });
    anim_menu.AddItem(anim_list_menu);
    return anim_menu;
}

function getListByRange(start, end) {
    var list = new List(String);
    for (var i = start; i <= end; i++) {
        list.Add(i.toString());
    }
    return list;
}
function getAnimsList() {
    var list = new List(String);
    list.Add("finger");
    list.Add("guitar");
    list.Add("shagging");
    list.Add("shagging");
    list.Add("synth");
    list.Add("kiss");
    list.Add("bro");
    list.Add("chicken");
    list.Add("chin");
    list.Add("dj");
    list.Add("dock");
    list.Add("facepalm");
    list.Add("fingerkiss");
    list.Add("freakout");
    list.Add("jazzhands");
    list.Add("knuckle");
    list.Add("nose");
    list.Add("no");
    list.Add("peace");
    list.Add("photo");
    list.Add("rock");
    list.Add("salute");
    list.Add("shush");
    list.Add("slowclap");
    list.Add("surrender");
    list.Add("thumbs");
    list.Add("taunt");
    list.Add("vsign");
    list.Add("wank");
    list.Add("wave");
    list.Add("loco");
    return list;
}
API.onKeyDown.connect(function (sender, args) {
    if (args === Keys.Back && menu_vehicle_actions !== null && menu_vehicle_actions.Visible === true) {
        menu_vehicle_actions.Visible = false;
        callHelpMenu();
    }
});
