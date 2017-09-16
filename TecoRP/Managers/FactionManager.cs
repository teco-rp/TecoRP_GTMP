using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TecoRP.Models;
using TecoRP.Database;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Shared.Math;
using GrandTheftMultiplayer.Shared;

namespace TecoRP.Managers
{
    public class FactionManager : Script
    {
        [Command("olusumdakiler", "/olusumdakiler")]
        public void OnRequestFactionOnlie(Client sender)
        {
            int _factionID = Convert.ToInt32(API.getEntityData(sender, "FactionId"));
            int _playerFactionRank = Convert.ToInt32(API.getEntityData(sender, "FactionRank"));
            if (_factionID>0)
            {
                string strPlayers = "_____" + ToFactionName(_factionID)+ "_____";
                foreach (var itemPlayer in API.getAllPlayers())
                {
                    if (API.getEntityData(itemPlayer, "FactionId") == _factionID)
                    {
                        strPlayers += "\n~y~("+db_FactionRanks.GetRank(_factionID,API.getEntityData(itemPlayer,"FactionRank")).RankName + ") ~s~"+db_Accounts.GetPlayerCharacterName(itemPlayer)+" ~h~("+API.getEntityData(itemPlayer,"ID")+")";
                    }
                }
                API.sendChatMessageToPlayer(sender, strPlayers);
            }else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Herhangi bir oluşumda değilsini.");
            }
        }

        [Command("rozetolustur", "/rozetolustur [Sahibi(Oyuncu ID)]")]
        public void CreateFactionLicense(Client sender, int targetPlayerId)
        {
            int _factionID = Convert.ToInt32(API.getEntityData(sender, "FactionId"));
            int _playerFactionRank = Convert.ToInt32(API.getEntityData(sender, "FactionRank"));
            if (_factionID == 0) { API.sendChatMessageToPlayer(sender, "~r~Herhangi bir oluşumda değilsiniz."); return; }

            var _FacRank = db_FactionRanks.GetFactionRanks(_factionID);

            if (_playerFactionRank  < _FacRank.Ranks[_FacRank.Ranks.Count - 2].RankLevel)
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bunun için yetkiniz yok.");
                return;
            }
            //if (factionRank >= _FacRank.Ranks[_FacRank.Ranks.Count - 2].RankLevel)

            var player = Database.db_Accounts.GetPlayerById(targetPlayerId);
            if (player != null)
            {
                if (API.getEntityData(player, "FactionId") != _factionID)
                {
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Kişi bu oluşumda değil.");
                    return;
                }

                switch (_factionID)
                {
                    case 1:
                        #region LSPD
                        if (InventoryManager.AddItemToPlayerInventory(player, new ClientItem
                        {
                            Count = 1,
                            Equipped = false,
                            SpecifiedValue = player.socialClubName,
                            ItemId = Database.db_Items.GameItems.Values.FirstOrDefault(x => x.Type == ItemType.License && x.Value_0 == "3").ID
                        }))
                        {
                            API.sendChatMessageToPlayer(sender, "~b~Rozet " + API.getEntityData(player, "CharacterName") + " adlı kişiye verildi.");
                        }
                        else
                        {
                            API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Kişinin envanterinde daha fazla yer yok.");
                        }
                        #endregion
                        break;
                    case 4:
                        #region LSMD
                        if (InventoryManager.AddItemToPlayerInventory(player, new ClientItem
                        {
                            Count = 1,
                            Equipped = false,
                            SpecifiedValue = player.socialClubName,
                            ItemId = Database.db_Items.GameItems.Values.FirstOrDefault(x => x.Type == ItemType.License && x.Value_0 == "4").ID
                        }))
                        {
                            API.sendChatMessageToPlayer(sender, "~b~Rozet " + API.getEntityData(player, "CharacterName") + " adlı kişiye verildi.");
                        }
                        else
                        {
                            API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Kişinin envanterinde daha fazla yer yok.");
                        }
                        #endregion
                        break;
                    case 5:
                        #region WZNews
                        if (InventoryManager.AddItemToPlayerInventory(player, new ClientItem
                        {
                            Count = 1,
                            Equipped = false,
                            SpecifiedValue = player.socialClubName,
                            ItemId = Database.db_Items.GameItems.Values.FirstOrDefault(x => x.Type == ItemType.License && x.Value_0 == "5").ID
                        }))
                        {
                            API.sendChatMessageToPlayer(sender, "~b~Rozet " + API.getEntityData(player, "CharacterName") + " adlı kişiye verildi.");
                        }
                        else
                        {
                            API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Kişinin envanterinde daha fazla yer yok.");
                        }
                        #endregion
                        break;
                    default:
                        break;
                }



            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Oyuncu bulunamadı.");
            }


            //var _inventory = (Inventory)API.getEntityData(sender, "inventory");

