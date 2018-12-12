using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
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
using System.Web;
using Newtonsoft.Json;
using TecoRP.Helpers;

namespace TecoRP.Database
{
    public class db_Accounts
    {
        public const string ACCOUNT_FOLDER = "cnc_accounts";
        public const string INVENTORY_FOLDER = "Data/inventories";

        public db_Accounts()
        {
            //Init();
        }
        public static void Init()
        {
            if (!Directory.Exists(ACCOUNT_FOLDER))
                Directory.CreateDirectory(ACCOUNT_FOLDER);
            if (!Directory.Exists(INVENTORY_FOLDER.Split('/')[0]))
                Directory.CreateDirectory(INVENTORY_FOLDER.Split('/')[0]);
            if (!Directory.Exists(INVENTORY_FOLDER.Split('/')[1]))
                Directory.CreateDirectory(INVENTORY_FOLDER.Split('/')[1]);
            API.shared.consoleOutput("Database initialized!");
        }

        public static bool DoesAccountExist(string name)
        {
            var path = Path.Combine(ACCOUNT_FOLDER, name);
            return File.Exists(path);
        }

        public static bool IsPlayerLoggedIn(Client player)
        {
            return API.shared.getEntityData(player, "LOGGED_IN") == true;
        }

        public static void CreatePlayerAccount(Client player, string password)
        {
            var path = Path.Combine(ACCOUNT_FOLDER, player.socialClubName);
            var pathInv = Path.Combine(INVENTORY_FOLDER, player.socialClubName);
            //if (!path.StartsWith(Directory.GetCurrentDirectory())) return;

            var data = new Models.User(player.socialClubName);

            var dataInv = new Inventory
            {
                OwnerSocialClubName = player.socialClubName,
                ItemList = new List<ClientItem> { new ClientItem { Count = 2, ItemId = 2 }, new ClientItem { ItemId = 5, Count = 1 } }
            };

            var ser = API.shared.toJson(data);
            var serInv = API.shared.toJson(dataInv);

            File.WriteAllText(path, ser);
            File.WriteAllText(pathInv, ser);
        }

        public static bool TryLoginPlayer(Client player, string password)
        {
            var path = Path.Combine(ACCOUNT_FOLDER, player.socialClubName);

            //if (!path.StartsWith(Directory.GetCurrentDirectory())) return false;

            var txt = File.ReadAllText(path);

            User playerObj = API.shared.fromJson(txt).ToObject<User>();

            return API.shared.getHashSHA256(password) == playerObj.Password;
        }

