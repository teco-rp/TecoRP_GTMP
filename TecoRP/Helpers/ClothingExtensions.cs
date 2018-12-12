using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using System;
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
            {
                API.shared.setEntityData(player, "Skin", PedHash.FreemodeFemale01);
                player.setSkin(PedHash.FreemodeFemale01);
            }
            return player;
        }

        public static bool CanWear(this Client player, Item item)
        {
            return Convert.ToInt32(item.Value_0) == Convert.ToInt32(player.getData("Gender"));
        }

        public static Client ApplyApperance(this Client player, ClothingData data)
        {
            player.setClothes(0, data.Head, 0);
            player.setClothes(2, data.Hair, data.HairColor);
            return player;
        }

        public static Client WearMask(this Client player, Item item)
        {
            player.setClothes(1, Convert.ToInt32(item.Value_1), Convert.ToInt32(item.Value_2));
            return player;
        }
        public static Client UnwearMask(this Client player, Item item)
        {
            player.setClothes(1, -1,0);
            return player;
        }

        public static Client WearTops(this Client player, Item item)
        {
            player.setClothes(8, -1, 0);
            player.setClothes(3, Convert.ToInt32(item.Value_3), 0);
            player.setClothes(11, Convert.ToInt32(item.Value_1), Convert.ToInt32(item.Value_2));
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
            if(player.getClothesDrawable(11) < 0)
            {
                throw new SoftException("Bunu yalnızca başka bir kıyafetin altına giyebilirsiniz.");
            }
            player.setClothes(8, Convert.ToInt32(item.Value_1), Convert.ToInt32(item.Value_0));
            return player;
        }

        public static Client UnwearUndershirt(this Client player)
        {
            if (API.shared.getEntityData(player, "Gender") == true)
            {
                player.setClothes(8, -1, 0);
            }
            else
            {
                player.setClothes(8, -1, 0);
            }
            return player;
        }
        public static Client WearPants(this Client player, Item item)
        {
            player.setClothes(4, Convert.ToInt32(item.Value_1), Convert.ToInt32(item.Value_2));
            return player;
        }
        public static Client UnwearPants(this Client player)
        {
            if (API.shared.getEntityData(player, "Gender") == true)
            {
                player.setClothes(4, 18, 0);
            }
            else
            {
                player.setClothes(4, 15, 0);
            }
            return player;
        }
        public static Client WearShoes(this Client player, Item item)
        {
            player.setClothes(6, Convert.ToInt32(item.Value_1), Convert.ToInt32(item.Value_2));
            return player;
        }

        public static Client WearBags(this Client player, Item item)
        {
            player.setClothes(5, Convert.ToInt32(item.Value_1), Convert.ToInt32(item.Value_2));
            return player;
        }

        public static Client UnwearBags(this Client player)
        {
            player.setClothes(5, -1, 0);
            return player;
        }
        public static Client UnwearShoes(this Client player)
        {
            if (API.shared.getEntityData(player, "Gender") == true)
            {
                player.setClothes(6, 34, 0);
            }
            else
            {
                player.setClothes(6, 35, 0);
            }
            return player;
        }
    }
}
