using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TecoRP.Models;

namespace TecoRP.Helpers
{
    public static class ClothingExtensions
    {
        public static Client SetSkinByGender(this Client player)
        {
            if (API.shared.getEntityData(player, "Gender") == true)
            {
                API.shared.setEntityData(player, "Skin", PedHash.FreemodeMale01);
                player.setSkin(PedHash.FreemodeMale01);
            }
            else
                API.shared.setEntityData(player, "Skin", PedHash.FreemodeFemale01);
                player.setSkin(PedHash.FreemodeFemale01);

            return player;
        }

        public static Client ApplyApperance(this Client player, ClothingData data)
        {
            player.setClothes(0, data.Head, 0);
            player.setClothes(2, data.Hair, data.HairColor);
            return player;
        }

        public static Client WearTops(this Client player, Item item)
        {
            player.setClothes(11, Convert.ToInt32(item.Value_0), Convert.ToInt32(item.Value_1)); 
            return player;
        }       

        public static Client UnwearTops(this Client player)
        {
            if (API.shared.getEntityData(player, "Gender") == true)
            {
                player.setClothes(3, 15, 0);
                player.setClothes(8, -1, 0);
                player.setClothes(11, 252, 0);
            }
            else
            {
                player.setClothes(3, 15, 0);
                player.setClothes(8, -1, 0);
                player.setClothes(11, 15, 0);
            }
            return player;
        }

        public static Client WearUndershirt(this Client player, Item item)
        {
            //player.setClothes(11, Convert.ToInt32(item.Value_0), Convert.ToInt32(item.Value_1)); 
            return player;
        }       

        public static Client UnearUndershirt(this Client player)
        {
            //if (API.shared.getEntityData(player, "Gender") == true)
            //{
            //    player.setClothes(3, 15, 0);
            //    player.setClothes(8, -1, 0);
            //    player.setClothes(11, 252, 0);
            //}
            //else
            //{
            //    player.setClothes(3, 15, 0);
            //    player.setClothes(8, -1, 0);
            //    player.setClothes(11, 15, 0);
            //}
            return player;
        }
    }
}
