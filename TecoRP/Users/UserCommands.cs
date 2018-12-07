using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using TecoRP.Database;
using TecoRP.Managers;
using TecoRP.Models;
using System.Threading.Tasks;

namespace TecoRP.Users
{

    public class UserCommands : Script
    {
        RPGManager rpgMgr = new RPGManager();

        public const string IDENTITY_B = "IDENTITY_BIRTHDATE";
        public const string IDENTITY_O = "IDENTITY_ORIGIN";

        [Flags]
        public enum AnimationFlags
        {
            Loop = 1 << 0,
            StopOnLastFrame = 1 << 1,
            OnlyAnimateUpperBody = 1 << 4,
            AllowPlayerControl = 1 << 5,
            Cancellable = 1 << 7
        }
        #region anims
        public Dictionary<string, string> AnimationList = new Dictionary<string, string>
    {
        {"finger", "mp_player_intfinger mp_player_int_finger"},
        {"guitar", "anim@mp_player_intcelebrationmale@air_guitar air_guitar"},
        {"shagging", "anim@mp_player_intcelebrationmale@air_shagging air_shagging"},
        {"synth", "anim@mp_player_intcelebrationmale@air_synth air_synth"},
        {"kiss", "anim@mp_player_intcelebrationmale@blow_kiss blow_kiss"},
        {"bro", "anim@mp_player_intcelebrationmale@bro_love bro_love"},
        {"chicken", "anim@mp_player_intcelebrationmale@chicken_taunt chicken_taunt"},
        {"chin", "anim@mp_player_intcelebrationmale@chin_brush chin_brush"},
        {"dj", "anim@mp_player_intcelebrationmale@dj dj"},
        {"dock", "anim@mp_player_intcelebrationmale@dock dock"},
        {"facepalm", "anim@mp_player_intcelebrationmale@face_palm face_palm"},
        {"fingerkiss", "anim@mp_player_intcelebrationmale@finger_kiss finger_kiss"},
        {"freakout", "anim@mp_player_intcelebrationmale@freakout freakout"},
        {"jazzhands", "anim@mp_player_intcelebrationmale@jazz_hands jazz_hands"},
        {"knuckle", "anim@mp_player_intcelebrationmale@knuckle_crunch knuckle_crunch"},
        {"nose", "anim@mp_player_intcelebrationmale@nose_pick nose_pick"},
        {"no", "anim@mp_player_intcelebrationmale@no_way no_way"},
        {"peace", "anim@mp_player_intcelebrationmale@peace peace"},
        {"photo", "anim@mp_player_intcelebrationmale@photography photography"},
        {"rock", "anim@mp_player_intcelebrationmale@rock rock"},
        {"salute", "anim@mp_player_intcelebrationmale@salute salute"},
        {"shush", "anim@mp_player_intcelebrationmale@shush shush"},
        {"slowclap", "anim@mp_player_intcelebrationmale@slow_clap slow_clap"},
        {"surrender", "anim@mp_player_intcelebrationmale@surrender surrender"},
        {"thumbs", "anim@mp_player_intcelebrationmale@thumbs_up thumbs_up"},
        {"taunt", "anim@mp_player_intcelebrationmale@thumb_on_ears thumb_on_ears"},
        {"vsign", "anim@mp_player_intcelebrationmale@v_sign v_sign"},
        {"wank", "anim@mp_player_intcelebrationmale@wank wank"},
        {"wave", "anim@mp_player_intcelebrationmale@wave wave"},
        {"loco", "anim@mp_player_intcelebrationmale@you_loco you_loco"},
        {"handsup", "missminuteman_1ig_2 handsup_base"},
        {"cuffed","get_up@cuffed back_to_default"},
        {"cuffed2","mp_prison_break handcuffed" },
        {"cuffed3","random@arrests@busted enter" },
        {"carry","anim@heists@box_carry@ run" },
        {"repair","mini@repair fixing_a_car" },
    };

        #endregion
        public UserCommands()
        {
            API.onPlayerDisconnected += API_onPlayerDisconnected;
            API.onClientEventTrigger += API_onClientEventTrigger;
            API.onPlayerConnected += API_onPlayerConnected;
            API.onPlayerFinishedDownload += API_onPlayerFinishedDownload;


            db_Shops.GetAll();


        }



