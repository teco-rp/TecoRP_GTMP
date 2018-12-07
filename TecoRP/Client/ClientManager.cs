using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using TecoRP.Database;
using TecoRP.Models;
using TecoRP.Users;
using static TecoRP.Managers.TradeManager;

namespace TecoRP.Clients
{
    public class ClientManager : Script
    {
        public ClientManager()
        {
            API.onClientEventTrigger += API_onClientEventTrigger;
        }

        private void API_onClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {

            switch (eventName)
            {
                case "storage_slot_selected":
                    #region storage
                    switch (arguments[0].ToString())
                    {
                        case "vehicleB":
                            Managers.VehicleManager.OnVehicleBaggageItemSelected(sender, Convert.ToInt32(arguments[1]), Convert.ToInt32(arguments[2]));

                            break;
                        case "vehicleT":
                            Managers.VehicleManager.OnVehicleTorpedoItemSelected(sender, Convert.ToInt32(arguments[1]), Convert.ToInt32(arguments[2]));
                            break;
                        case "house":
                            break;
                        default:
                            break;
                    }
                    #endregion
                    break;
                case "inventory_item_selection_selected":
                    #region itemSelected
                    switch (arguments[0].ToString())
                    {
                        case "vehicleB":
                            Managers.VehicleManager.PutVehicleBaggageItemByPlayer(sender, Convert.ToInt32(arguments[1]), Convert.ToInt32(arguments[2]));
                            break;
                        case "vehicleT":
                            API.shared.consoleOutput("triggered vehicleT");
                            Managers.VehicleManager.PutVehicleTorpedoItemByPlayer(sender, Convert.ToInt32(arguments[1]), Convert.ToInt32(arguments[2]));
                            break;
                        case "house":
                            break;
                        default:
                            break;
                    }
                    #endregion
                    break;
                case "anim_stop":
                    #region AnimStop
                    Animation.AnimationStop(sender);
                    //if ((API.hasEntityData(sender, "Cuffed") && (bool)API.getEntityData(sender, "Cuffed")) || API.getEntityData(sender, "Dead"))
                    //    return;
                    //else
                    //{
                    //    API.stopPlayerAnimation(sender);
                    //    Animation.CloseMobilePhone(sender);
                    //}
                    #endregion
                    break;
                case "lspd_guilty_list_request":
                    Managers.CrimeManager.OnGuiltyListSelected(sender);
                    break;
                case "lspd_fingerprint_request":
                    Managers.CrimeManager.OnFingerPrintScanned(sender, arguments[0].ToString());
                    break;
                case "lspd_crime_add_request":
                    Managers.CrimeManager.OnAddCrimeToPlayer(sender, arguments[0].ToString());
                    break;
                case "lspd_computer_return":
                    #region LSPD_Return
                    switch (arguments[0].ToString())
                    {

                        case "normal":
                            // SUÇLU ADINA TIKLAYINCA
                            break;
                        case "crimelist":
                            Managers.CrimeManager.OnCompleteAddCrimeToPlayer(sender, Convert.ToInt32(arguments[1]), arguments[2].ToString());
                            break;
                        case "remove":
                            Managers.CrimeManager.OnCompleteAddCrimeToPlayer(sender, Convert.ToInt32(arguments[1]), arguments[2].ToString());
                            break;
                        default:
                            break;
                    }
                    #endregion
                    break;
                case "retun_vault":
                    Managers.FactionManager.OnFactionVaultItemSelected(sender, Convert.ToInt32(arguments[0]));
                    break;
                case "return_building":
                    Managers.BuildingManager.OnBuildingSelected(sender, Convert.ToInt32(arguments[0]), Convert.ToInt32(arguments[1]));
                    break;
                case "return_building_buy":
                    Managers.BuildingManager.OnBuildingBuySelected(sender, Convert.ToInt32(arguments[0]), Convert.ToInt32(arguments[1]));
                    break;
                case "return_building_sell":
                    Managers.BuildingManager.OnBuildingSellSelected(sender, Convert.ToInt32(arguments[0]), Convert.ToInt32(arguments[1]), Convert.ToInt32(arguments[2].ToString().Replace("$", String.Empty)));
                    break;
                case "return_building_lock":
                    Managers.BuildingManager.OnBuildingLockSelected(sender, Convert.ToInt32(arguments[0]), Convert.ToInt32(arguments[1]));
                    break;
                case "return_building_ring":
                    Managers.BuildingManager.onBuildingRingSelected(sender, Convert.ToInt32(arguments[0]), Convert.ToInt32(arguments[1]));
                    break;
                case "request_sell_item":
                    Managers.InventoryManager.OnSellItemPlayerList(sender, Convert.ToInt32(arguments[0]));
                    break;
                case "return_sell_item_player_selection":
                    Managers.InventoryManager.OnSellItemSelected(sender, Convert.ToInt32(arguments[1]), Convert.ToInt32(arguments[0]), Convert.ToInt32(arguments[2].ToString().Replace("$", String.Empty)));
                    break;
                case "return_crafting":
                    Managers.CraftingManager.OnCraftingElementSelected(sender, Convert.ToInt32(arguments[0]), Convert.ToInt32(arguments[1]));
                    break;
                case "crafting_open_key":
                    Managers.CraftingManager.OnCraftingRequested(sender);
                    break;
                case "return_sell_player_select":
                    Managers.TradeManager.PlayerSelectedSell(sender, (SellType)Enum.Parse(typeof(SellType), arguments[0].ToString()), Convert.ToInt32(arguments[1]), Convert.ToInt32(arguments[2]), Convert.ToInt32(arguments[3].ToString().Replace("$", string.Empty)));
                    break;
                case "turn_off_engine":
                    Managers.VehicleManager.TurnOffEngine((GrandTheftMultiplayer.Server.Elements.Vehicle)arguments[0]);
                    break;
                case "delete_from_inventory":
                    Admin.AdminCommands.RemoveItemFromInventory(sender, arguments[0].ToString(), Convert.ToInt32(arguments[1]));
                    break;
                case "return_invite_broadcast":
                    WzNewsCommands.CompleteInvite(sender, Convert.ToInt32(arguments[2]));
                    break;
                case "return_kick_broadcast":
                    WzNewsCommands.KickPlayerFromBroadCast(sender, Convert.ToInt32(arguments[2]));
                    break;
                case "return_phone_advertisement":
                    Managers.AdvertisementManager.CompleteAdvertisement(sender, arguments[0].ToString());
                    break;
                case "return_advertisement_text":
                    Managers.AdvertisementManager.CompleteAdvertisement(sender, arguments[0].ToString(), false);
                    break;
                case "return_house_seleceted":
                    Managers.HouseManager.OnSelectedHouseInMenu(sender, Convert.ToInt32(arguments[0]));
                    break;
            }

        }


