using GrandTheftMultiplayer.Server.API;
using System.Collections.Generic;
using System.Linq;
using TecoRP.Database;

namespace TecoRP.Managers
{
    public class BlipManager : Script
    {

        public static List<GrandTheftMultiplayer.Server.Elements.Blip> BlipsOnMap = new List<GrandTheftMultiplayer.Server.Elements.Blip>();
        public BlipManager()
        {
            
            db_Blips dbBlips = new Database.db_Blips();
            dbBlips.GetAll();

            foreach (var item in db_Blips.currentBlips.Items)
            {
                BlipsOnMap.Add(API.createBlip(item.Position, item.Range, item.Dimension));
                BlipsOnMap.LastOrDefault().color = item.Color;
                BlipsOnMap.LastOrDefault().name = item.Name;
                BlipsOnMap.LastOrDefault().sprite = item.ModelId;
            }
        }



        
    }
}
