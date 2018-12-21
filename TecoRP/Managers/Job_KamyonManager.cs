using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TecoRP.Database;
using TecoRP.Jobs;
using TecoRP.Models;

namespace TecoRP.Managers
{
    public class Job_KamyonManager : Script
    {
        public const string JOB_ON = "ON_KAMYON_JOB";
        public const string JOB_MONEY = "ON_KAMYON_MONEY";
        public const string JOB_METALPART = "ON_KAMYON_METAL";
        public const string JOB_VEHICLE = "JOB_VEHICLE_OWNERID";

        public Job_KamyonManager()
        {
            API.onClientEventTrigger += API_onClientEventTrigger;
            API.onEntityEnterColShape += API_onEntityEnterColShape;
        }

        private void API_onEntityEnterColShape(GrandTheftMultiplayer.Server.Managers.ColShape colshape, GrandTheftMultiplayer.Shared.NetHandle entity)
        {
            if (API.hasEntityData(entity, JOB_ON) && Convert.ToInt32(API.getEntityData(entity, "JobId")) == 3)
            {
                Client sender = db_Players.GetPlayerById(API.getEntityData(entity, "ID"));
                if (sender.isInVehicle && API.getEntityData(sender, "ID") == API.getEntityData(sender.vehicle, JOB_VEHICLE))
                {
                    var _Index = db_KamyonJob.FindKamyonDeliveryPointIndexById(API.getEntityData(sender, JOB_ON));
                    if ((db_KamyonJob.CurrentDeliveryPoints.Item2[_Index] as ColShape) == colshape)
                    {
                        API.triggerClientEvent(sender, "remove_marker");

                        API.setEntityData(sender, JOB_ON, 0);
                        if (db_KamyonJob.CurrentDeliveryPoints.Item1[_Index].Type == Models.DeliveryType.Money)
                        {
                            API.setEntityData(sender, JOB_MONEY, (API.hasEntityData(sender, JOB_MONEY) ? API.getEntityData(sender, JOB_MONEY) : 0) + db_KamyonJob.CurrentDeliveryPoints.Item1[_Index].CompletedValue);
                            API.sendNotificationToPlayer(sender, "Bu teslimattan ~g~$" + db_KamyonJob.CurrentDeliveryPoints.Item1[_Index].CompletedValue + " kazandınız. \n Paranızı kamyonu teslim ettiğinizde alacaksınız.");
                            var returnPoint = Jobs.db_Jobs.currentJobsList.FirstOrDefault(x => x.Item1.JobId == 3).Item1.TakingPosition;
                            API.triggerClientEvent(sender, "create_marker", returnPoint.X, returnPoint.Y, returnPoint.Z - 1);
                        }
                        else
                        {
                            API.setEntityData(sender, JOB_METALPART, db_KamyonJob.CurrentDeliveryPoints.Item1[_Index].CompletedValue);
                            API.sendNotificationToPlayer(sender, "Bu teslimattan ~y~" + db_KamyonJob.CurrentDeliveryPoints.Item1[_Index].CompletedValue + " ~s~metal parça kazandınız. \n Kazancınızı tırı teslim ettiğinizde alacaksınız.");
                            var returnPoint = Jobs.db_Jobs.currentJobsList.FirstOrDefault(x => x.Item1.JobId == 3).Item1.TakingPosition;
                            API.triggerClientEvent(sender, "create_marker", returnPoint.X, returnPoint.Y, returnPoint.Z - 1);
                        }
                        API.sendChatMessageToPlayer(sender, "~y~Teslimatı tamamlamak için yük aldığınız noktaya gidip ~s~((//kamyon bitir )) ~y~komutunu kullanınız");


                    }
                }
                else
                {
                    API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Bu mesleğe başladığınız kamyon değil!");
                }
            }
        }
        private void API_onClientEventTrigger(GrandTheftMultiplayer.Server.Elements.Client sender, string eventName, params object[] arguments)
        {
            if (eventName == "return_kamyon_selector")
            {
                int _Id = Convert.ToInt32(arguments[0].ToString().Split(')')[0].Replace("(", String.Empty).Trim());
                CompleteYukAl(sender, _Id);
            }
        }
        [Command("kamyon", "/kamyon [basla/yukal/bitir]", GreedyArg = true)]
        public void RepairVehicleOnJob(Client sender, string type)
        {
            if ("basla".StartsWith(type.ToLower()))
            {
                #region Basla
                if (API.getEntityData(sender, "JobId") != 3) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Doğru meslekte değilsiniz."); return; }
                if (API.hasEntityData(sender, JOB_ON)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Zaten mesleğe başlamışsınız."); }
                var _vehicle = db_Vehicles.FindNearestVehicle(sender.position);
                if (_vehicle == null) return;
                if (Vector3.Distance(sender.position, _vehicle.VehicleOnMap.position) < 2)
                {
                    if (_vehicle.JobId == 3)
                    {
                        API.setEntityData(sender, JOB_ON, -1);
                        var loadingPoint = Jobs.db_Jobs.currentJobsList.FirstOrDefault(x => x.Item1.JobId == 3).Item1.TakingPosition;
                        API.setEntityData(sender.vehicle, "JOB_VEHICLE_OWNERID", API.getEntityData(sender, "ID"));
                        API.triggerClientEvent(sender, "create_marker", loadingPoint.X, loadingPoint.Y, loadingPoint.Z - 1);
                        API.sendChatMessageToPlayer(sender, "~y~Yük alma noktasına gidip ~s~((/kamyon yukal)) ~y~komutunu kullanabilirsiniz.");

                    }
                    else
                    {
                        API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Meslek kamyonunda olmalısınız.");
                    }
                    return;
                }

                #endregion
            }
            else
             if ("yukal".StartsWith(type.ToLower()))
            {
                #region yukal
                if (API.getEntityData(sender, "JobId") != 3) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Doğru meslekte değilsiniz."); return; }
                if (API.hasEntityData(sender, JOB_ON) && API.getEntityData(sender, JOB_ON) != -1) { API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Zaten mesleğe başlamışsınız."); return; }

                var _vehicle = db_Vehicles.FindNearestVehicle(sender.position);
                if (_vehicle == null) return;
                if (Vector3.Distance(sender.position, _vehicle.VehicleOnMap.position) < 2)
                {
                    if (_vehicle.JobId == 3)
                    {
                        if (Vector3.Distance(sender.position, db_Jobs.currentJobsList.FirstOrDefault(x => x.Item1.JobId == 3).Item1.TakingPosition) < 3)
                        {

                            API.setEntityData(sender.vehicle, JOB_VEHICLE, API.getEntityData(sender, "ID"));
                            List<string> names = new List<string>();
                            List<string> descriptions = new List<string>();
                            foreach (var item in db_KamyonJob.CurrentDeliveryPoints.Item1)
                            {
                                names.Add("(" + item.ID + ")" + item.Name + " | " + item.CompletedValue + (item.Type == Models.DeliveryType.Money ? "$" : "Metal"));
                                descriptions.Add("Uzaklık: " + Vector3.Distance(sender.position, item.DeliveryPoint));
                            }
                            API.triggerClientEvent(sender, "open_kamyon_selector", names.Count, names.ToArray(), descriptions.ToArray());
                            return;
                        }
                        else
                        {
                            API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Yük alma noktasında değilsiniz.");
                        }

                    }
                    else
                    {
                        API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Meslek kamyonunda olmalısınız.");
                    }
                    return;
                }

                #endregion
            }
            else
            if ("bitir".StartsWith(type.ToLower()))
            {
                #region bitir
                if (API.hasEntityData(sender, JOB_ON))
                {
                    if (API.getEntityData(sender, JOB_ON) != 0)
                    {
                        API.triggerClientEvent(sender, "remove_marker");
                        API.resetEntityData(sender, JOB_ON);
                    }
                    else
                    {
                        if (Vector3.Distance(sender.position, db_Jobs.currentJobsList.FirstOrDefault(x => x.Item1.JobId == 3).Item1.TakingPosition) < 6)
                        {
                            var _vehicle = db_Vehicles.FindNearestVehicle(sender.position);
                            if (_vehicle == null) return;
                            if (Vector3.Distance(sender.position, _vehicle.VehicleOnMap.position) < 2)
                            {
                                if (API.getEntityData(_vehicle.VehicleOnMap, JOB_VEHICLE) == API.getEntityData(sender, "ID"))
                                {

                                    API.triggerClientEvent(sender, "remove_marker");
                                    if (API.hasEntityData(sender, JOB_MONEY))
                                    {
                                        API.resetEntityData(sender, JOB_ON);
                                        JobManager.PlayerJobComplete(sender, 3);
                                        int savedMoney = (API.hasEntityData(sender, JOB_MONEY) ? API.getEntityData(sender, JOB_MONEY) : 0);
                                        InventoryManager.AddMoneyToPlayer(sender, savedMoney);
                                        API.resetEntityData(sender, JOB_MONEY);
                                        API.sendNotificationToPlayer(sender, "~g~$" + savedMoney + " ~s~kazandınız.");
                                    }
                                    if (API.hasEntityData(sender, JOB_METALPART))
                                    {
                                        API.resetEntityData(sender, JOB_ON);
                                        int savedMetalPart = API.getEntityData(sender, JOB_METALPART);
                                        JobManager.PlayerJobComplete(sender, 3);
                                        InventoryManager.AddMetalPartsToPlayer(sender,savedMetalPart);
                                        API.resetEntityData(sender, JOB_METALPART);
                                        API.sendNotificationToPlayer(sender, "~y~$" + savedMetalPart + " ~s~ adet metal parça kazandınız.");
                                    }

                                    db_Vehicles.Respawn(_vehicle.VehicleId);

                                    // API.sendChatMessageToPlayer(sender, "~y~Paranizi almak için ~s~(( /kamyon paramial ))");
                                }
                                else
                                {
                                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bu mesleğe başladığınız araç değil!");
                                }
                            }


                        }
                        else
                        {
                            if (type.ToLower().Split(' ').LastOrDefault() == "onayla")
                            {
                                API.triggerClientEvent(sender, "remove_marker");
                                API.resetEntityData(sender, JOB_ON);
                                API.resetEntityData(sender, JOB_MONEY);
                            }
                            else
                            {
                                API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Mesleğinizi teslim etmeden bitirseniz paranızı alamayacaksınız.\nYine de bitirmek için (( ~y~/kamyon bitir [onayla]~s~))\n ~y~Yükü aldığınız oktaya geldiğinizde ~s~/kamyoncu bitir ~y~komutunu kullanabilirsiniz.");
                            }
                        }
                    }
                }
                else
                {
                    API.sendChatMessageToPlayer(sender, "~r~Zaten mesleğe başlamamışsınız.");
                }
                #endregion
            }
            else
            if ("paramial".StartsWith(type.ToLower()))
            {
                #region paramial
                if ((API.hasEntityData(sender, JOB_MONEY) || API.hasEntityData(sender, JOB_METALPART)) && !API.hasEntityData(sender, JOB_ON))
                {
                    foreach (var item in db_Jobs.currentJobsList)
                    {
                        if (item.Item1.JobId == 3 && item.Item1.Dimension == sender.dimension && Vector3.Distance(sender.position, item.Item1.Position) < item.Item1.Range)
                        {
                            if (API.hasEntityData(sender, JOB_MONEY))
                            {
                                API.shared.consoleOutput("SAVED MONEY: " + API.getEntityData(sender, JOB_MONEY));
                                int savedMoney = (API.hasEntityData(sender, JOB_MONEY) ? API.getEntityData(sender, JOB_MONEY) : 0);
                                int playerMoney = API.getEntityData(sender, "Money");
                                playerMoney += savedMoney;
                                API.setEntityData(sender, "Money", playerMoney);
                                API.resetEntityData(sender, JOB_MONEY);
                                API.triggerClientEvent(sender, "update_money_display", playerMoney);
                                API.sendNotificationToPlayer(sender, "~g~$" + savedMoney + " ~s~kazandınız.");
                                return;
                            }
                            if (API.hasEntityData(sender, JOB_METALPART))
                            {
                                int savedMetalPart = API.getEntityData(sender, JOB_METALPART);
                                var _inventory = (Inventory)API.getEntityData(sender, "inventory");
                                _inventory.MetalParts += savedMetalPart;
                                API.setEntityData(sender, "inventory", _inventory);
                                API.resetEntityData(sender, JOB_METALPART);
                                API.sendNotificationToPlayer(sender, "~y~$" + savedMetalPart + " ~s~ adet metal parça kazandınız.");
                                return;
                            }

                        }
                    }
                }
                else
                {
                    API.sendNotificationToPlayer(sender, "~y~Önce mesainizi bitirmelisiniz.");
                    return;
                }
                API.sendChatMessageToPlayer(sender, "~y~Önce işi aldığınız yere gitmelisiniz.");
                #endregion
            }
        }

        public void CompleteYukAl(Client sender, int _DeliveryPointId)
        {
            if (!sender.isInVehicle) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Yük taşımaya uygun bir araçta olmalısınız."); return; }
            API.triggerClientEvent(sender, "remove_marker");
            var deliveryPoint = db_KamyonJob.GetDeliveryPoint(_Id: _DeliveryPointId);
            API.triggerClientEvent(sender, "create_marker", deliveryPoint.DeliveryPoint.X, deliveryPoint.DeliveryPoint.Y, deliveryPoint.DeliveryPoint.Z - 1);
            API.setEntityData(sender, JOB_ON, _DeliveryPointId);
            API.setEntityData(sender.vehicle.trailer, JOB_VEHICLE, API.getEntityData(sender, "ID"));
        }
    }
}
