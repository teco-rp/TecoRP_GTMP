using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using TecoRP.Database;
using TecoRP.Models;
using TecoRP.Users;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Shared;

namespace TecoRP.Managers
{
    public class InventoryManager : Script
    {

        [Flags]
        public enum AnimationFlags
        {
            Loop = 1 << 0,
            StopOnLastFrame = 1 << 1,
            OnlyAnimateUpperBody = 1 << 4,
            AllowPlayerControl = 1 << 5,
            Cancellable = 1 << 7
        }

        public InventoryManager()
        {
            API.onClientEventTrigger += API_onClientEventTrigger;

            // API.onPlayerWeaponSwitch += API_onPlayerWeaponSwitch;
        }

        private void API_onPlayerWeaponSwitch(Client player, WeaponHash oldValue)
        {
            if (oldValue != WeaponHash.Unarmed && API.shared.getPlayerWeapons(player).Length == 2)
            {
                Task.Run(async () =>
                {
                    await Task.Delay(1000);
                    PutWeaponToInventory(player, oldValue);
                });
            }
        }

        List<string> ConsumableWeapons = new List<string>
        {
            "smokegrenade", "grenade","stickybomb","ball"
        };

        private void API_onClientEventTrigger(Client sender, string eventName, params object[] arguments)
        {
            if (eventName == "key_I")
            {
                OnInventory(sender);
                return;
            }
            if (eventName == "key_E")
            {
                Users.UserCommands uCommands = new Users.UserCommands();
                uCommands.BuyFromShop(sender);
                uCommands = null;
                return;
            }
            if (eventName == "key_B")
            {
                //INCOMING ARGUMENTS [ItemIndex]
                var _itemId = (API.getEntityData(sender, "inventory") as Inventory).ItemList[Convert.ToInt32(arguments[0])].ItemId;
                var choosenItem = db_Items.GetItemById(_itemId);
                GivePlayerList(sender, 5, choosenItem.ID, choosenItem.Name);
                return;
            }
            if (eventName == "key_X")
            {
                if (API.getEntityData(sender, "Dead") == true) return;
                var _inventory = (API.getEntityData(sender, "inventory") as Inventory);
                var _clientItem = _inventory.ItemList[Convert.ToInt32(arguments[0])];
                var gameItem = db_Items.GetItemById(_clientItem.ItemId);
                //var choosenItem = db_Items.GameItems.Items.FirstOrDefault(x => x.ID == _clientItem.ItemId);
                if (gameItem.Droppable)
                {
                    if (db_Items.DropItem(_clientItem, sender))
                    {
                        //if (_inventory.ItemList.Remove(_clientItem))
                        if (RemoveItemFromPlayerInventory(sender, gameItem.ID))
                        {
                            API.playPlayerAnimation(sender, 0, "mp_weapon_drop", "drop_lh");

                        }
                        return;
                    }
                    else
                    {
                        API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Eşya yere atılamıyor.");
                        return;
                    }
                }
                else
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bu eşya yere atılamaz.");
                return;
            }

            if (eventName == "key_N")
            {
                if (API.getEntityData(sender, "Dead") == true) return;
                #region Key_N
                foreach (var itemDropped in db_Items.currentDroppedItems.Items)
                {
                    if (Vector3.Distance(sender.position + new Vector3(0, 0, -0.5), itemDropped.Position) < 1.5f)
                    {
                        if (itemDropped.FactionId > 0 && itemDropped.FactionId != API.getEntityData(sender, "FactionId")) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~" + FactionManager.ToFactionName(itemDropped.FactionId) + " adlı oluşuma ait eşyayı toplayamazsınız."); return; }
                        var _inventory = API.getEntityData(sender, "inventory") as Inventory;
                        if (_inventory.ItemList.Count < _inventory.InventoryMaxCapacity)
                        {
                            var gameItem = db_Items.GetItemById(itemDropped.Item.ItemId);
                            if (gameItem.Type == ItemType.Weapon && _inventory.ItemList.Any(x => db_Items.GetItemById(x.ItemId).Type == ItemType.Weapon && db_Items.GetItemById(x.ItemId).Value_2 == gameItem.Value_2))
                            {
                                API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Envanterinizde bu türden daha fazla silah bulunduramazsınız.");
                                return;
                            }

                            if (db_Items.RemoveDroppedItem(itemDropped))
                            {
                                API.playPlayerAnimation(sender, 0, "anim@mp_snowball", "pickup_snowball");
                                InventoryManager.AddItemToPlayerInventory(sender, itemDropped.Item);
                                //_inventory.ItemList.Add(itemDropped.Item);

                            }
                            return;
                        }
                        else
                        {
                            API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Envanterinizde daha fazla yer yok.");
                            return;
                        }
                    }
                }
                #endregion
                API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Etrafınızda toplayabileceğiniz bir nesne yok.");
                return;
            }
            if (eventName == "iventory_item_selected")
            {
                #region Inventoy Item Selected
                RPGManager rpgMgr = new RPGManager();

                Inventory _inventory = API.getEntityData(sender, "inventory");
                float _thirsty = (float)API.getEntityData(sender, "Thirsty");
                float _hunger = (float)API.getEntityData(sender, "Hunger");
                var itemTuple = GetItemFromPlayerInventory(sender, Convert.ToInt32(arguments[0]));
                var usedItemInInventory = itemTuple.Item2;
                var usedItem = itemTuple.Item1;
                switch (usedItem.Type)
                {
                    case ItemType.None:
                        return;

                    //-----------------------------
                    case ItemType.Drinkable:
                        if (API.getEntityData(sender, "Dead") == true) return;
                        #region DrinkableAction
                        if (_thirsty >= 90)
                        {
                            API.sendNotificationToPlayer(sender, "~y~Henüz susamadınız.", true); return;
                        }
                        Task.Run(async () =>
                        {

                            var drink = API.createObject(usedItem.ObjectId, sender.position, sender.rotation, sender.dimension);
                            API.attachEntityToEntity(drink, sender, "IK_R_Hand", new Vector3(0.015f, -0.07f, -0.06), new Vector3(280, 0, 0));

                            API.playPlayerAnimation(sender, (int)(AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "mini@sprunk", "plyr_buy_drink_pt2");
                            await Task.Delay(2000);
                            API.deleteEntity(drink);
                        });

                        _thirsty += Convert.ToSingle(usedItem.Value_0);
                        sender.health += Convert.ToInt32(usedItem.Value_1);
                        API.triggerClientEvent(sender, "update_hungerthirsty", _hunger, _thirsty);
                        API.setEntityData(sender, "Thirsty", _thirsty);
                        rpgMgr.Me(sender, " bir adet " + usedItem.Name + " çıkarır ve içmeye başlar.");
                        #endregion
                        break;

                    //-----------------------------
                    case ItemType.Eatable:
                        if (API.getEntityData(sender, "Dead") == true) return;
                        #region EatableAction

                        if (_hunger >= 80)
                        {
                            API.sendNotificationToPlayer(sender, "~y~Henüz acıkmadınız.", true); return;
                        }

                        Task.Run(async () =>
                        {
                            var drink = API.createObject(usedItem.ObjectId, sender.position, sender.rotation, sender.dimension);
                            API.attachEntityToEntity(drink, sender, "IK_R_Hand", new Vector3(0.015f, -0.07f, -0.06), new Vector3(280, 0, 0));

                            API.playPlayerAnimation(sender, (int)(AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "amb@code_human_wander_eating_donut@male@idle_a", "idle_c");
                            await Task.Delay(5000);
                            API.deleteEntity(drink);
                            _hunger += Convert.ToSingle(usedItem.Value_0);
                            sender.health += Convert.ToInt32(usedItem.Value_1);
                            API.triggerClientEvent(sender, "update_hungerthirsty", _hunger, _thirsty);
                            API.setEntityData(sender, "Hunger", _hunger);
                        });



                        rpgMgr.Me(sender, " bir adet " + usedItem.Name + " çıkarır ve yemeye başlar.");
                        #endregion
                        break;

                    case ItemType.CraftingPart:
                        if (API.getEntityData(sender, "Dead") == true) return;
                        #region craftingPartAction
                        var craftedItem = db_Items.GetItemById(Convert.ToInt32(usedItem.Value_0));
                        if (craftedItem != null)
                        {
                            RemoveItemFromPlayerInventory(sender, usedItem.ID);
                            for (int i = 0; i < Convert.ToInt32(usedItem.Value_1); i++)
                            {
                                AddItemToPlayerInventory(sender, craftedItem);
                            }
                            break;
                        }
                        #endregion
                        return;
                    //-----------------------------
                    case ItemType.FirstAid:
                        if (API.getEntityData(sender, "Dead") == true) return;
                        #region FirstAid
                        if (sender.health < 90)
                        {
                            int increaseValue = Convert.ToInt32(usedItem.Value_0);
                            sender.health += (increaseValue > 100 ? 100 : increaseValue);
                            rpgMgr.Me(sender, " ilkyardım kiti ile kendine ilkyardım uygular.");
                        }
                        else
                        {
                            API.sendNotificationToPlayer(sender, "Şu anda sağlığınız gayet iyi.");
                        }
                        #endregion
                        break;

                    case ItemType.Weapon:
                        if (API.getEntityData(sender, "Dead") == true) return;
                        #region WeaponAction
                        if (usedItem.Value_0 == "smokegrenade" || usedItem.Value_0 == "grenade") { break; }

                        if (usedItemInInventory.Equipped)
                        {
                            usedItemInInventory.Equipped = false;
                            PutWeaponToInventory(sender, API.shared.weaponNameToModel(usedItem.Value_0));
                            Animation.WearWeapon(sender, usedItem.ObjectId, Convert.ToInt32(usedItem.Value_2));

                            //var specifiedWeaponValue = String.IsNullOrEmpty(usedItemInInventory.SpecifiedValue) ? new Models.SpecifiedValueWeapon { Ammo = Convert.ToInt32(usedItem.Value_1), WeaponTint = WeaponTint.Normal } : API.fromJson(usedItemInInventory.SpecifiedValue).ToObject<SpecifiedValueWeapon>() as SpecifiedValueWeapon;
                            //specifiedWeaponValue.Ammo = API.getPlayerWeaponAmmo(sender, API.weaponNameToModel(usedItem.Value_0));
                            //usedItemInInventory.SpecifiedValue = API.toJson(specifiedWeaponValue);
                            sender.removeWeapon(API.weaponNameToModel(usedItem.Value_0));
                            return;
                        }

                        var guns = db_Items.GameItems.Values.Where(w => w.Type == ItemType.Weapon && _inventory.ItemList.Any(a=>a.ItemId == w.ID));
                        foreach (var item in _inventory.ItemList)
                        {
                            if (guns.Any(a=>a.ID == item.ItemId))
                            {

                                if (item.ItemId == usedItem.ID) { item.Equipped = true; Animation.RemovePlayerWeapon(sender, Convert.ToInt32(usedItem.Value_2)); continue; }
                                var specifiedWeaponValue = String.IsNullOrEmpty(item.SpecifiedValue) ? new SpecifiedValueWeapon { Ammo = 0, WeaponTint = WeaponTint.Normal } : API.fromJson(item.SpecifiedValue).ToObject<SpecifiedValueWeapon>() as SpecifiedValueWeapon;
                                specifiedWeaponValue.Ammo = API.getPlayerWeaponAmmo(sender, API.weaponNameToModel(db_Items.GetItemById(item.ItemId).Value_0));

                                // API.shared.consoleOutput(API.weaponNameToModel(db_Items.GetItemById(item.ItemId).Value_0)+" read ammo : " + specifiedWeaponValue.Ammo);

                                item.SpecifiedValue = API.toJson(specifiedWeaponValue);
                                item.Equipped = false;
                                var _gameitem = db_Items.GetItemById(item.ItemId);
                                Animation.WearWeapon(sender, _gameitem.ObjectId, Convert.ToInt32(_gameitem.Value_2));
                            }
                        }
                        var _WeaponSpecified = String.IsNullOrEmpty(usedItemInInventory.SpecifiedValue) ? new SpecifiedValueWeapon { Ammo = Convert.ToInt32(usedItem.Value_1), WeaponTint = WeaponTint.Normal } : (SpecifiedValueWeapon)API.fromJson(usedItemInInventory.SpecifiedValue).ToObject<SpecifiedValueWeapon>();
                        API.removeAllPlayerWeapons(sender);
                        //OLD
                        API.givePlayerWeapon(sender, API.weaponNameToModel(usedItem.Value_0), (_WeaponSpecified.Ammo < Convert.ToInt32(usedItem.Value_1) ? Convert.ToInt32(usedItem.Value_1) : _WeaponSpecified.Ammo),true, true);
                        API.setPlayerWeaponTint(sender, API.weaponNameToModel(usedItem.Value_0), _WeaponSpecified.WeaponTint);
                        API.setEntityData(sender, "inventory", _inventory);
                        #endregion
                        return;
                    //-----------------------------
                    case ItemType.WeaponPaint:
                        if (API.getEntityData(sender, "Dead") == true) return;
                        #region WeaponPaint
                        try
                        {
                            var _currentWeapon = API.getPlayerCurrentWeapon(sender);
                            var WeaponsInGame = db_Items.GameItems.Values.Where(w => w.Type == ItemType.Weapon).Select(s => s.ID);
                            var equippedWeaponItem = _inventory.ItemList.FirstOrDefault(x => x.Equipped == true && WeaponsInGame.Contains(x.ItemId));
                            if (equippedWeaponItem == null)
                            {
                                API.sendChatMessageToPlayer(sender,"Boyayabilmek için önce silahınızı elinize almalısınız.");
                                return;
                            }
                            API.setPlayerWeaponTint(sender, _currentWeapon, (WeaponTint)Enum.Parse(typeof(WeaponTint), usedItem.Value_0));
                            equippedWeaponItem.SpecifiedValue = API.toJson(new SpecifiedValueWeapon
                            {
                                Ammo = API.getPlayerWeaponAmmo(sender, _currentWeapon),
                                WeaponTint = (WeaponTint)Enum.Parse(typeof(WeaponTint), usedItem.Value_0)
                            });
                        }
                        catch (Exception ex)
                        {
                            API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bir hata oluştu.");
                            API.shared.consoleOutput(LogCat.Warn, ex.ToString());
                            return;
                        }
                        #endregion

                        break;
                    //-----------------------------
                    case ItemType.Armor:
                        if (API.getEntityData(sender, "Dead") == true) return;
                        #region ArmorAction
                        if (sender.armor >= Convert.ToInt32(usedItem.Value_0))
                        {
                            API.sendChatMessageToPlayer(sender, "~y~Zaten daha üst düzey bir korumadasınız.");
                        }
                        else
                        {
                            sender.armor = Convert.ToInt32(usedItem.Value_0);
                            rpgMgr.Me(sender, " kafasından geçirerek " + usedItem.Name + " kuşanır.");
                        }
                        #endregion
                        break;

                    //-----------------------------
                    case ItemType.Drug:
                        if (API.getEntityData(sender, "Dead") == true) return;

                        return;
                    //-----------------------------
                    case ItemType.Ammo:
                        if (API.getEntityData(sender, "Dead") == true) return;
                        #region AmmoAction

                        foreach (var itemWeapon in API.getPlayerWeapons(sender))
                        {
                            if (itemWeapon.ToString().ToLower() == usedItem.Value_0.ToLower())
                            {
                                if (API.getPlayerWeaponAmmo(sender, itemWeapon) < Convert.ToInt32(usedItem.Value_1))
                                {
                                    API.setPlayerWeaponAmmo(sender, itemWeapon, Convert.ToInt32(usedItem.Value_1));
                                   
                                    //API.givePlayerWeapon(sender, itemWeapon, Convert.ToInt32(usedItem.Value_1),true, true);
                                    rpgMgr.Me(sender, " elindeki " + itemWeapon.ToString() + " isimli silahının şarjörünü değiştirir.");
                                    goto BreakPoint;
                                }
                                else
                                {
                                    API.sendChatMessageToPlayer(sender, "~y~Şu anda dolu bir şarjörünüz bulunuyor.");
                                    return;
                                }
                            }
                        }

                        API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Elinizde bu şarjörü takabilecek silahınız yok. "); return;
                        //API.setPlayerWeaponAmmo(sender, API.getPlayerCurrentWeapon(sender), API.getPlayerWeaponAmmo(sender, API.getPlayerCurrentWeapon(sender)) + Convert.ToInt32(usedItem.Value_0));
                        #endregion
                        BreakPoint:
                        break;
                    //-----------------------------
                    case ItemType.License:
                        if (API.getEntityData(sender, "Dead") == true) return;
                        #region LicenseAction
                        switch (Convert.ToInt32(usedItem.Value_0))
                        {
                            case 0:
                                break;
                            case 1:
                                break;
                            case 2:
                                break;
                            case 3:
                                if (_inventory.ItemList.FirstOrDefault(x => x.ItemId == usedItem.ID).Equipped)
                                {
                                    _inventory.ItemList.FirstOrDefault(x => x.ItemId == usedItem.ID).Equipped = false;
                                    sender.resetNametagColor();
                                }
                                else
                                {
                                    _inventory.ItemList.FirstOrDefault(x => x.ItemId == usedItem.ID).Equipped = true;
                                    sender.nametagColor = new Color(0, 104, 255);
                                }
                                break;
                            case 4:
                                if (_inventory.ItemList.FirstOrDefault(x => x.ItemId == usedItem.ID).Equipped)
                                {
                                    _inventory.ItemList.FirstOrDefault(x => x.ItemId == usedItem.ID).Equipped = false;
                                    sender.resetNametagColor();
                                }
                                else
                                {
                                    _inventory.ItemList.FirstOrDefault(x => x.ItemId == usedItem.ID).Equipped = true;
                                    sender.nametagColor = new Color(255, 0, 100);
                                }
                                break;
                            case 5:
                                if (_inventory.ItemList.FirstOrDefault(x => x.ItemId == usedItem.ID).Equipped)
                                {
                                    _inventory.ItemList.FirstOrDefault(x => x.ItemId == usedItem.ID).Equipped = false;
                                    sender.resetNametagColor();
                                }
                                else
                                {
                                    _inventory.ItemList.FirstOrDefault(x => x.ItemId == usedItem.ID).Equipped = true;
                                    sender.nametagColor = new Color(20, 155, 100);
                                }
                                break;
                            default:

                                break;
                        }
                        #endregion
                        API.setEntityData(sender, "inventory", _inventory);
                        return;
                    //-----------------------------
                    case ItemType.Skin:
                        if (API.getEntityData(sender, "Dead") == true) return;
                        #region SkinAction
                        if (usedItemInInventory.Equipped)
                        {
                            API.sendChatMessageToPlayer(sender, "Üzerinizdeki son kıyafeti ~r~çıkaramazsınız.~s~ Ancak başka bir kıyafet ile değiştirebilirsiniz."); return;
                        }
                        if (Convert.ToBoolean(Convert.ToInt32(usedItem.Value_1)) != API.getEntityData(sender, "Gender"))
                        {
                            API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Bu kıyafeti sizin cinsiyetiniz için değil.");
                            return;
                        }
                        if (Convert.ToInt32(usedItem.Value_2) > 0 && Convert.ToInt32(usedItem.Value_2) != API.getEntityData(sender, "FactionId"))
                        {
                            API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bu kıyafeti giymek için " + FactionManager.ToFactionName(Convert.ToInt32(usedItem.Value_2)) + " üyesi olmalısınız.");
                            return;
                        }
                        var skins = db_Items.GameItems.Values.Where(x => x.Type == ItemType.Skin).Select(s => s.ID);
                        foreach (var item in _inventory.ItemList)
                        {
                            if (skins.Contains(item.ItemId))
                            {
                                item.Equipped = false;
                            }
                        }
                        usedItemInInventory.Equipped = true;
                        var _skin = API.pedNameToModel(usedItem.Value_0);
                        if (_skin.ToString() == "0") { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Geçersiz skin!"); return; }
                        API.setPlayerSkin(sender, _skin);
                        API.setEntityData(sender, "Skin", _skin);
                        API.setEntityData(sender, "inventory", _inventory);
                        #endregion
                        return;

                    //-----------------------------
                    case ItemType.Bag:
                        if (API.getEntityData(sender, "Dead") == true) return;
                        #region BagAction
                        if (usedItemInInventory.Equipped && _inventory.ItemList.Count > 10)
                        { API.sendChatMessageToPlayer(sender, "Bu çantayı çıkarabilmek için envanterin çok dolu."); return; }


                        else if (usedItemInInventory.Equipped)
                        {
                            usedItemInInventory.Equipped = false; _inventory.InventoryMaxCapacity = 10; API.setEntityData(sender, "inventory", _inventory);
                            if (API.hasEntityData(sender, "bag"))
                            {
                                GrandTheftMultiplayer.Server.Elements.Object bag = (GrandTheftMultiplayer.Server.Elements.Object)API.getEntityData(sender, "bag");
                                API.deleteEntity(bag);
                            }
                            return;
                        }
                        if (_inventory.ItemList.Count > usedItem.MaxCount)
                        {
                            var idList = db_Items.GameItems.Values.Where(s => s.Type == ItemType.Bag).Select(s => s.ID);
                            foreach (var item in _inventory.ItemList)
                            {
                                if (idList.Contains(item.ItemId))
                                {
                                    if (item.ItemId == usedItem.ID) { item.Equipped = true; continue; }
                                    item.Equipped = false;
                                }
                            }
                            _inventory.InventoryMaxCapacity = Convert.ToInt32(usedItem.Value_0);
                            API.setEntityData(sender, "inventory", _inventory);
                            API.consoleOutput(sender.rotation.ToString());
                            var bag = API.createObject(usedItem.ObjectId, sender.position, sender.rotation, sender.dimension);
                            API.attachEntityToEntity(bag, sender, "IK_Root", new Vector3(0, -0.15f, 0.35f), new Vector3(0, 0, 180));
                            //API.attachEntityToEntity(bag, sender, "SKEL_Spine0", new Vector3(0, -0.30f, 0.35f), new Vector3(0, -90, 180));
                            if (API.hasEntityData(sender, "bag"))
                            {
                                GrandTheftMultiplayer.Server.Elements.Object old_bag = (GrandTheftMultiplayer.Server.Elements.Object)API.getEntityData(sender, "bag");
                                API.deleteEntity(old_bag);
                            }
                            API.setEntityData(sender, "bag", bag);

                        }
                        else
                        {
                            API.sendChatMessageToPlayer(sender, "~y~Bu çantayı takabilmek için envanterin çok dolu.");
                        }
                        #endregion
                        return;
                    case ItemType.RepairPart:
                        if (API.getEntityData(sender, "Dead") == true) return;
                        #region RepairPartAction
                        var _vehicle = db_Vehicles.FindNearestVehicle(sender.position);
                        if (Vector3.Distance(sender.position, _vehicle.VehicleOnMap.position) < 4)
                        {
                            if (Convert.ToInt32(usedItem.Value_0) == -1)
                            {
                                for (int i = 0; i < 4; i++)
                                {
                                    if (API.isVehicleTyrePopped(_vehicle.VehicleOnMap, i))
                                    {
                                        API.popVehicleTyre(_vehicle.VehicleOnMap, i, true);
                                        goto Point;
                                    }
                                }
                                API.sendChatMessageToPlayer(sender, "~y~Etrafınızda lastiği değiştirilmesi gereken bir araç bulunmuyor.");
                                Point:
                                break;
                            }
                            else
                            if (Convert.ToInt32(usedItem.Value_0) == -2)
                            {
                                API.setVehicleHealth(_vehicle.VehicleOnMap, API.getVehicleHealth(_vehicle.VehicleOnMap) + Convert.ToInt32(usedItem.Value_1) > 1000 ? 1000 : API.getVehicleHealth(_vehicle.VehicleOnMap) + Convert.ToInt32(usedItem.Value_1));
                                API.sendChatMessageToPlayer(sender, "Tamir araca uygulandı.");
                                break;
                            }
                            else
                            if (Convert.ToInt32(usedItem.Value_0) == -3)
                            {
                                API.repairVehicle(_vehicle.VehicleOnMap);
                                API.sendChatMessageToPlayer(sender, "Tamir araca uygulandı.");
                            }
                            else
                            {
                                API.shared.consoleOutput(Convert.ToInt32(usedItem.Value_0) + " | val_0 ");
                                API.setVehicleMod(_vehicle.VehicleOnMap, Convert.ToInt32(usedItem.Value_0), Convert.ToInt32(usedItem.Value_1));
                                _vehicle.Mods[Convert.ToInt32(usedItem.Value_0)] = Convert.ToInt32(usedItem.Value_1);
                            }

                            break;
                        }
                        else
                        {
                            return;
                        }

                    #endregion

                    case ItemType.CraftingTable:
                        if (API.getEntityData(sender, "Dead") == true) return;
                        #region CraftingTable
                        var _table = db_Craftings.GetCraftingTableModel(Convert.ToInt32(usedItem.Value_0));
                        if (_table == null) { API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Böyle bir üretim masası bulunamadı. ~y~/rapor ~s~ile bildiriniz."); return; }
                        db_Craftings.CreateCraftingTableOnMap(new CraftingTablesOnMap
                        {
                            Dimension = sender.dimension,
                            Position = sender.position,
                            Rotation = sender.rotation,
                            CraftingTableModelId = _table.CraftingTableId,
                            Name = _table.Name,
                            OwnerSocialClubID = sender.socialClubName,
                        });
                        #endregion
                        break;
                    case ItemType.Furniture:
                        if (API.getEntityData(sender, "Dead") == true) return;
                        #region FurnitureAction
                        API.createObject(usedItem.ObjectId, sender.position, sender.rotation, sender.dimension);
                        #endregion

                        break;
                    case ItemType.Phone:
                        #region PhoneAction
                        SpecifiedValuePhone _phone = API.fromJson(usedItemInInventory.SpecifiedValue).ToObject<SpecifiedValuePhone>();
                        //Task.Run(async () =>
                        //{
                        if (API.hasEntityData(sender, "Cuffed") && (bool)API.getEntityData(sender, "Cuffed")) return;
                        Animation.OpenMobilePhone(sender, usedItem.ObjectId);
                        //API.playPlayerAnimation(sender, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "cellphone@", "f_cellphone_text_in");
                        //await Task.Delay(700);
                        //API.playPlayerAnimation(sender, (int)(AnimationFlags.Loop | AnimationFlags.OnlyAnimateUpperBody | AnimationFlags.AllowPlayerControl), "anim@cellphone@in_car@ds", "cellphone_text_read_base");
                        //});

                        List<string> appsOnPhone = new List<string>();
                        foreach (var item in _phone.Applications)
                        {
                            appsOnPhone.Add(item.ToString());
                        }
                        API.triggerClientEvent(sender, "phone_open", _phone.Applications.Count, appsOnPhone.ToArray(), null, _phone.FlightMode ? "UÇAK MODU" : (_phone.PhoneOperator == null ? "HAT YOK" : _phone.PhoneOperator.ToString()), _phone.PhoneNumber == null ? "Numara Yok" : _phone.PhoneNumber.ToString(), usedItemInInventory.ItemId);

                        #endregion

                        return;

                    case ItemType.Wearable:
                        #region Wearable
                        if (usedItem.Value_0.ToLower() == "megaphone")
                        {
                            if (usedItemInInventory.Equipped)
                            {
                                API.resetEntityData(sender, "M");
                                usedItemInInventory.Equipped = false;
                            }
                            else
                            {
                                API.setEntityData(sender, "M", true);
                                usedItemInInventory.Equipped = true;
                            }
                        }
                        if (usedItem.Value_0 == "2")
                        {
                            if (usedItemInInventory.Equipped)
                            {
                                API.resetEntityData(sender, "E");
                                usedItemInInventory.Equipped = false;
                            }
                            else
                            {
                                API.setEntityData(sender, "E", true);
                                usedItemInInventory.Equipped = true;
                            }
                        }
                        usedItemInInventory.Equipped = !usedItemInInventory.Equipped;
                        #endregion
                        return;
                    default:

                        break;
                }



                if (usedItemInInventory.Count > 1)
                {
                    usedItemInInventory.Count--;
                }
                else
                    _inventory.ItemList.Remove(usedItemInInventory);

                API.setEntityData(sender, "inventory", _inventory);
                #endregion
                return;
            }

            if (eventName == "shop_item_selected")
            {
                //args = [shopid] [index]
                var _Shop = db_Shops.GetShop(Convert.ToInt32(arguments[0]));
                int index = Convert.ToInt32(arguments[1]);
                if (_Shop == null) { API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Shop bulunamadı."); return; }
                int _PlayerMoney = API.getEntityData(sender, "Money");

                if (_PlayerMoney >= _Shop.SaleItemList[index].Price)
                {
                    var buyedItem = db_Items.GetItemById(_Shop.SaleItemList[index].GameItemId);
                    int missionNumber = (API.hasEntityData(sender, "Mission") ? API.getEntityData(sender, "Mission") : 0);

                    switch (buyedItem.Type)
                    {
                        case ItemType.Phone:
                            if (AddItemToPlayerInventory(sender, new Models.ClientItem
                            {
                                ItemId = buyedItem.ID,
                                Count = 1,
                                Equipped = false,
                                SpecifiedValue = API.toJson(new SpecifiedValuePhone { Applications = new List<Application> { Application.GPS }, AutoInternetPay = false, Balance = 0, FlightMode = false, PhoneOperator = null, Contacts = new Dictionary<string, string>(), Frequence = -1, InternetBalance = 0, PhoneNumber = null, })
                            }))
                            {
                                AddMoneyToPlayer(sender, -1 * _Shop.SaleItemList[index].Price);
                                #region AboutMission

                                if (missionNumber == 2)
                                {
                                    Clients.ClientManager.RemoveMissionMarker(sender);
                                    API.setEntityData(sender, "Mission", 3);
                                    UserCommands.TriggerUserMission(sender);
                                }

                                #endregion
                                return;
                            }
                            break;
                        case ItemType.Skin:
                            if (AddItemToPlayerInventory(sender, new ClientItem { ItemId = buyedItem.ID, Count = 1, Equipped = false }))
                            {
                                AddMoneyToPlayer(sender, -1 * _Shop.SaleItemList[index].Price);
                                #region AboutMission
                                if (missionNumber == 1)
                                {
                                    Clients.ClientManager.RemoveMissionMarker(sender);
                                    API.setEntityData(sender, "Mission", 2);
                                    UserCommands.TriggerUserMission(sender);
                                }
                                #endregion 
                                return;
                            }
                            break;
                        case ItemType.Weapon:
                            var _inventory = (Inventory)API.getEntityData(sender, "inventory");


                            if (Convert.ToInt32(buyedItem.Value_2) < 1 || _inventory.ItemList.Any(x => x.ItemId == 302 && x.SpecifiedValue == sender.socialClubName || Convert.ToInt32(buyedItem.Value_2) == 4))
                            {
                                if (AddItemToPlayerInventory(sender, new ClientItem { ItemId = buyedItem.ID, Count = 1, Equipped = false }))
                                {
                                    AddMoneyToPlayer(sender, -1 * _Shop.SaleItemList[index].Price);
                                    return;
                                }
                            }
                            else
                            {
                                API.sendChatMessageToPlayer(sender, "~r~UYARI: ~s~Buradan alışveriş yapmak için kendinize ait bir silah ruhsatına ihtiyacınız var.");
                                return;
                            }

                            break;
                        default:
                            if (AddItemToPlayerInventory(sender, new ClientItem { ItemId = buyedItem.ID, Count = 1, Equipped = false }))
                            {
                                AddMoneyToPlayer(sender, -1 * _Shop.SaleItemList[index].Price);
                                return;
                            }
                            break;
                    }
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bu eşya için envanterinizde daha fazla yer bulunmuyor.");

                }
                else
                {
                    API.shared.sendChatMessageToPlayer(sender, $"~r~UYARI: ~s~Bunu alabilmek için en az ~r~{_Shop.SaleItemList[index].Price}$ ~s~paranız olmalı. ");
                }



                ////coming args from client = [name = Format ("[itemname] | $[price]")]


                //#region shop item selected
                //int _money = API.getEntityData(sender, "Money");
                //int price = Convert.ToInt32(arguments[0].ToString().Split('$').LastOrDefault());

                //if (_money >= price)
                //{
                //    var _inventory = (Inventory)API.getEntityData(sender, "inventory");
                //    if (_inventory.ItemList.Count < _inventory.InventoryMaxCapacity)
                //    {
                //        var buyedItem = db_Items.GameItems.Items.FirstOrDefault(x => x.Name == arguments[0].ToString().Split('$').FirstOrDefault().Replace("|", String.Empty).Trim());

                //        if (_inventory.ItemList.Select(s => s.ItemId).Contains(buyedItem.ID))
                //        {
                //            if (_inventory.ItemList.FirstOrDefault(x => x.ItemId == buyedItem.ID).Count < db_Items.GameItems.Items.FirstOrDefault(x => x.ID == buyedItem.ID).MaxCount)
                //            {
                //                _inventory.ItemList.FirstOrDefault(x => x.ItemId == buyedItem.ID).Count++;
                //            }
                //            else
                //            {
                //                API.sendChatMessageToPlayer(sender, "~y~Bu eşyadan daha fazla taşıyamazsınız.");
                //                return;
                //            }
                //        }
                //        else
                //        {


                //            if (buyedItem.Type == ItemType.Phone)
                //            {
                //                _inventory.ItemList.Add(new ClientItem
                //                {
                //                    Count = 1,
                //                    ItemId = buyedItem.ID,
                //                    SpecifiedValue = API.toJson(new SpecifiedValuePhone { Applications = new List<Application> { Application.GPS }, AutoInternetPay = false, Balance = 0, FlightMode = false, PhoneOperator = null, Contacts = new Dictionary<string, string>(), Frequence = -1, InternetBalance = 0, PhoneNumber = null, })
                //                });

                //            }
                //            else
                //                if (buyedItem.Type == ItemType.Skin)
                //            {
                //                _inventory.ItemList.Add(new ClientItem
                //                {
                //                    Count = 1,
                //                    ItemId = buyedItem.ID,
                //                });
                //            }
                //            else
                //            {
                //                _inventory.ItemList.Add(new ClientItem
                //                {
                //                    Count = 1,
                //                    ItemId = buyedItem.ID,
                //                });
                //            }
                //        }

                //        API.setEntityData(sender, "inventory", _inventory);
                //        _money -= price;
                //        API.setEntityData(sender, "Money", _money);
                //        API.triggerClientEvent(sender, "update_money_display", _money);
                //        API.sendNotificationToPlayer(sender, "Satın alındı.", true);
                //    }
                //    else
                //    {
                //        API.sendChatMessageToPlayer(sender, "~y~Bu eşyayı almak için envanterinizde yeterli alan bulunmuyor.");
                //    }

                //}
                //else
                //{
                //    API.sendChatMessageToPlayer(sender, "~y~Bu eşyayı almak için paranız yetersiz.");
                //}
                //#endregion
                return;
            }

            if (eventName == "select_players_list")
            {
                #region select_player
                var playerId = Convert.ToInt32(arguments[0].ToString().Split(')')[0].Replace("(", String.Empty).Trim());
                foreach (var item in API.getAllPlayers())
                {
                    if (API.getEntityData(item, "ID") == playerId && Vector3.Distance(item.position, sender.position))
                    {
                        var giverInventory = (Inventory)API.getEntityData(sender, "inventory");
                        var takerInventory = (Inventory)API.getEntityData(item, "inventory");
                        var givingItem = giverInventory.ItemList.FirstOrDefault(x => x.ItemId == Convert.ToInt32(arguments[1].ToString()));
                        if (givingItem != null)
                        {
                            if (!givingItem.Equipped)
                            {
                                if (takerInventory.ItemList.Count < takerInventory.InventoryMaxCapacity)
                                {
                                    giverInventory.ItemList.Remove(givingItem);
                                    givingItem.Equipped = false;
                                    takerInventory.ItemList.Add(givingItem);
                                    API.setEntityData(sender, "inventory", giverInventory);
                                    API.setEntityData(item, "inventory", takerInventory);
                                    RPGManager rpgMgr = new RPGManager();
                                    rpgMgr.Me(sender, " adlı kişi " + item.nametag + " adlı kişiye bir eşya verir.");
                                }
                                else
                                {
                                    API.sendChatMessageToPlayer(sender, "Karşı tarafın envanterinde yeterli yer ~r~bulunmuyor.");
                                }
                            }
                            else
                            {
                                API.sendChatMessageToPlayer(sender, "~r~Lütfen önce eşyayı üzerinizden çıkarın!");
                            }
                        }
                        else
                            API.sendChatMessageToPlayer(sender, "Bir hata oluştu.");
                    }
                }
                #endregion
            }
            else
            if (eventName == "retun_players_list")
            {
                //RETURN - itemText - itemDescription
                int _Id = Convert.ToInt32(arguments[0].ToString().Split('(').LastOrDefault().Split(')').FirstOrDefault());

                foreach (var item in API.getAllPlayers())
                {
                    if (API.getEntityData(item, "ID") == _Id && Vector3.Distance(sender.position, item.position) < 3)
                    {
                        var _giverInventory = (Inventory)API.getEntityData(sender, "inventory");
                        var _takerInventory = (Inventory)API.getEntityData(sender, "inventory");
                        if (_takerInventory.ItemList.Count < _takerInventory.InventoryMaxCapacity)
                        {
                            try
                            {
                                var _givingItem = _giverInventory.ItemList.FirstOrDefault(x => x.ItemId == Convert.ToInt32(arguments[1]));
                                _giverInventory.ItemList.Remove(_givingItem);
                                _takerInventory.ItemList.Add(_givingItem);
                                RPGManager rpgMgr = new RPGManager();
                                rpgMgr.Me(sender, ", " + API.getEntityData(item, "CharacterName") + " adlı kişiye bir şeyler verir.");
                            }
                            catch (Exception ex)
                            {
                                API.consoleOutput(LogCat.Error, ex.ToString());
                                API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bir hata oluştu.");
                            }
                        }
                        else
                        {
                            API.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Karşı tarafın envanterinde bu eşyayı depolayacak yer yok.");
                        }
                        return;
                    }
                }
            }
        }

        [Command("envanter", "/i", Alias = "i")]
        public void OnInventory(Client sender)
        {
            Inventory _inventory = API.getEntityData(sender, "inventory");

            List<string> descList = new List<string>();
            List<string> nameList = new List<string>();
            //Dead Item Registry
            int _Index = 0; List<int> indexes = new List<int>();
            foreach (var item in _inventory.ItemList)
            {
                var _gameItem = db_Items.GetItemById(item.ItemId);
                if (_gameItem == null) { indexes.Add(_Index); continue; }
                nameList.Add((item.Equipped ? "*" : String.Empty) + _gameItem.Name + " (" + item.Count + ")");
                if (_gameItem.Type == ItemType.License)
                {
                    descList.Add("" + (String.IsNullOrEmpty(item.SpecifiedValue) ? "Belirsiz" : db_Accounts.GetOfflineUserDatas(item.SpecifiedValue).CharacterName) + " adlı kişiye ait.");
                    continue;
                }
                descList.Add(_gameItem.Description);
                _Index++;
            }
            string desc = "Eşyalarım  |  " + _inventory.ItemList.Count + " / " + _inventory.InventoryMaxCapacity;
            API.triggerClientEvent(sender, "inventory_open", nameList.Count(), nameList.ToArray(), descList.ToArray(), desc, sender.socialClubName);

            #region ForDeadRegistry
            foreach (var item in indexes)
            {
                _inventory.ItemList.RemoveAt(item);
            }
            API.setEntityData(sender, "inventory", _inventory);
            #endregion
        }
        [Command("envanteregerikoy")]
        public static void PutItToInvenotry(Client sender)
        {
            if (!(sender.currentWeapon == WeaponHash.Unarmed))
            {
                API.shared.consoleOutput(" current weapon " + sender.currentWeapon.ToString());
                var _gameItem = db_Items.GameItems.Values.FirstOrDefault(x => x.Value_0.ToLower().StartsWith(sender.currentWeapon.ToString().ToLower()));
                if (_gameItem != null)
                {
                    var _inventory = (Inventory)API.shared.getEntityData(sender, "inventory");
                    _inventory.ItemList.Add(new ClientItem { ItemId = _gameItem.ID, Count = 1 });
                    API.shared.setEntityData(sender, "inventory", _inventory);
                    API.shared.removePlayerWeapon(sender, sender.currentWeapon);
                }
                else
                {
                    API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~w~Bu çantanıza konabilecek bir eşya değil.");
                }
            }
            else
            {
                API.shared.sendChatMessageToPlayer(sender, "~y~Şu anda elinizde silah tutmuyorsunuz.");
            }
        }
        public static void PutWeaponToInventory(Client sender, WeaponHash _weapon)
        {
            try
            {
                var _inventory = GetPlayerInventory(sender);
                var _item = _inventory.ItemList.FirstOrDefault(x => API.shared.weaponNameToModel(db_Items.GetItemById(x.ItemId).Value_0) == _weapon);
                if (_item != null)
                {
                    _item.Equipped = false;
                    var _specifiedValue = (SpecifiedValueWeapon)(String.IsNullOrEmpty(_item.SpecifiedValue) ? new SpecifiedValueWeapon() : API.shared.fromJson(_item.SpecifiedValue).ToObject<SpecifiedValueWeapon>());

                    if (!sender.weapons.Contains(_weapon))
                        return;
                    _specifiedValue.Ammo = API.shared.getPlayerWeaponAmmo(sender, _weapon);
                    // API.shared.sendChatMessageToPlayer(sender, _weapon+" read ammo: "+_specifiedValue.Ammo);
                    _specifiedValue.WeaponTint = API.shared.getPlayerWeaponTint(sender, _weapon);
                    _item.SpecifiedValue = API.shared.toJson(_specifiedValue);
                    var _GameItem = db_Items.GetItemById(_item.ItemId);
                    Animation.WearWeapon(sender, _GameItem.ObjectId, Convert.ToInt32(_GameItem.Value_2));
                    UpdatePlayerInventory(sender, _inventory);
                    API.shared.removePlayerWeapon(sender, _weapon);
                }
            }
            catch (Exception ex)
            {
                if (ex.GetType() != typeof(NullReferenceException))
                {
                    API.shared.consoleOutput(LogCat.Warn, ex.ToString());
                }
            }
        }
        public void GivePlayerList(Client sender, int range, int itemId, string itemName)
        {
            List<string> nameList = new List<string>();
            foreach (var item in API.getAllPlayers())
            {
                if (sender != item && Vector3.Distance(sender.position, item.position) < range)
                {
                    nameList.Add(item.nametag);
                }
            }
            if (nameList.Count > 0)
            {
                API.triggerClientEvent(sender, "select_players_list", nameList.Count, nameList.ToArray(), itemId.ToString(), "Verilecek eşya " + itemName);
            }
            else
            {
                API.sendChatMessageToPlayer(sender, "~y~Etrafınızda eşya verebileceğiniz kimse yok.");
            }
        }
        [Command("kabulet")]
        public void AcceptTrade(Client sender)
        {
            if (!API.shared.hasEntityData(sender, "Offer")) { API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Herhangi bir teklif yok."); return; }
            TradeModel _trade = (TradeModel)API.getEntityData(sender, "Offer");

            var sellerPlayer = db_Accounts.IsPlayerOnline(_trade.SellerSocialClubID);
            if (sellerPlayer != null)
            {
                if (Vector3.Distance(sender.position, sellerPlayer.position) < 3)
                {
                    API.shared.resetEntityData(sender, "Offer");
                    var _item = GetItemFromPlayerInventory(sellerPlayer, _trade.OfferedItemIndex);
                    if (_item == null)
                    {
                        API.sendChatMessageToPlayer(sellerPlayer, "~r~HATA: ~s~Teklif ettiğiniz ürün artık üzerinizde bulunmuyor.");
                        API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Diğer oyuncu artık bu ürüne sahip değil");
                        return;
                    }

                    if (IsEnoughMoney(sender, _trade.OfferedPrice))
                    {
                        if (AddItemToPlayerInventory(sender, _item.Item2))
                        {
                            if (RemoveItemFromPlayerInventory(sellerPlayer, _item.Item2))
                            {
                                AddMoneyToPlayer(sender, -1 * _trade.OfferedPrice);
                                AddMoneyToPlayer(sellerPlayer, _trade.OfferedPrice);
                                API.shared.sendChatMessageToPlayer(sellerPlayer, "~y~Ticaret başarıyla gerçekleşti");
                                API.shared.sendChatMessageToPlayer(sender, "~y~Ticaret başarıyla gerçekleşti");
                            }
                        }
                        else
                        {
                            API.shared.sendChatMessageToPlayer(sellerPlayer, "~r~HATA: ~s~Karşı tarafın envanterinde yeterli alan yok.");
                            API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Envanterinizde bu alışveriş için yeterli alan yok.");
                        }
                    }
                    else
                    {
                        API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bu ticaret için yeterli paranız yok.");
                    }


                    //if (AddItemToPlayerInventory(player, _item.Item2))
                    //{
                    //    if (RemoveItemFromPlayerInventory(sender, GetItemFromPlayerInventory(sender, index).Item2))
                    //    {

                    //    }
                    //}

                }
                else
                {
                    API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Satıcı yanınızda olmalı.");
                }
            }
            else
            {
                API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Satıcı oyunda değil veya artık teklifi sunmuyor.");
            }


        }
        public static void OnSellItemPlayerList(Client sender, int index)
        {
            var _item = GetItemFromPlayerInventory(sender, index);
            if (_item != null)
            {
                if (!_item.Item1.Droppable) { API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bu eşyayı satamazsınız."); return; }
                if (_item.Item2.Equipped) { API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Kullanmakta olduğunuz bir eşyayı satamazsınız."); return; }
                List<string> names = new List<string>();
                List<int> ids = new List<int>();
                foreach (var item in API.shared.getAllPlayers())
                {
                    if (item == sender) continue;
                    if (Vector3.Distance(sender.position, item.position) < 5)
                    {
                        names.Add(db_Accounts.GetPlayerCharacterName(item));
                        ids.Add(API.shared.getEntityData(item, "ID"));
                    }
                }
                if (names.Count == 0)
                {
                    API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Etrafınızda ticaret yapabileceğiniz kimse yok.");
                    return;
                }
                Clients.ClientManager.ChoosePlayerToSellItem(sender, names, ids, index);

            }
        }

        public static void OnSellItemSelected(Client sender, int index, int targetPlayerId, int money)
        {
            var player = db_Accounts.FindPlayerById(targetPlayerId);
            if (player != null)
            {

                if (Vector3.Distance(sender.position, player.position) < 3)
                {
                    var _item = GetItemFromPlayerInventory(sender, index);
                    if (_item.Item2.Equipped) { API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Kullanmakta olduğunuz bir eşyayı satamazsınız."); return; }
                    if (!_item.Item1.Droppable) { API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bu eşya satılamaz."); return; }
                    if (API.shared.getEntityData(sender, "FactionId") > 0 && (_item.Item1.Type == ItemType.Weapon || _item.Item1.Type == ItemType.Armor || _item.Item1.Type == ItemType.FirstAid))
                    {
                        API.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Bu eşyayı " + FactionManager.ToFactionName(API.shared.getEntityData(sender, "FactionId")) + " oluşumundayken satamazsınız.");
                        return;
                    }
                    API.shared.setEntityData(player, "Offer", new TradeModel
                    {
                        OfferedItemIndex = index,
                        BuyerSocialClubID = player.socialClubName,
                        SellerSocialClubID = sender.socialClubName,
                        OfferedPrice = money
                    });
                    API.shared.sendChatMessageToPlayer(player, $"~y~~h~{db_Accounts.GetPlayerCharacterName(sender)} adlı kişi size {_item.Item1.Name} adlı eşyayı {money}$ karşılığında satmayı öneriyor. \n (( /kabulet )) (( /reddet ))");
                    API.shared.sendChatMessageToPlayer(sender, "~y~Teklifiniz gönderildi.");
                }
                else
                {
                    API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Oyuncu yanınızda olmalı.");
                }
            }
            else
            {
                API.shared.sendChatMessageToPlayer(sender, "~r~HATA: ~s~Oyuncu bulunamadı.");
            }
        }
        public static Tuple<Item, ClientItem> GetItemFromPlayerInventory(Client sender, int index)
        {
            Inventory _inventory = (Inventory)API.shared.getEntityData(sender, "inventory");
            if (index < _inventory.ItemList.Count)
            {
                var gameItem = db_Items.GetItemById(_inventory.ItemList[index].ItemId);
                if (gameItem != null)
                {
                    return new Tuple<Item, ClientItem>(gameItem, _inventory.ItemList[index]);
                }
                else
                    return null;
            }
            else
                return null;
        }
        public static Tuple<Item, ClientItem> GetWeaponFromPlayerInventory(Client sender, WeaponHash _type)
        {
            var _inventory = GetPlayerInventory(sender);
            var _clientItem = _inventory.ItemList.FirstOrDefault(x => API.shared.weaponNameToModel(db_Items.GetItemById(x.ItemId).Value_0) == _type);
            if (_clientItem != null)
            {
                return new Tuple<Item, ClientItem>(db_Items.GetItemById(_clientItem.ItemId), _clientItem);
            }
            return null;
        }
        public static Tuple<Item, ClientItem> GetItemFromPlayerInventoryById(Client sender, int gameItemId)
        {
            var _inventory = GetPlayerInventory(sender);
            var _item = _inventory.ItemList.FirstOrDefault(x => x.ItemId == gameItemId);
            if (_item != null)
            {
                return new Tuple<Item, ClientItem>(db_Items.GetItemById(gameItemId), _item);
            }
            return null;
        }

        public static List<Tuple<Item, ClientItem>> GetItemsFromPlayerInventory(Client sender, ItemType _type)
        {
            var _inventory = (Inventory)API.shared.getEntityData(sender, "inventory");
            var returnModel = new List<Tuple<Item, ClientItem>>();

            foreach (var item in _inventory.ItemList)
            {
                var gameItem = db_Items.GameItems.Values.FirstOrDefault(x => x.Type == _type && x.ID == item.ItemId);
                if (gameItem != null)
                {
                    returnModel.Add(new Tuple<Item, ClientItem>(gameItem, item));
                }
            }
            return returnModel;
            //API.shared.setEntityData(sender, "inventory", _inventory);
        }
        public static bool AddItemToPlayerInventory(Client player, ClientItem _clientItem)
        {
            var _inventory = (Inventory)API.shared.getEntityData(player, "inventory");
            var _gameItem = db_Items.GetItemById(_clientItem.ItemId);
            if (_gameItem == null) { return false; }
            if (_inventory.ItemList.Count < _inventory.InventoryMaxCapacity)
            {
                var itemInInventory = _inventory.ItemList.FirstOrDefault(x => x.ItemId == _gameItem.ID);
                if (itemInInventory != null && !(_gameItem.Type == ItemType.License || _gameItem.Type == ItemType.Phone))
                {
                    if (itemInInventory.Count < _gameItem.MaxCount)
                    {
                        itemInInventory.Count++;
                        return true;
                    }
                    else return false;
                }
                else
                {
                    _inventory.ItemList.Add(_clientItem);
                    itemInInventory = _inventory.ItemList.FirstOrDefault(x => x.ItemId == _gameItem.ID);
                }
                if (_gameItem.Type == ItemType.Weapon) { Animation.WearWeapon(player, _gameItem.ObjectId, Convert.ToInt32(_gameItem.Value_2)); }
                if (_gameItem.Type == ItemType.Phone) { PhoneManager.AddPhoneNumberToPlayer(player, itemInInventory); }
                API.shared.setEntityData(player, "inventory", _inventory);
                return true;
            }
            else
            {
                return false;
            }
        }
        public static bool AddItemToPlayerInventory(Client player, Item _gameItem)
        {
            var _inventory = (Inventory)API.shared.getEntityData(player, "inventory");

            if (_inventory.ItemList.Count < _inventory.InventoryMaxCapacity)
            {
                var clientItem = _inventory.ItemList.FirstOrDefault(x => x.ItemId == _gameItem.ID);
                if (_gameItem.Type == ItemType.Phone) PhoneManager.RemoveNumberFromPlayer(player, clientItem);

                if (clientItem != null)
                {
                    if (clientItem.Count < _gameItem.MaxCount)
                    {
                        clientItem.Count++;
                        return true;
                    }
                    else
                        return false;
                }
                else
                {
                    _inventory.ItemList.Add(new ClientItem { Count = 1, Equipped = false, ItemId = _gameItem.ID });
                }
                API.shared.setEntityData(player, "inventory", _inventory);
                return true;
            }
            return false;

        }
        public static bool RemoveItemFromPlayerInventory(Client player, ClientItem _clientItem)
        {
            var _inventory = (Inventory)API.shared.getEntityData(player, "inventory");
            if (_inventory != null)
            {
                if (!_clientItem.Equipped)
                {
                    if (db_Items.GetItemById(_clientItem.ItemId).Type == ItemType.Phone) PhoneManager.RemoveNumberFromPlayer(player, _clientItem);

                    if (_clientItem.Count > 1)
                        _clientItem.Count--;
                    else
                        _inventory.ItemList.Remove(_clientItem);

                    API.shared.setEntityData(player, "inventory", _inventory);
                    Animation.RemoveObjectIfWeapon(player, db_Items.GetItemById(_clientItem.ItemId));

                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
                return false;
        }
        public static bool RemoveItemFromPlayerInventory(Client player, int GameItemId)
        {
            var _inventory = (Inventory)API.shared.getEntityData(player, "inventory");
            if (_inventory != null)
            {
                var _Index = _inventory.ItemList.IndexOf(_inventory.ItemList.Find(x => x.ItemId == GameItemId));
                if (_Index >= 0)
                {
                    var _gameItem = db_Items.GetItemById(GameItemId);
                    if (_gameItem.Type == ItemType.Phone) PhoneManager.RemoveNumberFromPlayer(player, _inventory.ItemList[_Index]);

                    if (_inventory.ItemList[_Index].Count > 1)
                    {
                        _inventory.ItemList[_Index].Count--;
                    }
                    else
                    {
                        _inventory.ItemList.RemoveAt(_Index);
                    }
                    Animation.RemoveObjectIfWeapon(player, _gameItem);
                    API.shared.setEntityData(player, "inventory", _inventory);
                    return true;
                }
                else
                    return false;

            }
            else
                return false;
        }
        public static bool RemoveItemFromPlayerInventoryByIndex(Client player, int Index)
        {
            var _inventory = (Inventory)API.shared.getEntityData(player, "inventory");
            if (_inventory != null)
            {
                if (_inventory.ItemList.Count <= Index) { return false; }

                if (!_inventory.ItemList[Index].Equipped)
                {

                    //API.shared.consoleOutput("Item ID: " + _inventory.ItemList[Index].ItemId + " type " + db_Items.GetItemById(_inventory.ItemList[Index].ItemId).Type);
                    var _gameItem = db_Items.GetItemById(_inventory.ItemList[Index].ItemId);
                    Animation.RemoveObjectIfWeapon(player, _gameItem);
                    if (_gameItem.Type == ItemType.Phone) PhoneManager.RemoveNumberFromPlayer(player, _inventory.ItemList[Index]);

                    if (_inventory.ItemList[Index].Count > 1)
                    {
                        _inventory.ItemList[Index].Count--;
                    }
                    else
                    {
                        _inventory.ItemList.RemoveAt(Index);
                    }

                    API.shared.setEntityData(player, "inventory", _inventory);
                    return true;
                }
                return false;
            }
            else
                return false;
        }
        public static void LoadPlayerEquippedItems(Client sender)
        {
            var _inventory = (Inventory)API.shared.getEntityData(sender, "inventory");
            foreach (var item in _inventory.ItemList)
            {
                if (item.Equipped)
                {
                    var gameItem = db_Items.GetItemById(item.ItemId);
                    if (gameItem != null)
                    {
                        switch (gameItem.Type)
                        {
                            case ItemType.Bag:
                                var bag = API.shared.createObject(gameItem.ObjectId, sender.position, sender.rotation, sender.dimension);
                                API.shared.attachEntityToEntity(bag, sender, "IK_Root", new Vector3(0, -0.15f, 0.35f), new Vector3(0, 0, 180));
                                API.shared.setEntityData(sender, "bag", bag);
                                break;
                            case ItemType.Weapon:
                                SpecifiedValueWeapon _weaponSpec = String.IsNullOrEmpty(item.SpecifiedValue) ? new SpecifiedValueWeapon { Ammo = 0, WeaponTint = WeaponTint.Normal } : (SpecifiedValueWeapon)API.shared.fromJson(item.SpecifiedValue).ToObject<SpecifiedValueWeapon>();
                                //OLD
                                API.shared.givePlayerWeapon(sender, API.shared.weaponNameToModel(gameItem.Value_0), _weaponSpec.Ammo,true, true);
                                API.shared.setPlayerWeaponTint(sender, API.shared.getPlayerWeapons(sender).FirstOrDefault(), _weaponSpec.WeaponTint);
                                break;
                            case ItemType.License:
                                #region Rozet
                                switch (Convert.ToInt32(gameItem.Value_0))
                                {
                                    case 3:
                                        sender.nametagColor = new Color(0, 104, 255);
                                        break;
                                    case 4:
                                        sender.nametagColor = new Color(255, 0, 100);
                                        break;
                                    case 5:
                                        sender.nametagColor = new Color(20, 155, 100);
                                        break;
                                    default:
                                        break;
                                }
                                #endregion
                                break;
                            case ItemType.Wearable:
                                if (gameItem.Value_0 == "megaphone")
                                {
                                    API.shared.setEntityData(sender, "M", true);
                                }
                                if (gameItem.Value_0 == "2")
                                {
                                    API.shared.setEntityData(sender, "E", true);
                                }
                                break;
                            default:
                                break;
                        }
                    }

                }
            }
        }
        public static bool IsEquippedItem(Client player, int gameItemID)
        {
            var _item = GetItemFromPlayerInventoryById(player, gameItemID);
            if (_item != null)
            {
                return _item.Item2.Equipped;
            }
            return false;
        }
        //[Command("wc")]
        //public void WeaponCount(Client sender)
        //{
        //    API.shared.consoleOutput("weapons : "+API.shared.getPlayerWeapons(sender).Length.ToString());
        //}
        public static void RemovePlayerEquippedItems(Client sender)
        {
            if (API.shared.hasEntityData(sender, "bag"))
            {
                GrandTheftMultiplayer.Server.Elements.Object bag = (GrandTheftMultiplayer.Server.Elements.Object)API.shared.getEntityData(sender, "bag");
                API.shared.deleteEntity(bag);
            }
        }
        public static Inventory GetPlayerInventory(Client sender)
        {
            return (Inventory)API.shared.getEntityData(sender, "inventory");
        }
        public static void UpdatePlayerInventory(Client sender, Inventory _inventory)
        {
            API.shared.setEntityData(sender, "inventory", _inventory);
        }
        public static int GetPlayerMetalParts(Client sender)
        {
            return GetPlayerInventory(sender).MetalParts;
        }
        public static bool IsEnoughMetalParts(Client sender, int value)
        {
            return GetPlayerInventory(sender).MetalParts >= value;
        }
        public static void AddMetalPartsToPlayer(Client sender, int value, bool notify = true)
        {
            var _inventory = GetPlayerInventory(sender);
            _inventory.MetalParts += value;
            UpdatePlayerInventory(sender, _inventory);
            if (notify)
                API.shared.sendNotificationToPlayer(sender, "~y~" + value + " Metal Parça");
        }
        public static bool DoesInventoryHasSpace(Client sender)
        {
            var _inventory = (Inventory)API.shared.getEntityData(sender, "inventory");
            return _inventory.ItemList.Count < _inventory.InventoryMaxCapacity;
        }
        public static bool DoesPlayerHasItemById(Client sender, int itemId)
        {
            var _inventory = (Inventory)API.shared.getEntityData(sender, "inventory");
            return _inventory.ItemList.Any(x => x.ItemId == itemId);
        }
        public static int GetPlayerMoney(Client player)
        {
            return API.shared.getEntityData(player, "Money");
        }
        public static bool IsEnoughMoney(Client player, int price)
        {
            int money = API.shared.getEntityData(player, "Money");
            return money >= price;
        }
        public static void AddMoneyToPlayer(Client sender, int amount)
        {
            int money = API.shared.getEntityData(sender, "Money");
            money += amount;
            API.shared.setEntityData(sender, "Money", money);
            Clients.ClientManager.UpdateMoneyDisplay(sender, money);

            if (amount > 0)
                API.shared.sendNotificationToPlayer(sender, "~g~+" + amount + "$");
            else
                API.shared.sendNotificationToPlayer(sender, "~r~" + amount + "$");

        }
        public static int GetPlayerBankMoney(Client player)
        {
            return API.shared.getEntityData(player, "BankMoney");
        }
        public static bool IsEnougMoneyInBank(Client player, int amount)
        {
            return GetPlayerBankMoney(player) >= amount;
        }
        public static void AddMoneyToPlayerBank(Client player, int amount, bool notify = true)
        {
            int bankMoney = API.shared.getEntityData(player, "BankMoney");
            bankMoney += amount;
            API.shared.setEntityData(player, "BankMoney", bankMoney);
            if (notify)
                API.shared.sendNotificationToPlayer(player, "~s~Banka Hesabı:\n" + (amount < 0 ? "~r~" : "~g~") + amount + "$");
        }
        public static void AddMoneyToOfflinePlayerBank(string socialClubId, int amount)
        {
            var _player = db_Accounts.GetOfflineUserDatas(socialClubId);
            if (_player != null)
            {
                _player.BankMoney += amount;
                db_Accounts.SaveOfflineUserData(socialClubId, _player);
            }
        }
        public static List<Tuple<Item, ClientItem>> GetItemFromOfflineUser(string socialClubId, ItemType _type)
        {
            var model = new List<Tuple<Models.Item, Models.ClientItem>>();
            var _inventory = db_Accounts.GetOfflineUserInventory(socialClubId);
            if (_inventory != null)
            {
                foreach (var clientItem in _inventory.ItemList)
                {
                    var _gameItem = db_Items.GetItemById(clientItem.ItemId);
                    if (_gameItem.Type == _type)
                    {
                        model.Add(new Tuple<Item, ClientItem>(_gameItem, clientItem));
                    }
                }
            }
            return model;
        }

        [Command("ammo")]
        public void Ammo(Client sender)
        {
            API.sendChatMessageToPlayer(sender, "Mermisi: " + API.getPlayerWeaponAmmo(sender, sender.currentWeapon));
        }
    }
}
