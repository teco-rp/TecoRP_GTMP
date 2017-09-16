using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TecoRP.Models
{
    public class Advertisement
    {
        [XmlAttribute("ID")]
        public int AddvertisementID { get; set; }
        [XmlAttribute("Owner")]
        public string OwnerSocialClubId { get; set; }
        [XmlElement("Owner")]
        public string Text { get; set; }
        [XmlAttribute("Times")]
        public int Times { get; set; } = 1;
        public Advertisement(){}
        public Advertisement(string ownerSocilClubID, string text, int times = 1)
        {
            OwnerSocialClubId = ownerSocilClubID; Text = text; Times = times;
        }
    }

    [XmlRoot("Advertisements_List")]
    public class AdvertisementsList
    {
        [XmlElement("Advertisement")]
        public List<Advertisement> Advertisements { get; set; }
        public AdvertisementsList() { Advertisements = new List<Advertisement>(); }
    }
}
