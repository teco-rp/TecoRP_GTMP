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
using System.Reflection;

namespace TecoRP.Database
{
    public class db_Houses
    {
        static XmlSerializer xSer = new XmlSerializer(typeof(HouseList));
        public static string dataPath = "Data/Houses.xml";
        public static Dictionary<int, House> CurrentHousesDict = new Dictionary<int, House>();
        static db_Houses()
        {
            SpawnAll();
        }
        public static void SpawnAll()
        {
            HouseMarkerColor hmc = new Models.HouseMarkerColor();

            foreach (var item in GetAll().Items)
            {
                if (!item.IsInBuilding)
                {
                    item.LabelOnMap = API.shared.createTextLabel(item.Name, item.EntrancePosition, 10, 1, false, item.EntranceDimension);
                    item.MarkerOnMap = API.shared.createMarker(item.MarkerType, item.EntrancePosition, new Vector3(), item.EntranceRotation, new Vector3(1, 1, 1), 255,
                        item.IsSelling ? hmc.SaleColor.Red : hmc.NormalColor.Red,
                        item.IsSelling ? hmc.SaleColor.Green : hmc.NormalColor.Green,
                        item.IsSelling ? hmc.SaleColor.Blue : hmc.NormalColor.Blue,
                        item.EntranceDimension
                        );
                }
                CurrentHousesDict.Add(item.HouseId, item);
                //    CurrentHousesDict.Add(item,
                //        API.shared.createMarker(item.MarkerType, item.EntrancePosition, new Vector3(0, 0, 0), item.EntranceRotation, new Vector3(1, 1, 1),
                //        255,
                //        item.IsSelling ? hmc.SaleColor.Red : hmc.NormalColor.Red, item.IsSelling ? hmc.SaleColor.Green : hmc.NormalColor.Green, item.IsSelling ? hmc.SaleColor.Blue : hmc.NormalColor.Blue, item.EntranceDimension
                //        ));
            }
        }

        public static HouseList GetAll()
        {
            HouseList currentHouses = new HouseList();
            if (System.IO.File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(HouseList), new XmlRootAttribute("Houses_List"));
                    currentHouses = (HouseList)deserializer.Deserialize(reader);
                }
            }
            else
            {
                SaveChanges();
            }
            return currentHouses;
        }
        public static List<House> GetAllCurrents()
        {
            return CurrentHousesDict.Values.ToList();
            
        }
        public static void CreateHouse(House _model)
        {
            _model.HouseId = CurrentHousesDict.Count >0 ? CurrentHousesDict.LastOrDefault().Key + 1 : 1;
            _model.InteriorDimension = _model.HouseId;
            HouseMarkerColor hmc = new HouseMarkerColor();

            _model.LabelOnMap = API.shared.createTextLabel(_model.Name, _model.EntrancePosition, 10, 1, false, _model.EntranceDimension);
            _model.MarkerOnMap = API.shared.createMarker(_model.MarkerType, _model.EntrancePosition, new Vector3(), _model.EntranceRotation, new Vector3(1, 1, 1), 255,
                _model.IsSelling ? hmc.SaleColor.Red : hmc.NormalColor.Red,
                _model.IsSelling ? hmc.SaleColor.Green : hmc.NormalColor.Green,
                _model.IsSelling ? hmc.SaleColor.Blue : hmc.NormalColor.Blue,
                _model.EntranceDimension
                );
            while (true)
            {
                try
                {
                    CurrentHousesDict.Add(_model.HouseId, _model);
                    break;
                }
                catch (Exception ex)
                {
                    if (ex.GetType() == typeof(TargetInvocationException))
                    {
                        _model.HouseId++;
                    }
                }
            }

            //CurrentHousesDict.Add(_house, API.shared.createMarker(_house.MarkerType, _house.EntrancePosition, new Vector3(0, 0, 0), new Vector3(0, 0, 0), new Vector3(1, 1, 1), 255, hmc.SaleColor.Red, hmc.SaleColor.Green, hmc.SaleColor.Blue, _house.EntranceDimension));
            SaveChanges();
        }
        public static House GetHouse(int id)
        {
            return CurrentHousesDict[id];
        }
  

        public static bool UpdateHouse(House _model)
        {
            try
            {
                if (!_model.IsInBuilding)
                {
                    HouseMarkerColor hmc = new HouseMarkerColor();
                    _model.LabelOnMap.position = _model.EntrancePosition;
                    _model.LabelOnMap.dimension = _model.EntranceDimension;
                    _model.MarkerOnMap.color = new Color(
                        _model.IsSelling ? hmc.SaleColor.Red : hmc.NormalColor.Red,
                        _model.IsSelling ? hmc.SaleColor.Green : hmc.NormalColor.Green,
                        _model.IsSelling ? hmc.SaleColor.Blue : hmc.NormalColor.Blue
                       );
                    _model.MarkerOnMap.position = _model.EntrancePosition;
                    _model.MarkerOnMap.dimension = _model.EntranceDimension;
                }
                else
                {
                    if (_model.LabelOnMap != null)
                    {
                        API.shared.deleteEntity(_model.LabelOnMap);
                    }
                    if (_model.MarkerOnMap != null)
                    {
                        API.shared.deleteEntity(_model.MarkerOnMap);
                    }
                }
                SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                API.shared.consoleOutput(LogCat.Warn, ex.ToString());
                return false;
            }

        }
        public static bool RemoveHouse(int _Id)
        {
            try
            {
                if (CurrentHousesDict[_Id] !=null)
                {
                    if (!CurrentHousesDict[_Id].IsInBuilding)
                    {
                        API.shared.deleteEntity(CurrentHousesDict[_Id].LabelOnMap);
                        API.shared.deleteEntity(CurrentHousesDict[_Id].MarkerOnMap); 
                    }
                    bool result = CurrentHousesDict.Remove(_Id);
                    if (result)
                    {
                        SaveChanges();
                    }
                    return result;
                }
                return false;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void SaveChanges()
        {
            lock (CurrentHousesDict)
            {
                if (System.IO.Directory.Exists(dataPath.Split('/')[0]))
                {
                    XmlTextWriter xWriter = new XmlTextWriter(dataPath, System.Text.UTF8Encoding.UTF8);
                    xWriter.Formatting = Formatting.Indented;
                    xSer.Serialize(xWriter, new HouseList { Items = CurrentHousesDict.Values.ToList() });
                    xWriter.Dispose();
                }
                else
                {
                    System.IO.Directory.CreateDirectory(dataPath.Split('/')[0]);
                } 
            }
        }
        ~db_Houses()
        {
            SaveChanges();
        }

    }
}
