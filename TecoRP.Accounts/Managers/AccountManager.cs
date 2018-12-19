using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TecoRP.Accounts.Managers.Base;
using TecoRP.Accounts.Repositories;

namespace TecoRP.Accounts.Managers
{
    public class AccountManager : BaseManagerScript
    {
        public void Login(Client sender, string email, string password)
        {
            var user = AccountRepository.Instance.GetSingle(x => x.Email == email);
            if (user.PasswordHash != API.getHashSHA256(password))
            {
                API.triggerClientEvent(sender, "wrong_password");
                return;
            }


        }

        public void Register()
        {
            if (AccountRepository.Instance.Get(x => x.Email == email) != null)
            {
                API.triggerClientEvent(sender, "email_already_exist");
                return;
            }
        }
    }
}
