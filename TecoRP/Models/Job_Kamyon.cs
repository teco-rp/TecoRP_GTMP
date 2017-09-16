using GrandTheftMultiplayer.Shared.Math;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TecoRP.Models
{
    public class KamyonDeliveryPoint
    {
        [XmlAttribute("ID")]
        public int ID { get; set; }
        [XmlAttribute("Name")]
        public string Name { get; set; }
        [XmlElement("Position")]
        public Vector3 DeliveryPoint { get; set; }
        [XmlAttribute("Dimension")]
        public int DeliveryDimension { get; set; } = 0;
        [XmlElement("Type")]
        public DeliveryType Type { get; set; } = DeliveryType.Money;
        [XmlElement("Value")]
        public int CompletedValue { get; set; } = 0;
    }

    public enum DeliveryType
    {
        [Description("Para Karşılığı")]
        Money,
        [Description("Metal Parça Karşılığı")]
        MetalParts
    }
    [XmlRoot("KamyonDelivery_List")]
    public class KamyonDeliveryList
    {
        [XmlElement("DeliveryPoint")]
        public List<KamyonDeliveryPoint> Items { get; set; }
        public KamyonDeliveryList(){ Items = new List<KamyonDeliveryPoint>(); }
    }
}
