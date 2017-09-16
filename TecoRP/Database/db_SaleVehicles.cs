using GrandTheftMultiplayer.Server.API;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using TecoRP.Managers;
using TecoRP.Models;

namespace TecoRP.Database
{
    public class db_SaleVehicles
    {
        static XmlSerializer xSer = new XmlSerializer(typeof(SaleVehicleList));
        public static string dataPath = "Data/SaleVehicles.xml";
        public static SaleVehicleList currentSaleVehicleList = new SaleVehicleList();
        public db_SaleVehicles()
        {

        }

        public static SaleVehicleList GetAll()
        {
            if (System.IO.File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(SaleVehicleList), new XmlRootAttribute("SaleVehicle_List"));
                    currentSaleVehicleList = (SaleVehicleList)deserializer.Deserialize(reader);
                }
            }
            else
            {
                SaveChanges();
            }

            return currentSaleVehicleList;
        }

        public static void AddSaleVehicle(SaleVehicle _vehicleModel)
        {
            API.shared.consoleOutput("COUNT : " + currentSaleVehicleList.Items.Count+ " | model null : "+(_vehicleModel==null).ToString() );

            _vehicleModel.ID = currentSaleVehicleList.Items.Count > 0 ? currentSaleVehicleList.Items.LastOrDefault().ID + 1 : 1;
            currentSaleVehicleList.Items.Add(_vehicleModel);
            SaveChanges();
        }
        public static void RemoveSaleVehicleFully(int _Id)
        {
            var _Index = FindSaleVehicleIndexById(_Id);
            API.shared.deleteEntity(SaleVehicleManager.SaleVehiclesOnMap[_Index]);
            currentSaleVehicleList.Items.RemoveAt(_Index);
            SaleVehicleManager.SaleVehiclesOnMap.RemoveAt(_Index);
            SaveChanges();
        }
        

        public void RemoveSaleVehicle(int _Id)
        {
            var _Index = FindSaleVehicleIndexById(_Id);
            currentSaleVehicleList.Items.RemoveAt(_Id);
            SaveChanges();
        }

        public static int FindSaleVehicleIndexById(int _Id)
        {
            return currentSaleVehicleList.Items.IndexOf(currentSaleVehicleList.Items.Where(x => x.ID ==_Id).FirstOrDefault());

        }
        public static int FindSaleVehicleIdByIndex(int _Index)
        {
            return currentSaleVehicleList.Items[_Index].ID;
        }
        public static void SaveChanges()
        {

            if (System.IO.Directory.Exists(dataPath.Split('/')[0]))
            {
                XmlTextWriter xWriter = new XmlTextWriter(dataPath, Encoding.UTF8);
                xWriter.Formatting = Formatting.Indented;
                xSer.Serialize(xWriter, currentSaleVehicleList);
                xWriter.Dispose();
            }
            else
            {
                System.IO.Directory.CreateDirectory(dataPath.Split('/')[0]);
            }
        }
    }
}
