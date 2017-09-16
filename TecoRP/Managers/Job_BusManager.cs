using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using TecoRP.Jobs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using TecoRP.Database;

namespace TecoRP.Managers
{
    public class Job_BusManager : Script
    {
        public const string JOB_ON = "JOB_ON";
        public const string JOB_MONEY = "ON_BUS_MONEY";
        public const string JOB_VEHICLE = "JOB_VEHICLE_OWNERID";
        public Job_BusManager()
        {
            API.onEntityEnterColShape += API_onEntityEnterColShape;

        }
        Random rnd = new Random();
        private void API_onEntityEnterColShape(ColShape colshape, NetHandle entity)
        {
            if (API.getEntityData(entity, JOB_ON) != null && Convert.ToInt32(API.getEntityData(entity, "JobId")) == 1)
            {
                Client player = db_Accounts.GetPlayerById(API.getEntityData(entity, "ID"));

                if (player.isInVehicle && API.getEntityData(player, "ID") == API.getEntityData(player.vehicle, JOB_VEHICLE))
                {
                    int stopIndex = API.getEntityData(entity, JOB_ON);
                    if ((db_BusJob.CurrentBusStops.Item2[stopIndex] as ColShape) == colshape)
                    {
                        API.triggerClientEvent(player, "remove_marker");
                        stopIndex++;
                        if (stopIndex > db_BusJob.CurrentBusStops.Item1.Count - 1) { stopIndex = 0;  API.sendChatMessageToPlayer(player,"~y~Otobüs turunuzu bitirdiniz. Yeni bir tura başlamadan paranızı almak için ~s~(/otobus paramial)"); }
                        API.setEntityData(entity, JOB_ON, stopIndex);
                        int lastMoney = API.getEntityData(entity, JOB_MONEY) == null ? 0 : API.getEntityData(entity, JOB_MONEY);
                        int increasedMoney = rnd.Next(db_BusJob.CurrentBusStops.Item1[stopIndex].MinMoney, db_BusJob.CurrentBusStops.Item1[stopIndex].MaxMoney);
                        API.sendNotificationToPlayer(player, "Bu duraktan ~g~$" + increasedMoney + "~s~ kazandınız.\nToplam alacak: " + (lastMoney + increasedMoney)+"$");
                        API.setEntityData(entity, JOB_MONEY, lastMoney + increasedMoney);
                        var nextStop = db_BusJob.CurrentBusStops.Item1[stopIndex].Position;
                        API.triggerClientEvent(player, "create_marker", nextStop.X, nextStop.Y, nextStop.Z - 1);
                        player.vehicle.position = player.position;
                        //API.setVehicleEnginePowerMultiplier(player.vehicle, -99);
                        //Task.Run(async()=> {
                        //    await Task.Delay(2000);
                        //    API.setVehicleEnginePowerMultiplier(player.vehicle, 1);
                        //});
                    }
                }else
                {
                    API.sendNotificationToPlayer(player, "Bu ~r~mesleğe başladığınız~s~ araç değil!");
                }
            }

        }

        [Command("otobus", "/otobus ~y~[basla/bitir/paramial]",GreedyArg =true)]
        public void Otobus(Client sender, string type)
        {
            if ("basla".StartsWith(type.ToLower()))
            {
                if (sender.isInVehicle)
                {
                    if (API.getEntityData(sender, "JobId") == 1)
                    {
                        if (!API.hasEntityData(sender, JOB_ON))
                        {
                            var _vehicle = db_Vehicles.FindNearestVehicle(sender.position);
                            if (Vector3.Distance(sender.position, _vehicle.VehicleOnMap.position) < 2 && _vehicle.JobId == 1)
                            {
                                API.setEntityData(sender.vehicle, JOB_VEHICLE, API.getEntityData(sender, "ID"));
                                var firstBusStop = db_BusJob.CurrentBusStops.Item1.FirstOrDefault().Position;
                                API.triggerClientEvent(sender, "create_marker", firstBusStop.X, firstBusStop.Y, firstBusStop.Z - 1);
                                API.setEntityData(sender, JOB_ON, 0);
                                return;
                            }

                            API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Meslek aracında olmalısınız.");
                        }
                        else
                        {
                            API.sendChatMessageToPlayer(sender, "~y~ Zaten mesleğe başlamışsınız.");
                        }
                    }else
                    {
                        API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Doğru meslekte değilsiniz."); return;
                    }
                }
                else
                {
                    API.sendChatMessageToPlayer(sender, "~y~ Meslek otobüsünde olmalısınız.");
                }
                return;
            }
            if ("bitir".StartsWith(type.ToLower()))
            {
                if (API.getEntityData(sender, JOB_ON) != null && API.getEntityData(sender, JOB_ON) == 0)
                {
                    API.triggerClientEvent(sender, "remove_marker");
                    API.resetEntityData(sender, JOB_ON);
                    JobManager.PlayerJobComplete(sender, 1);
                    var _vehicle = db_Vehicles.FindNearestVehicle(sender.position);
                    db_Vehicles.Respawn(_vehicle.VehicleId);
                }
                else
                {
                    var splitted = type.Split(' ');
                    if (splitted.Length == 2 && splitted[1] == "onayla")
                    {
                        API.triggerClientEvent(sender, "remove_marker");
                        API.resetEntityData(sender, JOB_ON);
                    }
                    else
                    {
                        API.sendChatMessageToPlayer(sender, "~y~Mesleği şimdi bitirirseniz paranızı alamayacaksınız.\n~y~Yine de bitirmek için (~s~/otobus bitir [onayla]~y~)");

                    }
                }
                return;
            }
            if ("paramial".StartsWith(type.ToLower()))
            {
                if (!API.hasEntityData(sender,JOB_ON))
                {
                    foreach (var item in db_Jobs.currentJobsList)
                    {
                        if (item.Item1.JobId == 1 && item.Item1.Dimension == sender.dimension && Vector3.Distance(sender.position, item.Item1.Position) < item.Item1.Range)
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
                }
                API.sendChatMessageToPlayer(sender, "~y~Önce işi aldığınız yere gitmelisiniz.");
            }
        }
    }
}
