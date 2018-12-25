using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Server.API;
using TecoRP.Database;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Server.Elements;
using TecoRP.Models;
using GrandTheftMultiplayer.Shared.Math;

namespace TecoRP.Managers
{
    public class LicenseManager : Script
    {
        public const string ON_LICENSE_EXAM = "VEHICLE_LICENSE";
        public const string ON_LICENSE_EXAM_OWNER = "VEHICLE_LICENSE_OWNER";
        public LicenseManager()
        {
            db_LicensePoints.InitVehicleLicenseChks();

        }

        public static void ColshapeOnMap_onEntityEnterColShape(ColShape shape, GrandTheftMultiplayer.Shared.NetHandle entity)
        {
            if (!API.shared.hasEntityData(entity, "ID")) return;
            var _player = (Client)db_Players.FindPlayerById(API.shared.getEntityData(entity, "ID"));
            var _shapeIndex = db_LicensePoints.currentVehLicenseCheckpoints.Items.IndexOf(db_LicensePoints.currentVehLicenseCheckpoints.Items.FirstOrDefault(x => x.ColshapeOnMap == (CylinderColShape)shape));
            API.shared.consoleOutput("shape ındex: " + _shapeIndex);
            if (_shapeIndex >= 0)
            {
                if (API.shared.getEntityData(_player, ON_LICENSE_EXAM) == _shapeIndex)
                {
                    if (API.shared.getEntityData(_player.vehicle, ON_LICENSE_EXAM_OWNER) != API.shared.getEntityData(_player, "ID")) { API.shared.sendChatMessageToPlayer(_player, "~r~HATA: ~s~Bu sınava başladığınız araba değil."); return; }
                    _shapeIndex++;
                    if (_shapeIndex < db_LicensePoints.currentVehLicenseCheckpoints.Items.Count)
                    {
                        // SONRAKI CHECKPOINT
                        Clients.ClientManager.RemoveBlip(_player);
                        var pos = db_LicensePoints.currentVehLicenseCheckpoints.Items[_shapeIndex].Position;
                        Clients.ClientManager.ShowBlip(_player, pos.X, pos.Y, pos.Z);
                        API.shared.setEntityData(_player, ON_LICENSE_EXAM, _shapeIndex);
                        return;
                    }
                    else
                    {
                        //SON CHECKPOINT
                        var _vehicle = db_Vehicles.FindNearestVehicle(_player.position);
                        Clients.ClientManager.RemoveBlip(_player);

                        if (_player.vehicle.health == 1000)
                        {
                            if (_player.seatbelt)
                            {
                                int itemId = db_Items.GameItems.Values.FirstOrDefault(f => f.Type == ItemType.License && f.Value_0 == "0").ID;
                                API.shared.resetEntityData(_player, ON_LICENSE_EXAM);
                                API.shared.resetEntityData(_player.vehicle, ON_LICENSE_EXAM_OWNER);
                                if (InventoryManager.AddItemToPlayerInventory(_player, new Models.ClientItem
                                {
                                    Count = 1,
                                    Equipped = false,
                                    SpecifiedValue = _player.socialClubName,
                                    ItemId = itemId
                                }))
                                {
                                    API.shared.sendChatMessageToPlayer(_player, "~y~Ehliyetiniz envanterinize eklendi.");
                                }
                                else
                                {
                                    API.shared.sendChatMessageToPlayer(_player, "~r~HATA: ~s~Envanterinizde yeterli yer yok.");
                                    db_Items.DropItem(new Models.ClientItem { Count = 1, Equipped = false, ItemId = itemId, SpecifiedValue = _player.socialClubName }, _player);
                                }
                                db_Vehicles.Respawn(_vehicle.VehicleId);
                            }
                            else
                            {
                                API.shared.sendChatMessageToPlayer(_player, "~r~UYARI: ~s~Emniyet kemeriniz takılı olmadığı için sınavı geçemediniz.");
                                db_Vehicles.Respawn(_vehicle.VehicleId);
                            }
                        }
                        else
                        {
                            API.shared.sendChatMessageToPlayer(_player, "~r~UYARI: ~s~Sınav aracı hasar aldığından dolayı sınavı geçemediniz.");
                            db_Vehicles.Respawn(_vehicle.VehicleId);
                        }
                    }


                }
            }
            else
            {
                API.shared.consoleOutput("Ehliyette colshape index bulunamadı.");
            }

        }

