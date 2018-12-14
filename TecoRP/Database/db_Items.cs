using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using TecoRP.Managers;
using TecoRP.Models;

namespace TecoRP.Database
{

    public static class db_Items
    {
        static XmlSerializer xSer = new XmlSerializer(typeof(ItemList));
        static XmlSerializer xSerD = new XmlSerializer(typeof(DroppedItemList));
        public static string dataPath = "Data/Items.xml";
        public const string dataPathDropped = "Data/DroppedItems.xml";
        public static Dictionary<int, Item> GameItems = new Dictionary<int, Item>();
        public static DroppedItemList currentDroppedItems = new DroppedItemList();

        public static void InitGameItems()
        {
            API.shared.consoleOutput("GameItems yüklenmeye başladı.");

            GameItems.Clear();
            foreach (var item in GetAll().Items)
            {
                try
                {
                    GameItems.Add(item.ID, item);
                }
                catch (ArgumentException ex)
                {
                    API.shared.consoleOutput(LogCat.Error,"ID already exist: "+item.ID);
                }
            }
            API.shared.consoleOutput(GameItems.Count + " item başarıyla yüklendi.");

        }

        public static ItemList GetAll()
        {
            var returnModel = new ItemList();
            if (File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(ItemList), new XmlRootAttribute("AllItems"));
                    returnModel = (ItemList)deserializer.Deserialize(reader);
                }
            }
            else
            {
                SaveChanges();
            }