        public static void ShowMissionMarker(Client sender, float x, float y, float z, int missionNumber)
        {
            int markerType = 1;
            string missionText = "Görev Yok";
            switch (missionNumber)
            {
                case 0:
                    markerType = 498;
                    missionText = "Kimlik kartı çıkart.";
                    break;
                case 1:
                    markerType = 73;
                    missionText = "Kendinize bir kıyafet alın.";
                    break;
                case 2:
                    markerType = 521;
                    missionText = "Kendinize bir telefon alın.";
                    break;
                case 3:
                    markerType = 514;
                    missionText = "Telefonunuza hat alın.";
                    break;
                default:
                    markerType = 304;
                    break;
            }
            API.shared.triggerClientEvent(sender, "mission_marker_show", x, y, z, markerType, missionText);
        }
        public static void RemoveMissionMarker(Client sender)
        {
            API.shared.triggerClientEvent(sender, "mission_marker_hide");
        }

        public static void UpdateWaypoint(Client sender, Vector3 position)
        {
            API.shared.triggerClientEvent(sender, "update_waypoint", position.X, position.Y);
        }
        public static void ShowBlip(Client sender, float x, float y, float z)
        {
            API.shared.triggerClientEvent(sender, "create_marker", x, y, z);
        }
        public static void RemoveBlip(Client sender)
        {
            API.shared.triggerClientEvent(sender, "remove_marker");
        }