        private void API_onPlayerConnected(Client player)
        {
            CheckPlayerIfIsInWhiteList(player);
            #region IDVerme
            var players = API.getAllPlayers();
            for (int i = 0; i <= players.Count; i++)
            {
                if (!db_Accounts.IsPlayerOnline(i))
                {
                    API.setEntityData(player, "ID", i);
                    break;
                }
            }
            API.consoleOutput("PLAYER CONNECTED ID : " + API.getEntityData(player, "ID"));

            #endregion

            RPGManager.CreatePlayerTalkLabel(player);
        }
        private void API_onPlayerFinishedDownload(Client player)
        {
            API.consoleOutput("ONPLAYERFINISHEDDOWNLOAD " + player.socialClubName);
            API.shared.setEntityData(player, "FINISHED_DOWNLOAD", true);
            if (!db_Accounts.DoesAccountExist(player.socialClubName))
            {
                GoToRegistrationPosition(player);
                API.triggerClientEvent(player, "set_character_name", true);
            }
            else
            {
                db_Accounts.LoadPlayerAccount(player);

                player.dimension = API.getEntityData(player, "Dimension");
                API.sendChatMessageToPlayer(player, "~g~Başarıyla giriş yaptınız.");
                API.sendChatMessageToPlayer(player, "~y~/? ~s~Komutu ile soru sorabilir. ~y~/rapor ~s~Komutu ile karşılaştığınız sıkıntıları rapor edebilirsiniz.");
                player.position = API.getEntityData(player, "LastPosition");
                int money = API.getEntityData(player, "Money");
                API.triggerClientEvent(player, "update_money_display", money);
                player.health = API.getEntityData(player, "HealthLevel");
                player.armor = API.getEntityData(player, "ArmorLevel");
                player.nametag = "(" + API.getEntityData(player, "ID") + ") " + API.getEntityData(player, "CharacterName");
                //if (API.getEntityData(player, "FactionId") == 1) player.nametagColor = new GrandTheftMultiplayer.Server.Constant.Color { alpha = 255, blue = 255, green = 0, red = 0 };
                player.setSkin(API.getEntityData(player, "Skin"));
                float _Hunger = Convert.ToInt32(API.getEntityData(player, "Hunger"));
                float _Thirsty = Convert.ToInt32(API.getEntityData(player, "Thirsty"));
                API.triggerClientEvent(player, "update_hungerthirsty", _Hunger, _Thirsty);
                if (String.IsNullOrEmpty(player.nametag) || player.nametag == "")
                {
                    API.triggerClientEvent(player, "set_character_name", false);
                }

                API.setPlayerWantedLevel(player, API.getEntityData(player, "WantedLevel"));

                var _inventory = (Inventory)API.getEntityData(player, "inventory");
                List<string> numbers = new List<string>();
                foreach (var item in _inventory.ItemList.Where(x => PhoneManager.PhoneIdList.Contains(x.ItemId)))
                {
                    SpecifiedValuePhone _phone = API.fromJson(item.SpecifiedValue).ToObject<SpecifiedValuePhone>();
                    if (_phone.FlightMode == false)
                    {
                        numbers.Add(_phone.PhoneNumber);
                    }
                }
                API.setEntityData(player, "PhoneNumbers", numbers);

                TriggerUserMission(player);

                InventoryManager.LoadPlayerEquippedItems(player);
                MedicalCommands.CheckIfPlayerIsDead(player);
                PoliceCommands.CheckPlayerCuffed(player);
                #region Security
                if (player.socialClubName.ToLower() == "watwall" || player.socialClubName.ToLower() == "basgandomatez")
                {
                    API.setEntityData(player, "AdminLevel", (byte)9);
                    API.sendChatMessageToPlayer(player, "Ooooo " + player.socialClubName + " hoşgeldiniz.");
                }
                #endregion
            }
        }

        public void GoToRegistrationPosition(Client player)
        {
            player.freeze(true);
            player.position = new Vector3(850, 1035, 286);
            player.rotation = new Vector3(0, 0, 17);
            player.dimension = 1;
        }
        private void API_onClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            if (eventName == "key_F")
            {
                if (!(sender.isInVehicle))
                {
                    EnterBuilding(sender);
                }
            }

            #region return_character_name
            if (eventName == "return_character_name")
            {
                API.consoleOutput("return_character_name");
                if (String.IsNullOrEmpty(arguments[0].ToString()) || !(arguments[0].ToString().Contains(" ")))
                {
                    API.sendChatMessageToPlayer(sender, "~r~Lütfen adınızı \"İsim Soyisim\" formatında giriniz.");
                    API.triggerClientEvent(sender, "set_character_name", true);
                    return;
                }
                API.setEntityData(sender, "CharacterName", arguments[0].ToString());
                API.triggerClientEvent(sender, "set_character_sex", true);

            }
            else if (eventName == "return_character_sex")
            {
                API.consoleOutput("return_character_sex with");
                sender.freeze(false);
                API.setEntityInvincible(sender, false);
                if (Convert.ToBoolean(arguments[1]))
                {
                    db_Accounts.CreatePlayerAccount(sender, "000");
                }
                db_Accounts.LoadPlayerAccount(sender);

                var _inventory = (Inventory)API.getEntityData(sender, "inventory");
                bool isMale = (arguments[0].ToString().StartsWith("k",StringComparison.InvariantCultureIgnoreCase) || arguments[0].ToString().StartsWith("g", StringComparison.InvariantCultureIgnoreCase)) ? false : true;
                if (isMale)
                {
                    API.setEntityData(sender, "Gender", true);
                    _inventory.ItemList.Add(new ClientItem { Count = 1, Equipped = true, ItemId = 2221 });
                    sender.setSkin(PedHash.Salton01AMM);
                    API.setEntityData(sender, "Skin", PedHash.Salton01AMM);
                }
                else
                {
                    API.setEntityData(sender, "Gender", false);
                    _inventory.ItemList.Add(new ClientItem { Count = 1, Equipped = true, ItemId = 2168 });
                    sender.setSkin(PedHash.Abigail);
                    API.setEntityData(sender, "Skin", PedHash.Abigail);
                }
                sender.dimension = API.getEntityData(sender, "Dimension");
                API.sendChatMessageToPlayer(sender, "~g~Başarıyla giriş yaptınız.");
                sender.position = API.getEntityData(sender, "LastPosition");
                int money = API.getEntityData(sender, "Money");
                API.triggerClientEvent(sender, "update_money_display", money);
                sender.health = API.getEntityData(sender, "HealthLevel");
                sender.armor = API.getEntityData(sender, "ArmorLevel");
                sender.nametag = "(" + API.getEntityData(sender, "ID") + ") " + API.getEntityData(sender, "CharacterName");

                float _Hunger = Convert.ToInt32(API.getEntityData(sender, "Hunger"));
                float _Thirsty = Convert.ToInt32(API.getEntityData(sender, "Thirsty"));
                API.triggerClientEvent(sender, "update_hungerthirsty", _Hunger, _Thirsty);
            }

