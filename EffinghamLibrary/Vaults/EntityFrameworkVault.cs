using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using EffinghamDataAccess;
using EffinghamLibrary.Accounts;
using EffinghamLibrary.Helpers;

namespace EffinghamLibrary.Vaults
{
    public class EntityFrameworkVault : IVault
    {
        #region Fields and Properties
        private readonly DataAccessContext database;

        private readonly object bouncer;
        #endregion Fields and Properties

        #region Constructors
        static EntityFrameworkVault()
        {
            vault = null;
        }

        private EntityFrameworkVault()
        {
            bouncer = new object();
            database = new DataAccessContext();
            
            isDisposed = false;
        }
        #endregion Constructors

        #region IVault
        public void AddAccount(IBankAccountMultipleCurrency account, bool delayWrite = false)
        {
            lock (bouncer)
            {
                database.Accounts.Add(account.UnmarshallBankAccount());
                if (!delayWrite)
                {
                    database.SaveChanges();
                }
            }
        }

        public void DeleteAccount(IBankAccountMultipleCurrency account, bool delayWrite = false)
        {
            lock (bouncer)
            {
                AccountData toRemove = database.Accounts.Single(acct => acct.AccountNumber == account.AccountNumber);
                database.Accounts.Remove(toRemove);

                if (!delayWrite)
                {
                    database.SaveChanges();
                }
            }
        }

        public void FlushAccounts()
        {
            lock (bouncer)
            {
                database.SaveChanges();
            }
        }

        public IBankAccountMultipleCurrency GetAccount(int accountNumber)
        {
            lock (bouncer)
            {
                return database.Accounts.Single(account => account.AccountNumber == accountNumber).MarshallBankAccount();
            }
        }

        public IEnumerable<IBankAccountMultipleCurrency> GetAccounts()
        {
            lock (bouncer)
            {
                return database.Accounts.ToList().ConvertAll(account => account.MarshallBankAccount()).AsEnumerable();
            }
        }

        public Task<IEnumerable<IBankAccountMultipleCurrency>> GetAccountsAsync()
        {
            // #GetAccounts() implements locking so this should not be put in a lock.
            return Task.Run(() => GetAccounts());
        }

        public void UpdateAccount(IBankAccountMultipleCurrency account, bool delayWrite = false)
        {
            lock (bouncer)
            {
                AccountData accountData = database.Accounts.Single(acct => acct.AccountNumber == account.AccountNumber);
                accountData.Balance = account.Balance;
                accountData.CustomerName = accountData.CustomerName;

                if (!delayWrite)
                {
                    database.SaveChanges();
                }
            }
        }
        #endregion IVault

        #region Singleton Support
        private static EntityFrameworkVault vault;

        public static EntityFrameworkVault Instance
        {
            get
            {
                if (vault == null || vault.isDisposed)
                {
                    vault = new EntityFrameworkVault();
                }
                return vault;
            }
        }
        #endregion Singleton Support

        #region IDisposable Support
        private bool isDisposed;

        protected virtual void Dispose(bool disposing)
        {
            if (!isDisposed)
            {
                if (disposing)
                {
                    FlushAccounts();
                    database.Dispose();
                }

                isDisposed = true;
            }
        }

        ~EntityFrameworkVault()
        {
            Dispose(false);
        }

        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion IDisposable Support
    }
}
