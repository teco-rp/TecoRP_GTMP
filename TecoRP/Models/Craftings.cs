using System.Collections.Generic;
using System.Xml.Serialization;
using GrandTheftMultiplayer.Shared.Math;

namespace TecoRP.Models
{
    public class CraftingTablesOnMap
    {
        [XmlIgnore]
        public GrandTheftMultiplayer.Server.Elements.Object TableOnMap { get; set; }
        [XmlIgnore]
        public GrandTheftMultiplayer.Server.Elements.TextLabel TextLabelOnMap { get; set; }

        [XmlAttribute("TableOnMapId")]
        public int TableOnMapId { get; set; }
        [XmlAttribute("TableModel")]
        public int CraftingTableModelId { get; set; }
        [XmlAttribute("name")]
        public string Name { get; set; }
        [XmlElement("Owner")]
        public string OwnerSocialClubID { get; set; }
        [XmlElement("Position")]
        public Vector3 Position { get; set; }
        [XmlElement("Rotation")]
        public Vector3 Rotation { get; set; }
        [XmlAttribute("Dim")]
        public int Dimension { get; set; }
    }
    [XmlRoot("CraftingTablesOnMap_List")]
    public class CraftingTablesOnMapList
    {
        [XmlElement("CraftingTableOnMap")]
        public List<CraftingTablesOnMap> TablesOnMap { get; set; }
        public CraftingTablesOnMapList(){ TablesOnMap = new List<Models.CraftingTablesOnMap>(); }
    }

    public class CraftingTable
    {
        [XmlAttribute("ID")]
        public int CraftingTableId { get; set; }
        [XmlAttribute("Name")]
        public string Name { get; set; }
        [XmlAttribute("Object")]
        public int ObjectId { get; set; } = 0;
        [XmlElement("Craft")]
        public List<CraftingItem> Craftings { get; set; }
        public override string ToString()
        {
            return $"({CraftingTableId}) - {Name} ";
        }
    }
    [XmlRoot("CraftingTable_List")]
    public class CraftingTableList
    {
        [XmlElement("CraftingTable")]
        public List<CraftingTable> Tables { get; set; }
        public CraftingTableList() { Tables = new List<CraftingTable>(); }
    }

    public class CraftingItem
    {
        [XmlAttribute("CraftedItem")]
        public int CraftedGameItemId { get; set; }
        [XmlAttribute("RequiredMetalPart")]
        public int RequiredMetalPart { get; set; }
        [XmlElement("RequiredItem")]
        public List<int> RequredItemIds { get; set; }
        [XmlAttribute("Job")]
        public int RequiredJob { get; set; } = 0;
        [XmlAttribute("JobLevel")]
        public int RequiredJobLevel { get; set; } = 0;
        public CraftingItem(){ RequredItemIds = new List<int>(); }
        public override string ToString()
        {
            return "ItemID : " + CraftedGameItemId.ToString();
        }
    }

}