        public static void LoadPlayerAccount(Client player)
        {

            //if (!path.StartsWith(Directory.GetCurrentDirectory())) return;
            try
            {
                LoadPlayerData(player);
            }
            catch (Exception ex)
            {
                API.shared.consoleOutput(LogCat.Fatal, $"LoadPlayerData | {player.socialClubName} | {API.shared.getEntityData(player,"ID")} | {ex.ToString()}");
           
            }

            try
            {
                LoadPlayerInventory(player);
            }
            catch (Exception ex)
            {
                API.shared.consoleOutput(LogCat.Fatal, $"LoadPlayerInventory | {player.socialClubName} | {API.shared.getEntityData(player,"ID")} | {ex.ToString()}");

            }
            player.SetLoggedIn(true);
            //API.shared.setEntityData(player, "LOGGED_IN", true);
        }
        private static void LoadPlayerData(Client player)
        {
            var path = Path.Combine(ACCOUNT_FOLDER, player.socialClubName);
            var txt = File.ReadAllText(path);
            Models.User playerObj = API.shared.fromJson(txt).ToObject<Models.User>();
            foreach (var property in typeof(Models.User).GetProperties())
            {
                if (property.GetCustomAttributes(typeof(XmlIgnoreAttribute), false).Length > 0) continue;

                API.shared.setEntityData(player, property.Name, property.GetValue(playerObj, null));
            }
        }
        private static void LoadPlayerInventory(Client player)
        {
            
                var pathInv = Path.Combine(INVENTORY_FOLDER, player.socialClubName);
                string txtInv = "";
                if (!System.IO.File.Exists(pathInv))
                {
                    txtInv = API.shared.toJson(new Inventory { OwnerSocialClubName = player.socialClubName });
                    File.WriteAllText(pathInv, txtInv);

                }
                else
                    txtInv = File.ReadAllText(pathInv);


                Inventory inventoryObj = API.shared.fromJson(txtInv).ToObject<Inventory>();
                API.shared.setEntityData(player, "inventory", inventoryObj);
            
        }
        public static string GetPlayerCharacterName(Client sender)
        {
            return API.shared.getEntityData(sender, "CharacterName");
        }
        public static User GetOfflineUserDatas(string socialClubName)
        {
            if (String.IsNullOrEmpty(socialClubName)) { return null; }
            var path = Path.Combine(ACCOUNT_FOLDER, socialClubName);
            if (!File.Exists(path))
            {
                return null;
            }
            else
            {
                var txt = File.ReadAllText(path);
                Models.User playerObj = API.shared.fromJson(txt).ToObject<Models.User>();
                return playerObj;
            }
        }
        public static User GetOfflineUserDatas(string socialClubName, bool serverMapPath = false)
        {
            if (String.IsNullOrEmpty(socialClubName)) { return null; }
            //string path = "";
            //if (serverMapPath)
            //{
            //var path = Path.Combine(ACCOUNT_FOLDER, socialClubName);



            //    path = HttpContext.Current.Server.MapPath("~/"+combined);
            //}

            var path = Path.Combine(ACCOUNT_FOLDER, socialClubName);

            if (!File.Exists(path))
            {
                return null;
            }
            else
            {
                var txt = File.ReadAllText(path);
                Models.User playerObj = JsonConvert.DeserializeObject<Models.User>(txt);
                return playerObj;
            }
        }
        public static Inventory GetOfflineUserInventory(string socialClubName)
        {
            if (String.IsNullOrEmpty(socialClubName)) { return null; }
            var path = Path.Combine(INVENTORY_FOLDER, socialClubName);
            if (!File.Exists(path))
            {
                return null;
            }
            else
            {
                var txt = File.ReadAllText(path);
                Models.Inventory playerObj = API.shared.fromJson(txt).ToObject<Models.Inventory>();
                return playerObj;
            }
        }
        public static List<User> GetOfflineUserDatas()
        {
            var retunModel = new List<Models.User>();
            var path = Path.Combine(ACCOUNT_FOLDER);

            foreach (var item in Directory.GetFiles(path))
            {
                var txt = File.ReadAllText(item);
                retunModel.Add(API.shared.fromJson(txt).ToObject<Models.User>());
            }

            return retunModel;
        }

        public static User FindFingerPrint(string fingerPrint)
        {

            var path = Path.Combine(ACCOUNT_FOLDER);

            foreach (var item in Directory.GetFiles(path))
            {
                var txt = File.ReadAllText(item);

                var _user = (Models.User)API.shared.fromJson(txt).ToObject<Models.User>();
                if (_user.FingerPrint == fingerPrint)
                {
                    return _user;
                }

            }
            return null;
        }
        public static bool SaveOfflineUserData(string socialClubName, User playerObj)
        {
            var path = Path.Combine(ACCOUNT_FOLDER, socialClubName);
            if (!File.Exists(path)) return false;
            try
            {
                var ser = API.shared.toJson(playerObj);
                File.WriteAllText(path, ser);
                return true;
            }
            catch (Exception)
            {
                return false;
            }
        }

