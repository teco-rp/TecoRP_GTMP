using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Shared.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TecoRP.Models
{
    public class GasStation
    {
        [XmlAttribute("ID")]
        public int GasStationId { get; set; }
        [XmlElement("ConnectedBusinessId")]
        public Nullable<int> BusinessId { get; set; }
        [XmlElement("Position")]
        public Vector3 Position { get; set; }
        [XmlAttribute("Dimension")]
        public int Dimension { get; set; }
        [XmlAttribute("PricePerUnit")]
        public float PricePerUnit { get; set; }
        [XmlAttribute("GasInStock")]
        public float GasInStock { get; set; }
        [XmlAttribute("MaxGasStock")]
        public float MaxGasInStock { get; set; }
        [XmlAttribute("CompletedMoney")]
        public int CompletedMoney { get; set; }
    }
    [XmlRoot("GasStation_List")]
    public class GasStationList
    {
        [XmlElement("GasStation")]
        public List<GasStation> Items { get; set; }

    }
}
