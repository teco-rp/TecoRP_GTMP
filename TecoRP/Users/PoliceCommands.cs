using GrandTheftMultiplayer.Server.API;
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

namespace TecoRP.Users
{
    public class PoliceCommands : Script
    {
        [Flags]
        public enum AnimationFlags
        {
            Loop = 1 << 0,
            StopOnLastFrame = 1 << 1,
            OnlyAnimateUpperBody = 1 << 4,
            AllowPlayerControl = 1 << 5,
            Cancellable = 1 << 7
        }
        public static List<PhoneTicket> currentTickets = new List<PhoneTicket>();

        RPGManager rpgMgr = new RPGManager();

        [Command("kelepcele", "/kelepcele [OyuncuID]")]
        public void Cuff(Client sender, int targetPlayerId)
        {
            if (API.getEntityData(sender, "FactionId") != 1) { API.sendChatMessageToPlayer(sender, "~r~Bunun için polis olmalısınız."); return; }
            try { Convert.ToInt32(targetPlayerId); } catch (Exception) { API.sendChatMessageToPlayer(sender, "~y~Girdiğiniz parametre ~w~ID~y~ ve~w~ sayı ~y~olmalıdır."); return; }
            var player = db_Accounts.FindPlayerById(Convert.ToInt32(targetPlayerId));
            if (player == null) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Oyuncu bulunamadı."); return; }
            if (Vector3.Distance(player.position, sender.position) > 3) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Oyuncu yanınızda olmalı."); return; }
            if (API.getEntityData(player, "ID") == Convert.ToInt32(targetPlayerId))
            {
                if (API.hasEntityData(player, "Handsup") && API.getEntityData(player, "Handsup") == true)
                {
                    CuffPlayer(player);

                    rpgMgr.Me(sender, $" kelepçesini alır ve {db_Accounts.GetPlayerCharacterName(player)} adlı kişinin ellerini arkasında birleştirip kelepçeler .");
                }
                else
                {
                    API.sendChatMessageToPlayer(sender, "~y~Kişi önce teslim olmalıdır. ~w~( Ellerini kaldırmalı. )");
                }
                return;
            }

        }


