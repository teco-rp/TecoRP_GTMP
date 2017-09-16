using GrandTheftMultiplayer.Shared.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TecoRP.Models
{
    public class PhoneOperatorShop 
    {
        [XmlAttribute("ID")]
        public int ID { get; set; }
        [XmlElement("Position")]
        public Vector3 Position { get; set; }
        [XmlAttribute("Dimension")]
        public int Dimension { get; set; }
        [XmlAttribute("Operator")]
        public Operator OperatorType { get; set; }
    }
    [XmlRoot("PhoneOperatorShop_List")]
    public class PhoneOperatorShopList
    {
        [XmlElement("OperatorShop")]
        public List<PhoneOperatorShop> Items { get; set; }
        public PhoneOperatorShopList(){ Items = new List<Models.PhoneOperatorShop>(); }
    }
}
