using GrandTheftMultiplayer.Shared.Math;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using TecoRP.Models;

namespace TecoRP_Debugger.Models
{
    public class Shop
    {
        //-----------------------------------------------------
        [JsonProperty(PropertyName = "ID")]
        public int ShopId { get; set; }
        //-----------------------------------------------------
        [JsonProperty(PropertyName = "t")]
        public ShopType Type { get; set; }
        //-----------------------------------------------------
        [JsonProperty(PropertyName = "bid")]
        public int? BusinessId { get; set; }
        //-----------------------------------------------------
        [JsonProperty(PropertyName = "pos")]
        public Vector3 Position { get; set; }
        //-----------------------------------------------------
        [JsonProperty(PropertyName = "rot")]
        public Vector3 Rotation { get; set; }
        //-----------------------------------------------------
        [JsonProperty(PropertyName = "d")]
        public int Dimension { get; set; }
        //-----------------------------------------------------
        [JsonProperty(PropertyName = "s")]
        public Vector3 Scale { get; set; }
        //-----------------------------------------------------
        [JsonProperty(PropertyName = "r")]
        public int Range { get; set; } = 5;
        //-----------------------------------------------------
        [JsonProperty(PropertyName = "mt")]
        public int MarkerType { get; set; } = 1;
        //-----------------------------------------------------
        [JsonProperty(PropertyName = "c")]
        public MarkerColor MarkerColorRGB { get; set; }
        //-----------------------------------------------------
        [JsonProperty(PropertyName = "items")]
        public List<SaleItem> SaleItemList { get; set; }

    }
    public class SaleItem
    {
        [JsonProperty(PropertyName = "gid")]
        public int GameItemId { get; set; }
        [JsonProperty(PropertyName = "p")]
        public int Price { get; set; } = 10;
        [JsonProperty(PropertyName = "c")]
        public int Count { get; set; } = 1;
    }
    public enum ShopType
    {
        Regular,
        Clothes
    }
}
