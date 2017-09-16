using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
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

namespace TecoRP.Database
{
    public static class db_Arrests
    {
        static XmlSerializer xSer = new XmlSerializer(typeof(ArrestList));
        public static string dataPath = "Data/Arrests.xml";
        public static Tuple<List<Arrest>, List<Marker>, List<TextLabel>> currentArrests = new Tuple<List<Arrest>, List<Marker>, List<TextLabel>>(new List<Arrest>(), new List<Marker>(), new List<TextLabel>());
        static db_Arrests()
        {

        }

        public static void Init()
        {
            foreach (var item in GetAll().Items)
            {
                currentArrests.Item1.Add(item);
                currentArrests.Item2.Add(API.shared.createMarker(30, item.Position, new Vector3(0, 0, 0), item.Rotation, new Vector3(1, 1, 1), 200, 10, 20, 255, item.Dimension));
                currentArrests.Item3.Add(API.shared.createTextLabel(item.Name,item.Position+new Vector3(0,0,0.5),15,1,false,item.Dimension));
            }
        }
        public static ArrestList GetAll()
        {
            ArrestList returnModel = new ArrestList();
            if (File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(ArrestList), new XmlRootAttribute("Arrest_List"));
                    returnModel = (ArrestList)deserializer.Deserialize(reader);
                }
            }
            else
            {
                SaveChanges();
            }
            return returnModel;
        }

        public static void CreateArrest(Arrest _arrest)
        {
            _arrest.ArrestId = currentArrests.Item1.Count > 0 ? currentArrests.Item1.LastOrDefault().ArrestId + 1 : 1;
            currentArrests.Item1.Add(_arrest);
            currentArrests.Item2.Add(API.shared.createMarker(30, _arrest.Position, new Vector3(0, 0, 0), _arrest.Rotation, new Vector3(1, 1, 1), 200, 10, 20, 255, _arrest.Dimension));
            currentArrests.Item3.Add(API.shared.createTextLabel(_arrest.Name, _arrest.Position + new Vector3(0, 0, 0.5), 15, 1, false, _arrest.Dimension));
            SaveChanges();
        }

        public static Arrest GetArrestById(int _Id)
        {
            return currentArrests.Item1.FirstOrDefault(x => x.ArrestId == _Id);
        }
        public static bool UpdateArrest(Arrest _arrest)
        {
            var _Index = FindIndexById(_arrest.ArrestId);
            if (_Index>=0)
            {
                try
                {
                    currentArrests.Item1[_Index] = _arrest;
                    currentArrests.Item2[_Index].position = _arrest.Position;
                    currentArrests.Item2[_Index].rotation = _arrest.Rotation;
                    currentArrests.Item2[_Index].dimension = _arrest.Dimension;
                    currentArrests.Item3[_Index].text = _arrest.Name;
                    SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }
            return false;
        }

        public static int FindIndexById(int _Id)
        {
            return currentArrests.Item1.IndexOf(currentArrests.Item1.FirstOrDefault(x => x.ArrestId == _Id));
        }
        public static int FindIdByIndex(int _Index)
        {
            return currentArrests.Item1[_Index].ArrestId;
        }

        public static void SaveChanges()
        {
            lock (currentArrests)
            {
                if (System.IO.Directory.Exists(dataPath.Split('/')[0]))
                {
                    XmlTextWriter xWriter = new XmlTextWriter(dataPath, System.Text.UTF8Encoding.UTF8);
                    xWriter.Formatting = Formatting.Indented;
                    xSer.Serialize(xWriter, new ArrestList { Items = currentArrests.Item1 });
                    xWriter.Dispose();
                }
                else
                {
                    System.IO.Directory.CreateDirectory(dataPath.Split('/')[0]);
                } 
            }
        }
    }
}
