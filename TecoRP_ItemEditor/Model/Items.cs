using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TecoRP.Models;

namespace TecoRP_ItemEditor.Model
{
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
    /// <value_1> Type (Örn.: 0-Car, 1-Plane 2-Bike vs...) </value_1>
    /// <value_2> NONE</value_2>
    /// </License>
    /// <Skin>
    /// <value_0> SKIN_NAME </value_0>
    /// <value_1> NONE </value_1>
    /// <value_2> NONE </value_2>
    /// </Skin>
    /// <Bag>
    /// <value_0> CAPACITY </value_0>
    /// <value_1> NONE </value_1>
    /// <value_2> NONE</value_2>
    /// </Bag>
    /// <RepairPart>
    /// <value_0> NONE </value_0>
    /// <value_1> TYPE (Örn.: 0-Tyre 1-Engine) </value_1>
    /// <value_2> NONE</value_2>
    /// </RepairPart>
    
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

        public override string ToString()
        {
            return Name;
        }
    }

    public class Inventory
    {
        [XmlIgnore]
        public string OwnerSocialClubName { get; set; }
        public int MetalParts { get; set; }
        public int OtherParts { get; set; }
        public List<Item> ItemList { get; set; }
        public int InventoryMaxCapacity { get; set; } = 10;
    }

    public class InventoryList
    {
        public List<Inventory> Items { get; set; }
        public InventoryList() { Items = new List<Inventory>(); }
    }
}
