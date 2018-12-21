using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using TecoRP.Database;
using TecoRP.Managers;
using TecoRP.Models;

namespace TecoRP.Users
{
    public class MedicalCommands : Script
    {
        static RPGManager rpgMgr = new RPGManager();
        static List<PhoneTicket> currentTickets = new List<PhoneTicket>();
        [Flags]
        public enum AnimationFlags
        {
            Loop = 1 << 0,
            StopOnLastFrame = 1 << 1,
            OnlyAnimateUpperBody = 1 << 4,
            AllowPlayerControl = 1 << 5,
            Cancellable = 1 << 7
        }
        public MedicalCommands()
        {
            API.onPlayerDeath += API_onPlayerDeath;
        }
        private void API_onPlayerDeath(Client player, NetHandle entityKiller, int weapon)
        {
            Task.Run(async()=> {
                await Task.Delay(4000);
                API.shared.sendNativeToPlayer(player, Hash.FREEZE_ENTITY_POSITION, player, false);
                API.shared.sendNativeToPlayer(player, Hash.RESURRECT_PED, player);
                API.shared.sendNativeToPlayer(player, Hash.SET_PED_TO_RAGDOLL, player, true);
                API.shared.sendNativeToPlayer(player, Hash.NETWORK_RESURRECT_LOCAL_PLAYER, player.position.X, player.position.Y, player.position.Z, player.rotation.Z, false, false);
                API.shared.sendNativeToPlayer(player, Hash._RESET_LOCALPLAYER_STATE, player);
                API.shared.sendNativeToPlayer(player, Hash.RESET_PLAYER_ARREST_STATE, player);
                API.shared.sendNativeToPlayer(player, Hash.IGNORE_NEXT_RESTART, true);
                API.shared.sendNativeToPlayer(player, Hash._DISABLE_AUTOMATIC_RESPAWN, true);
                API.shared.sendNativeToPlayer(player, Hash.SET_FADE_IN_AFTER_DEATH_ARREST, true);
                API.shared.sendNativeToPlayer(player, Hash.SET_FADE_OUT_AFTER_DEATH, false);
                API.shared.sendNativeToPlayer(player, Hash.NETWORK_REQUEST_CONTROL_OF_ENTITY, player);

                DeadPlayer(player, 300);
                API.setTime(DateTime.Now.Hour, DateTime.Now.Minute);

            });
            }
        public static void DeadPlayer(Client sender, int seconds)
        {
            sender.playAnimation("dead", "dead_d", 1);
            API.shared.setEntityInvincible(sender, true);
            API.shared.setEntityData(sender, "Dead", true);
            API.shared.setEntityData(sender, "DeadSeconds", seconds);
            sender.freeze(true);
            sender.sendChatMessage("~c~(( ~w~Yaralısınız." + seconds + " saniye içerisinde kurtarılmazsanız hastahanede doğacaksınız. ~c~))");
            sender.sendChatMessage("911'i arayarak kendinize bir ambulans çağırabilir veya ~r~/öl ~s~komutu ile anında ölebilirsiniz.");
            Task.Run(async () =>
            {
                Point:
                await Task.Delay(1000);
                seconds = API.shared.getEntityData(sender, "DeadSeconds");
                seconds--;
                if(!sender.isInVehicle) sender.playAnimation("dead", "dead_d", 1);
                API.shared.setEntityData(sender, "DeadSeconds", seconds);
                if (seconds > 0 && API.shared.getEntityData(sender,"Dead")==true)
                {
                    goto Point;
                }
                if (API.shared.getEntityData(sender,"Dead")==false)
                {
                    return;
                }
                //rpgMgr.Me(sender, " acı bir şekilde son nefesini verir.");
                RespawnPlayer(sender);
            });
            // API.shared.setEntityData(sender, "Dead", false);          

        }

        
        public static void RespawnPlayer(Client sender, bool moneyTaken = true)
        {
            API.shared.setEntityInvincible(sender, false);
            sender.freeze(false);
            API.shared.setEntityData(sender, "Dead", false);
            sender.stopAnimation();
            if (moneyTaken)
            {
                sender.sendChatMessage("~c~(( ~w~Maalesef kurtarılamadınız. ~c~))");
                InventoryManager.AddMoneyToPlayer(sender, -1 * db_Hospital.hospital.RespawnPrice);
            }else
            {
             sender.sendChatMessage("~c~(( ~w~Kuratıldınız!. ~c~))");
            }
            sender.position = db_Hospital.hospital.RespawnPoint;
            sender.dimension = db_Hospital.hospital.RespawnDimension;
        }
        public static void CheckIfPlayerIsDead(Client sender)
        {
            int deadSeconds = API.shared.getEntityData(sender, "DeadSeconds");
            if ((bool)API.shared.getEntityData(sender, "Dead") == true && deadSeconds > 0)
            {
                DeadPlayer(sender, deadSeconds);
            }
        }

