using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;
using TecoRP_BugReporter.Models;

namespace TecoRP_BugReporter.Manage
{
    public class Mailer
    {
        public const string smtpAddress = "smtp.gmail.com";
        public const int portNumber = 587;
        public const bool enableSSL = true;
        

        internal static void SendMailToAll(MailContent _mail)
        {
            foreach(var itemMail in Config.Emails.GetAllReceivers().Emails)
            {
                using (MailMessage mail = new MailMessage())
                {
                    var emailFrom = Config.SenderMail.GetSender();
                    mail.From = new MailAddress(emailFrom.EmailAdress);
                    mail.To.Add(itemMail.Email);
                    itemMail.EmailSentInDay++;
                    
                    mail.Subject = _mail.Title;
                    mail.Body = _mail.MailText;
                    mail.IsBodyHtml = true;

                    using (SmtpClient smtp = new SmtpClient(smtpAddress, portNumber))
                    {
                        
                        smtp.Credentials = new NetworkCredential(emailFrom.EmailAdress, emailFrom.Password);
                        smtp.EnableSsl = true;
                        smtp.Send(mail);
                    }

                }
            }
        }
        internal static void SendMail()
        {

        }
    }
}
