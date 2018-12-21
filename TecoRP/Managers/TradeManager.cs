using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared.Math;
using TecoRP.Database;
using TecoRP.Models;

namespace TecoRP.Managers
{
    public class TradeManager : Script
    {
        public const string SELL_KEY = "TradeModel";
        [Command("sat", "/sat [eşya/araç/ev/metalparça] [Satılacak Obje ID / Miktar] ")]
        public static void Sell(Client sender, string type, int sellingObjId)
        {
            if ("eşya".StartsWith(type.ToLower()) || "esya".StartsWith(type.ToLower()))
            {
                API.shared.sendChatMessageToPlayer(sender, "~b~[?] İPUCU: ~s~Eşya satmak için envanterinizden eşyanın üzerine gelip ~y~'B'~s~ tuşuna basın.");
                return;
            }
            SellType _sellType;
            if (type.ToLower() == "arac" || type.ToLower() == "araç" || type.ToLower() == "araba")
            {
                #region Arac

                _sellType = SellType.vehicle;
                var _vehicle = db_Vehicles.GetVehicle(sellingObjId);
                if (_vehicle != null)
                {
                    if (_vehicle.OwnerSocialClubName == sender.socialClubName)
                    {
                        List<string> names = new List<string>();
                        List<int> IDs = new List<int>();

                        foreach (var itemPlayer in API.shared.getAllPlayers())
                        {
                            if (itemPlayer == sender) continue;
                            if (Vector3.Distance(itemPlayer.position, sender.position) < 4)
                            {
                                names.Add(db_Players.GetPlayerCharacterName(itemPlayer));
                                IDs.Add(API.shared.getEntityData(itemPlayer, "ID"));
                                //API.shared.consoleOutput(IDs.LastOrDefault().ToString());
                            }
                        }
                        if (names.Count == 0)
                        {
                            API.shared.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Etrafınızda satış yapabileceğiniz kimse yok.");
                        }
                        Clients.ClientManager.SellSelectorMenu(sender, names, IDs, _sellType, (int)_vehicle.VehicleId);
                        return;
                    }
                    else
                    {
                        API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Kendinize ait olmayan bir aracı satamazsınız.");
                    }
                }
                else
                {
                    API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Araç bulunamadı.");
                }
                #endregion
            }
            else
            if ("ev".StartsWith(type.ToLower()))
            {
                #region Ev
                _sellType = SellType.house;
                var _house = db_Houses.GetHouse(sellingObjId);
                if (_house != null)
                {
                    if (_house.OwnerSocialClubName == sender.socialClubName)
                    {
                        List<string> names = new List<string>();
                        List<int> IDs = new List<int>();

                        foreach (var itemPlayer in API.shared.getAllPlayers())
                        {
                            if (itemPlayer == sender) continue;
                            if (Vector3.Distance(itemPlayer.position, sender.position) < 4)
                            {
                                names.Add(db_Players.GetPlayerCharacterName(itemPlayer));
                                IDs.Add(API.shared.getEntityData(itemPlayer, "ID"));
                            }
                        }
                        if (names.Count == 0)
                        {
                            API.shared.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Etrafınızda satış yapabileceğiniz kimse yok.");
                        }
                        Clients.ClientManager.SellSelectorMenu(sender, names, IDs, _sellType, (int)_house.HouseId);
                        return;
                    }
                    else
                    {
                        API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Size ait olmayan bir evi satamazsınız.");
                    }
                }
                else
                {
                    API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Satılacak ev bulunamadı.");
                }
                #endregion
            }
            else
            if ("metalparça".StartsWith(type.ToLower()) || "metalparca".StartsWith(type.ToLower()))
            {
                #region MetalParca
                _sellType = SellType.metalparts;
                if (InventoryManager.IsEnoughMetalParts(sender, sellingObjId))
                {
                    List<string> names = new List<string>();
                    List<int> IDs = new List<int>();

                    foreach (var itemPlayer in API.shared.getAllPlayers())
                    {
                        if (itemPlayer == sender) continue;
                        if (Vector3.Distance(itemPlayer.position, sender.position) < 4)
                        {
                            names.Add(db_Players.GetPlayerCharacterName(itemPlayer));
                            IDs.Add(API.shared.getEntityData(itemPlayer, "ID"));
                        }
                    }
                    if (names.Count == 0)
                    {
                        API.shared.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Etrafınızda satış yapabileceğiniz kimse yok.");
                    }
                    Clients.ClientManager.SellSelectorMenu(sender, names, IDs, _sellType, sellingObjId);
                    return;
                }
                else
                    API.shared.sendChatMessageToPlayer(sender, $"~r~UYARI: ~s~Üzerinizde ~r~{sellingObjId}~s~ metal parça bulunmuyor. Yalnızca ~y~{InventoryManager.GetPlayerMetalParts(sender)} metal parçanız var.");

                #endregion  }
            }
        }
        public static void PlayerSelectedSell(Client sender, SellType _type, int sellingObjId, int targetplayerId, int price)
        {
            var _player = db_Players.FindPlayerById(targetplayerId);
            if (_player == null) { return; }

            switch (_type)
            {
                case SellType.vehicle:
                    TradeSellModel _tradeModel = new Models.TradeSellModel
                    {
                        BuyerSocialClubID = _player.socialClubName,
                        OfferedPrice = price,
                        SellerSocialClubID = sender.socialClubName,
                        SellingObjId = sellingObjId,
                        Type = _type
                    };

                    API.shared.setEntityData(_player, SELL_KEY, _tradeModel);
                    API.shared.sendChatMessageToPlayer(sender, "~y~Teklifiniz iletildi.");
                    API.shared.sendChatMessageToPlayer(_player, $"{db_Players.GetPlayerCharacterName(sender)}~y~ alı kişi size {db_Vehicles.GetVehicle(_tradeModel.SellingObjId).VehicleModelId} model aracı ~s~{_tradeModel.OfferedPrice}$ ~y~ ücret karşılığında satmayı öneriyor.\n ~y~((/onayla))~s~ yazarak teklifi kabul edebilirsiniz..");
                    break;
                case SellType.house:
                    TradeSellModel _tradeModelHouse = new Models.TradeSellModel
                    {
                        BuyerSocialClubID = _player.socialClubName,
                        OfferedPrice = price,
                        SellerSocialClubID = sender.socialClubName,
                        SellingObjId = sellingObjId,
                        Type = _type
                    };

                    API.shared.setEntityData(_player, SELL_KEY, _tradeModelHouse);
                    API.shared.sendChatMessageToPlayer(sender, "~y~Teklifiniz iletildi.");
                    API.shared.sendChatMessageToPlayer(_player, $"{db_Players.GetPlayerCharacterName(sender)}~y~ alı kişi size {db_Houses.GetHouse(_tradeModelHouse.SellingObjId).Name} adlı evi ~s~{_tradeModelHouse.OfferedPrice}$ ~y~ ücret karşılığında satmayı öneriyor.\n ~y~((/onayla))~s~ yazarak teklifi kabul edebilirsiniz..");

                    break;
                case SellType.metalparts:
                    TradeSellModel _tradeModelMetal = new Models.TradeSellModel
                    {
                        BuyerSocialClubID = _player.socialClubName,
                        OfferedPrice = price,
                        SellerSocialClubID = sender.socialClubName,
                        SellingObjId = sellingObjId,
                        Type = _type
                    };
                    API.shared.setEntityData(_player, SELL_KEY, _tradeModelMetal);
                    API.shared.sendChatMessageToPlayer(sender, "~y~Teklifiniz iletildi.");
                    API.shared.sendChatMessageToPlayer(_player, $"{db_Players.GetPlayerCharacterName(sender)}~y~ alı kişi size {_tradeModelMetal.SellingObjId} adet metal parçayı ~s~{_tradeModelMetal.OfferedPrice}$ ~y~ ücret karşılığında satmayı öneriyor.\n ~y~((/onayla))~s~ yazarak teklifi kabul edebilirsiniz..");


                    break;
                default:
                    break;
            }
        }
        [Command("onayla")]
        public void AcceptSellTrade(Client sender)
        {
            if (API.shared.hasEntityData(sender, SELL_KEY))
            {
                TradeSellModel _trade = (TradeSellModel)API.shared.getEntityData(sender, SELL_KEY);
                if (!InventoryManager.IsEnoughMoney(sender, _trade.OfferedPrice)) { API.shared.sendChatMessageToPlayer(sender, $"~r~UYARI: ~s~Bu alışveriş için en az ~r~{_trade.OfferedPrice}$ ~s~paranız olmalı."); API.shared.resetEntityData(sender, SELL_KEY); return; }

                switch (_trade.Type)
                {
                    case SellType.vehicle:
                        #region vehicle
                        var sellerPlayer = db_Players.IsPlayerOnline(_trade.SellerSocialClubID);
                        if (sellerPlayer != null)
                        {
                            if (VehicleManager.SetVehicleOwner(_trade.SellingObjId, sender.socialClubName))
                            {
                                InventoryManager.AddMoneyToPlayer(sellerPlayer, _trade.OfferedPrice);
                                InventoryManager.AddMoneyToPlayer(sender, -1 * _trade.OfferedPrice);
                                API.shared.sendChatMessageToPlayer(sellerPlayer, "~y~Alışveriş başarılı bir şekilde gerçekleşti.");
                                API.shared.sendChatMessageToPlayer(sender, "~y~Alışveriş başarılı bir şekilde gerçekleşti. ~s~((/araclarim)) ~y~yazarak yeni aracınızı görebilirsiniz.");
                            }
                            else
                            {
                                API.shared.sendChatMessageToPlayer(sender, "~y~Satılacak araç bulunamadı!");
                                API.shared.sendChatMessageToPlayer(sellerPlayer, "~y~Satılacak araç bulunamadı!");
                            }
                        }
                        else
                        {
                            API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Satışı yapan oyuncu oyunda değil.");
                            API.shared.resetEntityData(sender, SELL_KEY);
                        }
                        #endregion
                        break;
                    case SellType.house:
                        #region house
                        var hSellerPlayer = db_Players.IsPlayerOnline(_trade.SellerSocialClubID);
                        if (hSellerPlayer != null)
                        {
                            if (HouseManager.SetHouseOwner(_trade.SellingObjId, _trade.BuyerSocialClubID))
                            {
                                InventoryManager.AddMoneyToPlayer(hSellerPlayer, _trade.OfferedPrice);
                                InventoryManager.AddMoneyToPlayer(sender, -1 * _trade.OfferedPrice);
                                API.shared.sendChatMessageToPlayer(hSellerPlayer, "~y~Alışveriş başarılı bir şekilde gerçekleşti.");
                                API.shared.sendChatMessageToPlayer(sender, "~y~Alışveriş başarılı bir şekilde gerçekleşti.");
                            }
                            else
                            {
                                API.shared.sendChatMessageToPlayer(sender, "~y~Satılacak ev bulunamadı!");
                                API.shared.sendChatMessageToPlayer(hSellerPlayer, "~y~Satılacak ev bulunamadı!");
                            }
                        }
                        else
                        {
                            API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Satışı yapan oyuncu oyunda değil.");
                            API.shared.resetEntityData(sender, SELL_KEY);
                        }
                        #endregion
                        break;
                    case SellType.metalparts:
                        #region metalpart
                        var mSellerPlayer = db_Players.IsPlayerOnline(_trade.SellerSocialClubID);
                        if (mSellerPlayer != null)
                        {
                            InventoryManager.AddMetalPartsToPlayer(sender, _trade.SellingObjId);
                            InventoryManager.AddMetalPartsToPlayer(mSellerPlayer,-1 * _trade.SellingObjId);
                            InventoryManager.AddMoneyToPlayer(sender, -1 * _trade.OfferedPrice);
                            InventoryManager.AddMoneyToPlayer(mSellerPlayer, _trade.OfferedPrice);
                            API.shared.sendChatMessageToPlayer(mSellerPlayer, "~y~Alışveriş başarılı bir şekilde gerçekleşti.");
                            API.shared.sendChatMessageToPlayer(sender, "~y~Alışveriş başarılı bir şekilde gerçekleşti.");
                        }
                        else
                        {
                            API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Satışı yapan oyuncu oyunda değil.");
                            API.shared.resetEntityData(sender, SELL_KEY);
                        }
                        #endregion
                        break;
                    default:
                        break;
                }
            }
        }
    }
}
