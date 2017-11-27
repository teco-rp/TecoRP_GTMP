using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TecoRP_BugReporter.Manage
{
    public class Reporter
    {
        public static void SendExceptionToAll(Exception ex)
        {
            
            SendExceptionToAll(ex.ToString(),ex.GetType().ToString());
        }

        public static void SendExceptionToAll(string text,string title)
        {
            Mailer.SendMailToAll(new Models.MailContent
            {
                MailLevel = Models.Level.Normal,
                MailText = text,
                Title = title
            });
        }
    }
}
