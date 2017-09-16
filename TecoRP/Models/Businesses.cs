using GrandTheftMultiplayer.Shared.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GrandTheftMultiplayer.Server.Elements;

namespace TecoRP.Models
{
    public class Business
    {
        [XmlIgnore]
        public TextLabel LabelOnMap{ get; set; }
        [XmlAttribute("ID")]
        public int BusinessId { get; set; }
        [XmlAttribute("Building")]
        public int BuildingId { get; set; }
        [XmlAttribute("Name")]
        public string BusinessName { get; set; }
        [XmlElement("Owner")]
        public string OwnerSocialClubName { get; set; }
        [XmlAttribute("Closed")]
        public bool IsClosed { get; set; }
        [XmlAttribute("IsOnSale")]
        public bool IsSelling { get; set; }
        [XmlElement("Position")]
        public Vector3 Position { get; set; }
        [XmlAttribute("Dim")]
        public int Dimension { get; set; }
        [XmlElement("InteriorPosition")]
        public Vector3 InteriorPosition { get; set; }
        [XmlAttribute("interiorDim")]
        public int InteriorDimension { get; set; }
        [XmlAttribute("Price")]
        public int Price { get; set; }        
        [XmlElement("Vault")]
        public int VaultMoney { get; set; } = 0;
        [XmlElement("MaxVault")]
        public int MaxVaultMoney { get; set; } = 10000;
        [XmlElement("MinutesPast")]
        public int MinutesPast { get; set; } = 0;
    }

    [XmlRoot("Business_List")]
    public class BusinessList
    {
        [XmlElement("Business")]
        public List<Business> Items { get; set; }
        public BusinessList() { Items = new List<Business>(); }
    }
}
