using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using TecoRP.Models;
using TecoRP_Debugger.Models;

namespace TecoRP_Debugger
{
    class Program
    {
        static void Main(string[] args)
        {
        start:
            Console.WriteLine("1- SycnAllowedPlayers");


            if (Console.ReadLine() == "1")
            {
                Console.WriteLine("Senkronizasyon başladı.");
                SycnAllowedPlayer();
                Console.WriteLine("Tamamlandı!");
            }

            Console.ReadLine();
            goto start;
        }


        static TecoRPEntities db = new TecoRPEntities();
        public static void SycnAllowedPlayer()
        {
            var apps = db.Applications.Where(x => x.IsApproved == true);
            var allowedList = TecoRP.Database.db_WhiteList.GetAllowedPlayers();
            foreach (var item in apps)
            {
                if (!allowedList.Users.Any(a => a.SocialClubName == item.SocialClubName))
                {
                    allowedList.Users.Add(new TecoRP.Models.WhiteListUser { SocialClubName = item.SocialClubName, LastValidateTime = DateTime.Now.AddYears(2) });
                }
            }
        }


        //public static async Task AddToWhiteList(WhiteListUser user)
        //{
        //    using (ClientWebSocket client = new ClientWebSocket())
        //    {
        //        var connectionToken = new System.Threading.CancellationToken();
        //        var sendToken = new System.Threading.CancellationToken();

        //        await client.ConnectAsync(new Uri("ws://185.86.4.229:4499"), connectionToken);

        //        //var json = JsonConvert.SerializeObject(user);

        //        string command = "adduser";
        //        string parameter = user.SocialClubName;

        //        await client.SendAsync(/* WHAT TO SEND HERE? */ );

        //        //string fullString = command + " " + parameter;
        //        //await client.SendAsync(new ArraySegment<byte>(Encoding.UTF8.GetBytes(fullString)), WebSocketMessageType.Text, true, sendToken);

        //    }
        //}
    }
}
