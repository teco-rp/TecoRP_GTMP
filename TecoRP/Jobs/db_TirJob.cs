using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Managers;
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
    public class db_TirJob
    {
        static XmlSerializer xSer = new XmlSerializer(typeof(TirDeliveryPointsList));
        public static string dataPath = "Data/Job_Tir.xml";
        public static Tuple<List<TirDeliveryPoint>, List<CylinderColShape>> CurrentDeliveryPoints = new Tuple<List<TirDeliveryPoint>, List<CylinderColShape>>(new List<TirDeliveryPoint>(), new List<CylinderColShape>());
        static db_TirJob()
        {
        }

        public static void Init()
        {
            foreach (var item in GetAll().Items)
            {
                CurrentDeliveryPoints.Item1.Add(item);
                CurrentDeliveryPoints.Item2.Add(API.shared.createCylinderColShape(item.DeliveryPointPosition, 3, 4));
                CurrentDeliveryPoints.Item2.LastOrDefault().dimension = item.DeliveryPointDimension;
            }
        }
        public static TirDeliveryPointsList GetAll()
        {
            TirDeliveryPointsList returnModel = new Models.TirDeliveryPointsList();
            if (File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(TirDeliveryPointsList), new XmlRootAttribute("TirDeliveryPoints_List"));
                    returnModel = (TirDeliveryPointsList)deserializer.Deserialize(reader);
                }
            }
            else
            {
                SaveChanges();
            }
            return returnModel;
        }

        public static void CreateDeliveryPoint(TirDeliveryPoint _model)
        {
            _model.ID = CurrentDeliveryPoints.Item1.Count > 0 ? CurrentDeliveryPoints.Item1.LastOrDefault().ID + 1 : 1;
            CurrentDeliveryPoints.Item1.Add(_model);
            CurrentDeliveryPoints.Item2.Add(API.shared.createCylinderColShape(_model.DeliveryPointPosition, 3, 4));
            CurrentDeliveryPoints.Item2.LastOrDefault().dimension = _model.DeliveryPointDimension;
            SaveChanges();
        }

        public static TirDeliveryPoint GetDeliveryPoint(int _Id)
        {
            var _Index = FindTirDeliveryPointIndexById(_Id);
            if (_Index < 0) { return null; }
            return CurrentDeliveryPoints.Item1[_Index];
        }
        public static bool UpdateDeliveryPoint(TirDeliveryPoint _model)
        {
            var _Index = FindTirDeliveryPointIndexById(_model.ID);
            if (_Index < 0) { return false; }
            try
            {
                CurrentDeliveryPoints.Item1.RemoveAt(_Index);
                CurrentDeliveryPoints.Item2.RemoveAt(_Index);
                CurrentDeliveryPoints.Item1.Insert(_Index, _model);
                CurrentDeliveryPoints.Item2.Insert(_Index, API.shared.createCylinderColShape(_model.DeliveryPointPosition, 3, 4));
                CurrentDeliveryPoints.Item2[_Index].dimension = _model.DeliveryPointDimension;
                SaveChanges();
                return true;
            }
            catch (Exception)
            {
                API.shared.consoleOutput(LogCat.Error, "UPDATEDELIVERYPOINT");
                return false;
            }
        }
        public static bool RemoveTirDeliveryPoint(int _Id)
        {
            var _Index = FindTirDeliveryPointIndexById(_Id);
            if (_Index < 0) { return false; }
            try
            {
                API.shared.deleteColShape(CurrentDeliveryPoints.Item2[_Index]);
                CurrentDeliveryPoints.Item2.RemoveAt(_Index);
                CurrentDeliveryPoints.Item1.RemoveAt(_Index);
                SaveChanges();
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static int FindTirDeliveryPointIndexById(int _Id)
        {
            return CurrentDeliveryPoints.Item1.IndexOf(CurrentDeliveryPoints.Item1.FirstOrDefault(x => x.ID == _Id));
        }
        public static void SaveChanges()
        {
            if (System.IO.Directory.Exists(dataPath.Split('/')[0]))
            {
                XmlTextWriter xWriter = new XmlTextWriter(dataPath, System.Text.UTF8Encoding.UTF8);
                xWriter.Formatting = Formatting.Indented;
                xSer.Serialize(xWriter, new TirDeliveryPointsList { Items = CurrentDeliveryPoints.Item1.ToList() });
                xWriter.Dispose();
            }
            else
            {
                System.IO.Directory.CreateDirectory(dataPath.Split('/')[0]);
            }
        }
    }
}
