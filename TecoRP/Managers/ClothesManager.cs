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
using TecoRP.Managers.Base;
using TecoRP.Models;

namespace TecoRP.Managers
{
    public class ClothesManager : EventMethodTriggerBase
    {
        public ClothesManager()
        {
        }


        public void SaveCharacterApperance(Client sender, params object[] args)
        {
            ClothingData cData = new ClothingData
            {
                Head = Convert.ToInt32(args[0]),
                Eyes  = Convert.ToInt32(args[1]),
                Hair  = Convert.ToInt32(args[2]),
                HairColor  = Convert.ToInt32(args[3]),
            };
            
            API.shared.setEntityData(sender, nameof(User.ClothingData), cData);
            db_Players.SavePlayerAccount(sender);
            sender.dimension = API.getEntityData(sender, "Dimension");
            sender.position = API.getEntityData(sender, "LastPosition");
            sender.freeze(false);
            sender.SetLoggedIn(true);
            sender
                .SetSkinByGender()
                .UnwearTops()
                .UnwearPants()
                .UnwearShoes()
                .ApplyApperance(cData);

            InventoryManager.LoadPlayerEquippedItems(sender);
            UserManager.LoadPlayerStats(sender);
        }

        [Command("gorunum")]
        public void EditCharacterApperance(Client sender)
        {
            API.triggerClientEvent(sender, "ChooseCharacterApperance", sender.getData("Gender") == true ? "male" : "female");
        }

        public void LoadClothes(Client sender)
        {
            InventoryManager.LoadPlayerEquippedItems(sender);
        }
    }
}
