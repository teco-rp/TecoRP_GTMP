using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TecoRP.Database;

namespace TecoRP.Managers
{
    public class SaleVehicleManager : Script
    {
        public static List<Vehicle> SaleVehiclesOnMap = new List<Vehicle>();
        public SaleVehicleManager()
        {
            
            db_Houses.GetAll();
            foreach (var item in db_SaleVehicles.GetAll().Items)
            {
                SaleVehiclesOnMap.Add(API.createVehicle(item.VehicleModel, new Vector3(item.Position.X, item.Position.Y, item.Position.Z), new Vector3(item.Rotation.X, item.Rotation.Y, item.Rotation.Z), item.VehicleColors.Color_1, item.VehicleColors.Color_2, item.Dimension));
                API.setVehicleEngineStatus(SaleVehiclesOnMap.LastOrDefault(), false);
                
            }
        }
    }
}
