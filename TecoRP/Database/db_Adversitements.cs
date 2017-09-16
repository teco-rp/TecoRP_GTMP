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
    public class db_Adversitements
    {
        static XmlSerializer xSer = new XmlSerializer(typeof(AdvertisementsList));
        public static string dataPath = "Data/Advertisements.teco";



        public static List<Advertisement> GetAll()
        {
            AdvertisementsList returnModel = new AdvertisementsList();
            if (System.IO.File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(AdvertisementsList), new XmlRootAttribute("Advertisements_List"));
                    returnModel = (AdvertisementsList)deserializer.Deserialize(reader);
                }
            }
            else
            {
                SaveChanges(new List<Models.Advertisement>());
            }
            return returnModel.Advertisements;
        }
        public static int Add(Advertisement _model)
        {
            var _list = GetAll();
            _model.AddvertisementID = _list.Count > 0 ? _list.LastOrDefault().AddvertisementID + 1 : 1;
            _list.Add(_model);
            SaveChanges(_list);
            return _model.AddvertisementID;
        }
        public static bool Remove(int Id)
        {
            var _list = GetAll();
            bool result= _list.Remove(_list.FirstOrDefault(x=>x.AddvertisementID == Id));
            SaveChanges(_list);
            return result;
        }
        public static Advertisement GetById(int Id)
        {
            var _list = GetAll();
            return _list.FirstOrDefault(x => x.AddvertisementID == Id);
        }
        public static void SaveChanges(List<Advertisement> _model)
        {
            if (Directory.Exists(dataPath.Split('/')[0]))
            {
                XmlTextWriter xWriter = new XmlTextWriter(dataPath, System.Text.UTF8Encoding.UTF8);
                xWriter.Formatting = Formatting.Indented;
                xSer.Serialize(xWriter, new AdvertisementsList { Advertisements = _model });
                xWriter.Dispose();
            }
            else
            {
                Directory.CreateDirectory(dataPath.Split('/')[0]);
            }

        }
    }
}
