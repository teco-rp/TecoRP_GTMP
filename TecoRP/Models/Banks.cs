using GrandTheftMultiplayer.Shared.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TecoRP.Models
{
    public enum BankType
    {
        Bank,
        ATM
    }
    public class Bank
    {
        [XmlAttribute("ID")]
        public int BankId { get; set; }
        [XmlElement("BankType")]
        public BankType TypeOfBank { get; set; }
        [XmlElement("Rotation")]
        public Vector3 Rotation { get; set; }
        [XmlElement("Position")]
        public Vector3  Position { get; set; }
        [XmlAttribute("Dimension")]
        public int Dimension { get; set; }
        [XmlAttribute("MoneyCount")]
        public int MoneyCountInInside { get; set; } = 1500;
    }

    [XmlRoot("Bank_List")]
    public class BankList
    {
        [XmlElement("Bank")]
        public  List<Bank> Items { get; set; }
        public BankList(){ Items = new List<Models.Bank>(); }
    }
}
