using GrandTheftMultiplayer.Server.API;
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
using GrandTheftMultiplayer.Server.Constant;

namespace TecoRP.Database
{
    public static class db_Shops
    {
        static XmlSerializer xSer = new XmlSerializer(typeof(ShopList));
        public static string dataPath = "Data/Shops.xml";
        public static Dictionary<Shop, Marker> CurrentShopsList = new Dictionary<Shop, Marker>();

        static db_Shops()
        {
            SpawnAll();
        }
        public static void SpawnAll()
        {
            foreach (var item in GetAll().Items)
            {
                try
                {
                    CurrentShopsList.Add(item,
                        API.shared.createMarker(item.MarkerType, item.Position, new Vector3(0, 0, 0),
                        item.Rotation, item.Scale, 255, item.MarkerColorRGB.Red, item.MarkerColorRGB.Green,
                        item.MarkerColorRGB.Blue, item.Dimension
                        ));
                }
                catch (Exception ex)
                {
                    API.shared.consoleOutput(LogCat.Warn, ex.ToString());
                    continue;
                 }
            }
        }
        public static ShopList GetAll()
        {
            ShopList currentShops = new Models.ShopList();
            if (File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(ShopList), new XmlRootAttribute("Shop_List"));
                    currentShops = (ShopList)deserializer.Deserialize(reader);
                }
            }
            else
            {
                SaveChanges();
            }
            return currentShops;
        }

        public static Shop GetShop(int _Id)
        {
            return CurrentShopsList.FirstOrDefault(x => x.Key.ShopId == _Id).Key;
        }
        public static void CreateShop(Shop _shop)
        {
            _shop.ShopId = CurrentShopsList.Count > 0 ? CurrentShopsList.Keys.LastOrDefault().ShopId + 1 : 1;
            CurrentShopsList.Add(_shop, API.shared.createMarker(_shop.MarkerType, _shop.Position, new Vector3(0, 0, 0), _shop.Rotation, _shop.Scale, 255, _shop.MarkerColorRGB.Red, _shop.MarkerColorRGB.Green, _shop.MarkerColorRGB.Blue, _shop.Dimension));
            SaveChanges();
        }
        public static int FindShopIndexById(int _Id)
        {
            return CurrentShopsList.Keys.ToList().IndexOf(CurrentShopsList.Keys.FirstOrDefault(x => x.ShopId == _Id));
        }
        public static int FindShopIdByIndex(int _Index)
        {
            return CurrentShopsList.Keys.ToList()[_Index].ShopId;
        }
        public static void SaveChanges()
        {


            if (System.IO.Directory.Exists(dataPath.Split('/')[0]))
            {
                XmlTextWriter xWriter = new XmlTextWriter(dataPath, System.Text.UTF8Encoding.UTF8);
                xWriter.Formatting = Formatting.Indented;
                xSer.Serialize(xWriter, new ShopList { Items = CurrentShopsList.Keys.ToList() });
                xWriter.Dispose();
            }
            else
            {
                System.IO.Directory.CreateDirectory(dataPath.Split('/')[0]);
            }
        }

        public static void SaveChanges(ShopList _model)
        {
            lock (CurrentShopsList)
            {
                if (System.IO.Directory.Exists(dataPath.Split('/')[0]))
                {
                    XmlTextWriter xWriter = new XmlTextWriter(dataPath, System.Text.UTF8Encoding.UTF8);
                    xWriter.Formatting = Formatting.Indented;
                    xSer.Serialize(xWriter, _model);
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
