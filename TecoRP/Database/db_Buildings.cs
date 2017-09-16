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
using GrandTheftMultiplayer.Shared.Math;
using TecoRP.Models;

namespace TecoRP.Database
{
    public class db_Buildings
    {
        static XmlSerializer xSer = new XmlSerializer(typeof(BuildingList));
        public const string dataPath = "Data/Buildings.teco";
        public static Dictionary<int, Building> currentBuildings = new Dictionary<int, Building>();

        public static void SpawnAll()
        {
            API.shared.consoleOutput("Binalar yüklenmeye başladı.");
            foreach (var item in GetAll().Buildings)
            {
                item.LabelOnMap = API.shared.createTextLabel(item.BuildingName, item.Position, 10, 1, true, item.Dimension);
                item.MarkerOnMap = API.shared.createMarker(0, item.Position, new Vector3(), new Vector3(), new Vector3(1, 1, 1), 255, 45, 15, 255, item.Dimension);
                currentBuildings.Add(item.BuildingId, item);
            }            
            API.shared.consoleOutput(currentBuildings.Count +" adet bina yüklendi.");
        }

        public static BuildingList GetAll()
        {
            BuildingList returnModel = new Models.BuildingList();
            if (File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(BuildingList), new XmlRootAttribute("Building_List"));
                    returnModel = (BuildingList)deserializer.Deserialize(reader);
                }
            }
            else
            {
                SaveChanges();
            }
            return returnModel;
        }

        public static void CreateBuilding(Building _model)
        {
            if (_model!=null)
            {
                _model.BuildingId = currentBuildings.Count > 0 ? currentBuildings.LastOrDefault().Value.BuildingId + 1 : 1;
                _model.LabelOnMap = API.shared.createTextLabel(_model.BuildingName, _model.Position, 10, 1, true, _model.Dimension);
                _model.MarkerOnMap = API.shared.createMarker(0, _model.Position, new Vector3(), new Vector3(), new Vector3(1, 1, 1), 255, 45, 15, 255, _model.Dimension);
                currentBuildings.Add(_model.BuildingId, _model);
            }
            SaveChanges();
        }
        public static Building GetBuilding(int Id)
        {
            return currentBuildings[Id];
        }
        public static bool UpdateBuilding(Building _model)
        {
            if (_model!=null)
            {
                try
                {
                    _model.LabelOnMap.position = _model.Position;
                    _model.LabelOnMap.dimension = _model.Dimension;
                    _model.LabelOnMap.text = _model.BuildingName;
                    _model.MarkerOnMap.position = _model.Position;
                    _model.MarkerOnMap.dimension = _model.Dimension;
                    SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    API.shared.consoleOutput(LogCat.Warn, ex.ToString());
                    return false;
                }
            }
            return false;
        }

        public static bool RemoveBuilding(int Id)
        {
            try
            {
                foreach (var item in currentBuildings[Id].Floors )
                {
                    switch (item.Type)
                    {
                        case FloorType.House:
                            db_Houses.RemoveHouse(item.TypedObjectId);
                            break;
                        case FloorType.Business:
                            db_Businesses.Remove(item.TypedObjectId);
                            break;
                        case FloorType.Warehouse:
                            break;
                        default:
                            break;
                    }
                }
                API.shared.deleteEntity(currentBuildings[Id].LabelOnMap);
                API.shared.deleteEntity(currentBuildings[Id].MarkerOnMap);
                return currentBuildings.Remove(Id);
            }
            catch (Exception ex)
            {
                API.shared.consoleOutput(LogCat.Warn, ex.ToString());
                return false;
            }
        }

        public static bool AddFloorToBuilding(int buildingId, FloorType _type, int Id)
        {
            switch (_type)
            {
                case FloorType.House:
                    #region House
                    var house = db_Houses.GetHouse(Id);
                    if (house != null)
                    {
                        if(currentBuildings[buildingId] == null) { return false; }
                        currentBuildings[buildingId].Floors.Add(new Models.Floor
                        {
                            FloorNumber = currentBuildings[buildingId].Floors.Count > 0 ? currentBuildings[buildingId].Floors.LastOrDefault().FloorNumber + 1 : 1,
                            Type = _type,
                            TypedObjectId = house.HouseId
                        });
                        house.EntrancePosition = currentBuildings[buildingId].Position;
                        house.EntranceDimension = currentBuildings[buildingId].Dimension;
                        house.IsInBuilding = true;
                        SaveChanges();
                        return db_Houses.UpdateHouse(house);
                    }
                    else
                    {
                        return false;
                    }

                #endregion
                case FloorType.Business:
                    var business = db_Businesses.GetById(Id);
                    if (business != null)
                    {
                        if(currentBuildings[buildingId] == null) { return false; }
                        currentBuildings[buildingId].Floors.Add(new Floor {
                            FloorNumber = currentBuildings[buildingId].Floors.Count > 0 ? currentBuildings[buildingId].Floors.LastOrDefault().FloorNumber + 1 : 1,
                            Type = _type,
                            TypedObjectId = business.BusinessId
                        });
                        business.BuildingId = buildingId;
                        db_Businesses.SaveChanges();
                        db_Buildings.SaveChanges();
                    }
                    else
                        return false;
                    break;
                case FloorType.Warehouse:
                    break;
                default:
                    break;
            }

            return false;
        }

        public static Building GetNearestBuilding(Vector3 position)
        {
            Building nearest = currentBuildings.FirstOrDefault().Value;

            foreach (var itemBuilding in currentBuildings)
            {
                if (Vector3.Distance(position,itemBuilding.Value.Position)< Vector3.Distance(position,nearest.Position))
                {
                    nearest = itemBuilding.Value;
                }
            }
            return nearest;
        }
        public static void RemoveFloor(int buildingId,int floor)
        {

        }
        public static void SaveChanges()
        {
            lock (currentBuildings)
            {
                if (Directory.Exists(dataPath.Split('/')[0]))
                {
                    XmlTextWriter xWriter = new XmlTextWriter(dataPath, System.Text.UTF8Encoding.UTF8);
                    xWriter.Formatting = Formatting.Indented;
                    xSer.Serialize(xWriter, new BuildingList { Buildings = currentBuildings.Values.ToList() });
                    xWriter.Dispose();
                }
                else
                {
                    Directory.CreateDirectory(dataPath.Split('/')[0]);
                } 
            }
        }
    }
}
