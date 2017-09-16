using GrandTheftMultiplayer.Shared.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TecoRP.Models
{
    public class TirDeliveryPoint
    {
        [XmlAttribute("ID")]
        public int ID { get; set; }
        [XmlAttribute("Name")]
        public string Name { get; set; }
        [XmlElement("Position")]
        public Vector3 DeliveryPointPosition { get; set; }
        [XmlAttribute("Dimension")]
        public int DeliveryPointDimension { get; set; }
        [XmlAttribute("Money")]
        public int DeliveryPointMoney { get; set; }

    }
    [XmlRoot("TirDeliveryPoints_List")]
    public class TirDeliveryPointsList
    {
        [XmlElement("DeliveryPoint")]
        public List<TirDeliveryPoint> Items { get; set; }
        public TirDeliveryPointsList(){ Items = new List<Models.TirDeliveryPoint>(); }
    }

}
