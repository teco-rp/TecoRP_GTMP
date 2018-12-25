using GrandTheftMultiplayer.Shared.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TecoRP
{
    public class Constants
    {
        public const bool IS_LOGIN_ACTIVE = true;
        public const int MAX_CHARACTERS = 3;
        public const int STARTING_MONEY = 1250;
        public static Vector3 STARTING_POS => new Vector3 { X = -801, Y = -102, Z = 37 };
    }
}
