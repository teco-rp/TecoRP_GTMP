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
using TecoRP.Clients;
using TecoRP.Database;
using TecoRP.Helpers;
using TecoRP.Models;
using TecoRP.Users;

namespace TecoRP.Managers
{

    public class PhoneManager : Script
    {
        #region Flags
        [Flags]
        public enum AnimationFlags
        {
            Loop = 1 << 0,
            StopOnLastFrame = 1 << 1,
            OnlyAnimateUpperBody = 1 << 4,
            AllowPlayerControl = 1 << 5,
            Cancellable = 1 << 7
        }
        #endregion
        public PhoneManager()
        {
            API.onClientEventTrigger += API_onClientEventTrigger;
            API.onClientEventTrigger += (s, e, args) => this.GetType().GetMethod(e)?.Invoke(this, parameters: new object[] { s, args });
        }

        private void API_onClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            if (eventName == "return_phone_call")
            {
                API.playPlayerAnimation(sender, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "cellphone@str", "f_cellphone_call_listen_maybe_a");
                //try { Convert.ToInt32(arguments[0]); } catch (Exception) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Telefon numarası yalnızca sayılardan oluşabilir."); return; }
                CallWithPhone(sender, arguments[0].ToString(), Convert.ToInt32(arguments[1]));
            }
            else
            if (eventName == "return_phone_sms")
            {
                //API.playPlayerAnimation(sender, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "cellphone@", "f_cellphone_text_in");
                // try { Convert.ToInt32(arguments[0]); } catch (Exception) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Telefon numarası yalnızca sayılardan oluşabilir."); return; }
                SMSonPhone(sender, arguments[0].ToString(), arguments[1].ToString(), Convert.ToInt32(arguments[2]));
                Animation.AnimationStop(sender);
            }
            else
            if (eventName == "return_phone")
            {
                #region return_phone_switch
                switch (arguments[0].ToString().ToLower())
                {
                    case "rehber":
                        OnContacts(sender, Convert.ToInt32(arguments[1]));
                        break;
                    case "uçak modu":
                        OnFlightMode(sender, Convert.ToInt32(arguments[1]));
                        break;
                    case "*uçak modu":
                        OnFlightMode(sender, Convert.ToInt32(arguments[1]));
                        break;
                    case "gps":
                        OnGpsSelected(sender, Convert.ToInt32(arguments[1]));
                        break;
                    case "uygulama mağazası":
                        OnDownloadappStore(sender, Convert.ToInt32(arguments[1]));
                        break;
                    case "bankacılık":
                        BankManager bankMGr = new BankManager();
                        bankMGr.PhoneBank(sender, Convert.ToInt32(arguments[1]));
                        break;
                    case "emlakçılık":
                        OnEstateApp(sender, Convert.ToInt32(arguments[1]));
                        break;
                    default:
                        break;
                }
                #endregion
            }
            else
            if (eventName == "phone_contacts_add")
            {
                ContactAdd(sender, arguments[0].ToString(), arguments[1].ToString());
                if (API.hasEntityData(sender, "Cuffed") && (bool)API.getEntityData(sender, "Cuffed")) return;
                Animation.AnimationStop(sender);
            }
            else
            if (eventName == "phone_gps_point")
            {
                OnGpsPointRequested(sender, arguments[0].ToString(), Convert.ToInt32(arguments[1]));
                if (API.hasEntityData(sender, "Cuffed") && (bool)API.getEntityData(sender, "Cuffed")) return;
                Animation.AnimationStop(sender);
            }
            else
            if (eventName == "return_phone_download")
            {
                OnClickDownloadApp(sender, arguments[0].ToString(), Convert.ToInt32(arguments[1]));
                if (API.hasEntityData(sender, "Cuffed") && (bool)API.getEntityData(sender, "Cuffed")) return;
                Animation.AnimationStop(sender);
            }
            else
            if (eventName == "return_phone_radio_frequence")
            {
                try { Convert.ToInt32(arguments[0]); } catch (Exception) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Radyo frekansı bir sayı olmalı."); return; }
                OnRadioFrequence(sender, arguments[0].ToString());
                Animation.AnimationStop(sender);
            }

            if (eventName == "return_emlakci_selected")
            {
                API.consoleOutput("return_emlakci_selected");
                #region return_emlakci_selected Switch
                switch (arguments[0].ToString())
                {
                    case "En Yakın Satılık Evler":
                        OnEstateAppNearest(sender);
                        break;
                    case "En Ucuz Satılık Evler":
                        OnEstateApp(sender, Convert.ToInt32(arguments[1]));
                        break;
                    case "En Pahalı Satılık Evler":
                        OnEstateAppFarthest(sender);
                        break;
                    default:
                        int _Id = Convert.ToInt32(arguments[0].ToString().Split('(')[1].Split(')')[0]);
                        OnEstateHouseSelected(sender, _Id);
                        break;
                }
                #endregion
            }

        }

        public static List<int> PhoneIdList = db_Items.GameItems.Values.Where(x => x.Type == ItemType.Phone).Select(s => s.ID).ToList();
        RPGManager rpgMGr = new RPGManager();

