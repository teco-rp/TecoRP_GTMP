var res_Y = API.getScreenResolutionMaintainRatio().Height;
var vehicle_fuel = null;
API.onServerEventTrigger.connect(function (name, args) {
    if (name == "show_vehicle_fuel") {
        vehicle_fuel = args[0];
    }
    if (name == "save_fuel" && vehicle_fuel != null) {
        API.triggerServerEvent("return_save_fuel", vehicle_fuel);
    }
});


API.onUpdate.connect(function () {
    if (vehicle_fuel != null) {
        var fueltext = "Benzin: " + vehicle_fuel;
        if (vehicle_fuel > 15) {
            API.drawText(fueltext.substr(0, 11) + "%", 375, res_Y - 80, 1, 255, 255, 255, 200, 7, 0, true, false, 0);
        }
        else {
            if (vehicle_fuel <= 0) {
                if (API.getVehicleEngineStatus(API.getPlayerVehicle(API.getLocalPlayer()))) {
                    API.setVehicleEngineStatus(API.getPlayerVehicle(API.getLocalPlayer()), false);
                    API.triggerServerEvent("turn_off_engine", API.getPlayerVehicle(API.getLocalPlayer()));
                    API.sendChatMessage("~r~Aracın benzini bitti.");
                }

            } else {
                API.drawText(fueltext.substr(0, 11) + "%", 375, res_Y - 80, 1, 255, 20, 30, 200, 7, 0, true, false, 0);
            }
        }

        if (vehicle_fuel > 0 && API.getVehicleEngineStatus(API.getPlayerVehicle(API.getLocalPlayer()))) {
            var car = API.getPlayerVehicle(API.getLocalPlayer());
            var rpm = API.getVehicleRPM(car);
            vehicle_fuel = vehicle_fuel - rpm * 0.0008;
        }
    }
});
API.onPlayerExitVehicle.connect(function (vehicle) {
    API.triggerServerEvent("return_vehicle_fuel", vehicle_fuel, vehicle);
    vehicle_fuel = null;
});