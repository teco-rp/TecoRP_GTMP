using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Web;
using System.Xml;
using System.Xml.Serialization;
using TecoRP.Models;

namespace TecoRP.Database
{
    public class db_WhiteList
    {
        static XmlSerializer xSer = new XmlSerializer(typeof(WhiteList));
        public const string dataPath = "Whitelist/AllowedPlayers.xml";

        public static WhiteList GetAllowedPlayers(bool serverMapPath = false)
        {
            WhiteList returnModel = new Models.WhiteList();// string path = dataPath;
            //if (serverMapPath)
            //{
            //    //path = HttpContext.Current.Server.MapPath("~/" + dataPath.Split('/').FirstOrDefault());
            //    //path = Path.Combine(path, dataPath.Split('/').LastOrDefault());
            //}
            if (System.IO.File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(WhiteList), new XmlRootAttribute("Whitelist"));
                    returnModel = (WhiteList)deserializer.Deserialize(reader);
                }
            }
            else
            {
                SaveChanges(returnModel, serverMapPath);
            }

            return returnModel;
        }



        public static void AddPlayer(string socialClubName, bool serverMapPath = false)
        {
            var _list = GetAllowedPlayers(serverMapPath);
            _list.Users.Add(new Models.WhiteListUser { SocialClubName = socialClubName, LastValidateTime = DateTime.Now.AddYears(2) });
            SaveChanges(_list, serverMapPath);
        }
        public static void ChangeWhiteListSituation(bool active)
        {
            var _list = GetAllowedPlayers();
            _list.IsEnabled = active;
            SaveChanges(_list);
        }

        public static void SaveChanges(WhiteList _model, bool serverMapPath = false)
        {
            //string path = Path.Combine(dataPath.Split('/'));
            //if (serverMapPath)
            //{
            //    path = HttpContext.Current.Server.MapPath("~/" + dataPath.Split('/').FirstOrDefault());
            //    path = Path.Combine(path, dataPath.Split('/').LastOrDefault());
            //}
            if (System.IO.Directory.Exists(dataPath.Split('/')[0]))
            {
                XmlTextWriter xWriter = new XmlTextWriter(dataPath, System.Text.UTF8Encoding.UTF8);
                xWriter.Formatting = Formatting.Indented;
                xSer.Serialize(xWriter, _model);
                xWriter.Dispose();
            }
            else
            {
                System.IO.Directory.CreateDirectory(dataPath.Split('/')[0]);
            }
        }
    }
}
