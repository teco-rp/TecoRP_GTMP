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
using TecoRP.Models;

namespace TecoRP.Users
{
    public class TaxyCommands : Script
    {
        public static List<PhoneTicket> currentTickets = new List<PhoneTicket>();
        public List<string> ALLOWED_TAXIES = new List<string>
        {
            "Taxi",
        };
        public const string JOB_ON = "JOB_TAXY_ON";
        [Command("taksi", "/taksi [basla/bitir]")]
        public void TaxyGeneral(Client sender, string commandParam)
        {
            if (API.getEntityData(sender, "JobId") != 12) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bunu yapabilmek için taksici olmalısınız."); return; }
            var _vehicle = db_Vehicles.FindNearestVehicle(sender.position);
            if (_vehicle == null) return;
            if ("basla".StartsWith(commandParam.ToLower()))
            {
                if (Vector3.Distance(sender.position, _vehicle.VehicleOnMap.position) < 2)
                {
                    if (_vehicle.JobId == 12)
                    {
                        API.setEntityData(sender, JOB_ON, 0);
                        API.setEntityData(sender.vehicle, Job_KamyonManager.JOB_VEHICLE, API.getEntityData(sender, "ID"));
                        API.sendChatMessageToPlayer(sender, "~y~İşbaşı yaptınız. Taksi çağrıları size iletilecek.");
                    }
                    else
                    {
                        API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Meslek aracınızın içinde olmanız gerekiyor.");
                    }
                }
            }
            else
            if ("bitir".StartsWith(commandParam.ToLower()))
            {
                if (API.getEntityData(sender, "JobId") != 12) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bunu yapabilmek için taksici olmalısınız."); return; }
                if (API.hasEntityData(sender, JOB_ON) && API.getEntityData(sender, JOB_ON) == 0)
                {
                    API.resetEntityData(sender, JOB_ON);
                    API.sendChatMessageToPlayer(sender, "~y~Mesainiz bitti. Artık bildirimleri almayacaksınız.");
                }
            }
        }

        [Command("taksikabul", "/taksikabul [CağrıID]")]
        public void AcceptTaxyTicket(Client sender, int id)
        {
            int playerJobId = API.getEntityData(sender, "JobId");
            if (playerJobId != 12) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Uygun meslekte değilsiniz."); return; }
            if (!API.hasEntityData(sender, JOB_ON)) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bunun için işbaşı yapmış olmanız gerekmektedir."); return; }
            var _ticket = currentTickets.FirstOrDefault(x => x.ID == id);
            if (_ticket != null)
            {
                Clients.ClientManager.ShowBlip(sender, _ticket.Position.X, _ticket.Position.Y, _ticket.Position.Z);
                RPGManager.SenAllPlayersInJob(playerJobId, "~y~[TAKSİ]: ~s~" + db_Players.GetPlayerCharacterName(sender) + " adlı kişi bir çağrıyı kabul etti. (( " + _ticket.ID + " ))");
                var player = db_Players.IsPlayerOnline(_ticket.OwnerSocialClubID);
                if (player != null)
                {
                    API.sendChatMessageToPlayer(sender, "~y~[TAKSİ]: ~s~Çağrınız kabul edildi. Bulunduğunuz konumda bekleyin.");
                }
                currentTickets.Remove(_ticket);
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~y~[TAKSİ]: ~s~Çağrı bulunamadı.");
            }
        }
        [Command("taksireddet", "/taksireddet [ÇağrıID]")]
        public void RejectTaxyTicket(Client sender, int id)
        {
            int playerJobId = API.getEntityData(sender, "JobId");
            if (playerJobId != 12) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Uygun meslekte değilsiniz."); return; }
            var _ticket = currentTickets.FirstOrDefault(x => x.ID == id);
            if (_ticket != null)
            {
                RPGManager.SenAllPlayersInJob(playerJobId, "~y~[TAKSİ]: ~s~" + db_Players.GetPlayerCharacterName(sender) + " adlı kişi bir çağrıyı reddetti. (( "+_ticket.ID+" ))");
                var player = db_Players.IsPlayerOnline(_ticket.OwnerSocialClubID);
                if (player != null)
                {
                    API.sendChatMessageToPlayer(sender, "~y~[TAKSİ]: ~s~Çağrınız kabul edildi. Bulunduğunuz konumda bekleyin.");
                }
                currentTickets.Remove(_ticket);
            }
        }
        public static void AddPhoneTicket(PhoneTicket _model)
        {
            _model.ID = currentTickets.Count > 0 ? currentTickets.LastOrDefault().ID + 1 : 1;
            currentTickets.Add(_model);
            RPGManager.SenAllPlayersInJob(12, "~y~[TAKSİ] YENİ ÇAĞRI: ~s~" + _model.Text + " (( /taksikabul " + _model.ID + " ))");
        }
    }
}
