using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using GrandTheftMultiplayer.Shared.Math;

namespace TecoRP.Models
{
    public enum HouseElementType
    {
        Refrigeretor,
        Wardrobe,
        Bed,
        Tv,
        Shower,
        Toilet,
        Vault
    }
    public class HouseElements
    {
        public int HouseElementID { get; set; }
        public HouseElementType Type { get; set; }
        public Vector3  Position{ get; set; }
        public int Dimension { get; set; }
        public string SpecifiedValue { get; set; }
    }

    public class SpecifiedValueStore
    {
        List<Tuple<int,ClientItem>> ItemsWithDimInside { get; set; }
        
    }
}
