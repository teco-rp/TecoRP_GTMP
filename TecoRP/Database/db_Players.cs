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
    public class db_Players
    {
        public const string PLAYERS_FOLDER = "Characters";
        public const string INVENTORY_FOLDER = "Data/inventories";

        public db_Players()
        {
            //Init();
        }
        public static void Init()
        {
            if (!Directory.Exists(PLAYERS_FOLDER))
                Directory.CreateDirectory(PLAYERS_FOLDER);
            if (!Directory.Exists(INVENTORY_FOLDER.Split('/')[0]))
                Directory.CreateDirectory(INVENTORY_FOLDER.Split('/')[0]);
            if (!Directory.Exists(INVENTORY_FOLDER.Split('/')[1]))
                Directory.CreateDirectory(INVENTORY_FOLDER.Split('/')[1]);
            API.shared.consoleOutput("Database initialized!");
        }

        public static IEnumerable<Player> GetCharacters(Account account)
        {
            foreach (var item in account.Characters)
            {
                var path = Path.Combine(PLAYERS_FOLDER, item);
                if (File.Exists(path))
                    yield return JsonConvert.DeserializeObject<Player>(File.ReadAllText(path));
                else
                    account.Characters.Remove(item);
            }
        }

        public static Player CreatePlayerCharacter(Client client, Managers.PlayerCustomization customizationData, string characterName)
        {
            var _player = new Player(characterName);
            _player.CustomizationData = customizationData;
            API.shared.consoleOutput("Gender value is " + customizationData.Gender);
            _player.Gender = Convert.ToBoolean(customizationData.Gender);

            foreach (var property in _player.GetType().GetProperties())
            {
                client.setData(property.Name, property.GetValue(_player));
            }

            SavePlayerAccount(client, forceSave: true);
            return _player;
        }
        public static bool DoesPlayerExist(string name)
        {
            var path = Path.Combine(PLAYERS_FOLDER, name);
            return File.Exists(path);
        }

        public static bool IsPlayerLoggedIn(Client player)
        {
            return API.shared.getEntityData(player, "LOGGED_IN") == true;
        }

        public static void LoadPlayerData(Client player)
        {
            var path = Path.Combine(PLAYERS_FOLDER, player.socialClubName);
            var txt = File.ReadAllText(path);
            Models.Player playerObj = API.shared.fromJson(txt).ToObject<Models.Player>();
            foreach (var property in typeof(Models.Player).GetProperties())
            {
                if (property.GetCustomAttributes(typeof(XmlIgnoreAttribute), false).Length > 0) continue;

                API.shared.setEntityData(player, property.Name, property.GetValue(playerObj, null));
            }
        }
        public static void LoadPlayerInventory(Client player)
        {

            var pathInv = Path.Combine(INVENTORY_FOLDER, player.socialClubName);
            string txtInv = "";
            if (!System.IO.File.Exists(pathInv))
            {
                txtInv = API.shared.toJson(new Inventory { OwnerCharacterId = player.socialClubName });
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
        public static Player GetOfflineUserDatas(string playerId)
        {
            if (String.IsNullOrEmpty(playerId)) { return null; }
            var path = Path.Combine(PLAYERS_FOLDER, playerId);
            if (!File.Exists(path))
            {
                return null;
            }
            else
            {
                var txt = File.ReadAllText(path);
                Models.Player playerObj = API.shared.fromJson(txt).ToObject<Models.Player>();
                return playerObj;
            }
        }
        public static Player GetOfflineUserDatas(string socialClubName, bool serverMapPath = false)
        {
            if (String.IsNullOrEmpty(socialClubName)) { return null; }


            var path = Path.Combine(PLAYERS_FOLDER, socialClubName);

            if (!File.Exists(path))
            {
                return null;
            }
            else
            {
                var txt = File.ReadAllText(path);
                Models.Player playerObj = JsonConvert.DeserializeObject<Models.Player>(txt);
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
        public static List<Player> GetOfflineUserDatas()
        {
            var retunModel = new List<Models.Player>();
            var path = Path.Combine(PLAYERS_FOLDER);

            foreach (var item in Directory.GetFiles(path))
            {
                var txt = File.ReadAllText(item);
                retunModel.Add(API.shared.fromJson(txt).ToObject<Models.Player>());
            }

            return retunModel;
        }

        public static Player FindFingerPrint(string fingerPrint)
        {

            var path = Path.Combine(PLAYERS_FOLDER);

            foreach (var item in Directory.GetFiles(path))
            {
                var txt = File.ReadAllText(item);

                var _user = (Models.Player)API.shared.fromJson(txt).ToObject<Models.Player>();
                if (_user.FingerPrint == fingerPrint)
                {
                    return _user;
                }

            }
            return null;
        }
        public static bool SaveOfflineUserData(string socialClubName, Player playerObj)
        {
            var path = Path.Combine(PLAYERS_FOLDER, socialClubName);
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

        public static void SavePlayerAccount(Client player, bool forceSave = false)
        {
            //if (!API.shared.hasEntityData(player, "FINISHED_DOWNLOAD"))
            //    return;
            if (!forceSave && !IsPlayerLoggedIn(player))
            {
                API.shared.consoleOutput(LogCat.Warn, $"SavePlayerAccount | Player not logged in : {player.socialClubName} | {API.shared.getEntityData(player, "ID")}");
                return;
            }
            lock (player)
            {
                var charId = (string)player.getData(nameof(Player.CharacterId));
                var path = Path.Combine(PLAYERS_FOLDER, charId);
                var pathInv = Path.Combine(INVENTORY_FOLDER, charId);
                try
                {
                    //  var old = API.shared.fromJson(File.ReadAllText(path));
                    var data = GetOfflineUserDatas(player.socialClubName) ?? new Player(player.socialClubName);

                    foreach (var property in typeof(Models.Player).GetProperties())
                    {
                        if (property.GetCustomAttributes(typeof(XmlIgnoreAttribute), false).Length > 0) continue;

                        if (API.shared.hasEntityData(player, property.Name))
                        {
                            try
                            {
                                if (!API.shared.hasEntityData(player, property.Name)) { API.shared.consoleOutput(LogCat.Debug, $"{GetPlayerCharacterName(player)} | Save Account Error. Oyuncu Entity Data'ya sahip değil: {property.Name} | Son kayıt uygulandı. {property.Name} : {property.GetValue(data)}"); continue; }
                                property.SetValue(data, API.shared.getEntityData(player, property.Name), null);
                            }
                            catch (Exception ex)
                            {
                                API.shared.consoleOutput(LogCat.Fatal, ex.ToString());

                                if (ex.GetType() == typeof(NullReferenceException))
                                {
                                    API.shared.consoleOutput(LogCat.Warn, $"PlayerSaveAccountError:  Client {property.Name} adlı entity data'ya sahip değil!");
                                    continue;
                                }
                            }
                        }
                    }

                    if (!Directory.Exists(PLAYERS_FOLDER))
                        Directory.CreateDirectory(PLAYERS_FOLDER);

                    var ser = API.shared.toJson(data);
                    File.WriteAllText(path, ser);

                    var _currentInventory = Managers.InventoryManager.GetPlayerInventory(player);
                    if (_currentInventory == null)
                    {
                        if (!forceSave)
                            API.shared.consoleOutput(LogCat.Fatal, $"InventorySaveError! Player: {player.socialClubName} | NullReferenceException");
                        _currentInventory = new Inventory();
                    }

                    if (!Directory.Exists(INVENTORY_FOLDER))
                        Directory.CreateDirectory(INVENTORY_FOLDER);

                    _currentInventory.OwnerCharacterId = player.getData(nameof(Player.CharacterId));
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
            await Task.Run(() =>
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
