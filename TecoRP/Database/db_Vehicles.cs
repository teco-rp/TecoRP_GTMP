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
using GrandTheftMultiplayer.Shared;
using System.Web;

namespace TecoRP.Database
{

    public class db_Vehicles
    {
        static XmlSerializer xSer = new XmlSerializer(typeof(VehicleList));
        static XmlSerializer xSerTax = new XmlSerializer(typeof(TaxesList));
        public static string dataPath = "Data/Vehicles.xml";
        public static string dataPathTax = "Data/Taxes.xml";
        public static VehicleList currentVehList = new VehicleList();
        private static List<GrandTheftMultiplayer.Server.Elements.Vehicle> VehiclesOnMap = new List<GrandTheftMultiplayer.Server.Elements.Vehicle>();
        public db_Vehicles()
        {

        }

        public static void SpawnAll()
        {
            API.shared.consoleOutput("Araçlar yüklenmeye başladı.");


            foreach (var itemVeh in GetAll().Items)
            {
                itemVeh.VehicleOnMap = API.shared.createVehicle(itemVeh.VehicleModelId, itemVeh.LastPosition, itemVeh.LastRotation, itemVeh.Color1, itemVeh.Color2, itemVeh.Dimension);
                API.shared.setVehicleEngineStatus(itemVeh.VehicleOnMap, false);
                API.shared.setVehicleLocked(itemVeh.VehicleOnMap.handle, itemVeh.IsLocked);
                API.shared.setVehicleNumberPlate(itemVeh.VehicleOnMap.handle, itemVeh.Plate);
                API.shared.setVehicleLivery(itemVeh.VehicleOnMap, itemVeh.Livery);

                for (int i = 0; i < 69; i++)
                {
                    if (itemVeh.Mods[i] == -1) { continue; }
                    itemVeh.VehicleOnMap.setMod(i, itemVeh.Mods[i]);
                }
            }
            API.shared.consoleOutput(currentVehList.Items.Count + " araç başarıyla yüklendi.");

        }
        public static void RespawnAll()
        {
            foreach (var itemVeh in currentVehList.Items)
            {
                try
                {
                    if (API.shared.getVehicleOccupants(itemVeh.VehicleOnMap).Length > 0) continue;
                    API.shared.deleteEntity(itemVeh.VehicleOnMap);
                    itemVeh.VehicleOnMap = API.shared.createVehicle(itemVeh.VehicleModelId, itemVeh.LastPosition, itemVeh.LastRotation, itemVeh.Color1, itemVeh.Color2, itemVeh.Dimension);
                    API.shared.setVehicleEngineStatus(itemVeh.VehicleOnMap, false);
                    API.shared.setVehicleLocked(itemVeh.VehicleOnMap.handle, itemVeh.IsLocked);
                    API.shared.setVehicleNumberPlate(itemVeh.VehicleOnMap.handle, itemVeh.Plate);
                    API.shared.setVehicleLivery(itemVeh.VehicleOnMap, itemVeh.Livery);                    

                    for (int i = 0; i < 69; i++)
                    {
                        if (itemVeh.Mods[i] == -1) { continue; }
                        itemVeh.VehicleOnMap.setMod(i, itemVeh.Mods[i]);
                    }
                }
                catch (Exception ex)
                {
                    API.shared.consoleOutput(LogCat.Fatal, "Respawning of Vehicles Error: " + ex.ToString());
                    continue;
                }
            }
        }

