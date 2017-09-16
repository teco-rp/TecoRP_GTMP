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
    public class db_Craftings
    {
        static XmlSerializer xSer = new XmlSerializer(typeof(CraftingTablesOnMapList));
        static XmlSerializer xSerReady = new XmlSerializer(typeof(CraftingTableList));
        public static string dataPath = "Data/CraftingTables.teco";
        public static string dataPathReady = "Data/Craftings.teco";

        public static Dictionary<int, CraftingTable> currentCraftingTables = new Dictionary<int, CraftingTable>();
        public static Dictionary<int, CraftingTablesOnMap> craftingTablesOnMap = new Dictionary<int, CraftingTablesOnMap>();

        public static void SpawnAll()
        {
            API.shared.consoleOutput("CraftingTables yüklenmeye başladı.");
            foreach (var itemCTable in GetAll().TablesOnMap)
            {
                var _table = GetCraftingTableModel(itemCTable.CraftingTableModelId);
                itemCTable.TableOnMap = API.shared.createObject(_table.ObjectId, itemCTable.Position+new Vector3(0,0,-1), itemCTable.Rotation, itemCTable.Dimension);
                itemCTable.TextLabelOnMap = API.shared.createTextLabel(itemCTable.Name, itemCTable.Position, 15, 1, true, itemCTable.Dimension);
                craftingTablesOnMap.Add(itemCTable.TableOnMapId, itemCTable);
            }
            API.shared.consoleOutput($"{craftingTablesOnMap.Count} adet CraftingTable başarıyla yüklendi.");
        }

        public static CraftingTablesOnMapList GetAll()
        {
            CraftingTablesOnMapList returnModel = new Models.CraftingTablesOnMapList();
            if (System.IO.File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(CraftingTablesOnMapList), new XmlRootAttribute("CraftingTablesOnMap_List"));
                    returnModel = (CraftingTablesOnMapList)deserializer.Deserialize(reader);
                }
            }
            else
            {
                SaveChanges();
            }

            return returnModel;
        }
        public static List<CraftingTable> GetAllCraftingTableModels()
        {
            return currentCraftingTables.Values.ToList();
        }
        public static void Init()
        {
            if (System.IO.File.Exists(dataPathReady))
            {
                currentCraftingTables.Clear();
                var model = new CraftingTableList();
                using (var reader = new StreamReader(dataPathReady))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(CraftingTableList), new XmlRootAttribute("CraftingTable_List"));
                    model = (CraftingTableList)deserializer.Deserialize(reader);
                }
                foreach (var item in model.Tables)
                {
                    currentCraftingTables.Add(item.CraftingTableId, item);
                }
            }
            else
            {
                SaveChanges();
            }
        }

        public static CraftingTablesOnMap GetCraftingTableOnMap(int id)
        {
            return craftingTablesOnMap[id];
        }
        public static bool UpdateCraftingTableOnMap(CraftingTablesOnMap _model)
        {
            if (_model != null)
            {
                try
                {
                    var editedModel = GetCraftingTableOnMap(_model.CraftingTableModelId);
                    API.shared.deleteEntity(editedModel.TableOnMap);
                    var _table = GetCraftingTableModel(_model.CraftingTableModelId);
                    if (_table == null) return false;
                    editedModel.TableOnMap = API.shared.createObject(_table.ObjectId, _model.Position + new Vector3(0,0,-1), _model.Rotation, _model.Dimension);
                    editedModel.TextLabelOnMap.position = _model.Position;
                    editedModel.TextLabelOnMap.dimension = _model.Dimension;
                    SaveChanges(saveType.OnMap);
                    return true;
                }
                catch (Exception ex)
                {
                    API.shared.consoleOutput(LogCat.Warn, ex.ToString());
                }
                return false;
            }
            else
                API.shared.consoleOutput(LogCat.Warn, "UpdateCraftingTable model boş gönderildi.");
            return false;
        }
        public static bool CreateCraftingTableOnMap(CraftingTablesOnMap _model)
        {
            try
            {
                
                var _table = GetCraftingTableModel(_model.CraftingTableModelId);
                if (_table == null) return false;
                _model.TableOnMapId = craftingTablesOnMap.Count > 0 ? craftingTablesOnMap.LastOrDefault().Key + 1 : 1;
                _model.TableOnMap = API.shared.createObject(_table.ObjectId, _model.Position + new Vector3(0,0,-1), _model.Rotation, _model.Dimension);
                _model.TextLabelOnMap = API.shared.createTextLabel(_model.Name, _model.Position, 15, 1, true, _model.Dimension);
                craftingTablesOnMap.Add(_model.TableOnMapId, _model);
                SaveChanges(saveType.OnMap);
                API.shared.consoleOutput("saved crafting");
                return true;
            }
            catch (Exception ex)
            {
                API.shared.consoleOutput(LogCat.Warn, ex.ToString());
            }
            return false;
        }
        public static bool RemoveCraftingTableOnMap(int id)
        {
            var _table = GetCraftingTableOnMap(id);
            if (_table != null)
            {
                try
                {
                    API.shared.deleteEntity(_table.TextLabelOnMap);
                    API.shared.deleteEntity(_table.TableOnMap);
                    var result = craftingTablesOnMap.Remove(id);
                    SaveChanges();
                    return result;
                }
                catch (Exception ex)
                {
                    API.shared.consoleOutput(LogCat.Warn, ex.ToString());
                }
            }
            else
            {
                API.shared.consoleOutput(LogCat.Warn, "RemoveCraftingTableOnMap Yanlış ID gönderildi.");
            }
            return false;
        }
        public static CraftingTablesOnMap GetNearestCraftingTableOnMap(Vector3 position)
        {
            CraftingTablesOnMap nearest = craftingTablesOnMap.FirstOrDefault().Value;

            foreach (var itemTable in craftingTablesOnMap.Values)
            {
                if (Vector3.Distance(itemTable.Position, position) < Vector3.Distance(nearest.Position, position))
                {
                    nearest = itemTable;
                }
            }

            return nearest;
        }
        public static CraftingTable GetCraftingTableModel(int id)
        {
            return currentCraftingTables[id];
        }
        public static CraftingTable CreateCraftingTableModel(CraftingTable _model)
        {
            _model.CraftingTableId = currentCraftingTables.Count > 0 ? currentCraftingTables.LastOrDefault().Key + 1 : 1;
            currentCraftingTables.Add(_model.CraftingTableId, _model);
            SaveChanges(saveType.Model);
            return currentCraftingTables.LastOrDefault().Value;
        }
        public static bool RemoveCraftingTableModel(int id)
        {
            try
            {
                var result = currentCraftingTables.Remove(id);
                SaveChanges(saveType.Model);
                return result;
            }
            catch (Exception ex)
            {
                API.shared.consoleOutput(ex.ToString());
            }
            return false;
        }
        public static void AddCraftingToCraftingTAbleModel(int tableModelID, CraftingItem _craft)
        {
            var _table = GetCraftingTableModel(tableModelID);
            if (_table != null)
            {
                _table.Craftings.Add(_craft);
                SaveChanges(saveType.Model);
            }
        }
        public static void ClearCraftingsFromTableModel(int tableModelID)
        {
            var _table = GetCraftingTableModel(tableModelID);
            if (_table != null)
            {
                _table.Craftings.Clear();
                SaveChanges(saveType.Model);
            }
        }
        public enum saveType
        {
            OnMap,
            Model
        }
        public static void SaveChanges(saveType _Type = saveType.OnMap)
        {
            switch (_Type)
            {
                case saveType.OnMap:
                    if (Directory.Exists(dataPath.Split('/')[0]))
                    {
                        XmlTextWriter xWriter = new XmlTextWriter(dataPath, System.Text.UTF8Encoding.UTF8);
                        xWriter.Formatting = Formatting.Indented;
                        xSer.Serialize(xWriter, new CraftingTablesOnMapList { TablesOnMap = craftingTablesOnMap.Values.ToList() });
                        xWriter.Dispose();
                    }
                    else
                    {
                        Directory.CreateDirectory(dataPath.Split('/')[0]);
                    }
                    break;
                case saveType.Model:
                    if (Directory.Exists(dataPathReady.Split('/')[0]))
                    {
                        XmlTextWriter xWriter = new XmlTextWriter(dataPathReady, System.Text.UTF8Encoding.UTF8);
                        xWriter.Formatting = Formatting.Indented;
                        xSerReady.Serialize(xWriter, new CraftingTableList { Tables = currentCraftingTables.Values.ToList() });
                        xWriter.Dispose();
                    }
                    else
                    {
                        Directory.CreateDirectory(dataPath.Split('/')[0]);
                    }

                    break;
            }
        }

    }
}