        [Command("kelepcecikar", "/kelepcecikar [Oyuncu Id]")]
        public void UnCuffPlayer(Client sender, string identity)
        {
            if (API.getEntityData(sender, "FactionId") != 1) { API.sendChatMessageToPlayer(sender, "~r~Bunun için polis olmalısınız."); return; }

            try { Convert.ToInt32(identity); } catch (Exception) { API.sendChatMessageToPlayer(sender, "~y~Girdiğiniz parametre ~w~ID~y~ ve~w~ sayı ~y~olmalıdır."); return; }
            var player = db_Accounts.FindPlayerById(Convert.ToInt32(identity));
            if (Vector3.Distance(sender.position, player.position) < 2)
            {
                if (API.hasEntityData(player, "Cuffed") && API.getEntityData(player, "Cuffed") == true)
                {
                    UserCommands userCmds = new UserCommands();

                    API.stopPlayerAnimation(sender);
                    API.resetEntityData(sender, "Handsup");
                    API.resetEntityData(player, "Cuffed");
                    API.stopPlayerAnimation(player);
                    rpgMgr.Me(sender, ", " + API.getEntityData(player, "CharacterName") + " adlı kişinin kelepçelerini çıkarır.");
                    return;
                }
                else
                {
                    API.sendChatMessageToPlayer(sender, "~y~Kişi kelepçeli değil.");
                }
                return;
            }

        }
        [Command("surukle", "/surukle [Oyuncu Id]")]
        public void DragPlayer(Client sender, string identity)
        {
            if (API.getEntityData(sender, "FactionId") != 1) { API.sendChatMessageToPlayer(sender, "~r~Bunun için polis olmalısınız."); return; }
            foreach (var item in API.getAllPlayers())
            {
                try { Convert.ToInt32(identity); } catch (Exception) { API.sendChatMessageToPlayer(sender, "~y~Girdiğiniz parametre ~w~ID~y~ ve~w~ sayı ~y~olmalıdır."); return; }
                if (API.getEntityData(item, "ID") == Convert.ToInt32(identity))
                {
                    if (API.hasEntityData(item, "Cuffed") && API.getEntityData(item, "Cuffed") == true)
                    {
                        if (item.isInVehicle) { API.warpPlayerOutOfVehicle(item); }

                        API.attachEntityToEntity(item, sender, null, new Vector3(0.4f, 0, 0), new Vector3(0, 0, 0));
                        rpgMgr.Me(sender, ", " + API.getEntityData(item, "CharacterName") + " adlı kişinin omzundan tutar ve sürüklemeye başlar.");
                        return;
                    }
                    else
                    {
                        API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Sürükleyebilmek için kişinin önce kelepçeli olması gerekmektedir.");
                    }
                }
            }
        }
        [Command("suruklebirak", "/suruklebirak [Oyuncu Id]")]
        public void UnDragPlayer(Client sender, string identity)
        {
            if (API.getEntityData(sender, "FactionId") != 1) { API.sendChatMessageToPlayer(sender, "~r~Bunun için polis olmalısınız."); return; }
            foreach (var item in API.getAllPlayers())
            {
                try { Convert.ToInt32(identity); } catch (Exception) { API.sendChatMessageToPlayer(sender, "~y~Girdiğiniz parametre ~w~ID~y~ ve~w~ sayı ~y~olmalıdır."); return; }
                if (API.getEntityData(item, "ID") == Convert.ToInt32(identity))
                {
                    if (API.isEntityAttachedToEntity(item, sender))
                    {
                        item.detach();
                        return;
                    }
                    else
                    {
                        API.sendNotificationToPlayer(sender, "~b~Bu kişiyi sürüklemiyorsunuz.");
                        return;
                    }
                }
            }
        }
        [Command("aracaat", "/aracaat ~y~[oyuncu Id] [Koltuk(0-4)]")]
        public void PutPlayerIntoVehicle(Client sender, string identity, int _seat)
        {
            if (API.getEntityData(sender, "FactionId") != 1) { API.sendChatMessageToPlayer(sender, "~r~Bunun için polis olmalısınız."); return; }
            foreach (var item in API.getAllPlayers())
            {
                try { Convert.ToInt32(identity); } catch (Exception) { API.sendChatMessageToPlayer(sender, "~y~Girdiğiniz parametre ~w~ID~y~ ve~w~ sayı ~y~olmalıdır."); return; }
                if (API.getEntityData(item, "ID") == Convert.ToInt32(identity))
                {
                    var _vehicle = db_Vehicles.FindNearestVehicle(sender.position);
                    if (_vehicle == null) return;
                    if (Vector3.Distance(sender.position, _vehicle.VehicleOnMap.position) < 2)
                    {
                        if (API.hasEntityData(item, "Cuffed") && API.getEntityData(item, "Cuffed") == true)
                        {
                            if (!item.isInVehicle)
                            {
                                API.setPlayerIntoVehicle(item, _vehicle.VehicleOnMap, _seat);
                                rpgMgr.Me(sender, ", " + API.getEntityData(item, "CharacterName") + " adlı kişiyi kafasından bastırarak " + _vehicle.VehicleOnMap.displayName + " adlı araca sokar.");
                                API.playPlayerAnimation(item, 1, "mp_arresting", "sit");
                                return;
                            }
                            else
                            {
                                API.sendChatMessageToPlayer(sender, "~r~Kişi zaten bir araçta.");
                                return;

                            }
                        }
                        else
                        {
                            API.sendChatMessageToPlayer(sender, "~r~Kişinin kelepçeli olması gerekli.");
                        }

                    }

                    API.sendChatMessageToPlayer(sender, "~r~Yakınlarda araç bulunamadı!");
                    return;
                }
            }
        }

