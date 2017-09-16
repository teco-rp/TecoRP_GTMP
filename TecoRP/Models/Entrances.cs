using GrandTheftMultiplayer.Shared.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TecoRP.Models
{

    public class Entrance
    {
        [XmlElement("ID")]
        public int ID { get; set; }
        [XmlElement("Name")]
        public string Name { get; set; }
        [XmlElement("EntrancePos")]
        public Vector3 EntrancePosition { get; set; }
        [XmlElement("Interior")]
        public Vector3 InteriorPosition { get; set; }
        [XmlElement("Rotation")]
        public Vector3 Rotation { get; set; }
        [XmlElement("Direction")]
        public Vector3 Direction { get; set; }
        [XmlElement("InteriorDimension")]
        public int InteriorDimension { get; set; }
        [XmlElement("EntranceDimension")]
        public int EntranceDimension { get; set; }
        [XmlElement("MarkerType")]
        public int MarkerType { get; set; }
        [XmlElement("Scale")]
        public float Scale { get; set; } = 1;
        [XmlElement("Color")]
        public MarkerColor Color { get; set; }
    }

    public class MarkerColor
    {
        [XmlAttribute]
        public int Red { get; set; }
        [XmlAttribute]
        public int Green { get; set; }
        [XmlAttribute]
        public int Blue { get; set; }
        [XmlAttribute]
        public int Alpha { get; set; }
    }
    [XmlRoot("Entrance_List")]
    public class EntranceList
    {
        [XmlElement("Entrance")]
        public List<Entrance> Items { get; set; }
        public EntranceList() { Items = new List<Models.Entrance>(); }
    }
}

