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
using TecoRP.Models;

namespace TecoRP.Managers
{
    public class AdvertisementManager : Script
    {
        [Command("reklam")]
        public void OnAdvertisement(Client sender)
        {
            foreach (var item in db_FactionInteractives.currentFactionInteractives)
            {
                if (item.Value.Faction == 5 && Vector3.Distance(sender.position, item.Value.Position) < 4)
                {
                    OpenAdvertisementMenu(sender);
                    return;
                }
            }
        }
        [Command("reklamonayla", "/ro [ID]", Alias = "ro")]
        public void ApproveAdvertisement(Client sender, int id)
        {
            if (FactionManager.IsPlayerInFaction(sender, 5) || API.shared.getEntityData(sender, "AdminLevel") >= 1)
            {
                var _adv = db_Adversitements.GetById(id);
                if (_adv != null)
                {
                    var player = db_Players.IsPlayerOnline(_adv.OwnerSocialClubId);
                    if (player != null)
                    {
                        string number = PhoneManager.GetPhoneNumbers(player).FirstOrDefault();
                        API.shared.sendChatMessageToAll($"~g~[WZReklam] [{number}] {player.nametag}: {_adv.Text}");
                    }
                    else
                    {
                        var _player = db_Players.GetOfflineUserDatas(_adv.OwnerSocialClubId);
                        var _phone = InventoryManager.GetItemFromOfflineUser(_adv.OwnerSocialClubId, Models.ItemType.Phone).FirstOrDefault().Item2;
                        var specifiedValue = (SpecifiedValuePhone)API.fromJson(_phone.SpecifiedValue).ToObject<SpecifiedValuePhone>();
                        string number = specifiedValue.PhoneNumber;
                        API.shared.sendChatMessageToAll($"~g~[WZReklam] [{number}]{_player.CharacterName}: {_adv.Text}");
                    }
                    db_Adversitements.Remove(_adv.AddvertisementID);
                }
            }
            else
                API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bunun için yetkiniz yok.");

        }
        [Command("reklamreddet", "/rr [ID]", Alias = "rr")]
        public void RejectAdvertisement(Client sender, int id)
        {
            if (!FactionManager.IsPlayerInFaction(sender, 5) || API.shared.getEntityData(sender, "AdminLevel") >= 1)
            {

                var _adv = db_Adversitements.GetById(id);
                if (_adv != null)
                {
                    var player = db_Players.IsPlayerOnline(_adv.OwnerSocialClubId);
                    if (player != null)
                    {
                        string number = PhoneManager.GetPhoneNumbers(player).FirstOrDefault();
                        API.shared.sendChatMessageToPlayer(player, "~g~[WZReklam] ~s~Reklamınız onaylanmadı. Paranız banka hesabına geri yatırıldı.");
                        InventoryManager.AddMoneyToPlayerBank(player, 50);
                    }
                    else
                    {
                        InventoryManager.AddMoneyToOfflinePlayerBank(_adv.OwnerSocialClubId, 50);
                    }
                    API.shared.sendChatMessageToPlayer(sender, "~g~[WZReklam] ~s~Reklamı ~r~reddettiniz.");
                    db_Adversitements.Remove(_adv.AddvertisementID);
                }
            }

            else
                API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bunun için yetkiniz yok.");

        }

        [Command("reklamlar", "/rr [ID]", Alias = "rr")]
        public void Advertisements(Client sender)
        {
            if (FactionManager.IsPlayerInFaction(sender, 5) || API.shared.getEntityData(sender, "AdminLevel") >= 1)
            {
                var _list = db_Adversitements.GetAll();
                foreach (var item in _list)
                {
                    var player = db_Players.IsPlayerOnline(item.OwnerSocialClubId);
                    if (player != null)
                    {
                        API.shared.sendChatMessageToPlayer(sender, $"~g~[WZNews] ((/ro {item.AddvertisementID} | /rr {item.AddvertisementID}))~s~ -[{player.nametag}] {item.Text}");
                    }
                    else
                    {
                        var _player = db_Players.GetOfflineUserDatas(item.OwnerSocialClubId);
                        API.shared.sendChatMessageToPlayer(sender, $"~g~[WZNews] ((/ro {item.AddvertisementID} | /rr {item.AddvertisementID}))~s~-[{_player.CharacterName}] {item.Text}");

                    }

                }

            }
            else
                API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bunun için yetkiniz yok.");

        }

        public static void OpenAdvertisementMenu(Client sender)
        {
            Clients.ClientManager.GetAdvertisementText(sender);
        }

        public static void CompleteAdvertisement(Client sender, string text, bool phone = true)
        {
            if ((phone ? InventoryManager.IsEnougMoneyInBank(sender, 50) : InventoryManager.IsEnoughMoney(sender,50)))
            {
                if (phone)
                    InventoryManager.AddMoneyToPlayerBank(sender, -50);
                else
                    InventoryManager.AddMoneyToPlayer(sender, -50);

                var _Id = db_Adversitements.Add(new Models.Advertisement(sender.socialClubName, text));
                API.shared.sendChatMessageToPlayer(sender, "~g~[WZNews] ~s~- Reklamınız alındı. Onaylandıktan sonra yayınlanacaktır.");
                RPGManager.SendAllPlayersInFaction(5, $"~g~[WZNews] ((/ro {_Id} | /rr {_Id}))~s~ -[{db_Players.GetPlayerCharacterName(sender)}] {text}");
                Admin.AdminCommands.SendMessage($"~g~[WZNews] ((/ro {_Id} | /rr {_Id}))~s~ -[{db_Players.GetPlayerCharacterName(sender)}] {text}", 5, 1);
            }
            else
            {
                API.shared.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Bunun için "+(phone ? "bankanızda ": " ") +"en az ~r~50$~s~ paranız olmalı.");
            }
        }
    }
}
