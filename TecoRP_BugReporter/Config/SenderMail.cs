using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml;
using System.Xml.Serialization;
using TecoRP_BugReporter.Models;

namespace TecoRP_BugReporter.Config
{
    public class SenderMail
    {
        static XmlSerializer xSer = new XmlSerializer(typeof(MailSender));
        public static string dataPath = "Data/ReportSender.xml";
        public static MailSender mailSender = new MailSender();

        static SenderMail()
        {
            mailSender = GetSender();
        }
        public static MailSender GetSender()
        {
            MailSender returnModel = null;
            if (System.IO.File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(MailSender));
                    returnModel = (MailSender)deserializer.Deserialize(reader);
                }
            }
            else
            {
                SaveChanges(); 
            }

            return returnModel;
        }

        public static void UpdateMailSender(MailSender _model)
        {
            mailSender = _model;
            SaveChanges();
        }

        public static void SaveChanges()
        {
            lock (mailSender)
            {
                if (System.IO.Directory.Exists(dataPath.Split('/')[0]))
                {
                    XmlTextWriter xWriter = new XmlTextWriter(dataPath, System.Text.UTF8Encoding.UTF8);
                    xWriter.Formatting = Formatting.Indented;
                    xSer.Serialize(xWriter, mailSender);
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
