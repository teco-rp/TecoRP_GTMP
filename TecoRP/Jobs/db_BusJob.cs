using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared.Math;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using TecoRP.Models;

namespace TecoRP.Jobs
{
    public static class db_BusJob
    {
        static XmlSerializer xSer = new XmlSerializer(typeof(BusStopList));
        public static string dataPath = "Data/Job_Bus.xml";
        public static Tuple<List<BusStop>,List<CylinderColShape>> CurrentBusStops = new Tuple<List<BusStop>, List<CylinderColShape>>(new List<BusStop>(),new List<CylinderColShape>());


        static db_BusJob()
        {
        }

        public static void Init()
        {
           foreach (var item in GetAll().Items)
            {
                CurrentBusStops.Item1.Add(item);
                CurrentBusStops.Item2.Add(API.shared.createCylinderColShape(item.Position, 3, 4));
            }
        }
        public static BusStopList GetAll()
        {
            BusStopList returnModel = new Models.BusStopList();
            if (File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(BusStopList), new XmlRootAttribute("BusStop_List"));
                    returnModel = (BusStopList)deserializer.Deserialize(reader);
                }
            }
            else
            {
                SaveChanges();
            }
            return returnModel;
        }

        public static bool RemoveBusStop(int _Id)
        {
            var _Index = FindBusStopIndexById(_Id);
            if (_Index>=0)
            {
                try
                {
                    API.shared.deleteColShape(CurrentBusStops.Item2[_Index]);
                    CurrentBusStops.Item1.RemoveAt(_Index);
                    CurrentBusStops.Item2.RemoveAt(_Index);
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }

        public static int FindBusStopIndexById(int _Id)
        {
            return CurrentBusStops.Item1.IndexOf(CurrentBusStops.Item1.FirstOrDefault(x => x.ID == _Id));
        }
        public static int FindBusStopIdByIndex(int _Index)
        {
            return CurrentBusStops.Item1[_Index].ID;
        }

        public static bool CreateStop(BusStop _stop)
        {
            try
            {
                if (_stop.Position == null) return false;
                _stop.ID = CurrentBusStops.Item1.Count > 0 ? CurrentBusStops.Item1.LastOrDefault().ID + 1 : 1;
                CurrentBusStops.Item1.Add(_stop);
                CurrentBusStops.Item2.Add(API.shared.createCylinderColShape(_stop.Position, 3, 4));
                SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }

        }

        public static void SaveChanges()
        {
            if (System.IO.Directory.Exists(dataPath.Split('/')[0]))
            {
                XmlTextWriter xWriter = new XmlTextWriter(dataPath, System.Text.UTF8Encoding.UTF8);
                xWriter.Formatting = Formatting.Indented;
                xSer.Serialize(xWriter, new BusStopList { Items = CurrentBusStops.Item1.ToList() });
                xWriter.Dispose();
            }
            else
            {
                System.IO.Directory.CreateDirectory(dataPath.Split('/')[0]);
            }
        }
    }
}
