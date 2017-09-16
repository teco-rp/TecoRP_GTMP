using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TecoRP.Admin
{
    public class ServerCommands : Script
    {
        [Command("restart")]
        public void RestartServer(Client sender)
        {
            if (!(API.getEntityData(sender, "AdminLevel") > 2)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bunun için yetkiniz yok."); return; }

            API.stopResource("TecoRP");
            API.startResource("TecoRP");
        }
    }
}
