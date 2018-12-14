using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.WebSockets;
using System.Text;
using System.Threading.Tasks;
using TecoRP.Database;
using TecoRP.Models;
using TecoRP.Repository.Base;
using TecoRP_Debugger.Helpers;
using TecoRP_Debugger.Models;

namespace TecoRP_Debugger
{
    class Program
    {
        static void Main(string[] args)
        {
            Console.WriteLine("Tap To Start...");
            Console.ReadLine();


            Console.WriteLine("Completed");
            Console.ReadLine();
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
