using ModelMixer.Attributes;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace TecoRP_BugReporter.Models
{
    public class MailSender
    {
        //public string EmailAdress { get; set; } = "tecoroleplay@gmail.com";
        public string EmailAdress { get; set; } = "tecoroleplay@gmail.com";
        public string Password { get; set; } = "atlarlar";
    }

    public class MailContent
    {
        public string Title { get; set; }
        public string MailText { get; set; }
        public Level MailLevel { get; set; }
    }

    public enum Level
    {
        Fatal,
        Normal
    }



    public class ReceiverMail
    {
        [IgnoreCopy]
        [XmlAttribute("ID")]
        public Guid ID { get; set; }
        [XmlAttribute("Email")]
        public string Email { get; set; }
        [XmlAttribute("MaxEmail")]
        public int MaxEmailPerDay { get; set; }
        [XmlAttribute("Sent")]
        public int EmailSentInDay { get; set; }
        [XmlAttribute("Validation")]
        public DateTime ValidationTime { get; set; }
    }
    [XmlRoot("ReceiverMail_List")]
    public class ReceiverMailList
    {
        [XmlElement("Email")]
        public List<ReceiverMail> Emails { get; set; }
    }
}
