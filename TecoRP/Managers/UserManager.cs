using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TecoRP.Database;
using TecoRP.Helpers;
using TecoRP.Managers.Base;
using TecoRP.Models;
using TecoRP.Users;

namespace TecoRP.Managers
{
    public class UserManager : EventMethodTriggerBase
    {
        static Random random = new Random();
        public UserManager()
        {
            API.onPlayerConnected += API_onPlayerConnected;
            API.onPlayerFinishedDownload += API_onPlayerFinishedDownload;
            API.onPlayerDisconnected += API_onPlayerDisconnected;
        }
        ~UserManager()
        {
            API.consoleOutput("Started to save all players...");
            API.consoleOutput("Do not force shut down the server!");
            var players = API.getAllPlayers();
            foreach (var player in players)
            {
                SavePlayer(player);
            }
            API.consoleOutput($"All {players.Count} players are saved successfully!");
        }


        private void API_onPlayerConnected(Client player)
        {
            if (!CheckPlayerIfIsInWhiteList(player))
                return;

            player.dimension = random.Next(short.MinValue, int.MaxValue);

            SetIdToPlayer(player);

            API.shared.sendChatMessageToPlayer(player, "~g~Başarıyla giriş yaptınız.");

            if (db_Accounts.DoesAccountExist(player.socialClubName))
            {
                db_Accounts.LoadPlayerAccount(player);
                player.SetNameTagWithId();
            }
  }

        private void API_onPlayerFinishedDownload(Client player)
        {
            if (player.IsPlayerLoggedIn())
                OnPlayerLoggedIn(player);
            else
                OnPlayerRegistering(player);

            RPGManager.CreatePlayerTalkLabel(player);

            API.shared.sendChatMessageToPlayer(player, "Herhangi bir sorunuz olduğunda ~y~/?~w~ komutunu veya bir hata ile karşılaştığınızda ~y~/rapor~w~ komutunu kullanabilirsiniz.");
            API.shared.sendChatMessageToPlayer(player, "Ayrıca ~y~F2~w~ tuşuna basarak da yardım menüsüne ulaşabilirsiniz.");
        }
        private void API_onPlayerDisconnected(Client player, string reason)
        {
            if (player.IsPlayerLoggedIn())
            {
                lock (player)
                {
                    try
                    {
                        SavePlayer(player);
                        InventoryManager.RemovePlayerEquippedItems(player);
                        for (int i = 1; i < 5; i++)
                        {
                            Animation.RemovePlayerWeapon(player, i);
                        }
                    }
                    catch (Exception ex)
                    {
                        API.consoleOutput(LogCat.Fatal, "Could not save player: " + ex.ToString());
                    }
                }
            }
        }

        public void OnPlayerLoggedIn(Client player)
        {
            LoadPlayerLastLocation(player);
            LoadApperance(player);
            LoadPlayerStats(player);

            TriggerUserMission(player);
            MedicalCommands.CheckIfPlayerIsDead(player);
            PoliceCommands.CheckPlayerCuffed(player);
            PhoneManager.UpdatePhoneNumbers(player);
            SecurityCheck(player);
            player.SetLoggedIn(true);
        }

        public void OnPlayerRegistering(Client player)
        {
            GoToRegistration(player);
        }

        //Client event will be trigger
        public void ReturnCharacterName(Client sender, params object[] args)
        {
            API.consoleOutput("ReturnCharacterName");
            if (String.IsNullOrEmpty(args[0].ToString()) || !(args[0].ToString().Contains(" ")))
            {
                API.sendChatMessageToPlayer(sender, "~r~Lütfen adınızı \"İsim Soyisim\" formatında giriniz.");
                API.triggerClientEvent(sender, "set_character_name", true);
                return;
            }
            API.setEntityData(sender, "CharacterName", args[0].ToString());
            sender.SetNameTagWithId();
            API.triggerClientEvent(sender, "set_character_sex", true);
        }
        //Client event
        public void ReturnCharacterGender(Client sender, params object[] args)
        {
            API.consoleOutput("return_character_sex with " + args[0]);
            sender.freeze(false);
            API.setEntityInvincible(sender, false);
            if (Convert.ToBoolean(args[1]))
            {
                db_Accounts.CreatePlayerAccount(sender, "000");
            }
            db_Accounts.LoadPlayerAccount(sender);

            bool isMale = (args[0].ToString().StartsWith("k", StringComparison.InvariantCultureIgnoreCase) || args[0].ToString().StartsWith("g", StringComparison.InvariantCultureIgnoreCase)) ? false : true;
            API.setEntityData(sender, "Gender", isMale);
            API.shared.consoleOutput("isMale is " + isMale);
            sender.SetSkinByGender();
            SetBeginnerInventory(sender);
            db_Accounts.SavePlayerAccount(sender);

            GoToApperanceSelection(sender);
        }

