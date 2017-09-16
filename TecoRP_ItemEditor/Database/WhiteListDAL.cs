using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TecoRP_ItemEditor.Database
{
    public static class WhiteListDAL
    {
        public static TecoRP.Models.WhiteList currentUsers { get; set; }
        static WhiteListDAL()
        {
            currentUsers = TecoRP.Database.db_WhiteList.GetAllowedPlayers();
        }
        public static void Save()
        {
            TecoRP.Database.db_WhiteList.SaveChanges(currentUsers);
        }
    }
}
