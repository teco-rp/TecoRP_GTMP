using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GrandTheftMultiplayer.Shared.Math;

namespace TecoRP.Models
{
    public class FactionInteractive
    {
        [XmlIgnore]
        public GrandTheftMultiplayer.Server.Elements.TextLabel LabelOnMap { get; set; }
        [XmlAttribute("ID")]
        public int InteractiveID { get; set; }
        [XmlAttribute("FactionId")]
        public int Faction { get; set; }
        [XmlElement("Position")]
        public Vector3 Position { get; set; }
        [XmlAttribute("Dim")]
        public int Dimension { get; set; }
        [XmlAttribute("Name")]
        public string Name { get; set; }
    }

    [XmlRoot("FactionInteractive_List")]
    public class FactionInteractivesList
    {
        public List<FactionInteractive> Interactives { get; set; }
        public FactionInteractivesList(){ Interactives = new List<Models.FactionInteractive>(); }
    }
}
