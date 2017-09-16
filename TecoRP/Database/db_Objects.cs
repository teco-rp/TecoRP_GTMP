using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Serialization;
using TecoRP.Managers;
using TecoRP.Models;

namespace TecoRP.Database
{

    public static class db_Objects
    {
        static XmlSerializer xSer = new XmlSerializer(typeof(GameObjectList));
        public static string dataPath = "Data/GameObjects.xml";
        public static GameObjectList currentObjectList = new GameObjectList();

        static db_Objects()
        {
            GetAllGameObjects();
        }

        private static GameObjectList GetAllGameObjects()
        {
            if (System.IO.File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(GameObjectList), new XmlRootAttribute("Objects_List"));
                    currentObjectList = (GameObjectList)deserializer.Deserialize(reader);
                }
            }
            else
            {
                SaveChanges();
            }
            return currentObjectList;
        }

        public static long CreateObject(GameObject _object)
        {
            _object.ID = currentObjectList.Items.LastOrDefault() != null ? currentObjectList.Items.LastOrDefault().ID + 1 : 1;
            currentObjectList.Items.Add(_object);
            return _object.ID;

        }
        public static void SaveChanges()
        {
            if (System.IO.Directory.Exists(dataPath.Split('/')[0]))
            {
                XmlTextWriter xWriter = new XmlTextWriter(dataPath, System.Text.UTF8Encoding.UTF8);
                xWriter.Formatting = Formatting.Indented;
                xSer.Serialize(xWriter, currentObjectList);
                xWriter.Dispose();
            }
            else
            {
                System.IO.Directory.CreateDirectory(dataPath.Split('/')[0]);
            }
        }

        public static GrandTheftMultiplayer.Server.Elements.Object GetObjectById(long _ObjectId)
        {   
           return GameObjectsManager.ObjectsOnMap[currentObjectList.Items.IndexOf(currentObjectList.Items.Find(x => x.ID == _ObjectId))];           
        }
        public static GameObject GetObjectModelById(long _OjectId)
        {
            return currentObjectList.Items.Find(x=>x.ID == _OjectId);
        }
        public static long GetObjectIdByIndex(int _index)
        {
            return currentObjectList.Items[_index].ID;
        }
        public static int GetObjectIndexById(long _ObjectId)
        {
           return currentObjectList.Items.IndexOf(currentObjectList.Items.Find(x => x.ID == _ObjectId));
        }
    }
}
