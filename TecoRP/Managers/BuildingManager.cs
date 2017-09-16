using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared.Math;
using TecoRP.Database;
using TecoRP.Models;

namespace TecoRP.Managers
{

    //BUSINESS DEFAULT LOCATION
    // -1392, -482, 72
    public class BuildingManager : Script
    {
        [Command("bina")]
        public static void OnBuilding(Client sender)
        {
            var _building = db_Buildings.GetNearestBuilding(sender.position);
            if (Vector3.Distance(sender.position, _building.Position) < 3)
            {
                List<string> names = new List<string>();
                List<string> descs = new List<string>();
                foreach (var item in _building.Floors)
                {
                    switch (item.Type)
                    {
                        case Models.FloorType.House:
                            var _house = db_Houses.GetHouse(item.TypedObjectId);

                            if (_house.IsSelling)
                            {
                                if (_house.OwnerSocialClubName == sender.socialClubName)
                                {
                                    names.Add("~h~~g~" + _house.Name + (_house.IsLocked ? "" : "*"));
                                    descs.Add("(" + item.TypedObjectId.ToString() + ") ~s~Eviniz şu anda satılık. İstenen Fiyat:~g~ " + _house.Price + "$ ~s~İptal etmek için ~y~B~s~ tuşuna basabilirsiniz.");
                                }
                                else
                                {
                                    names.Add("~g~"+_house.Name);
                                    descs.Add("(" + item.TypedObjectId.ToString() + ") ~g~" + _house.Price + "$~s~'a satın almak için ~y~B~s~ tuşuna basın.");
                                }
                            }
                            else
                            {
                                if (_house.OwnerSocialClubName == sender.socialClubName)
                                {
                                    names.Add("~h~~y~" + _house.Name + (_house.IsLocked ? "" : "*"));
                                    descs.Add("(" + item.TypedObjectId.ToString() + ") Evinizi satılığa çıkarmak için ~y~X~s~ tuşuna basabilirsiniz.");
                                }
                                else
                                {
                                    names.Add( _house.Name);
                                    descs.Add("(" + item.TypedObjectId.ToString() + ")");
                                }


                            }

                            break;
                        case Models.FloorType.Business:
                            names.Add(db_Businesses.currentBusiness[item.TypedObjectId].BusinessName);
                            descs.Add(item.TypedObjectId.ToString());
                            break;
                        case Models.FloorType.Warehouse:
                            break;
                        default:
                            break;
                    }
                }

                Clients.ClientManager.OpenBuilding(sender, names, descs, _building.BuildingId,_building.BuildingName);
            }
        }
        public static void OnBuildingSelected(Client sender, int buildingId, int index)
        {
            var _building = db_Buildings.GetBuilding(buildingId);
            var floor = _building.Floors[index];
            if (!floor.IsLocked)
            {
                sender.position = floor.InteriorPosition;
                sender.dimension = floor.InteriorDimension;
            }else
            {
                API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bu daire kilitli!");
            }
           
        }
        public static void OnBuildingSellSelected(Client sender,int buildingId, int index,int price)
        {
            var _building = db_Buildings.GetBuilding(buildingId);
            var floor = _building.Floors[index];
            switch (floor.Type)
            {
                case Models.FloorType.House:
                    #region _house
                    var _house = db_Houses.GetHouse(floor.TypedObjectId);
                    if (_house.OwnerSocialClubName == sender.socialClubName)
                    {
                        if (_house.IsSelling)
                        {
                            _house.IsSelling = false;
                            API.shared.sendChatMessageToPlayer(sender, "~g~Eviniz satıştan çekildi. Artık satılık değil.");
                        }
                        else
                        {
                            _house.Price = price;
                            _house.IsSelling = true;
                        }
                        db_Houses.UpdateHouse(_house);
                        API.shared.sendChatMessageToPlayer(sender, "~g~Eviniz " + price + "$'a satılığa çıkarıldı. Yapılan satıştan 2% komisyon kesilecek.");
                    }
                    else
                    {
                        API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bu ev size ait değil!");
                    } 
                    #endregion
                    break;
                case Models.FloorType.Business:
                    break;
                case Models.FloorType.Warehouse:
                    break;
                default:
                    break;
            }
        }
        public static void OnBuildingBuySelected(Client sender, int buildingId,int index)
        {
            var _building = db_Buildings.GetBuilding(buildingId);
            var floor = _building.Floors[index];
            switch (floor.Type)
            {
                case Models.FloorType.House:
                    #region _house
                    var _house = db_Houses.GetHouse(floor.TypedObjectId);
                    if (_house.IsSelling)
                    {
                        #region CancelSelling
                        if (_house.OwnerSocialClubName == sender.socialClubName)
                        {
                            _house.IsSelling = false;
                            API.shared.sendChatMessageToPlayer(sender, "~g~Eviniz artık satılık değil!");

                            return;
                        }
                        #endregion

                        #region satinal
                        {
                            int money = API.shared.getEntityData(sender, "Money");
                            if (_house.Price <= money)
                            {

                                if (!String.IsNullOrEmpty(_house.OwnerSocialClubName))
                                {
                                    var sellerPlayer = db_Accounts.IsPlayerOnline(_house.OwnerSocialClubName);
                                    if (sellerPlayer != null)
                                    {
                                        #region payment
                                        if (InventoryManager.IsEnoughMoney(sender, _house.Price))
                                        {
                                            InventoryManager.AddMoneyToPlayer(sender, -1 * _house.Price);
                                            InventoryManager.AddMoneyToPlayerBank(sellerPlayer, (int)(_house.Price * 0.98));
                                        }
                                        else
                                                                      if (InventoryManager.IsEnougMoneyInBank(sender, _house.Price))
                                        {
                                            InventoryManager.AddMoneyToPlayerBank(sender, -1 * _house.Price);
                                            InventoryManager.AddMoneyToPlayerBank(sellerPlayer, (int)(_house.Price * 0.98));
                                        }
                                        else
                                        {
                                            API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Üzerinizde veya benka hesabınızda bu kadar para bulunmuyor.");
                                            return;
                                        } 
                                        #endregion

                                        _house.IsSelling = false;
                                        _house.OwnerSocialClubName = sender.socialClubName;
                                        API.shared.sendChatMessageToPlayer(sender, "~g~Tebrikler! ~s~Satışta olan eviniz başarıyla satıldı.");
                                        db_Houses.UpdateHouse(_house);
                                    }
                                    else
                                    {
                                        var sellerOfflinePlayer = db_Accounts.GetOfflineUserDatas(_house.OwnerSocialClubName);

                                        #region payment
                                        if (InventoryManager.IsEnoughMoney(sender, _house.Price))
                                        {
                                            InventoryManager.AddMoneyToPlayer(sender, -1 * _house.Price);
                                            InventoryManager.AddMoneyToOfflinePlayerBank(sellerOfflinePlayer.socialClubName, (int)(_house.Price * 0.98f));
                                        }
                                        else
                                                                 if (InventoryManager.IsEnougMoneyInBank(sender, _house.Price))
                                        {
                                            InventoryManager.AddMoneyToPlayerBank(sender, -1 * _house.Price);
                                            InventoryManager.AddMoneyToOfflinePlayerBank(sellerOfflinePlayer.socialClubName, (int)(_house.Price * 0.98f));
                                        }
                                        else
                                        {
                                            API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Üzerinizde veya benka hesabınızda bu kadar para bulunmuyor.");
                                            return;
                                        }

                                        #endregion

                                        _house.OwnerSocialClubName = sender.socialClubName;
                                        db_Accounts.SaveOfflineUserData(sellerOfflinePlayer.socialClubName, sellerOfflinePlayer);
                                        HouseMarkerColor hmc = new Models.HouseMarkerColor();
                                       // var _house = db_Houses.GetHouse(_house.HouseId);
                                       // _house.MarkerOnMap.color = new Color(hmc.NormalColor.Red, hmc.NormalColor.Green, hmc.NormalColor.Blue, 255);
                                        _house.IsSelling = false;
                                        db_Houses.UpdateHouse(_house);
                                    }
          
                                    return;
                                }
                                else
                                {
                                    _house.OwnerSocialClubName = sender.socialClubName;

                                    if (!_house.IsInBuilding)
                                    {
                                        HouseMarkerColor hmc = new Models.HouseMarkerColor();
                                        _house.MarkerOnMap.color = new Color(hmc.NormalColor.Red, hmc.NormalColor.Green, hmc.NormalColor.Blue, 255);
                                        _house.IsSelling = false; 
                                    }
                                    db_Houses.SaveChanges();
                                    money -= _house.Price;
                                    _house.IsSelling = false;
                                    API.shared.triggerClientEvent(sender, "update_money_display", money);
                                    API.shared.sendNotificationToPlayer(sender, "~g~Hayırlı olsun.\n~s~ Yeni bir ev satın aldınız. ");
                                }
                                db_Houses.UpdateHouse(_house);
                            }
                            else
                            {
                                API.shared.sendChatMessageToPlayer(sender, "~y~Bu evi almak için paranız en az " + _house.Price + " olmalı.");
                            }
                            return;
                        }

                        #endregion
                    }
                    #endregion
                    break;
                case Models.FloorType.Business:
                    break;
                case Models.FloorType.Warehouse:
                    break;
                default:
                    break;
            }
        }
        public static void onBuildingRingSelected(Client sender,int buildingId,int index)
        {
            var _building = db_Buildings.GetBuilding(buildingId);
            var floor = _building.Floors[index];
          
                    RPGManager.SendAllPlayersInRange(floor.InteriorPosition, 30, floor.InteriorDimension,
                        "~p~*****Kapı Zili Çalmaktadır.***"
                        );
            RPGManager rpgMgr = new Managers.RPGManager();
            rpgMgr.Me(sender, " adlı kişi elini düğmeye uzatır ve zili çalar.");  
        }
        public static void OnBuildingLockSelected(Client sender,int buildingId,int index)
        {
            var _building = db_Buildings.GetBuilding(buildingId);
            var floor = _building.Floors[index];
            if (floor.IsOwner(sender.socialClubName))
            {
                floor.IsLocked = !floor.IsLocked;
                RPGManager rpgMgr = new RPGManager();
                rpgMgr.Me(sender, (floor.IsLocked ? " adlı kişi dairesinni kilidini açar." : " adlı kişi dairesini kilitler"), 5);
            }
            else
            {
                API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bu daire size ait değil.");
            }
        }
        
    }
}
