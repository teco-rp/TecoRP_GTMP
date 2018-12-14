using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TecoRP.Database;
using TecoRP.Models;

namespace TecoRP.Managers
{
    public class ShopManager : Base.EventMethodTriggerBase
    {

        public void Key_E(Client sender)
        {
            Buy(sender);
        }

        public void Buy(Client sender)
        {
            foreach (var item in db_Shops.CurrentShopsList)
            {
                if (Vector3.Distance(sender.position, item.Position) <= item.Range + 2)
                {

                    var query = item.SaleItemList.Select(s => new
                    {
                        SaleItem = s,
                        GameItem = db_Items.GetItemById(s.GameItemId).AsSimply()
                    });
                    var json = JsonConvert.SerializeObject(query);
                    Debug.WriteLine(json.Length);
                    API.consoleOutput("Sender adminlevel is " + API.getEntityData(sender, "AdminLevel"));
                    API.triggerClientEvent(sender, "shop_open", item.ShopId, json, (int) API.getEntityData(sender, "AdminLevel"));

                    return;
                }
            }
        }
    }
}
