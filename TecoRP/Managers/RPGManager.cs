using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared.Math;
using System;
using System.Threading.Tasks;
using TecoRP.Database;

namespace TecoRP.Managers
{
    public class RPGManager : Script
    {

        public RPGManager()
        {
            API.onChatCommand += API_onChatCommand;
            API.onClientEventTrigger += API_onClientEventTrigger;
            API.onChatMessage += API_onChatMessage;
        }

        private void API_onChatMessage(Client sender, string message, CancelEventArgs cancel)
        {
            if (API.hasEntityData(sender, "OnCall"))
            {
                PhoneTalk(sender, message);
            }
            else
            if (API.hasEntityData(sender, "911"))
            {
                #region 911
                API.resetEntityData(sender, "911");
                if (message.ToLower() == "lspd" || message.ToLower().StartsWith("polis"))
                {
                    API.setEntityData(sender, "lspdCall", true);
                    API.sendChatMessageToPlayer(sender, "~b~[LSPD]: ~s~Bize sorununuzu kısaca açıklayın. (( Konumunuz otomatik gönderilecek. ))");
                }
                else if (message.ToLower() == "lsmd" || message.ToLower().StartsWith("hastane") || message.ToLower().StartsWith("ambulans"))
                {
                    API.setEntityData(sender, "lsmdCall", true);
                    API.sendChatMessageToPlayer(sender, "~r~[LSMD]: ~s~Bize sorununuzu kısaca açıklayın. (( Konumunuz otomatik gönderilecek. ))");
                }
                #endregion
            }
            else
            if (API.hasEntityData(sender, "100"))
            {
                #region 911
                API.resetEntityData(sender, "100");

                Users.TaxyCommands.AddPhoneTicket(new Models.PhoneTicket
                {
                    OwnerSocialClubID = sender.socialClubName,
                    Position = sender.position,
                    Text = message
                });
                #endregion
            }
            else
            if (API.hasEntityData(sender, "lspdCall"))
            {
                #region LSPD
                API.resetEntityData(sender, "lspdCall");
                Users.PoliceCommands.AddTicket(new Models.PhoneTicket
                {
                    OwnerSocialClubID = sender.socialClubName,
                    Position = sender.position,
                    Text = message
                });
                API.sendChatMessageToPlayer(sender, "~b~[LSPD]: ~s~Bildiriminiz tarafımıza ulaştı. En kısa sürede size dönüş yapacağız.");

                #endregion
            }
            else
            if (API.hasEntityData(sender, "lsmdCall"))
            {
                #region LSMD
                API.resetEntityData(sender, "lsmdCall");
                Users.MedicalCommands.AddPhoneTicket(message, sender.position, sender.socialClubName);
                API.sendChatMessageToPlayer(sender, "~r~[LSMD]: ~s~Bildiriminiz tarafımıza ulaştı. En kısa sürede size dönüş yapacağız.");

                #endregion
            }
            else
            if (API.hasEntityData(sender, "M"))
            {
                MegaphoneTalk(sender, message);
            }
            else
            {
                Talk(sender, message);
            }
            cancel.Cancel = true;
            if (API.hasEntityData(sender, Users.UserCommands.IDENTITY_B))
            {
                Users.UserCommands.IdentityComplete(sender, Users.UserCommands.identityType.Birthdate, message);
            }
            else
            if (API.hasEntityData(sender, Users.UserCommands.IDENTITY_O))
            {
                Users.UserCommands.IdentityComplete(sender, Users.UserCommands.identityType.Origin, message);
            }
        }

        private void API_onClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            if (eventName == "me")
            {
                Me(sender, arguments[0].ToString());
            }
            if (eventName == "do")
            {
                Do(sender, arguments[0].ToString());
            }
        }

        private void API_onChatCommand(Client sender, string command, CancelEventArgs cancel)
        {
            API.shared.consoleOutput(sender.socialClubName + " " + command.ToString());
            if (command.StartsWith("/me "))
            {
                cancel.Cancel = true;
                Me(sender, command.Remove(0, 3));
            }
            if (command.StartsWith("/do "))
            {
                cancel.Cancel = true;
                Do(sender, command.Remove(0, 3));
            }
            if (command.StartsWith("/s "))
            {
                cancel.Cancel = true;
                Shout(sender, command.Remove(0, 2));
            }
            if (command.StartsWith("/b "))
            {
                cancel.Cancel = true;
                Booc(sender, command.Remove(0, 2));
            }

        }

        public void Me(Client sender, string _action)
        {
            foreach (var player in API.getAllPlayers())
            {
                if (player.dimension == sender.dimension && Vector3.Distance(player.position, sender.position) < 45)
                {
                    API.sendChatMessageToPlayer(player, "~p~"+ db_Accounts.GetPlayerCharacterName(sender) + _action);
                }
            }
        }

