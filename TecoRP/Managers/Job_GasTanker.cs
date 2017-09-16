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

namespace TecoRP.Managers
{
    public class Job_GasTanker : Script
    {
        public const string JOB_ON = "JOB_GASTANKER_ON";
        public const string JOB_MONEY = "ON_BENZIN_MONEY";
        public const string JOB_VEHICLE = "JOB_VEHICLE_OWNERID";

        public const float JOB_GAS_FILL_VALUE = 200;

        public Job_GasTanker()
        {

        }

        [Command("benzinci", "/benzinci ~y~[basla/bitir/yukal/yukbosalt]")]
        public void GasStationJobBaseCommand(Client sender, string type)
        {
            if (API.getEntityData(sender, "JobId") != 4) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Doğru meslekte değilsiniz."); return; }
            if ("basla".StartsWith(type.ToLower()))
            {
                #region basla
                if (API.hasEntityData(sender, JOB_ON)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Zaten mesleğe başlamışsınız."); }
                var _vehicle = db_Vehicles.FindNearestVehicle(sender.position);
                if (_vehicle == null) return;
                    if (Vector3.Distance(sender.position, _vehicle.VehicleOnMap.position) < 2)
                    {
                        if (_vehicle.JobId == 4)
                        {
                            API.setEntityData(sender, JOB_ON, -1);
                            var loadingPoint = Jobs.db_Jobs.currentJobsList.FirstOrDefault(x => x.Item1.JobId == 4).Item1.TakingPosition;
                            API.setEntityData(sender.vehicle, "JOB_VEHICLE_OWNERID", API.getEntityData(sender, "ID"));
                            API.triggerClientEvent(sender, "create_marker", loadingPoint.X, loadingPoint.Y, loadingPoint.Z - 1);
                            API.sendChatMessageToPlayer(sender, "~y~Yük alma noktasına gidip ~s~((/benzinci yukal)) ~y~komutunu kullanabilirsiniz.");
                        }
                        else
                        {
                            API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Meslek aracında olmalısınız.");
                        }
                        return;
                    }
                
                #endregion
            }
            else
             if ("yukal".StartsWith(type.ToLower()) || "yükal".StartsWith(type.ToLower()))
            {
                #region yukal
                if (API.hasEntityData(sender, JOB_ON) && API.getEntityData(sender, JOB_ON) != -1) { API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Zaten mesleğe başlamışsınız."); return; }

                var _vehicle = db_Vehicles.FindNearestVehicle(sender.position);
                if (_vehicle == null) return;
                if (Vector3.Distance(sender.position, _vehicle.VehicleOnMap.position) < 2)
                    {
                        if (_vehicle.JobId == 4)
                        {
                            if (Vector3.Distance(sender.position, db_Jobs.currentJobsList.FirstOrDefault(x => x.Item1.JobId == 4).Item1.TakingPosition) < 3)
                            {
                                int lowestIndex = -1; float lastStock = float.MaxValue;
                                for (int j = 0; j < db_GasStations.CurrentGasStations.Item1.Count; j++)
                                {
                                    float stock = db_GasStations.CurrentGasStations.Item1[j].GasInStock;
                                    if (stock < lastStock && db_GasStations.CurrentGasStations.Item1[j].GasInStock + 50 < db_GasStations.CurrentGasStations.Item1[j].MaxGasInStock)
                                    {
                                        lowestIndex = j;
                                    }
                                    lastStock = stock;
                                }
                                if (lowestIndex == -1) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Şu anda benzine ihtiyacı olan bir istasyon bulunmuyor."); return; }
                                API.setEntityData(sender.vehicle, JOB_VEHICLE, API.getEntityData(sender, "ID"));
                                API.setEntityData(sender, JOB_ON, lowestIndex);
                                var pos = db_GasStations.CurrentGasStations.Item1[lowestIndex].Position;
                                API.triggerClientEvent(sender, "remove_marker");
                                API.triggerClientEvent(sender, "create_marker", pos.X, pos.Y, pos.Z - 1);
                                API.sendChatMessageToPlayer(sender, "~y~Teslim noktasına gidip ~s~ (( /benzinci yukbosalt )) ~y~komutunu kullanınız.");
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
            if ("yukbosalt".StartsWith(type.ToLower()) || "yükboşalt".StartsWith(type.ToLower()))
            {
                #region yukbosalt
                if (!sender.isInVehicle) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Aracınızı getirmelisiniz."); return; }
                if (API.hasEntityData(sender.vehicle, JOB_VEHICLE) && API.getEntityData(sender.vehicle, JOB_VEHICLE) != API.getEntityData(sender, "ID")) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bu benzin aldığınız araç değil."); return; }
                if (API.hasEntityData(sender, JOB_ON))
                {
                    int gasIndex = API.getEntityData(sender, JOB_ON);
                    if (Vector3.Distance(sender.position, db_GasStations.CurrentGasStations.Item1[gasIndex].Position) < 4)
                    {
                        db_GasStations.CurrentGasStations.Item1[gasIndex].GasInStock = (db_GasStations.CurrentGasStations.Item1[gasIndex].GasInStock + JOB_GAS_FILL_VALUE > db_GasStations.CurrentGasStations.Item1[gasIndex].MaxGasInStock ? db_GasStations.CurrentGasStations.Item1[gasIndex].MaxGasInStock : db_GasStations.CurrentGasStations.Item1[gasIndex].GasInStock + JOB_GAS_FILL_VALUE);
                        db_GasStations.Update(db_GasStations.CurrentGasStations.Item1[gasIndex]);
                        API.setEntityData(sender, JOB_ON, -2);
                        API.setEntityData(sender, JOB_MONEY,( API.hasEntityData(sender,JOB_MONEY) ? API.getEntityData(sender, JOB_MONEY) + db_GasStations.CurrentGasStations.Item1[gasIndex].CompletedMoney : db_GasStations.CurrentGasStations.Item1[gasIndex].CompletedMoney));
                        API.sendChatMessageToPlayer(sender, "~y~Paranızı yükü aldığınız yere gidip ~s~(( /benzinci bitir )) ~y~yazdığınızda alacaksınız.");
                        API.sendChatMessageToPlayer(sender, "Alacak olarak ~g~" + db_GasStations.CurrentGasStations.Item1[gasIndex].CompletedMoney + "$ ~s~hesabınzıa eklendi.");
                        var pos = db_Jobs.currentJobsList.FirstOrDefault(x => x.Item1.JobId == 4).Item1.TakingPosition;
                        API.triggerClientEvent(sender, "remove_marker");
                        API.triggerClientEvent(sender, "create_marker", pos.X, pos.Y, pos.Z - 1);
                    }
                    else
                    {
                        API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Teslim noktasına çok uzaksınız.");
                    }

                }
                else
                {
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Yük almamışsınız.");
                }
                #endregion
            }
            else
             if ("bitir".StartsWith(type.ToLower()))
            {
                #region bitir
                if (API.hasEntityData(sender, JOB_ON))
                {
                    if (API.getEntityData(sender, JOB_ON) >= 0)
                    {
                        API.triggerClientEvent(sender, "remove_marker");
                        API.resetEntityData(sender, JOB_ON);
                    }
                    else
                    {
                        if (Vector3.Distance(sender.position, db_Jobs.currentJobsList.FirstOrDefault(x => x.Item1.JobId == 4).Item1.TakingPosition) < 5)
                        {
                            if (API.getEntityData(sender.vehicle, JOB_VEHICLE) == API.getEntityData(sender, "ID"))
                            {
                                API.sendChatMessageToPlayer(sender, "~y~Paranizi almak için ~s~(( /benzinci paramial ))");
                                API.triggerClientEvent(sender, "remove_marker");
                                API.resetEntityData(sender, JOB_ON);
                                JobManager.PlayerJobComplete(sender, 4);
                            }
                            else
                            {
                                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bu mesleğe başladığınız araç değil!");
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
                                API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Mesleğinizi teslim etmeden bitirseniz paranızı alamayacaksınız.\nYine de bitirmek için (( ~y~/benzinci bitir [onayla]~s~))\n ~y~Yükü aldığınız oktaya geldiğinizde ~s~/benzinci bitir ~y~komutunu kullanabilirsiniz.");
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
            if ("paramial".StartsWith(type.ToLower()) || "paramıal".StartsWith(type.ToLower()))
            {
                #region paramial
                if ((API.hasEntityData(sender, JOB_MONEY) && !API.hasEntityData(sender, JOB_ON)))
                {
                    foreach (var item in db_Jobs.currentJobsList)
                    {
                        if (item.Item1.JobId == 4 && item.Item1.Dimension == sender.dimension && Vector3.Distance(sender.position, item.Item1.Position) < item.Item1.Range)
                        {

                            int savedMoney = API.getEntityData(sender, JOB_MONEY);
                            int playerMoney = API.getEntityData(sender, "Money");
                            playerMoney += savedMoney;
                            API.setEntityData(sender, "Money", playerMoney);
                            API.resetEntityData(sender, JOB_MONEY);
                            API.triggerClientEvent(sender, "update_money_display", playerMoney);
                            API.sendNotificationToPlayer(sender, "~g~$" + savedMoney + " ~s~kazandınız.");
                            return;
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
    }
}