        [Command("ol",Alias ="öl")]
        public void Die(Client sender)
        {
            if (API.getEntityData(sender,"Dead")==true)
            {
                API.setEntityData(sender, "DeadSeconds", 1);
            }else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bunu şimdi yapamazsınız.");
            }
        }
        [Command("ilkyardim", "/ilkyardim [OyuncuID]",Alias ="ilkyardım")]
        public void FirstAid(Client sender, int targetPlayerId)
        {
            int playerFactionId = API.getEntityData(sender, "FactionId");
            if (playerFactionId != 4) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Uygun oluşumda değilsiniz."); return; }

            var player = db_Players.GetPlayerById(targetPlayerId);
            if (player == null) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Oyuncu bulunamadı."); return; }
            if(API.hasEntityData(player, "firstAided")) { API.sendChatMessageToPlayer(sender,"~r~HATA: ~s~Bu kişiye zaten ilkyardım uygulanmış."); return; }
            if (Vector3.Distance(sender.position, player.position) < 3)
            {
                if (API.getEntityData(player, "Dead") == true)
                {
                    Clients.ClientManager.RemoveBlip(sender);
                    int _deadSeconds = API.getEntityData(player, "DeadSeconds");
                    _deadSeconds += 180;
                    API.setEntityData(player, "DeadSeconds", _deadSeconds);
                    API.setEntityData(player, "firstAided", true);
                    rpgMgr.Me(sender, " yerde baygın olarak yatan " + db_Players.GetPlayerCharacterName(player) + " adlı kişiye ilkyardım uygular.");
                    API.sendChatMessageToPlayer(sender, "(( ~c~Kişiyi kurtarmak için" + _deadSeconds + " saniyen var. Bu süre içerisinde hastaneye yetişrimelisin. ~s~))");
                    API.sendChatMessageToPlayer(sender, "(( ~c~" + _deadSeconds + " saniye içerisinde hastaneye yetiştirilemezsen öleceksin. ))");
                    API.playPlayerAnimation(sender, 0, "amb@code_human_police_investigate@idle_b", "idle_f");
                }
                else
                {
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bu oyuncunun ilkyardıma ihtiyacı yok.");
                }
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Oyuncu yanınızda olmalı.");
            }
        }
        [Command("tasi", "/taşı [OyuncuID]", Alias = "taşı")]
        public void DragPlayer(Client sender, int targetPlayerId)
        {
            int playerFactionId = API.getEntityData(sender, "FactionId");
            if (playerFactionId != 4) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Uygun oluşumda değilsiniz."); return; }
            var player = db_Players.GetPlayerById(targetPlayerId);
            if (player == null) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Oyuncu bulunamadı."); return; }
            if (Vector3.Distance(sender.position, player.position) < 3)
            {
                if (API.hasEntityData(player, "firstAided"))
                {
                    API.attachEntityToEntity(player, sender, null, new Vector3(0.1f, 0, -0.2f), new Vector3());
                }
                else
                {
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Oyuncuyu taşıyabilmek için önce ilk yardım uygulamanız gerekmekte.");
                }
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Oyuncu yanınızda olmalı.");
            }
        }
        [Command("aracayukle", "/aracayükle [OyuncuID] [Koltuk]", Alias = "aracayükle")]
        public void PutPlayerIntoVehicle(Client sender, int targetPlayerId, int seat)
        {
            int playerFactionId = API.getEntityData(sender, "FactionId");
            if (playerFactionId != 4) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Uygun oluşumda değilsiniz."); return; }
            var player = db_Players.GetPlayerById(targetPlayerId);
            if (player == null) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Oyuncu bulunamadı."); return; }
            if (Vector3.Distance(sender.position, player.position) < 3)
            {
                var _vehicle = db_Vehicles.FindNearestVehicle(sender.position);
                if (Vector3.Distance(_vehicle.VehicleOnMap.position, sender.position) < 5)
                {
                    if (_vehicle.FactionId == 4)
                    {
                        if (API.hasEntityData(player, "firstAided"))
                        {
                            API.detachEntity(player, true);
                            API.setPlayerIntoVehicle(player, _vehicle.VehicleOnMap, seat);
                            API.playPlayerAnimation(player, 0, "", "");
                            var pos = db_Hospital.hospital.InjuredDeliverPosition;
                            Clients.ClientManager.ShowBlip(sender, pos.X, pos.Y, pos.Z);

                        }
                        else
                        {
                            API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Oyuncuyu taşıyabilmek için önce ilk yardım uygulamanız gerekmekte.");
                        }
                    }
                    else
                    {
                        API.sendChatMessageToPlayer(sender, "~r~HATA: Bu aracı bu iş için kullanamazsınız.");
                    }
                }
                else
                {
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Yakınlarda bir araç yok.");
                }
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Oyuncu yanınızda olmalı.");
            }
        }

