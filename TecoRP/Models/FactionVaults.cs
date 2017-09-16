using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;

namespace TecoRP.Models
{
    public class FactionVault
    {
        [XmlIgnore]
        public TextLabel TextLabelOnMap { get; set; }
        [XmlAttribute("ID")]
        public int VaultId { get; set; }
        [XmlAttribute("Faction")]
        public int FactionId { get; set; }
        [XmlAttribute("Text")]
        public string Text { get; set; }
        [XmlElement("Position")]
        public Vector3 Position { get; set; }
        [XmlAttribute("Dimension")]
        public int Dimension { get; set; }
        [XmlElement("VaultItems")]
        public List<VaultItem> VaultItems { get; set; }
        public FactionVault(){ VaultItems = new List<VaultItem>(); }
    }

    public class VaultItem
    {
        [XmlAttribute("GameItemID")]
        public int GameItemID { get; set; }
        [XmlAttribute("MinRank")]
        public int MinRankLevel { get; set; }
    }

    [XmlRoot("FactionVaults_List")]
    public class FactionVaultsList
    {
        [XmlElement("FactionVault")]
        public List<FactionVault> Items { get; set; }
        public FactionVaultsList(){ Items = new List<FactionVault>(); }
    }
}
