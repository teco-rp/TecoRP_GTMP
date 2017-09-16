using GrandTheftMultiplayer.Shared.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GrandTheftMultiplayer.Server.Elements;
using TecoRP.Database;

namespace TecoRP.Models
{
    public class House
    {
        [XmlIgnore]
        public Marker MarkerOnMap { get; set; }
        [XmlIgnore]
        public TextLabel LabelOnMap { get; set; }
        [XmlAttribute("ID")]
        public int HouseId { get; set; }
        [XmlAttribute("Owner")]
        public string OwnerSocialClubName { get; set; } = null;
        [XmlAttribute("IsInBuilding")]
        public bool IsInBuilding { get; set; }
        [XmlElement("Name")]
        public string Name { get; set; } = "Ev";
        [XmlElement("Position")]
        public Vector3 EntrancePosition { get; set; }
        [XmlElement("Rotation")]
        public Vector3 EntranceRotation { get; set; }
        [XmlElement("Dimension")]
        public int EntranceDimension { get; set; } = 0;
        [XmlElement("InteriorPosition")]
        public Vector3 InteriorPosition { get; set; }
        [XmlElement("InteriorRotation")]
        public Vector3 InteriorRotation { get; set; }
        [XmlElement("InteriorDimension")]
        public int InteriorDimension { get; set; }
        [XmlElement("MarkerType")]
        public int MarkerType { get; set; } = 0;
        [XmlElement("Locked")]
        public bool IsLocked { get; set; } = false;
        [XmlElement("IsSelling")]
        public bool IsSelling { get; set; }
        [XmlElement("Price")]
        public int Price { get; set; }

    }

    [XmlRoot("Houses_List")]
    public class HouseList
    {
        [XmlElement("House")]
        public List<House> Items { get; set; }
        public HouseList() { Items = new List<House>(); }
    }

    public class HouseMarkerColor
    {
        public ColorRGB SaleColor { get; set; } = new ColorRGB(10, 255, 10);
        public ColorRGB NormalColor { get; set; } = new ColorRGB(255, 255, 255);
    }
    public class ColorRGB
    {
        public ColorRGB(int r, int g, int b)
        {
            Red = r; Green = g; Blue = b;
        }
        public int Red { get; set; }
        public int Green { get; set; }
        public int Blue { get; set; }
    }
    

}
