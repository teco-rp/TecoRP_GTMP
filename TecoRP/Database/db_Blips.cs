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
    public class db_Blips
    {
        static XmlSerializer xSer = new XmlSerializer(typeof(BlipList));
        public static string dataPath = "Data/Blips.xml";
        public static BlipList currentBlips = new BlipList();
        public db_Blips()
        {

        }

        public BlipList GetAll()
        {
            if (System.IO.File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(BlipList), new XmlRootAttribute("BlipList"));
                    currentBlips = (BlipList)deserializer.Deserialize(reader);
                }
            }
            else
            {
                SaveChanges();
            }

            return currentBlips;
        }

        public static void AddBlip(Blip _addedBlip)
        {
            _addedBlip.ID = currentBlips.Items.Count > 0 ? currentBlips.Items.LastOrDefault().ID + 1 : 1;
            currentBlips.Items.Add(_addedBlip);
            SaveChanges();
        }
        public static Blip GetBlip(int Id)
        {
            return currentBlips.Items.FirstOrDefault(x => x.ID == Id);
        }

        public static void SaveChanges()
        {
   
            if (System.IO.Directory.Exists(dataPath.Split('/')[0]))
            {
                XmlTextWriter xWriter = new XmlTextWriter(dataPath, System.Text.UTF8Encoding.UTF8);
                xWriter.Formatting = Formatting.Indented;
                xSer.Serialize(xWriter, currentBlips);
                xWriter.Dispose();
            }
            else
            {
                System.IO.Directory.CreateDirectory(dataPath.Split('/')[0]);
            }
        }

        public static int FindBlipIdByIndex(int _index)
        {
            return currentBlips.Items[_index].ID;
        }
        public static int FindBlipIndexById(int _id)
        {
            return currentBlips.Items.IndexOf(currentBlips.Items.Find(x => x.ID == _id));
        }
    }
}
