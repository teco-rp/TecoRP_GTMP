using System;
using System.Collections.Generic;

namespace TecoRP.Models
{
    public class Account : ISingleDocBase
    {
        public Account()
        {
        }

        public Account(string email, string passwordHash)
        {
            Email = email;
        }

        public string ID { get; set; }
        public string MainSocialClubName { get; set; }
        public string UDID { get; set; }
        public string Email { get; set; }
        public string PasswordHash { get; set; }
        public int MaxCharacters { get; set; } = 3;
        public List<string> Characters { get; set; } = new List<string>();
        public DateTime RegisterDate { get; set; }
        public List<LoginInfo> Logins { get; set; } = new List<LoginInfo>();

        public string UniqueName => Email;
    }

    public class LoginInfo
    {
        public LoginInfo()
        {
            Date = DateTime.UtcNow;
        }

        public LoginInfo( string ip, bool success = true) : this()
        {
            IP = ip;
            Success = success;
        }

        public DateTime Date { get; set; }
        public string IP { get; set; }
        public bool Success { get; set; }
    }
}