            return returnModel;
        }

        private static void CreateItem(Item _item)
        {
            _item.ID = GameItems.Count > 0 ? GameItems.LastOrDefault().Key + 1 : 1;
            GameItems.Add(_item.ID, _item);
            SaveChanges();
        }

        private static void EditItem(Item _item)
        {

            if (GameItems.Remove(_item.ID))
                GameItems.Add(_item.ID, _item);
            SaveChanges();
        }

        public static Item GetItemById(int _Id)
        {
            try
            {
                return GameItems[_Id];
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

        public static List<Item> GetItemsByIDs(IEnumerable<int> idList)
        {
            return GameItems.Values.Where(x => idList.Contains(x.ID)).ToList();
        }
        public static void InitDropped()
        {
            API.shared.consoleOutput("Yerdeki eşyalar yüklenmeye başladı.");

            foreach (var itemDropped in GetAllDropped().Items)
            {

                try
                {
                    //var gameItem = GameItems.FirstOrDefault(x => x.ID == itemDropped.Item.ItemId);
                    var gameItem = GameItems[itemDropped.Item.ItemId];
                    Vector3 rotationVector = new Vector3(gameItem.Type == ItemType.Weapon ? 90 : 0, 0, 0);
                    itemDropped.LabelInGame = API.shared.createTextLabel(gameItem.Name, itemDropped.SavedPosition, 10, 0.5f, true, itemDropped.SavedDim);
                    itemDropped.ObjectInGame = API.shared.createObject(gameItem.ObjectId, itemDropped.SavedPosition, rotationVector, itemDropped.SavedDim);
                    // API.shared.attachEntityToEntity(currentDroppedItems.Items.LastOrDefault().LabelInGame, currentDroppedItems.Items.LastOrDefault().ObjectInGame, null, new Vector3(), new Vector3());

                }
                catch (Exception ex)
                {
                    if (ex.GetType() == typeof(FormatException))
                    {
                        API.shared.consoleOutput("GAMEOBJECT Oluşturulamadı. Geçersiz Object");
                    }
                    API.shared.consoleOutput(LogCat.Warn, ex.ToString());
                }
            }
            API.shared.consoleOutput(currentDroppedItems.Items.Count + " adet yerdeki eşya yüklendi.");

        }
        public static DroppedItemList GetAllDropped()
        {
            if (File.Exists(dataPathDropped))
            {
                using (var reader = new StreamReader(dataPathDropped))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(DroppedItemList), new XmlRootAttribute("DroppedItem_List"));
                    currentDroppedItems = (DroppedItemList)deserializer.Deserialize(reader);
                }
            }
            else
            {
                SaveChanges();
            }
            return currentDroppedItems;
        }
        public static bool DropItem(ClientItem _ClientItem, Client droppedPlayer, bool saveFingerPrint = true)
        {
            //var gameItem = db_Items.GameItems.Items.FirstOrDefault(x => x.ID == _ClientItem.ItemId);
            var gameItem = db_Items.GameItems[_ClientItem.ItemId];
            if (gameItem != null)
            {
                if (!_ClientItem.Equipped)
                {
                    if (gameItem.Droppable)
                    {
                        try
                        {
                            Vector3 rotationVector = new Vector3(gameItem.Type == ItemType.Weapon ? 90 : 0, 0, 0);
                            currentDroppedItems.Items.Add(new DroppedItem
                            {
                                DroppedItemId = currentDroppedItems.Items.Count > 0 ? currentDroppedItems.Items.LastOrDefault().DroppedItemId + 1 : 1,
                                Item = _ClientItem,
                                FactionId = (GetItemById(_ClientItem.ItemId).Type == ItemType.Weapon || GetItemById(_ClientItem.ItemId).Type == ItemType.FirstAid) ? (FactionManager.GetPlayerFaction(droppedPlayer) <= 5 ? FactionManager.GetPlayerFaction(droppedPlayer) : 0) : 0,
                                ObjectInGame = API.shared.createObject(gameItem.ObjectId, droppedPlayer.position + new Vector3(0, 0, -0.9), droppedPlayer.rotation + rotationVector, droppedPlayer.dimension),
                                LabelInGame = API.shared.createTextLabel(gameItem.Name, droppedPlayer.position + new Vector3(0, 0, -0.75), 10, 0.5f, true, droppedPlayer.dimension),
                                SavedPosition = droppedPlayer.position + new Vector3(0, 0, -0.75),
                                SavedDim = droppedPlayer.dimension,
                                DroppedPlayerSocialClubName = saveFingerPrint ? droppedPlayer.socialClubName : "",
                            });
                            //API.shared.attachEntityToEntity(currentDroppedItems.Items.LastOrDefault().LabelInGame, currentDroppedItems.Items.LastOrDefault().ObjectInGame, null, new Vector3(), new Vector3());
                            SaveChanges(true);

                            return true;

                        }
                        catch (Exception ex)
                        {
                            if (ex.GetType() == typeof(FormatException))
                            {
                                API.shared.consoleOutput("Düşürülecek Item'ın object ID'si yok. (" + gameItem.Name + " | " + gameItem.ID + " | obj: " + gameItem.ObjectId + ")");
                            }
                            return false;
                        }
                    }
                }
            }
            return false;
        }
        public static bool RemoveDroppedItem(DroppedItem _droppedItem)
        {
            try
            {
                API.shared.deleteEntity(_droppedItem.LabelInGame);
                if (_droppedItem.ObjectInGame != null)
                    API.shared.deleteEntity(_droppedItem.ObjectInGame);
                currentDroppedItems.Items.Remove(_droppedItem);
                return true;
            }
            catch (Exception ex)
            {
                API.shared.consoleOutput(LogCat.Warn, ex.ToString());
                return false;
            }
        }

        public static void SaveChanges()
        {
            lock (GameItems)
            {
                if (Directory.Exists(dataPath.Split('/')[0]))
                {
                    XmlTextWriter xWriter = new XmlTextWriter(dataPath, UTF8Encoding.UTF8);
                    xWriter.Formatting = Formatting.Indented;
                    xSer.Serialize(xWriter, GameItems);
                    xWriter.Dispose();
                }
                else
                    Directory.CreateDirectory(dataPath.Split('/')[0]);
            }
        }

        public static void SaveChanges(bool dropped)
        {
            lock (currentDroppedItems)
            {
                if (!dropped) { SaveChanges(); return; }

                if (Directory.Exists(dataPathDropped.Split('/')[0]))
                {
                    XmlTextWriter xWriter = new XmlTextWriter(dataPathDropped, UTF8Encoding.UTF8);
                    xWriter.Formatting = Formatting.Indented;
                    xSerD.Serialize(xWriter, currentDroppedItems);
                    xWriter.Dispose();
                }
                else
                    Directory.CreateDirectory(dataPath.Split('/')[0]);
            }
        }

    }

}