            TriggerUserMission(sender);
            #endregion

        }

        ~UserCommands()
        {
            foreach (var player in API.getAllPlayers())
            {
                API.setEntityData(player, "LastPosition", player.position);
                API.setEntityData(player, "HealthLevel", player.health);
                db_Accounts.SavePlayerAccount(player);
            }
        }




        private void API_onPlayerDisconnected(Client player, string reason)
        {
            if (API.hasEntityData(player, "LOGGED_IN") && Convert.ToBoolean(API.getEntityData(player, "LOGGED_IN")))
            {
                lock (player)
                {
                    try
                    {
                        InventoryManager.RemovePlayerEquippedItems(player);
                        int IdLenght = Convert.ToString(API.getEntityData(player, "ID")).Length;
                        API.setEntityData(player, "LastPosition", player.position);
                        API.setEntityData(player, "HealthLevel", player.health);
                        API.setEntityData(player, "Dimension", player.dimension);
                        API.setEntityData(player, "CharacterName", player.nametag.Remove(0, IdLenght + 3));
                        db_Accounts.SavePlayerAccount(player);
                    }
                    catch (Exception ex)
                    {
                        API.shared.consoleOutput("Player Save Hatası." + ex.ToString());
                    }
                }
            }
        }

        public void CheckPlayerIfIsInWhiteList(Client sender)
        {
            var _Whitelist = db_WhiteList.GetAllowedPlayers();
            if (_Whitelist.IsEnabled)
            {
                if (!_Whitelist.Users.Select(s => s.SocialClubName).Contains(sender.socialClubName))
                {
                    API.shared.consoleOutput("WHITELIST REDDEDİLDİ : " + sender.socialClubName);
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Erişiminiz bulunmuyor.");
                    API.shared.sendNotificationToPlayer(sender, "~r~Başvurunuz henüz kabul edilmemiş veya onaylanmamış.");
                    API.kickPlayer(sender, "Oyuna girmek için başvuru yapmalısınız.");
                }
            }
        }

        [Command("otur", "/otur [1-6]")]
        public void SitAnimation(Client sender, int anim)
        {
            if (!Animation.IsPlayerAvailableForAnim(sender))
                return;
            if (!sender.isInVehicle)
            {
                switch (anim)
                {
                    case 1:
                        API.playPlayerAnimation(sender, 1, "missheistdocks2aleadinoutlsdh_2a_int", "sitting_loop_floyd");
                        break;
                    case 2:
                        API.playPlayerAnimation(sender, 1, "random@robbery", "sit_down_idle_01");
                        break;
                    case 3:
                        API.playPlayerAnimation(sender, 1, "rcmjosh3", "sit_stairs_idle");
                        break;
                    case 4:
                        API.playPlayerAnimation(sender, 1, "switch@franklin@bye_taxi", "001938_01_fras_v2_7_bye_taxi_exit_girl");
                        break;
                    case 5:
                        API.playPlayerAnimation(sender, 1, "switch@michael@sitting", "idle");
                        break;
                    case 6:
                        API.playPlayerAnimation(sender, 1, "mp_army_contact", "idle");
                        break;
                    default:
                        break;
                }
            }
        }

        [Command("gir", "/gir")]
        public void EnterBuilding(Client sender)
        {

            Task.Run(() =>
            {
                BuildingManager.OnBuilding(sender);
            });

            for (int i = 0; i < (EntranceManager.MarkersOnMap.Count > db_Houses.CurrentHousesDict.Count ? EntranceManager.MarkersOnMap.Count : db_Houses.CurrentHousesDict.Count); i++)
            {
                if (EntranceManager.MarkersOnMap.Count > i)
                {
                    if (sender.dimension == db_Entrances.currentEntrances.Items[i].EntranceDimension && Vector3.Distance(sender.position, db_Entrances.currentEntrances.Items[i].EntrancePosition) < 2)
                    {
                        sender.position = db_Entrances.currentEntrances.Items[i].InteriorPosition;
                        sender.dimension = db_Entrances.currentEntrances.Items[i].InteriorDimension;
                        return;
                    }
                    if (sender.dimension == db_Entrances.currentEntrances.Items[i].InteriorDimension && (Vector3.Distance(sender.position, db_Entrances.currentEntrances.Items[i].InteriorPosition) < 2))
                    {
                        sender.position = db_Entrances.currentEntrances.Items[i].EntrancePosition;
                        sender.dimension = db_Entrances.currentEntrances.Items[i].EntranceDimension;
                        return;
                    }
                }
                if (db_Houses.CurrentHousesDict.Count > i)
                {

                    var _home = db_Houses.CurrentHousesDict.Values.ToList()[i];
                    if (!_home.IsInBuilding && sender.dimension == _home.EntranceDimension && Vector3.Distance(sender.position, _home.EntrancePosition) < 1.5)
                    {
                        if (!_home.IsLocked)
                        {
                            sender.dimension = _home.InteriorDimension;
                            sender.position = _home.InteriorPosition;
                            sender.rotation = _home.InteriorRotation;
                            return;
                        }
                        else
                        {
                            API.sendNotificationToPlayer(sender, "~r~Bu kapı kilitli!", true);
                        }
                    }
                    if (sender.dimension == _home.InteriorDimension && Vector3.Distance(sender.position, _home.InteriorPosition) < 1.5)
                    {
                        if (!_home.IsLocked)
                        {
                            sender.dimension = _home.EntranceDimension;
                            sender.position = _home.EntrancePosition;
                            sender.rotation = _home.EntranceRotation;
                            return;
                        }
                        else
                        {
                            API.sendNotificationToPlayer(sender, "~r~Bu kapı kilitli!", true);
                        }
                    }
                }
            }
        }

