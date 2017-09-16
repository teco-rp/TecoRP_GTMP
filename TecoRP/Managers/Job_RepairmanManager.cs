using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using TecoRP.Database;
using TecoRP.Models;

namespace TecoRP.Managers
{
    public class Job_RepairmanManager : Script
    {
        static Random random = new Random();
        public const int Repair_Door_Requires = 500;
        public const int Item_Lastik_Requires = 250;
        public const int Item_RapairKit_Requires = 1000;
        [Command("tamir", "/tamir [motor/kapı]\n Motor: ~y~500 M.Parça ~s~Kapı: ~y~200M.Parça" + "\n~y~/modifiyeler")]
        public void RepairmanRepair(Client sender, string type)
        {
            if (sender.isInVehicle) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bunu araçtayken yapamazsınız."); return; }
            List<Models.Jobs> jobAbilities = (API.hasEntityData(sender, "JobAbilities") ? API.getEntityData(sender, "JobAbilities") : null);
            if (jobAbilities == null) { jobAbilities = new List<Models.Jobs>(); }
            var repairmanAbility = jobAbilities.FirstOrDefault(x => x.JobID == 11);
            if (repairmanAbility == null) { jobAbilities.Add(new Models.Jobs { JobID = 11, JobLevel = 1, JobsCompleted = 0 }); }
            repairmanAbility = jobAbilities.FirstOrDefault(x => x.JobID == 11);
            var _inventory = (Inventory)API.getEntityData(sender, "inventory");

            var _vehicle = db_Vehicles.FindNearestVehicle(sender.position);
            if (_vehicle == null) return;
            if (Vector3.Distance(sender.position, _vehicle.VehicleOnMap.position) < 5)
            {
                if ("motor".StartsWith(type.ToLower()))
                {
                    if (_vehicle.VehicleOnMap.isDoorOpen(4) || _vehicle.VehicleOnMap.isDoorBroken(4))
                    {
                        if ((repairmanAbility != null && repairmanAbility.JobLevel < 3) && API.getEntityData(sender, "JobId") != 11) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bunu meslekte değilken yapabilmek için bu meslekte 3 seviye olmalısınız."); return; }
                        if (API.getVehicleHealth(_vehicle.VehicleOnMap) >= 900) { API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Bu aracın tamire ihtiyacı yok."); return; }
                        int requiredRepairValue = 1000 - (int)API.getVehicleHealth(_vehicle.VehicleOnMap);
                        if (_inventory.MetalParts >= requiredRepairValue)
                        {
                            //API.consoleOutput("Null?: " + (repairmanAbility == null).ToString());
                            //API.consoleOutput("Level: " + repairmanAbility.JobLevel);
                            if (random.Next(0, 100) < repairmanAbility.JobLevel * 20)
                            {
                                API.setVehicleHealth(_vehicle.VehicleOnMap, 1000);
                                _inventory.MetalParts -= requiredRepairValue;
                                API.sendChatMessageToPlayer(sender, "~g~BAŞARILI: ~s~Aracın motorunu başarıyla tamir ettiniz.");
                                repairmanAbility.JobsCompleted++;
                            }
                            else
                            {
                                API.sendChatMessageToPlayer(sender, "~r~BAŞARISIZ: ~s~Tamir başarısız oldu." + (requiredRepairValue * 0.1) + " metal parça harcamış oldun.");
                                _inventory.MetalParts -= (int)(Convert.ToSingle(requiredRepairValue) * 0.1f);
                            }
                            API.setEntityData(sender, "JobAbilities", jobAbilities);
                            JobManager.PlayerJobComplete(sender, 11);
                        }
                        else
                        {
                            API.sendChatMessageToPlayer(sender, "~r~BAŞARISIZ: ~s~Bu tamir için üzerinizde yeterli metal parça yok." + requiredRepairValue + " gerekli. \n~y~Kamyonculuk yaparak bu parçalardan kazanabilirsin.");
                        }
                    }
                    else
                    {
                        API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Tamir edebilmek için önce kaputu açmanız gerekmektedir.");
                    }
                }
                else
                if ("kapi".StartsWith(type.ToLower()) || "kapı".StartsWith(type.ToLower()))
                {

                    if ((repairmanAbility != null && repairmanAbility.JobLevel < 2) && API.getEntityData(sender, "JobId") != 11) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bunu meslekte değilken yapabilmek için bu meslekte 2 seviye olmalısınız."); return; }

                    bool neededReapirDoor = false;
                    for (int j = 0; j < 5; j++)
                    {
                        if (API.isVehicleDoorBroken(_vehicle.VehicleOnMap, j)) { neededReapirDoor = true; break; }
                    }

                    if (_inventory.MetalParts >= Repair_Door_Requires)
                    {
                        if (neededReapirDoor)
                        {
                            for (int j = 0; j < 5; j++)
                            {
                                if (API.isVehicleDoorBroken(_vehicle.VehicleOnMap, j))
                                {

                                    _vehicle.VehicleOnMap.breakDoor(4);
                                    _vehicle.VehicleOnMap.fixDoor(4);
                                    _vehicle.VehicleOnMap.fixWindow(4);

                                    return;
                                }
                            }
                        }
                        else
                        {
                            API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bu aracın kapıları sağlam.");
                        }
                    }
                    else
                    {
                        API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Üzerinizde yeterli metal parça bulunmuyor. Gerekli olan: ~y~" + Repair_Door_Requires);
                    }
                }
            }


            API.setEntityData(sender, "inventory", _inventory);

        }

        [Command("modifiyeler")]
        public void ModsOnVehicle(Client sender)
        {
            if (JobManager.GetSkillLevel(sender, 11) < 2) { API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bunu yapabilecek yeteneğiniz yok."); return; }
            var _vehicle = db_Vehicles.FindNearestVehicle(sender.position);
            if (_vehicle != null && Vector3.Distance(_vehicle.VehicleOnMap.position, sender.position) < 4)
            {
                if (!_vehicle.IsLocked)
                {
                    List<string> names = new List<string>();
                    List<int> IDs = new List<int>();
                    foreach (var item in GetVehicleMods(_vehicle))
                    {
                        var gameItem = db_Items.GameItems.FirstOrDefault(x => x.Value.Type == ItemType.RepairPart && int.Parse(x.Value.Value_0) == item.Item1 && int.Parse(x.Value.Value_1) == item.Item2).Value;
                        if (gameItem == null)
                            continue;
                        names.Add(gameItem.Name);
                        IDs.Add(gameItem.ID);

                    }
                    Clients.ClientManager.ShowMods(sender, names, IDs, _vehicle);
                    return;
                }
                API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bu araç kilitli.");
                return;
            }
            API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Etrafınızda bir araç bulunmuyor.");
        }


        public static void CompleteRemoveMod(Client sender, int vehId, int gameItemId)
        {
            var _vehicle = db_Vehicles.GetVehicle(vehId);
            if (_vehicle != null && Vector3.Distance(_vehicle.VehicleOnMap.position, sender.position) < 4)
            {
                if (random.Next(0, 100) > (10 * JobManager.GetSkillLevel(sender, 11)))
                {
                    API.shared.sendChatMessageToPlayer(sender, "~r~BAŞARISIZ: ~s~Parçayı sökme bu sefer başarısız oldu.");
                    return;
                }

                if (InventoryManager.AddItemToPlayerInventory(sender, new ClientItem { Count = 1, Equipped = false, ItemId = gameItemId }))
                    _vehicle.VehicleOnMap.removeMod(int.Parse(db_Items.GetItemById(gameItemId).Value_0));


                API.shared.sendChatMessageToPlayer(sender,"~g~BAŞARILI: ~s~Parçayı sökme başarılı oldu.");
            }
            API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~ Araç bulunamadı.");
        }
        public static List<Tuple<int, int>> GetVehicleMods(Models.Vehicle _vehicle)
        {
            List<Tuple<int, int>> returnModel = new List<Tuple<int, int>>();
            for (int i = 0; i < 70; i++)
            {
                if (_vehicle.Mods[i] > -1)
                {
                    returnModel.Add(new Tuple<int, int>(i, _vehicle.Mods[i]));
                }
            }
            return returnModel;
        }

        //[Command("tamirci", "/tamirci [uret] [lastik/tamirkiti]", GreedyArg = true)]
        //public void Repairman(Client sender, string commandParam)
        //{
        //    if (API.getEntityData(sender, "JobId") != 11) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bunu yapabilmek için tamirci olmanız gerekiyor."); return; }
        //    string value = "";
        //    if (commandParam.Split(' ').Count() > 0)
        //    {
        //        value = commandParam.Split(' ').LastOrDefault();
        //        var _inventory = (Inventory)API.getEntityData(sender, "inventory");

        //        if ("lastik".StartsWith(value.ToLower()))
        //        {
        //            #region Lastik
        //            if (_inventory.MetalParts >= Item_Lastik_Requires)
        //            {
        //                var _lastik = db_Items.GameItems.Values.FirstOrDefault(x => x.Type == ItemType.RepairPart && x.Value_0 == "-1");
        //                if (_lastik.ID != 0)
        //                {
        //                    var _InInventoryIndex = _inventory.ItemList.IndexOf(_inventory.ItemList.FirstOrDefault(x => x.ItemId == _lastik.ID));
        //                    if (_InInventoryIndex >= 0)
        //                    {
        //                        if (_inventory.ItemList[_InInventoryIndex].Count < _lastik.MaxCount)
        //                        {
        //                            _inventory.ItemList[_InInventoryIndex].Count++;
        //                        }
        //                        else
        //                        {
        //                            API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bu eşyadan daha fazla taşıyamazsınız.");
        //                            return;
        //                        }

        //                    }
        //                    else
        //                    {
        //                        _inventory.ItemList.Add(new ClientItem { Count = 1, Equipped = false, ItemId = _lastik.ID });
        //                        _inventory.MetalParts -= Item_Lastik_Requires;
        //                    }
        //                    API.setEntityData(sender, "inventory", _inventory);

        //                }
        //                else
        //                {
        //                    API.consoleOutput(LogCat.Trace, "GameItem içerisinde lastik bulunamadı.");
        //                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Ciddi bir hata oluştu.");
        //                }
        //            }
        //            else
        //            {
        //                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~ÜZerinizde yeterli metal parça bulunmuyor. Gerekli olan: ~y~" + Item_Lastik_Requires);
        //            }
        //            #endregion
        //        }
        //        else
        //        if ("tamirkiti".StartsWith(value.ToLower()))
        //        {
        //            #region TamirKiti
        //            if (_inventory.MetalParts >= Item_RapairKit_Requires)
        //            {
        //                var repairKit = db_Items.GameItems.Values.FirstOrDefault(x => x.Type == ItemType.RepairPart && x.Value_0 == "-2");
        //                if (repairKit != null)
        //                {
        //                    var _InventoryIndex = _inventory.ItemList.IndexOf(_inventory.ItemList.FirstOrDefault(x => x.ItemId == repairKit.ID));
        //                    if (_InventoryIndex >= 0)
        //                    {
        //                        _inventory.ItemList[_InventoryIndex].Count++;
        //                        _inventory.MetalParts -= Item_RapairKit_Requires;
        //                        API.setEntityData(sender, "inventory", _inventory);
        //                        return;
        //                    }
        //                    else
        //                    {
        //                        _inventory.ItemList.Add(new Models.ClientItem { Count = 1, Equipped = false, ItemId = repairKit.ID });
        //                        API.setEntityData(sender, "inventory", _inventory);
        //                        return;
        //                    }
        //                }
        //                else
        //                {
        //                    API.consoleOutput(LogCat.Trace, "GameItem içerisinde lastik bulunamadı.");
        //                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Ciddi bir hata oluştu.");
        //                }
        //            }
        //            else
        //            {
        //                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~ÜZerinizde yeterli metal parça bulunmuyor. Gerekli olan: ~y~" + Item_RapairKit_Requires);
        //            }
        //            #endregion
        //        }
        //    }
        //    else
        //    {
        //        API.sendChatMessageToPlayer(sender, "/tamirci ~y~uret ~s~ [lastik/tamirkiti]");
        //    }
        //}

    }
}
