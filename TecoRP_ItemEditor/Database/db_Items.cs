using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using GrandTheftMultiplayer.Server.API;
using TecoRP.Models;

namespace TecoRP_ItemEditor.Database
{
    public static class db_Items
    {
        static XmlSerializer xSer = new XmlSerializer(typeof(ItemList));
        public static string dataPath = "Data/Items.xml";
        public static string dataPathSkins = "Data/Items(Skins).xml";
        public static ItemList currentItems = new ItemList();

        static db_Items()
        {
            GetAll();
        }
        public static ItemList GetAll()
        {
            if (File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(ItemList), new XmlRootAttribute("AllItems"));
                    currentItems = (ItemList)deserializer.Deserialize(reader);
                }
            }
            else
            {
                SaveChanges();
            }
            return currentItems;
        }

        public static void CreateItem(Item _item)
        {
            _item.ID = currentItems.Items.Count > 0 ? currentItems.Items.LastOrDefault().ID + 1 : 1;
            currentItems.Items.Add(_item);
            SaveChanges();
        }
        public static void EditItem(Item _item)
        {
            var _Index = FindIndexById(_item.ID);
            if (_Index >= 0)
            {
                currentItems.Items.RemoveAt(_Index);
                currentItems.Items.Insert(_Index, _item);
            }
            SaveChanges();
        }
        public static void RemoveByIndex(int _Index)
        {
            currentItems.Items.RemoveAt(_Index);
            SaveChanges();
        }

        public static Item GetItemById(int _Id)
        {
            try
            {
                return currentItems.Items.Find(x => x.ID == _Id);
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(ArgumentNullException))
                {
                    return null;
                }
                return new Item();
            }
        }

        public static int FindIndexById(int _Id)
        {
            return currentItems.Items.IndexOf(currentItems.Items.Find(x => x.ID == _Id));
        }
        public static int FindIdByIndex(int _Index)
        {
            return currentItems.Items[_Index].ID;
        }

        public static void GenerateAllSkins(int startId)
        {
            List<Item> skinList = new List<Item>();
            foreach (var item in Enum.GetNames(typeof(GrandTheftMultiplayer.Server.Constant.PedHash)))
            {
                skinList.Add(new Item
                {
                    Name = item.ToString(),
                    Droppable = true,
                    ID = startId,
                    MaxCount = 1,
                    Type = ItemType.Skin,
                    Value_0 = item.ToString(),
                    Value_1 = "0",
                    Value_2 = "0",
                    ObjectId = 0
                });

                startId++;
            }


            if (Directory.Exists((dataPathSkins.Split('/').Count() > 1 ? dataPathSkins.Split('/')[0] : dataPathSkins)))
            {
                XmlTextWriter xWriter = new XmlTextWriter(dataPathSkins, UTF8Encoding.UTF8);
                xWriter.Formatting = Formatting.Indented;
                xSer.Serialize(xWriter, new ItemList { Items = skinList });
                xWriter.Dispose();
            }
            else
            if (dataPathSkins.Split('/').Count() > 1)
                Directory.CreateDirectory(dataPathSkins.Split('/')[0]);

        }

        public static void SaveChanges()
        {
            if (Directory.Exists((dataPath.Split('/').Count() > 1 ? dataPath.Split('/')[0] : dataPath)))
            {
                XmlTextWriter xWriter = new XmlTextWriter(dataPath, UTF8Encoding.UTF8);
                xWriter.Formatting = Formatting.Indented;
                xSer.Serialize(xWriter, currentItems);
                xWriter.Dispose();
            }
            else
                if (dataPath.Split('/').Count() > 1)
                Directory.CreateDirectory(dataPath.Split('/')[0]);


        }

    }
}
