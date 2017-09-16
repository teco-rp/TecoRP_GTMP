using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TecoRP.Database;

namespace TecoRP.Managers
{
    public class Job_TaxyManager : Script
    {
        public List<string> ALLOWED_TAXIES = new List<string>
        {
            "Taxi",
        };
        public const string JOB_ON = "JOB_TAXY_ON";
        [Command("taksi", "/taksi [basla/bitir")]
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

        //[Command("hedefgoster")]
        //public void ShowWayPoint(Client sender)
        //{

        //}
    }
}
