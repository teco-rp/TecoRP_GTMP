using GrandTheftMultiplayer.Shared.Math;
using System.Collections.Generic;
using System.Xml.Serialization;

namespace TecoRP.Models
{
    public  class Job 
    {
        [XmlAttribute("ID")]
        public int ID { get; set; }
        [XmlAttribute("Name")]
        public string Name { get; set; }
        [XmlElement("Position")]
        public Vector3 Position { get; set; }
        [XmlAttribute("Dimension")]
        public int Dimension { get; set; }
        [XmlElement("TakingPosition")]
        public Vector3 TakingPosition { get; set; }
        [XmlElement("TakingDimension")]
        public int TakingDimension { get; set; } = 0;
        [XmlAttribute("Range")]
        public int Range { get; set; }
        [XmlAttribute("JobId")]
        public int JobId { get; set; }
    }
    [XmlRoot("Job_List")]
    public class JobList
    {
        [XmlElement("Job")]
        public List<Job> Items { get; set; }
        public JobList(){ Items = new List<Job>(); }
    }
}
