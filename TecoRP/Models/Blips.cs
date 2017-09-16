using GrandTheftMultiplayer.Shared.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TecoRP.Models
{
    [XmlRoot("BlipList")]
    public class BlipList
    {
        [XmlElement("Blip")]
        public List<Blip> Items { get; set; }
        public BlipList() { Items = new List<Models.Blip>(); }
    }
    public class Blip
    {
        [XmlElement("ID")]
        public int ID { get; set; }
        [XmlElement("Position")]
        public Vector3 Position { get; set; }
        [XmlElement("Name")]
        public string Name { get; set; }
        [XmlElement("ModelId")]
        public int ModelId { get; set; }
        [XmlElement("Color")]
        public int Color { get; set; }
        [XmlElement("Scale")]
        public int Scale { get; set; }
        [XmlElement("Range")]
        public int Range { get; set; }
        [XmlElement("Dimension")]
        public int Dimension { get; set; }
    }
}
