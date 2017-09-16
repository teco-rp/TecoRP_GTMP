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
    public class db_Entrances
    {
        static XmlSerializer xSer = new XmlSerializer(typeof(EntranceList));
        public static string dataPath = "Data/Entrances.xml";
        public static EntranceList currentEntrances = new EntranceList();
        public db_Entrances()
        {

        }
        public EntranceList GetAll()
        {
            if (System.IO.File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(EntranceList), new XmlRootAttribute("Entrance_List"));
                    currentEntrances = (EntranceList)deserializer.Deserialize(reader);
                }
            }
            else
            {
                 SaveChanges();
            }

            return currentEntrances;
        }

        public static void AddEntrance(Entrance _addedEntrance)
        {
            _addedEntrance.ID = currentEntrances.Items.Count > 0 ? currentEntrances.Items.LastOrDefault().ID + 1 : 1;
            _addedEntrance.InteriorDimension = currentEntrances.Items.Count > 0 ? currentEntrances.Items.LastOrDefault().ID + 1 : 1;
            currentEntrances.Items.Add(_addedEntrance);
            SaveChanges();
        }
        public static void SaveChanges()
        {
            lock (currentEntrances)
            {
                if (System.IO.Directory.Exists(dataPath.Split('/')[0]))
                {
                    XmlTextWriter xWriter = new XmlTextWriter(dataPath, System.Text.UTF8Encoding.UTF8);
                    xWriter.Formatting = Formatting.Indented;
                    xSer.Serialize(xWriter, currentEntrances);
                    xWriter.Dispose();
                }
                else
                {
                    System.IO.Directory.CreateDirectory(dataPath.Split('/')[0]);
                } 
            }
        }

        public static int FindBlipIdByIndex(int _index)
        {
            return currentEntrances.Items[_index].ID;
        }
        public static int FindBlipIndexById(int _id)
        {
            return currentEntrances.Items.IndexOf(currentEntrances.Items.Find(x => x.ID == _id));
        }

        public static void RemoveEntranceFully(int _id)
        {

        }
    }
}
