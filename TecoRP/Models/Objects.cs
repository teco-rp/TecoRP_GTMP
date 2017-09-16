using GrandTheftMultiplayer.Shared.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TecoRP.Models
{
    public class GameObject
    {
        [XmlElement("ObjectId")]
        public long ID { get; set; }
        [XmlElement("Dimension")]
        public int Dimension { get; set; } = 0;
        [XmlElement("ObjectModel")]
        public int ModelId { get; set; }

        [XmlElement("Rotation")]
        public Vector3 Rotation { get; set; }
        [XmlElement("FirstPos")]
        public Vector3 FirstPosition { get; set; }
        [XmlElement("SecondPos")]
        public Vector3 LastPosition { get; set; }
        [XmlElement("Owner")]
        public string OwnerSocialClubName { get; set; } = "";
        [XmlElement("Faction")]
        public int FactionId { get; set; } = 0;
    }
    [XmlRoot("Objects_List")]
    public class GameObjectList
    {
        [XmlElement("Object")]
        public List<GameObject> Items { get; set; }
        public GameObjectList(){ Items = new List<GameObject>();    }
    }
}