            //switch (_factionID)
            //{
            //    case 1:
            //        var licenseItem = _inventory.ItemList.FirstOrDefault(x => x.ItemId == 26);
            //        if (licenseItem != null)
            //        {
            //            if (Convert.ToInt32(licenseItem.SpecifiedValue) >= 4)
            //            {
            //                if (_inventory.ItemList.Count < _inventory.InventoryMaxCapacity)
            //                {
            //                    _inventory.ItemList.Add(new ClientItem { ItemId = 26, Count = 1, Equipped = false, SpecifiedValue = rank.ToString() });
            //                    API.setEntityData(sender, "inventory", _inventory);
            //                    return;
            //                }
            //                else
            //                {
            //                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Envanterinizde bunun için yer yok.");
            //                }
            //            }
            //            else
            //            {
            //                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bu yetkiye sahip değilsiniz.");
            //            }
            //        }
            //        else
            //        {
            //            API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Şu anda üzerinizde bu yetkiyi sağlayan bir rozet bulunmuyor.");
            //        }
            //        break;

            //    case 2:


            //        break;
            //    case 3:
            //        break;
            //    default:
            //        API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Rozet oluşturabileceğiniz bir oluşumda değilsiniz.");
            //        break;
            //}



        }

        [Command("olusumadavetet", "/olusumadavetet [Oyuncu Id]")]
        public void InviteToFaction(Client sender, int _Id)
        {
            var factionId = API.getEntityData(sender, "FactionId");
            if (factionId == 0) { API.sendChatMessageToPlayer(sender, "~r~Herhangi bir oluşumda değilsiniz."); return; }
            var factionRank = API.getEntityData(sender, "FactionRank");

            FactionRank _FacRank = db_FactionRanks.GetFactionRanks(factionId);
            if (factionRank >= _FacRank.Ranks[_FacRank.Ranks.Count - 2].RankLevel)
            {
                foreach (var itemPlayer in API.getAllPlayers())
                {
                    if (API.getEntityData(itemPlayer, "ID") == _Id)
                    {
                        if (API.getEntityData(itemPlayer, "FactionId") > 0)
                        {
                            API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bu oyuncu zaten başka bir oluşumda.");
                            return;
                        }
                        API.setEntityData(itemPlayer, "FactionInvite", factionId);
                        API.sendChatMessageToPlayer(itemPlayer, "~y~" + API.getEntityData(sender, "CharacterName") + " adlı kişi sizi " + ToFactionName(factionId) + " oluşumuna davet ediyor. (( /olusumkabulet ))");
                        API.sendChatMessageToPlayer(sender, "~y~" + API.getEntityData(itemPlayer, "CharacterName") + " adlı kişiyi " + ToFactionName(factionId) + " oluşumuna davet ettiniz.");
                        return;
                    }
                }
            }
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bunun için yetkiniz yok.");



        }
        [Command("olusumkabulet")]
        public void AcceptFaction(Client sender)
        {
            if (API.hasEntityData(sender, "FactionInvite"))
            {
                API.setEntityData(sender, "FactionId", API.getEntityData(sender, "FactionInvite"));
                API.setEntityData(sender, "FactionRank", 1);
                API.sendChatMessageToPlayer(sender, "~y~Oluşum davetini kabul ettiniz. Artık " + ToFactionName(API.getEntityData(sender, "FactionInvite")) + " adlı oluşumun üyesisiniz.");
                RPGManager.SendAllPlayersInFaction(API.getEntityData(sender, "FactionInvite"), "~s~" + db_Accounts.GetPlayerCharacterName(sender) + "~y~ adlı kişi oluşuma katıldı.");
                API.resetEntityData(sender, "FactionInvite");
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Herhangi bir oluşum davetiniz bulunmuyor.");
            }
        }
        [Command("olusumdancik")]
        public void QuitFromFaction(Client sender)
        {
            if (API.getEntityData(sender, "FactionId") != 0)
            {
                API.setEntityData(sender, "FactionId", 0);
                API.setEntityData(sender, "FactionRank", 1);
                API.sendChatMessageToPlayer(sender, "~y~Oluşumdan başarıyla çıktınız.");
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Zaten bir oluşumda değilsiniz.");
            }
        }

        [Command("olusumdanat", "/olusumdanat [OyuncuID]")]
        public void KickFromFAction(Client sender, int targetPlayerId)
        {
            var factionId = API.getEntityData(sender, "FactionId");
            if (factionId == 0) { API.sendChatMessageToPlayer(sender, "~r~Herhangi bir oluşumda değilsiniz."); return; }
            var factionRank = API.getEntityData(sender, "FactionRank");

            FactionRank _FacRank = db_FactionRanks.GetFactionRanks(factionId);
            if (factionRank >= _FacRank.Ranks.LastOrDefault().RankLevel)
            {
                var player = db_Accounts.GetPlayerById(targetPlayerId);
                if (API.getEntityData(player, "FactionId") == factionId)
                {
                    API.setEntityData(player, "FactionId", 0);
                    API.setEntityData(player, "FactionRank", 1);
                    API.sendChatMessageToPlayer(player, "~y~" + API.getEntityData(player, "CharacterName") + " adlı kişiyi oluşumdan başarıyla çıkarttınız.");
                }
                else
                {
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Kişi zaten oluşumda değil.");
                }
            }
        }

        [Command("rutbeler",Alias ="rütbeler")]
        public void ShowRanksOfPlayersFaction(Client sender)
        {
            int factionId = API.getEntityData(sender, "FactionId");
            if (factionId>0)
            {
                var _facRank = db_FactionRanks.GetFactionRanks(factionId);
                string strRanks = "";
                foreach (var item in _facRank.Ranks)
                {
                    strRanks += item.RankLevel + " - ~h~" + item.RankName + " \n";
                }
                API.shared.sendChatMessageToPlayer(sender, strRanks);
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Herhangi bir oluşumda değilsiniz.");
            }
        }
       [Command("rutbever", "/rutbever [Oyuncu (ID)] [Rutbe]")]
        public void GiveRankToPlayer(Client sender, int targetPlayerId, int rankLevel)
        {
            var factionId = API.getEntityData(sender, "FactionId");
            if (factionId == 0) { API.sendChatMessageToPlayer(sender, "~r~Herhangi bir oluşumda değilsiniz."); return; }
            var factionRank = API.getEntityData(sender, "FactionRank");

            FactionRank _FacRank = db_FactionRanks.GetFactionRanks(factionId);
            if (factionRank >= _FacRank.Ranks[_FacRank.Ranks.Count - 2].RankLevel)
            {
                var player = db_Accounts.GetPlayerById(targetPlayerId);
                if (player == null){ API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Oyuncu bulunamadı.");  return; }

                if (API.getEntityData(player, "FactionId") != factionId){ API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bu oyuncu sizin ile aynı oluşumda değil."); return; }
                if(API.getEntityData(player, "FactionId") > rankLevel) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bu oyuncunun rütbesi sizden yüksek olduğundan müdahale edemezsiniz."); return; }

                API.setEntityData(player, "FactionId", rankLevel);

            }else
            {
                API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Bunun için yetkiniz yok.");
            }
        }

        [Command("kasa")]
        public static void OnFactionVaultRequested(Client sender)
        {
            int playerFactionId = API.shared.getEntityData(sender, "FactionId");
            if (playerFactionId < 1) { API.shared.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Herhangi bir oluşumda değilsiniz."); return; }
            foreach (var itemVault in db_FactionVaults.currentVaults.Items)
            {
                if (Vector3.Distance(sender.position,itemVault.Position)<3)
                {
                    if(playerFactionId != itemVault.FactionId) { API.shared.sendChatMessageToPlayer(sender,"~r~HATA: ~s~Bu oluşumda değilsiniz."); return; }
                    int playerRank = API.shared.getEntityData(sender, "FactionRank");
                    //API.shared.consoleOutput("rank: " + playerRank);
                    List<string> names = new List<string>();
                    List<string> descs = new List<string>();
                    List<int> IDlist = new List<int>();
                    var _list = db_FactionVaults.GetFactionVaultItems(itemVault.VaultId, playerRank);
                    foreach (var item in _list)
                    {
                        names.Add(item.Name); descs.Add(item.Description); IDlist.Add(item.ID);
                    }
                    Clients.ClientManager.OpenVault(sender, names, descs, IDlist);
                    return;
                }
            }
            API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Yakınızda herhangi bir oluşum kasası bulunmuyor.");
        }

        public static void OnFactionVaultItemSelected(Client sender,int itemId)
        {
                         
            if(InventoryManager.AddItemToPlayerInventory(sender, new ClientItem { Count = 1, Equipped = false, ItemId = itemId }))
            {
                API.shared.sendChatMessageToPlayer(sender, "~y~Eşya oluşum kasasından alındı.");
                OnFactionVaultRequested(sender);
            }
            else
            {
                API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Envanterinizde daha fazla yer yok.");
            }
        }

        public static string ToFactionName(int factionId)
        {
            switch (factionId)
            {
                case 0:
                    return "Yok";
                case 1:
                    return "Polis Departmanı";
                case 2:
                    return "FIB";
                case 3:
                    return "Goverment";
                case 4:
                    return "LSMD";
                case 5:
                    return "WZ News";
                case 10:
                    return "SantoSecurity";
                default:
                    return "Yok";
            }
        }

        public static Rank GetPlayerRank(Client player)
        {
            int _facId = API.shared.getEntityData(player, "FactionId");
            int _facRank = API.shared.getEntityData(player, "FactionRank");
            if (_facId > 0)
            {
                var _rank = Database.db_FactionRanks.GetRank(_facId, _facRank);
                if (_rank != null)
                {
                    return _rank;
                }
            }
            return null;
        }

        public static string GetPlayerRankName(Client player)
        {
            try
            {

                return GetPlayerRank(player).RankName;
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(NullReferenceException))
                {
                    return "";
                }
                else
                    API.shared.consoleOutput(LogCat.Warn, ex.ToString());
                return "";
            }

        }
        public static int GetPlayerFaction(Client player)
        {
            return API.shared.getEntityData(player, "FactionId");
        }
        public static bool IsPlayerInFaction(Client player, int faction)
        {
            return faction == API.shared.getEntityData(player, "FactionId");
        }
        public static string GetPlayerFactionName(Client player)
        {
            return ToFactionName(API.shared.getEntityData(player, "FactionId"));
        }
    }
}
