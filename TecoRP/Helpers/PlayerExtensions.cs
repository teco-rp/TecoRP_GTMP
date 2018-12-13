using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TecoRP.Database;
using TecoRP.Models;

namespace TecoRP.Helpers
{
    public static class PlayerExtensions
    {
        public static bool IsPlayerLoggedIn(this Client client)
        {
            return (bool) (API.shared.getEntityData(client, "LOGGED_IN") ?? false);
        }
        public static Client SetLoggedIn(this Client client, bool isLoggedIn)
        {
            API.shared.setEntityData(client, "LOGGED_IN", isLoggedIn);
            return client;
        }

        /// <summary>
        /// </summary>
        /// <param name="nameTag"><see cref="null"/> means gets CharacterName from EntityData</param>
        public static Client SetNameTagWithId(this Client player, string nameTag = null)
        {
            player.nametag = "(" + API.shared.getEntityData(player, "ID") + ") " + (nameTag ?? player.GetCharacterName());
            return player;
        }

        public static string GetCharacterName(this Client player)
        {
            return  (API.shared.getEntityData(player, "CharacterName") as string) ?? db_Accounts.GetPlayerCharacterName(player);
        }

        public static Inventory GetInventory(this Client player)
        {
            return (player.getData("inventory") as Inventory) ?? new Inventory();
        }
        public static Client SetInventory(this Client player, Inventory inventory)
        {
            player.setData("inventory", inventory);
            return player;
        }
    }
}
