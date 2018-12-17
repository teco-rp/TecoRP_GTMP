using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using TecoRP.Database;
using TecoRP.Jobs;
using GrandTheftMultiplayer.Server.Constant;
using TecoRP.Models;


//5 bagaj - 4 hood
namespace TecoRP.Managers
{
    public class VehicleManager : Base.EventMethodTriggerBase
    {
        RPGManager rpgMgr = new RPGManager();

        public VehicleManager()
        {

            API.onClientEventTrigger += API_onClientEventTrigger;

            API.onPlayerExitVehicle += API_onPlayerExitVehicle;
            API.onPlayerEnterVehicle += API_onPlayerEnterVehicle1;
            API.onPlayerEnterVehicle += API_onPlayerEnterVehicle;
            API.onVehicleDoorBreak += API_onVehicleDoorBreak;
            VehicleSpawn();
        }

        ~VehicleManager()
        {
            db_Vehicles.UpdateAndSaveChanges();
        }
        private void API_onVehicleDoorBreak(NetHandle entity, int index)
        {
            API.setVehicleLocked(entity, false);
        }

        public void VehicleSpawn()
        {
            //#region VehiclesSpawn
            //foreach (var item in dbVeh.GetAll().Items)
            //{
            //    VehiclesOnMap.Add(API.createVehicle(item.VehicleModelId, item.LastPosition, item.LastRotation, 1, 1));
            //    VehiclesOnMap.LastOrDefault().locked = item.IsLocked;
            //    API.setVehicleNumberPlate(VehicleManager.VehiclesOnMap.LastOrDefault(), item.Plate);
            //    API.setVehicleEngineStatus(VehiclesOnMap.LastOrDefault(), false);
            //    for (int j = 0; j < 69; j++)
            //    {
            //        VehiclesOnMap.LastOrDefault().setMod(j, item.Mods[j]);
            //    }

            //    //API.triggerClientEventForAll("turnoff_engine", VehiclesOnMap.LastOrDefault());
            //}
            //#endregion
        }
        private void API_onClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            if (eventName == "key_Y") { VehicleEngine(sender); }
            if (eventName == "key_L") { LockCar(sender); }


            if (eventName == "find_vehicle")
            {
                string vehicleStr = arguments.FirstOrDefault().ToString();

                var news = vehicleStr.Split('(').LastOrDefault().Replace(")", String.Empty).Replace(" ", String.Empty);
                API.consoleOutput(" SAYI " + news);
                FindMyVehicle(sender, Convert.ToInt64(news));
                return;
            }
            if (eventName == "return_vehicle_fuel")
            {
                try
                {
                    SaveVehicleFuel(sender, Convert.ToInt32(arguments[0]), (NetHandle)arguments[1]);
                    API.consoleOutput("VEHICLE FUEL RETURNED AS : " + arguments[0]);
                }
                catch (Exception ex)
                {
                    if (ex.GetType() != typeof(InvalidCastException))
                    {
                        API.consoleOutput(LogCat.Warn, ex.ToString());
                    }
                }
                return;
            }
            if (eventName == "return_save_fuel")
            {
                try
                {
                    SaveVehicleFuel(sender, Convert.ToSingle(arguments[0]));
                }
                catch (Exception ex)
                {
                    if (ex.GetType() != typeof(InvalidCastException))
                    {
                        API.consoleOutput(LogCat.Warn, ex.ToString());
                    }
                }
                return;
            }
            if (eventName == "vehicle_tax")
            {
                string saltId = arguments[0].ToString().Replace("LS-", String.Empty);
                try
                {
                    var _vehicle = db_Vehicles.GetVehicle(Convert.ToInt64(saltId));
                    if (_vehicle != null)
                    {
                        API.sendChatMessageToPlayer(sender, "~y~_____" + _vehicle.VehicleModelId.ToString() + "_____\n" +
                                     "~y~Vergi: ~s~" + _vehicle.Tax + "$~y~  |   Plaka: ~s~" + _vehicle.Plate
                                       );
                    }
                    else
                    {
                        API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Araç bulunamdı.");
                    }
                }
                catch (Exception ex)
                {
                    if (ex.GetType() == typeof(FormatException))
                    {
                        API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Araç bulunamadı. Doğru plaka yazım şekli ~h~(( LS-19 ))");
                    }
                }
                return;
            }
            else
            if (eventName == "vehicle_tax_pay")
            {
                string saltId = arguments[0].ToString().Replace("LS-", String.Empty);
                try
                {
                    var _vehicle = db_Vehicles.GetVehicle(Convert.ToInt64(saltId));
                    if (_vehicle != null)
                    {
                        int bankMoney = API.getEntityData(sender, "BankMoney");
                        if (bankMoney >= _vehicle.Tax)
                        {
                            bankMoney -= (int)_vehicle.Tax;
                            API.sendChatMessageToPlayer(sender, "~y~Aracınızın vergisi ödendi.");
                            API.sendNotificationToPlayer(sender, "Banka Hesabı:\n~r~-" + _vehicle.Tax);
                            _vehicle.Tax = 0;
                            API.setEntityData(sender, "BankMoney", bankMoney);
                            _vehicle.IsBlockedForTax = false;
                            db_Vehicles.SaveChanges();
                        }
                        else
                        {
                            API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Bu vergiyi ödemek için hesabınızda yeterli ücret yok. (Gereken:~g~" + _vehicle.Tax + "~s~ Hesabınızda:~r~ " + bankMoney + " ~s~)");
                        }
                    }
                    else
                    {
                        API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Ara. bulunamadı.");
                    }
                }
                catch (Exception ex)
                {
                    if (ex.GetType() == typeof(FormatException))
                    {
                        API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Araç bulunamadı. Doğru plaka yazım şekli ~h~(( LS-19 ))");
                    }
                }
            }
        }

