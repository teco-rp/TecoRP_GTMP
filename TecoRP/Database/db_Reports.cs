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
    public class db_Reports
    {
        static XmlSerializer xSer = new XmlSerializer(typeof(ReportList));
        public const string dataPath = "Data/Reports.xml";

        public static ReportList GetAll()
        {
            ReportList returnModel = new ReportList();
            if (File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(ReportList), new XmlRootAttribute("Reports_List"));
                    returnModel = (ReportList)deserializer.Deserialize(reader);
                }
            }
            else
            {
                SaveChanges(returnModel);
            }
            return returnModel;
        }

        public static int AddReport(Report _model)
        {
            var _list = GetAll();
            _list.Reports.Add(_model);
            for (int i = 0; i < _list.Reports.Count; i++)
            {
                _list.Reports[i].ReportID = i + 1;
            }
            SaveChanges(_list);
            return _list.Reports.LastOrDefault().ReportID;
        }
        public static Report GetReport(int rId)
        {
            var _list = GetAll();
            return _list.Reports.FirstOrDefault(x => x.ReportID == rId);
        }
        public static IEnumerable<Report> GetReports(Models.ReportType _type)
        {
            var _list = GetAll();
            return _list.Reports.Where(x => x.Type == _type);
        }
        public static bool RemoveReport(int reportId)
        {
            var _list = GetAll();
            bool result= _list.Reports.Remove(_list.Reports.FirstOrDefault(x => x.ReportID == reportId));
            SaveChanges(_list);
            return result;
        }
        public static void SaveChanges(ReportList _model)
        {
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
