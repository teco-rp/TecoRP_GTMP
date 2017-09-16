using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Server.Managers;
using GrandTheftMultiplayer.Shared.Math;

namespace TecoRP.Models
{    
    public class LicenseTaking
    {
        [XmlAttribute("ID")]
        public int ID { get; set; }
        [XmlAttribute("Type")]
        public int LicenseType { get; set; }
        [XmlElement("Pedastrian")]
        public PedHash Ped { get; set; }
        [XmlElement("Position")]
        public Vector3 Position { get; set; }
        [XmlElement("Rotation")]
        public Vector3 Rotation { get; set; }
        [XmlElement("Dimension")]
        public int Dimension { get; set; }
        [XmlAttribute("Price")]
        public int Price { get; set; }
        [XmlElement("Text")]
        public string Text { get; set; }
    }

    [XmlRoot("LicenseTaking_List")]
    public class LicenseTakingsList
    {
        [XmlElement("LicenseTaking")]
        public List<LicenseTaking> Items { get; set; }
        public LicenseTakingsList(){ Items = new List<LicenseTaking>(); }
    }

    public class VehLicenseCheckpoint
    {
        [XmlIgnore]
        public CylinderColShape ColshapeOnMap { get; set; }
        [XmlAttribute("ID")]
        public int ID { get; set; }
        [XmlElement("Pos")]
        public Vector3 Position { get; set; }
        [XmlAttribute("Dim")]
        public int Dimension { get; set; }
    }

    [XmlRoot("VehLicenseCheckpoint_List")]
    public class VehicleLicenseCheckpointList
    {
        [XmlElement("Checkpoint")]
        public List<VehLicenseCheckpoint> Items { get; set; }
        public VehicleLicenseCheckpointList(){ Items = new List<Models.VehLicenseCheckpoint>(); }
    }
}