        public static void UpdateMoneyDisplay(Client sender, int money)
        {
            API.shared.triggerClientEvent(sender, "update_money_display", money);
        }

        public static void ShowStorageMenuToPlayer(Client sender, List<string> names, List<string> descs, string title, string type, int _Id)
        {
            //API.shared.consoleOutput("converted vehicleid: " + _Id);

            if (names.Count == descs.Count)
            {
                API.shared.triggerClientEvent(sender, "storage_open", names.Count, names.ToArray(), descs.ToArray(), title, type, _Id);
            }
            else
            {
                API.shared.consoleOutput("Storage açılırken isim listesi ile descripiton listesi aynı uzunlukta değildi.");
            }
        }

        public static void UpdateHunger(Client sender, float value)
        {
            var _thirsty = API.shared.getEntityData(sender, "Thirsty");
            API.shared.triggerClientEvent(sender, "update_hungerthirsty", value, _thirsty);
        }

        public static void UpdateThirsty(Client sender, float value)
        {
            var _hunger = API.shared.getEntityData(sender, "Hunger");
            API.shared.triggerClientEvent(sender, "update_hungerthirsty", _hunger, value);
        }
        public static void UpdateHungerAndThirsty(Client sender, float hunger, float thirsty)
        {
            API.shared.triggerClientEvent(sender, "update_hungerthirsty", hunger, thirsty);
        }

        public static void ShowInventoryForSelection(Client sender, string selectionType, int objectId)
        {

            //API.shared.consoleOutput("Converted Vehicle ID: " + objectId);
            Inventory _inventory = API.shared.getEntityData(sender, "inventory");

            List<string> descList = new List<string>();
            List<string> nameList = new List<string>();
            //Dead Item Registry
            int _Index = 0; List<int> indexes = new List<int>();
            foreach (var item in _inventory.ItemList)
            {
                var _gameItem = db_Items.GetItemById(item.ItemId);
                if (_gameItem == null) { indexes.Add(_Index); continue; }
                nameList.Add((item.Equipped ? "*" : String.Empty) + _gameItem.Name + " (" + item.Count + ")");
                descList.Add(_gameItem.Description);
                _Index++;
            }
            string desc = "Eşyalarım  |  " + _inventory.ItemList.Count + " / " + _inventory.InventoryMaxCapacity;
            API.shared.triggerClientEvent(sender, "inventory_open_selection", nameList.Count(), nameList.ToArray(), descList.ToArray(), desc, selectionType, objectId);

            #region ForDeadRegistry
            foreach (var item in indexes)
            {
                _inventory.ItemList.RemoveAt(item);
            }
            API.shared.setEntityData(sender, "inventory", _inventory);
            #endregion
        }

        public static void ShowGuiltyList(Client sender, List<string> names, List<string> descs)
        {
            if (names.Count == descs.Count)
            {
                API.shared.triggerClientEvent(sender, "lspd_guilty_list_open", names.Count, names.ToArray(), descs.ToArray(), "normal", null);
            }
        }
        public static void ShowLSPDComputer(Client sender)
        {
            API.shared.triggerClientEvent(sender, "lspd_main_menu");
        }
        public static void SendCrimeList(Client sender, List<string> names, List<string> descs, string socialClubName)
        {
            if (names.Count == descs.Count)
            {
                API.shared.triggerClientEvent(sender, "lspd_guilty_list_open", names.Count, names.ToArray(), descs.ToArray(), "crimelist", socialClubName);
            }
        }
        public static void SendCrimeListForRemove(Client sender, List<string> names, List<string> descs, string socialClubName = null)
        {
            if (names == descs)
            {
                API.shared.triggerClientEvent(sender, "lspd_guilty_list_open", names.Count, names.ToArray(), descs.ToArray(), "remove", socialClubName);
            }
        }
        public static void OpenVault(Client sender, List<string> names, List<string> descs, List<int> IDlist)
        {
            if (names.Count == descs.Count)
            {
                API.shared.triggerClientEvent(sender, "vault_open", names.Count, names.ToArray(), descs.ToArray(), IDlist);
            }
        }
        public static void DisplaySubtitle(Client sender, string text, int duration)
        {
            API.shared.triggerClientEvent(sender, "display_subtitle", text, duration);
        }
        public static void OpenBuilding(Client sender, List<string> names, List<string> descs, int buildingId, string buildingName = "Bina")
        {
            if (names.Count == descs.Count)
            {
                API.shared.triggerClientEvent(sender, "building_open", names.Count, names.ToArray(), descs.ToArray(), buildingId, buildingName);
            }
        }

