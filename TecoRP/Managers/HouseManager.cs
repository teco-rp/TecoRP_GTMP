using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TecoRP.Database;
using TecoRP.Models;

namespace TecoRP.Managers
{
    public class HouseManager : Script
    {

        //ikinci el satınal vergisi %2 

        [Command("ev", "/ev [satinal/fiyat/kilit/satiligacikar]", GreedyArg = true)]
        public void HouseCommand(Client sender, string type)
        {
            if ("fiyat".StartsWith(type.ToLower()))
            {
                foreach (var item in db_Houses.CurrentHousesDict.Values)
                {
                    if (Vector3.Distance(sender.position, item.EntrancePosition) < 2)
                    {
                        API.sendChatMessageToPlayer(sender, "~b~Evin fiyatı : " + item.Price);
                        return;
                    }
                }
            }
            if ("satinal".StartsWith(type.ToLower()))
            {
                #region SatinAlRegion
                foreach (var itemHouse in db_Houses.CurrentHousesDict.Values)
                {
                    if (Vector3.Distance(sender.position, itemHouse.EntrancePosition) < 2)
                    {
                        if (itemHouse.IsSelling)
                        {
                            int money = API.getEntityData(sender, "Money");
                            if (itemHouse.Price <= money)
                            {

                                if (!String.IsNullOrEmpty(itemHouse.OwnerSocialClubName))
                                {
                                    var sellerPlayer = db_Accounts.IsPlayerOnline(itemHouse.OwnerSocialClubName);
                                    if (sellerPlayer!=null)
                                    {
                                        int sellerBankMoney = API.getEntityData(sender, "BankMoney");
                                        sellerBankMoney += (int)(itemHouse.Price * 0.98f);
                                        money -= (int)itemHouse.Price;
                                        API.setEntityData(sender,"Money", money);
                                        API.setEntityData(sellerPlayer, "BankMoney", sellerBankMoney);
                                        API.triggerClientEvent(sender, "update_money_display", money);
                                        API.sendNotificationToPlayer(sender, "~r~-" + itemHouse.Price+"$");
                                        API.sendNotificationToPlayer(sellerPlayer, "Banka Hesabı:\n~g~+" + (int)(itemHouse.Price * 0.98f));
                                        SetHouseOwner(itemHouse.HouseId,sender.socialClubName);
                                        itemHouse.IsLocked = true;
                                        itemHouse.IsSelling = false;
                                        db_Houses.UpdateHouse(itemHouse);
                                        API.sendChatMessageToPlayer(sellerPlayer, "~g~Tebrikler! ~s~Satışta olan eviniz başarıyla satıldı.");
                                    }
                                    else
                                    {
                                        var sellerOfflinePlayer = db_Accounts.GetOfflineUserDatas(itemHouse.OwnerSocialClubName);
                                        sellerOfflinePlayer.BankAccount += itemHouse.Price;
                                        db_Accounts.SaveOfflineUserData(sellerOfflinePlayer.socialClubName, sellerOfflinePlayer);
                                        HouseMarkerColor hmc = new Models.HouseMarkerColor();
                                        var _house = db_Houses.GetHouse(itemHouse.HouseId);
                                        _house.MarkerOnMap.color = new Color(hmc.NormalColor.Red, hmc.NormalColor.Green, hmc.NormalColor.Blue, 255);
                                        _house.IsSelling = false;
                                        db_Houses.SaveChanges();

                                    }
                                    // if (db_Accounts.IsPlayerOnline(itemHouse.OwnerSocialClubName)!=null)
                                    // {

                                    // }
                                    //var sellerUser = db_Accounts.GetOfflineUserDatas(itemHouse.OwnerSocialClubName);
                                    // sellerUser.Money += itemHouse.Price;
                                    // itemHouse.IsSelling = false;

                                    //API.sendChatMessageToPlayer(sender, "~y~İkinci el satış henüz yapılmadı.");
                                    return;
                                }
                                else
                                {
                                    itemHouse.OwnerSocialClubName = sender.socialClubName;

                                    HouseMarkerColor hmc = new Models.HouseMarkerColor();
                                    var _house = db_Houses.GetHouse(itemHouse.HouseId);
                                    _house.MarkerOnMap.color = new Color(hmc.NormalColor.Red, hmc.NormalColor.Green, hmc.NormalColor.Blue, 255);
                                    _house.IsSelling = false;
                                    db_Houses.SaveChanges();
                                    money -= itemHouse.Price;
                                    API.triggerClientEvent(sender, "update_money_display", money);
                                    API.sendNotificationToPlayer(sender, "~g~Hayırlı olsun.\n~s~ Yeni bir ev satın aldınız. ");
                                }

                            }
                            else
                            {
                                API.sendChatMessageToPlayer(sender, "~y~Bu evi almak için paranız en az " + itemHouse.Price + " olmalı.");
                            }
                            return;
                        }
                        else
                        {
                            API.sendNotificationToPlayer(sender, "~r~Maalesef bu ev satılık değil.");
                        }
                    }
                }
                #endregion
            }
            if ("kilit".StartsWith(type.ToLower()))
            {
                #region  foreach
                foreach (var item in db_Houses.CurrentHousesDict.Values)
                {
                    if (sender.dimension == item.EntranceDimension && Vector3.Distance(sender.position, item.EntrancePosition) < 2)
                    {
                        if (item.OwnerSocialClubName == sender.socialClubName)
                        {
                            RPGManager rpgMgr = new RPGManager();
                            rpgMgr.Me(sender, (item.IsLocked ? " cebinden anahtarlarını çıkarır ve kapının kilidini açar." : " kapıyı kilitler."));
                            item.IsLocked = !item.IsLocked;
                        }
                        else
                        {
                            API.sendChatMessageToPlayer(sender, "~y~Bu ev size ait değil.");
                        }
                        return;
                    }
                    if (sender.dimension == item.InteriorDimension && Vector3.Distance(sender.position, item.InteriorPosition) < 2)
                    {
                        if (item.OwnerSocialClubName == sender.socialClubName)
                        {
                            RPGManager rpgMgr = new RPGManager();
                            rpgMgr.Me(sender, (item.IsLocked ? " cebinden anahtarlarını çıkarır ve kapının kilidini açar." : " kapıyı kilitler."));
                            item.IsLocked = !item.IsLocked;
                        }
                        else
                        {
                            API.sendChatMessageToPlayer(sender, "~y~Bu ev size ait değil.");
                        }
                        return;
                    }
                } 
                #endregion
               
            }

            if (type.ToLower().StartsWith("satiligacikar"))
            {
                if (type.Split(' ').Count() <= 1)
                {
                    API.sendChatMessageToPlayer(sender, "/ev satiligacikar ~y~[fiyat].");
                    return;
                }
                foreach (var item in db_Houses.CurrentHousesDict.Values)
                {
                    if (sender.dimension == item.EntranceDimension && Vector3.Distance(sender.position, item.EntrancePosition) < 2)
                    {
                        if (item.OwnerSocialClubName == sender.socialClubName)
                        {
                            int price = Convert.ToInt32(type.Split(' ')[1]);
                            item.IsSelling = true;
                            item.Price = price;
                            db_Houses.SaveChanges();
                            API.sendChatMessageToPlayer(sender, "~g~Eviniz " + price + "$ fiyatına satılığa çıkarıldı.");
                        }
                        else
                        {
                            API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bu ev size ait değil!");
                        }
                        return;
                    }
                }

            }



        }

