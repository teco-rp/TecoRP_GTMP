using GrandTheftMultiplayer.Shared.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TecoRP.Models
{
    public class Shop
    {
        [XmlAttribute("ID")]
        public int ShopId { get; set; }
        [XmlElement("BusinessId")]
        public Nullable<int> BusinessId { get; set; }
        [XmlElement("Position")]
        public Vector3 Position { get; set; }
        [XmlElement("Rotation")]
        public Vector3 Rotation { get; set; }
        [XmlAttribute("Dimension")]
        public int Dimension { get; set; }
        [XmlElement("Scale")]
        public Vector3 Scale { get; set; }
        [XmlAttribute("Range")]
        public int Range { get; set; } = 5;
        [XmlElement("Type")]
        public int MarkerType { get; set; } = 1;
        [XmlElement("Color")]
        public MarkerColor MarkerColorRGB { get; set; }
        
        [XmlElement("SaleItem")]
        public List<SaleItem> SaleItemList { get; set; }

    }
    public class SaleItem
    {
        [XmlAttribute("GameItemID")]
        public int GameItemId { get; set; }
        [XmlAttribute("Price")]
        public int Price { get; set; } = 10;
        [XmlAttribute("Count")]
        public int Count { get; set; } = 1;
    }

    [XmlRoot("Shop_List")]
    public class ShopList
    {
        [XmlElement("Shop")]
        public List<Shop> Items { get; set; }
        public ShopList(){ Items = new List<Shop>(); }
    }
}
