using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;
using GrandTheftMultiplayer.Shared.Math;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TecoRP.Models;
using TecoRP.Repository.Base;

namespace TecoRP.Database
{
    public class db_VehicleShops
    {
        private static IBaseRepository<VehicleShop> _repository = new JsonRepositoryBase<VehicleShop>("Data/VehicleShops.json");
        private static IBaseRepository<VehiclePrice> _pricesRepeository = new JsonRepositoryBase<VehiclePrice>("Data/VehiclePrices.json");
        public static void Init()
        {
            LoadVehPrices();
            SpawnAll();
        }

        public static void SpawnAll()
        {
            foreach (var item in _repository.Get())
            {
                item.MarkerOnMap = GenerateOnMap(item);
            }
        }

        public static VehicleShop Create(VehicleShop shop)
        {
            shop.VehicleShopId = _repository.Current.Count == 0 ? 1 : _repository.Current.Last().VehicleShopId + 1;
            shop.MarkerOnMap = GenerateOnMap(shop);
            _repository.Add(shop);
            return shop;
        }
        public static VehicleShop Get(int id)
        {
            return _repository.GetSingle(x => x.VehicleShopId == id);
        }
        public static void Update(VehicleShop shop)
        {
            _repository.Update(shop);
        }
        public static void Remove(int id)
        {
            var shopToRemove = _repository.GetSingle(x => x.VehicleShopId == id);
            RemoveFromMap(shopToRemove);
            _repository.Remove(shopToRemove);
        }

        public static VehicleShop FindNearest(Vector3 pos)
        {
            var _nearest = _repository.Current.FirstOrDefault();

            foreach (var shop in _repository.Current)
                if (Vector3.Distance(pos, shop.Position) < Vector3.Distance(pos, _nearest.Position))
                    _nearest = shop;

            return _nearest;
        }

        public static IEnumerable<VehiclePrice> GetPricesByClass(int @class)
        {
            foreach (var item in _pricesRepeository.Current)
                if (API.shared.getVehicleClass(item.Vehicle) == @class)
                    yield return item;
        }

        public static VehiclePrice GetPrice(VehicleHash hash)
        {
            return _pricesRepeository.GetSingle(x => x.Vehicle == hash);
        }

        public static void SetPrice(VehicleHash vehicle, int newPrice)
        {
            var edited = _pricesRepeository.GetSingle(x => x.Vehicle == vehicle);
            edited.Price = newPrice;
            _pricesRepeository.Update(edited);
        }
        private static void LoadVehPrices()
        {
            if (_pricesRepeository.Current == null || _pricesRepeository.Current.Count == 0)
            {
                GenerateInitalPrices();
                _pricesRepeository.SaveChanges();
            }

        }

        private static void GenerateInitalPrices()
        {
            foreach (VehicleHash veh in Enum.GetValues(typeof(VehicleHash)))
            {
                _pricesRepeository.Add(new VehiclePrice(veh, 10000));
            }
        }

        public static Marker GenerateOnMap(VehicleShop item)
        {
            return API.shared.createMarker(1, item.Position, new Vector3(), new Vector3(), new Vector3(1, 1, 1), 255, 0, 0, 255, item.Dimension);
        }
        public static void RemoveFromMap(VehicleShop item)
        {
            API.shared.deleteEntity(item.MarkerOnMap);
        }
    }
}
