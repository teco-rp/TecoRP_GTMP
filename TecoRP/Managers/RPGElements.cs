using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Timers;
using TecoRP.Database;
using GrandTheftMultiplayer.Server.Elements;
using TecoRP.Users;

namespace TecoRP.Managers
{
    public class RPGElements : Script
    {

        public static int TaxDelay = 60000;
        public static float SalaryMultiplier = 1;
        public RPGElements()
        {
            API.onPlayerDeath += API_onPlayerDeath;
            var taxes = db_Vehicles.GetVehicleTaxes();
            Task.Run(async () =>
            {
                API.consoleOutput("Player Task Başladı.");
                baslangic:
                try
                {
                    foreach (var itemPlayer in API.getAllPlayers())
                    {
                        if (API.shared.getEntityData(itemPlayer, "LOGGED_IN") != null && API.shared.getEntityData(itemPlayer, "LOGGED_IN") == true)
                        {
                            InventoryManager.CheckPlayerAttachedItems(itemPlayer);
                            float _Hunger = Convert.ToSingle(API.getEntityData(itemPlayer, "Hunger"));
                            float _Thirsty = Convert.ToSingle(API.getEntityData(itemPlayer, "Thirsty"));
                            int _playingMinutes = Convert.ToInt32(API.getEntityData(itemPlayer, "playingMinutes"));
                            _playingMinutes++;
                            if (_playingMinutes % 60 == 0)
                            {
                                #region Salary
                                int _salary = FactionManager.GetPlayerFaction(itemPlayer) > 0 ? (int)(((((_playingMinutes / 60) * 10) + 750) * SalaryMultiplier) * (FactionManager.GetPlayerRank(itemPlayer).RankLevel * 0.1f + 1)) : (int)((((_playingMinutes / 60) * 10) + 750) * SalaryMultiplier);
                                API.sendChatMessageToPlayer(itemPlayer, "~g~Maaşınız " + _salary + " hesabınıza eklendi.");
                                InventoryManager.AddMoneyToPlayer(itemPlayer, _salary);
                                #endregion
                            }
                            API.setEntityData(itemPlayer, "playingMinutes", _playingMinutes);

                            if (API.getEntityData(itemPlayer, "Jailed") == false)
                            {
                                #region HungerThirsty
                                if (_Thirsty <= 5 || _Hunger <= 5)
                                {
                                    itemPlayer.health -= 19;
                                }
                                else
                                {
                                    API.setEntityData(itemPlayer, "Hunger", _Hunger - 1);
                                    if (_playingMinutes % 2 == 0) { API.setEntityData(itemPlayer, "Thirsty", _Thirsty - 1f); }
                                }
                                API.triggerClientEvent(itemPlayer, "update_hungerthirsty", _Hunger, _Thirsty);
                                #endregion
                            }
                            else
                            {
                                //JAILED SITUATION
                                #region JailTime
                                int? jailTime = API.getEntityData(itemPlayer, "JailedTime");
                                if (jailTime != null)
                                {
                                    if (jailTime <= 0)
                                    {
                                        itemPlayer.position = db_Arrests.currentArrests.Item1.FirstOrDefault().Position;
                                        itemPlayer.dimension = db_Arrests.currentArrests.Item1.FirstOrDefault().Dimension;
                                        API.setEntityData(itemPlayer, "Jailed", false);
                                    }
                                    else
                                    {
                                        jailTime--;
                                        API.setEntityData(itemPlayer, "JailedTime", jailTime);
                                    }
                                }
                                #endregion
                            }
                            db_Accounts.SavePlayerAccount(itemPlayer);
                        }
                    }

                }
                catch (Exception ex)
                {
                    API.shared.consoleOutput(LogCat.Fatal, ex.ToString());
                    goto Outside;
                }             

                Outside:
                await Task.Delay(60000);
                goto baslangic;
            });

            Task.Run(async () =>
            {
                API.consoleOutput("Vehicle Task Başladı.");
                Start:
                #region Weather&Time
                API.setTime(DateTime.Now.Hour, DateTime.Now.Minute);

                try
                {
                    if (DateTime.Now.Minute == 0)
                    {
                        Random rnd = new Random();
                        if (rnd.Next(0, 100) < 5) { API.setWeather(11); goto OutOfWeatherTime; }
                        if (rnd.Next(0, 100) < 7) { API.setWeather(4); goto OutOfWeatherTime; }
                        if (rnd.Next(0, 100) < 7) { API.setWeather(5); goto OutOfWeatherTime; }
                        if (rnd.Next(0, 100) < 7) { API.setWeather(7); goto OutOfWeatherTime; }
                        if (rnd.Next(0, 100) < 8) { API.setWeather(6); goto OutOfWeatherTime; }
                        if (rnd.Next(0, 100) < 10) { API.setWeather(9); goto OutOfWeatherTime; }
                        if (rnd.Next(0, 100) < 10) { API.setWeather(10); goto OutOfWeatherTime; }
                        if (rnd.Next(0, 100) < 15) { API.setWeather(12); goto OutOfWeatherTime; }
                        if (rnd.Next(0, 100) < 15) { API.setWeather(8); goto OutOfWeatherTime; }
                        if (rnd.Next(0, 100) < 20) { API.setWeather(3); goto OutOfWeatherTime; }
                        if (rnd.Next(0, 100) < 25) { API.setWeather(1); goto OutOfWeatherTime; }
                        if (rnd.Next(0, 100) < 25) { API.setWeather(2); goto OutOfWeatherTime; }

                        API.setWeather(0);

                    }

                }
                catch (Exception ex)
                {
                    API.consoleOutput(LogCat.Warn, "Weather Error: " + ex.ToString());
                    goto OutOfWeatherTime;
                }

                #endregion

                OutOfWeatherTime:


                foreach (var itemVeh in db_Vehicles.GetAll())
                {
                    if (itemVeh.IsBlockedForTax == false && itemVeh.PastMinutes % 60 == 0)
                    {
                        if (itemVeh.JobId > 0 || itemVeh.FactionId > 0) { continue; }
                        var _vehTax = taxes.Find(x => x.VehicleName == itemVeh.VehicleModelId);
                        itemVeh.Tax += (_vehTax.TaxPerHour);
                        if (itemVeh.Tax >= _vehTax.MaxTax)
                        {
                            itemVeh.IsBlockedForTax = true;
                        }
                    }

                    if (!String.IsNullOrEmpty(itemVeh.RentedPlayerSocialClubId))
                    {
                        if ((DateTime.Now - itemVeh.RentedTime).Hours >= 5 && API.shared.getVehicleOccupants(itemVeh.VehicleOnMap).Length <= 0)
                        {
                            db_Vehicles.RemoveVehicle(itemVeh.VehicleId);
                        }
                    }

                    itemVeh.PastMinutes++;
                }
                db_Vehicles.SaveChanges();
                //API.consoleOutput("__________________________Minute Task____________________");
                await Task.Delay(60000);
                goto Start;
            });

            Task.Run(async () =>
            {
                foreach (var itemPlayer in db_Accounts.GetOfflineUserDatas())
                {
                    var _player = db_Accounts.IsPlayerOnline(itemPlayer.SocialClubName);
                    if (_player != null)
                    {
                        if (!String.IsNullOrEmpty(API.getEntityData(_player, "BankAccount")))
                        {
                            int _pastBankMinutes = API.getEntityData(_player, "PastBankMinutes");
                            if (_pastBankMinutes % 60 == 0)
                            {
                                int _bankMoney = API.getEntityData(_player, "BankMoney");
                                _bankMoney += (int)(_bankMoney * 0.01f);
                                _pastBankMinutes -= 60;
                                API.setEntityData(_player, "PastBankMinutes", _pastBankMinutes);
                                API.setEntityData(_player, "BankMoney", _bankMoney);
                            }
                        }
                    }
                    else
                    {
                        itemPlayer.PastBankMinutes++;
                        if (itemPlayer.PastBankMinutes % 60 == 0)
                        {
                            itemPlayer.BankMoney += (int)(itemPlayer.BankMoney * 0.01f);
                            itemPlayer.PastBankMinutes -= 60;
                        }
                        db_Accounts.SaveOfflineUserData(itemPlayer.SocialClubName, itemPlayer);
                    }
                }

                await Task.Delay(60000);
            });

        }

        private void API_onPlayerDeath(GrandTheftMultiplayer.Server.Elements.Client player, GrandTheftMultiplayer.Shared.NetHandle entityKiller, int weapon)
        {
            API.setEntityData(player, "Hunger", GetPlayerHunger(player) + 5);
            API.setEntityData(player, "Thirsty", GetPlayerThirsty(player) + 5);
            Clients.ClientManager.UpdateHungerAndThirsty(player, GetPlayerHunger(player), GetPlayerThirsty(player));
        }
        public static float GetPlayerHunger(Client sender)
        {
            return API.shared.getEntityData(sender, "Hunger");
        }
        public static float GetPlayerThirsty(Client sender)
        {
            return API.shared.getEntityData(sender, "Thirsty");
        }
    }
}