        [Command("ustunuara")]
        public void SearchOnPlayer(Client sender, string identity)
        {
            foreach (var item in API.getAllPlayers())
            {
                try { Convert.ToInt32(identity); } catch (Exception) { API.sendChatMessageToPlayer(sender, "~y~Girdiğiniz parametre ~w~ID~y~ ve~w~ sayı ~y~olmalıdır."); return; }
                if (API.getEntityData(item, "ID") == Convert.ToInt32(identity) && (Vector3.Distance(sender.position, item.position) < 2))
                {
                    if (API.hasEntityData(item, "Handsup") && API.getEntityData(item, "Handsup") == true)
                    {
                        rpgMgr.Me(sender, ", " + API.getEntityData(item, "CharacterName") + " adlı kişinin üzerini aramaya başlar.");
                        #region EnvanterSüzme
                        Inventory _inventory = API.getEntityData(item, "inventory");

                        List<string> descList = new List<string>();
                        List<string> nameList = new List<string>();

                        foreach (var item2 in db_Items.GameItems.Values.Where(x => _inventory.ItemList.Select(s => s.ItemId).Contains(x.ID)))
                        {
                            var _itemProps = _inventory.ItemList.FirstOrDefault(x => x.ItemId == item2.ID);
                            nameList.Add((_itemProps.Equipped ? "*" : String.Empty) + item2.Name + " (" + _itemProps.Count + ")");
                            descList.Add(item2.Description);
                        }
                        string desc = API.getEntityData(item, "CharacterName") + " adlı kişinin eşyaları |  " + _inventory.ItemList.Count + " / " + _inventory.InventoryMaxCapacity;
                        API.triggerClientEvent(sender, "inventory_search", nameList.Count(), nameList.ToArray(), descList.ToArray(), desc,item.socialClubName);

                        #endregion
                        return;
                    }
                    else
                        API.sendChatMessageToPlayer(sender, "Kişi önce ellerini kaldırmalıdır.");
                }
            }
        }
        [Command("hapseat", "/hapseat [Oyuncu Id]")]
        public void PutPlayerOnJail(Client sender, string identity)
        {
            if (API.getEntityData(sender, "FactionId") != 1) { API.sendChatMessageToPlayer(sender, "~r~Bunun için polis olmalısınız."); return; }
            try { Convert.ToInt32(identity); } catch (Exception) { API.sendChatMessageToPlayer(sender, "~y~Girdiğiniz parametre ~w~ID~y~ ve~w~ sayı ~y~olmalıdır."); return; }
            foreach (var itemPlayer in API.getAllPlayers())
            {
                if (API.getEntityData(itemPlayer, "ID") == Convert.ToInt32(identity) && Vector3.Distance(sender.position, itemPlayer.position) < 2)
                {
                    if (API.hasEntityData(itemPlayer, "Cuffed"))
                    {
                        foreach (var itemArrest in db_Arrests.currentArrests.Item1)
                        {
                            if (Vector3.Distance(sender.position, itemArrest.Position) < 2)
                            {
                                API.detachEntity(itemPlayer);
                                API.detachEntity(sender);
                                var playerCrimes = db_Crimes.GetPlayerCrimes(itemPlayer.socialClubName);
                                if (playerCrimes == null) { return; }
                                if (playerCrimes.Crimes == null && playerCrimes.Crimes.Count == 0) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bu kişinin herhangi bir suçu yok."); return; }
                                int totalStarts = 0;
                                foreach (var item in playerCrimes.Crimes)
                                {
                                    totalStarts += item.WantedLevel;
                                }
                                totalStarts = totalStarts > 5 ? 5 : totalStarts;
                                itemPlayer.freeze(false);
                                itemPlayer.position = itemArrest.ArrestPoint;
                                itemPlayer.dimension = itemArrest.ArrestPointDimension;
                                API.resetEntityData(itemPlayer, "Cuffed");
                                API.stopPlayerAnimation(itemPlayer);
                                API.setEntityData(itemPlayer, "Jailed", true);
                                API.setEntityData(itemPlayer, "JailedTime", totalStarts * 5);
                                db_Crimes.ClearPlayerCrimes(itemPlayer.socialClubName);
                                API.setPlayerWantedLevel(itemPlayer, 0);
                                API.setEntityData(itemPlayer, "WantedLevel", 0);
                                db_Accounts.SavePlayerAccount(sender);
                                API.sendChatMessageToPlayer(sender, "~s~" + API.getEntityData(itemPlayer, "CharacterName") + " ~b~adlı kişi " + (totalStarts * 5) + " dk hapse atıldı.");
                                API.sendChatMessageToPlayer(itemPlayer, "~s~" + (totalStarts * 5) + "~b~ dk hapse atıldınız.");
                                return;
                            }
                        }
                    }
                    return;
                }
            }
        }
        [Command("hapistencikar", "/hapistencikar [Oyuncu Id]")]
        public void GetOutPlayerOfJail(Client sender, string identity)
        {
            if (API.getEntityData(sender, "FactionId") != 1) { API.sendChatMessageToPlayer(sender, "~r~Bunun için polis olmalısınız."); return; }
            try { Convert.ToInt32(identity); } catch (Exception) { API.sendChatMessageToPlayer(sender, "~y~Girdiğiniz parametre ~w~ID~y~ ve~w~ sayı ~y~olmalıdır."); return; }
            var player = db_Accounts.GetPlayerById(Convert.ToInt32(identity));
            if (Vector3.Distance(sender.position, player.position) < 5)
            {
                player.position = sender.position + new Vector3(0, 0, 1);
                player.dimension = sender.dimension;
                API.resetEntityData(player, "JailedTime");
                //API.setEntityData(item, "JailedTime", 0);
                API.setEntityData(player, "Jailed", false);
                return;
            }

        }
        [Command("polisradyosu", "/pr [Mesaj] (IC)", Alias = "pr", GreedyArg = true)]
        public void PoliceRadio(Client sender, string text)
        {
            if (API.getEntityData(sender, "FactionId") != 1) { API.sendChatMessageToPlayer(sender, "~r~Bunun için polis olmalısınız."); return; }
            rpgMgr.UpdatePlayerTalkLabel(sender, "~b~" + text);
            foreach (var item in API.getAllPlayers())
            {
                if (API.getEntityData(item, "FactionId") == 1)
                {
                    var senderName = API.getEntityData(sender, "CharacterName");
                    API.sendChatMessageToPlayer(item, "(" + FactionManager.GetPlayerRankName(sender) + ") ~b~" + senderName + ": " + text);
                }
            }
        }

