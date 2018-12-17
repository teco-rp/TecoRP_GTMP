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
using TecoRP.Repository.Base;

namespace TecoRP.Database
{

    public class db_Vehicles
    {
        static IBaseRepository<Models.Vehicle> _repository = new JsonRepositoryBase<Models.Vehicle>("Data/Vehicles.json");
        static XmlSerializer xSerTax = new XmlSerializer(typeof(TaxesList));
        public static string dataPathTax = "Data/Taxes.xml";

        public db_Vehicles()
        {

        }

        public static void SpawnAll()
        {
            API.shared.consoleOutput("Araçlar yüklenmeye başladı.");

            foreach (var itemVeh in GetAll())
                GenerateOnMap(itemVeh);

            API.shared.consoleOutput(_repository.Current.Count + " araç başarıyla yüklendi.");
        }
        public static void RespawnAll()
        {
            foreach (var itemVeh in _repository.Get())
            {
                try
                {
                    if (API.shared.getVehicleOccupants(itemVeh.VehicleOnMap).Length > 0) continue;

                    RemoveFromMap(itemVeh);
                    GenerateOnMap(itemVeh);
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



                    for (int i = 0; i < 69; i++)
                    {
                        if (_vehicle.Mods[i] == -1) continue;
                        _vehicle.VehicleOnMap.setMod(i, _vehicle.Mods[i]);
                    }
                }
                catch (Exception ex)
                {
                    API.shared.consoleOutput(LogCat.Warn, ex.ToString());
                }
            }
        }
        public static IList<Models.Vehicle> GetAll()
        {
            return _repository.Get();
        }
        /// <summary>
        /// Doesn't effect to game. Just read datas
        /// </summary>
        /// <returns></returns>
        public static VehicleList ReadAll(bool isServerMapPath = false)
        {
            return new VehicleList
            {
                Items = _repository.Get().ToList()
            };
        }

        public static bool RemoveVehicle(long vehicleId)
        {
            try
            {
                var _veh = _repository.GetSingle(x => x.VehicleId == vehicleId);
                if (_veh != null)
                {
                    API.shared.deleteEntity(_veh.VehicleOnMap);
                    _repository.Remove(_veh);
                    return true;
                }
                return false;
            }
            catch (Exception ex)
            {
                API.shared.consoleOutput(LogCat.Fatal, "Removing Vehicle Error: " + ex.ToString());
                return false;
            }
        }
        public static bool RemoveVehicle(Models.Vehicle _model)
        {
            try
            {
                API.shared.deleteEntity(_model.VehicleOnMap);
                _repository.Remove(_model);
                return true;


            }
            catch (Exception ex)
            {
                API.shared.consoleOutput(LogCat.Warn, "Error while deleting Vehicle: " + ex.ToString());
                return false;
            }
        }

        public static Models.Vehicle FindVehicle(NetHandle handle)
        {
            return _repository.Current.FirstOrDefault(x => x.VehicleOnMap.handle == handle);
        }

        public static Models.Vehicle FindNearestVehicle(Vector3 pos)
        {
            Models.Vehicle nearestVehicle = _repository.Current.FirstOrDefault();
            foreach (var itemVeh in _repository.Current)
            {
                if (Vector3.Distance(pos, itemVeh.VehicleOnMap.position) < Vector3.Distance(pos, nearestVehicle.VehicleOnMap.position))
                {
                    nearestVehicle = itemVeh;
                }
            }
            return nearestVehicle;
        }
        public static Models.Vehicle FindNearestVehicle(Vector3 pos, int JobId)
        {
            Models.Vehicle nearestVehicle = _repository.Current.FirstOrDefault();
            foreach (var itemVeh in _repository.Current)
            {
                float _dist = Vector3.Distance(pos, itemVeh.VehicleOnMap.position);
                if (JobId == itemVeh.JobId && _dist < Vector3.Distance(pos, nearestVehicle.VehicleOnMap.position))
                {
                    nearestVehicle = itemVeh;
                }
            }
            return nearestVehicle;
        }

        public static Models.Vehicle GetVehicle(long vehicleId)
        {
            return _repository.GetSingle(x => x.VehicleId == vehicleId);
        }

        public static bool UpdateVehicleToModel(Models.Vehicle _model)
        {
            try
            {
                _model.LastPosition = _model.VehicleOnMap.position;
                _model.LastRotation = _model.VehicleOnMap.rotation;
                _model.Dimension = _model.VehicleOnMap.dimension;
                _model.IsLocked = _model.VehicleOnMap.locked;

                for (int i = 0; i < 69; i++)
                {
                    _model.Mods[i] = _model.VehicleOnMap.getMod(i);
                }
                _repository.Update(_model);
                return true;
            }
            catch (Exception ex)
            {
                API.shared.consoleOutput(LogCat.Fatal, ex.ToString());
                return false;
            }
        }

