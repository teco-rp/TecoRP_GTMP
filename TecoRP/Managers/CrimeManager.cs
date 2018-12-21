using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared.Math;
using TecoRP.Database;
using TecoRP.Models;
using TecoRP.Users;

namespace TecoRP.Managers
{
    public class CrimeManager : Script
    {
        public List<User> GetAllGuiltyPlayers()
        {
            List<User> returnModel = new List<Models.User>();
            foreach (var itemPlayer in db_Crimes.GetAll().Items)
            {
                returnModel.Add(db_Players.GetOfflineUserDatas(itemPlayer.OwnerSocialClubName));
            }
            return returnModel;
        }

        [Command("polisbilgisayari", "/pb", Alias = "pb")]
        public void PoliceComputer(Client sender)
        {
            int factionId = API.getEntityData(sender, "FactionId");
            if (factionId != 1) { API.sendChatMessageToPlayer(sender, "~r~Bunun için polis olmalısınız."); return; }

            var t = Task.Run(() =>
            {

                foreach (var item in db_FactionInteractives.currentFactionInteractives.Values)
                {
                    if (item.Faction == 1 && Vector3.Distance(item.Position, sender.position) < 4)
                    {
                        Clients.ClientManager.ShowLSPDComputer(sender);
                        return;
                    }
                }
                API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Bu araçta LSPD bilgisayarı bulunmuyor.");
            });
            if (sender.isInVehicle)
            {

                var _vehicle = db_Vehicles.FindNearestVehicle(sender.position);
                if (API.getVehicleClass(_vehicle.VehicleModelId) == 18)
                {
                    Clients.ClientManager.ShowLSPDComputer(sender);
                }
                else
                {
                    API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Bu araçta LSPD bilgisayarı bulunmuyor.");
                }
            }
            else
            {
                // API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bunun için LSPD bilgisayarı olan bir araçta olmalısın.");
            }
        }

        public static void OnGuiltyListSelected(Client sender)
        {
            //if (sender.isInVehicle)
            //{
                var guiltyList = db_Crimes.GetAll();
                if (guiltyList != null)
                {
                    List<string> names = new List<string>();
                    List<string> descs = new List<string>();
                    foreach (var item in guiltyList.Items)
                    {
                        try
                        {
                            if (item.Crimes.Count > 0)
                            {
                                names.Add(db_Players.GetOfflineUserDatas(item.OwnerSocialClubName).CharacterName);
                                descs.Add("Toplam Suç: " + item.CrimesBefore + " Son Suç: " + (item.Crimes.Count > 0 ? item.Crimes.LastOrDefault().Name : "Belirsiz."));
                            }
                        }
                        catch (Exception ex)
                        {
                            API.shared.consoleOutput(LogCat.Warn, ex.ToString());
                            continue;
                        }
                    }

                    Clients.ClientManager.ShowGuiltyList(sender, names, descs);
                return;
                //}


            }
            else
            {
                API.shared.sendChatMessageToPlayer(sender, "~r~Suçlu listesi boş");
            }
        }

        public static void OnFingerPrintScanned(Client sender, string fingerPrint)
        {
            PoliceCommands.FindFingerPrintOwner2(sender, fingerPrint);
        }

        public static void OnAddCrimeToPlayer(Client sender, string fingerPrint)
        {
            User player = db_Players.FindFingerPrint(fingerPrint);
            if (player != null)
            {
                List<string> names = new List<string>();
                List<string> descs = new List<string>();
                foreach (var item in db_Crimes.GetCrimeTypes().Items)
                {
                    names.Add(item.Name);
                    descs.Add("Suç ağırlığı: " + item.WantedLevel);
                }
                Clients.ClientManager.SendCrimeList(sender, names, descs, player.SocialClubName);
            }
            else
            {
                API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Kişi bulunamadı.");
            }
        }

        public static void OnCompleteAddCrimeToPlayer(Client sender, int index, string socialClubName)
        {
            var crimeList = db_Crimes.GetCrimeTypes().Items;
            db_Crimes.AddCrimeToPlayer(crimeList[index], socialClubName);

            var player = db_Players.IsPlayerOnline(socialClubName);
            if (player != null)
            {
                API.shared.setPlayerWantedLevel(player, API.shared.getPlayerWantedLevel(player) + crimeList[index].WantedLevel > 5 ? 5 : API.shared.getPlayerWantedLevel(player) + crimeList[index].WantedLevel);
                API.shared.setEntityData(player, "WantedLevel", API.shared.getPlayerWantedLevel(player));
            }
            else
            {
                var _offlinePlayer = db_Players.GetOfflineUserDatas(socialClubName);
                _offlinePlayer.WantedLevel = _offlinePlayer.WantedLevel + crimeList[index].WantedLevel > 5 ? 5 : _offlinePlayer.WantedLevel + crimeList[index].WantedLevel;
            }
            API.shared.sendChatMessageToPlayer(sender, "~y~Suç başarıyla eklendi.");

        }
    }
}
