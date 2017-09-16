using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TecoRP.Models
{
    public class PoliceDatabase
    {

    }

    public class Crime
    {
        [XmlAttribute("Owner")]
        public string OwnerSocialClubName { get; set; }
        [XmlElement("RegisterDate")]
        public int CrimesBefore { get; set; } = 0;
        [XmlElement("Crime")]
        public List<CrimeType> Crimes { get; set; }
        public Crime() { Crimes = new List<Models.CrimeType>(); }
    }

    public class CrimeType
    {
        [XmlAttribute("CrimeType")]
        public int CrimeTypeId { get; set; }
        [XmlAttribute("Name")]
        public string Name { get; set; }
        [XmlAttribute("WantedLevel")]
        public int WantedLevel { get; set; }
    }

    [XmlRoot("Crime_List")]
    public class CrimeList
    {
        [XmlElement("Crimes")]
        public List<Crime> Items { get; set; }
        public CrimeList() { Items = new List<Crime>(); }
    }

    [XmlRoot("CrimeType_List")]
    public class CrimeTypeList
    {
        [XmlElement("CrimeType")]
        public List<CrimeType> Items { get; set; }
        public CrimeTypeList(){ Items = new List<Models.CrimeType>(); }
    }

}