        public void UpdatePlayerTalkLabel(Client player, string text, float _range = 30, float size = 1)
        {
            if ((player.isInVehicle && player.vehicle.isWindowBroken(0)) || !player.isInVehicle)
            {
                if (API.hasEntityData(player, "TalkLabel"))
                {

                    TextLabel _textLabel = API.getEntityData(player, "TalkLabel");
                    API.shared.attachEntityToEntity(_textLabel, player, "IK_ROOT", new Vector3(0, 0, 1), new Vector3());
                    Task.Run(async () =>
                    {
                        _textLabel.position = player.position;
                        _textLabel.dimension = player.dimension;
                        _textLabel.range = _range;
                        _textLabel.text = text;
                        await Task.Delay((text.Length < 40 ? 40 : text.Length) * 100);
                        if (_textLabel.text == text)
                        {
                            _textLabel.text = "";
                        }

                    });
                }
                else
                {
                    API.consoleOutput("" + player.nametag + " isimli kullanıcıda text label bulunamadı.");
                    //var textLabel = API.createTextLabel(text, player.position, _range, 0.5f, false, player.dimension);
                    //API.setEntityData(player, "TalkLabel", textLabel );
                    //API.attachEntityToEntity(textLabel, player, null, new Vector3(0, 0, 1), new Vector3());

                }
            }
        }

        public void Me(Client sender, string _action, int range)
        {
            foreach (var player in API.getAllPlayers())
            {
                if (player.dimension == sender.dimension && Vector3.Distance(player.position, sender.position) < range)
                {
                    int _removeLength = Convert.ToString(API.getEntityData(player, "ID")).Length + 3;
                    API.sendChatMessageToPlayer(player, "~p~" + sender.nametag.Remove(0, _removeLength) + _action);
                }
            }
        }
        public void Do(Client sender, string _action)
        {
            foreach (var player in API.getAllPlayers())
            {
                if (player.dimension == sender.dimension && Vector3.Distance(player.position, sender.position) < 45)
                {
                    int _removeLength = Convert.ToString(API.getEntityData(player, "ID")).Length + 3;
                    API.sendChatMessageToPlayer(player, "~p~" + _action + " (( " + sender.nametag.Remove(0, _removeLength) + " ))");
                }
            }
        }
        public void Talk(Client sender, string _text)
        {
            UpdatePlayerTalkLabel(sender, _text);
            foreach (var player in API.getAllPlayers())
            {
                if (player.dimension == sender.dimension)
                {
                    if (Vector3.Distance(player.position, sender.position) < 15)
                    {
                        int _removeLength = Convert.ToString(API.getEntityData(player, "ID")).Length + 3;
                        API.sendChatMessageToPlayer(player, "~#F0F0F0~" + sender.nametag.Remove(0, _removeLength) + " söyler: " + _text);
                        continue;
                    }
                    if (Vector3.Distance(player.position, sender.position) < 30)
                    {
                        int _removeLength = Convert.ToString(API.getEntityData(player, "ID")).Length + 3;
                        API.sendChatMessageToPlayer(player, "~#e2e2e2~" + sender.nametag.Remove(0, _removeLength) + " söyler: " + _text);
                        continue;
                    }
                    if (Vector3.Distance(player.position, sender.position) < 45)
                    {
                        int _removeLength = Convert.ToString(API.getEntityData(player, "ID")).Length + 3;
                        API.sendChatMessageToPlayer(player, "~#bababa~" + sender.nametag.Remove(0, _removeLength) + " söyler: " + _text);
                    }

                }
            }

        }

        public void Booc(Client sender, string _text)
        {
            foreach (var player in API.getAllPlayers())
            {
                if (player.dimension == sender.dimension && Vector3.Distance(player.position, sender.position) < 30)
                {
                    API.sendChatMessageToPlayer(player, "~c~(( " + sender.nametag + " : " + _text + " ))");
                }
            }
        }

        public void Shout(Client sender, string _text)
        {
            foreach (var player in API.getAllPlayers())
            {
                if (player.dimension == sender.dimension && Vector3.Distance(player.position, sender.position) < 60)
                {
                    int _removeLength = Convert.ToString(API.getEntityData(player, "ID")).Length + 3;
                    API.sendChatMessageToPlayer(player, sender.nametag.Remove(0, _removeLength) + " bağırır: ~m~~h~" + _text);
                }
            }
        }

        public void MegaphoneTalk(Client sender, string _text)
        {
            UpdatePlayerTalkLabel(sender, _text, 75);
            foreach (var itemPlayer in API.getAllPlayers())
            {
                if (Vector3.Distance(sender.position, itemPlayer.position) < 75)
                {
                    API.sendChatMessageToPlayer(itemPlayer, $"({db_Accounts.GetPlayerCharacterName(sender)})~y~[MEGAFON]: ~s~{_text}");
                }
            }
        }