        [Command("yaraliteslimet", "/yaralıteslimet [OyuncuID]", Alias = "yaralıteslimet")]
        public void DeliverInjuredPlayer(Client sender, int targetPlayerId)
        {
            Clients.ClientManager.RemoveBlip(sender);
            int playerFactionId = API.getEntityData(sender, "FactionId");
            if (playerFactionId != 4) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Uygun oluşumda değilsiniz."); return; }
            var player = db_Players.GetPlayerById(targetPlayerId);
            if (player == null) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Oyuncu bulunamadı."); return; }
            if (Vector3.Distance(sender.position, player.position) < 5)
            {
                API.setEntityData(player, "Dead", false);
                API.resetEntityData(player, "firstAided");
                API.shared.resetEntityData(player, "DeadSeconds");

                Clients.ClientManager.RemoveBlip(sender);
                API.stopPlayerAnimation(player);
                RespawnPlayer(player,false);
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Oyuncu yanınızda olmalı.");
            }
        }

        [Command("cagrikabulet","/çağrıkabulet [ÇağrıID]",Alias = "çağrıkabulet")]
        public void AcceptTicket(Client sender,int ticketId)
        {
            int playerFactionId = API.getEntityData(sender, "FactionId");
            if (playerFactionId != 4) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Uygun oluşumda değilsiniz."); return; }
            var ticket = currentTickets.FirstOrDefault(x => x.ID == ticketId);
            if (ticket!=null)
            {
                Clients.ClientManager.ShowBlip(sender, ticket.Position.X, ticket.Position.Y, ticket.Position.Z-1);
                RPGManager.SendAllPlayersInFaction(4, "~r~[LSMD] ~s~" + db_Players.GetPlayerCharacterName(sender) + " bir çağrıyı kabul etti.");
                var player = db_Players.IsPlayerOnline(ticket.OwnerSocialClubID);
                if (player!=null)
                {
                    API.sendChatMessageToPlayer(player, "~r~[LSMD] : ~s~Çağrınız kabul edildi. Bulunduğunuz konumda bekleyin.");
                }
                currentTickets.Remove(ticket);
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Çağrı bulunamadı.");
            }
        }

        [Command("cagriiptalet", "/çağrıkabulet [ÇağrıID]", Alias = "çağrıiptalet")]
        public void RejectTicket(Client sender)
        {
            int playerFactionId = API.getEntityData(sender, "FactionId");
            if (playerFactionId != 4) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Uygun oluşumda değilsiniz."); return; }

            Clients.ClientManager.RemoveBlip(sender);
        }
        [Command("cagrilar",Alias ="çağrılar")]
        public void RequestAllTickets(Client sender)
        {
            int playerFactionId = API.getEntityData(sender, "FactionId");
            if (playerFactionId != 4) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Uygun oluşumda değilsiniz."); return; }
            string strTickets = "____ÇAĞRILAR____";
            foreach (var item in currentTickets)
            {
                strTickets += "\n~r~[LSMD] - "+item.ID+" - ~s~"+item.Text+"";
            }
            RPGManager.SendAllPlayersInFaction(4, strTickets);
        }

        [Command("mr","/mr [Mesajınız] (( IC ))",GreedyArg = true)]
        public void MedicalRadio(Client sender,string text)
        {
            int playerFactionId = API.getEntityData(sender, "FactionId");
            if (playerFactionId != 4) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Uygun oluşumda değilsiniz."); return; }
            Rank factionRank = db_FactionRanks.GetRank(4, API.getEntityData(sender, "FactionRank"));
            if (factionRank!=null)
            {
                RPGManager.SendAllPlayersInFaction(4, "~r~[LSMD]("+factionRank.RankName+") "+db_Players.GetPlayerCharacterName(sender)+": ~s~"+text); return;
            }
            RPGManager.SendAllPlayersInFaction(4, "~r~[LSMD] " + db_Players.GetPlayerCharacterName(sender) + ": ~s~"+text);

        }
        public static void AddPhoneTicket(string text,Vector3 position,string ownerSocialClubName)
        {
            var model = new PhoneTicket {
                ID = currentTickets.Count > 0 ? currentTickets.LastOrDefault().ID + 1 : 1,
                Position = position,
                Text = text,
                OwnerSocialClubID = ownerSocialClubName
            };
            currentTickets.Add(model);
            RPGManager.SendAllPlayersInFaction(4,"~r~[LSMD] YENİ ÇAĞRI: ~s~"+text+ "~r~(( /cagrikabulet "+model.ID+" ))");
        }
        public static void RevivePlayer(Client player)
        {
            API.shared.setEntityData(player, "Dead", false);
            API.shared.setEntityData(player, "DeadSeconds", 0);
            API.shared.stopPlayerAnimation(player);
            API.shared.setEntityInvincible(player, false);
            API.shared.freezePlayer(player, false);
            db_Players.SavePlayerAccount(player);
        }
    }
}
