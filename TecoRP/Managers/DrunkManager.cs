using GrandTheftMultiplayer.Server.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TecoRP.Clients;
using TecoRP.Models;

namespace TecoRP.Managers
{
    public class DrunkManager : Base.EventMethodTriggerBase
    {
        public static void CheckPlayer(Client player)
        {
            if (player.hasData(nameof(User.DrunkMinutes)))
            {
                var drunkSeconds = player.getData(nameof(User.DrunkMinutes)) as int?;
                if(drunkSeconds != null)
                {
                    ClientManager.SetPlayerDrunk(player);
                    return;
                }
            }
            ClientManager.SetPlayerUndrunk(player);
        }

        public static void DecreaseDrunkTime(Client player, int mins)
        {
            int? _mins = player.hasData(nameof(User.DrunkMinutes)) ? player.getData(nameof(User.DrunkMinutes)) : null;

            _mins = _mins.GetValueOrDefault(0) - mins;

            if (_mins.Value <= 0)
            {
                _mins = null;
            }
            player.setData(nameof(User.DrunkMinutes), _mins);
        }
    }
}
