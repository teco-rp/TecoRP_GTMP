using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using GrandTheftMultiplayer.Shared.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TecoRP.Models
{

    public class User
    {
        [XmlIgnore]
        public string socialClubName { get; set; }
        [XmlIgnore]
        public string Password { get; set; }
        public int FactionId { get; set; } = 0;
        public int JobId { get; set; } = 0;
        public int Dimension { get; set; } = 0;
        public int Mission { get; set; } = 0;
        public int FactionRank { get; set; } = 1;
        public string CharacterName { get; set; }
        public bool Gender { get; set; }
        public DateTime BirthDate { get; set; }
        public string Origin { get; set; }
        public int WantedLevel { get; set; } = 0;
        public float Hunger { get; set; } = 100;
        public float Thirsty { get; set; } = 100;
        public byte AdminLevel { get; set; } = 0;
        public PedHash Skin { get; set; }
        public int? Level { get; set; } = 1;
        public int playingMinutes { get; set; } = 0;
        public Vector3 LastPosition { get; set; }
        public int Money { get; set; } = 0;
        public int? BankMoney { get; set; }
        public string BankAccount { get; set; }
        public int PastBankMinutes { get; set; } = 0;
        public string FingerPrint { get; set; }
        public int ArmorLevel { get; set; } = 0;
        public int HealthLevel { get; set; } = 100;
        public bool Jailed { get; set; } = false;
        public int? JailedTime { get; set; }
        public bool Dead { get; set; } = false;
        public int DeadSeconds { get; set; } = 0;
        public bool Cuffed { get; set; } = false;
        public List<Jobs> JobAbilities { get; set; }
        public ClothingData ClothingData { get; set; } = new ClothingData();

        public User() { JobAbilities = new List<Jobs>(); }
        public int GetAge()
        {
            if (BirthDate!=null)
            {
                return DateTime.Now.Year - BirthDate.Year;
            }
            return 0;
        }
        public string GetFaction()
        {
            return Managers.FactionManager.ToFactionName(FactionId);
        }
        public string GetJob()
        {
            return Managers.JobManager.ToJobName(JobId);
        }
    }

    public class ClothingData
    {
        public int Head { get; set; }
        public int Eyes { get; set; }
        public int Hair { get; set; }
        public int HairColor { get; set; }
    }

    public class Jobs
    {
        public int JobID { get; set; }
        public int JobLevel { get; set; } = 1;
        public int JobsCompleted { get; set; } = 0;
        public string GetJobName()
        {
            return Managers.JobManager.ToJobName(JobID);
        }
    }

    [XmlRoot("Whitelist")]
    public class WhiteList
    {
        [XmlElement("IsEnabled")]
        public bool IsEnabled { get; set; } = true;
        [XmlElement("User")]
        public List<WhiteListUser> Users { get; set; }
        public WhiteList() { Users = new List<WhiteListUser>(); }
    }
    public class WhiteListUser
    {
        [XmlAttribute]
        public string SocialClubName { get; set; }
        [XmlElement]
        public DateTime LastValidateTime { get; set; } = DateTime.Now.AddYears(2);
    }


}