        private void API_onPlayerEnterVehicle(Client player, GrandTheftMultiplayer.Shared.NetHandle vehicle, int seat)
        {
            API.shared.consoleOutput($"Player {player.socialClubName} has entered {vehicle} vehicle");

            //var vehIndex = SaleVehicleManager.FindIndex(vehicle);
            //if (vehIndex != -1)
            //{
            //    var veh = db_SaleVehicles.currentSaleVehicleList.Items[vehIndex];

            //    if (veh.Interaction.Rent)
            //        API.sendChatMessageToPlayer(player, $"~b~Bu araç ${veh.Price.Rent} ödeyerek kiralanabilir. ~h~/arac kirala");

            //    if (veh.Interaction.Buy && veh.Price.Buy > 0)
            //        API.sendChatMessageToPlayer(player, $"~b~Bu araç ${veh.Price.Buy} ödeyerek satın alınabilir. ~h~/arac satinal");
            //}
        }
        private void API_onPlayerExitVehicle(Client player, NetHandle vehicle, int seat)
        {
            player.seatbelt = false;
        }

        private void API_onPlayerEnterVehicle1(Client player, NetHandle vehicle, int seat)
        {
            var _vehicle = db_Vehicles.FindVehicle(vehicle);
            API.shared.consoleOutput($"Player {player.socialClubName} has entered {vehicle} vehicle");
            if (_vehicle == null)
                return;
            if (API.getVehicleClass(_vehicle.VehicleModelId) == 13) { return; }

            if (_vehicle != null)
            {

                //API.consoleOutput("Vehicle JOB: " + db_Vehicles.GetAll()[_Index].JobId);
                if (_vehicle.JobId <= 0)
                {
                    if (player.vehicleSeat == -1)
                    {
                        API.shared.consoleOutput("Fuel: " + _vehicle.Fuel);
                        API.triggerClientEvent(player, "show_vehicle_fuel", _vehicle.Fuel);
                    }
                    else
                    if (player.vehicleSeat == 0)
                    {
                        API.shared.consoleOutput("Fuel: " + _vehicle.Fuel);
                        API.delay(3500, true, () =>
                        {
                            if (player.vehicleSeat == -1)
                            {
                                API.triggerClientEvent(player, "show_vehicle_fuel", _vehicle.Fuel);
                            }

                        });
                    }
                }


            }
            else
            {
                API.consoleOutput("Araç bulunamadı.");
            }
        }