        [Command("ara", "/ara [numara]")]
        public void CallWithPhone(Client sender, string number)
        {

            if (API.hasEntityData(sender, "ringing") || API.hasEntityData(sender, "OnCall"))
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Başka bir görüşmedeyken bunu yapamazsınız. ~y~(( /h ))");
                return;
            }
            var _inventory = (Inventory)API.getEntityData(sender, "inventory");
            var _phone = (SpecifiedValuePhone)API.fromJson(_inventory.ItemList.FirstOrDefault(x => PhoneIdList.Contains(x.ItemId) && (API.fromJson(x.SpecifiedValue).ToObject<SpecifiedValuePhone>() as SpecifiedValuePhone).FlightMode == false).SpecifiedValue).ToObject<SpecifiedValuePhone>();
            if (_phone != null)
            {
                API.consoleOutput("number: " + number);
                if (number == "123")
                {
                    API.sendChatMessageToPlayer(sender, "~g~Hattınızda : " + _phone.InternetBalance + " mb internet " + _phone.Balance + " puan konuşma hakkı bulunmaktadır.");
                    return;
                }
                else
                if (number == "100")
                {
                    API.sendChatMessageToPlayer(sender, "~y~[TAKSİ]:~s~ Taksi hattı buyrun.");
                    API.setEntityData(sender, "100", true);
                    return;
                }
                if (number == "911")
                {
                    API.sendChatMessageToPlayer(sender, "~g~[911] Bağlanmak istediğiniz departmanı belirleyin. ((LSMD / LSPD))");
                    API.shared.setEntityData(sender, "911", true);
                    return;
                }
                if (_phone.Balance > 0)
                {
                    foreach (var itemPlayer in API.getAllPlayers())
                    {
                        if (API.hasEntityData(itemPlayer, "PhoneNumbers") && (API.getEntityData(itemPlayer, "PhoneNumbers") as List<string>).Contains(number))
                        {
                            switch (_phone.PhoneOperator)
                            {
                                case Operator.Vodacell:
                                    _phone.Balance -= 2;
                                    break;
                                case Operator.LosTelecom:
                                    _phone.Balance--;
                                    break;
                                default:
                                    break;
                            }
                            string[] ringing = new string[] { _phone.PhoneNumber.ToString(), API.getEntityData(sender, "ID").ToString() };
                            API.setEntityData(itemPlayer, "ringing", ringing);
                            API.setEntityData(sender, "calling", API.getEntityData(itemPlayer, "ID"));
                            API.sendChatMessageToPlayer(itemPlayer, "~y~~h~ Telefonunuz çalıyor. " + _phone.PhoneNumber + " numara sizi arıyor. Açmak için (( /p ))");
                            API.sendChatMessageToPlayer(sender, "~y~~h~" + number + " telefon numarası aranıyor.");
                            rpgMGr.Me(sender, " telefonunu çıkarır, bir numara arar ve kulağına dayar.");
                            rpgMGr.Me(itemPlayer, " adlı kişinin telefonu çalmaya başlar.");

                            _inventory.ItemList.FirstOrDefault(x => PhoneIdList.Contains(x.ItemId)).SpecifiedValue = API.toJson(_phone);
                            API.setEntityData(sender, "inventory", _inventory);
                            return;
                        }
                    }
                    API.sendChatMessageToPlayer(sender, "~y~Aradığınız kişiye şu anda ulaşılamıyor. Lütfen daha sonra tekrar deneyiniz.");
                }
                else
                {
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Telefonunuzda yeterli bakiye yok.");
                }
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Telefonunuz yok.");
            }
        }

        public void CallWithPhone(Client sender, string number, int model_phone_id)
        {
            if (API.hasEntityData(sender, "ringing") || API.hasEntityData(sender, "OnCall"))
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Başka bir görüşmedeyken bunu yapamazsınız. ~y~(( /h ))");
                return;
            }
            var _inventory = (Inventory)API.getEntityData(sender, "inventory");
            var _phone = (SpecifiedValuePhone)API.fromJson(_inventory.ItemList.FirstOrDefault(x => PhoneIdList.Contains(x.ItemId) && x.ItemId == model_phone_id && (API.fromJson(x.SpecifiedValue).ToObject<SpecifiedValuePhone>() as SpecifiedValuePhone).FlightMode == false).SpecifiedValue).ToObject<SpecifiedValuePhone>();
            if (_phone != null)
            {
                if (number == "123")
                {
                    API.sendChatMessageToPlayer(sender, "~g~Hattınızda : " + _phone.InternetBalance + " mb internet " + _phone.Balance + " puan konuşma hakkı bulunmaktadır.");
                    Animation.AnimationStop(sender);
                    return;
                }
                if (number == "100")
                {
                    API.sendChatMessageToPlayer(sender, "~y~[TAKSİ]:~s~ Taksi hattı buyrun.");
                    API.setEntityData(sender, "100", true);
                    Animation.AnimationStop(sender);
                    return;
                }
                if (number == "911")
                {
                    API.sendChatMessageToPlayer(sender, "~g~[911] Bağlanmak istediğiniz departmanı belirleyin. ((LSMD / LSPD))");
                    API.shared.setEntityData(sender, "911", true);
                    Animation.AnimationStop(sender);
                    return;
                }
                if (_phone.Balance > 0)
                {
                    foreach (var itemPlayer in API.getAllPlayers())
                    {
                        if (API.hasEntityData(itemPlayer, "PhoneNumbers") && (API.getEntityData(itemPlayer, "PhoneNumbers") as List<string>).Contains(number))
                        {
                            switch (_phone.PhoneOperator)
                            {
                                case Operator.Vodacell:
                                    _phone.Balance -= 2;
                                    break;
                                case Operator.LosTelecom:
                                    _phone.Balance--;
                                    break;
                                default:
                                    break;
                            }
                            string[] ringing = new string[] { _phone.PhoneNumber.ToString(), API.getEntityData(sender, "ID").ToString() };
                            API.setEntityData(itemPlayer, "ringing", ringing);
                            API.setEntityData(sender, "calling", API.getEntityData(itemPlayer, "ID"));
                            API.sendChatMessageToPlayer(itemPlayer, "~y~~h~ Telefonunuz çalıyor. " + _phone.PhoneNumber + " numara sizi arıyor. Açmak için (( /p ))");
                            API.sendChatMessageToPlayer(sender, "~y~~h~" + number + " telefon numarası aranıyor.");
                            rpgMGr.Me(sender, " telefonunu çıkarır, bir numara arar ve kulağına dayar.");
                            rpgMGr.Me(itemPlayer, " adlı kişinin telefonu çalmaya başlar.");

                            _inventory.ItemList.FirstOrDefault(x => PhoneIdList.Contains(x.ItemId)).SpecifiedValue = API.toJson(_phone);
                            API.setEntityData(sender, "inventory", _inventory);
                            return;
                        }
                    }
                    API.sendChatMessageToPlayer(sender, "~y~Aradığınız kişiye şu anda ulaşılamıyor. Lütfen daha sonra tekrar deneyiniz.");
                }
                else
                {
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Telefonunuzda yeterli bakiye yok.");
                }
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Telefonunuz yok.");
            }
        }
        [Command("p")]
        public void AnswerPhone(Client sender)
        {
            if (API.hasEntityData(sender, "ringing"))
            {
                string[] ringing = API.getEntityData(sender, "ringing");
                var _player = db_Accounts.GetPlayerById(Convert.ToInt32(ringing[1]));
                API.resetEntityData(sender, "ringing");
                API.setEntityData(_player, "OnCall", API.getEntityData(sender, "ID"));
                API.setEntityData(sender, "OnCall", Convert.ToInt32(ringing[1]));
                rpgMGr.Me(sender, " telefonunu eline alır, çağrıyı cevaplar.");
                API.sendChatMessageToPlayer(_player, "~y~Karşı taraf telefonu yanıtladı.");
                API.playPlayerAnimation(sender, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "cellphone@str", "f_cellphone_call_listen_maybe_a");

            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Telefonunuz çalmıyor.");
            }
        }

        [Command("h")]
        public void HangupPhone(Client sender)
        {
            if (API.hasEntityData(sender, "ringing"))
            {
                string[] ringing = API.getEntityData(sender, "ringing");
                try
                {
                    var _player = db_Accounts.GetPlayerById(Convert.ToInt32(ringing[1]));
                    API.resetEntityData(sender, "ringing");
                    API.resetEntityData(_player, "ringing");
                    API.resetEntityData(sender, "calling");
                    API.resetEntityData(_player, "calling");
                    API.sendChatMessageToPlayer(sender, "~y~Çağrıyı reddettiniz. ~r~*");
                    API.sendChatMessageToPlayer(_player, "~y~Karşı taraf çağrınızı reddetti. ~r~*");
                    API.stopPlayerAnimation(sender);
                    API.stopPlayerAnimation(_player);
                }
                catch (Exception ex)
                {
                    API.consoleOutput(LogCat.Warn, ex.ToString());
                }
                return;
            }
            else
            if (API.hasEntityData(sender, "calling"))
            {
                int calling = API.getEntityData(sender, "calling");
                var _player = db_Accounts.GetPlayerById(Convert.ToInt32(calling));
                API.resetEntityData(sender, "ringing");
                API.resetEntityData(_player, "ringing");
                API.resetEntityData(sender, "calling");
                API.resetEntityData(_player, "calling");
                API.sendChatMessageToPlayer(sender, "~y~Aramayı iptal ettiniz. ~r~*");
                API.sendChatMessageToPlayer(_player, "~y~Karşı tarafçağrıyı sonlandırdı. ~r~*");
                if (API.hasEntityData(sender, "Cuffed") && (bool)API.getEntityData(sender, "Cuffed")) return;
                API.stopPlayerAnimation(sender);
                return;
            }
            else
            if (API.hasEntityData(sender, "OnCall"))
            {
                var _player = db_Accounts.GetPlayerById(API.getEntityData(sender, "OnCall"));
                API.resetEntityData(sender, "ringing");
                API.resetEntityData(_player, "ringing");
                API.resetEntityData(sender, "calling");
                API.resetEntityData(_player, "calling");
                API.resetEntityData(sender, "OnCall");
                API.resetEntityData(_player, "OnCall");
                API.sendChatMessageToPlayer(sender, "~y~Çağrıyı sonlandırdınız. ~r~*");
                API.sendChatMessageToPlayer(_player, "~y~Çağrıyı karşı taraf sonlandırdı. ~r~*");

                if (API.hasEntityData(sender, "Cuffed") && (bool)API.getEntityData(sender, "Cuffed")) return;
                API.stopPlayerAnimation(sender);
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Telefonla görüşmüyorsunuz.");
            }
        }

        [Command("sms", "/sms [Telefon Numarası] [Yazı]", GreedyArg = true)]
        public void SMSonPhone(Client sender, string phoneNumber, string text)
        {
            var _inventory = (Inventory)API.getEntityData(sender, "inventory");
            var _phone = (SpecifiedValuePhone)API.fromJson(_inventory.ItemList.FirstOrDefault(x => PhoneIdList.Contains(x.ItemId) && (API.fromJson(x.SpecifiedValue).ToObject<SpecifiedValuePhone>() as SpecifiedValuePhone).FlightMode == false).SpecifiedValue).ToObject<SpecifiedValuePhone>();
            if (_phone != null)
            {
                if (_phone.Balance > 0)
                {
                    foreach (var itemPlayer in API.getAllPlayers())
                    {
                        if ((API.getEntityData(itemPlayer, "PhoneNumbers") as List<string>).Contains(phoneNumber))
                        {
                            switch (_phone.PhoneOperator)
                            {
                                case Operator.Vodacell:
                                    _phone.Balance -= 2;
                                    break;
                                case Operator.LosTelecom:
                                    _phone.Balance--;
                                    break;
                                default:
                                    break;
                            }
                            API.sendChatMessageToPlayer(itemPlayer, "~y~" + API.getEntityData(sender, "CharacterName") + ": [SMS(" + _phone.PhoneNumber + ")] " + text);
                            API.sendChatMessageToPlayer(sender, "~y~Mesajınız ~g~gönderildi.");
                            _inventory.ItemList.FirstOrDefault(x => PhoneIdList.Contains(x.ItemId)).SpecifiedValue = API.toJson(_phone);
                            API.setEntityData(sender, "inventory", _inventory);
                            return;
                        }
                    }
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bu numaraya erişilemiyor.");
                }
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Telefonunuz yok.");
            }
        }
        public void SMSonPhone(Client sender, string phoneNumber, string text, int model_phone_id)
        {
            var _inventory = (Inventory)API.getEntityData(sender, "inventory");
            var _phone = (SpecifiedValuePhone)API.fromJson(_inventory.ItemList.FirstOrDefault(x => PhoneIdList.Contains(x.ItemId) && x.ItemId == model_phone_id && (API.fromJson(x.SpecifiedValue).ToObject<SpecifiedValuePhone>() as SpecifiedValuePhone).FlightMode == false).SpecifiedValue).ToObject<SpecifiedValuePhone>();
            if (_phone != null)
            {
                if (_phone.Balance > 0)
                {
                    foreach (var itemPlayer in API.getAllPlayers())
                    {
                        if ((API.getEntityData(itemPlayer, "PhoneNumbers") as List<string>).Contains(phoneNumber))
                        {
                            switch (_phone.PhoneOperator)
                            {
                                case Operator.Vodacell:
                                    _phone.Balance -= 2;
                                    break;
                                case Operator.LosTelecom:
                                    _phone.Balance--;
                                    break;
                                default:
                                    break;
                            }
                            API.sendChatMessageToPlayer(itemPlayer, "~y~" + API.getEntityData(sender, "CharacterName") + ": [SMS(" + _phone.PhoneNumber + ")] " + text);
                            API.sendChatMessageToPlayer(sender, "~y~Mesajınız ~g~gönderildi.");
                            _inventory.ItemList.FirstOrDefault(x => PhoneIdList.Contains(x.ItemId)).SpecifiedValue = API.toJson(_phone);
                            API.setEntityData(sender, "inventory", _inventory);
                            return;
                        }
                    }
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bu numaraya erişilemiyor.");
                }
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Telefonunuz yok.");
            }
        }
        [Command("hat", "/hat")]
        public void OnGSMOperator(Client sender)
        {
            foreach (var itemOperatorShop in db_PhoneOperatorShop.CurrentOperatorShop.Item1)
            {
                if (Vector3.Distance(itemOperatorShop.Position, sender.position) < 2)
                {
                    string pricing = null;

                    switch (itemOperatorShop.OperatorType)
                    {
                        case Operator.Vodacell:
                            pricing = "~g~Yeni hat: 50$ | Konuşma Puanı : 2 / 1$  | İnternet : 100mb / 10$";
                            break;
                        case Operator.LosTelecom:
                            pricing = "~g~Yeni hat: 65$ | Konuşma Puanı : 5 / 1$  | İnternet : 100mb / 15$";
                            break;
                        default:
                            break;
                    }
                    ClientManager.OpenOperatorMenu(sender, itemOperatorShop.OperatorType.ToString(), pricing);
                    return;
                }
            }
            API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Herhangi bir operatör satış noktasında değilsiniz.");
        }

        public void BuySimCard(Client sender, params object[] args)
        {
            API.shared.consoleOutput("Buy SimCard Triggered on Server-Side with arguments: " + string.Join(" | ",args));
            Operator operatorType = (Operator)Enum.Parse(typeof(Operator), args[0].ToString());

            int money = API.getEntityData(sender, "Money");
            if (money < (operatorType == Operator.Vodacell ? 50 : 65))
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bu hattı alabilmek için yeterli paranız bulunmuyor.");
                return;
            }
            var _inventory = (Inventory)API.getEntityData(sender, "inventory");
            if (_inventory.ItemList.Where(x => PhoneIdList.Contains(x.ItemId) && ((API.fromJson(x.SpecifiedValue).ToObject<SpecifiedValuePhone>() as SpecifiedValuePhone).FlightMode == true)).Count() > 1)
            {
                API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Envanterinizde birden fazla telefonunuz var.\nLütfen yalnızca hattınızı bağlamak istediğiniz telefonu uçak moduna alınız.");
            }
            else
            {
                try
                {
                    Random r = new Random();
                    List<char> dateTimeFormat = new List<char> { 'M', 'M', 'd', 'm', 'H', 'd', 'H', 'm', 's', 's' };
                    var _Index = _inventory.ItemList.IndexOf(_inventory.ItemList.FirstOrDefault(x => PhoneIdList.Contains(x.ItemId) && (API.fromJson(x.SpecifiedValue).ToObject<SpecifiedValuePhone>() as SpecifiedValuePhone).FlightMode == true));
                    if (_Index < 0) { API.sendChatMessageToPlayer(sender, "~UYARI:~ ~s~Lütfen sim kartınızı bağlayacağınız telefonu uçak moduna alınız."); return; }
                    var _phone = (SpecifiedValuePhone)API.fromJson(_inventory.ItemList[_Index].SpecifiedValue).ToObject<SpecifiedValuePhone>();
                    _phone.AutoInternetPay = false;
                    _phone.FlightMode = false;
                    string generatedDateFormat = "";
                    for (int i = 0; i < 10; i++)
                    {
                        int index = r.Next(0, dateTimeFormat.Count);
                        generatedDateFormat += dateTimeFormat[index].ToString();
                        dateTimeFormat.RemoveAt(index);
                    }
                    string generatedPhoneNumber = DateTime.Now.ToString(generatedDateFormat);
                    _phone.PhoneNumber = generatedPhoneNumber.Length > 10 ? generatedPhoneNumber.Substring(0, 10) : generatedPhoneNumber;
                    //_phone.PhoneNumber = DateTime.Now.ToString("dmssHHff");
                    _phone.PhoneOperator = operatorType;
                    _phone.InternetBalance = 150;
                    _phone.Balance = 20;
                    _inventory.ItemList[_Index].SpecifiedValue = API.toJson(_phone);
                    API.sendChatMessageToPlayer(sender, $"~g~Telefonunuza başlangıç hediyesi olarak {_phone.Balance} puan ve {_phone.InternetBalance} mb eklendi.");
                    API.sendChatMessageToPlayer(sender, $"~g~911'i arayarak acil servise,100'ü arayarak Taksi Merkezine, 123'ü arayarak kalan kullanım haklarınıza ulaşabilirsiniz.");
                    if (API.hasEntityData(sender, "PhoneNumbers")) { List<string> numbers = API.getEntityData(sender, "PhoneNumbers"); numbers.Add(_phone.PhoneNumber); }
                    API.setEntityData(sender, "PhoneNumbers", new List<string> { _phone.PhoneNumber });
                    money -= (operatorType == Operator.Vodacell ? 50 : 65);
                    API.setEntityData(sender, "inventory", _inventory);
                    API.setEntityData(sender, "Money", money);
                    API.triggerClientEvent(sender, "update_money_display", money);

                    #region AboutMission
                    int missionNumber = (API.hasEntityData(sender, "Mission") ? API.getEntityData(sender, "Mission") : 0);
                    if (missionNumber == 3)
                    {
                        Clients.ClientManager.RemoveMissionMarker(sender);
                        API.setEntityData(sender, "Mission", 4);
                        UserManager.TriggerUserMission(sender);
                    }
                    #endregion

                }
                catch (Exception ex)
                {
                    API.consoleOutput(LogCat.Warn, ex.ToString());
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~İşlem gerçekleştirilemedi.");
                }
                return;
            }
        }

        public void BuyCalling(Client sender, params object[] args)
        {
            API.shared.consoleOutput("Buy Calling Triggered on Server-Side with arguments: " + string.Join(" | ",args));
            int value = 0;
            Operator operatorType = (Operator)Enum.Parse(typeof(Operator), args[0].ToString());
            try { value = Convert.ToInt32(args[1]); } catch (Exception) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Girdiğiniz değer sayı olmalı."); return; }
            int money = API.getEntityData(sender, "Money");
            if (money >= (operatorType == Operator.Vodacell ? value / 2 : value / 5))
            {

                var _inventory = (Inventory)API.getEntityData(sender, "inventory");
                if (_inventory.ItemList.Where(x => PhoneIdList.Contains(x.ItemId) && ((API.fromJson(x.SpecifiedValue).ToObject<SpecifiedValuePhone>() as SpecifiedValuePhone).PhoneOperator == operatorType)).Count() > 1)
                {
                    API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Envanterinizde birden fazla aynı operatöre ait telefonunuz var.\nLütfen işlem yalnızca işlem yapmak istediğiniz telefonunuz açık olsun. Diğerlerini uçak moduna alın."); return;
                }
                else
                {

                }
                if (_inventory.ItemList.Where(x => PhoneIdList.Contains(x.ItemId) && ((API.fromJson(x.SpecifiedValue).ToObject<SpecifiedValuePhone>() as SpecifiedValuePhone).PhoneOperator == operatorType)).Count() > 1)
                {
                    API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Envanterinizde birden fazla aynı operatöre ait telefonunuz var.\nLütfen işlem yalnızca işlem yapmak istediğiniz telefonunuz açık olsun. Diğerlerini uçak moduna alın.");
                }
                else
                {
                    int _Index = 0;
                    foreach (var itemInvItem in _inventory.ItemList)
                    {
                        if (itemInvItem.SpecifiedValue != null && PhoneIdList.Contains(itemInvItem.ItemId))
                        {
                            var _phone = (SpecifiedValuePhone)API.fromJson(itemInvItem.SpecifiedValue).ToObject<SpecifiedValuePhone>();
                            if (_phone.PhoneOperator == operatorType && _phone.FlightMode == false)
                            {
                                _phone.Balance += value;
                                money -= (operatorType == Operator.Vodacell ? Convert.ToInt32((value / 2)) : Convert.ToInt32(value / 5));
                                API.setEntityData(sender, "Money", money);
                                API.triggerClientEvent(sender, "update_money_display", money);
                                _inventory.ItemList[_Index].SpecifiedValue = API.toJson(_phone);
                                API.setEntityData(sender, "inventory", _inventory);
                                API.sendChatMessageToPlayer(sender, "~g~Başarıyla hesabınıza " + value + " puanlık konuşma yüklendi.");
                                API.sendNotificationToPlayer(sender, "~r~-" + (operatorType == Operator.Vodacell ? Convert.ToInt32((value / 2)) : Convert.ToInt32(value / 5)) + "$", true);
                                return;
                            }
                        }
                        _Index++;
                    }
                }



            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bu kadar alabilmek için paranız yetersiz.");
            }
        }
        public void BuyInternet(Client sender, params object[] args)
        {
            API.shared.consoleOutput("Buy Internet Triggered on Server-Side with arguments: " + string.Join(" | ", args));

            int value = 0;
            Operator operatorType = (Operator)Enum.Parse(typeof(Operator), args[0].ToString());
            try { value = Convert.ToInt32(args[1]); } catch (Exception) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Girdiğiniz değer sayı olmalı."); }
            int money = API.getEntityData(sender, "Money");
            if (money >= (operatorType == Operator.Vodacell ? value * 0.10f : value * 0.15f))
            {
                var _inventory = (Inventory)API.getEntityData(sender, "inventory");
                if (_inventory.ItemList.Where(x => PhoneIdList.Contains(x.ItemId) && ((API.fromJson(x.SpecifiedValue).ToObject<SpecifiedValuePhone>() as SpecifiedValuePhone).PhoneOperator == operatorType)).Count() > 1)
                {
                    API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Envanterinizde birden fazla aynı operatöre ait telefonunuz var.\nLütfen işlem yalnızca işlem yapmak istediğiniz telefonunuz açık olsun. Diğerlerini uçak moduna alın.");
                }
                else
                {
                    int _Index = 0;
                    foreach (var itemInvItem in _inventory.ItemList)
                    {
                        if (itemInvItem.SpecifiedValue != null && PhoneIdList.Contains(itemInvItem.ItemId))
                        {
                            var _phone = (SpecifiedValuePhone)API.fromJson(itemInvItem.SpecifiedValue).ToObject<SpecifiedValuePhone>();
                            if (_phone.PhoneOperator == operatorType && _phone.FlightMode == false)
                            {
                                _phone.InternetBalance += value;
                                money -= (operatorType == Operator.Vodacell ? Convert.ToInt32((value * 0.10f)) : Convert.ToInt32(value * 0.15f));
                                API.setEntityData(sender, "Money", money);
                                API.triggerClientEvent(sender, "update_money_display", money);
                                _inventory.ItemList[_Index].SpecifiedValue = API.toJson(_phone);
                                API.setEntityData(sender, "inventory", _inventory);
                                API.sendChatMessageToPlayer(sender, "~g~Başarıyla hesabınıza " + value + "mb internet yüklendi.");
                                API.sendNotificationToPlayer(sender, "~r~-" + (operatorType == Operator.Vodacell ? Convert.ToInt32((value * 0.10f)) : Convert.ToInt32(value * 0.15f)) + "$", true);
                                return;
                            }
                        }
                        _Index++;
                    }
                }
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bu kadar alabilmek için paranız yetersiz.");
            }
        }
        public void BuyInternetCreditsForPhone(Client sender, int value, int model_phone_id)
        {
            var _inventory = (Inventory)API.getEntityData(sender, "inventory");
            var _Index = _inventory.ItemList.IndexOf(_inventory.ItemList.FirstOrDefault(x => x.ItemId == model_phone_id));
            if (_Index < 0) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Telefon bulunamadı."); return; }
            var _phone = API.fromJson(_inventory.ItemList[_Index].SpecifiedValue).ToObject<SpecifiedValuePhone>() as SpecifiedValuePhone;
            int _bankMoney = API.getEntityData(sender, "BankMoney");
            if (_bankMoney >= (_phone.PhoneOperator == Operator.Vodacell ? value * 0.10 : value * 0.15))
            {
                _bankMoney -= (int)(_phone.PhoneOperator == Operator.Vodacell ? value * 0.1 : value * 0.15);
                _phone.InternetBalance += value;

                _inventory.ItemList[_Index].SpecifiedValue = API.toJson(_phone);
                API.setEntityData(sender, "inventory", _inventory);
                API.setEntityData(sender, "BankMoney", _bankMoney);
                API.sendNotificationToPlayer(sender, "Banka Hesabı: \n~r~-" + (int)(_phone.PhoneOperator == Operator.Vodacell ? value * 0.1 : value * 0.15));
                return;
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Bankanızda yeterli bakiye bulunmuyor.");
            }
        }
        public void BuyPhoneCredits(Client sender, int value, int model_phone_id)
        {

            var _inventory = (Inventory)API.getEntityData(sender, "inventory");
            var _Index = _inventory.ItemList.IndexOf(_inventory.ItemList.FirstOrDefault(x => x.ItemId == model_phone_id));
            if (_Index < 0) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Telefon bulunamadı."); return; }
            var _phone = API.fromJson(_inventory.ItemList[_Index].SpecifiedValue).ToObject<SpecifiedValuePhone>() as SpecifiedValuePhone;

            int _bankMoney = API.getEntityData(sender, "BankMoney");
            if (_bankMoney >= (_phone.PhoneOperator == Operator.Vodacell ? value * 0.5 : value * 0.2))
            {
                _bankMoney -= (int)(_phone.PhoneOperator == Operator.Vodacell ? value * 0.5 : value * 0.2);
                _phone.Balance += value;

                _inventory.ItemList[_Index].SpecifiedValue = API.toJson(_phone);
                API.setEntityData(sender, "inventory", _inventory);
                API.setEntityData(sender, "BankMoney", _bankMoney);
                API.sendNotificationToPlayer(sender, "Banka Hesabı: \n~r~-" + (int)(_phone.PhoneOperator == Operator.Vodacell ? value * 0.5 : value * 0.2));
                return;
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Bankanızda yeterli bakiye bulunmuyor.");
            }


        }
        public void OnContacts(Client sender, int model_phone_id)
        {
            Inventory _inventory = API.getEntityData(sender, "inventory");
            SpecifiedValuePhone _phone = API.fromJson(_inventory.ItemList.FirstOrDefault(x => x.ItemId == model_phone_id).SpecifiedValue).ToObject<SpecifiedValuePhone>();
            if (_phone != null)
            {
                API.triggerClientEvent(sender, "phone_contacts", _phone.Contacts.Count, _phone.Contacts.Keys.ToArray(), _phone.Contacts.Values.ToArray());
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Telefon bulunamadı.");
            }
        }
        private void ContactAdd(Client sender, string number, string name)
        {
            //try { Convert.ToInt32(number); } catch (Exception) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Telefon numarası yalnızca rakamlardan oluşabilir."); return; }
            var _inventory = (Inventory)API.getEntityData(sender, "inventory");
            var _phone = (SpecifiedValuePhone)API.fromJson(_inventory.ItemList.FirstOrDefault(x => PhoneIdList.Contains(x.ItemId) && (API.fromJson(x.SpecifiedValue).ToObject<SpecifiedValuePhone>() as SpecifiedValuePhone).FlightMode == false).SpecifiedValue).ToObject<SpecifiedValuePhone>();
            if (_phone == null) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Envanterinizde telefon bulunmuyor."); return; }
            _phone.Contacts.Add(name, number);
            _inventory.ItemList.FirstOrDefault(x => PhoneIdList.Contains(x.ItemId)).SpecifiedValue = API.toJson(_phone);
            API.setEntityData(sender, "inventory", _inventory);
        }
        public void OnFlightMode(Client sender, int model_phone_id)
        {
            var _inventory = (Inventory)API.getEntityData(sender, "inventory");
            //var _phone = (SpecifiedValuePhone)API.fromJson(_inventory.ItemList.FirstOrDefault(x => PhoneIdList.Contains(x.ItemId) && (API.fromJson(x.SpecifiedValue).ToObject<SpecifiedValuePhone>() as SpecifiedValuePhone).FlightMode == false).SpecifiedValue).ToObject<SpecifiedValuePhone>();
            //if (_phone == null) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Envanterinizde telefon bulunmuyor."); return; }
            //_phone.FlightMode = !_phone.FlightMode;
            var _phone = API.fromJson(_inventory.ItemList.FirstOrDefault(x => x.ItemId == model_phone_id).SpecifiedValue).ToObject<SpecifiedValuePhone>() as SpecifiedValuePhone;
            if (_phone != null)
            {
                _phone.FlightMode = !(_phone.FlightMode);
                API.sendChatMessageToPlayer(sender, _phone.FlightMode ? "~y~Telefonunuz uçak moduna alındı. ~r~*" : "~y~ Telefonunuz uçak modundan çıkarıldı. ~g~*");
                API.setEntityData(sender, "inventory", _inventory);
                List<string> phoneNumbers = new List<string>();
                if (API.hasEntityData(sender, "PhoneNumbers"))
                {
                    phoneNumbers = API.getEntityData(sender, "PhoneNumbers");
                }

                if (_phone.FlightMode)
                {
                    phoneNumbers.Remove(_phone.PhoneNumber);
                }
                else
                {
                    phoneNumbers.Add(_phone.PhoneNumber);
                }
                _inventory.ItemList.FirstOrDefault(x => x.ItemId == model_phone_id).SpecifiedValue = API.toJson(_phone);
                API.setEntityData(sender, "inventory", _inventory);
                API.setEntityData(sender, "PhoneNumbers", phoneNumbers);
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Telefon bulunamadı.");
            }

        }

        public void OnGpsSelected(Client sender, int model_phone_id)
        {
            Inventory _inventory = (Inventory)API.getEntityData(sender, "inventory");
            int _Index = _inventory.ItemList.IndexOf(_inventory.ItemList.FirstOrDefault(x => x.ItemId == model_phone_id));
            var _phone = (SpecifiedValuePhone)API.fromJson(_inventory.ItemList[_Index].SpecifiedValue).ToObject<SpecifiedValuePhone>();
            if (!_phone.FlightMode)
            {
                if (_phone.InternetBalance > 2)
                {
                    List<string> names = new List<string>();
                    foreach (var item in db_Blips.currentBlips.Items)
                    {
                        names.Add(item.Name);
                    }
                    API.triggerClientEvent(sender, "phone_gps_open", names.Count, names, null);
                    _phone.InternetBalance--;
                    _inventory.ItemList[_Index].SpecifiedValue = API.toJson(_phone);
                    API.setEntityData(sender, "inventory", _inventory);
                }
                else
                {
                    API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Telefonunuzdaki internet yetersiz. Yükleme yapınız.");
                }
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Uçak modu açıkken bunu yapamazsınız.");
            }
        }

        public static void UpdatePhoneNumbers(Client player)
        {
            List<string> numbers = new List<string>();
            foreach (var item in player.GetInventory().ItemList.Where(x => PhoneManager.PhoneIdList.Contains(x.ItemId)))
            {
                SpecifiedValuePhone _phone = API.shared.fromJson(item.SpecifiedValue).ToObject<SpecifiedValuePhone>();
                if (_phone.FlightMode == false)
                    numbers.Add(_phone.PhoneNumber);
            }
            API.shared.setEntityData(player, "PhoneNumbers", numbers);
        }
        public void OnGpsPointRequested(Client sender, string localeText, int model_phone_id)
        {
            var _inventory = (Inventory)API.getEntityData(sender, "inventory");
            var _phone = API.fromJson(_inventory.ItemList.FirstOrDefault(x => x.ItemId == model_phone_id).SpecifiedValue).ToObject<SpecifiedValuePhone>() as SpecifiedValuePhone;
            if (_phone.FlightMode) { API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Uçak modundayken bunu yapamazsınız."); return; }
            if (_phone.InternetBalance <= 1) { API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Telefonnuuzda bunu yapabilmek için yeterli internet yok."); return; }
            _phone.InternetBalance--;
            _inventory.ItemList.FirstOrDefault(x => x.ItemId == model_phone_id).SpecifiedValue = API.toJson(_phone);
            API.setEntityData(sender, "inventory", _inventory);

            int _Index = 0, _nearestIndex = 0;
            int lastDistance = int.MaxValue;
            switch (localeText)
            {
                case "En Yakın Banka":
                    _Index = 0; _nearestIndex = 0; lastDistance = int.MaxValue;
                    foreach (var itemBank in db_Banks.CurrentBanks.Item1)
                    {
                        if (lastDistance > Vector3.Distance(sender.position, itemBank.Position))
                        {
                            _nearestIndex = _Index;
                        }
                        _Index++;
                        lastDistance = Convert.ToInt32(Vector3.Distance(sender.position, itemBank.Position));
                    }
                    API.triggerClientEvent(sender, "update_waypoint", db_Banks.CurrentBanks.Item1[_nearestIndex].Position.X, db_Banks.CurrentBanks.Item1[_nearestIndex].Position.Y);
                    break;

                case "En Yakın Telefoncu":
                    _Index = 0; _nearestIndex = 0;
                    lastDistance = int.MaxValue;
                    foreach (var item in db_PhoneOperatorShop.CurrentOperatorShop.Item1)
                    {
                        if (lastDistance > Vector3.Distance(sender.position, item.Position))
                        {
                            _nearestIndex = _Index;
                        }
                        _Index++;
                        lastDistance = Convert.ToInt32(Vector3.Distance(sender.position, item.Position));
                    }
                    API.triggerClientEvent(sender, "update_waypoint", db_PhoneOperatorShop.CurrentOperatorShop.Item1[_nearestIndex].Position.X, db_PhoneOperatorShop.CurrentOperatorShop.Item1[_nearestIndex].Position.Y);
                    break;

                case "En Yakın Alışveriş":
                    _Index = 0; _nearestIndex = 0; lastDistance = int.MaxValue;
                    foreach (var item in db_Shops.CurrentShopsList)
                    {
                        if (lastDistance > Vector3.Distance(sender.position, item.Position))
                        {
                            _nearestIndex = _Index;
                        }
                        lastDistance = Convert.ToInt32(Vector3.Distance(sender.position, item.Position));
                        _Index++;
                    }
                    API.triggerClientEvent(sender, "update_waypoint", db_Shops.CurrentShopsList[_nearestIndex].Position.X, db_Shops.CurrentShopsList[_nearestIndex].Position.Y);
                    break;

                case "En Yakın Araç Satıcısı":
                    _Index = 0; _nearestIndex = 0; lastDistance = int.MaxValue;
                    foreach (var item in db_SaleVehicles.currentSaleVehicleList.Items)
                    {
                        if (lastDistance > Vector3.Distance(sender.position, new Vector3(item.Position.X, item.Position.Y, item.Position.Z)))
                        {
                            _nearestIndex = _Index;
                        }
                        _Index++;
                        lastDistance = Convert.ToInt32(Vector3.Distance(sender.position, new Vector3(item.Position.X, item.Position.Y, item.Position.Z)));
                    }
                    API.triggerClientEvent(sender, "update_waypoint", db_SaleVehicles.currentSaleVehicleList.Items[_nearestIndex].Position.X, db_SaleVehicles.currentSaleVehicleList.Items[_nearestIndex].Position.Y);
                    break;

                case "En Yakın Meslek":
                    var pos = Jobs.db_Jobs.currentJobsList.OrderBy(o => Vector3.Distance(sender.position, o.Item1.Position)).FirstOrDefault().Item1.Position;
                    API.triggerClientEvent(sender, "update_waypoint", pos.X, pos.Y);
                    break;

                case "En Yakın Benzinlik":


                    var posGas = db_GasStations.CurrentGasStations.Item1.OrderBy(o => Vector3.Distance(o.Position, sender.position)).FirstOrDefault().Position;
                    API.triggerClientEvent(sender, "update_waypoint", posGas.X, posGas.Y);
                    break;

                default:
                    foreach (var item in db_Blips.currentBlips.Items)
                    {
                        if (localeText == item.Name)
                        {
                            API.triggerClientEvent(sender, "update_waypoint", item.Position.X, item.Position.Y);
                            return;
                        }
                    }
                    break;

            }
        }
        public void OnDownloadappStore(Client sender, int model_phone_id)
        {
            Inventory _inventory = (Inventory)API.getEntityData(sender, "inventory");
            int _Index = _inventory.ItemList.IndexOf(_inventory.ItemList.FirstOrDefault(x => x.ItemId == model_phone_id));
            if (_Index >= 0)
            {
                var _phone = (SpecifiedValuePhone)API.fromJson(_inventory.ItemList[_Index].SpecifiedValue).ToObject<SpecifiedValuePhone>();
                if (_phone.FlightMode == false)
                {
                    if (_phone.InternetBalance > 0)
                    {
                        List<string> names = new List<string>();
                        List<string> descriptions = new List<string>();
                        foreach (var item in Enum.GetValues(typeof(Application)))
                        {
                            Application app = (Application)item;
                            if (_phone.Applications.Contains(app))
                            {
                                names.Add("*" + item.ToString());
                                descriptions.Add("Bu uygulama şu anda telefonunuzda yüklü. Silmek için seçiniz.");
                            }
                            else
                            {
                                names.Add(item.ToString());
                                descriptions.Add("Bu uygulamayı yükleyebilirsiniz.");
                            }
                        }
                        _phone.InternetBalance--;
                        _inventory.ItemList[_Index].SpecifiedValue = API.toJson(_phone);
                        API.triggerClientEvent(sender, "phone_downloadscreen", names.Count, names.ToArray(), descriptions.ToArray());
                    }
                    else
                    {
                        API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Hattınızda internet hakkınız kalmamış.");
                    }
                }
                else
                {
                    API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Bu işlem için telefonunuzu uçak modundan çıkarmalısınız.");
                }
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Telefon bulunamadı.");
            }
        }

        public void OnClickDownloadApp(Client sender, string appName, int model_phone_id)
        {
            Inventory _inventory = (Inventory)API.getEntityData(sender, "inventory");
            int _Index = _inventory.ItemList.IndexOf(_inventory.ItemList.FirstOrDefault(x => x.ItemId == model_phone_id));
            if (_Index < 0) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Telefon bulunamadı."); return; }
            var _phone = (SpecifiedValuePhone)API.fromJson(_inventory.ItemList[_Index].SpecifiedValue).ToObject<SpecifiedValuePhone>();
            if (_phone.FlightMode) { API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Uçak modundayken bunu yapamazsınız."); return; }


            bool Remove = appName.Contains("*") ? true : false;
            appName = appName.Contains("*") ? appName.Replace("*", String.Empty) : appName;
            foreach (var item in Enum.GetValues(typeof(Application)))
            {
                if (appName == item.ToString())
                {
                    if (Remove)
                    {
                        _phone.Applications.Remove((Application)item);
                        API.sendChatMessageToPlayer(sender, "~y~Uygulamanız başarıyla kaldırıldı.");
                        _inventory.ItemList[_Index].SpecifiedValue = API.toJson(_phone);
                        API.setEntityData(sender, "inventory", _inventory);
                    }
                    else
                    {
                        if (_phone.InternetBalance > 24)
                        {
                            if (Convert.ToInt32(db_Items.GetItemById(_inventory.ItemList[_Index].ItemId).Value_0) > _phone.Applications.Count)
                            {
                                _phone.Applications.Add((Application)item);
                                _phone.InternetBalance -= 25;
                                _inventory.ItemList[_Index].SpecifiedValue = API.toJson(_phone);
                                API.setEntityData(sender, "inventory", _inventory);
                                API.sendChatMessageToPlayer(sender, "~y~Uygulamanız başarıyla indirildi.");
                            }
                            else
                            {
                                API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Telefonunuz daha fazla uygulamayı kaldırmıyor. Birkaçını silmeyi deneyin.");
                            }
                        }
                        else
                        {
                            API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Bu işlemi gerçekleştirmek için yeterli internetiniz yok.");
                        }
                    }
                    return;
                }
            }

        }
        public void OnRadioFrequence(Client sender, string frequence)
        {
            API.setEntityData(sender, "Frequence", Convert.ToInt32(frequence));
            API.sendChatMessageToPlayer(sender, "~y~Radyo frekansınız " + frequence + " olarak ayarlandı. Kullanmak için ~s~( /r )");
        }
        [Command("radyo", "/r [yazı]", Alias = "r", GreedyArg = true)]
        public void RadioTalk(Client sender, string text)
        {
            Inventory _inventory = (Inventory)API.getEntityData(sender, "inventory");
            int _Index = _inventory.ItemList.IndexOf(_inventory.ItemList.FirstOrDefault(x => PhoneIdList.Contains(x.ItemId) && (API.fromJson(x.SpecifiedValue).ToObject<SpecifiedValuePhone>() as SpecifiedValuePhone).FlightMode == false));
            var _phone = (SpecifiedValuePhone)API.fromJson(_inventory.ItemList[_Index].SpecifiedValue).ToObject<SpecifiedValuePhone>();
            if (_phone.Applications.Contains(Application.Radyo))
            {
                if (_phone.InternetBalance > 0)
                {
                    //rpgMGr.Me(sender, " kulaklığındaki mikrofona doğru bir şeyler fısıldar.");
                    rpgMGr.UpdatePlayerTalkLabel(sender, "~#F1F1F2~" + text, 15);
                    foreach (var itemPlayer in API.getAllPlayers())
                    {
                        if (API.getEntityData(itemPlayer, "Frequence") == API.getEntityData(sender, "Frequence"))
                        {
                            API.sendChatMessageToPlayer(itemPlayer, "~#00ffd4~[Radyo(" + API.getEntityData(sender, "Frequence") + ")]" + API.getEntityData(sender, "CharacterName") + ": " + text);
                        }
                    }
                    //API.sendChatMessageToPlayer(sender, "~#00ffd4~[Radyo(" + API.getEntityData(sender, "Frequence") + ")]" + API.getEntityData(sender, "CharacterName") + ": " + text);
                    _phone.InternetBalance--;
                    _inventory.ItemList[_Index].SpecifiedValue = API.toJson(_phone);
                    API.setEntityData(sender, "inventory", _inventory);
                }
                else
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Hattınızda internet hakkınız kalmamış.");

            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Telefonunuzda Radyo uygulaması yüklü değil.");
            }
        }
        public void OnEstateApp(Client sender, int model_phone_id)
        {
            Inventory _inventory = (Inventory)API.getEntityData(sender, "inventory");
            int _Index = _inventory.ItemList.IndexOf(_inventory.ItemList.FirstOrDefault(x => x.ItemId == model_phone_id));
            if (_Index >= 0)
            {
                var _phone = (SpecifiedValuePhone)API.fromJson(_inventory.ItemList[_Index].SpecifiedValue).ToObject<SpecifiedValuePhone>();
                if (_phone.FlightMode) { API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Bu işlemi telefonunuz uçak modundayken yapamazsınız. "); return; }
                if (_phone.InternetBalance > 0)
                {
                    _phone.InternetBalance--;
                    List<string> names = new List<string>(); List<string> descriptions = new List<string>();
                    var housesToList = db_Houses.CurrentHousesDict.Values.Where(x => x.IsSelling = true).OrderByDescending(x => x.Price);
                    foreach (var item in housesToList)
                    {
                        names.Add($"~y~({ item.HouseId})~s~{item.Name} | $ {item.Price}");
                        descriptions.Add("Buradan uzaklığı: " + Vector3.Distance(sender.position, item.EntrancePosition));
                    }
                    API.triggerClientEvent(sender, "phone_emlakci_open", names.Count, names.ToArray(), descriptions.ToArray());
                    _inventory.ItemList[_Index].SpecifiedValue = API.toJson(_phone);
                    API.setEntityData(sender, "inventory", _inventory);
                    return;
                }
                else
                {
                    API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Telefonunuzda internet hakkınız kalmamış. ");
                    return;
                }
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Telefon bulunamadı.");
            }

        }
        public void OnEstateAppNearest(Client sender)
        {
            Inventory _inventory = (Inventory)API.getEntityData(sender, "inventory");
            int _Index = _inventory.ItemList.IndexOf(_inventory.ItemList.FirstOrDefault(x => PhoneIdList.Contains(x.ItemId) && (API.fromJson(x.SpecifiedValue).ToObject<SpecifiedValuePhone>() as SpecifiedValuePhone).FlightMode == false));
            if (_Index >= 0)
            {
                var _phone = (SpecifiedValuePhone)API.fromJson(_inventory.ItemList[_Index].SpecifiedValue).ToObject<SpecifiedValuePhone>();
                if (_phone.InternetBalance > 0)
                {
                    _phone.InternetBalance--;
                    List<string> names = new List<string>(); List<string> descriptions = new List<string>();
                    var housesToList = db_Houses.CurrentHousesDict.Values.Where(x => x.IsSelling = true).OrderBy(x => Vector3.Distance(sender.position, x.EntrancePosition));
                    foreach (var item in housesToList)
                    {
                        names.Add("~y~(" + item.HouseId + ")~s~" + item.Name + " | $" + item.Price);
                        descriptions.Add("Buradan uzaklığı: " + Vector3.Distance(sender.position, item.EntrancePosition));
                    }
                    API.triggerClientEvent(sender, "phone_emlakci_open", names.Count, names.ToArray(), descriptions.ToArray());
                    _inventory.ItemList[_Index].SpecifiedValue = API.toJson(_phone);
                    API.setEntityData(sender, "inventory", _inventory);
                    return;
                }
                else
                {
                    API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Telefonunuzda internet hakkınız kalmamış. ");
                    return;
                }
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Telefon bulunamadı.");
            }
        }
        public void OnEstateAppFarthest(Client sender)
        {
            Inventory _inventory = (Inventory)API.getEntityData(sender, "inventory");
            int _Index = _inventory.ItemList.IndexOf(_inventory.ItemList.FirstOrDefault(x => PhoneIdList.Contains(x.ItemId) && (API.fromJson(x.SpecifiedValue).ToObject<SpecifiedValuePhone>() as SpecifiedValuePhone).FlightMode == false));
            if (_Index >= 0)
            {
                var _phone = (SpecifiedValuePhone)API.fromJson(_inventory.ItemList[_Index].SpecifiedValue).ToObject<SpecifiedValuePhone>();
                if (_phone.InternetBalance > 0)
                {
                    _phone.InternetBalance--;
                    List<string> names = new List<string>(); List<string> descriptions = new List<string>();
                    var housesToList = db_Houses.CurrentHousesDict.Values.Where(x => x.IsSelling = true).OrderByDescending(x => Vector3.Distance(sender.position, x.EntrancePosition));
                    foreach (var item in housesToList)
                    {
                        names.Add("~y~(" + item.HouseId + ")~s~" + item.Name + " | $" + item.Price);
                        descriptions.Add("Buradan uzaklığı: " + Vector3.Distance(sender.position, item.EntrancePosition));
                    }
                    API.triggerClientEvent(sender, "phone_emlakci_open", names.Count, names.ToArray(), descriptions.ToArray());
                    _inventory.ItemList[_Index].SpecifiedValue = API.toJson(_phone);
                    API.setEntityData(sender, "inventory", _inventory);
                    return;
                }
                else
                {
                    API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Telefonunuzda internet hakkınız kalmamış. ");
                    return;
                }
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Telefon bulunamadı.");
            }
        }
        public void OnEstateHouseSelected(Client sender, int _houseId)
        {
            API.consoleOutput("onHouseSelected");
            Inventory _inventory = (Inventory)API.getEntityData(sender, "inventory");
            int _Index = _inventory.ItemList.IndexOf(_inventory.ItemList.FirstOrDefault(x => PhoneIdList.Contains(x.ItemId) && (API.fromJson(x.SpecifiedValue).ToObject<SpecifiedValuePhone>() as SpecifiedValuePhone).FlightMode == false));
            if (_Index >= 0)
            {
                var _phone = (SpecifiedValuePhone)API.fromJson(_inventory.ItemList[_Index].SpecifiedValue).ToObject<SpecifiedValuePhone>();
                if (_phone.Applications.Contains(Application.GPS))
                {
                    if (_phone.InternetBalance > 0)
                    {
                        _phone.InternetBalance--;
                        var bahsiGecenEv = db_Houses.GetHouse(_houseId);
                        API.triggerClientEvent(sender, "update_waypoint", bahsiGecenEv.EntrancePosition.X, bahsiGecenEv.EntrancePosition.Y);
                        _inventory.ItemList[_Index].SpecifiedValue = API.toJson(_phone);
                        API.setEntityData(sender, "inventory", _inventory);
                        return;
                    }
                    else
                    {
                        API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Telefonunuzda internet hakkınız kalmamış. ");
                        return;
                    }
                }
                else
                {
                    API.sendChatMessageToPlayer(sender, "~r~UYARI: Telefonunuzda GPS uygulaması yüklü olmalıdır.");
                }

            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Telefon bulunamadı.");
            }
        }

        public static List<string> GetPhoneNumbers(Client player)
        {
            return (API.shared.getEntityData(player, "PhoneNumbers") as List<string>);
        }
        public static void UpdatePhoneNumbers(Client player, List<string> _numbers)
        {
            API.shared.setEntityData(player, "PhoneNumbers", _numbers);
        }
        public static void AddPhoneNumberToPlayer(Client player, string number)
        {
            var numbers = GetPhoneNumbers(player);
            numbers.Add(number);
            UpdatePhoneNumbers(player, numbers);
        }
        public static void AddPhoneNumberToPlayer(Client player, ClientItem clientItem)
        {
            if (clientItem == null) { API.shared.consoleOutput(LogCat.Warn, "AddPhoneNumbers | ClientItem NULL"); return; }
            SpecifiedValuePhone _phone = (SpecifiedValuePhone)(String.IsNullOrEmpty(clientItem.SpecifiedValue) ? new SpecifiedValuePhone() : API.shared.fromJson(clientItem.SpecifiedValue).ToObject<SpecifiedValuePhone>());
            if (!String.IsNullOrEmpty(_phone.PhoneNumber))
            {
                AddPhoneNumberToPlayer(player, _phone.PhoneNumber);
            }
            else
            {
                API.shared.consoleOutput(LogCat.Warn, "Add Phone Number, number in phone is null");
            }
        }
        public static void RemoveNumberFromPlayer(Client player, string number)
        {
            var numbers = GetPhoneNumbers(player);
            numbers.Remove(number);
            UpdatePhoneNumbers(player, numbers);
        }
        public static void RemoveNumberFromPlayer(Client player, ClientItem clientItem)
        {
            SpecifiedValuePhone _phone = (SpecifiedValuePhone)(String.IsNullOrEmpty(clientItem.SpecifiedValue) ? new SpecifiedValuePhone() : API.shared.fromJson(clientItem.SpecifiedValue).ToObject<SpecifiedValuePhone>());
            if (!String.IsNullOrEmpty(_phone.PhoneNumber))
            {
                RemoveNumberFromPlayer(player, _phone.PhoneNumber);
            }
            else
            {
                API.shared.consoleOutput(LogCat.Warn, "Add Phone Number, number in phone is null");
            }

        }
    }
}
