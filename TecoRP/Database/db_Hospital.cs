using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Shared.Math;
using TecoRP.Models;

namespace TecoRP.Database
{
    public class db_Hospital
    {
        public static Hospital hospital = new Hospital();
        public const string PATH = "Data/Hospital.teco";
        public static void ReadData()
        {
            if (Directory.Exists(PATH.Split('/')[0]))
            {
                if (File.Exists(PATH))
                {
                   var txt= File.ReadAllText(PATH);
                    hospital =(Hospital)API.shared.fromJson(txt).ToObject<Hospital>();
                }
            }else
            {
                Directory.CreateDirectory(PATH.Split('/')[0]);
            }
        }

        

        public static void UpdateDeliverPoint(Vector3 position, int dimension = 0)
        {
            hospital.InjuredDeliverPosition = position;
            hospital.InjuredDeliverDimension = dimension;
            SaveChanges();
        }
        public static void UpdateRespawnPoint(Vector3 position, int dimension = 0)
        {
            hospital.RespawnPoint = position;
            hospital.RespawnDimension = dimension;
            SaveChanges();
        }
        public static void UpdateRespawnPrice(int newPrice)
        {
            hospital.RespawnPrice = newPrice;
            SaveChanges();
        }
        public static void SaveChanges()
        {
            if (Directory.Exists(PATH.Split('/')[0]))
            {
                var txt = API.shared.toJson(hospital);
                File.WriteAllText(PATH, txt);
            }else
            {
                Directory.CreateDirectory(PATH.Split('/')[0]);
            }
        }
        ~db_Hospital()
        {
            SaveChanges();
        }
    }
}
