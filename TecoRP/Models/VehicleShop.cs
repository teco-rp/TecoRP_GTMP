using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TecoRP.Models
{
    public class VehicleShop
    {
        [JsonIgnore]
        public GrandTheftMultiplayer.Server.Elements.Marker MarkerOnMap { get; set; }
        [JsonProperty("id")]
        public int VehicleShopId { get; set; }
        [JsonProperty("pos")]
        public Vector3 Position { get; set; }
        [JsonProperty("rot")]
        public Vector3 Rotation { get; set; }

        [JsonProperty("d")]
        public int Dimension { get; set; }
        [JsonProperty("dPos")]
        public Vector3 DeliveryPosition { get; set; }
        [JsonProperty("dRot")]
        public Vector3 DeliveryRotation { get; set; }
        [JsonProperty("dd")]
        public int DeliveryDimension { get; set; }
        [JsonProperty("c")]
        public int VehicleClass { get; set; }
    }

    public class VehiclePrice
    {
        public VehiclePrice()
        {
        }

        public VehiclePrice(VehicleHash vehicle, int price)
        {
            Price = price;
            Vehicle = vehicle;
        }

        [JsonProperty("p")]
        public int Price { get; set; }
        [JsonConverter(typeof(StringEnumConverter))]
        [JsonProperty("m")]
        public VehicleHash Vehicle { get; set; }

    }

}
