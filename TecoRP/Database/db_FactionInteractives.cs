using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Constant;
using TecoRP.Models;

namespace TecoRP.Database
{
    public class db_FactionInteractives
    {
        static XmlSerializer xSer = new XmlSerializer(typeof(FactionInteractivesList));
        public static string dataPath = "Data/FactionInteractives.teco";

        public static Dictionary<int, FactionInteractive> currentFactionInteractives = new Dictionary<int, FactionInteractive>();

        public static void SpawnAll()
        {
            foreach (var item in GetAll().Interactives)
            {
                item.LabelOnMap = API.shared.createTextLabel(item.Name, item.Position, 15, 1, false, item.Dimension);
                currentFactionInteractives.Add(item.InteractiveID, item);
            }
        }
        public static void ClearAll()
        {
            foreach (var item in currentFactionInteractives)
            {
                if (item.Value.LabelOnMap != null)
                {
                    API.shared.deleteEntity(item.Value.LabelOnMap);
                }
            }
            currentFactionInteractives.Clear();
        }

        public static FactionInteractivesList GetAll()
        {
            FactionInteractivesList returnModel = new Models.FactionInteractivesList();
            if (System.IO.File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(FactionInteractivesList), new XmlRootAttribute("FactionInteractive_List"));
                    returnModel = (FactionInteractivesList)deserializer.Deserialize(reader);
                }
            }
            else
            {
                SaveChanges();
            }

            return returnModel;
        }

        public static void Create(FactionInteractive _model)
        {
            _model.InteractiveID = currentFactionInteractives.Count > 0 ? currentFactionInteractives.LastOrDefault().Key + 1 : 1;
            _model.LabelOnMap = API.shared.createTextLabel(_model.Name, _model.Position, 15, 1, false, _model.Dimension);
            currentFactionInteractives.Add(_model.InteractiveID, _model);
            SaveChanges();
        }
        public static bool Remove(int Id)
        {
            try
            {
                var _model = GetById(Id);
                if (_model != null)
                {
                    API.shared.deleteEntity(_model.LabelOnMap);
                    bool result = currentFactionInteractives.Remove(Id);
                    SaveChanges();
                    return result;
                }
                return false;
            }
            catch (Exception ex)
            {
                API.shared.consoleOutput(LogCat.Warn,ex.ToString());
                return false;
            }
        }
        public static void Update(FactionInteractive _model)
        {
            var edited = GetById(_model.InteractiveID);
            if (edited!=null)
            {
                edited.LabelOnMap.position = _model.Position;
                edited.LabelOnMap.dimension = _model.Dimension;
                edited.LabelOnMap.text = _model.Name;
                SaveChanges();
            }
            else
            {
                API.shared.consoleOutput(LogCat.Warn,"FactionInteractive Update, model NULL geldi.");
            }
        }
        public static FactionInteractive GetById(int Id)
        {
            try
            {
                return currentFactionInteractives[Id];
            }
            catch (Exception)
            {
                return null;
            }
        }
        public static void SaveChanges()
        {

            if (Directory.Exists(dataPath.Split('/')[0]))
            {
                XmlTextWriter xWriter = new XmlTextWriter(dataPath, System.Text.UTF8Encoding.UTF8);
                xWriter.Formatting = Formatting.Indented;
                xSer.Serialize(xWriter, new FactionInteractivesList { Interactives = currentFactionInteractives.Values.ToList() });
                xWriter.Dispose();
            }
            else
            {
                Directory.CreateDirectory(dataPath.Split('/')[0]);
            }
        }
    }
}
