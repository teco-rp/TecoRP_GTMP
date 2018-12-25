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
using TecoRP.Models;
using GrandTheftMultiplayer.Server.Constant;

namespace TecoRP.Managers
{
    public class BankManager : Script
    {
        public BankManager()
        {
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        private void API_onClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            try
            {
                switch (eventName)
                {
                    case "bank_withdraw":
                        BankWithdraw(sender, Convert.ToInt32(arguments[0]));
                        break;
                    case "bank_deposit":
                        BankDeposit(sender, Convert.ToInt32(arguments[0]));
                        break;
                    case "bank_createaccount":
                        CreateBankAccount(sender);
                        break;
                    case "bank_transfer":
                        TransferMoney(sender, arguments[0].ToString(), arguments[1].ToString());
                        break;
                    case "return_bank":
                        PhoneManager pMgr = new PhoneManager();
                        if (arguments[0].ToString() == "KONTOR YÜKLE")
                        {
                            try { Convert.ToInt32(arguments[2]); Convert.ToInt32(arguments[1]); } catch (Exception) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Parametreler sayı olmalıydı."); return; }
                            pMgr.BuyPhoneCredits(sender, Convert.ToInt32(arguments[2]), Convert.ToInt32(arguments[1]));
                        }
                        else
                        if (arguments[0].ToString() == "INTERNET YÜKLE")
                        {
                            try { Convert.ToInt32(arguments[2]); Convert.ToInt32(arguments[1]); } catch (Exception) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Parametreler sayı olmalıydı."); return; }
                            pMgr.BuyInternetCreditsForPhone(sender, Convert.ToInt32(arguments[2]), Convert.ToInt32(arguments[1]));
                        }
                        break;
                    default:
                        break;
                }
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(FormatException))
                {
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Parametre hatalı girildi.");
                }
                else API.shared.consoleOutput(LogCat.Warn, ex.ToString());
            }
        }

