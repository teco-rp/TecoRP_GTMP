using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
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
   
    public class db_PhoneOperatorShop
    {
        static XmlSerializer xSer = new XmlSerializer(typeof(PhoneOperatorShopList));
        public static string dataPath = "Data/OperatorShops.xml";
        public static Tuple<List<PhoneOperatorShop>, List<Marker>, List<TextLabel>> CurrentOperatorShop = new Tuple<List<PhoneOperatorShop>, List<Marker>, List<TextLabel>>(new List<PhoneOperatorShop>(), new List<Marker>(), new List<TextLabel>());


        public static void SpawnAll()
        {
            foreach (var item in GetAll().Items)
            {
                CurrentOperatorShop.Item1.Add(item);
                CurrentOperatorShop.Item2.Add(API.shared.createMarker(1,
                    item.Position+new Vector3(0,0,-1),new Vector3(0,0,0),new Vector3(0,0,0),new Vector3(1,1,1),255,
                    item.OperatorType == Operator.LosTelecom ? 10 : 255,
                    item.OperatorType == Operator.LosTelecom ? 255 : 10,
                    30,item.Dimension
                    ));
                CurrentOperatorShop.Item3.Add(API.shared.createTextLabel(item.OperatorType.ToString()+" ((/hat))",
                    item.Position + new Vector3(0,0,0.5f),15,1,false,item.Dimension
                    ));
            }
        }

        public static PhoneOperatorShopList GetAll()
        {
            PhoneOperatorShopList returnModel = new Models.PhoneOperatorShopList();
            if (System.IO.File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(PhoneOperatorShopList), new XmlRootAttribute("PhoneOperatorShop_List"));
                    returnModel = (PhoneOperatorShopList)deserializer.Deserialize(reader);
                }
            }
            else
            {
                SaveChanges();
            }
            return returnModel;
        }

        public static PhoneOperatorShop GetById(int _Id)
        {
            return CurrentOperatorShop.Item1.FirstOrDefault(x => x.ID == _Id);
        }
        public static bool Update(PhoneOperatorShop _model)
        {
            var _Index = FindIndexById(_model.ID);
            if (_Index>=0)
            {
                try
                {
                    CurrentOperatorShop.Item1[_Index] = _model;
                    CurrentOperatorShop.Item2[_Index].position = _model.Position +new Vector3(0,0,1);
                    CurrentOperatorShop.Item2[_Index].dimension = _model.Dimension;
                    CurrentOperatorShop.Item2[_Index].color = _model.OperatorType == Operator.LosTelecom ? new Color(10, 255, 30) : new Color(255, 10, 30);
                    CurrentOperatorShop.Item3[_Index].position = _model.Position + new Vector3(0, 0, 0.5);
                    CurrentOperatorShop.Item3[_Index].dimension = _model.Dimension;
                    CurrentOperatorShop.Item3[_Index].text = _model.OperatorType.ToString() + " ((/hat))";
                    SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    API.shared.consoleOutput(LogCat.Warn, ex.ToString());
                    return false;
                }
            }
            else
            return false;
        }

        public static void Create(PhoneOperatorShop _model)
        {
            _model.ID = CurrentOperatorShop.Item1.Count > 0 ? CurrentOperatorShop.Item1.LastOrDefault().ID + 1 : 1;
            CurrentOperatorShop.Item1.Add(_model);
            CurrentOperatorShop.Item2.Add(API.shared.createMarker(1,
                _model.Position+new Vector3(0,0,-1), new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(1, 1, 1), 255,
                _model.OperatorType == Operator.LosTelecom ? 10 : 255,
                _model.OperatorType == Operator.LosTelecom ? 255 : 10,
                30, _model.Dimension
                ));
            CurrentOperatorShop.Item3.Add(API.shared.createTextLabel(_model.OperatorType.ToString() + " ((/hat))",
                _model.Position + new Vector3(0, 0, 0.5f), 15, 1, false, _model.Dimension
                ));
            SaveChanges();
        }

        public static bool Remove(int _Id)
        {
            var _Index = FindIndexById(_Id);
            if (_Index>=0)
            {
                API.shared.deleteEntity(CurrentOperatorShop.Item2[_Index]);
                API.shared.deleteEntity(CurrentOperatorShop.Item2[_Index]);
                CurrentOperatorShop.Item1.RemoveAt(_Index);
                CurrentOperatorShop.Item2.RemoveAt(_Index);
                CurrentOperatorShop.Item3.RemoveAt(_Index);
                SaveChanges();
                return true;
            }else
            {
                return false;
            }
        }
        public static int FindIndexById(int _Id)
        {
            return CurrentOperatorShop.Item1.IndexOf(CurrentOperatorShop.Item1.FirstOrDefault(x => x.ID == _Id));
        }
        public static void SaveChanges()
        {
            lock (CurrentOperatorShop)
            {
                if (System.IO.Directory.Exists(dataPath.Split('/')[0]))
                {
                    XmlTextWriter xWriter = new XmlTextWriter(dataPath, System.Text.UTF8Encoding.UTF8);
                    xWriter.Formatting = Formatting.Indented;
                    xSer.Serialize(xWriter, new PhoneOperatorShopList { Items = CurrentOperatorShop.Item1 });
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
