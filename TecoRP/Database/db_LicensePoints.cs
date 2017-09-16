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
using TecoRP.Models;

namespace TecoRP.Database
{
    public class db_LicensePoints
    {
        static XmlSerializer xSer = new XmlSerializer(typeof(LicenseTakingsList));
        static XmlSerializer xSerVehChk = new XmlSerializer(typeof(VehicleLicenseCheckpointList));
        public static string dataPath = "Data/LicenseTakings.xml";
        public static string dataPathVehChk = "Data/VehLicenseCheckpoints.xml";
        public static Tuple<List<LicenseTaking>, List<TextLabel>,List<Ped>> CurrentLicenseTakings = new Tuple<List<LicenseTaking>, List<TextLabel>,List<Ped>>(new List<LicenseTaking>(), new List<TextLabel>(), new List<Ped>());
        public static VehicleLicenseCheckpointList currentVehLicenseCheckpoints = new VehicleLicenseCheckpointList();

        public static void SpawnAll()
        {
            foreach (var item in GetAll().Items)
            {
                CurrentLicenseTakings.Item1.Add(item);
                CurrentLicenseTakings.Item2.Add(API.shared.createTextLabel(item.Text,item.Position+new Vector3(0,0,0.5),10,1,true,item.Dimension));
                CurrentLicenseTakings.Item3.Add(API.shared.createPed(item.Ped,item.Position,1,item.Dimension));
                CurrentLicenseTakings.Item3.LastOrDefault().rotation = item.Rotation;
            }
        }

        public static void InitVehicleLicenseChks()
        {
            foreach (var itemVehLicChk in GetAllVehLicenseChks().Items)
            {
                itemVehLicChk.ColshapeOnMap = API.shared.createCylinderColShape(itemVehLicChk.Position, 3, 3);
                itemVehLicChk.ColshapeOnMap.dimension = itemVehLicChk.Dimension;
                itemVehLicChk.ColshapeOnMap.onEntityEnterColShape += ColshapeOnMap_onEntityEnterColShape;
            }
        }

        private static void ColshapeOnMap_onEntityEnterColShape(GrandTheftMultiplayer.Server.Managers.ColShape shape, GrandTheftMultiplayer.Shared.NetHandle entity)
        {
            //API.shared.consoleOutput("triggered");
            Managers.LicenseManager.ColshapeOnMap_onEntityEnterColShape(shape, entity);
        }

