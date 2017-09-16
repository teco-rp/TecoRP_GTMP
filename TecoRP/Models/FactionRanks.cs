using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TecoRP.Models
{
    public class FactionRank
    {
        [XmlAttribute("FactionId")]
        public int FactionId { get; set; }
        [XmlElement("Rank")]
        public List<Rank> Ranks { get; set; }
        public FactionRank(){ Ranks = new List<Models.Rank>(); }
            //{
            //    new Rank { RankLevel=3, RankName ="Başkan" },
            //    new Rank { RankLevel=2, RankName ="Yardımcı" },
            //    new Rank { RankLevel=1, RankName ="Üye" }
            //}
            

    }
    public class Rank
    {
        [XmlAttribute("Level")]
        public int RankLevel { get; set; }
        [XmlAttribute("Name")]
        public string RankName { get; set; }
    }

    [XmlRoot("FactionRank_List")]
    public class FactionRankList
    {
        [XmlElement("FactionRank")]
        public List<FactionRank> Items { get; set; }
        public FactionRankList(){ Items = new List<FactionRank>(); }
    }
}
