using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
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
    public class ClothesManager : Script
    {
        public ClothesManager()
        {
            API.onClientEventTrigger += (s, eventName, args)
                => this.GetType().GetMethod(eventName)?.Invoke(this, parameters: new object[] { s,args });
        }


        public void SaveCharacterApperance(Client sender, object[] args)
        {
            ClothingData cData = new ClothingData
            {
                Head = Convert.ToInt32(args[0]),
                Eyes  = Convert.ToInt32(args[1]),
                Hair  = Convert.ToInt32(args[2]),
                HairColor  = Convert.ToInt32(args[3]),
            };

            sender.ApplyApperance(cData);
            API.shared.setEntityData(sender, nameof(User.ClothingData), cData);
            db_Accounts.SavePlayerAccount(sender);
            sender.dimension = API.getEntityData(sender, "Dimension");
            sender.position = API.getEntityData(sender, "LastPosition");
            sender.freeze(false);
            sender.SetLoggedIn(true);
        }

        [Command("gorunum")]
        public void EditCharacterApperance(Client sender)
        {
            API.triggerClientEvent(sender, "ChooseCharacterApperance", sender.getData("Gender") == true ? "male" : "female");
        }
    }
}