        public static LicenseTakingsList GetAll()
        {
            LicenseTakingsList returnModel = new Models.LicenseTakingsList();
            if (System.IO.File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(LicenseTakingsList), new XmlRootAttribute("LicenseTaking_List"));
                    returnModel = (LicenseTakingsList)deserializer.Deserialize(reader);
                }
            }
            else
            {
                SaveChanges();
            }
            return returnModel;
        }

        public static VehicleLicenseCheckpointList GetAllVehLicenseChks()
        {
            
            if (System.IO.File.Exists(dataPathVehChk))
            {
                using (var reader = new StreamReader(dataPathVehChk))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(VehicleLicenseCheckpointList), new XmlRootAttribute("VehLicenseCheckpoint_List"));
                    currentVehLicenseCheckpoints = (VehicleLicenseCheckpointList)deserializer.Deserialize(reader);
                }
            }
            else
            {
                SaveChanges();
            }
            return currentVehLicenseCheckpoints;
        }

        public static void Create(LicenseTaking _model)
        {
            _model.ID = CurrentLicenseTakings.Item1.Count > 0 ? CurrentLicenseTakings.Item1.LastOrDefault().ID + 1 : 1;
            CurrentLicenseTakings.Item1.Add(_model);
            CurrentLicenseTakings.Item2.Add(API.shared.createTextLabel(_model.Text,_model.Position,15,1,true,_model.Dimension));
            CurrentLicenseTakings.Item3.Add(API.shared.createPed(_model.Ped, _model.Position, 1, _model.Dimension));
            CurrentLicenseTakings.Item3.LastOrDefault().rotation = _model.Rotation;
            SaveChanges();
        }

        public static void CreateVehLicenseChk(VehLicenseCheckpoint _model)
        {
            if (_model == null) return;

            _model.ID = currentVehLicenseCheckpoints.Items.Count > 0 ? currentVehLicenseCheckpoints.Items.LastOrDefault().ID + 1 : 1;
            _model.ColshapeOnMap = API.shared.createCylinderColShape(_model.Position, 3, 3);
            _model.ColshapeOnMap.dimension = _model.Dimension;
            _model.ColshapeOnMap.onEntityEnterColShape += ColshapeOnMap_onEntityEnterColShape;
            currentVehLicenseCheckpoints.Items.Add(_model);
            SaveChanges(true);
        }
        public static LicenseTaking GetById(int _Id)
        {
            var _Index = FindIndexByID(_Id);
            if (_Index >=0)
            {
                return CurrentLicenseTakings.Item1[_Index];
            }else
            {
                return null;
            }
        } 
        public static VehLicenseCheckpoint GetLicenseCheckPoint(int _Id)
        {
            return currentVehLicenseCheckpoints.Items.FirstOrDefault(x => x.ID == _Id);
        }
        public static bool Update(LicenseTaking _model)
        {
            var _Index = FindIndexByID(_model.ID);
            if (_Index >=0)
            {
                try
                {
                    CurrentLicenseTakings.Item1[_Index] = _model;
                    CurrentLicenseTakings.Item2[_Index].text = _model.Text;
                    CurrentLicenseTakings.Item2[_Index].position = _model.Position+new Vector3(0,0,0.5);
                    CurrentLicenseTakings.Item2[_Index].dimension= _model.Dimension;
                    CurrentLicenseTakings.Item3[_Index].position= _model.Position;
                    CurrentLicenseTakings.Item3[_Index].rotation = _model.Rotation;

                    SaveChanges();

                    return true;
                }
                catch (Exception ex)
                {
                    API.shared.consoleOutput("LicensePoint | UPDATE | Hata: " + ex.Message);
                    return false;
                }
            }
            else
            {
                return false;

            }
        }
        public static bool Update(VehLicenseCheckpoint _model)
        {
            if (_model == null) return false;
            var _Index = FindVehLicenseChkIndexById(_model.ID);
            if (_Index >= 0)
            {
                try
                {
                    currentVehLicenseCheckpoints.Items[_Index].Position = _model.Position;
                    currentVehLicenseCheckpoints.Items[_Index].Dimension = _model.Dimension;
                    API.shared.deleteColShape(currentVehLicenseCheckpoints.Items[_Index].ColshapeOnMap);
                    currentVehLicenseCheckpoints.Items[_Index].ColshapeOnMap = API.shared.createCylinderColShape(_model.Position, 3, 3);
                    currentVehLicenseCheckpoints.Items[_Index].ColshapeOnMap.dimension = _model.Dimension;
                    SaveChanges(true);
                    return true;
                }
                catch (Exception ex)
                {
                    API.shared.consoleOutput(LogCat.Warn, ex.ToString());
                    return false;
                }
            }
            else
                return false;
            

        }
        public static bool Remove(int _Id)
        {
            var _Index = FindIndexByID(_Id);
            if (_Index>=0)
            {
                try
                {
                    API.shared.deleteEntity(CurrentLicenseTakings.Item2[_Index]);
                    API.shared.deleteEntity(CurrentLicenseTakings.Item3[_Index]);
                    CurrentLicenseTakings.Item1.RemoveAt(_Index);
                    CurrentLicenseTakings.Item2.RemoveAt(_Index);
                    CurrentLicenseTakings.Item3.RemoveAt(_Index);
                    SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    if (ex.GetType() != typeof(IndexOutOfRangeException))
                    {
                        API.shared.consoleOutput("Remove | License | Error: " + ex.ToString());
                    }
                    return false;
                }
            }else
            {
                return false;
            }
        }
        public static bool RemoveVehLicenseCheckpoint(int _Id)
        {
            var _Index = FindVehLicenseChkIndexById(_Id);
            if (_Index >= 0)
            {
                try
                {
                    API.shared.deleteColShape(currentVehLicenseCheckpoints.Items[_Index].ColshapeOnMap);
                    currentVehLicenseCheckpoints.Items.RemoveAt(_Index);
                    SaveChanges(true);
                    return true;
                }
                catch (Exception ex)
                {
                    if (ex.GetType() == typeof(IndexOutOfRangeException))
                    {
                        API.shared.consoleOutput("Index çok büyüktü: " + ex.ToString());
                    }
                    API.shared.consoleOutput(LogCat.Warn, ex.ToString());
                    return false;
                }
            }
            else
                return false;
        }
        public static int FindIndexByID(int _Id)
        {
            return CurrentLicenseTakings.Item1.IndexOf(CurrentLicenseTakings.Item1.FirstOrDefault(x => x.ID == _Id));
        }
        public static int FindVehLicenseChkIndexById(int _Id)
        {
            return currentVehLicenseCheckpoints.Items.IndexOf(currentVehLicenseCheckpoints.Items.FirstOrDefault(x => x.ID == _Id));
        }
        public static void SaveChanges()
        {
            if (System.IO.Directory.Exists(dataPath.Split('/')[0]))
            {
                XmlTextWriter xWriter = new XmlTextWriter(dataPath, System.Text.UTF8Encoding.UTF8);
                xWriter.Formatting = Formatting.Indented;
                xSer.Serialize(xWriter, new LicenseTakingsList { Items = CurrentLicenseTakings.Item1 });
                xWriter.Dispose();
            }
            else
            {
                System.IO.Directory.CreateDirectory(dataPath.Split('/')[0]);
            }
        }
        public static void SaveChanges(bool vehLicense)
        {
            if(!vehLicense) { SaveChanges(); return; }

            if (System.IO.Directory.Exists(dataPathVehChk.Split('/')[0]))
            {
                XmlTextWriter xWriter = new XmlTextWriter(dataPathVehChk, System.Text.UTF8Encoding.UTF8);
                xWriter.Formatting = Formatting.Indented;
                xSerVehChk.Serialize(xWriter, currentVehLicenseCheckpoints);
                xWriter.Dispose();
            }
            else
            {
                System.IO.Directory.CreateDirectory(dataPathVehChk.Split('/')[0]);
            }
        }
    }
}
