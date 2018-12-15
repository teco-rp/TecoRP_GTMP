using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using TecoRP.Database;
using TecoRP.Managers;
using TecoRP.Models;
using TecoRP.Jobs;

namespace TecoRP.Admin
{
    public class AdminCommands : Managers.Base.EventMethodTriggerBase
    {

        [Command("ahelp", "/ahelp [page(1-2-3)] - Admin Help")]
        public void AdminHelp(Client sender, int page)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            switch (page)
            {
                case 1:
                    API.sendChatMessageToPlayer(sender,
               " ~y~ ADMIN KOMUTLARI ~W~\n" +
               "/aduty /bring, /goto, /moveto, /freeze, /unfreeze /createlabel, /socialclubname /revive /events \n" +
               "/spec, /specoff, /set,  /give, /entercar /removeweapon, /dimension /a, /tryanim ,/reload  \n" +
               "/veh, /kill, /createblip, /edit, /createentrance, /clothes /accessory /setnametagcolor /opendoor  \n" +
               "/createsalevehicle, /editsalevehicle /svid /notificationall /announcement /repair /break /blackout\n" +
               "/createhouse /editHouse /houseid /editshop /createshop /addshopitem /removeshopitem /reorderid, \n" +
               "/shopid /jobedit /addbusstop /busstopid /clearinventory /cleardroppeditems /loadipl, /removeipl \n" +
               "/createarrest, /editarrest, /arrestid /tyre /createoperatorshop /editoperatorshop /operatorshopid \n" +
               "/createbank /editbank /bankid /showinventory /refill /weather /hour /snow /mod /setextra /enginepower /enginetorque"
               );
                    break;
                case 2:
                    API.sendChatMessageToPlayer(sender,
             " ~y~ ADMIN KOMUTLARI ~W~\n" +
             " /createtirdelivery, /edittirdelivery, /tirdeliveryid /createkamyondelivery /kamyondeliveryid \n" +
             "/editkamyondelivery /creategasstation /editgasstation /gasstationid /createbusiness /businessid \n" +
             "/connect2business /createlicensepoint /editlicensepoint /addvehlicpoint /editvehlicpoint /resettax \n" +
             "/createfactionvault /editfactionvault /factionvaultid (fvid) /additemfactionvault /removeitemfactionvault \n" +
             "/sethospitalpoint /sethospitaldeliverpoint /createbuilding /editbuilding /addfloor /removefloor /alignhouses \n" +
             "/lock"
             );
                    break;
                case 3:
                    API.sendChatMessageToPlayer(sender,
            " ~y~ ADMIN KOMUTLARI ~W~\n" +
            " /ranks /addrank /removerank /addcrime /removecrime \n" +
            "/sorular /reports /cevapla /accept /reject /find /clearplayerstar\n " +
            "/createcraftingtable /ctableid /editctable /createfactioninteractive /editfi /fid /drunk"
            );
                    break;
                default:
                    break;
            }
        }


        [Command("loadipl", "/loadipl [name/all/*]", GreedyArg = true)]
        public void LoadIplCommand(Client sender, string ipl)
        {
            if (!(API.getEntityData(sender, "AdminLevel") > 0)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            if ("all".StartsWith(ipl.ToLower()) || "*" == ipl)
            {
                API.sendChatMessageToPlayer(sender, "~h~Tüm IPL'ler yüklenmeye başladı.");
                GameObjectsManager.GetAllIPLSs();
                API.sendChatMessageToPlayer(sender, "~h~Tüm IPL'ler başarıyla yüklendi.");
            }
            else
            {
                API.requestIpl(ipl);
                API.consoleOutput("LOADED IPL " + ipl);
                API.sendChatMessageToPlayer(sender, "Loaded IPL ~b~" + ipl + "~w~.");
            }
        }

        [Command("removeipl", "/removeipl [name/all/*]")]
        public void RemoveIplCommand(Client sender, string ipl)
        {
            if (!(API.getEntityData(sender, "AdminLevel") > 0)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            if ("all".StartsWith(ipl.ToLower()) || ipl == "*")
            {
                API.sendChatMessageToPlayer(sender, "~h~Tüm IPL'ler silinmeye başladı.");
                GameObjectsManager.RemoveAllIPLs();
                API.sendChatMessageToPlayer(sender, "~h~Tüm IPL'ler başarıyla silindi.");
                return;
            }
            API.removeIpl(ipl);
            API.consoleOutput("REMOVED IPL " + ipl);
            API.sendChatMessageToPlayer(sender, "Removed IPL ~b~" + ipl + "~w~.");
        }


        [Command("aduty")]
        public void AdminDuty(Client sender)
        {
            if (!(API.getEntityData(sender, "AdminLevel") > 0)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            if (API.shared.hasEntityData(sender, "Aduty"))
            {
                sender.nametag = "(" + API.getEntityData(sender, "ID") + ")" + API.getEntityData(sender, "CharacterName");
                API.shared.resetEntityData(sender, "Aduty");
            }
            else
            {
                API.setEntityData(sender, "Aduty", true);
                API.setPlayerNametag(sender, sender.name);
            }
        }

        [Command("save")]
        public void Save(Client sender)
        {
            if (!(API.getEntityData(sender, "AdminLevel") > 0)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            db_Vehicles.UpdateAndSaveChanges();
        }
        [Command("tryanim", "/tryanim [animDict] [animName]", GreedyArg = true)]
        public void TryAnim(Client sender, string commandParam)
        {
            string[] anim = commandParam.Split(' ');
            sender.playAnimation(anim[0], anim.LastOrDefault(), 0);
        }

        [Command("enginepower")]
        public void EnginePower(Client sender, float multiplier)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            var _vehicle = db_Vehicles.FindNearestVehicle(sender.position);
            API.setVehicleEnginePowerMultiplier(_vehicle.VehicleOnMap, multiplier);
        }
        [Command("enginetorque")]
        public void EngineTorque(Client sender, float multiplier)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            var _vehicle = db_Vehicles.FindNearestVehicle(sender.position);
            API.setVehicleEngineTorqueMultiplier(_vehicle.VehicleOnMap, multiplier);
        }
        [Command("events")]
        public void Events(Client sender)
        {
            if (!(API.getEntityData(sender, "AdminLevel") > 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            API.sendChatMessageToPlayer(sender, "Özel günlerde maaş arttırılabilir. \n Rastgele araç veren piyango yapılabilir. \n Bet eklenebilir. \n Emlakçı uygulaması - fiyata yere göre en uygun satılık evi bulma \n İnternet bankacılığı uygulaması. \nTelefona radyo /r \n" +
                "Telefondan konum gönderme"
                );

        }
        [Command("weather")]
        public void SetWeather(Client sender, int weather)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            API.setWeather(weather);
        }
        [Command("hour")]
        public void SetHour(Client sender, int hour, int minute)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            if (hour > 23 || hour < 0)
                return;
            API.setTime(hour, minute);

        }
        [Command("snow")]
        public void SnowMode(Client sender)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            API.triggerClientEventForAll("snow_all");
        }
        [Command("repair", "/repair")]
        public void Repair(Client sender)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            var _vehicle = db_Vehicles.FindNearestVehicle(sender.position);

            if (Vector3.Distance(_vehicle.VehicleOnMap.position, sender.position) < 5)
            {
                _vehicle.VehicleOnMap.repair();
                return;
            }

        }