        public static bool IsPlayersVehicleById(Client sender, long _vehId)
        {
            return _repository.GetSingle(x => x.VehicleId == _vehId).OwnerSocialClubName == sender.socialClubName;
        }
        public static IList<Models.Vehicle> GetPlayerVehicles(Client sender)
        {
            return _repository.Get(x => x.OwnerSocialClubName == sender.socialClubName);
        }
        public static IEnumerable<Models.Vehicle> GetOfflinePlayerVehicles(string socialClubName, bool serverMapPath = true)
        {
            foreach (var item in ReadAll(true).Items)
                if (item.OwnerSocialClubName == socialClubName)
                    yield return item;
        }

        //TODO: Move to new JSON Serializer
        public static List<Tax> GetVehicleTaxes()
        {
            List<Tax> returnModel = new List<Models.Tax>();
            if (File.Exists(dataPathTax))
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
                var id = _repository.Current.Count > 0 ? _repository.Current.LastOrDefault().VehicleId + 1 : 1;
                var _veh = new Models.Vehicle
                {
                    VehicleId = id,
                    Plate = "LS" + id,
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

                _repository.Add(_veh);
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
                var id = _repository.Current.Count > 0 ? _repository.Current.LastOrDefault().VehicleId + 1 : 1;
                var _veh = new Models.Vehicle
                {
                    VehicleId = id,
                    Plate = "LS" + id,
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

                _repository.Add(_veh);
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
                var id = _repository.Current.Count > 0 ? _repository.Current.LastOrDefault().VehicleId + 1 : 1;
                var _veh = new Models.Vehicle
                {
                    VehicleId = id,
                    Plate = "LS" + id,
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

                _repository.Add(_veh);
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
                var id = _repository.Current.Count > 0 ? _repository.Current.LastOrDefault().VehicleId + 1 : 1;
                var _veh = new Models.Vehicle
                {
                    VehicleOnMap = API.shared.createVehicle(vehModel, _Owner.position, _Owner.rotation, 5, 10, _Owner.dimension),
                    VehicleId = id,
                    Plate = "LS-" + id,
                    Dimension = _Owner.dimension,
                    FactionId = -1,
                    JobId = -1,
                    VehicleModelId = vehModel,
                    Fuel = 100,
                    IsBlockedForTax = false,
                    IsLocked = false,
                    LastPosition = _Owner.position,
                    LastRotation = _Owner.rotation,
                    ModColor1 = 0,
                    ModColor2 = 0,
                    Color1 = 5,
                    Color2 = 10,
                    PastMinutes = 0,
                    OwnerSocialClubName = _Owner.socialClubName,
                    Tax = 0,
                };

                _repository.Add(_veh);
                _veh.VehicleOnMap.numberPlate = _veh.Plate;
                for (int i = 0; i < 69; i++)
                {
                    _veh.Mods[i] = -1;
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
        public static Models.Vehicle CreateVehicle(VehicleHash vehModel, Vector3 pos, Vector3 rot, int dim = 0, string ownerSocialClubName = null)
        {
            try
            {
                var id = _repository.Current.Count > 0 ? _repository.Current.LastOrDefault().VehicleId + 1 : 1;

                var _veh = new Models.Vehicle
                {
                    VehicleOnMap = API.shared.createVehicle(vehModel, pos, rot, 5, 10, dim),
                    VehicleId = id,
                    Plate = "LS-" + id,
                    Dimension = dim,
                    FactionId = -1,
                    JobId = -1,
                    VehicleModelId = vehModel,
                    Fuel = 100,
                    IsBlockedForTax = false,
                    IsLocked = false,
                    LastPosition = pos,
                    LastRotation = rot,
                    ModColor1 = 0,
                    ModColor2 = 0,
                    Color1 = 5,
                    Color2 = 10,
                    PastMinutes = 0,
                    OwnerSocialClubName = ownerSocialClubName,
                    Tax = 0,
                };

                _repository.Add(_veh);


                _veh.VehicleOnMap.numberPlate = _veh.Plate;
                for (int i = 0; i < 69; i++)
                {
                    _veh.Mods[i] = -1;
                }
                SaveChanges();
                return _veh;
            }
            catch (Exception ex)
            {
                API.shared.consoleOutput(LogCat.Warn, "Vehicle Creation Error: " + ex.ToString());
                return null;
            }
        }
        public static bool CreateRentedVehicle(VehicleHash vehModel, string rentedSocialClubName, Vector3 pos, Vector3 rot, int dim = 0)
        {
            try
            {
                Random r = new Random();
                if (vehModel.ToString() == "0") { return false; }
                var id = _repository.Current.Count > 0 ? _repository.Current.LastOrDefault().VehicleId + 1 : 1;

                var _veh = new Models.Vehicle
                {
                    VehicleOnMap = API.shared.createVehicle(vehModel, pos, rot, r.Next(0, 125), r.Next(0, 125), dim),
                    VehicleId = id,
                    Plate = "LS-" + id,
                    Dimension = dim,
                    FactionId = -1,
                    JobId = -1,
                    VehicleModelId = vehModel,
                    Fuel = 100,
                    IsBlockedForTax = false,
                    IsLocked = false,
                    LastPosition = pos,
                    LastRotation = rot,
                    ModColor1 = 0,
                    ModColor2 = 0,
                    Color1 = 5,
                    Color2 = 10,
                    PastMinutes = 0,
                    OwnerSocialClubName = null,
                    RentedTime = DateTime.Now,
                    RentedPlayerSocialClubId = rentedSocialClubName,
                    Tax = 0,
                };

                _repository.Add(_veh);

                _veh.VehicleOnMap.numberPlate = _veh.Plate;
                for (int i = 0; i < 69; i++)
                {
                    //if (i == 24) { continue; }
                    //if (i == 23) { currentVehList.Items.LastOrDefault().Mods[i] = 0; currentVehList.Items.LastOrDefault().VehicleOnMap.setMod(i, 0); }
                    _veh.Mods[i] = -1;
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
            _repository.SaveChanges();
        }
        public static void SaveChanges(TaxesList _list)
        {

            if (System.IO.Directory.Exists(dataPathTax.Split('/')[0]))
            {
                XmlTextWriter xWriter = new XmlTextWriter(dataPathTax, System.Text.UTF8Encoding.UTF8);
                xWriter.Formatting = Formatting.Indented;
                xSerTax.Serialize(xWriter, _list);
                xWriter.Dispose();
            }
            else
            {
                System.IO.Directory.CreateDirectory(dataPathTax.Split('/')[0]);
            }
        }


        public static Models.Vehicle GenerateOnMap(Models.Vehicle vehicle)
        {
            vehicle.VehicleOnMap = API.shared.createVehicle(vehicle.VehicleModelId, vehicle.LastPosition, vehicle.LastRotation, vehicle.Color1, vehicle.Color2, vehicle.Dimension);
            API.shared.setVehicleEngineStatus(vehicle.VehicleOnMap, false);
            API.shared.setVehicleLocked(vehicle.VehicleOnMap.handle, vehicle.IsLocked);
            API.shared.setVehicleNumberPlate(vehicle.VehicleOnMap.handle, vehicle.Plate);
            API.shared.setVehicleLivery(vehicle.VehicleOnMap, vehicle.Livery);

            for (int i = 0; i < 69; i++)
            {
                if (vehicle.Mods[i] == -1) { continue; }
                vehicle.VehicleOnMap.setMod(i, vehicle.Mods[i]);
            }

            #region DamageLoading
            if (vehicle.SpecifiedDamage != null)
            {
                foreach (var itemDoor in vehicle.SpecifiedDamage.BrokenDoors)
                    API.shared.breakVehicleDoor(vehicle.VehicleOnMap, itemDoor, true);

                foreach (var itemWindow in vehicle.SpecifiedDamage.BrokenWindows)
                    API.shared.breakVehicleWindow(vehicle.VehicleOnMap, itemWindow, true);

                foreach (var itemTyre in vehicle.SpecifiedDamage.PoppedTyres)
                    API.shared.popVehicleTyre(vehicle.VehicleOnMap, itemTyre, true);

            }
            #endregion
            return vehicle;
        }

        public static Models.Vehicle RemoveFromMap(Models.Vehicle vehicle)
        {
            API.shared.deleteEntity(vehicle.VehicleOnMap.handle);
            return vehicle;
        }

        /// <summary>
        /// This sukcs. Saves all vehicled including Job & Faction vehicles.
        /// </summary>
        public static void UpdateAndSaveChanges()
        {
            //foreach (var itemVeh in _repository.Current)
            //    try
            //    {
            //        UpdateVehicleToModel(itemVeh);
            //    }
            //    catch (Exception ex)
            //    {
            //        API.shared.consoleOutput(LogCat.Fatal, itemVeh.VehicleOnMap.model + " araç kaydedilirken hata: \n" + ex.ToString());
            //    }

            //API.shared.consoleOutput(_repository.Current.Count + " araç kaydedildi.");
        }
    }
}