        [Command("c", "/c - Gives to you your coordinates (x, | y | z)")]
        public void coordinate(Client sender)
        {
            API.sendChatMessageToPlayer(sender, sender.position.X + " | " + sender.position.Y + " | " + sender.position.Z + "  __ " + sender.rotation.X + " | " + sender.rotation.Y + " | " + sender.rotation.Z);
        }
        [Command("park", "/park")]
        public void ParkVehicle(Client sender)
        {
            if (sender.isInVehicle)
            {
                var _vehicle = db_Vehicles.FindNearestVehicle(sender.position);

                if (db_Vehicles.IsPlayersVehicleById(sender, _vehicle.VehicleId) || API.getEntityData(sender, "AdminLevel") > 0)
                {
                    _vehicle.LastPosition = _vehicle.VehicleOnMap.position + new Vector3(0, 0, 1);
                    _vehicle.LastRotation = _vehicle.VehicleOnMap.rotation;
                    _vehicle.Color1 = _vehicle.VehicleOnMap.primaryColor;
                    _vehicle.Color2 = _vehicle.VehicleOnMap.secondaryColor;
                    _vehicle.IsLocked = _vehicle.VehicleOnMap.locked;
                    db_Vehicles.SaveChanges();
                    API.sendChatMessageToPlayer(sender, "~y~ Aracınız park edildi.");
                    return;
                }
                else
                {
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bu araç size ait değil.");
                    return;
                }
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Aracınızın içinde olmalısınız.");
            }
        }
        [Command("pl")]
        public void Player(Client sender)
        {
            API.sendChatMessageToPlayer(sender, "~y~F: " + API.getEntityData(sender, "FactionId") + " J: " + API.getEntityData(sender, "JobId") + " Dim: " + sender.dimension);
        }
        [Command("motor", "/motor")]
        public void VehicleEngine(Client sender)
        {
            try
            {
                if (sender.isInVehicle)
                {
                    if (API.getPlayerVehicleSeat(sender) == -1)
                    {
                        var _vehicle = db_Vehicles.FindVehicle(sender.vehicle.handle);
                        if (_vehicle == null) return;
                        if (API.getVehicleClass(_vehicle.VehicleModelId) == 13) return;
                        if (_vehicle.IsBlockedForTax) { API.sendChatMessageToPlayer(sender, "~s~UYARI: ~s~Aracınız, vergisi ödenmediğinden mühürlenmiştir."); return; }
                        if (db_Vehicles.IsPlayersVehicleById(sender, _vehicle.VehicleId)
                            ||
                            (API.getEntityData(sender.vehicle, "JOB_VEHICLE_OWNERID") == API.getEntityData(sender, "ID") || (_vehicle.FactionId == API.getEntityData(sender, "FactionId") && _vehicle.FactionId > 0))
                            ||
                            (_vehicle.FactionId == 0 && String.IsNullOrEmpty(_vehicle.OwnerSocialClubName))
                            ||
                            (API.getEntityData(sender.vehicle, LicenseManager.ON_LICENSE_EXAM_OWNER) == API.getEntityData(sender, "ID"))
                            ||
                            _vehicle.RentedPlayerSocialClubId == sender.socialClubName
                            )
                        {
                            if (_vehicle.Fuel > 0)
                            {
                                API.setVehicleEngineStatus(_vehicle.VehicleOnMap, !_vehicle.VehicleOnMap.engineStatus);
                                rpgMgr.Me(sender, (_vehicle.VehicleOnMap.engineStatus ? $" anahtarı aracın kontağına sokar ve saat yönünde çevirir." : " kontaktaki anahtarı saat yönünün tersinde çevirir ve motoru durdurur."));
                                if (!_vehicle.VehicleOnMap.engineStatus) { SaveVehicleFuel(sender); }
                                return;
                            }
                            else
                            {
                                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bu aracın benzini yok.");
                            }
                        }

                        else
                        {
                            API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bu aracı çalıştıramazsınız.");
                            return;
                        }

                    }
                    else
                    {
                        API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Aracınızın sürücü koltuğunda olmalısınız.");
                    }
                }
                else
                {
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Aracınızın içinde olmalısınız.");
                }
            }
            catch (Exception)
            {
                return;
            }
        }
        [Command("kilit", "/kilit")]
        public void LockCar(Client sender)
        {
            foreach (var itemVeh in db_Vehicles.GetAll())
            {
                if (Vector3.Distance(sender.position, itemVeh.VehicleOnMap.position) < 10 && (itemVeh.OwnerSocialClubName == sender.socialClubName || itemVeh.FactionId == API.getEntityData(sender, "FactionId") || API.getEntityData(itemVeh.VehicleOnMap, Job_TirManager.JOB_VEHICLE) == API.getEntityData(sender, "ID") || itemVeh.RentedPlayerSocialClubId == sender.socialClubName))
                {
                    if (itemVeh.VehicleOnMap.isDoorBroken(0)) { return; }
                    itemVeh.VehicleOnMap.locked = !itemVeh.VehicleOnMap.locked;
                    itemVeh.IsLocked = itemVeh.VehicleOnMap.locked;
                    rpgMgr.PlayAudio(itemVeh.VehicleOnMap.position, "CarLock.ogg", 15);
                    //rpgMgr.PlaySound(itemVeh.VehicleOnMap.position, "Click", "DLC_HEIST_HACKING_SNAKE_SOUNDS", 15);
                    rpgMgr.Me(sender, " " + itemVeh.VehicleOnMap.displayName + " model" + (itemVeh.VehicleOnMap.locked ? " aracını kilitler." : " aracının kilidini açar."));
                    return;
                }
            }
        }
        [Command("aracimibul", "/aracimibul ~y~[id]")]
        public void FindMyVehicle(Client sender, long _vehId)
        {
            var _vehicle = db_Vehicles.GetVehicle(_vehId);

            if (_vehicle.OwnerSocialClubName == sender.socialClubName)
            {
                API.triggerClientEvent(sender, "update_waypoint", _vehicle.VehicleOnMap.position.X, _vehicle.VehicleOnMap.position.Y);

            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bu araç size ait değil.");
            }
        }
        [Command("araclarim", "/araclarim")]
        public void MyVehicles(Client sender)
        {
            List<string> vehList = new List<string>();

            foreach (var item in db_Vehicles.GetPlayerVehicles(sender))
            {
                vehList.Add(item.VehicleModelId.ToString() + " (" + item.VehicleId + ")");
                API.consoleOutput("" + item.VehicleModelId.ToString() + " (" + item.VehicleId + ")");
            }

            if (vehList.Count > 0)
            {
                API.triggerClientEvent(sender, "my_vehicles",
             vehList.Count > 0 ? vehList[0] : "",
             vehList.Count > 1 ? vehList[1] : "",
             vehList.Count > 2 ? vehList[2] : "",
             vehList.Count > 3 ? vehList[3] : "",
             vehList.Count > 4 ? vehList[4] : ""
             );
            }
            else
            {
                API.sendNotificationToPlayer(sender, "Henüz bir aracınız yok.", true);
                rpgMgr.PlaySound(sender.position, "ERROR", "HUD_AMMO_SHOP_SOUNDSET", 1);
            }

            //API.triggerClientEvent(sender, "my_vehicles",vehList);
            //string strVehList = String.Empty;

            //foreach (var item in dbVeh.GetPlayerVehicles(sender))
            //{
            //    strVehList += " \n~w~" + item.VehicleId + " ~b~" + item.VehicleModelId.ToString();
            //}
            //API.sendChatMessageToPlayer(sender, strVehList);
        }
        [Command("ekemer", "/ekemer -Emniyet Kemerini takar")]
        public void Belt(Client sender)
        {
            if (sender.isInVehicle)
            {
                sender.seatbelt = !sender.seatbelt;

                rpgMgr.Me(sender, sender.seatbelt ? " emniyet kemerini takar." : " emniyet kemerini çıkarır.");
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Araçta değilken emniyet kemerini takamazsınız.");
            }
        }

