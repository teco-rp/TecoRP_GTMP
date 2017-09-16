using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
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
    public class Job_BankDeliveryManager : Script
    {
        public const string JOB_ON = "JOB_ON";
        public const string JOB_MONEY = "ON_BANK_MONEY";
        public const string JOB_VEHICLE = "JOB_VEHICLE_OWNERID";

        public const int JOB_MONEY_PER_ATM = 300;

        public int AverageValue
        {
            get
            {
                int total = 0; int count = 0;
                foreach (var item in db_Banks.CurrentBanks.Item1)
                {
                    if (item.TypeOfBank == Models.BankType.ATM)
                    {
                        total += item.MoneyCountInInside;
                        count++;
                    }
                }
                return total / count;
            }
        }

        public Job_BankDeliveryManager()
        {

        }
        [Command("bankacı", "/bankacı ~y~[basla/bitir/yukal/yukbosalt]", Alias = "bankaci")]
        public void BankDeliveryJobBaseCommand(Client sender, string type)
        {
            if (API.getEntityData(sender, "JobId") != 5) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Doğru meslekte değilsiniz."); return; }
            if ("basla".StartsWith(type.ToLower()) || "başla".StartsWith(type.ToLower()))
            {
                #region basla
                if (API.hasEntityData(sender, JOB_ON)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Zaten mesleğe başlamışsınız."); }
                var _vehicle = db_Vehicles.FindNearestVehicle(sender.position);
                    if (Vector3.Distance(sender.position, _vehicle.VehicleOnMap.position) < 2)
                    {
                        if (_vehicle.JobId == 5)
                        {
                            API.setEntityData(sender, JOB_ON, -1);
                            var loadingPoint = Jobs.db_Jobs.currentJobsList.FirstOrDefault(x => x.Item1.JobId == 5).Item1.TakingPosition;
                            API.setEntityData(sender.vehicle, "JOB_VEHICLE_OWNERID", API.getEntityData(sender, "ID"));
                            API.triggerClientEvent(sender, "create_marker", loadingPoint.X, loadingPoint.Y, loadingPoint.Z - 1);
                            API.sendChatMessageToPlayer(sender, "~y~Yük alma noktasına gidip ~s~((/bankaci yukal)) ~y~komutunu kullanabilirsiniz.");
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
                        if (_vehicle.JobId == 5)
                        {
                            if (Vector3.Distance(sender.position, db_Jobs.currentJobsList.FirstOrDefault(x => x.Item1.JobId == 5).Item1.TakingPosition) < 3)
                            {
                                int lowestIndex = -1; float lastStock = float.MaxValue;
                                for (int j = 0; j < db_Banks.CurrentBanks.Item1.Count; j++)
                                {
                                    float stock = db_Banks.CurrentBanks.Item1[j].MoneyCountInInside;
                                    if (stock < lastStock)
                                    {
                                        lowestIndex = j;
                                    }
                                    lastStock = stock;
                                }
                                if (lowestIndex == -1) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Şu anda benzine ihtiyacı olan bir istasyon bulunmuyor."); return; }
                                API.setEntityData(sender.vehicle, JOB_VEHICLE, API.getEntityData(sender, "ID"));
                                API.setEntityData(sender, JOB_ON, lowestIndex);
                                var pos = db_Banks.CurrentBanks.Item1[lowestIndex].Position;
                                API.triggerClientEvent(sender, "remove_marker");
                                API.triggerClientEvent(sender, "create_marker", pos.X, pos.Y, pos.Z - 1);
                                API.sendChatMessageToPlayer(sender, "~y~Teslim noktasına gidip ~s~ (( /bankacı yukbosalt )) ~y~komutunu kullanınız.");
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
                if (!sender.isInVehicle) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Aracınızı da getirmelisiniz."); return; }
                if (API.hasEntityData(sender.vehicle, JOB_VEHICLE) && API.getEntityData(sender.vehicle, JOB_VEHICLE) != API.getEntityData(sender, "ID")) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bu teslimata başladığınız araç değil."); return; }
                if (API.hasEntityData(sender, JOB_ON))
                {
                    int bankIndex = API.getEntityData(sender, JOB_ON);
                    if (Vector3.Distance(sender.position, db_Banks.CurrentBanks.Item1[bankIndex].Position) < 4)
                    {
                        try
                        {
                            if (db_Banks.CurrentBanks.Item1[bankIndex].MoneyCountInInside < AverageValue)
                            {
                                var _bank = db_Banks.CurrentBanks.Item1.FirstOrDefault(x => x.TypeOfBank == Models.BankType.Bank && x.MoneyCountInInside > 5000);
                                if (_bank!=null)
                                {
                                    _bank.MoneyCountInInside -= 5000;
                                }
                                db_Banks.CurrentBanks.Item1[bankIndex].MoneyCountInInside += 5000;
                            }
                            else
                            {
                                db_Banks.CurrentBanks.Item1[bankIndex].MoneyCountInInside -= ((db_Banks.CurrentBanks.Item1[bankIndex].MoneyCountInInside - AverageValue) - 1000);
                                db_Banks.CurrentBanks.Item1.FirstOrDefault(x => x.TypeOfBank == Models.BankType.Bank).MoneyCountInInside += ((db_Banks.CurrentBanks.Item1[bankIndex].MoneyCountInInside - AverageValue) - 1000);
                            }
                            db_Banks.Update(db_Banks.CurrentBanks.Item1[bankIndex]);
                            API.setEntityData(sender, JOB_MONEY, (API.hasEntityData(sender, JOB_MONEY) ? API.getEntityData(sender, JOB_MONEY) : 0) + JOB_MONEY_PER_ATM);
                            API.sendChatMessageToPlayer(sender, "Alacak olarak ~g~" + JOB_MONEY_PER_ATM + "$ ~s~hesabınıza eklendi.");
                            API.triggerClientEvent(sender, "remove_marker");
                            JobManager.PlayerJobComplete(sender, 5);
                            if (db_Banks.CurrentBanks.Item1.FirstOrDefault(x => x.TypeOfBank == Models.BankType.Bank && x.MoneyCountInInside >= 5000) != null)
                            {
                                if (bankIndex + 1 >= db_Banks.CurrentBanks.Item1.Count)
                                {
                                    var pos = db_Jobs.currentJobsList.FirstOrDefault(x => x.Item1.JobId == 5).Item1.Position;
                                    API.setEntityData(sender, JOB_ON, -2);
                                    API.triggerClientEvent(sender, "create_marker", pos.X, pos.Y, pos.Z - 1);
                                    API.sendChatMessageToPlayer(sender, "~y~Paranızı işi aldığınız yere gidip ~s~(( /bankacı bitir )) ~y~yazdıktan sonra alacaksınız.");
                                }
                                else
                                {
                                    var pos = db_Banks.CurrentBanks.Item1[bankIndex + 1].Position;
                                    API.setEntityData(sender, JOB_ON, bankIndex + 1);
                                    API.triggerClientEvent(sender, "create_marker", pos.X, pos.Y, pos.Z - 1);
                                }
                            }
                            else
                            {
                                API.sendChatMessageToPlayer(sender, "~y~Banka daha fazla nakit transferi yapmak istemiyor. Geri dönüp maaşınızı alabilirsiniz.\n~y~ Yükü aldığınzı yere gidip ~s~((/bankacı paramial)) ~y~yazabilirsiniz.");
                            }
                        }
                        catch (Exception ex)
                        {
                            API.consoleOutput(LogCat.Fatal, ex.ToString());
                            API.sendChatMessageToPlayer(sender, "~y~Banka daha fazla nakit transferi yapmak istemiyor. Geri dönüp maaşınızı alabilirsiniz.\n~y~ Yükü aldığınzı yere gidip ~s~((/bankacı paramial)) ~y~yazabilirsiniz.");
                        }
                    }
                }
                #endregion
            }
            else
            if ("bitir".StartsWith(type.ToLower()))
            {
                #region bitir
                if (API.hasEntityData(sender, JOB_ON))
                {
                    if (Vector3.Distance(sender.position, db_Jobs.currentJobsList.FirstOrDefault(x => x.Item1.JobId == 5).Item1.TakingPosition) < 5)
                    {
                        if (API.getEntityData(sender.vehicle, JOB_VEHICLE) == API.getEntityData(sender, "ID"))
                        {
                            API.sendChatMessageToPlayer(sender, "~y~Paranizi almak için ~s~(( /bankacı paramial ))");
                            API.triggerClientEvent(sender, "remove_marker");
                            API.resetEntityData(sender, JOB_ON);
                        }
                        else
                        {
                            API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bu mesleğe başladığınız araç değil!");
                        }
                    }
                    else
                    {
                        API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Bitirebilmek için yükü aldığınız konuma gitmeniz gerekmekte.");
                        API.triggerClientEvent(sender, "remove_marker");
                        var pos = db_Jobs.currentJobsList.FirstOrDefault(x => x.Item1.JobId == 5).Item1.TakingPosition;
                        API.triggerClientEvent(sender, "create_marker", pos.X, pos.Y, pos.Z - 1);
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
                        if (item.Item1.JobId == 5 && item.Item1.Dimension == sender.dimension && Vector3.Distance(sender.position, item.Item1.Position) < item.Item1.Range)
                        {
                            if (API.hasEntityData(sender, JOB_MONEY))
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
                            else
                            {
                                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Birikmiş alacak paranız bulunmuyor.");
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
    }
}
