using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TecoRP.Database;
using TecoRP.Models;

namespace TecoRP.Managers
{
    public class VehicleShopManager : Base.EventMethodTriggerBase
    {
        public VehicleShopManager()
        {

        }

        public void Key_E(Client sender)
        {
            if (sender.isInVehicle)
                return;

            var nearest = db_VehicleShops.FindNearest(sender.position);
            if (Vector3.Distance(sender.position, nearest.Position) < 2)
            {
                var jsonVehs = JsonConvert.SerializeObject(db_VehicleShops.GetPricesByClass(nearest.VehicleClass));
                var jsonShop = JsonConvert.SerializeObject(nearest);

                var adminLevel = (int)sender.getData(nameof(Player.AdminLevel));
                API.consoleOutput("AdminLevel is " + adminLevel);
                API.triggerClientEvent(sender, "open_vehicle_shop", jsonVehs, jsonShop,adminLevel );
            }
        }

        public void BuyVehicle(Client sender, params object[] args)
        {
            var shop = db_VehicleShops.Get(Convert.ToInt32(args[0]));
            VehicleHash hash = API.vehicleNameToModel(args[1].ToString());

            var vehPrice = db_VehicleShops.GetPrice(hash);
            if(vehPrice == null) { API.sendChatMessageToPlayer(sender,"~r~HATA: ~w~Araç bulunamadı."); return; }

            if (!InventoryManager.IsEnoughMoney(sender, vehPrice.Price)) { API.sendChatMessageToPlayer(sender, $"~r~HATA: ~w~Bu aracı alabilmek için en az ~g~${vehPrice.Price}~w~ paranız olmalı."); return; }

            InventoryManager.AddMoneyToPlayer(sender, -vehPrice.Price);

            var veh = db_Vehicles.CreateVehicle(hash,shop.DeliveryPosition,shop.DeliveryRotation,shop.DeliveryDimension,sender.socialClubName);
            sender.setIntoVehicle(veh.VehicleOnMap, -1);
        }

        public void UpdateVehiclePrice(Client sender, params object[] args) //[vehicleShopId] [VehicleModel] [newPrice]
        {
            var vehShop = db_VehicleShops.Get(Convert.ToInt32(args[0]));
            var vehHash = (VehicleHash)Enum.Parse(typeof(VehicleHash), args[1].ToString());
            var newPrice = Convert.ToInt32(args[2].ToString().Replace("$", string.Empty));

            db_VehicleShops.SetPrice(vehHash, newPrice);
        }
    }
}
