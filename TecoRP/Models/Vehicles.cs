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
        [XmlIgnore]
        public GrandTheftMultiplayer.Server.Elements.Vehicle VehicleOnMap{ get; set; }
        [XmlElement("VehId")]
        public long VehicleId { get; set; }
        [XmlAttribute("Livery")]
        public int Livery { get; set; } = 0;

        [XmlElement("VehModel")]
        public VehicleHash VehicleModelId { get; set; }
        [XmlElement("VehOwner")]
        public string OwnerSocialClubName { get; set; }

        [XmlAttribute("PastMinutes")]
        public int PastMinutes { get; set; } = 0;
        [XmlAttribute("Tax")]
        public float Tax { get; set; } = 0;
        public bool IsBlockedForTax { get; set; } = false;
        [XmlElement("Plate")]
        public string Plate { get; set; }
        [XmlElement("faction")]
        public int FactionId { get; set; } = -1;

        [XmlElement("job")]
        public int JobId { get; set; } = -1;
        [XmlAttribute("RentedTime")]
        public DateTime RentedTime { get; set; }
        [XmlAttribute("RentedPlayer")]
        public string RentedPlayerSocialClubId { get; set; } 

        [XmlElement("Color_1")]
        public int Color1 { get; set; }
        [XmlElement("Color_2")]
        public int Color2 { get; set; }
        [XmlElement("Mod_Color_1")]
        public int mod_color1 { get; set; }
        [XmlElement("Mod_Color_2")]
        public int mod_color2 { get; set; }
        [XmlElement("Mods")]
        public int[] Mods { get; set; } = new int[70];

        [XmlElement("IsLocked")]
        public bool IsLocked { get; set; }
        [XmlElement("Fuel")]
        public float Fuel { get; set; }
        [XmlElement("LastPosition")]
        public Vector3 LastPosition { get; set; }
        [XmlElement("Quaternation")]
        public Vector3 LastRotation { get; set; }
        [XmlAttribute("Dimension")]
        public int Dimension { get; set; } = 0;
        [XmlElement("Baggage")]
        public string BaggageItems { get; set; }
        [XmlElement("MaxBaggageCount")]
        public int MaxBaggageCount { get; set; } = 10;
        [XmlElement("Torpedo")]
        public string TorpedoItems { get; set; } 
        [XmlElement("MaxTorpedoCount")]
        public int MaxTorpedoCount { get; set; } = 5;

        [XmlElement("Damage")]
        public string  SpecifiedDamage { get; set; } //JSON - SpecifiedValueForDamage

        public int GetBaggageItemsCount()
        {
            if (!String.IsNullOrEmpty(BaggageItems))
            {
                return JsonConvert.DeserializeObject<StorageList>(BaggageItems).Items.Count;
            }
            return 0;
        }
        public int GetTorpedoItemsCount()
        {
            if (!String.IsNullOrEmpty(BaggageItems))
            {
                return JsonConvert.DeserializeObject<StorageList>(TorpedoItems).Items.Count;
            }
            return 0;
        }
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