        public static void Respawn(long _Id)
        {
            var _vehicle = GetVehicle(_Id);
            if (_vehicle != null)
            {
                try
                {
                    API.shared.deleteEntity(_vehicle.VehicleOnMap);

                    _vehicle.VehicleOnMap = API.shared.createVehicle(_vehicle.VehicleModelId, _vehicle.LastPosition, _vehicle.LastRotation, _vehicle.Color1, _vehicle.Color2, _vehicle.Dimension);
                    API.shared.setVehicleEngineStatus(_vehicle.VehicleOnMap, false);
                    API.shared.setVehicleLocked(_vehicle.VehicleOnMap.handle, _vehicle.IsLocked);
                    API.shared.setVehicleNumberPlate(_vehicle.VehicleOnMap.handle, _vehicle.Plate);
                    _vehicle.VehicleOnMap.numberPlate = _vehicle.Plate;
                    API.shared.setVehicleLivery(_vehicle.VehicleOnMap, _vehicle.Livery);

                    #region DamageLoading
                    if (!String.IsNullOrEmpty(_vehicle.SpecifiedDamage))
                    {
                        var dmg = API.shared.fromJson(_vehicle.SpecifiedDamage).ToObject<SpecifiedValueForDamage>() as SpecifiedValueForDamage;
                        foreach (var itemDoor in dmg.BrokenDoors)
                        {
                            API.shared.breakVehicleDoor(_vehicle.VehicleOnMap, itemDoor, true);
                        }
                        foreach (var itemWindow in dmg.BrokenWindows)
                        {
                            API.shared.breakVehicleWindow(_vehicle.VehicleOnMap, itemWindow, true);
                        }
                        foreach (var itemTyre in dmg.PoppedTyres)
                        {
                            API.shared.popVehicleTyre(_vehicle.VehicleOnMap, itemTyre, true);
                        }
                    }
                    #endregion

                    for (int i = 0; i < 69; i++)
                    {
                        if (_vehicle.Mods[i] == -1) { continue; }
                        _vehicle.VehicleOnMap.setMod(i, _vehicle.Mods[i]);
                    }
                }
                catch (Exception ex)
                {
                    API.shared.consoleOutput(LogCat.Warn, ex.ToString());
                }
            }
        }
        public static VehicleList GetAll()
        {
            if (System.IO.File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(VehicleList), new XmlRootAttribute("Vehicle_List"));
                    currentVehList = (VehicleList)deserializer.Deserialize(reader);
                }
            }
            else
            {
                SaveChanges();
            }

