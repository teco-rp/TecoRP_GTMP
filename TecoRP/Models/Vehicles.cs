using GrandTheftMultiplayer.Shared.Math;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GrandTheftMultiplayer.Shared;
using Newtonsoft.Json;

namespace TecoRP.Models
{
    public class Vehicle
    {
        private StorageList _baggageItems = new StorageList();
        private StorageList _torpedoItems;

        [JsonIgnore]
        public GrandTheftMultiplayer.Server.Elements.Vehicle VehicleOnMap { get; set; }
        //-------------------------------------------------------------------
        [JsonProperty(PropertyName = "id")]
        public long VehicleId { get; set; }
        //-------------------------------------------------------------------
        [JsonProperty(PropertyName = "li")]
        public int Livery { get; set; } = 0;
        //-------------------------------------------------------------------
        [JsonProperty(PropertyName = "model")]
        public VehicleHash VehicleModelId { get; set; }
        //-------------------------------------------------------------------
        [JsonProperty(PropertyName = "owner")]
        public string OwnerSocialClubName { get; set; }
        //-------------------------------------------------------------------
        [JsonProperty(PropertyName = "pm")]
        public int PastMinutes { get; set; } = 0;
        //-------------------------------------------------------------------
        [JsonProperty(PropertyName = "tax")]
        public float Tax { get; set; } = 0;
        //-------------------------------------------------------------------
        [JsonProperty(PropertyName = "tb")]
        public bool IsBlockedForTax { get; set; } = false;
        //-------------------------------------------------------------------
        [JsonProperty(PropertyName = "p")]
        public string Plate { get; set; }
        //-------------------------------------------------------------------
        [JsonProperty(PropertyName = "f")]
        public int FactionId { get; set; } = -1;
        //-------------------------------------------------------------------
        [JsonProperty(PropertyName = "j")]
        public int JobId { get; set; } = -1;
        //-------------------------------------------------------------------
        [JsonProperty(PropertyName = "rt")]
        public DateTime RentedTime { get; set; }
        //-------------------------------------------------------------------
        [JsonProperty(PropertyName = "rOwner")]
        public string RentedPlayerSocialClubId { get; set; }
        //-------------------------------------------------------------------
        [JsonProperty(PropertyName = "c1")]
        public int Color1 { get; set; }
        //-------------------------------------------------------------------
        [JsonProperty(PropertyName = "c2")]
        public int Color2 { get; set; }
        //-------------------------------------------------------------------
        [JsonProperty(PropertyName = "mc1")]
        public int ModColor1 { get; set; }
        //-------------------------------------------------------------------
        [JsonProperty(PropertyName = "mc2")]
        public int ModColor2 { get; set; }
        //-------------------------------------------------------------------
        [JsonProperty(PropertyName = "mods")]
        public int[] Mods { get; set; } = new int[70];
        //-------------------------------------------------------------------
        [JsonProperty(PropertyName = "l")]
        public bool IsLocked { get; set; }
        //-------------------------------------------------------------------
        [JsonProperty(PropertyName = "fuel")]
        public float Fuel { get; set; }
        //-------------------------------------------------------------------
        [JsonProperty(PropertyName = "lp")]
        public Vector3 LastPosition { get; set; }
        //-------------------------------------------------------------------
        [JsonProperty(PropertyName = "lr")]
        public Vector3 LastRotation { get; set; }
        //-------------------------------------------------------------------
        [JsonProperty(PropertyName = "d")]
        public int Dimension { get; set; } = 0;
        //-------------------------------------------------------------------
        [JsonProperty(PropertyName = "b")]
        public StorageList BaggageItems { get => _baggageItems ?? new StorageList(); set => _baggageItems = value; }         //-------------------------------------------------------------------
        [JsonProperty(PropertyName = "mb")]
        public int MaxBaggageCount { get; set; } = 10;
        //-------------------------------------------------------------------
        [JsonProperty(PropertyName = "t")]
        public StorageList TorpedoItems { get => _torpedoItems ?? new StorageList(); set => _torpedoItems = value; }
        //-------------------------------------------------------------------
        [JsonProperty(PropertyName = "mt")]
        public int MaxTorpedoCount { get; set; } = 5;
        //-------------------------------------------------------------------
        [JsonProperty(PropertyName = "dg")]
        public SpecifiedValueForDamage SpecifiedDamage { get; set; } //JSON - SpecifiedValueForDamage
    }


