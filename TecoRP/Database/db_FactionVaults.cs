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
using TecoRP.Models;

namespace TecoRP.Database
{
    public class db_FactionVaults
    {
        static XmlSerializer xSer = new XmlSerializer(typeof(Models.FactionVaultsList));
        public const string dataPath = "Data/FactionVaults.xml";
        public static FactionVaultsList currentVaults = new FactionVaultsList();

        public static void SpawnAll()
        {
            foreach (var item in GetAll().Items)
            {
                try
                {
                    item.TextLabelOnMap = API.shared.createTextLabel(item.Text, item.Position, 15, 1, false, item.Dimension);
                }
                catch (Exception ex)
                {
                    API.shared.consoleOutput(LogCat.Fatal, ex.ToString());
                    continue;
                }
            }
        }

        public static FactionVaultsList GetAll()
        {
            if (System.IO.File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(FactionVaultsList), new XmlRootAttribute("FactionVaults_List"));
                    currentVaults = (FactionVaultsList)deserializer.Deserialize(reader);
                }
            }
            else
            {
                SaveChanges();
            }

            return currentVaults;
        }

        public static void CreateFactionVault(FactionVault _model)
        {
            _model.VaultId = currentVaults.Items.Count > 0 ? currentVaults.Items.LastOrDefault().VaultId + 1 : 1;
            _model.TextLabelOnMap = API.shared.createTextLabel(_model.Text, _model.Position, 15, 1, false, _model.Dimension);
            currentVaults.Items.Add(_model);
            SaveChanges();
        }
        public static FactionVault GetFactionVault(int id)
        {
            return currentVaults.Items.FirstOrDefault(x => x.VaultId == id);
        }
        public static bool UpdateFactionVault(FactionVault _model)
        {
            try
            {
                _model.TextLabelOnMap.text = _model.Text;
                _model.TextLabelOnMap.position = _model.Position;
                _model.TextLabelOnMap.dimension = _model.Dimension;
                SaveChanges();
                return true;
            }
            catch (Exception ex)
            {
                API.shared.consoleOutput(ex.ToString());
                return false;
            }

        }

        public static bool RemoveFactionVault(int id)
        {
            var deletedItem = currentVaults.Items.FirstOrDefault(x => x.VaultId == id);
            if (deletedItem!=null)
            {
                return currentVaults.Items.Remove(deletedItem);
                
            }
            return false;

        }
        public static bool AddItemToFactionVault(int vaultId,Item _item,int minRankLevel=1)
        {
            var editedItem = GetFactionVault(vaultId);
            if (editedItem!=null)
            {
                editedItem.VaultItems.Add(new VaultItem { GameItemID = _item.ID, MinRankLevel = minRankLevel });
                SaveChanges();
                return true;
            }
            return false;
        }
        public static bool RemoveItemFromFactionVault(int vaultId,int itemId)
        {
            var editedItem = GetFactionVault(vaultId);
            if (editedItem!=null)
            {
                var removedItem = editedItem.VaultItems.FirstOrDefault(x => x.GameItemID == itemId);
                if (removedItem!=null)
                {
                    bool result = editedItem.VaultItems.Remove(removedItem);
                    SaveChanges();
                    return result;
                }

            }
            return false;
        }
        public static bool RemoveItemFromFactionVault(FactionVault _vault, int itemId)
        {
            if (_vault != null)
            {
                var removedItem = _vault.VaultItems.FirstOrDefault(x => x.GameItemID == itemId);
                if (removedItem != null)
                {
                    bool result =_vault.VaultItems.Remove(removedItem);
                    SaveChanges();
                    return result;
                }

            }
            return false;
        }

        public static List<Item> GetFactionVaultItems(int vaultId,int rank)
        {
            var _vault = currentVaults.Items.FirstOrDefault(x => x.VaultId == vaultId);
            if (_vault!=null)
            {
                if (_vault.VaultItems!=null)
                {
                    return  db_Items.GetItemsByIDs(_vault.VaultItems.Where(w=>w.MinRankLevel <= rank).Select(s => s.GameItemID).ToList());
                }
            }
            return new List<Item>();
        }
        public static List<Item> GetFactionVaultItems(FactionVault _vault, int rank)
        {           
                if (_vault.VaultItems != null)
                {
                    return db_Items.GetItemsByIDs(_vault.VaultItems.Where(w => w.MinRankLevel <= rank).Select(s => s.GameItemID).ToList());
                }
            
            return new List<Item>();
        }

        public static void SaveChanges()
        {
            lock (currentVaults)
            {
                if (System.IO.Directory.Exists(dataPath.Split('/')[0]))
                {
                    lock (currentVaults)
                    {
                        XmlTextWriter xWriter = new XmlTextWriter(dataPath, System.Text.UTF8Encoding.UTF8);
                        xWriter.Formatting = Formatting.Indented;
                        xSer.Serialize(xWriter, currentVaults);
                        xWriter.Dispose();
                    }
                }
                else
                {
                    System.IO.Directory.CreateDirectory(dataPath.Split('/')[0]);
                } 
            }
        }
    }
}
