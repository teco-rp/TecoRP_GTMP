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
                    API.triggerClientEvent(sender, "shop_open", item.ShopId, json, (int)API.getEntityData(sender, "AdminLevel"), (bool) API.getEntityData(sender,"Gender"));

                    return;
                }
            }
        }

        public void BuyItem(Client sender, params object[] args)
        {
            //args = [shopid] [gameItemId] [index]
            var _Shop = db_Shops.GetShop(Convert.ToInt32(args[0]));

            if (_Shop == null) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Satıcı bulunamadı."); return; }

            var _saleItem = _Shop.SaleItemList.FirstOrDefault(x => x.GameItemId == Convert.ToInt32(args[1]));
            if (_Shop == null) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bu eşya bu satıcıda satılmıyor."); return; }

            int index = Convert.ToInt32(args[2]);

            if (!InventoryManager.IsEnoughMoney(sender, _saleItem.Price)) { API.sendChatMessageToPlayer(sender, $"Bunu alabilmek için en az ~r~{_saleItem.Price}$ ~s~paranız olmalı."); return; }


            var buyedItem = db_Items.GetItemById(_saleItem.GameItemId);
            int missionNumber = (API.hasEntityData(sender, "Mission") ? API.getEntityData(sender, "Mission") : 0);

            switch (buyedItem.Type)
            {
                case ItemType.Phone:
                    if (InventoryManager.AddItemToPlayerInventory(sender, new Models.ClientItem
                    {
                        ItemId = buyedItem.ID,
                        Count = 1,
                        Equipped = false,
                        SpecifiedValue = API.toJson(new SpecifiedValuePhone { Applications = new List<Application> { Application.GPS }, AutoInternetPay = false, Balance = 0, FlightMode = false, PhoneOperator = null, Contacts = new Dictionary<string, string>(), Frequence = -1, InternetBalance = 0, PhoneNumber = null, })
                    }))
                    {
                        InventoryManager.AddMoneyToPlayer(sender, -1 * _Shop.SaleItemList[index].Price);
                        #region AboutMission

                        if (missionNumber == 2)
                        {
                            Clients.ClientManager.RemoveMissionMarker(sender);
                            API.setEntityData(sender, "Mission", 3);
                            UserManager.TriggerUserMission(sender);
                        }

                        #endregion
                        return;
                    }
                    break;
                case ItemType.Skin:
                    if (InventoryManager.AddItemToPlayerInventory(sender, new ClientItem { ItemId = buyedItem.ID, Count = 1, Equipped = false }))
                    {
                        InventoryManager.AddMoneyToPlayer(sender, -1 * _Shop.SaleItemList[index].Price);
                        return;
                    }
                    break;
                case ItemType.Weapon:
                    var _inventory = (Inventory)API.getEntityData(sender, "inventory");

                    if (Convert.ToInt32(buyedItem.Value_2) < 1 || _inventory.ItemList.Any(x => x.ItemId == 302 && x.SpecifiedValue == sender.socialClubName || Convert.ToInt32(buyedItem.Value_2) == 4))
                    {
                        if (InventoryManager.AddItemToPlayerInventory(sender, new ClientItem { ItemId = buyedItem.ID, Count = 1, Equipped = false }))
                        {
                            InventoryManager.AddMoneyToPlayer(sender, -1 * _Shop.SaleItemList[index].Price);
                        }
                    }
                    else
                    {
                        API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Buradan alışveriş yapmak için kendinize ait bir silah ruhsatına ihtiyacınız var.");
                    }

                    return;
                default:
                    if (InventoryManager.AddItemToPlayerInventory(sender, new ClientItem { ItemId = buyedItem.ID, Count = 1, Equipped = false }))
                    {
                        InventoryManager.AddMoneyToPlayer(sender, -1 * _Shop.SaleItemList[index].Price);
                        #region AboutMission
                        if (missionNumber == 1 && buyedItem.Type >= ItemType.Mask && buyedItem.Type <= ItemType.Tops)
                        {
                            Clients.ClientManager.RemoveMissionMarker(sender);
                            API.setEntityData(sender, "Mission", 2);
                            UserManager.TriggerUserMission(sender);
                        }
                        #endregion
                        return;
                    }
                    break;
            }
            API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bu eşya için envanterinizde daha fazla yer bulunmuyor.");
        }
    }
}
