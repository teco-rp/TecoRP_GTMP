using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TecoRP.Accounts.Models;
using TecoRP.Accounts.Repository.Base;

namespace TecoRP.Accounts.Repositories
{
    public class AccountRepository : JsonRepositoryBase<Account>
    {
        private static readonly object lockingObj;
        private static IBaseRepository<Account> _instance;
        public static IBaseRepository<Account> Instance
        {
            get
            {
                lock (lockingObj)
                    if (_instance == null)
                        _instance = new AccountRepository();

                return _instance;
            }
        }

    }
}