        [Command("parmakizi", "/parmakizi ~y~- Yerdeki eşyaları kimin attığını bulur.")]
        public void Fingerprint(Client sender)
        {
            if (API.getEntityData(sender, "FactionId") != 1) { API.sendChatMessageToPlayer(sender, "~r~Bunun için polis olmalısınız."); return; }
            foreach (var itemDropped in db_Items.currentDroppedItems.Items)
            {
                if (Vector3.Distance(sender.position, itemDropped.Position) < 1.5f)
                {
                    if (String.IsNullOrEmpty(itemDropped.DroppedPlayerSocialClubName))
                    {
                        API.sendChatMessageToPlayer(sender, "~b~Herhangi bir parmak izine rastlanmadı.");
                    }
                    else
                    {
                        var droppedPlayer = db_Accounts.GetOfflineUserDatas(itemDropped.DroppedPlayerSocialClubName);
                        API.sendChatMessageToPlayer(sender, "~b~" + itemDropped.LabelInGame.text + " adlı eşyada ~s~" + droppedPlayer.FingerPrint + "~b~ olarak kayıtlı parmak izi bulundu.");
                    }
                    return;
                }
            }

            API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Yakınınızda üzerinde parmakizi okuyabileceğiniz bir eşya yok.");
        }
        [Command("parmakizial", "/parmakizial [OyuncuID]")]
        public void GetFingerPrintFromPlayer(Client sender, int targetPlayerId)
        {
            if (API.getEntityData(sender, "FactionId") != 1) { API.sendChatMessageToPlayer(sender, "~r~Bunun için polis olmalısınız."); return; }
            var player = db_Accounts.GetPlayerById(targetPlayerId);
            if (player != null)
            {
                if (API.hasEntityData(player, "Cuffed") || API.hasEntityData(player, "Handsup"))
                {
                    API.sendChatMessageToPlayer(sender, "~b~" + API.getEntityData(player, "CharacterName") + " adlı kişinin parmak izi ~s~" + API.getEntityData(player, "FingerPrint") + "~b~ olarak belirlendi.");
                }
                else
                {
                    API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Kişinin parmak izini okuyabilmeniz için kişi ellerini kaldırmalı veya kelepçeli olmalı. ~h~~y~(/teslimol)");
                }
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Oyuncu bulunamadı.");
            }
        }

