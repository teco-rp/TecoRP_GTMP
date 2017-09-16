using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using TecoRP.Models;

namespace TecoRP.Database
{
    public class db_GasStations
    {
        static XmlSerializer xSer = new XmlSerializer(typeof(GasStationList));
        public static string dataPath = "Data/GasStations.xml";
        public static Tuple<List<GasStation>, List<TextLabel>> CurrentGasStations = new Tuple<List<GasStation>, List<TextLabel>>(new List<GasStation>(), new List<TextLabel>());

        public static void SpawnAll()
        {
            API.shared.consoleOutput("Benzin istasyonları yüklenmeye başladı.");
            foreach (var item in GetAll().Items)
            {
                CurrentGasStations.Item1.Add(item);
                CurrentGasStations.Item2.Add(API.shared.createTextLabel("Stok: " + item.GasInStock + "/" + item.MaxGasInStock + "((/benzin))", item.Position, 15, 1, true, item.Dimension));
            }
            API.shared.consoleOutput(CurrentGasStations.Item1.Count+" adet benzin istasyonu yüklendi.");
        }
        public static GasStationList GetAll()
        {
            GasStationList returnModel = new GasStationList();
            if (System.IO.File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(GasStationList), new XmlRootAttribute("GasStation_List"));
                    returnModel = (GasStationList)deserializer.Deserialize(reader);
                }
            }
            else
            {
                SaveChanges();
            }
            return returnModel;
        }
        public static void Create(GasStation _model)
        {
            _model.GasStationId = CurrentGasStations.Item1.Count > 0 ? CurrentGasStations.Item1.LastOrDefault().GasStationId + 1 : 1;
            CurrentGasStations.Item1.Add(_model);
            CurrentGasStations.Item2.Add(API.shared.createTextLabel("Stok: " + _model.GasInStock + "/" + _model.MaxGasInStock + "((/benzin))", _model.Position, 15, 1, true, _model.Dimension));
            SaveChanges();
        }
        public static GasStation GetById(int _Id)
        {
            var _Index = FindIndexById(_Id);
            if (_Index >= 0)
            {
                return CurrentGasStations.Item1[_Index];

            }
            else
                return null;
        }
        
        public static bool Update(GasStation _model)
        {
            var _Index = FindIndexById(_model.GasStationId);
            if (_Index >= 0)
            {
                try
                {
                    CurrentGasStations.Item1[_Index] = _model;
                    CurrentGasStations.Item2[_Index].position = _model.Position;
                    CurrentGasStations.Item2[_Index].text = "Stok: " + _model.GasInStock + "/" + _model.MaxGasInStock + "((/benzin))";
                    CurrentGasStations.Item2[_Index].dimension = _model.Dimension;
                    SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    if (ex.GetType() == typeof(NullReferenceException))
                    {
                        API.shared.consoleOutput("Öge bulunamadı.");
                    }
                    if (ex.GetType() == typeof(IndexOutOfRangeException))
                    {
                        API.shared.consoleOutput("Listede olmayan bir öge istendi.");
                    }
                    if (ex.GetType() == typeof(FormatException))
                    {
                        API.shared.consoleOutput("Hatalı parametre girildi.");
                    }
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        public static bool Remove(int _Id)
        {
            var _Index = FindIndexById(_Id);
            if (_Index >= 0)
            {
                try
                {
                    API.shared.deleteEntity(CurrentGasStations.Item2[_Index]);
                    CurrentGasStations.Item1.RemoveAt(_Index);
                    CurrentGasStations.Item2.RemoveAt(_Index) ;
                    SaveChanges();
                    return true;
                }
                catch (Exception)
                {
                    return false;
                }
            }else
            {
                return false;
            }
        }
        public static int FindIndexById(int _Id)
        {
            return CurrentGasStations.Item1.IndexOf(CurrentGasStations.Item1.FirstOrDefault(x => x.GasStationId == _Id));
        }

        public static void SaveChanges()
        {
            lock (CurrentGasStations)
            {
                if (Directory.Exists(dataPath.Split('/')[0]))
                {
                    XmlTextWriter xWriter = new XmlTextWriter(dataPath, System.Text.UTF8Encoding.UTF8);
                    xWriter.Formatting = Formatting.Indented;
                    xSer.Serialize(xWriter, new GasStationList { Items = CurrentGasStations.Item1 });
                    xWriter.Dispose();
                }
                else
                {
                    Directory.CreateDirectory(dataPath.Split('/')[0]);
                } 
            }
        }
    }
}