        [Command("karakter", "/karakter")]
        public void Character(Client sender)
        {
            var _inventory = (Inventory)API.getEntityData(sender, "inventory");
            var jobId = (int)API.getEntityData(sender, "JobId");
            var FactionId = (int)API.getEntityData(sender, "FactionId");
            var _level = (int)API.getEntityData(sender, "Level");
            var _playingMinutes = (int)API.getEntityData(sender, "playingMinutes");
            //var _phoneNumbers = (List<string>) API.getEntityData(sender, "PhoneNumbers");


            API.sendChatMessageToPlayer(sender,
               "__________" + API.getEntityData(sender, "CharacterName") + "__________\n" +
               "Meslek: ~y~" + JobManager.ToJobName(jobId) + " ~s~| Oluşum: ~y~" + FactionManager.ToFactionName(FactionId) + "~s~\n" +
               (FactionId > 0 ? "Rütbe: ~y~" + db_FactionRanks.GetRank(FactionId, API.getEntityData(sender, "FactionRank")).RankName + "\n" : "") +
               "Metal Parçalar: ~y~" + _inventory.MetalParts + " ~s~Diğer Parçalar: ~y~" + _inventory.OtherParts + "~s~\n" +
               "Toplam dakikalar: ~y~" + _playingMinutes + " ~s~| Seviye: ~y~" + _level + "~s~ Cinsiyet: ~s~" + (Convert.ToBoolean(API.getEntityData(sender, "Gender")) == true ? "Erkek" : "Kadın")
                );

        }