        [Command("silahruhsati", "/silahruhsatı [OyuncuID]", Alias = "silahruhsatı")]
        public void WeaponLicense(Client sender, int targetPlayerId)
        {
            int _factionID = Convert.ToInt32(API.getEntityData(sender, "FactionId"));
            int _playerFactionRank = Convert.ToInt32(API.getEntityData(sender, "FactionRank"));
            if (_factionID != 1) { API.sendChatMessageToPlayer(sender, "~r~Bunun için polis olmalısınız."); return; }

            var _FacRank = db_FactionRanks.GetFactionRanks(_factionID);

            if (_playerFactionRank < _FacRank.Ranks[_FacRank.Ranks.Count - 3].RankLevel)
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bunun için yetkiniz yok.");
                return;
            }

            var player = db_Accounts.GetPlayerById(targetPlayerId);
            if (player != null)
            {
                if (Vector3.Distance(sender.position, player.position) < 3)
                {
                    if (InventoryManager.AddItemToPlayerInventory(player, new ClientItem
                    {
                        Count = 1,
                        Equipped = false,
                        SpecifiedValue = player.socialClubName,
                        ItemId = Database.db_Items.GameItems.Values.FirstOrDefault(x => x.Type == ItemType.License && x.Value_0 == "2").ID
                    }))
                    {
                        rpgMgr.Me(sender, " adlı kişi " + API.getEntityData(player, "CharacterName") + " adlı kişiye silah ruhsatı verir.");
                        rpgMgr.Me(player, " uzatılan silah ruhsatını sağ eli ile kavrayıp kendine doğru çeker.");
                    }
                    else
                    {
                        API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Karşı tarafın envanterinde daha fazla yer yok.");
                        API.sendChatMessageToPlayer(player, "~r~HATA: ~s~Envanterinize silah ruhsatı için yeterli yer yok.");
                    }

                }
                else
                {
                    API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Oyuncu yanınızda olmalı.");
                }
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Oyuncu bulunamadı.");
            }

        }
        [Command("ihbar", "/ihbar [ihbarID]")]
        public void TrackTicket(Client sender, int tId)
        {
            int _factionID = Convert.ToInt32(API.getEntityData(sender, "FactionId"));
            if (_factionID != 1) { API.sendChatMessageToPlayer(sender, "~r~Bunun için polis olmalısınız."); return; }

            var _ticket = currentTickets.FirstOrDefault(x => x.ID == tId);
            if (_ticket != null)
            {
                Clients.ClientManager.UpdateWaypoint(sender, _ticket.Position);
                API.sendChatMessageToPlayer(sender, "~b~[LSPD]: ~s~İhbar bölgesi ~b~haritanızda ~s~işaretlendi.");
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~İhbar bulunamadı.");
            }
        }

        [Command("ihbarsil", "/ihbarsil [ihbarID]")]
        public void RemoveTicket(Client sender, int tId)
        {
            int _factionID = Convert.ToInt32(API.getEntityData(sender, "FactionId"));
            if (_factionID != 1) { API.sendChatMessageToPlayer(sender, "~r~Bunun için polis olmalısınız."); return; }
            var _ticket = currentTickets.FirstOrDefault(x => x.ID == tId);
            if (_ticket != null)
            {
                RPGManager.SendAllPlayersInFaction(1, "~b~[LSPD]: ~s~" + db_Accounts.GetPlayerCharacterName(sender) + "~b~ adlı kişi bir ihbarı sildi. ~s~( " + _ticket.ID + " )");
                currentTickets.Remove(_ticket);
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~İhbar bulunamadı.");
            }
        }
        [Command("ihbarlar")]
        public void RequestAllTickets(Client sender)
        {
            int _factionID = Convert.ToInt32(API.getEntityData(sender, "FactionId"));
            if (_factionID != 1) { API.sendChatMessageToPlayer(sender, "~r~Bunun için polis olmalısınız."); return; }
            string strTickets = "";
            foreach (var item in currentTickets)
            {
                strTickets += $"\n~b~[LSPD] (İhbar {item.ID})~s~ {item.Text}";
            }
            if (currentTickets.Count == 0)
                API.shared.sendChatMessageToPlayer(sender, "~b~[LSPD] ~s~Aktif ihbar bulunmuyor.");
            else
                API.shared.sendChatMessageToPlayer(sender, strTickets);
        }