        [Command("break", "/break [Door/window] [number]")]
        public void Break(Client sender, string type, int _value)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            var _vehicle = db_Vehicles.FindNearestVehicle(sender.position);
            if ("door".StartsWith(type.ToLower()))
            {
                _vehicle.VehicleOnMap.breakDoor(_value);
            }
            if ("window".StartsWith(type.ToLower()))
            {
                _vehicle.VehicleOnMap.breakWindow(_value);
            }
        }
        [Command("mod", "/mod [slot] [mod]")]
        public void ModifyVehicle(Client sender, int slot, int mod)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }

            var _vehicle = db_Vehicles.FindNearestVehicle(sender.position);
            //if (!sender.isInVehicle)
            //{
            if (Vector3.Distance(sender.position, _vehicle.VehicleOnMap.position) < 5)
            {
                if (mod == -1)
                {
                    _vehicle.VehicleOnMap.removeMod(slot);
                    API.sendChatMessageToPlayer(sender, "~y~" + slot + " slotundaki mod silindi.");
                }
                else
                {
                    _vehicle.VehicleOnMap.setMod(slot, mod);
                    _vehicle.Mods[slot] = mod;
                }
                return;
            }
            //}
            //else
            //{
            //    API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Araçtayken bunu yapamazsınız.");
            //    break;
            //}
        }

        [Command("setextra")]
        public void SetVehicleExtra(Client sender, int slot, bool enabled)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            var _vehicle = db_Vehicles.FindNearestVehicle(sender.position);
            API.setVehicleExtra(_vehicle.VehicleOnMap, slot, enabled);
        }
        [Command("setlivery")]
        public void SetVehicleLivery(Client sender, int _livery)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            var _vehicle = db_Vehicles.FindNearestVehicle(sender.position);
            if (Vector3.Distance(sender.position, _vehicle.LastPosition) < 4)
            {
                API.setVehicleLivery(_vehicle.VehicleOnMap, _livery);
                _vehicle.Livery = _livery;
                db_Vehicles.SaveChanges();
            }
        }

        [Command("marker")]
        public void Marker(Client sender)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            API.triggerClientEvent(sender, "create_marker", sender.position.X, sender.position.Y, sender.position.Z);
        }
        [Command("removemarker")]
        public void RemoveMarker(Client sender)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            API.triggerClientEvent(sender, "remove_marker");
        }
        [Command("socialclubname", "/scn [PlayerId]")]
        public void SocialClubName(Client sender, int _Id)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            foreach (var item in API.getAllPlayers())
            {
                if (API.getEntityData(item, "ID") == _Id)
                {
                    API.sendChatMessageToPlayer(sender, item.nametag + " ~y~( " + item.socialClubName + " )");
                    return;
                }
            }
        }
        [Command("revive", "/revive [PlayerID]")]
        public void RevivePlayer(Client sender, int targetPlayerId)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            var player = db_Accounts.FindPlayerById(targetPlayerId);
            if (player != null)
            {
                Users.MedicalCommands.RevivePlayer(sender);
            }
            else
            {
                API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Oyuncu bulunamadı.");
            }
        }
        [Command("sethudcolor", "/sethudcolor [playerid] [red] [greed] [blue]")]
        public void SetHudColor(Client sender, int playerId, int red, int green, int blue)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            var _player = db_Accounts.FindPlayerById(playerId);
            if (_player != null)
            {
                API.triggerClientEvent(_player, "set_ui_color", red, green, blue);
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Oyuncu bulunamadı.");
            }
        }
        [Command("ban", GreedyArg = true)]
        public void BanPlayer(Client sender, int PlayerID, string reason)
        {
            if (!(API.getEntityData(sender, "AdminLevel") > 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            foreach (var item in API.getAllPlayers())
            {
                if (API.getEntityData(item, "ID") == PlayerID)
                {
                    API.sendChatMessageToAll("~r~" + item.nametag + " adlı kişi sunucudan yasaklandı. SEBEP: " + reason);
                    API.sendChatMessageToPlayer(sender, "~y~Kişinin Social Club hesabı : " + item.socialClubName);
                    reason = reason.Replace("_", " ");
                    API.banPlayer(item, reason);
                }
            }
        }
        [Command("unban")]
        public void Unban(Client sender, string socialClubName)
        {
            if (!(API.getEntityData(sender, "AdminLevel") > 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            API.unbanPlayer(socialClubName);
            API.sendNotificationToPlayer(sender, "~y~İşlem gönderildi.");
        }
        [Command("kick", GreedyArg = true)]
        public void KickPlayer(Client sender, int PlayerID, string _reason)
        {
            if (!(API.getEntityData(sender, "AdminLevel") > 0)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            foreach (var item in API.getAllPlayers())
            {
                if (API.getEntityData(item, "ID") == PlayerID)
                {
                    _reason = _reason.Replace("_", " ");
                    API.sendChatMessageToAll("~r~" + item.nametag + " adlı kişi sunucudan atıldı. SEBEP: " + _reason);
                    API.kickPlayer(item, _reason);
                }
            }
        }

        [Command("bring", "/bring [Player/Vehicle] [Name/vehid]")]
        public void Bring(Client sender, string type, string identity)
        {
            if (!(API.getEntityData(sender, "AdminLevel") > 0)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            if (type.ToLower().StartsWith("p"))
            {
                var player = db_Accounts.GetPlayerById(Convert.ToInt32(identity));
                if (player != null)
                {
                    if (player.isInVehicle)
                    {
                        player.vehicle.position = sender.position + new Vector3(1, 0, 1);
                        player.vehicle.dimension = sender.dimension;
                        foreach (var item in API.getVehicleOccupants(player.vehicle))
                        {
                            item.dimension = sender.dimension;
                        }
                    }
                    else
                    {
                        player.position = sender.position + new Vector3(1, 0, 0);
                        player.dimension = sender.dimension;

                    }
                }

            }
            if (type.ToLower().StartsWith("v"))
            {
                try
                {
                    Convert.ToInt32(identity);
                }
                catch (Exception) { API.sendChatMessageToPlayer(sender, "~r~Hata: ~w~ID kısmına sayı girmelisiniz."); return; }

                var _vehicle = db_Vehicles.GetVehicle(Convert.ToInt64(identity));
                if (_vehicle != null)
                {
                    _vehicle.VehicleOnMap.position = sender.position + new Vector3(1, 1, 1);
                    _vehicle.VehicleOnMap.dimension = sender.dimension;
                }
                else
                {
                    API.sendNotificationToPlayer(sender, "Araç bulunamadı!", true);
                }
            }
        }

        [Command("respawn", "/respawn [player/vehicle/salevehicle] [name/(vehid/all)]")]
        public void Respawn(Client sender, string type, string value)
        {
            if (!(API.getEntityData(sender, "AdminLevel") > 0)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            if ("player".StartsWith(type.ToLower()))
            {
                value = value.Replace("_", " ");
                API.sendNotificationToPlayer(sender, "bu komut henüz yazılmadı.");
            }
            if ("vehicle".StartsWith(type.ToLower()) || "car".StartsWith(type.ToLower()))
            {
                if ("all".StartsWith(value.ToLower()))
                {
                    db_Vehicles.RespawnAll();
                }
                else
                {
                    try
                    {
                        db_Vehicles.Respawn(Convert.ToInt64(value));
                    }
                    catch (Exception ex)
                    {
                        if (ex.GetType() == typeof(FormatException))
                        {
                            API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~ID sayı olmalıydı. Sayı dışındaki parametreler: ~y~[all/*]");
                        }
                    }
                }
            }
            if ("salevehicle".StartsWith(type.ToLower()) || "sv".StartsWith(type.ToLower()))
            {
                for (int i = 0; i < SaleVehicleManager.SaleVehiclesOnMap.Count; i++)
                {
                    API.deleteEntity(SaleVehicleManager.SaleVehiclesOnMap[i]);

                    var _vehicleModel = db_SaleVehicles.currentSaleVehicleList.Items[i];
                    SaleVehicleManager.SaleVehiclesOnMap.RemoveAt(i);
                    SaleVehicleManager.SaleVehiclesOnMap.Insert(i, API.createVehicle(_vehicleModel.VehicleModel, new Vector3(_vehicleModel.Position.X, _vehicleModel.Position.Y, _vehicleModel.Position.Z), new Vector3(_vehicleModel.Rotation.X, _vehicleModel.Rotation.Y, _vehicleModel.Rotation.Z), _vehicleModel.VehicleColors.Color_1, _vehicleModel.VehicleColors.Color_2, _vehicleModel.Dimension));
                    API.setVehicleEngineStatus(SaleVehicleManager.SaleVehiclesOnMap[i], false);
                }
            }
        }

        [Command("reorderid", "/reorderid [players/vehicles]")]
        public void ReOrderIds(Client sender, string type)
        {
            if ("players".StartsWith(type.ToLower()))
            {
                var allPlayers = API.getAllPlayers();
                for (int i = 0; i < allPlayers.Count; i++)
                {
                    API.setEntityData(allPlayers[i], "ID", i);
                    allPlayers[i].nametag = "(" + i + ") " + API.getEntityData(allPlayers[i], "CharacterName");
                }
            }
            if ("vehicles".StartsWith(type.ToLower()))
            {
                for (int i = 0; i < db_Vehicles.GetAll().Count; i++)
                {
                    db_Vehicles.GetAll()[i].VehicleId = (i + 1);
                    db_Vehicles.GetAll()[i].Plate = "LS-" + (i + 1);
                    API.setVehicleNumberPlate(db_Vehicles.GetAll()[i].VehicleOnMap, "LS-" + (i + 1));
                }
                db_Vehicles.SaveChanges();
            }

        }
        [Command("spec", "/spec(tate) [name]")]
        public void Spectate(Client sender, string _name)
        {

            if (!(API.getEntityData(sender, "AdminLevel") > 0)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }

            foreach (var item in API.getAllPlayers())
            {
                if (API.getEntityData(item, "ID") == Convert.ToInt32(_name) || item.socialClubName.StartsWith(_name))
                {
                    if (sender != item)
                    {
                        API.setPlayerToSpectatePlayer(sender, item);
                    }
                    else
                    {
                        API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Kendinizi izleyemezsiniz.");
                    }
                }
            }
        }

        [Command("clothes")]
        public void SetPedClothesCommand(Client sender, int slot, int drawable, int texture)
        {
            if (!(API.getEntityData(sender, "AdminLevel") > 0)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            API.setPlayerClothes(sender, slot, drawable, texture);
            API.sendChatMessageToPlayer(sender, "Clothes applied successfully!");
        }

        [Command("specoff", "/spec(tate)")]
        public void SpectateOff(Client sender)
        {
            if (!(API.getEntityData(sender, "AdminLevel") > 0)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            if (API.isPlayerSpectating(sender))
            {
                API.unspectatePlayer(sender);
            }
            else
                API.sendChatMessageToPlayer(sender, "Zaten kimseyi izlemiyorsunuz.");
        }

        [Command("goto", "/goto [Player/Vehicle/salevehicle/Pos/Entrance/House] [Name/ID/X_Y_Z]", GreedyArg = true)]
        public void GoTo(Client sender, string type, string identity)
        {
            if (!(API.getEntityData(sender, "AdminLevel") > 0)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            #region player

            if ("player".StartsWith(type.ToLower()))
            {
                var player = db_Accounts.FindPlayerById(Convert.ToInt32(identity));
                if (player != null)
                {
                    if (sender.isInVehicle)
                    {
                        sender.vehicle.position = player.position + new Vector3(1, 0, 0);
                        foreach (var item in API.getVehicleOccupants(sender.vehicle))
                        {
                            item.dimension = player.dimension;
                        }
                    }
                    else
                    {
                        sender.position = player.position + new Vector3(1, 0, 0);
                        sender.dimension = player.dimension;
                    }

                }

                #endregion
            }
            else
                if ("vehicle".StartsWith(type.ToLower()) || "car".StartsWith(type.ToLower()))
            {
                #region vehicle
                try
                {
                    var _vehicle = db_Vehicles.GetVehicle(Convert.ToInt64(identity));
                    if (_vehicle != null)
                    {
                        if (sender.isInVehicle)
                        {
                            sender.vehicle.position = _vehicle.VehicleOnMap.position + new Vector3(0, 0, 1);
                            foreach (var item in API.getVehicleOccupants(sender.vehicle))
                            {
                                item.dimension = _vehicle.VehicleOnMap.dimension;
                            }
                        }
                        else
                        {
                            sender.position = _vehicle.VehicleOnMap.position + new Vector3(0, 0, 1);
                            sender.dimension = _vehicle.VehicleOnMap.dimension;
                        }
                    }
                    else
                    {
                        API.consoleOutput("VEH IS NULL");
                    }
                }
                catch (Exception ex)
                {
                    if (ex.GetType() == typeof(IndexOutOfRangeException))
                    {
                        API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Böyle bir araç bulunmuyor.");
                    }
                    else
                    if (ex.GetType() == typeof(FormatException))
                    {
                        API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Parametre sayı olmalıydı.");
                    }
                    else
                        API.consoleOutput(" [GOTO - VEHICLE] HATA : " + ex.Message);
                }
                #endregion
            }
            else
            if ("pos".StartsWith(type.ToLower()))
            {
                if (identity.Count(x => x == ' ') == 2)
                {
                    var splitted = identity.Split(' ');
                    if (splitted.Length > 2)
                    {
                        sender.position = new Vector3(Convert.ToSingle(splitted[0]), Convert.ToSingle(splitted[1]), Convert.ToSingle(splitted[2]));
                    }
                }
                else
                {
                    sender.position = new Vector3(Convert.ToSingle(identity.Split('_')[0]), Convert.ToSingle(identity.Split('_')[1]), Convert.ToSingle(identity.Split('_')[2]));
                }
            }
            else
            if ("entrance".StartsWith(type.ToLower()))
            {
                sender.position = db_Entrances.currentEntrances.Items[db_Entrances.FindBlipIndexById(Convert.ToInt32(identity))].EntrancePosition;
            }
            else
                if ("salevehicle".StartsWith(type.ToLower()) || "sv".StartsWith(type.ToLower()))
            {
                sender.position = SaleVehicleManager.SaleVehiclesOnMap[db_SaleVehicles.FindSaleVehicleIndexById(Convert.ToInt32(identity))].position + new Vector3(0, 0, 1);
            }
            else
                 if ("house".StartsWith(type.ToLower()))
            {
                var _house = db_Houses.GetHouse(Convert.ToInt32(identity));
                sender.position = _house.EntrancePosition + new Vector3(0, 0, 1);
            }
            else
            if ("busstop".StartsWith(type.ToLower()))
            {
                foreach (var item in db_BusJob.CurrentBusStops.Item1)
                {
                    if (item.ID == Convert.ToInt32(identity))
                    {
                        sender.position = item.Position + new Vector3(0, 0, 1);
                        sender.dimension = item.Dimension;
                        return;
                    }
                }
            }
            else
            if ("shop".StartsWith(type.ToLower()))
            {
                var _Shop = db_Shops.GetShop(Convert.ToInt32(identity));
                if (_Shop != null)
                {
                    sender.position = _Shop.Position;
                    sender.dimension = _Shop.Dimension;
                }
                else
                {
                    API.sendNotificationToPlayer(sender, "~r~Shop bulunamadı!");
                }
            }
            else
            if ("building".StartsWith(type.ToLower()))
            {
                var _building = db_Buildings.GetBuilding(Convert.ToInt32(identity));
                if (_building != null)
                {
                    if (sender.isInVehicle)
                    {
                        sender.vehicle.position = _building.Position;
                        foreach (var itemPlayer in API.shared.getVehicleOccupants(sender.vehicle))
                        {
                            itemPlayer.dimension = _building.Dimension;
                        }
                    }
                    else
                    {
                        sender.position = _building.Position;
                        sender.dimension = _building.Dimension;
                    }
                }
                else
                {
                    API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bina bulunamadı.");
                }
            }
        }

        [Command("moveto", "/moveto [Player/Vehicle] [(Player)Name] [(Target)Name/ID] [Duration]")]
        public void MoveTo(Client sender, string type, string identity, int duration)
        {
            if (!(API.getEntityData(sender, "AdminLevel") > 0)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            if (type.ToLower().StartsWith("p"))
            {
                foreach (var item in API.getAllPlayers())
                {
                    if (API.getEntityData(item, "ID") == Convert.ToInt32(identity) || item.socialClubName.ToLower().StartsWith(identity.ToLower()) || item.nametag.ToLower().StartsWith(identity.ToLower()))
                    {
                        sender.movePosition(item.position + new Vector3(1, 0, 0), duration);
                    }
                }
            }
            else
                if (type.ToLower().StartsWith("v"))
            {
                var _vehicle = db_Vehicles.GetVehicle(Convert.ToInt64(identity));
                sender.position = _vehicle.VehicleOnMap.position + new Vector3(0, 0, 2);
            }
        }

        [Command("set", "/set ~y~[armor/hp/money/skin/admin/name/faction/job/hunger/thirsty/rank/wanted/gender/team]~s~ [(Player)] [value]", GreedyArg = true)]
        public void Set(Client sender, string type, string targetPlayer, string value)
        {
            if ("armor".StartsWith(type.ToLower()))
            {
                if (!(API.getEntityData(sender, "AdminLevel") > 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
                foreach (var item in API.getAllPlayers())
                {
                    if (API.getEntityData(item, "ID") == Convert.ToInt32(targetPlayer))
                    {
                        //if (value.GetType() != typeof(Int32)) { API.sendChatMessageToPlayer(sender, "Parametre sayı olmalı."); return; }
                        API.setPlayerArmor(item, Convert.ToInt32(value));
                        API.sendChatMessageToPlayer(sender, "~y~" + item.name + " adlı kişinin zırhı " + value + " olarak ayarlandı.");
                    }
                }
            }
            else
            if ("hp".StartsWith(type.ToLower()))
            {
                if (!(API.getEntityData(sender, "AdminLevel") > 0)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
                foreach (var item in API.getAllPlayers())
                {
                    if (API.getEntityData(item, "ID") == Convert.ToInt32(targetPlayer) || item.socialClubName.ToLower().StartsWith(targetPlayer.ToLower()) || item.nametag.ToLower().StartsWith(targetPlayer.Replace("_", " ").ToLower()))
                    {
                        //if (value.GetType() != typeof(Int32)) { API.sendChatMessageToPlayer(sender, "Parametre sayı olmalı."); return; }
                        API.setPlayerHealth(item, Convert.ToInt32(value));
                        API.sendChatMessageToPlayer(sender, "~y~" + item.name + " adlı kişinin canı " + value + " olarak ayarlandı.");
                        return;
                    }
                }
            }
            else
            if ("money".StartsWith(type.ToLower()))
            {
                if (!(API.getEntityData(sender, "AdminLevel") > 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
                foreach (var item in API.getAllPlayers())
                {
                    if (API.getEntityData(item, "ID") == Convert.ToInt32(targetPlayer) || item.socialClubName.ToLower().StartsWith(targetPlayer.ToLower()) || item.nametag.ToLower().StartsWith(targetPlayer.Replace("_", " ").ToLower()))
                    {
                        int money = API.getEntityData(item, "Money");
                        money = Convert.ToInt32(value);
                        API.triggerClientEvent(item, "update_money_display", money);
                        API.setEntityData(item, "Money", money);
                        return;
                    }
                }
            }
            else
            if ("skin".StartsWith(type.ToLower()))
            {
                if (!(API.getEntityData(sender, "AdminLevel") > 0)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
                foreach (var item in API.getAllPlayers())
                {
                    if (API.getEntityData(item, "ID") == Convert.ToInt32(targetPlayer) || (item.socialClubName.ToLower().StartsWith(targetPlayer.ToLower()) || item.nametag.ToLower().StartsWith(targetPlayer.Replace("_", " ").ToLower())))
                    {
                        API.setPlayerSkin(item, API.pedNameToModel(value));
                        API.setEntityData(item, "Skin", API.pedNameToModel(value).ToString() != "0" ? API.pedNameToModel(value) : API.getEntityData(item, "Skin"));
                        //API.setPlayerSkin(item,GrandTheftMultiplayer.Server.Constant.PedHash.)
                        return;
                    }
                }
            }
            else
            if ("admin".StartsWith(type.ToLower()))
            {
                if (!(API.getEntityData(sender, "AdminLevel") > 0)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
                foreach (var item in API.getAllPlayers())
                {
                    if (API.getEntityData(item, "ID") == Convert.ToInt32(targetPlayer) || item.socialClubName.ToLower().StartsWith(targetPlayer.ToLower()) || item.nametag.ToLower().StartsWith(targetPlayer.Replace("_", " ").ToLower()))
                    {
                        API.setEntityData(item, "AdminLevel", Convert.ToByte(value));
                        API.sendChatMessageToPlayer(sender, "~y~" + item.socialClubName + " adlı kullanıcı " + value + " level admin olarak atandı.");
                        API.sendChatMessageToPlayer(item, "~y~" + value + " level admin olarak atandınız.");
                        return;
                    }
                }
            }

            if ("name".StartsWith(type.ToLower()))
            {
                if (!(API.getEntityData(sender, "AdminLevel") > 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
                foreach (var item in API.getAllPlayers())
                {
                    if (API.getEntityData(item, "ID") == Convert.ToInt32(targetPlayer) || item.socialClubName.ToLower().StartsWith(targetPlayer.ToLower()) || item.nametag.ToLower().StartsWith(targetPlayer.Replace("_", " ").ToLower()))
                    {
                        value = value.Replace("_", " ");
                        item.nametag = "(" + API.getEntityData(item, "ID") + ") " + value;
                        API.setEntityData(item, "CharacterName", value);
                        API.sendChatMessageToPlayer(sender, "~y~" + item.socialClubName + " adlı kullanıcınun adı " + value + " olarak değiştirildi.");
                        API.sendChatMessageToPlayer(item, "~y~Adınız " + value + " olarak değiştirildi.");
                        return;
                    }
                }
            }
            if ("faction".StartsWith(type.ToLower()))
            {
                if (!(API.getEntityData(sender, "AdminLevel") > 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
                foreach (var item in API.getAllPlayers())
                {
                    if (API.getEntityData(item, "ID") == Convert.ToInt32(targetPlayer) || item.socialClubName.ToLower().StartsWith(targetPlayer.ToLower()) || item.nametag.ToLower().StartsWith(targetPlayer.Replace("_", " ").ToLower()))
                    {
                        API.setEntityData(item, "FactionId", Convert.ToInt32(value));
                        API.setEntityData(item, "FactionRank", 1);
                        //if (value == "1")
                        //{
                        //    item.nametagColor = new GrandTheftMultiplayer.Server.Constant.Color { alpha = 255, blue = 255, green = 0, red = 0 };
                        //}
                        //else
                        //    item.nametagColor = new GrandTheftMultiplayer.Server.Constant.Color { alpha = 255, blue = 255, green = 255, red = 255 };
                        return;
                    }
                }
            }
            if ("job".StartsWith(type.ToLower()))
            {
                if (!(API.getEntityData(sender, "AdminLevel") > 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
                foreach (var item in API.getAllPlayers())
                {
                    if (API.getEntityData(item, "ID") == Convert.ToInt32(targetPlayer) || item.socialClubName.ToLower().StartsWith(targetPlayer.ToLower()) || item.nametag.ToLower().StartsWith(targetPlayer.Replace("_", " ").ToLower()))
                    {
                        API.setEntityData(item, "JobId", Convert.ToInt32(value));
                        return;
                    }
                }
            }
            else
            if ("hunger".StartsWith(type.ToLower()))
            {
                if (!(API.getEntityData(sender, "AdminLevel") > 0)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
                foreach (var item in API.getAllPlayers())
                {
                    if (API.getEntityData(item, "ID") == Convert.ToInt32(targetPlayer))
                    {
                        float _Thirsty = Convert.ToInt32(API.getEntityData(item, "Thirsty"));
                        API.setEntityData(item, "Hunger", Convert.ToInt32(value));
                        API.triggerClientEvent(item, "update_hungerthirsty", value, _Thirsty);
                    }
                }
            }
            else
            if ("thirsty".StartsWith(type.ToLower()))
            {
                if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
                foreach (var item in API.getAllPlayers())
                {
                    if (API.getEntityData(item, "ID") == Convert.ToInt32(targetPlayer))
                    {
                        float _Hunger = Convert.ToInt32(API.getEntityData(item, "Hunger"));
                        API.triggerClientEvent(item, "update_hungerthirsty", _Hunger, value);
                        API.setEntityData(item, "Thirsty", Convert.ToInt32(value));

                    }
                }
            }
            else
            if ("rank".StartsWith(type.ToLower()))
            {
                if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
                foreach (var itemPlayer in API.getAllPlayers())
                {
                    if (API.getEntityData(itemPlayer, "ID") == Convert.ToInt32(targetPlayer))
                    {
                        int facitonId = API.getEntityData(itemPlayer, "FactionId");
                        if (facitonId > 0)
                        {
                            var _rank = db_FactionRanks.GetRank(facitonId, Convert.ToInt32(value));
                            if (_rank != null)
                            {
                                API.setEntityData(itemPlayer, "FactionRank", _rank.RankLevel);

                            }
                            else
                            {
                                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Kişinin oluşumunda böyle bir rütbe bulunmuyor.");
                            }
                        }
                        else
                        {
                            API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Oyuncu bir oluşumda değil.");
                        }
                    }
                }
            }
            else
            if ("wanted".StartsWith(type.ToLower()))
            {
                var player = db_Accounts.GetPlayerById(Convert.ToInt32(targetPlayer));
                if (player != null)
                {

                    API.shared.setEntityData(player, "WantedLevel", Convert.ToInt32(value) > 5 ? 5 : Convert.ToInt32(value));
                    API.shared.setPlayerWantedLevel(player, API.shared.getEntityData(player, "WantedLevel"));
                }
            }
            else
            if ("team".StartsWith(type.ToLower()))
            {
                try
                {

                    var player = db_Accounts.GetPlayerById(Convert.ToInt32(targetPlayer));
                    if (player != null)
                    {
                        API.shared.setPlayerTeam(player, Convert.ToInt32(value));
                    }
                    else
                    {
                        API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Oyuncu bulunamadı!");
                    }
                }
                catch (Exception ex)
                {
                    if (ex.GetType() == typeof(FormatException))
                    {
                        API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Parametreler sayı olmalıydı.");
                    }
                }
            }

        }

        [Command("freeze", "/freeze [player]")]
        public void Freeze(Client sender, string targetPlayer)
        {
            if (!(API.getEntityData(sender, "AdminLevel") > 0)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            foreach (var item in API.getAllPlayers())
            {
                if (API.getEntityData(item, "ID") == Convert.ToInt32(targetPlayer) || item.socialClubName.ToLower().StartsWith(targetPlayer.ToLower()) || item.nametag.ToLower().StartsWith(targetPlayer.Replace("_", " ").ToLower()))
                {
                    item.freeze(true);
                    API.freezePlayer(item, true);
                }
            }
        }
        [Command("unfreeze", "/unfreeze [player]")]
        public void Unfreeze(Client sender, string targetPlayer)
        {
            if (!(API.getEntityData(sender, "AdminLevel") > 0)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            foreach (var item in API.getAllPlayers())
            {
                if (API.getEntityData(item, "ID") == Convert.ToInt32(targetPlayer) || item.socialClubName.ToLower().StartsWith(targetPlayer.ToLower()) || item.nametag.ToLower().StartsWith(targetPlayer.Replace("_", " ").ToLower()))
                {
                    item.freeze(false);
                    API.freezePlayer(item, false);
                }
            }
        }

        [Command("give", "/give [money/vehicle/weapon/item/metalpart] [(player)Name] [ value / itemId]")]
        public void Give(Client sender, string type, string targetPlayer, string value)
        {
            targetPlayer = targetPlayer.Replace("_", " ");
            if ("money".StartsWith(type.ToLower()))
            {
                if (!(API.getEntityData(sender, "AdminLevel") > 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
                foreach (var item in API.getAllPlayers())
                {
                    if (API.getEntityData(item, "ID") == Convert.ToInt32(targetPlayer))
                    {
                        int money = API.getEntityData(item, "Money");
                        money += Convert.ToInt32(value);
                        API.triggerClientEvent(item, "update_money_display", money);
                        API.setEntityData(item, "Money", money);
                        return;
                    }
                }
            }
            if ("vehicle".StartsWith(type.ToLower()) || "car".StartsWith(type.ToLower()))
            {
                #region Vehicle
                if (!(API.getEntityData(sender, "AdminLevel") > 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
                foreach (var item in API.getAllPlayers())
                {
                    if (API.getEntityData(item, "ID") == Convert.ToInt32(targetPlayer))
                    {
                        if (!item.isInVehicle)
                        {
                            var _RequestedVehicle = API.vehicleNameToModel(value);
                            API.consoleOutput("Requested Vehicle " + _RequestedVehicle.ToString());

                            if (_RequestedVehicle.ToString() == "0")
                            {
                                API.sendChatMessageToPlayer(sender, "Geçersiz araç");
                                return;
                            }

                            var player = db_Accounts.GetPlayerById(Convert.ToInt32(targetPlayer));
                            if (player != null)
                            {
                                if (db_Vehicles.CreateVehicle(_RequestedVehicle, player))
                                    API.setPlayerIntoVehicle(player, db_Vehicles.GetAll().LastOrDefault().VehicleOnMap, -1);
                                else
                                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Hatalı parametre");
                            }
                            else
                            {
                                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Oyuncu bulunamadı.");
                            }

                        }
                        else
                        {
                            API.sendChatMessageToPlayer(sender, "Kişi araçtayken bunu yapamazsınız.");
                            API.sendChatMessageToPlayer(item, "Şu anda araçtayken yeni araç alamazsınız.");
                        }
                        return;
                    }
                }
                #endregion
            }
            if ("weapon".StartsWith(type.ToLower()))
            {
                if (!(API.getEntityData(sender, "AdminLevel") > 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
                foreach (var item in API.getAllPlayers())
                {
                    if (API.getEntityData(item, "ID") == Convert.ToInt32(targetPlayer))
                    {
                        //OLD
                        //API.givePlayerWeapon(item, API.weaponNameToModel(value), 99999, false, false);
                        API.givePlayerWeapon(item, API.weaponNameToModel(value), 99999,true,true);
                    }
                }
            }
            if ("item".StartsWith(type.ToLower()))
            {
                #region Item
                if (!(API.getEntityData(sender, "AdminLevel") > 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
                try
                {
                    if (db_Items.GameItems.Any(x => x.Key == Convert.ToInt32(value)))
                    {
                        foreach (var item in API.getAllPlayers())
                        {
                            if (API.getEntityData(item, "ID") == Convert.ToInt32(targetPlayer))
                            {
                                //var obj = db_Items.GameItems.Items.FirstOrDefault(x => x.ID == Convert.ToInt32(value));
                                var _inventory = (Inventory)API.getEntityData(item, "inventory");
                                var _Index = _inventory.ItemList.FindIndex(X => X.ItemId == Convert.ToInt32(value));
                                if (_Index >= 0)
                                {
                                    _inventory.ItemList[_Index].Count++;
                                }
                                else
                                {
                                    if (PhoneManager.PhoneIdList.Contains(Convert.ToInt32(value)))
                                    {
                                        _inventory.ItemList.Add(new ClientItem { ItemId = Convert.ToInt32(value), Count = 1, SpecifiedValue = API.toJson(new SpecifiedValuePhone { PhoneNumber = "0000", Balance = 9999, PhoneOperator = Operator.LosTelecom, Contacts = new Dictionary<string, string>(), Applications = new List<Application> { Application.GPS } }) });
                                    }
                                    else
                                        _inventory.ItemList.Add(new ClientItem { ItemId = Convert.ToInt32(value), Count = 1 });
                                }

                                API.setEntityData(item, "inventory", _inventory);
                                API.sendNotificationToPlayer(sender, "" + db_Items.GameItems[Convert.ToInt32(value)].Name + " adlı eşya " + item.nametag + " adlı kişiye verildi.");
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    if (ex.GetType() == typeof(FormatException))
                    {
                        API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Item parametresi sayılal türde bir ID olmalıydı.");
                    }
                    API.consoleOutput(LogCat.Warn, ex.ToString());
                }
                #endregion
            }
            if ("metalpart".StartsWith(type.ToLower()))
            {
                var player = db_Accounts.FindPlayerById(Convert.ToInt32(targetPlayer));
                if (player != null)
                {
                    InventoryManager.AddMetalPartsToPlayer(player, Convert.ToInt32(value), false);
                }
                else
                {
                    API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Oyuncu bulunamadı.");
                }
            }
            if ("gender".StartsWith(type.ToLower()))
            {
                var player = db_Accounts.FindPlayerById(Convert.ToInt32(targetPlayer));
                if (player!=null)
                {
                    API.shared.setEntityData(player, "Gender", Convert.ToBoolean(value));
                }
            }
        }

        [Command("remove", "/remove ~y~[TYPE] ~s~ [ID|weaponname/All] \n TYPES: ~y~(vehicle/blip/entrance/salevehicle/house/shop/bank/busstop/tirdeliverypoint/kamyondeliverypoint/gasstation\n vehlicpoint/ctable)")]
        public void Remove(Client sender, string type, string value)
        {
            if ("vehicle".StartsWith(type.ToLower()) || "car".StartsWith(type.ToLower()))
            {
                if (!(API.getEntityData(sender, "AdminLevel") >= 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
                #region Vehicle
                var _vehicle = db_Vehicles.GetVehicle(Convert.ToInt64(value));
                if (_vehicle != null)
                {
                    if (db_Vehicles.RemoveVehicle(_vehicle))
                    {
                        API.sendChatMessageToPlayer(sender, "~y~ Araç başarıyla kaldırıldı.");
                    }
                }
                else
                {
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Araç bulunamadı.");
                }
                #endregion
            }
            else
            if ("blip".StartsWith(type.ToLower()))
            {
                if (!(API.getEntityData(sender, "AdminLevel") >= 3)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
                #region Blip
                var _index = db_Blips.FindBlipIndexById(Convert.ToInt32(value));
                API.deleteEntity(BlipManager.BlipsOnMap[_index]);
                BlipManager.BlipsOnMap.RemoveAt(_index);
                db_Blips.currentBlips.Items.RemoveAt(_index);
                db_Blips.SaveChanges();
                API.sendChatMessageToPlayer(sender, "~y~ Blip başarıyla kaldırıldı.");
                #endregion
            }
            else
            if ("entrance".StartsWith(type.ToLower()))
            {
                if (!(API.getEntityData(sender, "AdminLevel") >= 3)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
                #region Entrance
                if (!(API.getEntityData(sender, "AdminLevel") >= 3)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
                var _index = db_Entrances.FindBlipIndexById(Convert.ToInt32(value));
                API.deleteEntity(EntranceManager.MarkersOnMap[_index]);
                db_Entrances.currentEntrances.Items.RemoveAt(_index);
                EntranceManager.MarkersOnMap.RemoveAt(_index);
                db_Entrances.SaveChanges();
                API.sendChatMessageToPlayer(sender, "~y~ Entrance başarıyla kaldırıldı.");
                #endregion
            }
            else
            if ("salevehicle".StartsWith(type.ToLower()) || "sv".StartsWith(type.ToLower()))
            {
                if (!(API.getEntityData(sender, "AdminLevel") >= 3)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
                #region SaleVehicle
                try
                {
                    if (!(API.getEntityData(sender, "AdminLevel") >= 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
                    db_SaleVehicles.RemoveSaleVehicleFully(Convert.ToInt32(value));
                    API.sendNotificationToPlayer(sender, "~y~Başarıyla kaldırıldı.");
                }
                catch (Exception)
                {
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bir hata oluştu. Parametreleri kontrol edin.");
                }
                #endregion
            }
            else
            if ("shop".StartsWith(type.ToLower()))
            {
                if (!(API.getEntityData(sender, "AdminLevel") >= 3)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
                #region Shop

                try
                {
                    var _Index = db_Shops.FindShopIndexById(Convert.ToInt32(value));
                    API.deleteEntity(db_Shops.CurrentShopsList[_Index].MarkerOnMap);
                    if (db_Shops.CurrentShopsList.Remove(db_Shops.CurrentShopsList.FirstOrDefault(x => x.ShopId == Convert.ToInt32(value))))
                    {
                        API.sendChatMessageToPlayer(sender, "~~s~Başarıyla ~g~silindi.");
                    }
                    else
                    {
                        API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Silinemedi.");
                    }
                    //db_Shops.CurrentShopsList.Remove(db_Shops.CurrentShopsList.Keys.ToList()[_Index]);
                    //db_Shops.CurrentShopsList.Remove(db_Shops.CurrentShopsList.Keys.FirstOrDefault(x => x.ShopId == Convert.ToInt32(value)));
                    db_Shops.SaveChanges();
                }
                catch (Exception)
                {
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Hatalı parametre girildi.");
                }
                #endregion
            }
            else
            if ("house".StartsWith(type.ToLower()))
            {
                if (!(API.getEntityData(sender, "AdminLevel") >= 3)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
                #region House
                try { Convert.ToInt32(value); } catch (Exception) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Ev ID'si sayı olmalıydı."); return; }
                if (db_Houses.RemoveHouse(Convert.ToInt32(value)))
                {
                    API.sendNotificationToPlayer(sender, "~g~Başarıyla Silindi.");
                }
                else
                {
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~ Bir hata oluştu. Enis ile iletişime geçin.");
                }
                #endregion
            }
            else
            if ("busstop".StartsWith(type.ToLower()))
            {
                if (!(API.getEntityData(sender, "AdminLevel") >= 3)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
                #region BusStop
                try { Convert.ToInt32(value); } catch (Exception) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~BusStop ID'si sayı olmalıydı."); return; }
                if (db_BusJob.RemoveBusStop(Convert.ToInt32(value)))
                {
                    API.sendNotificationToPlayer(sender, "Bus Stop silindi.");
                }
                else
                {
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~ Bir hata oluştu. Enis ile iletişime geçin.");
                }
                #endregion
            }
            else
            if ("tirdeliverypoint".StartsWith(type.ToLower()))
            {
                if (!(API.getEntityData(sender, "AdminLevel") >= 3)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
                #region TirDelivery
                try { Convert.ToInt32(value); } catch (Exception) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~DeliveryPoint ID'si sayı olmalıydı."); return; }
                if (db_TirJob.RemoveTirDeliveryPoint(Convert.ToInt32(value)))
                {
                    API.sendChatMessageToPlayer(sender, "Delivery Point silindi.");
                }
                else
                {
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~ Bir hata oluştu. Enis ile iletişime geçin.");
                }
                #endregion
            }
            else
            if ("kamyondeliverypoint".StartsWith(type.ToLower()))
            {
                if (!(API.getEntityData(sender, "AdminLevel") >= 3)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
                #region KamyonDelivery
                try { Convert.ToInt32(value); } catch (Exception) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~DeliveryPoint ID'si sayı olmalıydı."); return; }
                if (db_KamyonJob.RemoveTirDeliveryPoint(Convert.ToInt32(value)))
                {
                    API.sendChatMessageToPlayer(sender, "Delivery Point silindi.");
                }
                else
                {
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~ Bir hata oluştu. Enis ile iletişime geçin.");
                }
                #endregion
            }
            else
            if ("gasstation".StartsWith(type.ToLower()))
            {
                if (!(API.getEntityData(sender, "AdminLevel") >= 3)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
                #region gasstation
                if (db_GasStations.Remove(Convert.ToInt32(value)))
                {
                    API.sendChatMessageToPlayer(sender, "~g~BAŞARILI: ~s~Benzinlik kaldırıldı.");
                }
                else
                {
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Benzinlik kaldırılamadı.");
                }
                #endregion
            }
            else
            if ("bank".StartsWith(type.ToLower()))
            {
                if (!(API.getEntityData(sender, "AdminLevel") >= 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
                #region bank
                try
                {
                    if (db_Banks.Remove(Convert.ToInt32(value)))
                    {
                        API.sendChatMessageToPlayer(sender, "~y~ATM Başarıyla kaldırıldı.");
                    }
                    else
                        API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~ATM kaldırılamadı.");

                }
                catch (Exception ex)
                {
                    if (ex.GetType() == typeof(FormatException))
                    {
                        API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~ATM ID sayı olmalıydı.");
                    }
                }
                #endregion
            }
            else
            if ("vehlicpoint".StartsWith(type.ToLower()))
            {
                if (!(API.getEntityData(sender, "AdminLevel") >= 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
                #region VehicleLicenseCheckPoint
                try
                {
                    if (db_LicensePoints.RemoveVehLicenseCheckpoint(Convert.ToInt32(value)))
                    {
                        API.sendChatMessageToPlayer(sender, "~y~Öge başarıyla silindi.");
                    }
                    else
                    {
                        API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Öge silinemedi.");
                    }
                }
                catch (Exception ex)
                {
                    if (ex.GetType() == typeof(FormatException))
                    {
                        API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Girilen id sayı olmalıydı.");
                    }
                    else
                        API.shared.consoleOutput(LogCat.Warn, ex.ToString());
                }
                #endregion
            }
            else
            if ("building".StartsWith(type.ToLower()))
            {
                if (!(API.getEntityData(sender, "AdminLevel") >= 3)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
                #region RemoveBuilding
                if (db_Buildings.RemoveBuilding(Convert.ToInt32(value)))
                {
                    API.sendChatMessageToPlayer(sender, "~y~Building silindi.");
                }
                else
                {
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Silinemedi.");
                }
                #endregion
            }
            else
            if ("ctable".StartsWith(type.ToLower()))
            {
                if (!(API.getEntityData(sender, "AdminLevel") >= 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
                if (db_Craftings.RemoveCraftingTableOnMap(Convert.ToInt32(value)))
                {
                    API.shared.sendChatMessageToPlayer(sender, "~y~Başarıyla silindi.");
                }
                else
                {
                    API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Silinemedi.");
                }
            }
            else
            if ("fi".StartsWith(type.ToLower()))
            {
                if (!(API.getEntityData(sender, "AdminLevel") >= 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
                if (db_FactionInteractives.Remove(Convert.ToInt32(value)))
                {
                    API.shared.sendChatMessageToPlayer(sender, "Kaldırıldı.");
                }
                else
                {
                    API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Kaldırılamadı.");

                }
            }

        }
        [Command("removeweapon", "/removeweapon [(player) Name] [weaponname/All]")]
        public void RemoveWeapon(Client sender, string targetPlayer, string _weapon)
        {
            if (!(API.getEntityData(sender, "AdminLevel") > 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            foreach (var item in API.getAllPlayers())
            {
                if (item.socialClubName.ToLower().StartsWith(targetPlayer.ToLower()) || item.nametag.ToLower().StartsWith(targetPlayer.ToLower()))
                {
                    if (_weapon.ToLower() == "all")
                    {
                        API.removeAllPlayerWeapons(item);
                        API.sendChatMessageToPlayer(sender, "~y~" + item.name + " adlı kişinin tüm silahları kaldırıldı.");
                        API.sendChatMessageToPlayer(item, "~y~Tüm silahlarınıza el konuldu.");
                    }
                    else
                    {
                        API.removePlayerWeapon(item, API.weaponNameToModel(_weapon));
                        API.sendChatMessageToPlayer(sender, "~y~" + item.name + " adlı kişinin silahı kaldırıldı.");
                        API.sendChatMessageToPlayer(item, "~y~" + _weapon + " adlı silahınıza el konuldu.");
                    }

                }
            }
        }

        [Command("createlabel", "/createlabel [text] [size]")]
        public void CreateLabel(Client sender, string text, float size, float range)
        {
            if (!(API.getEntityData(sender, "AdminLevel") > 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            API.createTextLabel(text, sender.position + new Vector3(0, 0, 1.5f), range, size);
        }

        [Command("veh", "/veh [spawn/setpark] [name/vehid]", GreedyArg = true)]
        public void Veh(Client sender, string type, string identity)
        {
            if (!(API.getEntityData(sender, "AdminLevel") > 0)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            if ("spawn".StartsWith(type.ToLower()))
            {
                if (!sender.isInVehicle)
                {
                    var _RequestedVehicle = API.vehicleNameToModel(identity);
                    API.consoleOutput("Requested Vehicle " + _RequestedVehicle.ToString());

                    if (_RequestedVehicle.ToString() == "0")
                    {
                        API.sendChatMessageToPlayer(sender, "Geçersiz araç");
                        return;
                    }
                    Database.db_Vehicles dbVeh = new Database.db_Vehicles();
                    if (db_Vehicles.CreateVehicle(_RequestedVehicle, sender.position, sender.rotation, sender.dimension))
                    {
                        API.setPlayerIntoVehicle(sender, db_Vehicles.GetAll().LastOrDefault().VehicleOnMap, -1);
                    }
                    else
                    {
                        API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Hatalı parametre");
                    }
                }
                else
                {
                    API.sendChatMessageToPlayer(sender, "Şu anda araçtayken yeni araç alamazsınız.");
                }
            }

            if ("setpark".StartsWith(type.ToLower()))
            {
                var _vehicle = db_Vehicles.GetVehicle(Convert.ToInt64(identity));
                if (_vehicle != null)
                {
                    sender.position += new Vector3(0, 0, 1);
                    _vehicle.VehicleOnMap.position = sender.position;
                    _vehicle.LastRotation = sender.rotation;
                    _vehicle.VehicleOnMap.dimension = sender.dimension;
                    _vehicle.IsLocked = (_vehicle.JobId >= 0 ? false : true);
                    db_Vehicles.UpdateVehicleToModel(_vehicle);

                    API.sendChatMessageToPlayer(sender, "~y~ Aracınız park edildi.");
                    return;
                }
                else
                {
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Araç bulunamadı.");
                }

            }


        }

        [Command("createobject", "/createobject [ModelId] [X] [Y] [Z] [dimension = 0]")]
        public void CreateObject(Client sender, int modelId, float x, float y, float z, int dimension)
        {
            if (!(API.getEntityData(sender, "AdminLevel") > 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            var _createdObject = API.createObject(modelId, new Vector3(x, y, z), sender.rotation, dimension);
            db_Objects.CreateObject(new Models.GameObject
            {
                Dimension = dimension,
                FactionId = 0,
                FirstPosition = new Vector3(x, y, z),
                Rotation = sender.rotation,
                ModelId = modelId,
                OwnerSocialClubName = ""
            });
            db_Objects.SaveChanges();
            GameObjectsManager.ObjectsOnMap.Add(_createdObject);
        }
        [Command("objid", "/objid [range]")]
        public void ObjectId(Client sender, int range)
        {
            for (int i = 0; i < GameObjectsManager.ObjectsOnMap.Count; i++)
            {
                if (Vector3.Distance(GameObjectsManager.ObjectsOnMap[i].position, sender.position) < range)
                {
                    API.sendChatMessageToPlayer(sender, "~y~Obje ID : ~w~" + db_Objects.GetObjectIdByIndex(i) + " ~y~Model : ~w~" + db_Objects.currentObjectList.Items[i].ModelId);
                }
            }
        }

        [Command("attach", "/attach [player] [targetPlayer]")]
        public void Attach(Client sender, string player, string targetPlayer)
        {
            if (!(API.getEntityData(sender, "AdminLevel") > 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            foreach (var _player in API.getAllPlayers())
            {
                if (API.getEntityData(_player, "ID") == Convert.ToInt32(player) || _player.socialClubName.ToLower().StartsWith(player.ToLower()) || _player.nametag.ToLower().StartsWith(player.ToLower()))
                {
                    foreach (var _targetPlayer in API.getAllPlayers())
                    {

                        if (API.getEntityData(_targetPlayer, "ID") == Convert.ToInt32(targetPlayer))
                        {
                            _player.attachTo(_targetPlayer, "SKEL_L_Finger01", new Vector3(1, 0, 0), _targetPlayer.rotation);
                            break;
                        }
                        return;
                    }
                }
            }
        }
        [Command("entercar", "/entercar [vehid]")]
        public void EnterCar(Client sender, long VehicleId)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            try
            {
                var _vehicle = db_Vehicles.GetVehicle(VehicleId);
                if (_vehicle != null)
                {
                    API.setPlayerIntoVehicle(sender, _vehicle.VehicleOnMap, -1);
                }
                else
                {
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Araç bulunamadı.");
                }
            }
            catch (Exception ex)
            {
                //if (ex.GetType() == typeof(IndexOutOfRangeException))
                //{
                //    API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Araç bulunamadı.");
                //}
                API.consoleOutput(LogCat.Warn, ex.ToString());
            }
        }

        [Command("kill", "/kill [player]")]
        public void KillPlayer(Client sender, string targetPlayer)
        {
            if (!(API.getEntityData(sender, "AdminLevel") > 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            foreach (var _player in API.getAllPlayers())
            {
                if (API.getEntityData(_player, "ID") == Convert.ToInt32(targetPlayer) || _player.socialClubName.ToLower().StartsWith(targetPlayer.ToLower()) || _player.nametag.ToLower().StartsWith(targetPlayer.Replace("_", " ").ToLower()))
                {
                    _player.kill();
                    API.sendNotificationToPlayer(sender, _player.nametag + " öldü.");
                    //API.sendNotificationToPlayer(_player," Admin tarafından öldürüldünüz.");
                }
            }
        }
        [Command("detach", "/detach [player]")]
        public void Detach(Client sender, string player)
        {
            if (!(API.getEntityData(sender, "AdminLevel") > 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            foreach (var _player in API.getAllPlayers())
            {
                if (API.getEntityData(_player, "ID") == Convert.ToInt32(player) || _player.socialClubName.ToLower().StartsWith(player.ToLower()) || _player.nametag.ToLower().StartsWith(player.Replace("_", " ").ToLower()))
                {
                    _player.detach();
                }
            }
        }

        [Command("dimension", "/dim [vehicle/player] [vehid/name] [dimension(default 0 )]", Alias = "dim")]
        public void DimensionChange(Client sender, string type, string identity, int dim)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            if ("vehicle".StartsWith(type.ToLower()) || "car".StartsWith(type.ToLower()))
            {
                try
                {
                    var _vehicle = db_Vehicles.GetVehicle(Convert.ToInt64(identity));
                    if (_vehicle != null)
                    {
                        _vehicle.VehicleOnMap.dimension = dim;
                        API.sendNotificationToPlayer(sender, "Aracın dimension'ı " + dim + " olarak ayarlandı.");
                    }
                }
                catch (Exception ex)
                {
                    API.consoleOutput("ADMINCOMMANDS - DIMENSION | " + ex.Message);
                }
            }
            if ("player".StartsWith(type.ToLower()))
            {
                foreach (var _player in API.getAllPlayers())
                {
                    if (API.getEntityData(_player, "ID") == Convert.ToInt32(identity) || _player.socialClubName.ToLower().StartsWith(identity.ToLower()) || _player.nametag.ToLower().StartsWith(identity.Replace("_", " ").ToLower()))
                    {
                        _player.dimension = dim;
                        API.sendNotificationToPlayer(sender, _player.nametag + "adlı oyuncu'nun dimension'u " + dim + " olarak ayarlandı.");
                    }
                }
            }
        }

        [Command("addmarker")]
        public void AddMarker(Client sender, int x, int y, int z)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            //update_marker
            API.triggerClientEvent(sender, "update_marker", x, y, z);
        }

        [Command("cc", "/cc")]
        public void ClearChat(Client sender)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            string strEmpy = "";
            for (int i = 0; i < 42; i++)
            {
                strEmpy += " \n ";
            }
            API.sendChatMessageToAll(strEmpy);
        }

        [Command("createblip", "/createblip [name] [model] [color] [range 1-0] [dimension]")]
        public void CreateBlip(Client sender, string _name, int _model, int _color, int _range, int _dimension)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }

            _name = _name.Replace("_", " ");

            var _createdBlip = API.createBlip(sender.position, _range, _dimension);
            _createdBlip.sprite = _model;
            _createdBlip.name = _name;
            _createdBlip.routeColor = _color;
            _createdBlip.color = _color;

            BlipManager.BlipsOnMap.Add(_createdBlip);
            db_Blips.AddBlip(new Models.Blip { Color = _color, Dimension = _dimension, ModelId = _model, Name = _name, Position = sender.position, Range = _range });
        }

        [Command("blipid", "/blipid [range]")]
        public void BlipId(Client sender, int range)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }

            for (int i = 0; i < BlipManager.BlipsOnMap.Count; i++)
            {
                if (Vector3.Distance(sender.position, BlipManager.BlipsOnMap[i].position) < range)
                {
                    API.sendChatMessageToPlayer(sender, " - " + BlipManager.BlipsOnMap[i].name + " ~b~(" + db_Blips.FindBlipIdByIndex(i) + ")");
                }
            }
        }
        //[Command("editblip")]
        //public void EditBlip(Client sender,int blipId)
        //{
        //    if (!(API.getEntityData(sender, "AdminLevel") >= 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
        //    var _blip = db_Blips
        //}
        [Command("createentrance", "/createentrance [Name] [MarkerType]")]
        public void CreateEntrance(Client sender, string name, int markerType)
        {
            //Adminlik Kontrolü
            if (!(API.getEntityData(sender, "AdminLevel") >= 3)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }

            Vector3 pos = new Vector3();
            if (markerType == 1) pos = sender.position - new Vector3(0, 0, 1.0f);
            else pos = sender.position;

            var _createdMarker = API.createMarker(markerType, pos, new Vector3(0, 0, 0), sender.rotation, new Vector3(1, 1, 1), 100, 196, 6, 19, sender.dimension);
            EntranceManager.MarkersOnMap.Add(_createdMarker);

            name = name.Replace("_", " ");
            db_Entrances.AddEntrance(new Models.Entrance
            {
                Color = new Models.MarkerColor { Alpha = 100, Blue = 19, Green = 6, Red = 196, },
                Name = name == "-" ? null : name,
                Direction = new Vector3(0, 0, 0),
                EntrancePosition = pos,
                InteriorPosition = new Vector3(135, -749, 259),
                EntranceDimension = sender.dimension,
                MarkerType = markerType,
                Rotation = sender.rotation,
                Scale = 1
            });

            API.sendNotificationToPlayer(sender, "Entrance Eklendi.", true);

        }

        [Command("entranceid", "/entranceid")]
        public void EntranceId(Client sender)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 3)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }

            for (int i = 0; i < EntranceManager.MarkersOnMap.Count; i++)
            {
                if (Vector3.Distance(sender.position, EntranceManager.MarkersOnMap[i].position) < 5)
                {
                    API.sendChatMessageToPlayer(sender, " - Entrance ID ~b~(" + db_Entrances.currentEntrances.Items[i].ID + ")");
                }
            }
        }

        [Command("editentrance", "/... [id] [~y~color/~g~interior/~b~scale/~o~pos/~p~type] \n [~y~A_R_G_B / ~g~X_Y_Z -me /~b~SAYI/~o~X_Y_Z - me/~p~type]")]
        public void EditEntrance(Client sender, int _id, string type, string value)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 3)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            var _index = db_Entrances.FindBlipIndexById(_id);

            if ("color".StartsWith(type.ToLower()))
            {
                try
                {
                    string[] coord = value.Split('_');
                    EntranceManager.MarkersOnMap[_index].color = new GrandTheftMultiplayer.Server.Constant.Color(Convert.ToInt32(coord[1]), Convert.ToInt32(coord[2]), Convert.ToInt32(coord[3]), Convert.ToInt32(coord[0]));
                    db_Entrances.currentEntrances.Items[_index].Color = new Models.MarkerColor { Alpha = Convert.ToInt32(coord[0]), Red = Convert.ToInt32(coord[1]), Green = Convert.ToInt32(coord[2]), Blue = Convert.ToInt32(coord[3]) };
                    coord = null;
                    API.sendNotificationToPlayer(sender, "Renk güncellendi.");
                }
                catch (Exception ex)
                {
                    API.consoleOutput("EDIT DIMENSION HATA : " + ex.Message);
                    API.sendNotificationToPlayer(sender, "İşlem başarısız.", false);
                }
            }
            else if ("interior".StartsWith(type.ToLower()))
            {
                try
                {
                    if ("me".StartsWith(value.ToLower()))
                    {
                        db_Entrances.currentEntrances.Items[_index].InteriorPosition = sender.position;
                        db_Entrances.currentEntrances.Items[_index].InteriorDimension = sender.dimension;
                        db_Entrances.SaveChanges();
                        return;
                    }
                    string[] coord = value.Split('_');
                    db_Entrances.currentEntrances.Items[_index].InteriorPosition = new Vector3(Convert.ToSingle(coord[0]), Convert.ToSingle(coord[1]), Convert.ToSingle(coord[2]));
                    coord = null;
                    API.sendNotificationToPlayer(sender, "interior güncellendi.");
                }
                catch (Exception ex)
                {
                    API.consoleOutput("EDIT DIMENSION HATA : " + ex.Message);
                    API.sendNotificationToPlayer(sender, "İşlem başarısız.", false);
                }

            }
            else if ("scale".StartsWith(type.ToLower()))
            {
                try
                {
                    EntranceManager.MarkersOnMap[_index].scale = new Vector3(Convert.ToSingle(value), Convert.ToSingle(value), Convert.ToSingle(value));
                    db_Entrances.currentEntrances.Items[_index].Scale = Convert.ToInt32(value);
                    API.sendNotificationToPlayer(sender, "Scale güncellendi.");
                }
                catch (Exception ex)
                {
                    API.consoleOutput("EDIT DIMENSION HATA : " + ex.Message);
                    API.sendNotificationToPlayer(sender, "İşlem başarısız.", false);
                }
            }
            else if ("pos".StartsWith(type.ToLower()))
            {
                try
                {
                    if ("me".StartsWith(value.ToLower()))
                    {
                        if (db_Entrances.currentEntrances.Items[_index].MarkerType == 1)
                            EntranceManager.MarkersOnMap[_index].position = sender.position + new Vector3(0, 0, -1.0f);
                        else EntranceManager.MarkersOnMap[_index].position = sender.position;
                        db_Entrances.currentEntrances.Items[_index].EntrancePosition = sender.position;
                        db_Entrances.currentEntrances.Items[_index].EntranceDimension = sender.dimension;
                        EntranceManager.MarkersOnMap[_index].dimension = sender.dimension;
                        db_Entrances.SaveChanges();
                        return;
                    }
                    string[] coord = value.Split('_');
                    EntranceManager.MarkersOnMap[_index].position = new Vector3(Convert.ToSingle(coord[0]), Convert.ToSingle(coord[1]), Convert.ToSingle(coord[2]));
                    db_Entrances.currentEntrances.Items[_index].EntrancePosition = new Vector3(Convert.ToSingle(coord[0]), Convert.ToSingle(coord[1]), Convert.ToSingle(coord[2]));
                    db_Entrances.currentEntrances.Items[_index].EntranceDimension = sender.dimension;
                    EntranceManager.MarkersOnMap[_index].dimension = sender.dimension;
                    API.sendNotificationToPlayer(sender, "Renk güncellendi.");
                }
                catch (Exception ex)
                {
                    API.consoleOutput("EDIT DIMENSION HATA : " + ex.Message);
                    API.sendNotificationToPlayer(sender, "İşlem başarısız.", false);
                }
            }
            else
                if ("type".StartsWith(type.ToLower()))
            {
                db_Entrances.currentEntrances.Items[_index].MarkerType = Convert.ToInt32(value);
                EntranceManager.MarkersOnMap[_index].markerType = Convert.ToInt32(value);
            }
            db_Entrances.SaveChanges();

        }
        [Command("editveh", "/editveh ~y~[faction/job/plate/color/owner]~s~ [vehid] [value]", Alias = "edit", GreedyArg = true)]
        public void EditVehicle(Client sender, string type, long vehid, string value)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            var _vehicle = db_Vehicles.GetVehicle(vehid);
            if (_vehicle != null)
            {
                if ("faction".StartsWith(type.ToLower()))
                {
                    _vehicle.FactionId = Convert.ToInt32(value);
                    _vehicle.JobId = -1;
                }
                if ("job".StartsWith(type.ToLower()))
                {
                    _vehicle.FactionId = -1;
                    _vehicle.JobId = Convert.ToInt32(value);
                }
                if ("plate".StartsWith(type.ToLower()))
                {
                    _vehicle.Plate = value;
                    _vehicle.VehicleOnMap.numberPlate = value;
                }
                if (type.ToLower().StartsWith("color"))
                {
                    var splitted = value.Split(' ');
                    if (splitted.Length < 2)
                    {
                        API.sendChatMessageToPlayer(sender, "/editveh color [vehid] ~y~[Color 1] [Color 2]"); return;
                    }

                    _vehicle.Color1 = Convert.ToInt32(splitted[0]);
                    _vehicle.Color2 = Convert.ToInt32(splitted[1]);
                    API.setVehiclePrimaryColor(_vehicle.VehicleOnMap, Convert.ToInt32(splitted[0]));
                    API.setVehicleSecondaryColor(_vehicle.VehicleOnMap, Convert.ToInt32(splitted[1]));
                    //_vehicle.VehicleOnMap.primaryColor = Convert.ToInt32(splitted[0]);
                    //_vehicle.VehicleOnMap.secondaryColor = Convert.ToInt32(splitted[1]);
                    db_Vehicles.SaveChanges();
                    API.sendChatMessageToPlayer(sender, "~y~Renkler güncellendi.");
                }
                if ("owner".StartsWith(type.ToLower()))
                {
                    _vehicle.OwnerSocialClubName = value;
                }
                db_Vehicles.SaveChanges();
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Araç bulunamadı.");
            }
        }

        [Command("setnametagcolor", "/setnametagcolor [Player] [Color(R_G_B)]")]
        public void Setnametagcolor(Client sender, string player, string color)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            player = player.Replace("_", " ");
            string[] colors = color.Split('_');
            foreach (var _player in API.getAllPlayers())
            {
                if (API.getEntityData(_player, "ID") == Convert.ToInt32(player))
                {
                    _player.nametagColor = new Color { alpha = 255, red = Convert.ToInt32(colors[0]), green = Convert.ToInt32(colors[1]), blue = Convert.ToInt32(colors[1]) };
                }
            }
        }

        [Command("opendoor", "/opendoor [Door(0-5)]")]
        public void OpenDoor(Client sender, int door)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            var _vehicle = db_Vehicles.FindNearestVehicle(sender.position);

            if (Vector3.Distance(sender.position, _vehicle.VehicleOnMap.position) < 3)
            {
                _vehicle.VehicleOnMap.openDoor(door);
                return;
            }

        }

        [Command("createsalevehicle", "/csv [VehicleModel] [Price]", Alias = "csv")]
        public void CreateSaleVehicle(Client sender, string vehicleName, int price)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            var _vehModel = API.vehicleNameToModel(vehicleName);
            if (_vehModel.ToString() == "0") { API.sendNotificationToPlayer(sender, "Geçersiz Araç!"); return; }

            SaleVehicleManager.SaleVehiclesOnMap.Add(API.createVehicle(_vehModel, sender.position, sender.rotation, 5, 10, 0));
            db_SaleVehicles.AddSaleVehicle(new Models.SaleVehicle
            {
                Interaction = new Models.AttributeData2<bool> { Buy = true, Rent = false },
                Position = new Models.AttributeData3<float> { X = sender.position.X, Y = sender.position.Y, Z = sender.position.Z + 2 },
                DeliverPosition = new Models.AttributeData3<float> { X = sender.position.X + 2, Y = sender.position.Y + 1, Z = sender.position.Z + 2 },
                DeliverRotation = new Models.AttributeData3<float> { X = sender.rotation.X, Y = sender.rotation.Y, Z = sender.rotation.Z },
                Price = new Models.AttributeData2<int> { Buy = price, Rent = -1 },
                Rotation = new Models.AttributeData3<float> { X = sender.rotation.X, Y = sender.rotation.Y, Z = sender.rotation.Z },
                VehicleColors = new Models.Colors { Color_1 = 5, Color_2 = 10 },
                VehicleModel = _vehModel,
                Dimension = sender.dimension
            });
            API.setVehicleEngineStatus(SaleVehicleManager.SaleVehiclesOnMap.LastOrDefault(), false);
        }
        [Command("salevehicleid", "/salevehicleid [range]", Alias = "svid")]
        public void SaleVehicleId(Client sender, int range)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            for (int i = 0; i < SaleVehicleManager.SaleVehiclesOnMap.Count; i++)
            {
                if (Vector3.Distance(sender.position, SaleVehicleManager.SaleVehiclesOnMap[i].position) < range)
                {
                    API.sendChatMessageToPlayer(sender, "~y~" + SaleVehicleManager.SaleVehiclesOnMap[i].model.ToString() + " ~s~(" + db_SaleVehicles.FindSaleVehicleIdByIndex(i) + ")");
                }
            }
        }
        [Command("houseid", Alias = "hid")]
        public void HouseId(Client sender)
        {
            foreach (var item in db_Houses.CurrentHousesDict.Values)
            {
                if (Vector3.Distance(sender.position, item.EntrancePosition) < 5)
                {
                    API.sendChatMessageToPlayer(sender, "~y~(" + item.HouseId + ") ~w~" + item.Name);
                }
            }
        }

        [Command("editsalevehicle", "/esv [ID] [buy/rent/color/pos/deliverpos] [value]", Alias = "esv", GreedyArg = true)]
        public void EditSaleVehicle(Client sender, int _Id, string type, string value)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            var _Index = db_SaleVehicles.FindSaleVehicleIndexById(_Id);
            if ("buy".StartsWith(type.ToLower()))
            {
                try { Convert.ToInt32(value); } catch (Exception) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Fiyat sayı girilmeli."); }
                db_SaleVehicles.currentSaleVehicleList.Items[_Index].Price.Buy = Convert.ToInt32(value);
                if (Convert.ToInt32(value) < 0) db_SaleVehicles.currentSaleVehicleList.Items[_Index].Interaction.Buy = false;
                else db_SaleVehicles.currentSaleVehicleList.Items[_Index].Interaction.Buy = true;
                API.sendNotificationToPlayer(sender, "~y~Satış fiyatı " + value + " olarak güncellendi.");
            }
            if ("rent".StartsWith(type.ToLower()))
            {
                try
                {
                    db_SaleVehicles.currentSaleVehicleList.Items[_Index].Price.Rent = Convert.ToInt32(value);
                    API.sendNotificationToPlayer(sender, "~y~Kiralama fiyatı " + value + " olarak güncellendi.");
                    if (Convert.ToInt32(value) < 0) db_SaleVehicles.currentSaleVehicleList.Items[_Index].Interaction.Rent = false;
                    else db_SaleVehicles.currentSaleVehicleList.Items[_Index].Interaction.Rent = true;

                }
                catch (Exception ex) { if (ex.GetType() == typeof(FormatException)) API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Fiyat sayı girilmeli."); }
            }
            if ("color".StartsWith(type.ToLower()))
            {
                SaleVehicleManager.SaleVehiclesOnMap[_Index].primaryColor = Convert.ToInt32(value.Split(' ')[0]);
                SaleVehicleManager.SaleVehiclesOnMap[_Index].secondaryColor = Convert.ToInt32(value.Split(' ')[1]);
                db_SaleVehicles.currentSaleVehicleList.Items[_Index].VehicleColors = new Models.Colors { Color_1 = Convert.ToInt32(value.Split(' ')[0]), Color_2 = Convert.ToInt32(value.Split(' ')[0]) };
                API.sendNotificationToPlayer(sender, "~y~Aracın rengi " + value + " olarak güncellendi.");
            }
            if ("position".StartsWith(type.ToLower()))
            {
                SaleVehicleManager.SaleVehiclesOnMap[_Index].position = sender.position;
                SaleVehicleManager.SaleVehiclesOnMap[_Index].rotation = sender.rotation;
                SaleVehicleManager.SaleVehiclesOnMap[_Index].dimension = sender.dimension;
                db_SaleVehicles.currentSaleVehicleList.Items[_Index].Position = new Models.AttributeData3<float> { X = sender.position.X, Y = sender.position.Y, Z = sender.position.Z };
                db_SaleVehicles.currentSaleVehicleList.Items[_Index].Rotation = new Models.AttributeData3<float> { X = sender.rotation.X, Y = sender.rotation.Y, Z = sender.rotation.Z };
                sender.position += new Vector3(0, 0, 1);

                API.sendNotificationToPlayer(sender, "~y~Pozisyonu konumunuz olarak güncellendi.");
            }
            if ("deliverpos".StartsWith(type.ToLower()))
            {
                db_SaleVehicles.currentSaleVehicleList.Items[_Index].DeliverPosition = new Models.AttributeData3<float> { X = sender.position.X, Y = sender.position.Y, Z = sender.position.Z };
                db_SaleVehicles.currentSaleVehicleList.Items[_Index].DeliverRotation = new Models.AttributeData3<float> { X = sender.rotation.X, Y = sender.rotation.Y, Z = sender.rotation.Z };
                db_SaleVehicles.currentSaleVehicleList.Items[_Index].DeliverDimension = sender.dimension;

                API.sendNotificationToPlayer(sender, "~y~Teslim Pozisyonu konumunuz olarak güncellendi.");
            }
            db_SaleVehicles.SaveChanges();
        }

        [Command("createhouse", "/cs [price]", Alias = "cs")]
        public void Create(Client sender, string value)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            HouseMarkerColor hmc = new HouseMarkerColor();
            db_Houses.CreateHouse(new House
            {
                EntranceDimension = sender.dimension,
                EntrancePosition = sender.position,
                EntranceRotation = sender.rotation * -1,
                InteriorPosition = new Vector3(266, -1007, -101),
                InteriorRotation = sender.rotation,
                IsSelling = true,
                IsLocked = true,
                Price = Convert.ToInt32(value),
                MarkerType = 0,
                Name = "Ev",
            });
        }
        [Command("edithouse", "/eh [HouseId] ~y~[position/name/interior/markertype/selling/price]~s~ [value]")]
        public void EditHouse(Client sender, int identity, string type, string value)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            if ("interior".StartsWith(type.ToLower()))
            {
                try
                {
                    var _house = db_Houses.GetHouse(Convert.ToInt32(identity));
                    _house.InteriorPosition = sender.position;
                    _house.InteriorRotation = sender.rotation;
                    _house.InteriorDimension = sender.dimension;
                    db_Houses.SaveChanges();
                    API.sendNotificationToPlayer(sender, "~y~Interior, pozisyonunuz olarak güncellendi.");
                }
                catch (Exception ex)
                {
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bir hata oluştu. Enis ile iletişime geçin.");
                    API.consoleOutput("edithouse | interior : " + ex);
                }
            }
            if ("position".StartsWith(type.ToLower()))
            {
                try
                {
                    var _house = db_Houses.GetHouse(Convert.ToInt32(identity));
                    _house.EntrancePosition = sender.position;
                    _house.EntranceRotation = sender.rotation;
                    _house.EntranceDimension = sender.dimension;
                    db_Houses.UpdateHouse(_house);
                    API.sendNotificationToPlayer(sender, "~y~Entrance, pozisyonunuz olarak güncellendi.");
                }
                catch (Exception ex)
                {
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bir hata oluştu. Enis ile iletişime geçin.");
                    API.consoleOutput("edithouse | pos : " + ex);
                }
            }
            if ("markertype".StartsWith(type.ToLower()))
            {
                try
                {
                    var _house = db_Houses.GetHouse(Convert.ToInt32(identity));
                    _house.MarkerType = Convert.ToInt32(value);
                    _house.MarkerOnMap.markerType = Convert.ToInt32(value);
                    db_Houses.UpdateHouse(_house);
                    API.sendNotificationToPlayer(sender, "~y~Markertype, " + value + " olarak güncellendi.");
                }
                catch (Exception ex)
                {
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bir hata oluştu. Enis ile iletişime geçin.");
                    API.consoleOutput("edithouse | pos : " + ex);
                }
            }
            if ("selling".StartsWith(type.ToLower()))
            {
                try
                {
                    try { Convert.ToBoolean(value); } catch (Exception) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Değer 1 veya 0 olmalı"); }
                    var _house = db_Houses.GetHouse(Convert.ToInt32(identity));

                    HouseMarkerColor hmc = new Models.HouseMarkerColor();
                    if (Convert.ToBoolean(value))
                    {
                        _house.MarkerOnMap.color = new GrandTheftMultiplayer.Server.Constant.Color { alpha = 255, red = hmc.SaleColor.Red, green = hmc.SaleColor.Green, blue = hmc.SaleColor.Blue };
                    }
                    else
                    {
                        _house.MarkerOnMap.color = new GrandTheftMultiplayer.Server.Constant.Color { alpha = 255, red = hmc.NormalColor.Red, green = hmc.NormalColor.Green, blue = hmc.NormalColor.Blue };
                    }
                    _house.IsSelling = Convert.ToBoolean(value);
                    db_Houses.UpdateHouse(_house);
                    API.sendNotificationToPlayer(sender, "~y~Satış, " + Convert.ToBoolean(value) + " olarak güncellendi.");
                }
                catch (Exception ex)
                {
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bir hata oluştu. Enis ile iletişime geçin.");
                    API.consoleOutput("edithouse | pos : " + ex);
                }
            }
            if ("price".StartsWith(type.ToLower()))
            {
                try
                {
                    var _house = db_Houses.GetHouse(Convert.ToInt32(identity));
                    _house.Price = Convert.ToInt32(value);
                    API.sendNotificationToPlayer(sender, "~y~Fiyatı, " + value + " olarak güncellendi.");
                    db_Houses.SaveChanges();
                }
                catch (Exception ex)
                {
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bir hata oluştu. Enis ile iletişime geçin.");
                    API.consoleOutput("edithouse | pos : " + ex);
                }
            }
            else
            if ("name".StartsWith(type.ToLower()))
            {
                var _house = db_Houses.GetHouse(Convert.ToInt32(identity));
                if (_house != null)
                {
                    _house.Name = value;
                    db_Houses.UpdateHouse(_house);
                }
            }
            else
            if ("owner".StartsWith(type.ToLower()))
            {
                var _house = db_Houses.GetHouse(Convert.ToInt32(identity));
                if (_house != null)
                {
                    _house.OwnerSocialClubName = value.Replace(" ", String.Empty);
                    db_Houses.UpdateHouse(_house);
                }
            }
        }

        [Command("createshop", "/createshop ~y~[range]")]
        public void CreateShop(Client sender, int range)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 3)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            db_Shops.CreateShop(new Shop
            {
                Scale = new Vector3(range, range, 1),
                Position = sender.position + new Vector3(0, 0, -1),
                Rotation = sender.rotation,
                Dimension = sender.dimension,
                MarkerColorRGB = new MarkerColor { Alpha = 255, Blue = 206, Green = 30, Red = 10 },
                MarkerType = 1,
                Range = range,
                SaleItemList = new List<SaleItem> { new SaleItem { Count = 1, GameItemId = 2, Price = 100 } }
            });
        }
        [Command("addshopitem", "/addshopitem ~y~[shopId] [ItemID] [StackCount] ~w~[Price]")]
        public void AddShopItem(Client sender, int shopId, int ItemId, int StackCount, int itemPrice)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 3)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            var _Index = db_Shops.FindShopIndexById(shopId);
            if (_Index >= 0)
            {
                try
                {
                    var _GameItem = db_Items.GetItemById(ItemId);
                    if (_GameItem != null)
                    {
                        db_Shops.CurrentShopsList.FirstOrDefault(x => x.ShopId == shopId).SaleItemList.Add(new SaleItem
                        {
                            Count = StackCount,
                            GameItemId = ItemId,
                            Price = itemPrice
                        });
                        db_Shops.SaveChanges();
                    }
                    else
                    {
                        API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Item bulunamadı.");
                    }

                    API.sendNotificationToPlayer(sender, "~y~" + _GameItem.Name + "adlı eşya satıcıya başarıyla eklendi.", true);
                }
                catch (Exception ex)
                {
                    API.consoleOutput("ADMINCOMMANDS | ADDSHOPITEM Error: " + ex);
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bir hata oluştu. Enis ile iletişime geçin :)");
                }
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Shop bulunamadı.");
            }
        }

        public void AddItemToShop(Client sender, params object[] args /*int shopId, string itemRange, string price*/)
        {
            int shopId = Convert.ToInt32(args[0]);
            string[] range = args[1].ToString().Split('-');
            int minId = Convert.ToInt32(range[0]);
            int maxId = Convert.ToInt32(range[1]);
            int price = Convert.ToInt32(args[2].ToString().Replace("$",string.Empty));

            for (int i = minId; i <= maxId; i++)
            {
                AddShopItem(sender, shopId, i, 1, price);
            }

        }

        [Command("removeshopitem", "/removeshopitem ~y~[ShopID] ~s~[ItemId]")]
        public void RemoveShopItem(Client sender, int _shopId, int _itemId)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 3)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            var _Index = db_Shops.FindShopIndexById(_shopId);
            if (_Index >= 0)
            {
                try
                {
                    foreach (var item in db_Shops.CurrentShopsList[_Index].SaleItemList)
                    {
                        if (item.GameItemId == _itemId)
                        {
                            db_Shops.CurrentShopsList.FirstOrDefault(x => x.ShopId == _shopId).SaleItemList.Remove(item);
                            API.sendNotificationToPlayer(sender, "~y~Başarıyla kaldırıldı.");
                            db_Shops.SaveChanges();
                            return;
                        }
                    }
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~ Item Bulunamadı.");
                }
                catch (Exception ex)
                {
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~ Bir hata oluştu.");
                    API.consoleOutput(LogCat.Error, ex.ToString());
                }
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Shop bulunamadı.");
            }
        }
        public void RemoveItemFromShop(Client sender, params object[] args)
        {
            RemoveShopItem(sender, Convert.ToInt32(args[0]), Convert.ToInt32(args[1]));
        }
        public void UpdatePriceShopItem(Client sender, params object[] args)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 3)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            var shopId = Convert.ToInt32(args[0]);
            var gameItemId = Convert.ToInt32(args[1]);
            var newPrice = Convert.ToInt32(args[2].ToString().Replace("$", string.Empty));

            var _shop = db_Shops.GetShop(shopId);

            var editedItem = _shop.SaleItemList.FirstOrDefault(x => x.GameItemId == gameItemId);
            editedItem.Price = newPrice;
            db_Shops.SaveChanges();
        }

        [Command("renameGameItem","/renameGameItem [GameItemID] [NewName]")]
        public void RenameGameItem(Client sender, params object[] args)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 3)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }

            var editedItem = db_Items.GetItemById(Convert.ToInt32(args[0]));

            editedItem.Name = args[1].ToString();

            db_Items.SaveChanges();

            API.shared.sendChatMessageToPlayer(sender, $"~o~[GameItems] ~w~ID: {args[0]}, eşyanın adı {args[1]} olarak güncellendi.");
        }
        [Command("shopid", "/shopid [range]")]
        public void Shopid(Client sender, int range)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            foreach (var item in db_Shops.CurrentShopsList)
            {
                if (Vector3.Distance(sender.position, item.Position) < range)
                {
                    API.sendChatMessageToPlayer(sender, "~y~Dim: ~s~" + item.Dimension + " ~y~ID: ~s~(" + item.ShopId + ")");
                }
            }
        }

        [Command("editshop", "/editshop ~y~[shopId] ~s~[position/color/range] [value/ R G B]", GreedyArg = true)]
        public void EditShop(Client sender, int shopId, string type, string value)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 3)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            if ("position".StartsWith(type.ToLower()))
            {
                try
                {
                    var _Index = db_Shops.FindShopIndexById(shopId);
                    if (shopId >= 0)
                    {
                        db_Shops.CurrentShopsList[_Index].Position = sender.position + new Vector3(0, 0, -1);
                        db_Shops.CurrentShopsList[_Index].Dimension = sender.dimension;
                        db_Shops.CurrentShopsList[_Index].MarkerOnMap.position = sender.position + new Vector3(0, 0, -1);
                        db_Shops.CurrentShopsList[_Index].MarkerOnMap.dimension = sender.dimension;
                        db_Shops.SaveChanges();
                        API.sendNotificationToPlayer(sender, "~y~Shop, pozisyonunuz olarak güncellendi.");
                        return;
                    }
                    else
                    {
                        API.sendChatMessageToPlayer(sender, "~r~HATA: Shop bulunamadı.");
                    }
                }
                catch (Exception ex)
                {
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bir hata oluştu. Enis ile iletişime geçin.");
                    API.consoleOutput("editShop | pos : " + ex);
                }
            }
            else
            if ("color".StartsWith(type.ToLower()))
            {
                var _Index = db_Shops.FindShopIndexById(shopId);
                if (_Index >= 0)
                {
                    try
                    {
                        db_Shops.CurrentShopsList[_Index].MarkerColorRGB = new MarkerColor { Red = Convert.ToInt32(value.Split(' ')[0]), Green = Convert.ToInt32(value.Split(' ')[1]), Blue = Convert.ToInt32(value.Split(' ')[2]) };
                        db_Shops.CurrentShopsList[_Index].MarkerOnMap.color = new Color(Convert.ToInt32(value.Split(' ')[0]), Convert.ToInt32(value.Split(' ')[1]), Convert.ToInt32(value.Split(' ')[2]));
                        db_Shops.SaveChanges();
                        return;

                    }
                    catch (Exception)
                    {
                        API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bir hata oluştu. Parametrelerinizi kontrol edin.");
                    }
                }
            }
            else
            if ("range".StartsWith(type.ToLower()))
            {
                var _Index = db_Shops.FindShopIndexById(shopId);
                if (_Index >= 0)
                {
                    try
                    {
                        db_Shops.CurrentShopsList[_Index].Range = Convert.ToInt32(value);
                        db_Shops.CurrentShopsList[_Index].MarkerOnMap.scale = new Vector3(Convert.ToInt32(value), Convert.ToInt32(value), 1);
                        db_Shops.SaveChanges();
                    }
                    catch (Exception)
                    {
                        API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bir hata oluştu. Parametrelerinizi kontrol edin.");
                    }
                }
            }
        }

        [Command("jobedit", "/jobedit [jobid] [pos/name/takingpoint] [me/value/-1]", GreedyArg = true)]
        public void JobEdit(Client sender, int jobId, string type, string value)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 3)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }

            var editedModel = db_Jobs.GetJob(jobId);
            if (editedModel != null)
            {
                if ("pos".StartsWith(type.ToLower()))
                {
                    if (value == "-1")
                    {
                        if (db_Jobs.Remove(editedModel.ID))
                            API.sendChatMessageToPlayer(sender, "~g~BAŞARILI: ~s~Meslek başarıyla kaldırıldı.");
                        else
                            API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Meslek kaldırılırken hata meydana geldi.");
                        return;
                    }


                    editedModel.Position = sender.position;
                    editedModel.Dimension = sender.dimension;
                }
                if ("name".StartsWith(type.ToLower()))
                {
                    editedModel.Name = value;
                }
                if ("takingpoint".StartsWith(type.ToLower()))
                {
                    editedModel.TakingPosition = sender.position;
                    editedModel.TakingDimension = sender.dimension;
                }


                if (db_Jobs.EditJob(editedModel))
                    API.sendNotificationToPlayer(sender, "İşlem başarılı");
                else
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bir hata oluştu.");
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bir hata oluştu. Enis ile iletişime geçin.");
            }

        }
        [Command("addbusstop")]
        public void AddBusStop(Client sender)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 3)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            if (db_BusJob.CreateStop(new BusStop { Dimension = sender.dimension, Position = sender.position }))
            {
                API.sendNotificationToPlayer(sender, "Bus stop eklendi.");
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Eklenemedi.");
            }
        }
        [Command("busstopid", "/busstopid [range]  alias = /bsi", Alias = "bsi")]
        public void BusStopId(Client sender, int range)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 3)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            foreach (var item in db_BusJob.CurrentBusStops.Item1)
            {
                if (Vector3.Distance(sender.position, item.Position) <= range)
                {
                    API.sendChatMessageToPlayer(sender, "~h~Bus Stop (" + item.ID + ")");
                }
            }
        }

        [Command("clearinventory", "/ci [playerID]")]
        public void ClearInventory(Client sender, int identity)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            foreach (var item in API.getAllPlayers())
            {
                if (API.getEntityData(item, "ID") == Convert.ToInt32(identity))
                {
                    var _inventory = (Inventory)API.getEntityData(item, "inventory");
                    _inventory.ItemList.Clear();
                    API.setEntityData(item, "inventory", _inventory);
                    return;
                }
            }
        }

        [Command("createarrest", "/createarrest [name]")]
        public void CreateArrest(Client sender, string _name)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 3)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            db_Arrests.CreateArrest(new Models.Arrest
            {
                ArrestPoint = sender.position + new Vector3(2, 1, 1),
                ArrestPointDimension = sender.dimension,
                Name = _name,
                Dimension = sender.dimension,
                Position = sender.position,
                Rotation = sender.rotation,
            });
        }

        [Command("arrestid", "/arrestid [range]")]
        public void ArrestId(Client sender, int range)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            foreach (var item in db_Arrests.currentArrests.Item1)
            {
                if (Vector3.Distance(sender.position, item.Position) < range)
                {
                    API.sendChatMessageToPlayer(sender, "~y~" + item.Name + "  ~h~~w~(" + item.ArrestId + ")");
                }
            }
        }
        [Command("editarrest", "/editarrest [arrestid] ~y~[position/arrestpoint/name] ~s~[me / Text]")]
        public void EditArrest(Client sender, int _arrestId, string type, string _name)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 3)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }

            var editedArrest = db_Arrests.GetArrestById(_arrestId);
            if ("position".StartsWith(type))
            {
                editedArrest.Position = sender.position;
                editedArrest.Dimension = sender.dimension;
                if (db_Arrests.UpdateArrest(editedArrest))
                {
                    API.sendNotificationToPlayer(sender, "Pozisyonu, pozisyonunuz olarak güncellendi.");
                }
                else
                {
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Güncellenemedi.");
                }
            }
            else
            if ("arrestpoint".StartsWith(type))
            {
                editedArrest.ArrestPoint = sender.position;
                editedArrest.ArrestPointDimension = sender.dimension;
                if (db_Arrests.UpdateArrest(editedArrest))
                {
                    API.sendNotificationToPlayer(sender, "Hapis pozisyonu, pozisyonunuz olarak güncellendi.");
                }
                else
                {
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Güncellenemedi.");
                }
            }
            else
            if ("name".StartsWith(type))
            {
                editedArrest.Name = _name;
                if (db_Arrests.UpdateArrest(editedArrest))
                {
                    API.sendNotificationToPlayer(sender, "Adı, " + _name + " olarak güncellendi.");
                }
                else
                {
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Güncellenemedi.");
                }
            }
        }

        [Command("tyre", "tyre [ispopped/pop/unpop] [tyreNumber] ")]
        public void Tyre(Client sender, string type, int _tyre)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 0)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            var _vehicle = db_Vehicles.FindNearestVehicle(sender.position);
            if (_vehicle == null) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Araç bulunamadı."); return; }
            if ("ispopped".StartsWith(type.ToLower()))
            {
                API.sendChatMessageToPlayer(sender, "~y~" + _vehicle.VehicleOnMap.isTyrePopped(_tyre).ToString());
                return;
            }
            else
            if ("pop".StartsWith(type.ToLower()))
            {
                API.popVehicleTyre(_vehicle.VehicleOnMap, _tyre, true);
            }
            else
            if ("unpop".StartsWith(type.ToLower()))
            {
                API.popVehicleTyre(_vehicle.VehicleOnMap, _tyre, false);
            }
        }
        [Command("reload", "/reload [Items/Craftings]")]
        public void Reload(Client sender, string type)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 0)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            if ("items".StartsWith(type.ToLower()))
            {
                db_Items.InitGameItems();
                API.sendChatMessageToPlayer(sender, "~y~" + db_Items.GameItems.Count + " eşya başarıyla yüklendi.");
            }
            if ("craftings".StartsWith(type.ToLower()))
            {
                db_Craftings.Init();
                API.sendChatMessageToPlayer(sender, "~y~" + db_Craftings.currentCraftingTables.Count + " craftingTable başarıyla yüklendi.");
            }
        }

        [Command("createoperatorshop", "/createoperatorshop [VodaCell/LosTelecom]")]
        public void CreateOperatorShop(Client sender, string type)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            var _type = "vodacell".StartsWith(type.ToLower()) ? Operator.Vodacell : Operator.LosTelecom;
            db_PhoneOperatorShop.Create(new PhoneOperatorShop
            {
                Dimension = sender.dimension,
                OperatorType = _type,
                Position = sender.position,
            });
        }
        [Command("editoperatorshop", "/editoperatorshop [ID] [operator/position] [Value]", GreedyArg = true)]
        public void EditOperatorShop(Client sender, int _Id, string type, string value)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            var editedModel = db_PhoneOperatorShop.GetById(_Id);
            if (editedModel != null)
            {
                if ("operator".StartsWith(type.ToLower()))
                {
                    editedModel.OperatorType = "vodacell".StartsWith(value.ToLower()) ? Operator.Vodacell : Operator.LosTelecom;
                }
                else
                if ("position".StartsWith(type.ToLower()))
                {
                    editedModel.Dimension = sender.dimension;
                    editedModel.Position = sender.position;
                }
                db_PhoneOperatorShop.Update(editedModel);

            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~İlgili nesne bulunamadı.");
            }
        }
        [Command("operatorshopid", "operatorshopid [range]")]
        public void OperatorShopId(Client sender, int range)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            foreach (var item in db_PhoneOperatorShop.CurrentOperatorShop.Item1)
            {
                if (Vector3.Distance(sender.position, item.Position) < range)
                {
                    API.sendChatMessageToPlayer(sender, "~s~" + item.OperatorType.ToString() + " ~y~(" + item.ID + ")");
                }
            }
        }
        [Command("createbank", "/createbank [bank/atm]")]
        public void CreateBank(Client sender, string type)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            db_Banks.Create(new Bank
            {
                Dimension = sender.dimension,
                Position = sender.position,
                Rotation = sender.rotation,
                TypeOfBank = "bank".StartsWith(type.ToLower()) ? BankType.Bank : BankType.ATM,
                MoneyCountInInside = "bank".StartsWith(type.ToLower()) ? 5000 : 1500,
            });
        }
        [Command("bankid", "/bankid [range]")]
        public void BankId(Client sender, int range)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            foreach (var item in db_Banks.CurrentBanks.Item1)
            {
                if (Vector3.Distance(sender.position, item.Position) < range)
                {
                    API.sendChatMessageToPlayer(sender, "~y~" + item.TypeOfBank.ToString() + " ~s~~h~(" + item.BankId + ")" + " Money Stock: " + item.MoneyCountInInside);
                }
            }
        }
        [Command("editbank", "/editbank [bankid] ~y~[position/type/moneyinside] ~s~[me/(bank/atm)/value]")]
        public void EditBank(Client sender, int bankId, string type, string value)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            var editedBank = db_Banks.GetById(bankId);
            if ("position".StartsWith(type.ToLower()))
            {
                editedBank.Dimension = sender.dimension;
                editedBank.Position = sender.position;
            }
            else
            if ("type".StartsWith(type.ToLower()))
            {
                if ("bank".StartsWith(value.ToLower()))
                {
                    editedBank.TypeOfBank = BankType.Bank;
                }
                else
                if ("atm".StartsWith(value.ToLower()))
                {
                    editedBank.TypeOfBank = BankType.ATM;
                }
            }
            else
            if ("moneyinside".StartsWith(type.ToLower()))
            {
                try { Convert.ToInt32(value); } catch (Exception) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Parametre sayı olmalıydı."); return; }
                editedBank.MoneyCountInInside = Convert.ToInt32(value);
            }


            if (db_Banks.Update(editedBank))
                API.sendChatMessageToPlayer(sender, "~y~Başarıyla güncellendi.");
            else
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Güncellenemedi.");
        }

        [Command("createtirdelivery", "/ctd [Price] [Name]", Alias = "ctd", GreedyArg = true)]
        public void CreateTirDeliveryPoint(Client sender, int price, string name)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            db_TirJob.CreateDeliveryPoint(new Models.TirDeliveryPoint { DeliveryPointDimension = sender.dimension, DeliveryPointMoney = price, DeliveryPointPosition = sender.position, Name = name });
            API.sendChatMessageToPlayer(sender, "~y~Başarıyla eklendi.");
        }

        [Command("tirdeliveryid", "/tdi [range]", Alias = "tdi")]
        public void TirDeliveryId(Client sender, int range)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            foreach (var item in db_TirJob.CurrentDeliveryPoints.Item1)
            {
                if (sender.dimension == item.DeliveryPointDimension && Vector3.Distance(sender.position, item.DeliveryPointPosition) < range)
                {
                    API.sendChatMessageToPlayer(sender, "~y~(" + item.ID + ") ~s~" + item.Name);
                }
            }
        }
        [Command("edittirdelivery", "/etd [deliveryid] [name/pos/money]")]
        public void EditTirDeliveryPoint(Client sender, int ID, string type, string commandParam)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            try { Convert.ToInt32(ID); } catch (Exception) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~ID sayı olmalıydı."); return; }
            var editedDelivery = db_TirJob.GetDeliveryPoint(Convert.ToInt32(ID));
            if ("name".StartsWith(type.ToLower()))
            {
                editedDelivery.Name = commandParam;
            }
            else
            if ("position".StartsWith(type.ToLower()))
            {
                editedDelivery.DeliveryPointPosition = sender.position;
                editedDelivery.DeliveryPointDimension = sender.dimension;
            }
            else
            if ("money".StartsWith(type.ToLower()))
            {
                try { Convert.ToInt32(commandParam); } catch (Exception) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~ID sayı olmalıydı."); return; }
                editedDelivery.DeliveryPointMoney = Convert.ToInt32(commandParam);
            }

            if (db_TirJob.UpdateDeliveryPoint(editedDelivery))
            {
                API.sendChatMessageToPlayer(sender, "~y~Delivery Point Güncellendi.");
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Delivery Point güncellenirken bir hata oluştu. Konsol çıktısına bakın.");
            }
        }

        [Command("showinventory", "/showinventory [PlayerId]")]
        public void ShowInventory(Client sender, int identity)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            foreach (var itemPlayer in API.getAllPlayers())
            {
                if (identity == API.getEntityData(itemPlayer, "ID"))
                {
                    var _inventory = (Inventory)API.getEntityData(itemPlayer, "inventory");

                    List<string> names = new List<string>(); List<string> descriptions = new List<string>();
                    foreach (var item in _inventory.ItemList)
                    {
                        var _gameItem = db_Items.GameItems[item.ItemId];
                        names.Add((item.Equipped ? "*" : String.Empty) + _gameItem.Name + " (" + item.Count + ")");
                        descriptions.Add(_gameItem.Description);
                    }
                    API.triggerClientEvent(sender, "inventory_search",
                        names.Count,
                        names.ToArray(),
                        descriptions.ToArray(),
                        (API.getEntityData(itemPlayer, "CharacterName") + " eşyaları | " + _inventory.ItemList.Count + "/" + _inventory.InventoryMaxCapacity),
                        itemPlayer.socialClubName
                        );

                    API.shared.sendChatMessageToPlayer(sender, $"~y~{itemPlayer.nametag} | Para: ~s~{InventoryManager.GetPlayerMoney(itemPlayer)}$ ~y~- Banka: ~s~{InventoryManager.GetPlayerBankMoney(itemPlayer)}$");
                    return;
                }
            }
        }
        [Command("createkamyondelivery", "/ckd [Name] [Money/Parts] [Price/Count]", Alias = "ckd")]
        public void CreateKamyonDeliveryPoint(Client sender, string name, string type, int value)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            db_KamyonJob.CreateDeliveryPoint(new KamyonDeliveryPoint
            {
                CompletedValue = value,
                DeliveryDimension = sender.dimension,
                DeliveryPoint = sender.position,
                Name = name,
                Type = ("money".StartsWith(type.ToLower()) ? DeliveryType.Money : DeliveryType.MetalParts),
            });
            API.sendChatMessageToPlayer(sender, "~g~TAMAMLANDI: ~s~Delivery point başarıyla eklendi.");
        }
        [Command("editkamyondelivery", "/ekd [id] ~y~[pos/name/type~s~(money/parts)~y~] [value~s~(price/count)~y~]", Alias = "ekd")]
        public void EditKamyonDeliveryPoint(Client sender, int _Id, string type, string value)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            var editedModel = db_KamyonJob.GetDeliveryPoint(Convert.ToInt32(_Id));
            if (editedModel != null)
            {
                if ("position".StartsWith(type.ToLower()))
                {
                    editedModel.DeliveryPoint = sender.position;
                    editedModel.DeliveryDimension = sender.dimension;
                }
                else
                if ("name".StartsWith(type.ToLower()))
                {
                    editedModel.Name = value;
                }
                else
                if ("type".StartsWith(type.ToLower()))
                {
                    editedModel.Type = ("money".StartsWith(type.ToLower()) ? DeliveryType.Money : DeliveryType.MetalParts);
                }
                db_KamyonJob.UpdateDeliveryPoint(editedModel);
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Delivery Point Bulunamadı!");
            }
        }
        [Command("kamyondeliveryid", "/kdid [range]", Alias = "kdid")]
        public void KamyonDeliveryPointID(Client sender, int range)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            foreach (var item in db_KamyonJob.CurrentDeliveryPoints.Item1)
            {
                if (Vector3.Distance(sender.position, item.DeliveryPoint) > range)
                {
                    API.sendChatMessageToPlayer(sender, "~y~(" + item.ID + ")~s~ " + item.Name);
                }
            }
        }
        [Command("creategasstation", "/creategasstation [PricePerUnit] [JobDeliverMoney]")]
        public void CreateGasStation(Client sender, float pricePerUnit, int money)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            db_GasStations.Create(new GasStation { CompletedMoney = money, Dimension = sender.dimension, GasInStock = 10000, MaxGasInStock = 10000, Position = sender.position, PricePerUnit = pricePerUnit });
            API.sendChatMessageToPlayer(sender, "~g~EKLENDİ: ~s~Benzinlik başarıyla eklendi.");
        }
        [Command("gasstationid", "/gsid [range]", Alias = "gsid")]
        public void GasStationId(Client sender, int range)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            foreach (var itemGas in db_GasStations.CurrentGasStations.Item1)
            {
                if (Vector3.Distance(sender.position, itemGas.Position) < range)
                {
                    API.sendChatMessageToPlayer(sender, "~y~(" + itemGas.GasStationId + ") ~s~- Benzin: " + itemGas.GasInStock + "/" + itemGas.MaxGasInStock);
                }
            }
        }
        [Command("editgasstation", "/editgasstation [id] ~y~[gas/maxgas/unitprice/jobprice/position]~s~ [Value]")]
        public void EditGasStation(Client sender, int _Id, string type, string value)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            try
            {
                var editedModel = db_GasStations.GetById(_Id);
                if (editedModel != null)
                {
                    if ("gas".StartsWith(type.ToLower()))
                    {
                        editedModel.GasInStock = Convert.ToSingle(value);
                    }
                    else
                     if ("maxgas".StartsWith(type.ToLower()))
                    {
                        editedModel.MaxGasInStock = Convert.ToSingle(value);
                    }
                    else
                     if ("unitprice".StartsWith(type.ToLower()))
                    {
                        editedModel.PricePerUnit = Convert.ToSingle(value);
                    }
                    else
                     if ("position".StartsWith(type.ToLower()))
                    {
                        editedModel.Position = sender.position;
                    }

                    if (db_GasStations.Update(editedModel))
                    {
                        API.sendChatMessageToPlayer(sender, "~g~DÜZENLENDİ: ~s~Benzinlik başarıyla düzenlendi.");
                    }
                    else
                    {
                        API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bir hata oluştu. Düzenleme başarısız.");
                    }
                }
                else
                {
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Benzinlik bulunamadı.");
                }
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(FormatException))
                {
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Parametre sayı olarak girilmeliydi.");
                }
            }
        }

        [Command("createbusiness", "/createbusiness [Price] [Name]", AddToHelpmanager = true, GreedyArg = true)]
        public void CreateBusiness(Client sender, int _price, string _name)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            db_Businesses.Create(new Models.Business
            {
                BusinessName = _name,
                Price = _price,
                Dimension = sender.dimension,
                Position = sender.position,
                MaxVaultMoney = 15000,
                IsSelling = true,
                VaultMoney = 5000,
                //MoneyIncomePerHour = 100,
                OwnerSocialClubName = null,
                IsClosed = false,
            });
            API.sendChatMessageToPlayer(sender, "~g~OLUŞTURULDU: ~s~Başarıyla oluşturuldu.");
        }

        [Command("businessid", "/bid [range]")]
        public void BusinessId(Client sender, int range)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            foreach (var itemBusiness in db_Businesses.currentBusiness.Values)
            {
                if (Vector3.Distance(sender.position, itemBusiness.Position) < range)
                {
                    API.sendChatMessageToPlayer(sender, "~y~(" + itemBusiness.BusinessId + ") ~s~" + itemBusiness.BusinessName);
                }
            }
        }
        [Command("editbusiness", "/editbusiness [id] ~y~[pos/price/closed/name/moneyPerHour/vault/maxvault] ~s~[value]")]
        public void EditBusiness(Client sender, int _Id, string type, string value)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            var editedModel = db_Businesses.GetById(_Id);
            if ("pos".StartsWith(type.ToLower()))
            {
                editedModel.Position = sender.position;
                editedModel.Dimension = sender.dimension;
            }
            else
            if ("price".StartsWith(type.ToLower()))
            {
                try { Convert.ToInt32(value); } catch (Exception ex) { if (ex.GetType() == typeof(FormatException)) API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Parametre sayı olmalıydı."); return; }
                editedModel.Price = Convert.ToInt32(value);
            }
            else
            if ("closed".StartsWith(type.ToLower()))
            {
                editedModel.IsClosed = (value == "0" || value.ToLower() == "false" ? false : true);
            }
            else
            if ("moneyperhour".StartsWith(type.ToLower()))
            {
                try { Convert.ToInt32(value); } catch (Exception ex) { if (ex.GetType() == typeof(FormatException)) API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Parametre sayı olmalıydı."); return; }
                //editedModel.MoneyIncomePerHour = Convert.ToInt32(value);
            }
            else
            if ("vault".StartsWith(type.ToLower()))
            {
                try { Convert.ToInt32(value); } catch (Exception ex) { if (ex.GetType() == typeof(FormatException)) API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Parametre sayı olmalıydı."); return; }
                editedModel.VaultMoney = Convert.ToInt32(value);
            }
            else
              if ("maxvault".StartsWith(type.ToLower()))
            {
                try { Convert.ToInt32(value); } catch (Exception ex) { if (ex.GetType() == typeof(FormatException)) API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Parametre sayı olmalıydı."); return; }
                editedModel.MaxVaultMoney = Convert.ToInt32(value);
            }
            db_Businesses.Update(editedModel);
        }

        /// [Command("connect2business", "/c2b [gas/shop] [ID] [BusinessId]", Alias = "c2b")]
        /// public void ConnectToBusiness(Client sender, string type, int _Id, int _BusinessId)
        /// {
        ///     if (!(API.getEntityData(sender, "AdminLevel") >= 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
        ///     var _IndexBusiness = db_Businesses.FindIndexById(_BusinessId);
        ///     if (_IndexBusiness >= 0)
        ///     {
        ///         if ("gas".StartsWith(type.ToLower()))
        ///         {
        ///             var _IndexGas = db_GasStations.FindIndexById(_Id);
        ///             if (_IndexGas >= 0)
        ///             {
        ///                 try
        ///                 {
        ///                     db_GasStations.CurrentGasStations.Item1[_IndexGas].BusinessId = _BusinessId;
        ///                     db_GasStations.Update(db_GasStations.CurrentGasStations.Item1[_IndexGas]);
        ///                     API.sendChatMessageToPlayer(sender, "~g~BAŞARILI: ~s~" + _Id + " ID'li benzinlik " + db_Businesses.currentBusiness.Item1[_IndexBusiness].BusinessName + " adlı işyerine bağlandı.");
        ///                     return;
        ///                 }
        ///                 catch (Exception ex)
        ///                 {
        ///                     if (ex.GetType() == typeof(IndexOutOfRangeException))
        ///                     {
        ///                         API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~ Index hatası");
        ///                     }
        ///                     API.consoleOutput("HATA | ConnectToBusiness : " + ex.ToString());
        ///                 }
        ///             }
        ///             else
        ///             {
        ///                 API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Benzin İstasyonu bulunamadı.");
        ///             }
        ///         }
        ///         else
        ///         if ("shop".StartsWith(type.ToLower()))
        ///         {
        ///             var _IndexShop = db_Shops.FindShopIndexById(_Id);
        ///             if (_IndexShop >= 0)
        ///             {
        ///                 try
        ///                 {
        ///                     db_Shops.CurrentShopsList.Keys.FirstOrDefault(x => x.ShopId == _Id).BusinessId = _BusinessId;
        ///                     API.sendChatMessageToPlayer(sender, "~g~BAŞARILI: ~s~" + _Id + " ID'li shop " + db_Businesses.currentBusiness.Item1[_IndexBusiness].BusinessName + " adlı işyerine bağlandı.");
        ///                     return;
        ///                 }
        ///                 catch (Exception ex)
        ///                 {
        ///                     if (ex.GetType() == typeof(IndexOutOfRangeException))
        ///                     {
        ///                         API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~ Index hatası");
        ///                     }
        ///                     API.consoleOutput("HATA | ConnectToBusiness : " + ex.ToString());
        ///                 }
        ///
        ///             }
        ///             else
        ///             {
        ///                 API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Shop bulunamadı.");
        ///             }
        ///         }
        ///     }
        ///     else
        ///     {
        ///         API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~İşyeri bulunamadı.");
        ///     }
        /// }
        [Command("createlicensepoint", "/createlicensepoint [id(type)] [pedname] [text] ")]
        public void CreateLicensePoint(Client sender, int type, string pedname, string text)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 3)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            var pedHash = API.pedNameToModel(pedname);
            if (pedHash.ToString() == "0") { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Geçerli bir ped giriniz."); return; }
            db_LicensePoints.Create(new LicenseTaking
            {
                Dimension = sender.dimension,
                Position = sender.position,
                LicenseType = type,
                Ped = pedHash,
                Price = 1000,
                Rotation = sender.rotation,
                Text = text
            });
        }

        [Command("editlicensepoint", "/editlicensepoint [ID] [pos/text/price/ped] [value]", GreedyArg = true)]
        public void EditLicensePoint(Client sender, int _Id, string type, string commandParam)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            var _editedModel = db_LicensePoints.GetById(_Id);
            if (_editedModel != null)
            {
                if ("pos".StartsWith(type.ToLower()))
                {
                    _editedModel.Position = sender.position;
                    _editedModel.Dimension = sender.dimension;
                }
                else
                if ("text".StartsWith(type.ToLower()))
                {
                    _editedModel.Text = commandParam;
                }
                else
                if ("price".StartsWith(type.ToLower()))
                {
                    try
                    {
                        _editedModel.Price = Convert.ToInt32(commandParam);
                    }
                    catch (Exception ex)
                    {
                        if (ex.GetType() == typeof(FormatException))
                        {
                            API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Geçerli bir ücret girin.");
                        }
                    }
                }
                else
                if ("ped".StartsWith(type.ToLower()))
                {
                    var pedHash = API.pedNameToModel(commandParam);
                    if (pedHash.ToString() == "0")
                    {
                        API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Yazdığınız Pedastrian bulunamadı!"); return;
                    }
                    _editedModel.Ped = pedHash;
                }

                db_LicensePoints.Update(_editedModel);


            }
        }
        [Command("refill", "/refill ~y~[fuels/gasstations]")]
        public void Refill(Client sender, string type)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            if ("fuels".StartsWith(type.ToLower()))
            {
                foreach (var itemVeh in db_Vehicles.GetAll())
                {
                    itemVeh.Fuel = 100;
                }
                API.sendChatMessageToPlayer(sender, "~y~Araçların benzinleri dolduruldu.");
            }
            else
            if ("gasstations".StartsWith(type.ToLower()))
            {
                foreach (var item in db_GasStations.CurrentGasStations.Item1)
                {
                    item.GasInStock = item.MaxGasInStock;
                    db_GasStations.Update(item);
                }
                API.sendChatMessageToPlayer(sender, "~y~Benzinliklerin stokları dolduruldu.");
            }
        }
        [Command("createbuilding", "/createbuilding [Name]", GreedyArg = true)]
        public void CreateBuilding(Client sender, string name)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            db_Buildings.CreateBuilding(new Building
            {
                BuildingName = name,
                Dimension = sender.dimension,
                Position = sender.position,
                Floors = new List<Floor>()
            });
        }

        [Command("buildingid", "/bid", Alias = "bid")]
        public void BuildingID(Client sender)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            foreach (var item in db_Buildings.currentBuildings)
            {
                if (Vector3.Distance(sender.position, item.Value.Position) < 6)
                {
                    API.shared.sendChatMessageToPlayer(sender, $"~y~({item.Value.BuildingId})~s~ - {item.Value.BuildingName}");
                }
            }
        }
        [Command("addfloor", "/addfloor [BuildingID] [House/Business/Warehouse] [ID]")]
        public void AddFloor(Client sender, int bId, string type, int oId)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            bool result = false;
            if ("house".StartsWith(type.ToLower()))
            {
                result = db_Buildings.AddFloorToBuilding(bId, FloorType.House, oId);
            }
            else
            if ("business".StartsWith(type.ToLower()))
            {
                result = db_Buildings.AddFloorToBuilding(bId, FloorType.Business, oId);
            }
            //else
            //if ("warehouse".StartsWith(type.ToLower()))
            //{
            //    result = db_Buildings.AddFloorToBuilding(bId,FloorType.Warehouse,oId);
            //}

            if (result)
            {
                API.sendChatMessageToPlayer(sender, "~g~BAŞARILI: ~s~Binaya başarıyla eklendi.");
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA:: ~s~Bina veya ev bulunamadı.");
            }
        }
        [Command("editbuilding", "/editbuilding [name/pos] [value]")]
        public void EditBuilding(Client sender, int bId, string type, string value)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            var _building = db_Buildings.GetBuilding(bId);
            if (_building == null) { API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bina bulunamadı."); return; }
            if ("name".StartsWith(type.ToLower()))
            {
                _building.BuildingName = value;
            }
            if ("position".StartsWith(type.ToLower()))
            {
                _building.Position = sender.position;
                _building.Dimension = sender.dimension;
            }
            db_Buildings.UpdateBuilding(_building);
        }
        [Command("alignhouses")]
        public void AlignHouses(Client sender)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            foreach (var itemBuilding in db_Buildings.GetAll().Buildings)
            {
                foreach (var itemHouse in itemBuilding.Floors)
                {
                    itemHouse.EntrancePosition = itemBuilding.Position;
                    itemHouse.EntranceDimension = itemBuilding.Dimension;
                }
            }
        }
        [Command("lock")]
        public void LockSaleHouses(Client sender, bool locked)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            foreach (var item in db_Houses.CurrentHousesDict)
            {
                if (item.Value.IsSelling && String.IsNullOrEmpty(item.Value.OwnerSocialClubName))
                {
                    item.Value.IsLocked = locked;
                }
            }
            API.shared.sendChatMessageToPlayer(sender, "~y~Tüm satılık evler" + (locked ? " kilitlendi." : "in kilidi açıldı."));
            db_Houses.SaveChanges();
        }
        [Command("cleardroppeditems", "/cleardroppeditems [type/all(*)/range]", GreedyArg = true)]
        public void ClearDroppedItems(Client sender, string commandParam)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            Models.ItemType _type;
            if (Enum.TryParse<ItemType>(commandParam, out _type))
            {
                for (int i = db_Items.currentDroppedItems.Items.Count - 1; i >= 0; i--)
                {
                    if (db_Items.GetItemById(db_Items.currentDroppedItems.Items[i].Item.ItemId).Type == _type)
                    {
                        try
                        {
                            API.deleteEntity(db_Items.currentDroppedItems.Items[i].ObjectInGame);
                            API.deleteEntity(db_Items.currentDroppedItems.Items[i].LabelInGame);
                            db_Items.currentDroppedItems.Items.Remove(db_Items.currentDroppedItems.Items[i]);
                        }
                        catch (Exception ex)
                        {
                            API.consoleOutput(LogCat.Fatal, ex.ToString());
                        }
                    }
                }
            }
            else
            {
                if (commandParam.ToLower().StartsWith("range"))
                {
                    try
                    {
                        for (int i = db_Items.currentDroppedItems.Items.Count - 1; i >= 0; i--)
                        {
                            if (Vector3.Distance(sender.position, db_Items.currentDroppedItems.Items[i].Position) <= Convert.ToInt32(commandParam.Split(' ')[1]))
                            {
                                db_Items.RemoveDroppedItem(db_Items.currentDroppedItems.Items[i]);
                            }
                        }
                    }
                    catch (Exception ex)
                    {
                        if (ex.GetType() == typeof(IndexOutOfRangeException) || ex.GetType() == typeof(ArgumentOutOfRangeException))
                        {
                            API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Mesafe sayı olmalıydı.");
                        }
                        else
                            API.shared.consoleOutput(LogCat.Warn, ex.ToString());
                    }
                }
                else
                {
                    for (int i = db_Items.currentDroppedItems.Items.Count - 1; i >= 0; i--)
                    {
                        try
                        {
                            API.deleteEntity(db_Items.currentDroppedItems.Items[i].ObjectInGame);
                            API.deleteEntity(db_Items.currentDroppedItems.Items[i].LabelInGame);
                            db_Items.currentDroppedItems.Items.Remove(db_Items.currentDroppedItems.Items[i]);
                        }
                        catch (Exception ex)
                        {
                            if (ex.GetType() == typeof(NullReferenceException))
                            {
                                API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Item bulunamadı.");
                            }
                            API.consoleOutput(LogCat.Fatal, ex.ToString());
                        }
                    }
                }
            }
        }

        [Command("addvehlicpoint", "/addvehlicpoint")]
        public void AddVehicleLicenseCheckpoint(Client sender)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            db_LicensePoints.CreateVehLicenseChk(new VehLicenseCheckpoint
            {
                Dimension = sender.dimension,
                Position = sender.position + new Vector3(0, 0, -1),
            });
            API.sendNotificationToPlayer(sender, "~y~Eklendi.", true);
        }
        [Command("editvehlic", "/editvehlicpoint [id] [pos]")]
        public void EditVehicleLicenseCheckpoint(Client sender, int _Id, string type)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }

            var editedModel = db_LicensePoints.GetLicenseCheckPoint(_Id);
            if (editedModel == null) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Vehicle License Checkpoint bulunamadı."); return; }
            if ("pos".StartsWith(type.ToLower()))
            {
                editedModel.Position = sender.position;
                editedModel.Dimension = sender.dimension;
            }
            if (db_LicensePoints.Update(editedModel))
            {
                API.sendChatMessageToPlayer(sender, "~y~Pozisyonu başarıyla güncellendi.");
            }
            else
            {
                API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Güncellenirken bir hata oluştu.");
            }
        }
        [Command("resettax")]
        public void ResetTaxes(Client sender)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            foreach (var itemVeh in db_Vehicles.GetAll())
            {
                itemVeh.Tax = 0;
                itemVeh.IsBlockedForTax = false;
            }
            db_Vehicles.SaveChanges();
        }
        [Command("blackout")]
        public void BlackoutCommand(Client sender, bool blackout)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            API.sendNativeToAllPlayers(0x1268615ACE24D504, blackout);
        }

        [Command("ranks", "/ranks ~y~[FactionId]")]
        public void Ranks(Client sender, int _factionID)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            var _facRank = db_FactionRanks.GetFactionRanks(_factionID);
            string strRanks = "";
            //API.shared.consoleOutput("Faction Rank Length : " + _facRank.Ranks.Count);
            foreach (var item in _facRank.Ranks)
            {
                strRanks += item.RankLevel + " - ~h~" + item.RankName + " \n";
            }
            API.shared.sendChatMessageToPlayer(sender, strRanks);
        }
        [Command("addrank", "/addrank [FactionID] [Name]", GreedyArg = true)]
        public void AddRrankToFaction(Client sender, int facId, string name)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            if (db_FactionRanks.AddRankToFaction(facId, new Rank { RankName = name }))
            {
                API.sendChatMessageToPlayer(sender, "~y~" + name + " adlı yetki eklendi.");
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bir hata oluştu. Enis ile iletişime geçin.");
            }
        }
        [Command("removerank", "/removerank [FactionID] [RankLevel]")]
        public void RemoveRank(Client sender, int facId, int rankLevel)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            if (db_FactionRanks.RemoveFactionRank(facId, rankLevel))
            {
                API.sendChatMessageToPlayer(sender, "~y~Rütbe başarıyla kaldırıldı.");
            }
            else
            {
                API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Rütbe kaldırılamadı. ");
            }
        }
        [Command("addcrime", "/addcrime [wantedLevel] [text]", GreedyArg = true)]
        public void AddCrimeType(Client sender, int _WantedLevel, string _text)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            db_Crimes.AddNewCrimeType(new Models.CrimeType { Name = _text, WantedLevel = _WantedLevel > 5 ? 5 : _WantedLevel });
            API.sendChatMessageToPlayer(sender, "~y~Suç LSPD bilgisayarına eklendi.");
        }

        [Command("removecrime", "/removecrime")]
        public void RemoveCrime(Client sender)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            List<string> names = new List<string>();
            List<string> descs = new List<string>();
            foreach (var item in db_Crimes.GetCrimeTypes().Items)
            {
                names.Add(item.Name);
                descs.Add("Suç seviyesi: " + item.WantedLevel);
            }

            Clients.ClientManager.SendCrimeListForRemove(sender, names, descs);

        }
        public static void CompleteRemoveCrimeType(int index)
        {
            var _list = db_Crimes.GetCrimeTypes();
            db_Crimes.RemoveCrimeType(_list.Items[index].CrimeTypeId);

        }

        [Command("createfactionvault", "/createfactionvault [FactionID]")]
        public void CreateFactionVault(Client sender, int facId)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            db_FactionVaults.CreateFactionVault(new FactionVault
            {
                Dimension = sender.dimension,
                Position = sender.position,
                Text = FactionManager.ToFactionName(facId) + " ((/kasa))",
                FactionId = facId,
                VaultItems = new List<VaultItem>()
            });
        }
        [Command("editfactionvault", "/editfactionvault [pos/faction/text] [FactionID] [value]", GreedyArg = true)]
        public void EditFactionVault(Client sender, string type, int facId, string value)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            var edited = db_FactionVaults.GetFactionVault(facId);
            if (edited == null) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Oluşum kasası bulunamadı."); return; }
            if ("position".StartsWith(type.ToLower()))
            {
                edited.Position = sender.position;
                edited.Dimension = sender.dimension;
            }
            else
            if ("faction".StartsWith(type.ToLower()))
            {
                edited.FactionId = Convert.ToInt32(value);
            }
            else
            if ("text".StartsWith(type.ToLower()))
            {
                edited.Text = value;
            }


            db_FactionVaults.UpdateFactionVault(edited);
        }

        [Command("factionvaultid", "/factionvaultid [range]", Alias = "fvid")]
        public void FactionVaultId(Client sender, int range)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            foreach (var item in db_FactionVaults.currentVaults.Items)
            {
                if (Vector3.Distance(sender.position, item.TextLabelOnMap.position) <= range)
                {
                    API.sendChatMessageToPlayer(sender, "~y~(" + item.VaultId + ") ~s~Faction: " + item.FactionId + " | " + item.Text);
                }
            }
        }

        [Command("additemfactionvault", "/additemfactionvault [VaultID] [ItemID] [MinRank]")]
        public void AddItemToFactionVault(Client sender, int vaultId, int itemId, int minRank)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            var _item = db_Items.GetItemById(itemId);
            if (_item == null) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Item bulunamadı."); return; }
            if (db_FactionVaults.AddItemToFactionVault(vaultId, _item, minRank))
            {
                API.sendChatMessageToPlayer(sender, "~y~" + _item.Name + " adlı öge başarıyla eklendi.");
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Oluşum kasası bulunamadı.");
            }
        }

        [Command("removeitemfactionvault", "/removeitemfactionvault")]
        public void RemoveItemFromFactionVault(Client sender, int _vaultId, int itemId)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            var _vault = db_FactionVaults.GetFactionVault(_vaultId);
            if (_vault == null) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Oluşum kasası bulunamadı."); return; }
            if (db_FactionVaults.RemoveItemFromFactionVault(_vault, itemId))
            {
                API.sendChatMessageToPlayer(sender, "~y~Öge başarıyla kaldırıldı.");
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Item oluşum kasasında bulunamadı.");
            }
        }

        [Command("cevapla", "/cevapla [SoruID] [CevapMesajı]", GreedyArg = true)]
        public void AnswerQuestion(Client sender, int qId, string text)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            var question = db_Reports.GetReport(qId);
            var player = db_Accounts.IsPlayerOnline(question.OwnerSocialClubID);
            if (player != null)
            {
                API.sendChatMessageToPlayer(player, "~b~[?]~s~~h~ Sorunuzun Cevabı: ~h~" + text);
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~b~[?]~s~~h~ Sorunun sahibi online olmadığından yanıtı göremedi.");
            }
            API.sendChatMessageToPlayer(sender, "~b~[?]~s~~h~ Soruyu yanıtladınız.");
            SendMessage($"~b~[?]~s~ {sender.nametag} adlı kişi {question.ReportID} numaralı soruyu yanıtladı.");
            db_Reports.RemoveReport(question.ReportID);

        }

        [Command("accept", "/cevapla [ReportId]")]
        public void AcceptReport(Client sender, int qId)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            var question = db_Reports.GetReport(qId);
            var player = db_Accounts.IsPlayerOnline(question.OwnerSocialClubID);
            if (player != null)
            {
                API.sendChatMessageToPlayer(sender, "~r~[!]~s~~h~ - Raporun sahibi şu anda online: " + player.nametag);
                API.sendChatMessageToPlayer(player, "~r~[!]~s~~h~ - Raporunuz Onaylandı. İnceleniyor.");
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~#f1fac1~[?] Şu anda raporun sahibi online değil.");
            }
            db_Reports.RemoveReport(question.ReportID);
        }

        [Command("reject", "/cevapla [ReportId]")]
        public void RejectReport(Client sender, int qId)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            var question = db_Reports.GetReport(qId);
            var player = db_Accounts.IsPlayerOnline(question.OwnerSocialClubID);
            if (player != null)
            {
                API.sendChatMessageToPlayer(sender, "~r~[!]~s~~h~ - Raporun sahibi şu anda online: " + player.nametag);
                API.sendChatMessageToPlayer(player, "~r~[!]~s~~h~ - Raporunuz Reddedildi. Uygun bir rapor gönderebilirsiniz.");
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~[!]~s~~h~ Şu anda raporun sahibi online değil.");
            }
            db_Reports.RemoveReport(question.ReportID);

        }

        [Command("reports")]
        public void OnReports(Client sender)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            var reports = db_Reports.GetReports(ReportType.Report);

            foreach (var itemReport in reports)
            {
                API.sendChatMessageToPlayer(sender, "~r~[!] ~s~~h~- " + itemReport.ReportID + " - " + itemReport.ReportText + " ((/accept " + itemReport.ReportID + " ))" + " ((/reject " + itemReport.ReportID + " ))");
            }
        }

        [Command("sorular")]
        public void OnQuestions(Client sender)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            var reports = db_Reports.GetReports(ReportType.Question);

            foreach (var itemQuestion in reports)
            {
                API.sendChatMessageToPlayer(sender, "~b~[?] - ~s~~h~" + itemQuestion.ReportID + " - " + itemQuestion.ReportText + " ((/cevapla " + itemQuestion.ReportID + "))");
            }
        }

        [Command("accessory")]
        public void Accessory(Client sender, int slot, int drawable, int texture)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            API.setPlayerAccessory(sender, slot, drawable, texture);
        }
        [Command("sethospitalpoint", "/sethospitalpoint [RespawnMoney]")]
        public void SetHospitalPoint(Client sender, int money)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            db_Hospital.UpdateRespawnPoint(sender.position, sender.dimension);
            db_Hospital.UpdateRespawnPrice(money);
        }
        [Command("sethospitaldeliverpoint", "/sethospitalpoint [RespawnMoney]")]
        public void SetHospitalDeliverPoint(Client sender)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            db_Hospital.UpdateDeliverPoint(sender.position, sender.dimension);
        }
        [Command("find", "/find [player/vehicle] [ID]")]
        public void Find(Client sender, string type, int id)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            if ("player".StartsWith(type.ToLower()))
            {
                var player = db_Accounts.FindPlayerById(id);
                if (player != null)
                {
                    Clients.ClientManager.UpdateWaypoint(sender, player.position);
                }
                else
                {
                    API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Oyuncu bulunamadı.");
                }
                return;
            }
            if ("vehicle".StartsWith(type.ToLower()))
            {
                var _vehicle = db_Vehicles.GetVehicle(id);
                if (_vehicle != null)
                {
                    Clients.ClientManager.UpdateWaypoint(sender, _vehicle.VehicleOnMap.position);
                }
                else
                {
                    API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Araç bulunamadı.");
                }
                return;
            }
        }
        [Command("createcraftingtable", "/cctable [TypeID]", Alias = "cctable")]
        public void CreateCraftingTable(Client sender, int TypeId)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            var _table = db_Craftings.GetCraftingTableModel(TypeId);
            if (_table != null)
            {
                db_Craftings.CreateCraftingTableOnMap(new CraftingTablesOnMap
                {
                    Dimension = sender.dimension,
                    Position = sender.position,
                    Rotation = sender.rotation,
                    CraftingTableModelId = _table.CraftingTableId,
                    Name = _table.Name
                });
            }
            else
            {
                API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Crafting Table Türü bulunamadı.");
            }

        }
        [Command("ctableid", "ctableid [range]")]
        public void CraftingTableId(Client sender, int range)
        {
            foreach (var item in db_Craftings.craftingTablesOnMap.Values)
            {
                if (Vector3.Distance(item.Position, sender.position) <= range)
                {
                    API.shared.sendChatMessageToPlayer(sender, $"~y~({item.TableOnMapId}) - {item.Name}");
                }
            }
        }
        [Command("editctable", "/editctable [CTableID] ~y~[position/Typeid]~s~ [Parameter]")]
        public void EditCraftingTable(Client sender, int id, string type, string value)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            var _table = db_Craftings.GetCraftingTableOnMap(id);
            if (_table == null) { API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~CTable Bulunamadı."); return; }
            if ("position".StartsWith(type.ToLower()))
            {
                _table.Position = sender.position;
                _table.Rotation = sender.rotation;
                _table.Dimension = sender.dimension;
            }
            if ("typeid".StartsWith(type.ToLower()))
            {
                try
                {
                    _table.CraftingTableModelId = Convert.ToInt32(value);
                }
                catch (Exception ex)
                {
                    if (ex.GetType() == typeof(FormatException))
                    {
                        API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Hatalı parametre girildi.");
                    }
                }
            }
            db_Craftings.UpdateCraftingTableOnMap(_table);
        }

        [Command("clearplayerstar", "/clearplayerstar [ID]")]
        public void ClearPlayerStar(Client sender, int targetPlayerId)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            var player = db_Accounts.FindPlayerById(targetPlayerId);
            if (player != null)
            {
                player.wantedLevel = 0;
            }
            else
            {
                API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Oyuncu bulunamadı.");
            }
        }
        [Command("createfactioninteractive", "/cfi [Faction] [Name]", Alias = "cfi", GreedyArg = true)]
        public void CreateFactionInteractive(Client sender, int factionId, string name)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            db_FactionInteractives.Create(new FactionInteractive
            {
                Dimension = sender.dimension,
                Faction = factionId,
                Position = sender.position,
                Name = name,
            });
            API.shared.sendNotificationToPlayer(sender, "Eklendi.");
        }
        [Command("factioninteractiveid", "/fid", Alias = "fid")]
        public void FactionInteractiveId(Client sender)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            foreach (var item in db_FactionInteractives.currentFactionInteractives)
            {
                if (Vector3.Distance(item.Value.Position, sender.position) < 8)
                {
                    API.shared.sendChatMessageToPlayer(sender, $"~y~({item.Key}) ~s~ - {item.Value.Name}");
                }
            }
        }
        [Command("editfactioninteractive", "/editfi [ID] [pos/name/faction] [value]", Alias = "editfi", GreedyArg = true)]
        public void EditFactionInteractive(Client sender, int Id, string type, string value)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            var edited = db_FactionInteractives.GetById(Id);
            if (edited != null)
            {
                switch (type.ToLower())
                {
                    case "pos":
                        edited.Position = sender.position;
                        edited.Dimension = sender.dimension;
                        break;
                    case "name":
                        edited.Name = value;
                        break;
                    case "faction":
                        try
                        {
                            edited.Faction = Convert.ToInt32(value);
                        }
                        catch (Exception)
                        {
                            API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Faction ID Sayı olmalıydı."); return;
                        }
                        break;
                }
                db_FactionInteractives.Update(edited);
            }
        }

        [Command("drunk")]
        public void Drunk(Client sender, int playerId, int drunk)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }

            var player = db_Accounts.GetPlayerById(playerId);
            if (player == null)
            {
                API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Oyuncu bulunamadı.");
                return;
            }

            if (drunk == 1)
                Clients.ClientManager.SetPlayerDrunk(player);
            if(drunk == 0)
                Clients.ClientManager.SetPlayerUndrunk(player);
        }
        public static void RemoveItemFromInventory(Client sender, string ownerSocialClubId, int index)
        {
            if (!(API.shared.getEntityData(sender, "AdminLevel") >= 2)) { API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            var player = db_Accounts.IsPlayerOnline(ownerSocialClubId);
            if (player != null)
            {
                InventoryManager.RemoveItemFromPlayerInventoryByIndex(player, index);
            }
            else
            {
                API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Oyuncu bulunamadı!");
            }
        }
        public static void SendMessage(string text, int adminLevel = 1)
        {
            foreach (var itemPlayer in API.shared.getAllPlayers())
            {
                if (API.shared.getEntityData(itemPlayer, "AdminLevel") >= adminLevel)
                {
                    API.shared.sendChatMessageToPlayer(itemPlayer, "~b~[A]" + text);
                }
            }
        }
        public static void SendMessage(string text, int exceptFaction, int adminLevel = 1)
        {
            foreach (var itemPlayer in API.shared.getAllPlayers())
            {
                if (FactionManager.GetPlayerFaction(itemPlayer) == exceptFaction) continue;
                if (API.shared.getEntityData(itemPlayer, "AdminLevel") >= adminLevel)
                {
                    API.shared.sendChatMessageToPlayer(itemPlayer, "~b~[A]" + text);
                }
            }
        }


        // 678465 uğurun twitter kodu
        // ugursari.com.tr

        ///[Command("editobj", "/editobj [pos/pos2/rotation/faction/owner/dimension/] [id] [Vector3(x,xx_y,yy_z,zz) / param]")]
        ///public void EditObject(Client sender, string type, long _id, int _x, int _y, int _z)
        ///{
        ///    var _index = db_Objects.GetObjectIndexById(_id);
        ///    //API.consoleOutput(" Koordinat : " + _vectorInput.Split('_')[0] + " " + _vectorInput.Split('_')[1] + " " + _vectorInput.Split('_')[2]);
        ///    // var editedObj = GameObjectsManager.ObjectsOnMap[_index];
        ///    //var editedModelObj = db_Objects.currentObjectList.Items[_index];
        ///    if ("position".StartsWith(type.ToLower()))
        ///    {                
        ///        GameObjectsManager.ObjectsOnMap[_index].position = new Vector3(_x, _y, _z);
        ///        db_Objects.currentObjectList.Items[_index].FirstPosition = new Vector3(_x, _y, _z);
        ///    }
        ///    else
        ///    if ("pos2".StartsWith(type.ToLower()))
        ///    {
        ///        db_Objects.currentObjectList.Items[_index].LastPosition = new Vector3(_x, _y, _z);
        ///    }
        ///    if ("rotation".StartsWith(type.ToLower()))
        ///    {
        ///        GameObjectsManager.ObjectsOnMap[_index].rotation = new Vector3(_x, _y, _z);
        ///        db_Objects.currentObjectList.Items[_index].Rotation = new Vector3(_x, _y, _z);
        ///    }
        ///    if ("faction".StartsWith(type.ToLower()))
        ///    {
        ///        db_Objects.currentObjectList.Items[_index].FactionId = Convert.ToInt32(_x);
        ///    }
        ///    if ("owner".StartsWith(type.ToLower()))
        ///    {
        ///        db_Objects.currentObjectList.Items[_index].OwnerSocialClubName = _x.ToString();
        ///    }
        ///    if ("dimension".StartsWith(type.ToLower()))
        ///    {
        ///        db_Objects.currentObjectList.Items[_index].FactionId = _x;
        ///    }
        ///    db_Objects.SaveChanges();
        ///}
    }
}

