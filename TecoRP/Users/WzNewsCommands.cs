using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared.Math;
using TecoRP.Database;
using TecoRP.Managers;

namespace TecoRP.Users
{
    public class WzNewsCommands : Script
    {
        public WzNewsCommands()
        {
            API.onChatMessage += API_onChatMessage;
        }

        private void API_onChatMessage(Client sender, string message, CancelEventArgs cancel)
        {
            if (API.hasEntityData(sender, BROADCAST) && InventoryManager.IsEquippedItem(sender, 238))
            {
                API.shared.sendChatMessageToAll($"~g~[WZNews] [{(FactionManager.IsPlayerInFaction(sender, 5) ? "Sunucu" : "Konuk") }] ({db_Accounts.GetPlayerCharacterName(sender)}): " + message);
            }
            if (API.hasEntityData(sender, INVITATION))
            {
                API.shared.sendChatMessageToAll($"~g~[WZNews] [{(FactionManager.IsPlayerInFaction(sender, 5) ? "Konuk Sunucu" : "Konuk") }] ({db_Accounts.GetPlayerCharacterName(sender)}): " + message);
            }
        }

        public const string BROADCAST = "on_broadcast";
        public const string INVITATION = "on_invited_broadcast";
        RPGManager rpgMgr = new RPGManager();
        [Command("canliyayin", "/cy [baslat/davetet/durdur]", Alias = "cy")]
        public void StartBroadcast(Client sender, string param)
        {
            if (!FactionManager.IsPlayerInFaction(sender, 5))
            {
                API.shared.sendChatMessageToPlayer(sender, $"~r~HATA: ~s~Bunun için {FactionManager.ToFactionName(5)} oluşumunda olmanız gerekmektedir."); return;
            }

            switch (param)
            {
                case "baslat":
                case "başlat":
                    if (!InventoryManager.DoesPlayerHasItemById(sender,238))
                    {
                        API.shared.sendChatMessageToPlayer(sender,"~g~[WZNews] ~s~Bunun için mikrofona ihtiyacınız var.");
                        return;
                    }
                    API.shared.setEntityData(sender, BROADCAST, true);
                    rpgMgr.Me(sender, " canlı yayın için hazırlıklara başlar.");
                    API.shared.sendChatMessageToPlayer(sender, "~g~[WZNews] - Canlı yayın başlattınız.");
                    break;
                case "davetet":
                    List<string> names = new List<string>();
                    List<int> IDs = new List<int>();
                    foreach (var itemPlayer in API.shared.getAllPlayers())
                    {
                        if (Vector3.Distance(itemPlayer.position, sender.position) < 3)
                        {
                            names.Add(db_Accounts.GetPlayerCharacterName(itemPlayer));
                            IDs.Add(API.shared.getEntityData(itemPlayer, "ID"));
                        }
                    }
                    Clients.ClientManager.InviteBroadcastSelectorMenu(sender, names, IDs);
                    break;
                case "durdur":
                    API.shared.resetEntityData(sender, BROADCAST);
                    RemoveAllInvitedPlayers(sender);
                    API.shared.sendChatMessageToAll($"~g~[WZNews] - *** YAYIN SONA ERDİ ***");
                    break;
                case "ayril":
                case "ayrıl":
                    if (API.shared.hasEntityData(sender, INVITATION))
                    {
                        API.shared.sendChatMessageToPlayer(sender, "~g~[WZNews] ~s~Yayından ayrıldınız.");
                        var player = db_Accounts.GetPlayerById(API.shared.getEntityData(sender, INVITATION));
                        if (player != null)
                        {
                            API.shared.sendChatMessageToPlayer(player, "~g~[WZNews] ~s~" + db_Accounts.GetPlayerCharacterName(sender) + " adlı kişi yayından ayrıldı.");
                        }
                        API.shared.resetEntityData(sender, INVITATION);
                    }
                    else
                    {
                        API.shared.sendChatMessageToPlayer(sender, "~g~[WZNews] ~s~Zaten bir yayında değilsiniz.");
                    }
                    break;
                case "cikar":
                case "çıkar":
                    List<string> namesk = new List<string>();
                    List<int> IDsk = new List<int>();
                    foreach (var itemPlayer in API.shared.getAllPlayers())
                    {
                        if (Vector3.Distance(itemPlayer.position, sender.position) < 3)
                        {
                            namesk.Add(db_Accounts.GetPlayerCharacterName(itemPlayer));
                            IDsk.Add(API.shared.getEntityData(itemPlayer, "ID"));
                        }
                    }
                    Clients.ClientManager.KickBroadcastSelectorMenu(sender, namesk, IDsk);
                    break;
            }
        }
        public static void RemoveAllInvitedPlayers(Client inviterPlayer)
        {
            foreach (var itemPlayer in API.shared.getAllPlayers())
            {
                if (API.shared.hasEntityData(itemPlayer, INVITATION) && API.shared.getEntityData(itemPlayer, INVITATION) == API.shared.getEntityData(inviterPlayer, "ID"))
                {
                    API.shared.resetEntityData(itemPlayer, INVITATION);
                }
            }
        }
        public static void CompleteInvite(Client invitor, int invitedPlayerID)
        {
            var player = db_Accounts.FindPlayerById(invitedPlayerID);
            if (player != null)
            {
                RPGManager rpgMgr = new RPGManager();
                rpgMgr.Me(invitor, $" elindeki mikrofonu {db_Accounts.GetPlayerCharacterName(player)} adlı kişiye uzatır.");
                API.shared.sendChatMessageToPlayer(player, "~g~[WZNews] ~s~Canlı yayına dahil oldunuz.  Çıkmak için ~y~((/cy ayrıl))");
                API.shared.sendChatMessageToPlayer(invitor, $"~g~[WZNews] ~s~{db_Accounts.GetPlayerCharacterName(player)} adlı kişi canlı yayına aldınız. Çıkarmak için ~y~((/cy cikar))");
                API.shared.setEntityData(player, INVITATION, API.shared.getEntityData(invitor, "ID"));
            }
            else
            {
                API.shared.sendChatMessageToPlayer(invitor, "~r~HATA: ~s~Oyuncu bulunamadı.");
            }
        }

        public static void KickPlayerFromBroadCast(Client sender, int targetPlayerId)
        {
            var player = db_Accounts.GetPlayerById(targetPlayerId);
            if (player != null)
            {
                if (API.shared.hasEntityData(player, INVITATION))
                {
                    var invitorPlayer = db_Accounts.GetPlayerById(API.shared.getEntityData(player, INVITATION));
                    if (invitorPlayer != null)
                    {
                        if (invitorPlayer != sender)
                        {
                            API.shared.sendChatMessageToPlayer(invitorPlayer, $"~g~[WZNews] ~s~{db_Accounts.GetPlayerCharacterName(player)} adlı kişi yayından {db_Accounts.GetPlayerCharacterName(sender)} tarafından çıkarıldı.");
                        }
                        API.shared.sendChatMessageToPlayer(sender, $"~g~[WZNews] ~s~{db_Accounts.GetPlayerCharacterName(player)} adlı kişi yayından çıkarıldı.");
                        API.shared.sendChatMessageToPlayer(player, $"~g~[WZNews] ~s~{db_Accounts.GetPlayerCharacterName(sender)} adlı kişi sizi yayından çıkardı.");
                        API.shared.resetEntityData(player, INVITATION);
                    }
                }
            }
        }
    }
}