        public static void ChoosePlayerToSellItem(Client sender, List<string> names, List<int> ids, int index)
        {
            API.shared.triggerClientEvent(sender, "sell_item_player_selection", names.Count, names.ToArray(), ids.ToArray(), index);
        }
        public static void OpenCraftingMenuToPlayer(Client sender, List<string> names, List<string> descs, int craftingTableModel)
        {
            if (names.Count == descs.Count)
            {
                API.shared.triggerClientEvent(sender, "crafting_open", names.Count, names.ToArray(), descs.ToArray(), craftingTableModel);

            }
        }

        public static void StartAudio(Client player, string localPath)
        {
            API.shared.triggerClientEvent(player, "start_audio", localPath);
        }

        public static void SellSelectorMenu(Client player, List<string> names, List<int> IDs, SellType _type, int sellingObjId)
        {
            API.shared.triggerClientEvent(player, "sell_player_select", names.Count, names.ToArray(), IDs.ToArray(), _type.ToString(), sellingObjId, "return_sell_player_select");
        }

        public static void InviteBroadcastSelectorMenu(Client player, List<string> names, List<int> IDs)
        {
            API.shared.triggerClientEvent(player, "sell_player_select", names.Count, names.ToArray(), IDs.ToArray(), " ", " ", "return_invite_broadcast");
        }
        public static void KickBroadcastSelectorMenu(Client player, List<string> names, List<int> IDs)
        {
            API.shared.triggerClientEvent(player, "sell_player_select", names.Count, names.ToArray(), IDs.ToArray(), " ", " ", "return_kick_broadcast");
        }
        public static void GetAdvertisementText(Client player)
        {
            GetUserInput(player, "return_advertisement_text");
        }
        public static void GetUserInput(Client player, string returnName)
        {
            API.shared.triggerClientEvent(player, "get_input", returnName);
        }
        public static void ShowPlayerMenu(Client player, List<string> names, List<string> descs)
        {
            if (names.Count == descs.Count)
            {
                API.shared.triggerClientEvent(player, "open_skills", names.Count, names, descs);
            }
        }
        public static void ShowHousesToPlayer(Client player,List<string> names,List<int> IDs)
        {
            ShowCustomMenu(player, names, new List<string>(), IDs, "EVLER", "Sahip olduğunuz evler:", "return_house_seleceted");
        }
        public static void ShowMods(Client player,List<string> names, List<int> ItemIDs, Models.Vehicle vehicle)
        {
            ShowCustomMenu(player, names, new List<string>(), ItemIDs,vehicle.VehicleId,vehicle.VehicleModelId.ToString(), "Araçtaki modifiyeler:", "return_mod_selected");
        }
        public static void ShowCustomMenu(Client sender, List<string> names, List<string> descs, List<int> IDs,object customArgs, string title = "MENU", string subTitle = "", string returnName = "")
        {
            API.shared.triggerClientEvent(sender, "open_custom_menu", names.Count, names.ToArray(), descs.ToArray(), IDs.ToArray(),customArgs,returnName,title,subTitle);
        }

        public static void OpenOperatorMenu(Client player,string operatorName, string pricingSentence)
        {
            API.shared.triggerClientEvent(player, "open_operator_menu", operatorName, pricingSentence);
        }
    }
}
