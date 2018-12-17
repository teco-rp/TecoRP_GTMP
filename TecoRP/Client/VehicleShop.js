/// <reference path="types-gt-mp/index.d.ts" />


var vehicle_shop_menu;
var isAdmin = false;
var vehShop = null;
let data = null;
var lastCreatedVeh = null;

API.onServerEventTrigger.connect(function (eventName, args) {
    if (eventName == "open_vehicle_shop") {
        data = JSON.parse(args[0]);
        vehShop = JSON.parse(args[1]);
        isAdmin = args[2] > 2;

        vehicle_shop_menu = API.createMenu("Galeri", "Buradan satın alabileceğiniz araçlar:", 0, 0, 6);

        for (var i = 0; i < data.length; i++) {
            if (data[i].p < 0) {
                if (isAdmin) {
                    vehicle_shop_menu.AddItem(API.createMenuItem("~c~" + data[i].m + " | $" + data[i].p, data[i].m));
                }
                else {
                    data.splice(i, 1);
                    i = i - 1;
                }
            }
            else {
                vehicle_shop_menu.AddItem(API.createMenuItem(data[i].m + " | $" + data[i].p, data[i].m));
            }
        }

        if (isAdmin) {
            vehicle_shop_menu.AddItem(API.createColoredItem("ADMIN HELP", "Fiyat değiştirmek için ~r~P~w~ tuşunu kullanabilirsiniz. ~r~-1~w~ satılmayacağı anlamına gelmektedir.", "#ff7300", "#f1f1f2"));
        }
        vehicle_shop_menu.Visible = true;
        //API.showCursor(true);
        vehicle_shop_menu.OnItemSelect.connect(function (sender, selected, index) {
            API.deleteEntity(API.getPlayerVehicle(API.getLocalPlayer()));
            vehicle_shop_menu.Visible = false;
            vehicle_shop_menu.Clear();
            vehicle_shop_menu = null;
            //API.showCursor(false);
            API.triggerServerEvent("BuyVehicle", vehShop.id, selected.Description);

        });

        vehicle_shop_menu.OnIndexChange.connect(function (sender, index) {

            //API.deleteEntity(API.getPlayerVehicle(API.getLocalPlayer()));
            if (lastCreatedVeh != null)
                API.deleteEntity(lastCreatedVeh);

            var pos = new Vector3(vehShop.pos.X, vehShop.pos.Y, vehShop.pos.Z);
            var rot = new Vector3(vehShop.rot.X, vehShop.rot.Y, vehShop.rot.Z);
            var vehHash = API.vehicleNameToModel(data[index].m);

            lastCreatedVeh = API.createVehicle(vehHash, pos, rot);
            //API.setPlayerIntoVehicle(lastCreatedVeh, -1);
            API.setEntityCollisionless(lastCreatedVeh, false);
            API.setVehicleEngineStatus(lastCreatedVeh, false);
        });
    }
});

API.onKeyDown.connect(function (sender, args) {

    if (vehicle_shop_menu != null && vehicle_shop_menu.Visible == true) {

        if (args.KeyCode == Keys.Escape || args.KeyCode == Keys.Back) {
            API.deleteEntity(lastCreatedVeh);
            vehicle_shop_menu.Visible = false;
            vehicle_shop_menu.Clear();
            vehicle_shop_menu = null;
            API.showCursor(false);
        }

        if (args.KeyCode == Keys.P && isAdmin) {

            let item = data[vehicle_shop_menu.CurrentSelection];
            var newMoney = API.getUserInput("$" + item.p, 12);
            API.triggerServerEvent("UpdateVehiclePrice", vehShop.id, item.m, newMoney);

            var idx = vehicle_shop_menu.CurrentSelection;
            vehicle_shop_menu.RemoveItemAt(idx);
            vehicle_shop_menu.InsertItem(API.createMenuItem(data[idx].m + "  $" + newMoney, data[idx].m), idx);
            vehicle_shop_menu.RefreshIndex();
        }
    }
});