            return currentVehList;
        }
        /// <summary>
        /// Doesn't effect to game. Just read datas
        /// </summary>
        /// <returns></returns>
        public static VehicleList ReadAll(bool isServerMapPath = false)
        {
            string path = dataPath;
            //if (isServerMapPath)
            //{
            //    path = HttpContext.Current.Server.MapPath("~/"+ dataPath);
            //}

            VehicleList returnModel = new VehicleList();
            if (System.IO.File.Exists(path))
            {
                using (var reader = new StreamReader(path))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(VehicleList), new XmlRootAttribute("Vehicle_List"));
                    returnModel = (VehicleList)deserializer.Deserialize(reader);
                }
            }
            else
                return null;

            return returnModel;
        }

        public static bool RemoveVehicle(long _Id)
        {
            var _Index = FindVehicleIndexById(_Id);
            if (_Index >= 0)
            {
                try
                {
                    API.shared.deleteEntity(currentVehList.Items[_Index].VehicleOnMap);
                    currentVehList.Items.RemoveAt(_Index);
                    return true;
                }
                catch (Exception ex)
                {
                    API.shared.consoleOutput(LogCat.Fatal, "Removing Vehicle Error: " + ex.ToString());
                    return false;
                }
            }
            else
                return false;
        }
        public static bool RemoveVehicle(Models.Vehicle _model)
        {
            try
            {
                var _Index = FindVehicleIndexById(_model.VehicleId);
                if (_Index >= 0)
                {
                    API.shared.deleteEntity(_model.VehicleOnMap);
                    currentVehList.Items.RemoveAt(_Index);
                    return true;
                }
                else
                {
                    return false;
                }

            }
            catch (Exception ex)
            {
                API.shared.consoleOutput(LogCat.Warn, "Error while deleting Vehicle: " + ex.ToString());
                return false;
            }
        }
        public static Models.Vehicle FindNearestVehicle(Vector3 pos)
        {
            Models.Vehicle nearestVehicle = currentVehList.Items.FirstOrDefault();
            foreach (var itemVeh in currentVehList.Items)
            {
                if (Vector3.Distance(pos, itemVeh.VehicleOnMap.position) < Vector3.Distance(pos, nearestVehicle.VehicleOnMap.position))
                {
                    nearestVehicle = itemVeh;
                }
            }
            return nearestVehicle;
        }
        public static Models.Vehicle FindNearestVehicle(Vector3 pos,int JobId)
        {
            Models.Vehicle nearestVehicle = currentVehList.Items.FirstOrDefault();
            foreach (var itemVeh in currentVehList.Items)
            {
                float _dist = Vector3.Distance(pos, itemVeh.VehicleOnMap.position);
                if (JobId == itemVeh.JobId &&  _dist < Vector3.Distance(pos, nearestVehicle.VehicleOnMap.position))
                {
                    nearestVehicle = itemVeh;
                }
            }
            return nearestVehicle;
        }
        public static int FindVehicleIndexById(long _vehId)
        {
            return currentVehList.Items.IndexOf(currentVehList.Items.FirstOrDefault(x => x.VehicleId == _vehId));
        }
        public static long FindVehicleIdByIndex(int _vehIndex)
        {
            return currentVehList.Items[_vehIndex].VehicleId;
        }
        public static Models.Vehicle FindVehicleByIndex(int _vehIndex)
        {
            return currentVehList.Items[_vehIndex];
        }

        public static Models.Vehicle GetVehicle(long _Id)
        {
            return currentVehList.Items.Find(x => x.VehicleId == _Id);
        }

        public static bool UpdateVehicleToModel(Models.Vehicle _model)
        {
            var _Index = FindVehicleIndexById(_model.VehicleId);          

            if (_Index >= 0)
            {
                currentVehList.Items[_Index].LastPosition = _model.VehicleOnMap.position;
                currentVehList.Items[_Index].LastRotation = _model.VehicleOnMap.rotation;
                currentVehList.Items[_Index].Dimension = _model.VehicleOnMap.dimension;
                currentVehList.Items[_Index].IsLocked = _model.VehicleOnMap.locked;

                for (int i = 0; i < 69; i++)
                {
                    currentVehList.Items[_Index].Mods[i] = _model.VehicleOnMap.getMod(i);
                }
                SaveChanges();
                return true;
            }
            else
            {
                return false;
            }
        }

        public static bool IsPlayersVehicleById(Client sender, long _vehId)
        {
            return currentVehList.Items.Find(x => x.VehicleId == _vehId).OwnerSocialClubName == sender.socialClubName;
        }

        public static int GetVehicleJobId(Client sender, long _vehId)
        {
            return currentVehList.Items.Find(x => x.VehicleId == _vehId).JobId;
        }
        public static int GetVehicleFactionId(Client sender, long _vehId)
        {
            return currentVehList.Items.Find(x => x.VehicleId == _vehId).FactionId;
        }
        public static List<Models.Vehicle> GetPlayerVehicles(Client sender)
        {
            return currentVehList.Items.Where(x => x.OwnerSocialClubName == sender.socialClubName).ToList();
        }
        public static List<Models.Vehicle> GetOfflinePlayerVehicles(string socialClubName, bool serverMapPath=true)
        {
            List<Models.Vehicle> returnModel = new List<Models.Vehicle>();
            foreach (var item in ReadAll(true).Items)
            {
                if (item.OwnerSocialClubName == socialClubName)
                {
                    returnModel.Add(item);
                }
            }

            return returnModel;
        }
        public void UpdateVehicleLocations(List<GrandTheftMultiplayer.Server.Elements.Vehicle> _vehiclesOnMap)
        {
            for (int i = 0; i < _vehiclesOnMap.Count; i++)
            {
                try
                {
                    currentVehList.Items[i].LastPosition = _vehiclesOnMap[i].position;
                    currentVehList.Items[i].LastRotation = _vehiclesOnMap[i].rotation;
                }
                catch (Exception)
                {
                    continue;
                }
            }


        }

        public static List<Tax> GetVehicleTaxes()
        {
            List<Tax> returnModel = new List<Models.Tax>();
            if (System.IO.File.Exists(dataPathTax))
            {
                using (var reader = new StreamReader(dataPathTax))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(TaxesList), new XmlRootAttribute("Taxes_List"));
                    returnModel = ((TaxesList)deserializer.Deserialize(reader)).Items;
                }
            }
            else
            {
                SaveChanges();
            }

            return returnModel;
        }

        public bool CreateCar(Client _Owner, GrandTheftMultiplayer.Shared.VehicleHash _vehicle)
        {
            try
            {
                var _veh = new Models.Vehicle
                {
                    VehicleId = currentVehList.Items.Count > 0 ? currentVehList.Items.LastOrDefault().VehicleId + 1 : 1,
                    Plate = "LS" + (currentVehList.Items.Count > 0 ? currentVehList.Items.LastOrDefault().VehicleId + 1 : 1).ToString(),
                    //Color1 = 131,
                    //Color2 = 131,
                    Fuel = 100,
                    IsLocked = false,
                    LastPosition = _Owner.position,
                    LastRotation = _Owner.rotation,
                    OwnerSocialClubName = _Owner.socialClubName,
                    VehicleModelId = _vehicle,
                    FactionId = -1,
                    JobId = -1,
                };

                for (int i = 0; i <= 69; i++)
                {
                    _veh.Mods[i] = -1;
                }

                currentVehList.Items.Add(_veh);
                SaveChanges();


                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public bool CreateCar(Vector3 _position, GrandTheftMultiplayer.Shared.VehicleHash _vehicle)
        {
            try
            {
                var _veh = new Models.Vehicle
                {
                    VehicleId = currentVehList.Items.Count > 0 ? currentVehList.Items.LastOrDefault().VehicleId + 1 : 1,
                    Plate = "LS" + (currentVehList.Items.Count > 0 ? currentVehList.Items.LastOrDefault().VehicleId + 1 : 1).ToString(),
                    Color1 = 131,
                    Color2 = 131,
                    Fuel = 100,
                    IsLocked = false,
                    LastPosition = _position,
                    LastRotation = _position,
                    OwnerSocialClubName = "",
                    VehicleModelId = _vehicle,
                    FactionId = -1,
                    JobId = -1
                };

                for (int i = 0; i <= 69; i++)
                {
                    _veh.Mods[i] = -1;
                }

                currentVehList.Items.Add(_veh);
                SaveChanges();


                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }
        public static bool OwnACar(Client _Owner, GrandTheftMultiplayer.Server.Elements.Vehicle _vehicle, GrandTheftMultiplayer.Shared.VehicleHash _VehModelId)
        {
            try
            {
                var _veh = new Models.Vehicle
                {
                    VehicleId = currentVehList.Items.Count > 0 ? currentVehList.Items.LastOrDefault().VehicleId + 1 : 1,
                    Plate = "LS" + (currentVehList.Items.Count > 0 ? currentVehList.Items.LastOrDefault().VehicleId + 1 : 1).ToString(),
                    //Color1 = _vehicle.primaryColor,
                    //Color2 = _vehicle.secondaryColor,
                    Fuel = 100,
                    IsLocked = false,
                    LastPosition = _vehicle.position,
                    LastRotation = _vehicle.rotation,
                    OwnerSocialClubName = _Owner.socialClubName,
                    VehicleModelId = _VehModelId,
                    FactionId = -1,
                    JobId = -1,
                };

                for (int i = 0; i <= 69; i++)
                {
                    _veh.Mods[i] = -1;
                }

                currentVehList.Items.Add(_veh);
                SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                API.shared.consoleOutput(LogCat.Warn, ex.ToString());
                return false;
            }
        }



        public static bool CreateVehicle(VehicleHash vehModel, Client _Owner)
        {
            try
            {
                if (vehModel.ToString() == "0") { return false; }
                currentVehList.Items.Add(new Models.Vehicle
                {
                    VehicleOnMap = API.shared.createVehicle(vehModel, _Owner.position, _Owner.rotation,5,10, _Owner.dimension),
                    VehicleId = currentVehList.Items.Count > 0 ? currentVehList.Items.LastOrDefault().VehicleId + 1 : 1,
                    Plate = "LS-" + (currentVehList.Items.Count > 0 ? currentVehList.Items.LastOrDefault().VehicleId + 1 : 1),
                    Dimension = _Owner.dimension,
                    FactionId = -1,
                    JobId = -1,
                    VehicleModelId = vehModel,
                    Fuel = 100,
                    IsBlockedForTax = false,
                    IsLocked = false,
                    LastPosition = _Owner.position,
                    LastRotation = _Owner.rotation,
                    mod_color1 = 0,
                    mod_color2 = 0,
                    Color1 = 5,
                    Color2 = 10,
                    PastMinutes = 0,
                    OwnerSocialClubName = _Owner.socialClubName,
                    Tax = 0,
                });
                currentVehList.Items.LastOrDefault().VehicleOnMap.numberPlate = currentVehList.Items.LastOrDefault().Plate;
                for (int i = 0; i < 69; i++)
                {
                    currentVehList.Items.LastOrDefault().Mods[i] = -1;
                }
                SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                API.shared.consoleOutput(LogCat.Warn, "Vehicle Creation Error: " + ex.ToString());
                return false;
            }
        }
        public static bool CreateVehicle(VehicleHash vehModel, Vector3 pos, Vector3 rot, int dim = 0, string ownerSocialClubName = null)
        {
            try
            {
                Random r = new Random();
                currentVehList.Items.Add(new Models.Vehicle
                {
                    VehicleOnMap = API.shared.createVehicle(vehModel, pos, rot, 5, 10, dim),
                    VehicleId = currentVehList.Items.Count > 0 ? currentVehList.Items.LastOrDefault().VehicleId + 1 : 1,
                    Plate = "LS-" + (currentVehList.Items.Count > 0 ? currentVehList.Items.LastOrDefault().VehicleId + 1 : 1),
                    Dimension = dim,
                    FactionId = -1,
                    JobId = -1,
                    VehicleModelId = vehModel,
                    Fuel = 100,
                    IsBlockedForTax = false,
                    IsLocked = false,
                    LastPosition = pos,
                    LastRotation = rot,
                    mod_color1 = 0,
                    mod_color2 = 0,
                    Color1 = 5,
                    Color2 = 10,
                    PastMinutes = 0,
                    OwnerSocialClubName = ownerSocialClubName,
                    Tax = 0,
                });
                currentVehList.Items.LastOrDefault().VehicleOnMap.numberPlate = currentVehList.Items.LastOrDefault().Plate;
                for (int i = 0; i < 69; i++)
                {
                    currentVehList.Items.LastOrDefault().Mods[i] = -1;
                }
                SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                API.shared.consoleOutput(LogCat.Warn, "Vehicle Creation Error: " + ex.ToString());
                return false;
            }
        }
        public static bool CreateRentedVehicle(VehicleHash vehModel,string rentedSocialClubName, Vector3 pos, Vector3 rot, int dim = 0)
        {
            try
            {
                Random r = new Random();
                if (vehModel.ToString() == "0") { return false; }
                currentVehList.Items.Add(new Models.Vehicle
                {
                    VehicleOnMap = API.shared.createVehicle(vehModel,pos,rot, r.Next(0,125), r.Next(0,125), dim),
                    VehicleId = currentVehList.Items.Count > 0 ? currentVehList.Items.LastOrDefault().VehicleId + 1 : 1,
                    Plate = "LS-" + (currentVehList.Items.Count > 0 ? currentVehList.Items.LastOrDefault().VehicleId + 1 : 1),
                    Dimension = dim,
                    FactionId = -1,
                    JobId = -1,
                    VehicleModelId = vehModel,
                    Fuel = 100,
                    IsBlockedForTax = false,
                    IsLocked = false,
                    LastPosition = pos,
                    LastRotation = rot,
                    mod_color1 = 0,
                    mod_color2 = 0,
                    Color1 = 5,
                    Color2 = 10,
                    PastMinutes = 0,
                    OwnerSocialClubName = null,
                    RentedTime = DateTime.Now,
                    RentedPlayerSocialClubId = rentedSocialClubName,
                    Tax = 0,
                });
                currentVehList.Items.LastOrDefault().VehicleOnMap.numberPlate = currentVehList.Items.LastOrDefault().Plate;
                for (int i = 0; i < 69; i++)
                {
                    //if (i == 24) { continue; }
                    //if (i == 23) { currentVehList.Items.LastOrDefault().Mods[i] = 0; currentVehList.Items.LastOrDefault().VehicleOnMap.setMod(i, 0); }
                    currentVehList.Items.LastOrDefault().Mods[i] = -1;
                    //currentVehList.Items.LastOrDefault().VehicleOnMap.setMod(i, -1);
                }
                SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                API.shared.consoleOutput(LogCat.Warn, "Vehicle Creation Error: " + ex.ToString());
                return false;
            }
        }
        public static void SaveChanges()
        {

            if (System.IO.Directory.Exists(dataPath.Split('/')[0]))
            {
                XmlTextWriter xWriter = new XmlTextWriter(dataPath, System.Text.UTF8Encoding.UTF8);
                xWriter.Formatting = Formatting.Indented;
                xSer.Serialize(xWriter, currentVehList);
                xWriter.Dispose();
            }
            else
            {
                System.IO.Directory.CreateDirectory(dataPath.Split('/')[0]);
            }
        }
        public static void SaveChanges(TaxesList _list)
        {
            lock (currentVehList)
            {
                if (System.IO.Directory.Exists(dataPathTax.Split('/')[0]))
                {
                    XmlTextWriter xWriter = new XmlTextWriter(dataPathTax, System.Text.UTF8Encoding.UTF8);
                    xWriter.Formatting = Formatting.Indented;
                    xSer.Serialize(xWriter, _list);
                    xWriter.Dispose();
                }
                else
                {
                    System.IO.Directory.CreateDirectory(dataPathTax.Split('/')[0]);
                } 
            }
        }

        public static void UpdateAndSaveChanges()
        {
            //try
            //{
            //    foreach (var itemVeh in currentVehList.Items)
            //    {
            //        UpdateVehicleToModel(itemVeh);
            //    }
            //    API.shared.consoleOutput(currentVehList.Items.Count + " araç kaydedildi.");
            //}
            //catch (Exception ex)
            //{
            //    API.shared.consoleOutput(LogCat.Fatal, "Araçlar kaydedilirken hata: " + ex.ToString());
            //}
        }
    }
}
