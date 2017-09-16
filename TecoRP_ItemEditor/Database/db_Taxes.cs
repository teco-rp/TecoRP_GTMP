using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using GrandTheftMultiplayer.Server.Elements;
using GrandTheftMultiplayer.Shared;
using TecoRP.Models;

namespace TecoRP_ItemEditor.Database
{
    public class db_Taxes
    {
        static XmlSerializer xSerTax = new XmlSerializer(typeof(TaxesList));
        public const string dataPathTax = "Data/Taxes.xml";
        public static List<Tax> currentTaxes = new List<Tax>();


        public static List<Tax> GetAllTaxes()
        {
            var returnModel = new TaxesList();
            if (System.IO.File.Exists(dataPathTax))
            {
                using (var reader = new StreamReader(dataPathTax))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(TaxesList), new XmlRootAttribute("Taxes_List"));
                    returnModel = (TaxesList)deserializer.Deserialize(reader);
                }
                currentTaxes = returnModel.Items;
            }
            else
            {
                SaveChanges();
            }
            return currentTaxes;
        }

        public static void GenerateAllVehicles()
        {
            currentTaxes.Clear();
            foreach (var item in Enum.GetNames(typeof(VehicleHash)))
            {
                currentTaxes.Add(new Tax { MaxTax = 5000, TaxPerHour = 15, VehicleName = (VehicleHash)(Enum.Parse(typeof(VehicleHash), item))});
            }
            SaveChanges();
        }

        public static void SaveChanges()
        {
            lock (currentTaxes)
            {

                if (System.IO.Directory.Exists(dataPathTax.Split('/')[0]))
                {
                    XmlTextWriter xWriter = new XmlTextWriter(dataPathTax, System.Text.UTF8Encoding.UTF8);
                    xWriter.Formatting = Formatting.Indented;
                    xSerTax.Serialize(xWriter, new TaxesList { Items = currentTaxes });
                    xWriter.Dispose();
                }
                else
                {
                    System.IO.Directory.CreateDirectory(dataPathTax.Split('/')[0]);
                } 
            }
        }

    }
}
