using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared.Math;
using TecoRP.Database;

namespace TecoRP.Managers
{
    public class CraftingManager : Script
    {
        [Command("uretim")]
        public static void OnCraftingRequested(Client sender)
        {
            var _table = db_Craftings.GetNearestCraftingTableOnMap(sender.position);
            if (_table != null)
            {
                if (Vector3.Distance(sender.position, _table.Position) < 3)
                {
                    List<string> names = new List<string>();
                    List<string> descs = new List<string>();

                    var _tableModel = db_Craftings.GetCraftingTableModel(_table.CraftingTableModelId);
                    if (_table != null)
                    {
                        foreach (var item in _tableModel.Craftings)
                        {
                            names.Add(db_Items.GetItemById(item.CraftedGameItemId).Name);
                            string desc = "Gerekli Malzemeler: ";
                            #region MetalPartControl
                            if (item.RequiredMetalPart > 0)
                            {
                                if (item.RequiredMetalPart <= InventoryManager.GetPlayerMetalParts(sender))
                                    desc += "~g~Metal Parça: " + item.RequiredMetalPart + " ~s~| ";
                                else
                                    desc += "~r~Metal Parça: " + item.RequiredMetalPart + " ~s~| ";
                            }
                            #endregion
                            foreach (var itemRequired in item.RequredItemIds)
                            {
                                if (InventoryManager.DoesPlayerHasItemById(sender, itemRequired))
                                {
                                    desc += "~g~" + db_Items.GetItemById(itemRequired).Name + "~s~ | ";
                                }
                                else
                                {
                                    desc += "~r~" + db_Items.GetItemById(itemRequired).Name + "~s~ | ";
                                }
                            }
                            descs.Add(desc);
                        }

                        Clients.ClientManager.OpenCraftingMenuToPlayer(sender, names, descs, _table.CraftingTableModelId);
                    }
                    else
                    {
                        API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bu üretim masasının konfigürasyonu yanlış. ~y~/rapor~s~ ile bilririniz.");
                    }

                }
                else
                {
                    API.shared.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Etrafınızda bir üretim objesi bulunmuyor.");
                }
            }
            else
            {
                API.shared.consoleOutput(LogCat.Warn, "Oyunda Üretim Makinesi Yok (CraftingTable)");
            }
        }

        public static void OnCraftingElementSelected(Client sender, int craftingModelID, int index)
        {
            if (!InventoryManager.DoesInventoryHasSpace(sender)) { API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Envanterinizde yeterli alan yok."); return; }
            try
            {
                var _tableModel = db_Craftings.GetCraftingTableModel(craftingModelID);
                var crafting = _tableModel.Craftings[index];
                API.shared.consoleOutput(crafting.ToString() + "  " + db_Items.GetItemById(crafting.CraftedGameItemId).Name);
                if (crafting.RequiredMetalPart > InventoryManager.GetPlayerMetalParts(sender)) { API.shared.sendChatMessageToPlayer(sender,"~r~UYARI: ~s~Bu üretim için yeterli metal parçanız bulunmuyor."); return; }
                    foreach (var item in crafting.RequredItemIds)
                    {
                        API.shared.consoleOutput(InventoryManager.DoesPlayerHasItemById(sender, item).ToString());
                        if (!InventoryManager.DoesPlayerHasItemById(sender, item))
                        {
                            API.shared.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Bu eşyayı üretmek için üzerinizde yeterli eşya bulunmuyor.");
                            return;
                        }
                    }

                foreach (var item in crafting.RequredItemIds)
                {
                    InventoryManager.RemoveItemFromPlayerInventory(sender, item);
                }
                InventoryManager.AddMetalPartsToPlayer(sender, -1 * crafting.RequiredMetalPart);
                InventoryManager.AddItemToPlayerInventory(sender, new Models.ClientItem { Count = 1, Equipped = false, ItemId = crafting.CraftedGameItemId });
                API.shared.sendChatMessageToPlayer(sender,"~g~BAŞARILI: ~s~Üretimi başarıyla tamamladınız.");
                JobManager.PlayerJobComplete(sender, crafting.RequiredJob);
            }
            catch (Exception ex)
            {
                if (ex.GetType() == typeof(ArgumentOutOfRangeException) || ex.GetType() == typeof(IndexOutOfRangeException))
                {
                    API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Index hatası. ~y~/rapor ~s~ile bildiriniz.");
                }
                else
                {
                    API.shared.consoleOutput(LogCat.Fatal, ex.ToString());
                }
            }
        }
    }
}