        // [Command("parmakizisorgula", "/parmakizisorgula ~b~[Parmakİzi]")]
        public void FindFingerPrintOwner(Client sender, string fingerPrint)
        {
            if (API.getEntityData(sender, "FactionId") != 1) { API.sendChatMessageToPlayer(sender, "~r~Bunun için polis olmalısınız."); return; }
            var _user = db_Accounts.FindFingerPrint(fingerPrint);
            if (_user != null)
            {
                API.sendChatMessageToPlayer(sender, "~b~Parmak izi sahibi: ~s~" + _user.CharacterName + "\n" +
                        "~b~Meslek: ~s~" + JobManager.ToJobName(_user.JobId) + "~b~ | Yaş: ~s~" + (DateTime.Now.Year - _user.BirthDate.Year) + "\n" +
                        "~b~Doğum Yeri: ~s~" + _user.Origin + "~b~ | Cinsiyet: ~s~" + (_user.Gender ? "Erkek" : "Kadın") + "\n" +
                        "~b~Son Görülme: ~s~ (Son görüldüğü yer haritanızda işaretlendi.)"
                        );

                var _userClient = db_Accounts.IsPlayerOnline(_user.SocialClubName);
                if (_userClient != null)
                {
                    API.triggerClientEvent(sender, "update_waypoint", _userClient.position.X, _userClient.position.Y);
                }
                else
                {
                    API.triggerClientEvent(sender, "update_waypoint", _user.LastPosition.X, _user.LastPosition.Y);
                }
            }
            else
            {
                API.shared.sendChatMessageToPlayer(sender, "~r~Parmak izin bulunamadı.");
            }
            //FindFingerPrintOwner(fingerPrint);

        }

        public static void CuffPlayer(Client player)
        {
            API.shared.stopPlayerAnimation(player);
            API.shared.resetEntityData(player, "Handsup");
            //API.freezePlayer(item, true);
            API.shared.playPlayerAnimation(player, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "mp_arresting", "idle");
            API.shared.setEntityData(player, "Cuffed", true);
        }
        public static void CheckPlayerCuffed(Client player)
        {
            bool cuffed = API.shared.getEntityData(player, "Cuffed");
            if (cuffed)
            {
                CuffPlayer(player);
            }
        }
        public static void FindFingerPrintOwner2(Client sender, string fingerPrint)
        {
            var _user = db_Accounts.FindFingerPrint(fingerPrint);
            if (_user != null)
            {
                API.shared.sendChatMessageToPlayer(sender, "~b~Parmak izi sahibi: ~s~" + _user.CharacterName + "\n" +
                        "~b~Meslek: ~s~" + JobManager.ToJobName(_user.JobId) + "~b~ | Yaş: ~s~" + (DateTime.Now.Year - _user.BirthDate.Year) + "\n" +
                        "~b~Doğum Yeri: ~s~" + _user.Origin + "~b~ | Cinsiyet: ~s~" + (_user.Gender ? "Erkek" : "Kadın") + "\n" +
                        "~b~Son Görülme: ~s~ (Son görüldüğü yer haritanızda işaretlendi.)"
                        );

                var _userClient = db_Accounts.IsPlayerOnline(_user.SocialClubName);
                if (_userClient != null)
                {
                    API.shared.triggerClientEvent(sender, "update_waypoint", _userClient.position.X, _userClient.position.Y);
                }
                else
                {
                    API.shared.triggerClientEvent(sender, "update_waypoint", _user.LastPosition.X, _user.LastPosition.Y);
                }
            }
            else
            {
                API.shared.sendChatMessageToPlayer(sender, "~r~Parmak izin bulunamadı.");
            }
        }

        public static void AddTicket(PhoneTicket _ticket)
        {
            _ticket.ID = currentTickets.Count > 0 ? currentTickets.LastOrDefault().ID + 1 : 1;
            RPGManager.SendAllPlayersInFaction(1, "~b~[LSPD]: YENİ İHBAR: ~s~" + _ticket.Text + " ((/ihbar " + _ticket.ID + "))");
            currentTickets.Add(_ticket);
        }

        //public void FindFingerPrintOwner(string fingerPrint)
        //{
        //    var _user = db_Accounts.FindFingerPrint(fingerPrint);
        //    API.sendChatMessageToPlayer()
        //}
    }
}