        [Command("w", "/w [OyuncuID] [Mesajınız] ((IC))", GreedyArg = true)]
        public void WhisperToPlayer(Client sender, int targetPlayerId,string _text)
        {
            var player = db_Accounts.GetPlayerById(targetPlayerId);
            if (player != null)
            {
                if (Vector3.Distance(sender.position, player.position) > 3) { return; }
                API.sendChatMessageToPlayer(player, $"{db_Accounts.GetPlayerCharacterName(sender)}~818181~[FISILTI]: " + _text);
            }
            Me(sender, $" adlı oyuncu {db_Accounts.GetPlayerCharacterName(player)} adlı oyuncuya bir şeyler fısıldar.");
        }
        [Command("notificationall", "/notifyall [text]", Alias = "notifyall", GreedyArg = true)]
        public void NotifyToAll(Client sender, string _text)
        {
            if (!(API.getEntityData(sender, "AdminLevel") > 0)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            API.sendNotificationToAll(_text, true);
        }
        [Command("announcement", "/duyuru [text]", Alias = "duyuru", GreedyArg = true)]
        public void AnnouncementToAll(Client sender, string _text)
        {
            if (!(API.getEntityData(sender, "AdminLevel") > 0)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            API.sendChatMessageToAll("~g~DUYURU: ~w~~h~" + _text);
        }

        [Command("a", "/a [Text] -adminChat", GreedyArg = true)]
        public void AdminChat(Client sender, string text)
        {
            if (!(API.getEntityData(sender, "AdminLevel") >= 1)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }
            foreach (var item in API.getAllPlayers())
            {
                if (API.getEntityData(item, "AdminLevel") >= 1)
                {
                    API.sendChatMessageToPlayer(item, "~h~" + sender.nametag + "[" + API.getEntityData(sender, "AdminLevel") + "]", "~b~" + text);
                }
            }
        }

        [Command("pm", "/pm [OyuncuId] [Yazı]", GreedyArg = true)]
        public void PersonelMessage(Client sender, int identity, string text)
        {
            foreach (var item in API.getAllPlayers())
            {
                if (API.getEntityData(item, "ID") == identity)
                {
                    API.sendChatMessageToPlayer(sender, "~h~" + item.nametag + " -- " + text);
                    API.sendChatMessageToPlayer(item, "~h~" + sender.nametag + " >> ~s~" + text);
                    return;
                }
            }
        }
        public void PhoneTalk(Client sender, string text)
        {
            UpdatePlayerTalkLabel(sender, "~#f1f1f2~" + text);
            foreach (var player in API.getAllPlayers())
            {
                if (API.getEntityData(sender, "OnCall") == API.getEntityData(player, "ID") /*|| (player.dimension == sender.dimension && Vector3.Distance(player.position, sender.position) < 30)*/)
                {
                    int _removeLength = Convert.ToString(API.getEntityData(player, "ID")).Length + 3;
                    API.sendChatMessageToPlayer(player, sender.nametag.Remove(0, _removeLength) + " telefonda: ~m~" + text);
                }
            }
        }
        public void PlaySound(Vector3 soundCenter, string soundName, string soundsetname, int range)
        {
            foreach (var item in API.getAllPlayers())
            {
                if (Vector3.Distance(soundCenter, item.position) < range)
                {
                    API.playSoundFrontEnd(item, soundName, soundsetname);
                }
            }
        }

        public void PlayAudio(Vector3 soundCenter, string soundPath, int range)
        {
            //CarLock
            foreach (var item in API.getAllPlayers())
            {
                if (Vector3.Distance(soundCenter,item.position)<range)
                {
                    Clients.ClientManager.StartAudio(item, soundPath);
                }
            }
        }

        public static TextLabel CreatePlayerTalkLabel(Client player)
        {
            var _textLabel = API.shared.createTextLabel("", player.position, 30, 0.5f, false, player.dimension);
            API.shared.attachEntityToEntity(_textLabel, player, "IK_ROOT", new Vector3(0, 0, 1), new Vector3());
            API.shared.setEntityData(player, "TalkLabel", _textLabel);
            return _textLabel;
        }
        public static void SendAllPlayersInFaction(int factionId, string text)
        {
            foreach (var itemPlayer in API.shared.getAllPlayers())
            {
                if (API.shared.getEntityData(itemPlayer, "FactionId") == factionId)
                {
                    API.shared.sendChatMessageToPlayer(itemPlayer, text);
                }
            }
        }
        public static void SendAllPlayersInRange(Vector3 position, float range, int dimension, string text)
        {
            foreach (var itemPlayer in API.shared.getAllPlayers())
            {
                if (itemPlayer.dimension == dimension && Vector3.Distance(position, itemPlayer.position) < range)
                {
                    API.shared.sendChatMessageToPlayer(itemPlayer, text);
                }
            }
        }
        public static void SenAllPlayersInJob(int jobId, string message)
        {
            foreach (var itemPlayer in API.shared.getAllPlayers())
            {
                if (API.shared.getEntityData(itemPlayer, "JobId") == jobId)
                {
                    API.shared.sendChatMessageToPlayer(itemPlayer, message);
                }
            }
        }

    }
}