        [Command("evlerim")]
        public void MyHouses(Client sender)
        {
            List<string> names = new List<string>();
            List<int> IDs = new List<int>();
            foreach (var itemHouse in db_Houses.GetAllCurrents())
            {
                if (itemHouse.OwnerSocialClubName == sender.socialClubName)
                {
                    names.Add(itemHouse.Name);
                    IDs.Add(itemHouse.HouseId);
                }
            }
            if (names.Count == 0)
            {
                API.shared.sendNotificationToPlayer(sender, "Henüz bir eviniz yok."); return;
            }
            Clients.ClientManager.ShowHousesToPlayer(sender, names, IDs);
            //Clients.ClientManager.ShowCustomMenu(sender, names, null, IDs, "EVLER", "Sahip olduğunuz evler.", "return_house_selected");
        }
        public static void OnSelectedHouseInMenu(Client sender,int id)
        {
            var _house = db_Houses.GetHouse(id);
            if (_house!=null)
            {
                if (_house.OwnerSocialClubName == sender.socialClubName)
                {
                    Clients.ClientManager.UpdateWaypoint(sender, _house.EntrancePosition);
                    return;
                }
                API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bu ev size ait değil!");
                return;
            }

            API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Ev bulunamadı!");
        }

        public static bool SetHouseOwner(int houseId, string OwnerSocialClubName)
        {
            var house = db_Houses.GetHouse(houseId);
            if (house!=null)
            {
                house.OwnerSocialClubName = OwnerSocialClubName;
                house.IsSelling = false;
                house.IsLocked = true;
                db_Houses.UpdateHouse(house);
                return true;
            }else
            {
                return false;
            }
        }
    }
}
