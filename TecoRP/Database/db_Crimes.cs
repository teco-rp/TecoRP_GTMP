using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using TecoRP.Models;

namespace TecoRP.Database
{
    public class db_Crimes
    {
        static XmlSerializer xSer = new XmlSerializer(typeof(CrimeList));
        static XmlSerializer xSerType = new XmlSerializer(typeof(CrimeTypeList));
        public static string dataPath = "Data/Crimes.xml";
        public static string dataPathType = "Data/CrimeTypes.xml";
        public static CrimeList currentCrimes = new CrimeList();

        public db_Crimes()
        {

        }

        public static CrimeList GetAll()
        {
            if (System.IO.File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(CrimeList), new XmlRootAttribute("Crime_List"));
                    currentCrimes = (CrimeList)deserializer.Deserialize(reader);
                }
            }
            else
            {
                SaveChanges();
            }

            return currentCrimes;
        }

        public static CrimeTypeList GetCrimeTypes()
        {
            CrimeTypeList returnModel = new Models.CrimeTypeList();
            if (System.IO.File.Exists(dataPathType))
            {
                using (var reader = new StreamReader(dataPathType))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(CrimeTypeList), new XmlRootAttribute("CrimeType_List"));
                    returnModel = (CrimeTypeList)deserializer.Deserialize(reader);
                }
            }
            else
            {
                SaveChanges();
            }

            return returnModel;
        }

        public static void AddCrimeToPlayer(CrimeType _crime, string socialClubID)
        {
            Crime edited = currentCrimes.Items.FirstOrDefault(x => x.OwnerSocialClubName == socialClubID);
            if (edited==null)
            {
                edited = new Crime();
                edited.OwnerSocialClubName = socialClubID;
                currentCrimes.Items.Add(edited);
            }
            edited.Crimes.Add(_crime);
            edited.CrimesBefore++;
            SaveChanges();
        }
        public static void ClearPlayerCrimes(string socialClubID)
        {
            var edited = currentCrimes.Items.FirstOrDefault(x => x.OwnerSocialClubName == socialClubID);
            if (edited!=null)
            {
                if (edited.Crimes!=null)
                {
                    edited.Crimes.Clear();
                    SaveChanges();
                }
                else
                {
                    edited.Crimes = new List<Models.CrimeType>();
                }
            }
        }
        public static Crime GetPlayerCrimes(string SocialClubID)
        {
            return currentCrimes.Items.FirstOrDefault(x => x.OwnerSocialClubName == SocialClubID);
        }
        public static bool RemovePlayerCrime(string socialClubID, CrimeType _crime)
        {
            var edited = currentCrimes.Items.FirstOrDefault(x => x.OwnerSocialClubName == socialClubID);
            if (edited==null){return false;}

            return edited.Crimes.Remove(_crime);
        }
        public static void AddNewCrimeType(CrimeType _model)
        {
            var _list = GetCrimeTypes();
            _model.CrimeTypeId = _list.Items.Count > 0 ? _list.Items.LastOrDefault().CrimeTypeId + 1 : 1;
            _list.Items.Add(_model);
            SaveChanges(_list);
        }
        public static void UpdateCrimeType(CrimeType _model)
        {
            var _list = GetCrimeTypes();
            var edited = _list.Items.FirstOrDefault(x => x.CrimeTypeId == _model.CrimeTypeId);
            edited = _model;
            SaveChanges(_list);
        }
        public static void RemoveCrimeType(int _Id)
        {
            var _list = GetCrimeTypes();
            var removedIndex = _list.Items.Remove(_list.Items.FirstOrDefault(x => x.CrimeTypeId == _Id));
            SaveChanges(_list);
        }

        public static void SaveChanges()
        {

            if (System.IO.Directory.Exists(dataPath.Split('/')[0]))
            {
                XmlTextWriter xWriter = new XmlTextWriter(dataPath, System.Text.UTF8Encoding.UTF8);
                xWriter.Formatting = Formatting.Indented;
                xSer.Serialize(xWriter, currentCrimes);
                xWriter.Dispose();
            }
            else
            {
                System.IO.Directory.CreateDirectory(dataPath.Split('/')[0]);
            }
        }

        public static void SaveChanges(CrimeTypeList _model)
        {

            lock (_model)
            {
                if (Directory.Exists(dataPathType.Split('/')[0]))
                {
                    XmlTextWriter xWriter = new XmlTextWriter(dataPathType, System.Text.UTF8Encoding.UTF8);
                    xWriter.Formatting = Formatting.Indented;
                    xSerType.Serialize(xWriter, _model);
                    xWriter.Dispose();
                }
                else
                {
                    System.IO.Directory.CreateDirectory(dataPath.Split('/')[0]);
                } 
            }
        }
    }

 
}
