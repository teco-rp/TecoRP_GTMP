using GrandTheftMultiplayer.Shared.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TecoRP.Models
{
    public class Arrest
    {
        [XmlAttribute("ID")]
        public int ArrestId { get; set; }
        [XmlAttribute("Name")]
        public string Name { get; set; }
        [XmlAttribute("Dimension")]
        public int Dimension { get; set; }
        [XmlElement("Position")]
        public Vector3 Position { get; set; }
        [XmlElement("Rotation")]
        public Vector3 Rotation { get; set; }
        [XmlElement("ArrestPoint")]
        public Vector3 ArrestPoint { get; set; }
        [XmlElement("ArrestPointDimension")]
        public int ArrestPointDimension { get; set; } = 0;
    }
    [XmlRoot("Arrest_List")]
    public class ArrestList
    {
        [XmlElement("Arrest")]
        public List<Arrest> Items { get; set; }
        public ArrestList(){ Items = new List<Models.Arrest>(); }
    }
}
