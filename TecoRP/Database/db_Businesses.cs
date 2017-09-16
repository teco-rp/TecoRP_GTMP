using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using TecoRP.Models;
using GrandTheftMultiplayer.Server.Constant;

namespace TecoRP.Database
{
    public class db_Businesses
    {
        static XmlSerializer xSer = new XmlSerializer(typeof(BusinessList));
        public const string dataPath = "Data/Businesses.xml";
        public static Dictionary<int, Business> currentBusiness = new Dictionary<int, Business>();

        public db_Businesses()
        {

        }
        public static void SpawnAll()
        {
            foreach (var item in GetAll().Items)
            {
                item.LabelOnMap = API.shared.createTextLabel(item.BusinessName, item.Position, 10, 1, true, item.Dimension);
                currentBusiness.Add(item.BusinessId, item);
            }
        }
        public static BusinessList GetAll()
        {
            BusinessList returnModel = new BusinessList();
            if (File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(BusinessList), new XmlRootAttribute("Business_List"));
                    returnModel = (BusinessList)deserializer.Deserialize(reader);
                }
            }
            else
            {
                SaveChanges();
            }
            return returnModel;
        }

        public static void Create(Business _model)
        {
            _model.BusinessId = currentBusiness.Count > 0 ? currentBusiness.LastOrDefault().Value.BusinessId + 1 : 1;
            _model.LabelOnMap = API.shared.createTextLabel(_model.BusinessName, _model.Position, 10, 1, true, _model.Dimension);
            currentBusiness.Add(_model.BusinessId, _model);
            SaveChanges();
        }
        public static Business GetById(int _Id)
        {
            return currentBusiness[_Id];
        }
        public static bool Update(Business _model)
        {

            if (currentBusiness[_model.BusinessId] != null)
            {
                try
                {
                    currentBusiness[_model.BusinessId].LabelOnMap.position = _model.Position;
                    currentBusiness[_model.BusinessId].LabelOnMap.text = _model.BusinessName;
                    currentBusiness[_model.BusinessId].LabelOnMap.dimension = _model.Dimension;
                    SaveChanges();
                }
                catch (Exception ex)
                {
                    API.shared.consoleOutput("Business | Update Error: " + ex.Message);
                    return false;
                }

                return true;
            }
            else
            {
                API.shared.consoleOutput("Business Id bulunamadı.");
                return false;
            }
        }
        public static bool Remove(int _Id)
        {
            if (currentBusiness[_Id] != null)
            {
                try
                {
                    API.shared.deleteEntity(currentBusiness[_Id].LabelOnMap);
                    bool result = currentBusiness.Remove(_Id);
                    SaveChanges();
                    return result;
                }
                catch (Exception ex)
                {
                    API.shared.consoleOutput(LogCat.Warn, ex.ToString());
                    return false;
                }
            }
            else
            {
                return false;
            }
        }



        public static void SaveChanges()
        {
            lock (currentBusiness)
            {
                if (System.IO.Directory.Exists(dataPath.Split('/')[0]))
                {
                    XmlTextWriter xWriter = new XmlTextWriter(dataPath, System.Text.UTF8Encoding.UTF8);
                    xWriter.Formatting = Formatting.Indented;
                    xSer.Serialize(xWriter, new BusinessList { Items = currentBusiness.Values.ToList() });
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