    [XmlRoot("Vehicle_List")]
    public class VehicleList
    {
        [XmlElement("Vehicle")]
        public List<Vehicle> Items { get; set; }

        public VehicleList() { Items = new List<Vehicle>(); }
    }

  
    public class SpecifiedValueForDamage
    {
        public List<int> BrokenDoors { get; set; }
        public List<int> BrokenWindows { get; set; }
        public float VehicleHealth { get; set; }
        public List<int> PoppedTyres { get; set; }
        public SpecifiedValueForDamage()
        {
            BrokenDoors = new List<int>();
            BrokenDoors = new List<int>();
            PoppedTyres = new List<int>();
        }
    }

  

    public class SaleVehicle
    {
        [XmlElement("ID")]
        public int ID { get; set; }
        [XmlElement("Model")]
        public GrandTheftMultiplayer.Shared.VehicleHash VehicleModel { get; set; }
        [XmlElement("Position")]
        public AttributeData3<float> Position { get; set; }
        [XmlElement("Rotation")]
        public AttributeData3<float> Rotation { get; set; }
        [XmlElement("DeliverPosition")]
        public AttributeData3<float> DeliverPosition { get; set; }
        [XmlElement("DeliverRotation")]
        public AttributeData3<float> DeliverRotation { get; set; }
        [XmlElement("Colors")]
        public Colors VehicleColors { get; set; }
        [XmlElement("Interaction")]
        public AttributeData2<bool> Interaction { get; set; }
        [XmlElement("Price")]
        public AttributeData2<int> Price { get; set; }
        [XmlElement("Dimension")]
        public int Dimension { get; set; } = 0;
        [XmlElement("DeliverDimension")]
        public int DeliverDimension { get; set; } = 0;
    }

    [XmlRoot("SaleVehicle_List")]
    public class SaleVehicleList
    {
        [XmlElement("SaleVehicle")]
        public List<SaleVehicle> Items { get; set; }
        public SaleVehicleList() { Items = new List<SaleVehicle>(); }
    }

    #region AttrubteData

    public class AttributeData<T>
    {
        [XmlAttribute]
        public T Value { get; set; }
    }
    public class AttributeData2<T>
    {
        [XmlAttribute]
        public T Rent { get; set; }
        [XmlAttribute]
        public T Buy { get; set; }
    }
    public class AttributeData3<T>
    {
        [XmlAttribute]
        public T X { get; set; }
        [XmlAttribute]
        public T Y { get; set; }
        [XmlAttribute]
        public T Z { get; set; }

        public Vector3 ToVector3()
        {
            return new Vector3(Convert.ToSingle(X),Convert.ToSingle(Y),Convert.ToSingle(Z));
        }
    }
    public class Colors
    {
        [XmlAttribute]
        public int Color_1 { get; set; }
        [XmlAttribute]
        public int Color_2 { get; set; }
    }
    #endregion


    public class Tax 
    {
        [XmlAttribute("Vehicle")]
        public VehicleHash  VehicleName { get; set; }
        [XmlAttribute("TaxPerHour")]
        public float TaxPerHour { get; set; }
        [XmlAttribute("MaxTax")]
        public float MaxTax { get; set; }
    }

    [XmlRoot("Taxes_List")]
    public class TaxesList
    {
        [XmlElement("Tax")]
        public List<Tax> Items { get; set; }
        public TaxesList() { Items = new List<Tax>(); }
    }

}
