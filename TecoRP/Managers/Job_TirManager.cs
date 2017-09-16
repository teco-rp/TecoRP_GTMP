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
    public class Job_TirManager : Script
    {
        public const string JOB_ON = "ON_TIR_JOB";
        public const string JOB_MONEY = "ON_TIR_MONEY";
        public const string JOB_VEHICLE = "JOB_VEHICLE_OWNERID";
        public Job_TirManager()
        {
            API.onEntityEnterColShape += API_onEntityEnterColShape;
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        private void API_onClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            if (eventName == "return_tir_selector")
            {
                int _Id = Convert.ToInt32(arguments[0].ToString().Split(')')[0].Replace("(", String.Empty).Trim());
                CompleteYukAl(sender, _Id);
            }
        }

        private void API_onEntityEnterColShape(GrandTheftMultiplayer.Server.Managers.ColShape colshape, GrandTheftMultiplayer.Shared.NetHandle entity)
        {
            if (API.hasEntityData(entity, JOB_ON) && Convert.ToInt32(API.getEntityData(entity, "JobId")) == 2)
            {
                Client sender = db_Accounts.GetPlayerById(API.getEntityData(entity, "ID"));
                if (sender.isInVehicle && API.getEntityData(sender, "ID") == API.getEntityData(sender.vehicle, JOB_VEHICLE))
                {
                    var _Index = db_TirJob.FindTirDeliveryPointIndexById(API.getEntityData(sender, JOB_ON));
                    if ((db_TirJob.CurrentDeliveryPoints.Item2[_Index] as ColShape) == colshape)
                    {
                        API.triggerClientEvent(sender, "remove_marker");
                        API.setEntityData(sender, JOB_ON, 0);
                        API.setEntityData(sender, JOB_MONEY, db_TirJob.CurrentDeliveryPoints.Item1[_Index].DeliveryPointMoney);
                        API.sendNotificationToPlayer(sender, "Bu teslimattan ~g~$" + db_TirJob.CurrentDeliveryPoints.Item1[_Index].DeliveryPointMoney + " kazandınız. \n Paranızı tırı teslim ettiğinizde alacaksınız.");
                        var returnPoint = Jobs.db_Jobs.currentJobsList.FirstOrDefault(x => x.Item1.JobId == 2).Item1.TakingPosition;
                        API.triggerClientEvent(sender, "create_marker", returnPoint.X, returnPoint.Y, returnPoint.Z - 1);
                        API.sendChatMessageToPlayer(sender, "~y~Teslimatı tamamlamak için yük aldığınız noktaya gidip ~s~((//tir bitir )) ~y~komutunu kullanınız");
                    }
                }
                else
                {
                    API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Bu mesleğe başladığınız trailer değil!");
                }
            }
        }

        [Command("tir", "/tir [basla/yukal/bitir/paramial]",GreedyArg =true)]
        public void Tir(Client sender, string type)
        {
            if ("basla".StartsWith(type.ToLower()))
            {
                #region basla
                if (API.getEntityData(sender, "JobId") != 2) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Doğru meslekte değilsiniz."); return; }
                if (!sender.isInVehicle) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Meslek aracında olmalısınız."); return; }
                // if (sender.vehicle.trailer.Value == 0) { API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Başlayabilmek için trailer bağlamalısınız."); return; }
                var _vehicle = db_Vehicles.FindNearestVehicle(sender.position);
                if(_vehicle.JobId != 2) { API.sendChatMessageToPlayer(sender,"~r~HATA: ~s~Meslek aracında değilsiniz."); return; }
                if (API.hasEntityData(sender, JOB_ON)) { API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Zaten mesleğe başlamışsınız."); return; }
                API.setEntityData(sender.vehicle, JOB_VEHICLE, API.getEntityData(sender, "ID"));

                API.setEntityData(sender, JOB_ON, -1);
                var loadingPoint = Jobs.db_Jobs.currentJobsList.FirstOrDefault(x => x.Item1.JobId == 2).Item1;
                API.triggerClientEvent(sender, "create_marker", loadingPoint.TakingPosition.X, loadingPoint.TakingPosition.Y, loadingPoint.TakingPosition.Z - 1);
                API.sendChatMessageToPlayer(sender, "~y~Yük alma noktasına gidip ~s~((/tir yukal)) ~y~komutunu kullanabilirsiniz.");
                
                #endregion
            }
            else
             if ("yukal".StartsWith(type.ToLower()))
            {
                #region yukal
                
                if (API.getEntityData(sender, "JobId") != 2) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Doğru meslekte değilsiniz."); return; }
                if (!sender.isInVehicle) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Meslek aracında olmalısınız."); return; }
             //   if (sender.vehicle.trailer.Value == 0 ) { API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Başlayabilmek için trailer bağlamalısınız."); return; }
                if (API.hasEntityData(sender, JOB_ON) && API.getEntityData(sender, JOB_ON) != -1) { API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Zaten mesleğe başlamışsınız."); return; }
                var loadingPoint = Jobs.db_Jobs.currentJobsList.FirstOrDefault(x => x.Item1.JobId == 2).Item1;
                if (Vector3.Distance(sender.position,loadingPoint.TakingPosition)>4)
                {
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Yük alma noktasında değilsiniz.");
                    return;
                }

                List<string> names = new List<string>();
                List<string> descriptions = new List<string>();
                foreach (var item in db_TirJob.CurrentDeliveryPoints.Item1)
                {
                    names.Add("(" + item.ID + ")" + item.Name + " | " + item.DeliveryPointMoney + "$");
                    descriptions.Add("Uzaklık: " + Vector3.Distance(sender.position, item.DeliveryPointPosition));
                }
                API.triggerClientEvent(sender, "open_tir_selector", names.Count, names.ToArray(), descriptions.ToArray());
                return;
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
                        //zaten hedefe gitmediyse
                        API.triggerClientEvent(sender, "remove_marker");
                        API.resetEntityData(sender, JOB_ON);
                    }
                    else
                    {
                        //hedefe ulaştı dönüyosa
                        if (Vector3.Distance(sender.position,db_Jobs.currentJobsList.FirstOrDefault(x=>x.Item1.JobId == 2).Item1.TakingPosition)<6)
                        {
                            //teslim noktasındaysa
                            API.sendChatMessageToPlayer(sender, "~y~Paranizi almak için ~s~(( /tir paramial ))");
                            API.triggerClientEvent(sender, "remove_marker");
                            API.resetEntityData(sender, JOB_ON);
                            JobManager.PlayerJobComplete(sender, 2);
                        }
                        else
                        {
                            //yoldaysa
                            if (type.ToLower().Split(' ').LastOrDefault() == "onayla")
                            {
                                //onayladıysa
                                API.triggerClientEvent(sender, "remove_marker");
                                API.resetEntityData(sender, JOB_ON);
                                API.resetEntityData(sender, JOB_MONEY);
                            }
                            else
                            {
                                //direk onaysız yazdıysa
                                API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Mesleğinizi teslim etmeden bitirseniz paranızı alamayacaksınız.\nYine de bitirmek için (( ~y~/tir bitir [onayla]~s~))\n ~y~Yükü aldığınız oktaya geldiğinizde ~s~/tir bitir ~y~komutunu kullanabilirsiniz.");
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
                if (API.hasEntityData(sender, JOB_MONEY))
                {
                    foreach (var item in db_Jobs.currentJobsList)
                    {
                        if (item.Item1.JobId == 2 && item.Item1.Dimension == sender.dimension && Vector3.Distance(sender.position, item.Item1.Position) < 3)
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

                #endregion
            }
        }
        public void CompleteYukAl(Client sender, int _DeliverPointId)
        {
            if(!sender.isInVehicle) { API.sendChatMessageToPlayer(sender,"~r~HATA: ~s~Yük taşımaya uygun bir araçta olmalısınız."); return; }
           // if(sender.vehicle.trailer.Value == 0) { API.sendChatMessageToPlayer(sender,"~r~HATA: ~s~Yük taşımaya uygun bir araçta olmalısınız."); return; }
            API.triggerClientEvent(sender, "remove_marker");
            var deliveryPoint = db_TirJob.GetDeliveryPoint(_DeliverPointId);
            API.triggerClientEvent(sender, "create_marker", deliveryPoint.DeliveryPointPosition.X, deliveryPoint.DeliveryPointPosition.Y, deliveryPoint.DeliveryPointPosition.Z - 1);
            API.setEntityData(sender, JOB_ON, _DeliverPointId);
            API.setEntityData(sender.vehicle, JOB_VEHICLE, API.getEntityData(sender, "ID"));
        }
    }
}