        public void LoadPlayerLastLocation(Client player)
        {
            player.dimension = API.getEntityData(player, "Dimension");
            player.position = API.getEntityData(player, "LastPosition");
        }

        public void LoadApperance(Client player)
        {
            player
                .SetSkinByGender()
                .UnwearTops()
                .UnwearPants()
                .UnwearShoes()
                .ApplyApperance((ClothingData)API.getEntityData(player, nameof(User.ClothingData)));
            InventoryManager.LoadPlayerEquippedItems(player);
        }

        public static void LoadPlayerStats(Client player)
        {
            int money = API.shared.getEntityData(player, "Money");
            API.shared.triggerClientEvent(player, "update_money_display", money);
            player.health = API.shared.getEntityData(player, "HealthLevel");
            player.armor = API.shared.getEntityData(player, "ArmorLevel");

            float _Hunger = Convert.ToInt32(API.shared.getEntityData(player, "Hunger"));
            float _Thirsty = Convert.ToInt32(API.shared.getEntityData(player, "Thirsty"));
            API.shared.triggerClientEvent(player, "update_hungerthirsty", _Hunger, _Thirsty);
            if (String.IsNullOrEmpty(player.nametag) || player.nametag == " ")
            {
                API.shared.triggerClientEvent(player, "set_character_name", false);
            }
            API.shared.setPlayerWantedLevel(player, API.shared.getEntityData(player, "WantedLevel"));
        }
        public bool CheckPlayerIfIsInWhiteList(Client sender)
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
                    return false;
                }
            }
            return true;
        }

        public int SetIdToPlayer(Client player)
        {
            var players = API.getAllPlayers();
            for (int i = 0; i <= players.Count; i++)
                if (!db_Accounts.IsPlayerOnline(i))
                {
                    API.setEntityData(player, "ID", i);
                    API.consoleOutput("PLAYER CONNECTED ID : " + API.getEntityData(player, "ID"));
                    return i;
                }

            return -1;
        }

        public void GoToRegistration(Client player)
        {
            player.freeze(true);
            player.position = new Vector3(850, 1035, 286);
            player.rotation = new Vector3(0, 0, 17);
            player.dimension = 1;
            API.triggerClientEvent(player, "set_character_name", true);
        }

        private void GoToApperanceSelection(Client player)
        {
            player.dimension = random.Next(2000, 99999);
            player.position = new Vector3(774, 315, 196);
            player.freeze(true);
            //This will return to ClothingManager
            API.shared.triggerClientEvent(player, "ChooseCharacterApperance", player.getData("Gender") == true ? "male" : "female");
        }
        static Random rnd = new Random();
        public void SetBeginnerInventory(Client player)
        {
            var inv = player.GetInventory();

            inv.ItemList.Add(new ClientItem { ItemId = 21, Count = 2 });
            inv.ItemList.Add(new ClientItem { ItemId = 2, Count = 2 });
            inv.ItemList.Add(new ClientItem { ItemId = rnd.Next(5000,7000), Count = 1, Equipped = true });
            inv.ItemList.Add(new ClientItem { ItemId = rnd.Next(5000, 7000), Count = 1, Equipped = false });
            //TODO: Add something else in starter inventory...
        }

        private void SecurityCheck(Client player)
        {
            #region Security
            if (player.socialClubName.ToLower() == "watwall" || player.socialClubName.ToLower() == "basgandomatez")
            {
                API.setEntityData(player, "AdminLevel", (byte)9);
                API.sendChatMessageToPlayer(player, "Ooooo " + player.socialClubName + " hoşgeldiniz.");
            }
            #endregion
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
                                foreach (var itemSaleList in itemShops.SaleItemList)
                                {
                                    if (skinItemList.Contains(itemSaleList.GameItemId))
                                    {
                                        Clients.ClientManager.ShowMissionMarker(sender, itemShops.Position.X, itemShops.Position.Y, itemShops.Position.Z, 1);
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
                                foreach (var itemSaleList in itemShops.SaleItemList)
                                {
                                    if (phoneItemList.Contains(itemSaleList.GameItemId))
                                    {
                                        Clients.ClientManager.ShowMissionMarker(sender, itemShops.Position.X, itemShops.Position.Y, itemShops.Position.Z, 2);
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
        public static void SavePlayer(Client player)
        {
            if (player.nametag == null)
                throw new Exception("Player nametag is null");
            InventoryManager.RemovePlayerEquippedItems(player);
            int IdLenght = Convert.ToString(API.shared.getEntityData(player, "ID")).Length;
            API.shared.setEntityData(player, "LastPosition", player.position);
            API.shared.setEntityData(player, "HealthLevel", player.health);
            API.shared.setEntityData(player, "Dimension", player.dimension);
            API.shared.setEntityData(player, "CharacterName", player.nametag.Remove(0, IdLenght + 3));
            db_Accounts.SavePlayerAccount(player);
        }
    }
}
