using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Shared.Math;

namespace TecoRP.Models
{
    public class Hospital
    {
        public Vector3 InjuredDeliverPosition { get; set; } = new Vector3(-450, -340, 34);
        public int InjuredDeliverDimension { get; set; } = 0;
        public Vector3 RespawnPoint { get; set; } = new Vector3(-450, -340, 34);
        public int RespawnDimension { get; set; } = 0;
        public int RespawnPrice { get; set; } = 250;
    }
    public class PhoneTicket
    {
        public int ID { get; set; }
        public string OwnerSocialClubID { get; set; }
        public string Text { get; set; }
        public Vector3 Position { get; set; } 
    }
    
}
