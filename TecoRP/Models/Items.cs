using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Shared.Math;
using TecoRP.Database;

namespace TecoRP.Models
{
    #region StoringForGlobal
    /// <summary>
    /// <drinkable>
    /// <value_0> THIRSTY_INCREASE_VALUE </value_0>
    /// <value_1> HEALTH_INCREASE_VALUE </value_1>
    /// <value_2> DRUNK_INCREASE_VALUE</value_2>
    /// </drinkable>
    ///   <Eatable>
    /// <value_0> HUNGER_INCREASE_VALUE </value_0>
    /// <value_1> HEALTH_INCREASE_VALUE </value_1>
    /// <value_2> NONE</value_2>
    /// </Eatable>
    /// <Weapon>
    /// <value_0> WEAPON_MODEL </value_0>
    /// <value_1> WEAPON_AMMO </value_1>
    /// <value_2> NONE</value_2>
    /// </Weapon>
    /// <Armor>
    /// <value_0> ARMOR_INCREASE_VALUE </value_0>
    /// <value_1> NONE</value_1>
    /// <value_2> NONE</value_2>
    /// </Armor>
    /// <Drug>
    /// <value_0> DRUNK_INCREASE_VALUE </value_0>
    /// <value_1> NONE</value_1>
    /// <value_2> NONE</value_2>
    /// </Drug>
    /// <Ammo>
    /// <value_0> NONE </value_0>
    /// <value_1> NONE</value_1>
    /// <value_2> NONE</value_2>
    /// </Ammo>
    /// <License>
    /// <value_0> NONE </value_0>
    /// <value_1> Type 0 -Car | 1 -Bike | 2 -Sea | 3 -Plane </value_1>
    /// <value_2> NONE</value_2>
    /// </License>
    public enum ItemType
    {
        None,
        Drinkable,
        Eatable,
        FirstAid,
        Weapon,
        Armor,
        Drug,
        Ammo,
        WeaponPaint,
        License,
        Skin,
        Bag,
        RepairPart,
        Furniture,
        Phone,
        CraftingPart,
        CraftingTable,
        Wearable
    }
    public enum SellType
    {
        vehicle,
        house,
        metalparts
    }
    public class Item
    {
        [XmlAttribute("ID")]
        public int ID { get; set; }
        [XmlAttribute("MaxStack")]
        public int MaxCount { get; set; } = 1;
        [XmlAttribute("Name")]
        public string Name { get; set; }
        [XmlElement("Description")]
        public string Description { get; set; }
        [XmlAttribute("Type")]
        public ItemType Type { get; set; }
        [XmlAttribute("ObjectID")]
        public int ObjectId { get; set; } = 0;
        [XmlAttribute("Droppable")]
        public bool Droppable { get; set; } = true;
        [XmlAttribute("Value_0")]
        public string Value_0 { get; set; }
        [XmlAttribute("Value_1")]
        public string Value_1 { get; set; }
        [XmlAttribute("Value_2")]
        public string Value_2 { get; set; }
    }

    [XmlRoot("AllItems")]
    public class ItemList
    {
        [XmlElement("Item")]
        public List<Item> Items { get; set; }
        public ItemList() { Items = new List<Item>(); }
    }
    #endregion



    #region StoreForPlayer
    public class ClientItem
    {
        //public int ClientItemId { get; set; }
        [XmlAttribute("ItemId")]
        public int ItemId { get; set; }
        [XmlAttribute("Count")]
        public int Count { get; set; }
        [XmlAttribute("SpecifiedValue")]
        public string SpecifiedValue { get; set; }  //License Owner, Phone etc.
        [XmlAttribute("Equipeed")]
        public bool Equipped { get; set; }
    }


    public class StorageList
    {
        public List<ClientItem> Items { get; set; }
        public StorageList(){ Items = new List<ClientItem>(); }
    }