        public static void SavePlayerAccount(Client player)
        {
            //if (!API.shared.hasEntityData(player, "FINISHED_DOWNLOAD"))
            //    return;
            if (!IsPlayerLoggedIn(player))
            {
                API.shared.consoleOutput(LogCat.Warn,$"SavePlayerAccount | Player not logged in : {player.socialClubName} | {API.shared.getEntityData(player,"ID")}");
                return;
            }
            lock (player)
            {
                var path = Path.Combine(ACCOUNT_FOLDER, player.socialClubName);
                var pathInv = Path.Combine(INVENTORY_FOLDER, player.socialClubName);
                try
                {
                    //  var old = API.shared.fromJson(File.ReadAllText(path));
                    var data = GetOfflineUserDatas(player.socialClubName) ?? new User(player.socialClubName);
                    
                    foreach (var property in typeof(Models.User).GetProperties())
                    {
                        if (property.GetCustomAttributes(typeof(XmlIgnoreAttribute), false).Length > 0) continue;

                        if (API.shared.hasEntityData(player, property.Name))
                        {
                            try
                            {
                                if (!API.shared.hasEntityData(player, property.Name)) { API.shared.consoleOutput(LogCat.Debug,$"{GetPlayerCharacterName(player)} | Save Account Error. Oyuncu Entity Data'ya sahip değil: {property.Name} | Son kayıt uygulandı. {property.Name} : {property.GetValue(data)}"); continue; }
                                property.SetValue(data, API.shared.getEntityData(player, property.Name), null);
                            }
                            catch (Exception ex)
                            {
                                API.shared.consoleOutput(LogCat.Fatal, ex.ToString());

                                if (ex.GetType() == typeof(NullReferenceException))
                                {
                                    API.shared.consoleOutput(LogCat.Warn,$"PlayerSaveAccountError:  Client {property.Name} adlı entity data'ya sahip değil!") ;
                                    continue;
                                }
                            }
                        }
                    }
                    var ser = API.shared.toJson(data);

                    File.WriteAllText(path, ser);

                    var _currentInventory = Managers.InventoryManager.GetPlayerInventory(player);
                    if (_currentInventory == null)
                    {
                        API.shared.consoleOutput(LogCat.Fatal, $"InventorySaveError! Player: {player.socialClubName} | NullReferenceException");
                        _currentInventory = new Inventory();
                    }
                    _currentInventory.OwnerSocialClubName = player.socialClubName;
                    var serInv = API.shared.toJson(_currentInventory);
                    File.WriteAllText(pathInv, serInv);
                }
                catch (Exception ex)
                {
                    API.shared.consoleOutput(ex.ToString());
                }
            }
        }
        public async Task SavePlayerAccountAsync(Client player)
        {
            await Task.Run(()=> 
            {
                SavePlayerAccount(player);
            });
        }
        public static Client GetPlayerById(int _Id)
        {
            try
            {
                foreach (var item in API.shared.getAllPlayers())
                {
                    if (_Id == API.shared.getEntityData(item, "ID"))
                    {
                        return item;
                    }
                }
                return null;
            }
            catch (Exception ex)
            {
                API.shared.consoleOutput(LogCat.Fatal, ex.ToString());
                return null;
            }
        }

        public static bool IsPlayerOnline(int _Id)
        {
            try
            {
                foreach (var item in API.shared.getAllPlayers())
                {
                    if (API.shared.getEntityData(item, "ID") == _Id)
                    {
                        return true;
                    }
                }
                return false;
            }
            catch (Exception ex)
            {
                API.shared.consoleOutput(LogCat.Fatal, ex.ToString());
                return false;
            }
        }
        public static Client FindPlayerById(int _Id)
        {
            foreach (var item in API.shared.getAllPlayers())
            {
                if (API.shared.getEntityData(item, "ID") == _Id)
                {
                    return item;
                }
            }
            return null;
        }
        public static Client IsPlayerOnline(string socialClubName)
        {
            foreach (var item in API.shared.getAllPlayers())
            {
                if (item.socialClubName == socialClubName)
                {
                    return item;
                }
            }
            return null;
        }

    }
}
