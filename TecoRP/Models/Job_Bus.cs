using GrandTheftMultiplayer.Shared.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TecoRP.Models
{
    public class BusStop
    {
        [XmlAttribute("ID")]
        public int ID { get; set; }
        [XmlElement("Position")]
        public Vector3 Position { get; set; }
        [XmlAttribute("Dimension")]
        public int Dimension { get; set; }
        [XmlElement("MaxMoney")]
        public int MaxMoney { get; set; } = 25;
        [XmlElement("MinMoney")]
        public int MinMoney { get; set; } = 5;
    }

    [XmlRoot("BusStop_List")]
    public class BusStopList
    {
        [XmlElement("BusStop")]
        public List<BusStop> Items { get; set; }
        public BusStopList(){ Items = new List<BusStop>(); }
    }
}
