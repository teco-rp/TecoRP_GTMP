using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared.Math;
using TecoRP.Database;

namespace TecoRP.Models
{
    public enum FloorType
    {
        House,
        Business,
        Warehouse,
    }

    public class Floor
    {
        [XmlAttribute("Floor")]
        public int FloorNumber { get; set; }
        [XmlAttribute("Type")]
        public FloorType Type { get; set; }
        [XmlAttribute("TypeObjectId")]
        public int TypedObjectId { get; set; }
        [XmlIgnore]
        public Vector3 EntrancePosition
        {
            get {
                switch (Type)
                {
                    case FloorType.House:
                        return db_Houses.GetHouse(TypedObjectId).EntrancePosition;
                    case FloorType.Business:
                        break;
                    case FloorType.Warehouse:
                        break;
                }
                return new Vector3();
            }
            set
            {
                switch (Type)
                {
                    case FloorType.House:
                        db_Houses.GetHouse(TypedObjectId).EntrancePosition = value;
                        db_Houses.SaveChanges();
                        break;
                    case FloorType.Business:
                        break;
                    case FloorType.Warehouse:
                        break;
                }
            }
        }
        [XmlIgnore]
        public int EntranceDimension
        {
            get
            {
                switch (Type)
                {
                    case FloorType.House:
                        return db_Houses.GetHouse(TypedObjectId).EntranceDimension;
                    case FloorType.Business:
                        break;
                    case FloorType.Warehouse:
                        break;
                }
                return 0; 
            }
            set
            {
                switch (Type)
                {
                    case FloorType.House:
                        db_Houses.GetHouse(TypedObjectId).EntranceDimension = value;
                        db_Houses.SaveChanges();
                        break;
                    case FloorType.Business:
                        break;
                    case FloorType.Warehouse:
                        break;
                }
            }
        }
        [XmlIgnore]
        public Vector3 InteriorPosition
        {
            get
            {
                switch (Type)
                {
                    case FloorType.House:
                        return db_Houses.GetHouse(TypedObjectId).InteriorPosition;
                    case FloorType.Business:
                        break;
                    case FloorType.Warehouse:
                        break;
                    default:
                        return new Vector3();
                }
                return new Vector3();
            }
        }
        [XmlIgnore]
        public int InteriorDimension{ get
            {
                switch (Type)
                {
                    case FloorType.House:
                        return db_Houses.GetHouse(TypedObjectId).InteriorDimension;
                    case FloorType.Business:
                        break;
                    case FloorType.Warehouse:
                        break;
                    default:
                        break;
                }
                return 0;
            }
        }
        [XmlIgnore]
        public bool IsLocked{ get {
                switch (Type)
                {
                    case FloorType.House:
                        return db_Houses.GetHouse(TypedObjectId).IsLocked;
                    case FloorType.Business:
                        return db_Businesses.GetById(TypedObjectId).IsClosed;
                    case FloorType.Warehouse:
                        break;
                    default:
                        return false;
                }
                return false;
            }
            set
            {
                switch (Type)
                {
                    case FloorType.House:
                        var _house = db_Houses.GetHouse(TypedObjectId);
                        _house.IsLocked = value;
                        db_Houses.SaveChanges();
                        break;
                    case FloorType.Business:
                        var _business = db_Businesses.GetById(TypedObjectId);
                        _business.IsClosed = value;
                        db_Businesses.SaveChanges();
                        break;
                    case FloorType.Warehouse:
                        break;
                    default:
                        break;
                }
            }
        }
        public bool IsOwner(string socialClubName)
        {
            switch (Type)
            {
                case FloorType.House:
                    return socialClubName == db_Houses.GetHouse(TypedObjectId).OwnerSocialClubName;
                case FloorType.Business:
                    return socialClubName == db_Businesses.GetById(TypedObjectId).OwnerSocialClubName;
                case FloorType.Warehouse:
                    break;
                default:
                    break;
            }
            return false;
        }
    }
    public class Building
    {
        [XmlIgnore]
        public TextLabel LabelOnMap { get; set; }
        [XmlIgnore]
        public Marker MarkerOnMap { get; set; }
        [XmlAttribute("ID")]
        public int BuildingId { get; set; }
        [XmlAttribute("Name")]
        public string BuildingName { get; set; }
        [XmlElement("position")]
        public Vector3 Position { get; set; }
        [XmlAttribute("Dimension")]
        public int Dimension { get; set; }
        [XmlElement("Floor")]
        public List<Floor> Floors { get; set; }
    }

    [XmlRoot("Building_List")]
    public class BuildingList
    {
        [XmlElement("Building")]
        public List<Building> Buildings { get; set; }
        public BuildingList() { Buildings = new List<Building>(); }
    }
}
