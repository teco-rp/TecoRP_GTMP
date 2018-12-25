using GrandTheftMultiplayer.Server.API;
using GrandTheftMultiplayer.Server.Elements;
using System;
using TecoRP.Helpers;
using TecoRP.Models;
using TecoRP.Repository.Base;

namespace TecoRP.Database
{
    public class db_Accounts
    {
        static ISingleDocRepositoryBase<Account> _repository = new JsonSingleDocRepositoryBase<Account>("Accounts/");

        /// <summary>
        /// Registers a player
        /// </summary>
        /// <exception cref="SoftException">If Email is already exist</exception>
        public static Account Register(Client player, string email, string password)
        {
            if (_repository.GetSingle(email) != null)
                throw new SoftException("Bu E-posta adı zaten kayıtlı.");

            var addedAccount = new Account
            {
                Email = email,
                PasswordHash = API.shared.getHashSHA256(password),
                ID = Guid.NewGuid().ToString("N"),
                MainSocialClubName = player.socialClubName,
                MaxCharacters = 3,
                RegisterDate = DateTime.UtcNow,
                UDID = player.uniqueHardwareId,
            };
            _repository.Add(addedAccount);
            return addedAccount;
        }
        /// <summary>
        /// </summary>
        /// <exception cref="SoftException">Some fail situations</exception>
        public static Account Login(Client player, string email, string password)
        {
            var loggedInAccount = _repository.GetSingle(email);
            if (loggedInAccount == null)
                throw new SoftException("Böyle bir hesap bulunmuyor.");
            if (loggedInAccount.PasswordHash != API.shared.getHashSHA256(password))
            {
                loggedInAccount.Logins.Add(new LoginInfo(player.address,false));
                _repository.Update(loggedInAccount);
                throw new SoftException("Parola hatalı görünüyor.");
            }

            loggedInAccount.Logins.Add(new LoginInfo(player.address));
            _repository.Update(loggedInAccount);
            return loggedInAccount;
        }

        public static void AddCharacter(Client player, string characterId)
        {
            Account account = player.getData(nameof(Account)) as Account;
            account.Characters.Add(characterId);
            player.setData(nameof(Account), account);
            _repository.Update(account);
        }
    }
}
