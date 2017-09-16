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
    public class db_KamyonJob
    {
        static XmlSerializer xSer = new XmlSerializer(typeof(KamyonDeliveryList));
        public static string dataPath = "Data/Job_Kamyon.xml";
        public static Tuple<List<KamyonDeliveryPoint>, List<CylinderColShape>> CurrentDeliveryPoints = new Tuple<List<KamyonDeliveryPoint>, List<CylinderColShape>>(new List<KamyonDeliveryPoint>(), new List<CylinderColShape>());
        public db_KamyonJob()
        {

        }
        public static void Init()
        {
            foreach (var item in GetAll().Items)
            {
                CurrentDeliveryPoints.Item1.Add(item);
                CurrentDeliveryPoints.Item2.Add(API.shared.createCylinderColShape(item.DeliveryPoint, 3, 4));
                CurrentDeliveryPoints.Item2.LastOrDefault().dimension = item.DeliveryDimension;
            }
        }
        public static KamyonDeliveryList GetAll()
        {
            KamyonDeliveryList returnModel = new Models.KamyonDeliveryList();
            if (File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(KamyonDeliveryList), new XmlRootAttribute("KamyonDelivery_List"));
                    returnModel = (KamyonDeliveryList)deserializer.Deserialize(reader);
                }
            }
            else
            {
                SaveChanges();
            }
            return returnModel;
        }

        public static void CreateDeliveryPoint(KamyonDeliveryPoint _model)
        {
            _model.ID = CurrentDeliveryPoints.Item1.Count > 0 ? CurrentDeliveryPoints.Item1.LastOrDefault().ID + 1 : 1;
            CurrentDeliveryPoints.Item1.Add(_model);
            CurrentDeliveryPoints.Item2.Add(API.shared.createCylinderColShape(_model.DeliveryPoint, 3, 4));
            CurrentDeliveryPoints.Item2.LastOrDefault().dimension = _model.DeliveryDimension;
            SaveChanges();
        }
        public static KamyonDeliveryPoint GetDeliveryPoint(int _Id)
        {
            var _Index = FindKamyonDeliveryPointIndexById(_Id);
            if (_Index < 0) { return null; }
            return CurrentDeliveryPoints.Item1[_Index];
        }
        public static bool UpdateDeliveryPoint(KamyonDeliveryPoint _model)
        {
            var _Index = FindKamyonDeliveryPointIndexById(_model.ID);
            if (_Index < 0) { return false; }
            try
            {
                CurrentDeliveryPoints.Item1.RemoveAt(_Index);
                CurrentDeliveryPoints.Item2.RemoveAt(_Index);
                CurrentDeliveryPoints.Item1.Insert(_Index, _model);
                CurrentDeliveryPoints.Item2.Insert(_Index, API.shared.createCylinderColShape(_model.DeliveryPoint, 3, 4));
                CurrentDeliveryPoints.Item2[_Index].dimension = _model.DeliveryDimension;
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
            var _Index = FindKamyonDeliveryPointIndexById(_Id);
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

        public static int FindKamyonDeliveryPointIndexById(int _Id)
        {
            return CurrentDeliveryPoints.Item1.IndexOf(CurrentDeliveryPoints.Item1.FirstOrDefault(x => x.ID == _Id));
        }
        public static void SaveChanges()
        {
            if (System.IO.Directory.Exists(dataPath.Split('/')[0]))
            {
                XmlTextWriter xWriter = new XmlTextWriter(dataPath, System.Text.UTF8Encoding.UTF8);
                xWriter.Formatting = Formatting.Indented;
                xSer.Serialize(xWriter, new KamyonDeliveryList { Items = CurrentDeliveryPoints.Item1.ToList() });
                xWriter.Dispose();
            }
            else
            {
                System.IO.Directory.CreateDirectory(dataPath.Split('/')[0]);
            }
        }
    }
}