        [Command("vehid", "/vehid ~y~ Yakındaki aracın ID'sini gösterir.")]
        public void GetVehicleId(Client sender)
        {
            foreach (var itemVeh in db_Vehicles.GetAll())
            {
                if (Vector3.Distance(sender.position, itemVeh.VehicleOnMap.position) < 6)
                {
                    API.sendChatMessageToPlayer(sender, itemVeh.VehicleOnMap.displayName + " (" + itemVeh.VehicleId + ")  -  H: " + itemVeh.VehicleOnMap.health + " F: " + itemVeh.FactionId + " J:" + itemVeh.JobId);
                }
            }
        }
        [Command("kemerkontrol", "/kemerkontrol [Oyuncu(ID)]")]
        public void CheckBelt(Client sender, string identity)
        {
            foreach (var item in API.getAllPlayers())
            {
                if (API.getEntityData(item, "ID") == Convert.ToInt32(identity))
                {
                    if (item != sender)
                    {
                        if (Vector3.Distance(item.position, sender.position) < 5)
                        {
                            if (item.isInVehicle)
                            {
                                rpgMgr.Me(sender, " " + API.getEntityData(item, "CharacterName") + " adlı kişinin emniyet kemerini kontrol eder.");
                                API.sendChatMessageToPlayer(sender, " Emniyet Kemeri " + (item.seatbelt ? "~g~ Takılı." : "~r~Takılmamış."));
                                return;
                            }
                            else
                            {
                                API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~" + item.socialClubName + " adlı oyuncu araçta değil.");
                            }
                        }
                        else
                        {
                            API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~" + item.socialClubName + " adlı oyuncu çok uzakta.");
                        }
                    }
                    else
                    {
                        API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Kendi kemerinizi kontrol edemezsiniz.");
                    }
                }
            }
        }