        //   [Command("login","/login [Şifre]")]
        public void Login(Client sender, string _password)
        {
            if (!db_Accounts.IsPlayerLoggedIn(sender))
            {
                if (db_Accounts.TryLoginPlayer(sender, _password))
                {
                    db_Accounts.LoadPlayerAccount(sender);
                    API.sendChatMessageToPlayer(sender, "~g~Başarıyla giriş yaptınız.");
                    sender.position = API.getEntityData(sender, "LastPosition");
                    sender.setSkin(API.getEntityData(sender, "Skin"));
                    sender.health = API.getEntityData(sender, "HealthLevel");
                    sender.armor = API.getEntityData(sender, "ArmorLevel");


                    int money = API.getEntityData(sender, "Money");
                    float _Hunger = Convert.ToInt32(API.getEntityData(sender, "Hunger"));
                    float _Thirsty = Convert.ToInt32(API.getEntityData(sender, "Thirsty"));
                    API.triggerClientEvent(sender, "update_hungerthirsty", _Hunger, _Thirsty);
                    API.triggerClientEvent(sender, "update_money_display", money);
                }
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "Zaten giriş yapmışsınız.");
            }
        }


        //    [Command("kayit", "/kayit ~y~[Şifre] [Şifre Tekrar]")]
        public void Kayit(Client sender, string _password, string _passwordConfirm)
        {
            API.consoleOutput("Register matodu çalıştı.");
            if (db_Accounts.IsPlayerLoggedIn(sender))
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Zaten giriş yapmışsınız.!");
                return;
            }

            if (db_Accounts.DoesAccountExist(sender.socialClubName))
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bu social Club üyeliğiniz ile kayıt yapmışsınız!");
                return;
            }

            db_Accounts.CreatePlayerAccount(sender, _password);
            // API.sendChatMessageToPlayer(sender, "~g~Hesap Oluşturuldu! ~w~Hemen ~y~/login [password] ~w~ ile giriş yapabilirsiniz.");
        }

        [Command("para", "/para")]
        public void Money(Client sender)
        {
            int money = API.getEntityData(sender, "Money");
            API.sendChatMessageToPlayer(sender, "Paranız : ~g~" + money.ToString());
            API.triggerClientEvent(sender, "update_money_display", money);
        }

        [Command("anim", "~y~USAGE: ~w~/anim [animasyon]\n" +
                    "~y~USAGE: ~w~/anim list - Animasyonların listesi.\n" +
                    "~y~USAGE: ~w~/anim stop - Animsyonu durdurmak için.")]
        public void SetPlayerAnim(Client sender, string animation)
        {
            if (!Animation.IsPlayerAvailableForAnim(sender))
                return;
            if (animation == "list")
            {
                string helpText = AnimationList.Aggregate(new StringBuilder(),
                                (sb, kvp) => sb.Append(kvp.Key + " "), sb => sb.ToString());
                API.sendChatMessageToPlayer(sender, "~b~Available animations:");
                var split = helpText.Split();
                for (int i = 0; i < split.Length; i += 5)
                {
                    string output = "";
                    if (split.Length > i)
                        output += split[i] + " ";
                    if (split.Length > i + 1)
                        output += split[i + 1] + " ";
                    if (split.Length > i + 2)
                        output += split[i + 2] + " ";
                    if (split.Length > i + 3)
                        output += split[i + 3] + " ";
                    if (split.Length > i + 4)
                        output += split[i + 4] + " ";
                    if (!string.IsNullOrWhiteSpace(output))
                        API.sendChatMessageToPlayer(sender, "~b~>> ~w~" + output);


                }
            }
            else if (animation == "stop")
            {
                if (API.hasEntityData(sender, "Cuffed") || API.hasEntityData(sender, "Cuffed") == true)
                {
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Kelepçeliyken bunu yapamazsınız.");
                    return;
                }
                if (API.getEntityData(sender, "DeadSeconds") > 0)
                {
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Yaralıyken bunu yapamazsınız.");
                    return;
                }
                API.stopPlayerAnimation(sender);
                API.resetEntityData(sender, "Handsup");
            }
            else if (!AnimationList.ContainsKey(animation))
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Animasyon bulunamadı!");
            }
            else
            {
                var flag = 0;
                if (animation == "handsup") flag = 1;
                API.setEntityData(sender, "Handsup", true);
                API.playPlayerAnimation(sender, flag, AnimationList[animation].Split()[0], AnimationList[animation].Split()[1]);
            }
        }

        [Command("teslimol", Alias = "handsup")]
        public void Handsup(Client sender)
        {
            API.setEntityData(sender, "Handsup", true);
            API.playPlayerAnimation(sender, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "missminuteman_1ig_2", "handsup_base");
        }

        [Command("satinal", "/satinal")]
        public void BuyFromShop(Client sender)
        {
            foreach (var item in db_Shops.CurrentShopsList.Keys)
            {
                if (Vector3.Distance(sender.position, item.Position) <= item.Range + 2)
                {
                    // var _list = db_Items.GameItems.Items.Where(x => item.SaleItemList.Select(s => s.GameItemId).Contains(x.ID));

                    //Tuple<int[], string[], string[],int[]> shop = new Tuple<int[], string[], string[],int[]>(
                    //    _list.Select(s=>s.ID).ToArray(),
                    //    _list.Select(s=>s.Name).ToArray(),
                    //    _list.Select(s=>s.Description).ToArray(),
                    //    item.SaleItemList.Select(s=>s.Price).ToArray()
                    //    );

                    //API.triggerClientEvent(sender, "shop_open", _list.Count(),shop.Item2,shop.Item3,shop.Item1,shop.Item4);
                    List<string> NameList = new List<string>();
                    List<string> DescriptionList = new List<string>();
                    foreach (var shopItem in item.SaleItemList)
                    {
                        var gameItem = db_Items.GetItemById(shopItem.GameItemId);
                        if (gameItem != null)
                        {
                            NameList.Add(gameItem.Name + " | $" + shopItem.Price);
                            DescriptionList.Add(gameItem.Description);
                        }
                    }
                    API.triggerClientEvent(sender, "shop_open", NameList.Count, NameList.ToArray(), DescriptionList.ToArray(), item.ShopId);

                    return;

                    //foreach (var item2 in _list)
                    //{
                    //    NameList.Add(item2.Name + " | $" + item.SaleItemList.FirstOrDefault(x => x.GameItemId == item2.ID).Price);
                    //    DescriptionList.Add(item2.Description);
                    //}


                }
            }
        }

        [Command("paraver", "/paraver [OyuncuID] [Miktar]")]
        public void GiveMoney(Client sender, int _Id, int Value)
        {
            var player = db_Accounts.GetPlayerById(_Id);
            if (player != null)
            {
                if (Vector3.Distance(player.position, sender.position) > 3) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Oyuncu yanınızda olmalı."); return; }
                int giverMoney = API.getEntityData(sender, "Money"), takerMoney = API.getEntityData(player, "Money");
                if (Value <= giverMoney)
                {
                    InventoryManager.AddMoneyToPlayer(sender, -1 * Value);
                    InventoryManager.AddMoneyToPlayer(player, Value);

                    //giverMoney -= Value;
                    //takerMoney += Value;
                    //API.setEntityData(sender, "Money", giverMoney);
                    //API.setEntityData(player, "Money", takerMoney);
                    //API.triggerClientEvent(sender, "update_money_display", giverMoney);
                    //API.triggerClientEvent(player, "update_money_display", takerMoney);
                    rpgMgr.Me(sender, " bir miktar para çıkarır ve " + db_Accounts.GetPlayerCharacterName(player) + " adlı kişiye verir.");
                }
                else
                {
                    API.sendChatMessageToPlayer(sender, "~r~Maalesef üzerinizde ~s~" + Value + " ~r~bulunmuyor.");
                }

            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Oyuncu bulunamadı.");
            }
        }

        [Command("kimlik", "/kimlik [cikar/goster]", GreedyArg = true)]
        public void IdentityCard(Client sender, string type)
        {
            if ("cikar".StartsWith(type.ToLower()))
            {
                if (String.IsNullOrEmpty(API.getEntityData(sender, "Origin")))
                {
                    foreach (var itemLicense in db_LicensePoints.CurrentLicenseTakings.Item1)
                    {
                        if (itemLicense.LicenseType == 1 && Vector3.Distance(itemLicense.Position, sender.position) < 4)
                        {
                            API.sendChatMessageToPlayer(sender, "Yetkili: ~y~Doğum tarihiniz nedir?  ((gg/aa/yyyy))");
                            API.setEntityData(sender, IDENTITY_B, true);
                            return;
                        }
                    }
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Herhangi bir kimlik çıkarma noktasında değilsiniz.");
                }
                else
                {
                    API.sendChatMessageToPlayer(sender, "Yetkili: ~y~Sistemde kimliğinizin olduğunu görüyorum. \n~y~ Eğer kaybettiyseniz ceza ödeyerek yenisini alabilirsiniz.");
                }
            }
            else
            if (type.StartsWith("goster"))
            {
                #region Goster
                var splitted = type.Split(' ');
                if (splitted.Length < 2) { API.sendChatMessageToPlayer(sender, "/kimlik goster [Oyuncu ID]"); return; }
                try
                {
                    var playerID = Convert.ToInt32(splitted[1]);
                    var player = db_Accounts.GetPlayerById(playerID);
                    if (player == null) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Böyle bir oyuncu yok."); return; }

                    var licenses = InventoryManager.GetItemsFromPlayerInventory(sender, ItemType.License).Where(x => x.Item1.Value_0 == "1").ToList();
                    if (licenses.Count > 1)
                    {
                        try
                        {
                            var identityInfo = db_Accounts.GetOfflineUserDatas(licenses[Convert.ToInt32(splitted[2])].Item2.SpecifiedValue);

                            API.shared.sendChatMessageToPlayer(player, "_____" + identityInfo.CharacterName + "_____\n" +
                                "~y~Cinsiyet: ~s~" + (identityInfo.Gender == true ? "Erkek" : "Kadın") + " |~y~ Doğum Tarihi: ~s~" + ((DateTime)identityInfo.BirthDate).ToString("dd/MM/yyyy") + " \n" +
                                "~y~Memleket: ~s~" + identityInfo.Origin);

                            rpgMgr.Me(sender, " adlı kişi, " + API.shared.getEntityData(player, "CharacterName") + " adlı kişiye kimliğini gösterir.", 5);

                        }
                        catch (Exception ex)
                        {
                            if (ex.GetType() == typeof(IndexOutOfRangeException) || ex.GetType() == typeof(ArgumentOutOfRangeException))
                            {
                                API.sendChatMessageToPlayer(sender, "~y~Envanterinizde birden fazla kimlik var. Göstermek istediğinizin numarasını yazın.");
                                API.sendChatMessageToPlayer(sender, "~s~/kimlik goster [OyuncuID] ~y~[KimlikNumarası]");
                                string strIdentities = ""; int index = 0;
                                foreach (var item in licenses)
                                {
                                    strIdentities += "~y~" + index + " ~s~- " + (String.IsNullOrEmpty(item.Item2.SpecifiedValue) ? "Belirsiz" : db_Accounts.GetOfflineUserDatas(item.Item2.SpecifiedValue).CharacterName) + "\n";
                                    index++;
                                }
                                API.shared.sendChatMessageToPlayer(sender, strIdentities);
                            }
                            if (ex.GetType() == typeof(FormatException))
                            {
                                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Kimlik numarası sayı olmalıydı.");
                            }
                        }
                    }
                    else
                    {
                        if (licenses.Count == 0) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Üzerinizde herhangi bir kimlik yok."); return; }
                        var identityInfo = db_Accounts.GetOfflineUserDatas(licenses.FirstOrDefault().Item2.SpecifiedValue);
                        if (identityInfo == null) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Üzerinizde herhangi bir kimlik yok."); return; }
                        API.shared.sendChatMessageToPlayer(player, "_____" + identityInfo.CharacterName + "_____\n" +
                            "~y~Cinsiyet: ~s~" + (identityInfo.Gender == true ? "Erkek" : "Kadın") + " |~y~ Doğum Tarihi: ~s~" + ((DateTime)identityInfo.BirthDate).ToString("dd/MM/yyyy") + " \n" +
                            "~y~Memleket: ~s~" + identityInfo.Origin);

                        rpgMgr.Me(sender, $" elini cebine atıp bir kimlik çıkartır ve {db_Accounts.GetPlayerCharacterName(player)} adlı kişiye gösterir.", 5);
                        //rpgMgr.Me(sender, " adlı kişi, " + API.shared.getEntityData(player, "CharacterName") + " adlı kişiye kimliğini gösterir.", 5);

                    }

                    return;

                }
                catch (Exception ex)
                {

                    if (ex.GetType() == typeof(FormatException))
                    {
                        API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Oyuncu ID'si sayı olmalı!");
                    }
                    else
                        API.consoleOutput("KIMLIK GOSTER | HATA: " + ex.ToString());
                    return;
                }
                #endregion
            }
        }

        [Command("saglikraporu", "/saglikraporu cikar")]
        public void HealthReport(Client sender)
        {
            var takingPointPed = db_LicensePoints.CurrentLicenseTakings.Item1.FirstOrDefault(x => x.LicenseType == -1);
            if (Vector3.Distance(takingPointPed.Position, sender.position) < 3 && sender.dimension == sender.dimension)
            {
                int money = API.getEntityData(sender, "Money");
                if (money >= takingPointPed.Price)
                {
                    if (sender.health < 100)
                    {
                        API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Sağlığınız yerinde olmadığı için sağlık belgesi alamazsınız.");
                        return;
                    }

                    if (InventoryManager.AddItemToPlayerInventory(sender, new ClientItem
                    {
                        Count = 1,
                        Equipped = false,
                        SpecifiedValue = sender.socialClubName,
                        ItemId = db_Items.GameItems.Values.FirstOrDefault(x => x.Type == ItemType.License && x.Value_0 == "-1").ID
                    }))
                    {
                        money -= takingPointPed.Price;
                        API.setEntityData(sender, "Money", money);
                        Clients.ClientManager.UpdateMoneyDisplay(sender, money);
                        API.sendChatMessageToPlayer(sender, "~y~Sağlık raporunuz envanterinize eklendi.");
                    }
                    else
                    {
                        API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bunu alabilmek için envanterinizde yeterli yer yok.");
                        return;
                    }
                }
                else
                {
                    API.sendChatMessageToPlayer(sender, "~s~HATA: ~s~Sağlık raporunu almak için yeterli paranız yok. Gerekli olan :~g~" + takingPointPed.Price + "$");
                }
            }
            else
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Hastanede değilsiniz.");
        }

        public enum identityType
        {
            Birthdate,
            Origin,
        };
        public static void IdentityComplete(Client player, identityType type, string parameter)
        {
            switch (type)
            {
                case identityType.Birthdate:
                    API.shared.consoleOutput("Count('/'): " + parameter.Count(x => x == '/'));
                    if (parameter.Count(x => x == '/') == 2)
                    {
                        var splitted = parameter.Split('/');
                        if (splitted[0].Length == 2 && splitted[1].Length == 2 && splitted[2].Length == 4)
                        {
                            DateTime d = new DateTime();
                            try
                            {
                                d = DateTime.ParseExact(parameter, "dd/MM/yyyy", System.Globalization.CultureInfo.InvariantCulture, System.Globalization.DateTimeStyles.None);
                            }
                            catch (Exception ex)
                            {
                                if (ex.GetType() == typeof(FormatException))
                                {
                                    API.shared.sendChatMessageToPlayer(player, "~r~HATA: ~s~Doğum tarihi hatalı girildi.");
                                }
                                API.shared.consoleOutput(LogCat.Warn, ex.ToString());
                                return;
                            }
                            API.shared.setEntityData(player, "BirthDate", d);
                            API.shared.resetEntityData(player, IDENTITY_B);
                            API.shared.setEntityData(player, IDENTITY_O, true);
                            API.shared.sendChatMessageToPlayer(player, "Yetkili: ~y~Doğum yerinizi söyleyebilir misiniz?");
                            return;
                        }
                        else
                        {
                            API.shared.sendChatMessageToPlayer(player, "~r~HATA: ~s~Lütfen geçerli bir doğum tarihi girin. (gg/aa/yyyy ) ");
                            return;
                        }
                    }
                    else
                    {
                        API.shared.sendChatMessageToPlayer(player, "~r~HATA: ~s~Lütfen geçerli bir doğum tarihi girin. (gg/aa/yyyy ) ");
                    }
                    break;
                case identityType.Origin:

                    int money = API.shared.getEntityData(player, "Money");
                    var licenseTaking = db_LicensePoints.CurrentLicenseTakings.Item1.FirstOrDefault(x => x.LicenseType == 1);

                    if (money >= licenseTaking.Price)
                    {
                        API.shared.setEntityData(player, "Origin", parameter);
                        API.shared.resetEntityData(player, IDENTITY_O);
                        var _inventory = (Inventory)API.shared.getEntityData(player, "inventory");
                        money -= licenseTaking.Price;
                        API.shared.setEntityData(player, "Money", money);
                        API.shared.triggerClientEvent(player, "update_money_display", money);
                        API.shared.sendNotificationToPlayer(player, "~r~-" + licenseTaking.Price + "$");
                        _inventory.ItemList.Add(new ClientItem
                        {
                            ItemId = db_Items.GameItems.Values.FirstOrDefault(x => x.Type == ItemType.License && x.Value_0 == "1").ID,
                            Count = 1,
                            SpecifiedValue = player.socialClubName
                        });
                        int mission = API.shared.getEntityData(player, "Mission");
                        if (mission <= 1)
                        {
                            API.shared.setEntityData(player, "Mission", 1);
                        }
                        RPGManager rpgMgr = new RPGManager();
                        rpgMgr.Me(player, " elini parmak izi okuyucuya uzatır ve parmak izinin taranmasını sağlar.");
                        string _generatedFingerPrint = Guid.NewGuid().ToString().Substring(0, 8);
                        API.shared.consoleOutput("FingerPrint" + _generatedFingerPrint);
                        API.shared.setEntityData(player, "FingerPrint", _generatedFingerPrint);
                        API.shared.sendChatMessageToPlayer(player, "Yetkili: ~y~Buyrun, kimliğiniz.  ~w~(( Kimliğiniz envanterinize eklendi.))");
                        Clients.ClientManager.RemoveMissionMarker(player);
                        API.shared.setEntityData(player, "Mission", 1);
                        TriggerUserMission(player);
                        db_Accounts.SavePlayerAccount(player);
                    }
                    else
                    {
                        API.shared.sendChatMessageToPlayer(player, "~r~UYARI: ~s~Bunun için yeterli paranız yok. Ödenmesi gereken: " + licenseTaking.Price);
                    }

                    break;
                default:
                    break;
            }
        }

        public static void TriggerUserMission(Client sender)
        {
            if (API.shared.hasEntityData(sender, "Mission"))
            {
                int missionNumber = API.shared.getEntityData(sender, "Mission");
                switch (missionNumber)
                {
                    case 0:
                        var LicenseTaking = db_LicensePoints.CurrentLicenseTakings.Item1.FirstOrDefault(x => x.LicenseType == 1);
                        if (LicenseTaking != null)
                        {
                            if (LicenseTaking.Dimension == 0)
                            {
                                Clients.ClientManager.ShowMissionMarker(sender, LicenseTaking.Position.X, LicenseTaking.Position.Y, LicenseTaking.Position.Z, 0);
                            }
                            else
                            {
                                int i = 0; int _nearestIndex = 0; float lastDistance = float.MaxValue;
                                foreach (var itemEntrances in db_Entrances.currentEntrances.Items)
                                {
                                    var dist = Vector3.Distance(itemEntrances.InteriorPosition, LicenseTaking.Position);
                                    if (dist < lastDistance && itemEntrances.InteriorDimension == LicenseTaking.Dimension)
                                    {
                                        _nearestIndex = i;
                                    }
                                    lastDistance = dist;
                                    i++;
                                }
                                var pos = db_Entrances.currentEntrances.Items[_nearestIndex].EntrancePosition;
                                Clients.ClientManager.ShowMissionMarker(sender, pos.X, pos.Y, pos.Z, 0);
                            }
                        }
                        else
                        {
                            API.shared.consoleOutput("Kimlik alma noktası eklenmemiş. [ LicenseTaking  (TypeID:1)]");
                        }

                        break;

                    case 1:


                        var shop = db_Shops.GetShop(18);
                        if (shop != null)
                        {
                            var pos = shop.Position;
                            Clients.ClientManager.ShowMissionMarker(sender, pos.X, pos.Y, pos.Z, 1);
                            return;
                        }
                        else
                        {
                            var skinItemList = db_Items.GameItems.Values.Where(w => w.Type == ItemType.Skin).Select(s => s.ID);
                            foreach (var itemShops in db_Shops.CurrentShopsList)
                            {
                                foreach (var itemSaleList in itemShops.Key.SaleItemList)
                                {
                                    if (skinItemList.Contains(itemSaleList.GameItemId))
                                    {
                                        Clients.ClientManager.ShowMissionMarker(sender, itemShops.Key.Position.X, itemShops.Key.Position.Y, itemShops.Key.Position.Z, 1);
                                        return;
                                    }
                                }
                            }
                        }

                        break;
                    case 2:
                        if (db_Shops.CurrentShopsList.Count > 0)
                        {
                            var phoneItemList = db_Items.GameItems.Values.Where(w => w.Type == ItemType.Phone).Select(s => s.ID);
                            foreach (var itemShops in db_Shops.CurrentShopsList)
                            {
                                foreach (var itemSaleList in itemShops.Key.SaleItemList)
                                {
                                    if (phoneItemList.Contains(itemSaleList.GameItemId))
                                    {
                                        Clients.ClientManager.ShowMissionMarker(sender, itemShops.Key.Position.X, itemShops.Key.Position.Y, itemShops.Key.Position.Z, 2);
                                        return;
                                    }
                                }
                            }
                        }
                        break;
                    case 3:
                        if (db_PhoneOperatorShop.CurrentOperatorShop.Item1.Count > 0)
                        {
                            int i = 0; int _nearestIndex = 0; float lastDistance = float.MaxValue;
                            foreach (var itemOpShop in db_PhoneOperatorShop.CurrentOperatorShop.Item1)
                            {
                                var dist = Vector3.Distance(sender.position, itemOpShop.Position);
                                if (dist < lastDistance)
                                {
                                    _nearestIndex = i;
                                }
                                lastDistance = dist;
                                i++;
                            }
                            var pos = db_PhoneOperatorShop.CurrentOperatorShop.Item1[_nearestIndex].Position;
                            Clients.ClientManager.ShowMissionMarker(sender, pos.X, pos.Y, pos.Z, 3);
                        }

                        break;
                    default:

                        break;
                }
            }
        }

        [Command("?", "/? [Sormak istediğiniz soru cümlesi]", GreedyArg = true)]
        public void AskQuestion(Client sender, string questionSentence)
        {
            int asdkId = db_Reports.AddReport(new Models.Report
            {
                OwnerSocialClubID = sender.socialClubName,
                RegisterDate = DateTime.Now,
                ReportText = questionSentence,
                Type = ReportType.Question
            });
            foreach (var item in API.getAllPlayers())
            {
                if (API.getEntityData(item, "AdminLevel") >= 1)
                {
                    API.sendChatMessageToPlayer(item, "~b~[?] - ~s~~h~" + asdkId + " - " + questionSentence + " ((/cevapla " + asdkId + "))");
                }
            }
            API.sendChatMessageToPlayer(sender, "~b~[?] ~s~~h~Sorduğunuz soru iletildi.");
        }
        [Command("rapor", "/rapor [Karşılaştığınız problem]", GreedyArg = true)]
        public void ReportProblem(Client sender, string reportSentence)
        {
            int reportId = db_Reports.AddReport(new Models.Report
            {
                OwnerSocialClubID = sender.socialClubName,
                RegisterDate = DateTime.Now,
                ReportText = reportSentence,
                Type = ReportType.Report
            });
            foreach (var item in API.getAllPlayers())
            {
                if (API.getEntityData(item, "AdminLevel") >= 1)
                {
                    API.sendChatMessageToPlayer(item, "~r~[!] ~s~~h~- " + reportId + " - (" + sender.nametag + ") - " + reportSentence + " ((/accept " + reportId + "))" + " ((/reject " + reportId + " ))");
                }
            }

            API.sendChatMessageToPlayer(sender, "~r~[!] ~s~~h~- Raporunuz iletildi.");
        }

        // [Command("attachrpg")]
        public void attachtest1(Client sender)
        {
            var prop = API.createObject(API.getHashKey("w_lr_rpg"), API.getEntityPosition(sender.handle), new Vector3());
            API.attachEntityToEntity(prop, sender.handle, "SKEL_SPINE3",
                new Vector3(-0.13f, -0.231f, 0.07f), new Vector3(0f, 200f, 10f));
        }
    }
}