    public class TradeModel
    {
        public string SellerSocialClubID { get; set; }
        public string BuyerSocialClubID { get; set; }
        public int OfferedItemIndex { get; set; }
        public int OfferedPrice { get; set; }
    }
    public class TradeSellModel
    {
        public SellType Type { get; set; }
        public int SellingObjId { get; set; }
        public string SellerSocialClubID { get; set; }
        public string BuyerSocialClubID { get; set; }
        public int OfferedPrice { get; set; }
    }

    //Model to serialize to JSON
    public class Inventory
    {
        public string OwnerSocialClubName { get; set; }
        public int MetalParts { get; set; }
        public int OtherParts { get; set; }
        public List<ClientItem> ItemList { get; set; } = new List<ClientItem>();
        public int InventoryMaxCapacity { get; set; } = 10;

        public bool IsItemExist(int itemId)
        {
            return ItemList.Any(x => x.ItemId == itemId);
        }
        public bool IsItemExist(ItemType _type)
        {
            return ItemList.Any(x => db_Items.GameItems[x.ItemId].Type==_type );
        }
        public bool IsItemExist(ItemType _type, string Value_2)
        {
            return ItemList.Any(x => db_Items.GameItems[x.ItemId].Type == _type && db_Items.GameItems[x.ItemId].Value_2 == Value_2);
        }
    }
    #endregion

    [XmlRoot("DroppedItem_List")]
    public class DroppedItemList
    {
        public List<DroppedItem> Items { get; set; }
        public DroppedItemList(){ Items = new List<DroppedItem>(); }
    }

    public class DroppedItem
    {
        [XmlIgnore]
        public GrandTheftMultiplayer.Server.Elements.Object ObjectInGame { get; set; }
        [XmlIgnore]
        public GrandTheftMultiplayer.Server.Elements.TextLabel LabelInGame { get; set; }
        [XmlAttribute("ID")]
        public int DroppedItemId { get; set; }
        [XmlAttribute("DroppedPlayer")]
        public string DroppedPlayerSocialClubName { get; set; }
        [XmlElement("ClientItem")]
        public ClientItem Item { get; set; }
        [XmlElement("Position")]
        public Vector3 SavedPosition { get; set; } = new Vector3();
        [XmlAttribute("Dimension")]
        public int SavedDim { get; set; } = 0;
        [XmlAttribute("Faction")]
        public int FactionId { get; set; } = 0;
        [XmlIgnore]
        public Vector3 Position { get {
                return LabelInGame.position;
            }
            set{
                LabelInGame.position = value;
                ObjectInGame.position = value;
            }

        }
    }



    public enum Application
    {
        [Description("HARİTALAR")]
        GPS,
        [Description("Mesajlaşma")]
        Mesajlaşma,
        Bankacılık,
        [Description("Sosyal Medya")]
        SosyalMedya,
        Radyo,
        Emlakçılık,
        Reklamcılık,
        [Description("E-Ticaret")]
        Eticaret,
    }
    
    public enum Operator
    {
        Vodacell,
        LosTelecom
    }
    //Model to serialize in SpecifiedValue for Phone
    public class SpecifiedValuePhone
    {
        public Operator? PhoneOperator { get; set; } = null;
        public string PhoneNumber { get; set; }
        public bool FlightMode { get; set; } = false;
        public int Balance { get; set; } = 0;
        public int InternetBalance { get; set; } = 0;
        public DateTime LastPaidInternet { get; set; }
        public bool AutoInternetPay { get; set; } = false;
        public int Frequence { get; set; } = -1;
        public List<Application> Applications { get; set; }
        public Dictionary<string, string> Contacts { get; set; } = new Dictionary<string, string>();
    }

    public class SpecifiedValueWeapon
    {
        public int Ammo { get; set; } = 0;
        public WeaponTint WeaponTint { get; set; } = WeaponTint.Normal;

        public SpecifiedValueWeapon()
        {
            Ammo = 0;
            WeaponTint = WeaponTint.Normal;
        }
    }

}