        // JOBID 99 olan araçlar ehliyet arabası
        [Command("ehliyet", "/ehliyet [basla/goster]", GreedyArg = true)]
        public void VehicleLicenseMainCommand(Client sender, string type)
        {
            if ("basla".StartsWith(type.ToLower()))
            {
                var _inventory = (Inventory)API.getEntityData(sender, "inventory");
                if (_inventory.ItemList.FirstOrDefault(x => x.ItemId == db_Items.GameItems.Values.FirstOrDefault(f => f.Type == ItemType.License && f.Value_0 == "0").ID) != null)
                {
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Zaten ehliyetiniz bulunuyor."); return;
                }

                var licenseTakingPoint = db_LicensePoints.CurrentLicenseTakings.Item1.FirstOrDefault(x => x.LicenseType == 0);
                if (licenseTakingPoint == null) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Şu anda herhangi bir ehliyet kursu bulunmuyor."); return; }
                if (Vector3.Distance(sender.position, licenseTakingPoint.Position) > 3)
                {
                    API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Herhangi bir ehliyet alma noktasının yakınında değilsiniz.");
                    return;
                }
                var _HealthReport = _inventory.ItemList.FirstOrDefault(x => x.ItemId == db_Items.GameItems.Values.FirstOrDefault(f => f.Type == ItemType.License && f.Value_0 == "-1").ID);
                if (_HealthReport != null)
                {
                    int money = API.getEntityData(sender, "Money");
                    if (money >= licenseTakingPoint.Price)
                    {
                        money -= licenseTakingPoint.Price;
                        API.setEntityData(sender, "Money", money);
                        API.triggerClientEvent(sender, "update_money_display", money);
                        InventoryManager.RemoveItemFromPlayerInventory(sender, _HealthReport.ItemId);
                        var pos = db_Vehicles.FindNearestVehicle(sender.position, 99).VehicleOnMap.position;
                        API.setEntityData(sender, ON_LICENSE_EXAM, "-1");
                        Clients.ClientManager.ShowBlip(sender, pos.X, pos.Y, pos.Z);
                        API.sendChatMessageToPlayer(sender, "~s~Ehliyet sınavı için araca binin ve ~y~/ehliyet sınav~s~ yazın.");

                        return;
                    }
                    else
                    {
                        API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Ehliyet alabilmek için yeterli paranız bulunmuyor. Gerekli olan: ~g~" + licenseTakingPoint.Price + "$");
                    }
                }
                else
                {
                    API.shared.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Bunun için öncelikle sağlık raporu almalısınız.");
                }

            }
            else
            if (type.ToLower().StartsWith("goster"))
            {
                var licenses = InventoryManager.GetItemsFromPlayerInventory(sender, ItemType.License).Where(x => x.Item1.Value_0 == "0").ToList();
                var splitted = type.Split(' ');
                if (splitted.Length < 2) { API.shared.sendChatMessageToPlayer(sender,"/ehliyet goster [OyuncuID]"); return; }
                var targetPlayer = db_Players.GetPlayerById(Convert.ToInt32(splitted[1]));
                if (targetPlayer == null){ API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Oyuncu bulunamadı."); return; }

                Player userInformation = new Player(sender.socialClubName);
                if (licenses.Count > 1)
                {
                    try
                    {
                        userInformation = db_Players.GetOfflineUserDatas(licenses[Convert.ToInt32(splitted[2])].Item2.SpecifiedValue);

                    }
                    catch (Exception ex)
                    {
                        if (ex.GetType() == typeof(ArgumentOutOfRangeException) || ex.GetType() == typeof(IndexOutOfRangeException))
                        {
                            API.sendChatMessageToPlayer(sender, "~y~Envanterinizde birden fazla kimlik var. Göstermek istediğinizin numarasını yazın.");
                            API.sendChatMessageToPlayer(sender, "~s~/ehliyet goster [OyuncuID] ~y~[Ehliyet Numarası]");
                            string strIdentities = ""; int index = 0;
                            foreach (var item in licenses)
                            {
                                strIdentities += "~y~" + index + " ~s~- " + (String.IsNullOrEmpty(item.Item2.SpecifiedValue) ? "Belirsiz" : db_Players.GetOfflineUserDatas(item.Item2.SpecifiedValue).CharacterName) + "\n";
                                index++;
                            }
                            API.shared.sendChatMessageToPlayer(sender, strIdentities); return;
                        }
                    }
                }
                else
                {
                    userInformation = db_Players.GetOfflineUserDatas(licenses.FirstOrDefault().Item2.SpecifiedValue);
                }

                if(userInformation == null) { userInformation.CharacterName = "Belirsiz"; }

                API.shared.sendChatMessageToPlayer(targetPlayer, "~y~Adı: ~s~" + userInformation.CharacterName + "~y~ | ~s~Sürücü yetkinliği: ~g~VAR");
                RPGManager rpgMgr = new Managers.RPGManager();
                rpgMgr.Me(sender, " adlı kişi " + API.getEntityData(targetPlayer, "CharacterName") + " adlı kişiye ehliyetini gösterir.");
            }
            if ("sinav".StartsWith(type.ToLower()) || "sınav".StartsWith(type.ToLower()))
            {
                if (sender.isInVehicle)
                {
                    if (API.getEntityData(sender, ON_LICENSE_EXAM) == "-1")
                    {
                        var _vehicle = db_Vehicles.FindNearestVehicle(sender.position);
                        if (_vehicle.JobId == 99)
                        {
                            Clients.ClientManager.RemoveBlip(sender);
                            sender.vehicle.repair();
                            API.setEntityData(sender, ON_LICENSE_EXAM, 0);
                            API.setEntityData(sender.vehicle, ON_LICENSE_EXAM_OWNER, API.getEntityData(sender, "ID"));
                            var firstPoint = db_LicensePoints.currentVehLicenseCheckpoints.Items.FirstOrDefault();
                            if (firstPoint != null)
                            {
                                Clients.ClientManager.ShowBlip(sender, firstPoint.Position.X, firstPoint.Position.Y, firstPoint.Position.Z);
                            }
                            else
                            {
                                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Şu anda ehliyet için bir rota bulunmuyor.");
                            }
                        }
                        else
                        {
                            API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Ehliyet aracında olmalısınız.");
                        }
                    }
                    else
                    {
                        API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Öncelikle ehliyet kursuna başlamış olmalısınız.");
                    }
                }
                else
                {
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bunun için ehliyet aracında olmalısınız.");
                }
            }
        }

    }
}