        [Command("banka", Alias = "atm")]
        public void OnBankOrATM(Client sender)
        {
            foreach (var itemBank in db_Banks.CurrentBanks.Item1)
            {
                if (Vector3.Distance(sender.position, itemBank.Position) < 2)
                {
                    if (String.IsNullOrEmpty(API.getEntityData(sender, "BankAccount")))
                    {
                        if (itemBank.TypeOfBank == Models.BankType.Bank)
                        {
                            string[] names = new string[] { "HESAP AÇ" };
                            string[] descriptions = new string[] { "Bir banka hesabınız olmadan bu işlemleri yapamazsınız." };
                            API.triggerClientEvent(sender, "bank_open", names.Count(), names, descriptions, (itemBank.TypeOfBank == Models.BankType.Bank ? "BANKA" : "ATM"), "Hesabınız yok!", null);
                        }
                        else
                            API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~ATM'leri kullanabilmek için en yakın bankaya gidip bir hesap açabilirsiniz.");
                    }
                    else
                    {
                        string[] names = new string[] { "PARA ÇEK", "PARA YATIR", "HAVALE YAP", "KONTOR YÜKLE", "INTERNET YÜKLE", "VERGİ SORGULA", "VERGİ ÖDE" };
                        string[] descriptions = new string[] {
                            "Banka hesabınızdan para çekebilirsiniz. Yalnız faizinizin bozulabileceğini unutmayın.",
                            "Banka hesabınızın üzerine para yatırın. Faiziniz bozulmaz. Yalnız önceki para üzerinden faiz alırsınız.",
                            "Başka bir hesaba para aktarımı yapabilirsiniz.",
                            "Telefonunuza konuşma puanı yükleyebilirsiniz.",
                            "Telefonunuza internet hakkı yükleyebilirsiniz.",
                            "Aracınızın ne kadar vergi borcu olduğunu plakasından sorgulayabilirsiniz.",
                            "Aracınızın vergi borcunu ödeyebilirsiniz."

                        };
                        API.triggerClientEvent(sender, "bank_open", names.Count(), names, descriptions, (itemBank.TypeOfBank == Models.BankType.Bank ? "BANKA" : "ATM"), "Hesap No: " + API.getEntityData(sender, "BankAccount") + " | Bakiye: " + API.getEntityData(sender, "BankMoney"), null);
                    }
                    return;
                }
            }
        }
        public void PhoneBank(Client sender, int model_phone_id)
        {
            var _inventory = (Inventory)API.getEntityData(sender, "inventory");

            var _phone = API.fromJson(_inventory.ItemList.FirstOrDefault(x => x.ItemId == model_phone_id).SpecifiedValue).ToObject<SpecifiedValuePhone>() as SpecifiedValuePhone;
            if (_phone.FlightMode) { API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Bu uygulamayı uçak modunda kullanamazsınız."); return; }
            if (_phone.InternetBalance < 2) { API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Bu işlem için yeterli internetiniz bulunmuyor."); return; }

            _phone.InternetBalance -= 2;
            _inventory.ItemList.FirstOrDefault(x => x.ItemId == model_phone_id).SpecifiedValue = API.toJson(_phone);
            API.setEntityData(sender, "inventory", _inventory);
            if (String.IsNullOrEmpty(API.getEntityData(sender, "BankAccount")))
            {
                API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Bu uygulamayı kullanabilmek için bankaya gidip bir banka hesabı açmalısınız.");
            }
            else
            {
                string[] names = new string[] { "HAVALE YAP", "KONTOR YÜKLE", "INTERNET YÜKLE", "VERGİ SORGULA", "VERGİ ÖDE" };
                string[] descriptions = new string[] {
                    "Başka bir hesaba para aktarımı yapabilirsiniz.",
                    "Telefonunuza konuşma puanı yükleyebilirsiniz.",
                    "Telefonunuza internet hakkı yükleyebilirsiniz.",
                    "Aracınızın ne kadar vergi borcu olduğunu plakasından sorgulayabilirsiniz.",
                    "Aracınızın vergi borcunu ödeyebilirsiniz."
                };
                API.triggerClientEvent(sender, "bank_open", names.Count(), names, descriptions, "Bankacılık Uygulaması", "Hesap No: " + API.getEntityData(sender, "BankAccount") + " | Bakiye: " + API.getEntityData(sender, "BankMoney"), model_phone_id);
                //API.triggerClientEvent(sender, "bank_open", names.Count(), names, descriptions, "Bankacılık Uygulaması");
            }
        }
        public void BankWithdraw(Client sender, int value)
        {
            int money = API.getEntityData(sender, "Money");
            int bankMoney = API.getEntityData(sender, "BankMoney");
            int _Index = 0;
            if (bankMoney >= value)
            {
                foreach (var itemBank in db_Banks.CurrentBanks.Item1)
                {
                    if (Vector3.Distance(sender.position, itemBank.Position) < 3)
                    {
                        if (itemBank.MoneyCountInInside >= value)
                        {
                            money += value;
                            bankMoney -= value;
                            API.setEntityData(sender, "Money", money);
                            API.setEntityData(sender, "BankMoney", bankMoney);
                            API.triggerClientEvent(sender, "update_money_display", money);
                            API.sendNotificationToPlayer(sender, "~g~+" + value + "$");
                            API.sendNotificationToPlayer(sender, "Banka Hesabı:\n ~r~-" + value + "$");
                            itemBank.MoneyCountInInside -= value;
                            db_Banks.Update(itemBank);
                            API.sendNotificationToPlayer(sender, "~g~İşlem başarıyla tamamlandı.");
                            return;
                        }
                        else
                        {
                            API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~ATM'de şu anda yeterli miktar bulunmuyor. Stokta: ~g~" + itemBank.MoneyCountInInside + "$");
                        }
                        break;
                    }
                    _Index++;
                }
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Banka hesabınızda yeterli bakiyeniz bulunmuyor.");
            }
        }

        public void BankDeposit(Client sender, int value)
        {
            int money = API.getEntityData(sender, "Money");
            int bankMoney = API.getEntityData(sender, "BankMoney");
            if (money >= value)
            {
                foreach (var itemBank in db_Banks.CurrentBanks.Item1)
                {
                    if (Vector3.Distance(sender.position, itemBank.Position) < 3)
                    {
                        money -= value;
                        bankMoney += value;
                        API.setEntityData(sender, "Money", money);
                        API.setEntityData(sender, "BankMoney", bankMoney);
                        API.triggerClientEvent(sender, "update_money_display", money);
                        API.sendNotificationToPlayer(sender, "~r~-" + value + "$");
                        API.sendNotificationToPlayer(sender, "Banka Hesabı:\n ~g~+" + value + "$");
                        API.sendNotificationToPlayer(sender, "~g~İşlem başarıyla tamamlandı.");
                        itemBank.MoneyCountInInside += value;
                        db_Banks.Update(itemBank);
                        return;
                    }
                }
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Üzerinizde yeterli nakit bulunmuyor.");
            }
        }

        public void CreateBankAccount(Client sender)
        {
            string account = "LS-";
            Random r = new Random();
            List<char> dateTimeFormat = new List<char> { 'M', 'M', 'd', 'm', 'H', 'd', 'H', 'm', 's', 's' };
            string generatedDateFormat = "";
            for (int i = 0; i < 10; i++)
            {
                int index = r.Next(0, dateTimeFormat.Count);
                generatedDateFormat += dateTimeFormat[index].ToString();
                dateTimeFormat.RemoveAt(index);
            }
            string generatedPhoneNumber = DateTime.Now.ToString(generatedDateFormat);
            account += generatedPhoneNumber.Substring(0, 6);
            API.setEntityData(sender, "BankAccount", account);
            API.sendChatMessageToPlayer(sender, "~g~Hesabınzı başarıyla açıldı. Hesap numaranız: " + account);
        }

        public void TransferMoney(Client sender, string bankAccount, string value)
        {
            int bankMoney = API.getEntityData(sender, "BankMoney");
            int valueToTranfser = 0;
            try { valueToTranfser = Convert.ToInt32(value); } catch (Exception) { API.sendChatMessageToPlayer(sender, "~r~HAT:A ~s~Gönderilecek parayı sayı olarak giriniz."); return; }
            if (bankMoney >= Convert.ToInt32(value))
            {

                foreach (var itemPlayer in db_Players.GetOfflineUserDatas())
                {
                    if (itemPlayer.BankAccount == bankAccount)
                    {
                        var clientPlayer = db_Players.IsPlayerOnline(itemPlayer.CharacterId);
                        if (clientPlayer != null)
                        {
                            int takerBankMoney = API.getEntityData(clientPlayer, "BankMoney");
                            bankMoney -= valueToTranfser;
                            takerBankMoney += valueToTranfser;
                            API.setEntityData(clientPlayer, "BankMoney", takerBankMoney);
                            API.sendNotificationToPlayer(clientPlayer, "Banka Hesabı:\n~g~+" + valueToTranfser);
                        }
                        else
                        {
                            itemPlayer.BankMoney += valueToTranfser;
                            bankMoney -= valueToTranfser;
                        }
                        db_Players.SaveOfflineUserData(itemPlayer.CharacterId, itemPlayer);
                        API.sendNotificationToPlayer(sender, "Banka Hesabı:\n~r~-" + valueToTranfser);
                        API.setEntityData(sender, "BankMoney", bankMoney);

                    }
                }


                foreach (var itemPlayer in API.getAllPlayers())
                {
                    if (API.getEntityData(itemPlayer, "BankAccount") == bankAccount)
                    {
                        int takerBankMoney = API.getEntityData(itemPlayer, "BankMoney");
                        bankMoney -= valueToTranfser;
                        takerBankMoney += valueToTranfser;
                        API.setEntityData(sender, "BankMoney", bankMoney);
                        API.setEntityData(itemPlayer, "BankMoney", takerBankMoney);
                        API.sendNotificationToPlayer(sender, "Banka Hesabı:\n~r~-" + valueToTranfser);
                        API.sendNotificationToPlayer(itemPlayer, "Banka Hesabı:\n~g~+" + valueToTranfser);
                        return;
                    }
                }
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Banka hesabı aktif değil. ((Oyuncunun oyunda olması gerekiyor.))");
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Banka hesabınızda yeterli miktar bulunmuyor.");
            }

        }
    }
}