        [Command("kapikir", "/kapikir")]
        public void BreakDoor(Client sender)
        {
            if (sender.currentWeapon != GrandTheftMultiplayer.Shared.WeaponHash.Crowbar) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bunun için bir levyeye ihtiyacınız var."); return; }

            var _vehicle = db_Vehicles.FindNearestVehicle(sender.position);

            for (int j = 0; j < 5; j++)
            {
                if (!API.isVehicleDoorBroken(_vehicle.VehicleOnMap, j) && API.isVehicleWindowBroken(_vehicle.VehicleOnMap, j))
                {
                    if (j == 0) { API.setVehicleLocked(_vehicle.VehicleOnMap, false); }
                    API.breakVehicleDoor(_vehicle.VehicleOnMap, j, true);
                    return;
                }
            }
            API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Kapıyı kırabilmek için önce kapının camını kırmalısınız.");
            return;

        }
        [Command("bagaj")]
        public void VehicleBaggage(Client sender)
        {
            if (!sender.isInVehicle)
            {
                var _vehicle = db_Vehicles.FindNearestVehicle(sender.position);

                API.shared.consoleOutput("Vehicle Class: " + API.shared.getVehicleClass(_vehicle.VehicleModelId));
                if (API.shared.getVehicleClass(_vehicle.VehicleModelId) == 13 || API.shared.getVehicleClass(_vehicle.VehicleModelId) == 8)
                {
                    API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bu aracın bagajı yok.");
                    return;
                }

                if (Vector3.Distance(sender.position, _vehicle.VehicleOnMap.position) < 4)
                {
                    if (!_vehicle.VehicleOnMap.locked)
                    {
                        if (!API.isVehicleDoorBroken(_vehicle.VehicleOnMap, 5))
                        {
                            if (API.getVehicleDoorState(_vehicle.VehicleOnMap, 5))
                            {
                                rpgMgr.Me(sender, range: 15, _action: " " + _vehicle.VehicleModelId + " model aracın bagajını kapatır.");
                                _vehicle.VehicleOnMap.closeDoor(5);
                            }
                            else
                            {
                                rpgMgr.Me(sender, range: 15, _action: " " + _vehicle.VehicleModelId + " model aracın bagajını açar.");
                                _vehicle.VehicleOnMap.openDoor(5);
                            }
                        }
                        else
                        {
                            API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Bu aracın bagajı yok.");
                        }
                        return;
                    }
                    else
                    {
                        API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bu araç kilitli."); return;
                    }
                }
                API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Bunu yapabilmek için bir aracın yakınında olmalısınız.");
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Arabadayken bunu yapamazsınız.");
            }
        }
        [Command("kaput")]
        public void VehicleHood(Client sender)
        {
            if (!sender.isInVehicle)
            {
                var _vehicle = db_Vehicles.FindNearestVehicle(sender.position);

                if (Vector3.Distance(sender.position, _vehicle.VehicleOnMap.position) < 4)
                {
                    if (!API.isVehicleDoorBroken(_vehicle.VehicleOnMap, 4))
                    {
                        if (!_vehicle.VehicleOnMap.locked)
                        {
                            if (API.getVehicleDoorState(_vehicle.VehicleOnMap, 4))
                            {
                                rpgMgr.Me(sender, range: 15, _action: " " + _vehicle.VehicleModelId + " model aracın kaputunu kapatır.");
                                _vehicle.VehicleOnMap.closeDoor(4);
                            }
                            else
                            {
                                rpgMgr.Me(sender, range: 15, _action: " " + _vehicle.VehicleModelId + " model aracın kaputunu açar.");
                                _vehicle.VehicleOnMap.openDoor(4);
                            }
                            return;
                        }
                        else
                        {
                            API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bu araç kilitli."); return;
                        }
                    }
                    else
                    {
                        API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Aracın kaputu yok veya kırılmış."); return;
                    }

                }
                API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Bunu yapabilmek için bir aracın yakınında olmalısınız.");
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Arabadayken bunu yapamazsınız.");
            }
        }
        [Command("benzin", "/benzin ~y~[fiyat/doldur/bidondandoldur]", GreedyArg = true)]
        public void Fuel(Client sender, string commandParam)
        {
            string[] splitted = commandParam.Split(' ');

            if (splitted.Length > 1)
            {
                float requestedFuel = Convert.ToSingle(splitted[1]);
                if ("doldur".StartsWith(splitted[0].ToLower()))
                {
                    #region doldur


                    var _vehicle = db_Vehicles.FindNearestVehicle(sender.position);
                    if (sender.isInVehicle) { API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Bunun için motoru kapayıp arabadan inmelisiniz."); return; }
                    if (Vector3.Distance(_vehicle.VehicleOnMap.position, sender.position) < 4 || sender.currentWeapon == WeaponHash.PetrolCan)
                    {
                        if (_vehicle.VehicleOnMap.engineStatus == false)
                        {
                            foreach (var itemGas in db_GasStations.CurrentGasStations.Item1)
                            {
                                if (Vector3.Distance(sender.position, itemGas.Position) < 2)
                                {
                                    //Models.Business business = null;
                                    //if (itemGas.BusinessId != null)
                                    //{
                                    //    business = db_Businesses.currentBusiness[(int)itemGas.BusinessId];
                                    //    if (business != null)
                                    //    {
                                    //        if (business.IsClosed)
                                    //        {
                                    //            API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bu işyeri şu anda hizmet veremiyor.");
                                    //            return;
                                    //        }
                                    //    }
                                    //}


                                    #region PetrolCan                                  

                                    if (sender.currentWeapon == WeaponHash.PetrolCan)
                                    {
                                        if (!InventoryManager.IsEnoughMoney(sender, (int)(requestedFuel * itemGas.PricePerUnit))) { API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Bu kadar alabilmek için paranız yetersiz."); return; }
                                        if (itemGas.MaxGasInStock < requestedFuel) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bu benzin istasyonunda yeterli benzin bulunmuyor."); return; }
                                        if (API.getPlayerWeaponAmmo(sender, WeaponHash.PetrolCan) + (int)requestedFuel > 50) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bidonunuz daha fazla benzin alamıyor."); return; }
                                        InventoryManager.AddMoneyToPlayer(sender, -1 * (int)(requestedFuel * itemGas.PricePerUnit));
                                        API.shared.setPlayerWeaponAmmo(sender, WeaponHash.PetrolCan, API.getPlayerWeaponAmmo(sender, WeaponHash.PetrolCan) + (int)requestedFuel);
                                        return;
                                    }


                                    #endregion



                                    if (_vehicle.Fuel + requestedFuel > 100) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bu araç daha fazla benzin alamaz. Boş alan:~y~" + (100 - _vehicle.Fuel) + "%"); return; }

                                    if (itemGas.GasInStock >= requestedFuel)
                                    {
                                        int money = API.getEntityData(sender, "Money");
                                        if (money >= requestedFuel * itemGas.PricePerUnit)
                                        {
                                            money -= Convert.ToInt32(requestedFuel * itemGas.PricePerUnit);
                                            _vehicle.Fuel += requestedFuel;
                                            try
                                            {
                                                //if (business != null && (business.VaultMoney + requestedFuel * itemGas.PricePerUnit) <= itemGas.MaxGasInStock)
                                                //{
                                                //    business.VaultMoney += (int)(requestedFuel * itemGas.PricePerUnit);
                                                //}
                                                //else
                                                //{
                                                //    business.VaultMoney = business.MaxVaultMoney;
                                                //}
                                            }
                                            catch (Exception)
                                            {

                                            }

                                            API.setEntityData(sender, "Money", money);
                                            API.triggerClientEvent(sender, "update_money_display", money);
                                            API.sendNotificationToPlayer(sender, "~r~-" + Convert.ToInt32(requestedFuel * itemGas.PricePerUnit) + "$");
                                            itemGas.GasInStock -= requestedFuel;
                                            db_GasStations.Update(itemGas);
                                        }
                                        else
                                        {
                                            API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~" + (int)requestedFuel + " litre alabilmek için " + (requestedFuel * itemGas.PricePerUnit) + "$' a ihtiyacınız var.");
                                        }

                                    }
                                    else
                                    {
                                        API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bu benzinlikte yeterli benzin stoğu kalmamış.");
                                    }
                                    return;
                                }
                            }
                        }
                        else
                        {
                            API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Bunun için önce aracın motorunu kapamalısınız.");

                        }
                    }
                    else
                    {
                        API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Aracınızı da getirmelisiniz."); return;
                    }
                    API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Yakınlarda benzin istasyonu yok.");
                    #endregion
                }
            }
            else
            {
                if (commandParam.ToLower().StartsWith("fiyat"))
                #region fiyat
                {
                    int _Index = 0, _NearestIndex = 0;
                    float lastDistance = float.MaxValue;
                    foreach (var item in db_GasStations.CurrentGasStations.Item1)
                    {
                        float dist = Vector3.Distance(sender.position, item.Position);
                        if (dist < lastDistance)
                        {
                            _NearestIndex = _Index;
                        }
                        lastDistance = dist;
                        _Index++;
                    }
                    API.sendChatMessageToPlayer(sender, "~y~Buradaki benzin fiyatı ~s~1% ~y~başına ~s~" + db_GasStations.CurrentGasStations.Item1[_NearestIndex].PricePerUnit + "$");
                }
                else
                       if (commandParam.ToLower().StartsWith("doldur"))
                {
                    API.sendChatMessageToPlayer(sender, "~s~/benzin doldur ~y~[litre]");
                }
                #endregion

                if ("bidondandoldur".StartsWith(commandParam.ToLower()))
                {
                    #region Bidondandoldur
                    if (sender.currentWeapon != WeaponHash.PetrolCan) { API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Elinize benzin bidonu almalısınız."); return; }
                    if (API.getPlayerWeaponAmmo(sender, WeaponHash.PetrolCan) <= 0) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Elinizdeki bidonda hiç benzin yok."); return; }
                    var _vehicle = db_Vehicles.FindNearestVehicle(sender.position);
                    if (Vector3.Distance(_vehicle.VehicleOnMap.position, sender.position) < 3)
                    {
                        if (!_vehicle.IsLocked)
                        {
                            if (_vehicle.VehicleOnMap.engineStatus == true) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Önce aracın motorunu kapatın."); return; }
                            //benzini doldurma işlemi
                            _vehicle.Fuel += API.getPlayerWeaponAmmo(sender, WeaponHash.PetrolCan);
                            API.setPlayerWeaponAmmo(sender, WeaponHash.PetrolCan, 0);
                            rpgMgr.Me(sender, $" adlı kişi elindeki benzin bidonu ile {_vehicle.VehicleModelId} model araca benzin doldurur.");

                        }
                        else
                        {
                            API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bu araç kilitli!");
                        }
                        return;
                    }
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Yakınınızda bir araç olmalı.");
                    #endregion
                }
            }

        }
        [Command("bagajabak")]
        public static void LookToBaggage(Client sender)
        {
            var _vehicle = db_Vehicles.FindNearestVehicle(sender.position);
            if (API.shared.getVehicleClass(_vehicle.VehicleModelId) == 13 || API.shared.getVehicleClass(_vehicle.VehicleModelId) == 8) { API.shared.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Bu aracın bagajı yok."); return; }
            if ((API.shared.getVehicleDoorState(_vehicle.VehicleOnMap, 5) || API.shared.isVehicleDoorBroken(_vehicle.VehicleOnMap, 5)) && Vector3.Distance(_vehicle.VehicleOnMap.position, sender.position) < 3.5f)
            {
                List<string> names = new List<string>();
                List<string> descs = new List<string>();
                for (int i = 0; i < _vehicle.MaxBaggageCount; i++)
                {
                    try
                    {
                        var gameItem = db_Items.GetItemById(_vehicle.BaggageItems.Items[i].ItemId);
                        names.Add(gameItem.Name);
                        descs.Add(gameItem.Description);
                    }
                    catch (ArgumentOutOfRangeException)
                    {
                        names.Add("--EŞYA KOY--");
                        descs.Add("Bu slota eşya koymak için seçin");
                        break;
                    }
                    catch (NullReferenceException ex)
                    {
                        API.shared.consoleOutput(LogCat.Error, ex.ToString());
                    }
                    catch (Exception ex)
                    {
                        API.shared.consoleOutput(LogCat.Error, ex?.ToString());
                    }
                }
                API.shared.consoleOutput($"{names.Count} baggage items found for {_vehicle.VehicleModelId}({_vehicle.VehicleId}) vehicle.");
                Clients.ClientManager.ShowStorageMenuToPlayer(sender, names, descs, _vehicle.VehicleModelId + " Bagajı", "vehicleB", (int)_vehicle.VehicleId);
            }
            else
            {
                API.shared.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Bunu yapabilmek için etrafınızda bagajı açık bir araç olmalı.");
            }
        }
        [Command("torpido")]
        public static void LookToTorpedo(Client sender)
        {
            if (sender.isInVehicle)
            {
                if (sender.vehicleSeat == -1 || sender.vehicleSeat == 0)
                {
                    
                    var _vehicle = db_Vehicles.FindVehicle(sender.vehicle.handle);

                    #region Listeleme
                    List<string> names = new List<string>();
                    List<string> descs = new List<string>();

                    API.shared.consoleOutput("COUNT: " + _vehicle.TorpedoItems.Items.Count);
                    for (int i = 0; i < _vehicle.MaxBaggageCount; i++)
                    {
                        try
                        {
                            var gameItem = db_Items.GetItemById(_vehicle.TorpedoItems.Items[i].ItemId);
                            names.Add(gameItem.Name);

                            descs.Add(gameItem.Description);
                            API.shared.consoleOutput("Name: " + gameItem.Name + " had been added.");
                            API.shared.consoleOutput("Desc: " + gameItem.Description + " had been added.");

                        }
                        catch (Exception ex)
                        {
                            if (ex.GetType() == typeof(ArgumentOutOfRangeException))
                            {
                                names.Add("--EKLE--");
                                descs.Add("Bu slota eşya koymak için seçin");
                                API.shared.consoleOutput("boş eklendi.");
                                break;
                            }
                            if (ex.GetType() == typeof(NullReferenceException))
                            {
                                API.shared.consoleOutput(LogCat.Warn, ex.ToString());
                                continue;
                            }
                            API.shared.consoleOutput(LogCat.Fatal, ex.ToString());
                        }
                    }
                    #endregion
                    Clients.ClientManager.ShowStorageMenuToPlayer(sender, names, descs, _vehicle.VehicleModelId + " Torpido Gözü", "vehicleT", Convert.ToInt32(_vehicle.VehicleId));
                }
                else
                {
                    API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bunun için ön koltukta oturmalısınız.");
                }
            }
            else
            {
                API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bunun için aracın içinde olmalısınız.");
            }
        }
        public static void OnVehicleBaggageItemSelected(Client sender, int vehId, int index)
        {
            var _vehicle = db_Vehicles.GetVehicle(vehId);
            if (_vehicle != null)
            {

                if (_vehicle.BaggageItems.Items.Count <= index)
                {

                    Clients.ClientManager.ShowInventoryForSelection(sender, "vehicleB", Convert.ToInt32(_vehicle.VehicleId));
                    return;

                }
                else
                {
                    var takenItem = _vehicle.BaggageItems.Items[index];
                    if (InventoryManager.AddItemToPlayerInventory(sender, takenItem))
                    {
                        _vehicle.BaggageItems.Items.RemoveAt(index);
                    }
                }
            }
            else
            {
                API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Araç bulunamadı.");
            }
        }
        public static void OnVehicleTorpedoItemSelected(Client sender, int vehId, int index)
        {
            var _vehicle = db_Vehicles.GetVehicle(vehId);
            if (_vehicle != null)
            {
                if (_vehicle.TorpedoItems.Items.Count <= index)
                {
                    //EŞYA KOYMA
                    API.shared.consoleOutput("TRIGGERED EŞYA KOYMA");
                    Clients.ClientManager.ShowInventoryForSelection(sender, "vehicleT", Convert.ToInt32(_vehicle.VehicleId));
                    return;
                }
                else
                {
                    //EŞYA ALMA
                    var takenItem = _vehicle.TorpedoItems.Items[index];
                    if (InventoryManager.AddItemToPlayerInventory(sender, takenItem))
                    {
                        _vehicle.TorpedoItems.Items.RemoveAt(index);
                    }
                    else
                    {
                        API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Envanterinizde daha fazla yer yok.");
                    }
                }
            }
            else
            {
                API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Araç bulunamadı.");
            }
        }
        public static bool PutVehicleBaggageItemByPlayer(Client sender, int vehId, int index)
        {
            var _clientItem = InventoryManager.GetItemFromPlayerInventory(sender, index);

            if (InventoryManager.RemoveItemFromPlayerInventoryByIndex(sender, index))
            {
                if (AddVehicleBaggageItem(_clientItem.Item2, Convert.ToInt64(vehId)))
                {

                    RPGManager rpgMgr = new Managers.RPGManager();
                    rpgMgr.Me(sender, " adlı kişi bagaja bir şeyler yerleştirir.", 8);
                    LookToBaggage(sender);
                }
                else
                {
                    API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bu eşyayı bagaja koyamazsınız.");
                    InventoryManager.AddItemToPlayerInventory(sender, _clientItem.Item2);
                    //API.shared.consoleOutput(LogCat.Fatal, "Bagaja eklenmedi ancak player'in envanterinden silindi.");
                }
            }
            else
            {
                API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bir hata oluştu.");
                API.shared.consoleOutput(LogCat.Fatal, "Bagaja eklenemedi.");
            }

            return true;
        }
        public static bool PutVehicleTorpedoItemByPlayer(Client sender, int vehId, int index)
        {
            var _clientItem = InventoryManager.GetItemFromPlayerInventory(sender, index).Item2;

            if (InventoryManager.RemoveItemFromPlayerInventory(sender, _clientItem))
            {
                if (AddVehicleTorpedoItem(_clientItem, vehId))
                {

                    LookToTorpedo(sender);
                    return true;
                }
                else
                {
                    InventoryManager.RemoveItemFromPlayerInventory(sender, _clientItem);
                    API.shared.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Bu eşyayı koyamazsınız.");
                    API.shared.consoleOutput(LogCat.Fatal, "Torpidoya eklendi ancak oyuncunun envanterinden silinmedi!");
                    return false;
                }
            }
            else
                return false;
        }
        public static bool AddVehicleBaggageItem(ClientItem _clientItem, long vehId)
        {
            var _vehicle = db_Vehicles.GetVehicle(vehId);
            if (_vehicle != null)
            {
                if (_vehicle.BaggageItems.Items.Count < _vehicle.MaxBaggageCount)
                {
                    _vehicle.BaggageItems.Items.Add(_clientItem);
                    db_Vehicles.SaveChanges();
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                API.shared.consoleOutput("Bagaja item eklenirken araç bulunamadı.");
                return false;
            }
        }
        public static bool AddVehicleBaggageItem(ClientItem _clientItem, Models.Vehicle _vehicle)
        {
            if (_vehicle != null)
            {
                if (_vehicle.BaggageItems.Items.Count + 1 < _vehicle.MaxBaggageCount)
                {
                    _vehicle.BaggageItems.Items.Add(_clientItem);
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                API.shared.consoleOutput("Bagaja item eklenirken araç bulunamadı.");
                return false;
            }
        }
        public static bool AddVehicleTorpedoItem(ClientItem _clientItem, long vehId)
        {
            var _vehicle = db_Vehicles.GetVehicle(vehId);
            if (_vehicle != null)
            {
                if (_vehicle.TorpedoItems.Items.Count < _vehicle.MaxTorpedoCount)
                {
                    _vehicle.TorpedoItems.Items.Add(_clientItem);
                    return true;
                }
                return false;
            }
            else
            {
                API.shared.consoleOutput("Torpidoya item eklenirken araç bulunamadı.");
                return false;
            }
        }
        public static Models.Vehicle FindVehicleByGameVehicle(GrandTheftMultiplayer.Server.Elements.Vehicle _vehicle)
        {
            foreach (var item in db_Vehicles.GetAll())
            {
                if (item.VehicleOnMap == _vehicle)
                {
                    return item;
                }
            }
            return null;
        }
        public static void TurnOffEngine(GrandTheftMultiplayer.Server.Elements.Vehicle _vehicle)
        {
            API.shared.setVehicleEngineStatus(_vehicle, false);
        }
        public static bool SetVehicleOwner(int VehicleId, string OwnerSocialClubID)
        {
            var _vehicle = db_Vehicles.GetVehicle(VehicleId);
            if (_vehicle != null)
            {
                _vehicle.OwnerSocialClubName = OwnerSocialClubID;
                //_vehicle.FactionId = -1;
                //_vehicle.JobId = -1;
                db_Vehicles.SaveChanges();
                return true;
            }
            return false;

        }
        public static void SaveVehicleFuel(Client sender)
        {
            API.shared.triggerClientEvent(sender, "save_fuel");
        }
        public static void SaveVehicleFuel(Client sender, float fuel)
        {
            var _vehicle = db_Vehicles.FindNearestVehicle(sender.position);
            if (_vehicle != null)
            {
                _vehicle.Fuel = fuel;

            }
            else
            {
                API.shared.consoleOutput("Index bulunamadı. | SaveVehicleFuel");
            }
        }
        public void SaveVehicleFuel(Client sender, int _fuel, NetHandle vehicle)
        {
            //var _Index = VehiclesOnMap.IndexOf(VehiclesOnMap.Find(x => x.handle ==  vehicle));
            var _vehicle = db_Vehicles.FindNearestVehicle(sender.position);
            if (_vehicle != null)
            {
                _vehicle.Fuel = _fuel;
                db_Vehicles.SaveChanges();
            }
            else
            {
                API.consoleOutput("Araç bulunamadı! ");
            }
        }
    }
}