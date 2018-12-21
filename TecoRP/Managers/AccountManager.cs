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
using TecoRP.Models;

namespace TecoRP.Managers
{
    public class AccountManager : Base.EventMethodTriggerBase
    {
        public static event EventHandler<EventArgs<Client>> OnPlayerLogin;

        public AccountManager()
        {
            API.onPlayerConnected += API_onPlayerConnected;
            API.onPlayerFinishedDownload += API_onPlayerFinishedDownload;
        }
        private void API_onPlayerConnected(Client player)
        {
            if (!CheckPlayerIfIsInWhiteList(player))
                return;

            player.dimension = Main.random.Next(short.MinValue, int.MaxValue);

            SetIdToPlayer(player);

            API.sendChatMessageToPlayer(player, "~g~Başarıyla giriş yaptınız.");
        }

        private void API_onPlayerFinishedDownload(Client player)
        {

        }


        public void Register(Client player, string email, string password)
        {
            API.consoleOutput("Register has triggered on server with following parameters:");
            API.consoleOutput(email + " " + password);
            try
            {
                var user = db_Accounts.Register(player, email, password);
                API.triggerClientEvent(player, "register_result", true);              
            }
            catch (SoftException ex)
            {
                API.triggerClientEvent(player, "register_result", false, ex.Message);
            }
            catch (Exception ex)
            {
                API.consoleOutput(LogCat.Fatal, ex.ToString());
                API.triggerClientEvent(player, "register_result", false, "Bir sorun oluştu. Daha sonra tekrar deneyin.");
            }
        }

        public void Login(Client player, string email, string password)
        {
            try
            {
                var user = db_Accounts.Login(player, email, password);

                var chars = db_Players.GetCharacters(user);

                API.triggerClientEvent(player, "login_result", true);
                player.position = new Vector3(402.8664, -996.4108, -99.00027);
                player.transparency = 0;
                API.consoleOutput("user characters " + user.Characters.Count);
                API.triggerClientEvent(player, "go_character_selection", user.ToJson());
            }
            catch (SoftException ex)
            {
                API.triggerClientEvent(player, "login_result", false, ex.Message);
            }
            catch (Exception ex)
            {
                API.consoleOutput(LogCat.Fatal, ex.ToString());
                API.triggerClientEvent(player, "login_result", false, "Bir sorun oluştu. Daha sonra tekrar deneyin.");
            }
        }

        public bool CheckPlayerIfIsInWhiteList(Client sender)
        {
            var _Whitelist = db_WhiteList.GetAllowedPlayers();
            if (_Whitelist.IsEnabled)
            {
                if (!_Whitelist.Users.Select(s => s.SocialClubName).Contains(sender.socialClubName))
                {
                    API.consoleOutput("WHITELIST REDDEDİLDİ : " + sender.socialClubName);
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Erişiminiz bulunmuyor.");
                    API.sendNotificationToPlayer(sender, "~r~Başvurunuz henüz kabul edilmemiş veya onaylanmamış.");
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
                if (!db_Players.IsPlayerOnline(i))
                {
                    API.setEntityData(player, "ID", i);
                    API.consoleOutput("PLAYER CONNECTED ID : " + API.getEntityData(player, "ID"));
                    return i;
                }

            return -1;
        }
    }
}
