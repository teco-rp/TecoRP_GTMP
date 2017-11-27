using ModelMixer.Processes;
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
    public class Emails
    {
        static XmlSerializer xSer = new XmlSerializer(typeof(Models.ReceiverMailList));
        public static string dataPath = "Data/ReportMailList.xml";
        public static ReceiverMailList currentMailReceivers = new Models.ReceiverMailList();

        static Emails()
        {
            GetAllReceivers();
        }
        public static ReceiverMailList GetAllReceivers()
        {
            if (System.IO.File.Exists(dataPath))
            {
                using (var reader = new StreamReader(dataPath))
                {
                    XmlSerializer deserializer = new XmlSerializer(typeof(ReceiverMailList), new XmlRootAttribute("ReceiverMail_List"));
                    currentMailReceivers = (ReceiverMailList)deserializer.Deserialize(reader);
                }
            }
            else
            {
                SaveChanges();
            }

            return currentMailReceivers;
        }


        public static bool AddReceiver(ReceiverMail _model)
        {
            if (!String.IsNullOrEmpty(_model.Email))
            {
                _model.ID = Guid.NewGuid();
                currentMailReceivers.Emails.Add(_model);
                SaveChanges();
                return true;
            }

            return false;
        }

        public static ReceiverMail GetReceiver(Guid _Id)
        {
            return currentMailReceivers.Emails.FirstOrDefault(x => x.ID == _Id);
        }
        public static bool UpdateReceiver(ReceiverMail _model)
        {
            var edited = GetReceiver(_model.ID);
            if (edited!=null)
            {
                edited.CopyFrom(_model);
                SaveChanges();
                return true;
            }
            return false;
        }

        public static bool RemoveReceiver(ReceiverMail _model)
        {
            var deleted = GetReceiver(_model.ID);
            if (deleted !=null)
            {
                currentMailReceivers.Emails.Remove(_model);
                SaveChanges();
                return true;
            }
            return false;
        }
        public static void SaveChanges()
        {
            lock (currentMailReceivers)
            {
                if (System.IO.Directory.Exists(dataPath.Split('/')[0]))
                {
                    XmlTextWriter xWriter = new XmlTextWriter(dataPath, System.Text.UTF8Encoding.UTF8);
                    xWriter.Formatting = Formatting.Indented;
                    xSer.Serialize(xWriter, currentMailReceivers);
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
