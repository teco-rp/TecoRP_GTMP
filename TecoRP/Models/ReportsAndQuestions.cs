using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TecoRP.Models
{
    public enum ReportType
    {
        Report,
        Question
    }

    public class Report
    {
        [XmlAttribute("ID")]
        public int ReportID { get; set; }
        [XmlAttribute("Owner")]
        public string OwnerSocialClubID { get; set; }
        [XmlAttribute("Type")]
        public ReportType Type { get; set; }
        [XmlAttribute("DateTime")]
        public DateTime RegisterDate { get; set; }
        [XmlAttribute("Text")]
        public string ReportText { get; set; }
  
    }

    [XmlRoot("Reports_List")]
    public class ReportList
    {
        [XmlElement("Reports")]
        public List<Report> Reports { get; set; }
        public ReportList(){ Reports = new List<Report>(); }
    }
}
