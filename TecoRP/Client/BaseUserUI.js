/// <reference path="types-gt-mp/index.d.ts" />

/// <reference path="Vehicles.js"/>

var help_main_menu = null;
var menu_vehicle_actions = null;

API.onKeyDown.connect(function (sender, args) {

    switch (args.KeyCode) {
        case Keys.F2:
        callHelpMenu();
            break;
    }   
});

function callHelpMenu() {
    if (help_main_menu == null) {
        help_main_menu = API.createMenu("YARDIM", 100, 0, 3);
        help_main_menu.AddItem(API.createMenuItem("Kısayollar", "Oyun içerisinde kullanabileceğiniz kısayol tuşlarıdır."));
        help_main_menu.AddItem(API.createMenuItem("Araçlar", "Araçlar hakkında bilmeniz gerekenler."));
        help_main_menu.AddItem(API.createMenuItem("Evler", "Evler hakkında bilgiler."));
        help_main_menu.AddItem(API.createMenuItem("Envanter", "Evler hakkında bilgiler."));
        help_main_menu.OnItemSelect.connect(function (sender,selected,index) {
            switch (index) {
                case 0:
                    break;
                case 1:
                    help_main_menu.Visible = false;
                    callVehicleActionsMenu();
                    break;
                case 2:
                    break;
                case 3:
                    help_main_menu.Visible = false;
                    API.triggerServerEvent("key_I");
                    break;
            }
        });
    }
    help_main_menu.Visible = true;
    API.showCursor(false);    
}

function callShortcutsMenu() {

}

function callVehicleActionsMenu() {
    if (menu_vehicle_actions == null) {
        menu_vehicle_actions = API.createMenu("Araclar", "Araçlarla ilgili işlemler ve kısayolları", 100, 0, 3);
        menu_vehicle_actions.AddItem(API.createMenuItem("Motor", "Morotu çalıştırmak için ~y~T~w~ tuşunu kullanabilirsiniz. ((/motor))"));
        menu_vehicle_actions.AddItem(API.createMenuItem("Kilit", "Aracınızın kilitlemek/kilidini açmak için ~y~L~w~ tuşunu kullanabilirsiniz. ((/kilit))"));
        menu_vehicle_actions.AddItem(API.createMenuItem("Bagaj", "Aracınızın bagajını açmak için ~y~/bagaj~w~ komutunu kullanabilirsiniz."));
        menu_vehicle_actions.AddItem(API.createMenuItem("Bagaja Bak", "Aracınızın bagajının içine bakmak için ~y~/bagajabak~w~ komutunu kullanabilirsiniz."));
        menu_vehicle_actions.AddItem(API.createMenuItem("Torpido Gözü", "Aracınızın torpido gözündeki eşyalara göz atmak için ~y~/torpido~w~ komutunu kullanabilirsiniz."));
        menu_vehicle_actions.AddItem(API.createMenuItem("Kaput", "Aracınızın kaputunu açmak için ~y~/kaput~w~ komutunu kullanabilirsiniz."));
        menu_vehicle_actions.AddItem(API.createMenuItem("Araçlarım", "Tüm araçlarınızı gösteren listeyi açar. Ayrıca bu listeden araçlarınızın yerini öğrenebilirsiniz. ((~y~/araclarim~w~))"));
        menu_vehicle_actions.AddItem(API.createMenuItem("Park Et", "Aracınızın tüm özellikleriyle birlikte konumunu kaydetmeye yarar. Ayrıca ~y~/park~w~ komutu da kullanılabilir." ));
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
    }
    menu_vehicle_actions.Visible = true;
}

API.onKeyDown.connect(function (sender, args) {
    if (args === Keys.Back && menu_vehicle_actions !== null && menu_vehicle_actions.Visible === true) {
        menu_vehicle_actions.Visible = false;
        callHelpMenu();
    }
});

