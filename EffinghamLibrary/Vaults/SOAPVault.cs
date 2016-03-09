using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using EffinghamLibrary.Accounts;

namespace EffinghamLibrary.Vaults
{
    /// <summary>
    /// Implements a vault using SOAP as the persistence backbone.
    /// </summary>
    public class SoapVault : IVault
    {
        #region Fields and Properties
        /// <summary>
        /// Defines the name but not the fully qualified location of the data file on disk.
        /// </summary>
        private const string DataFile = @"Accounts.dat";

        private IList<IBankAccountMultipleCurrency> accounts;

        private bool isFlushed;

        private ReaderWriterLockSlim instanceLock;
        #endregion Fields and Properties

        #region Constructors
        static SoapVault()
        {
            vault = null;
        }

        private SoapVault()
        {
            accounts = new List<IBankAccountMultipleCurrency>();
            isFlushed = false;
            instanceLock = new ReaderWriterLockSlim();
        }
        #endregion Constructors

        #region IVault Support
        /// <summary>
        /// Retrieves the collection of accounts stored by the vault.
        /// </summary>
        /// <returns>The enumerable collection of accounts stored in the vault.</returns>
        public IEnumerable<IBankAccountMultipleCurrency> GetAccounts()
        {
            return accounts.AsEnumerable();
        }

        /// <summary>
        /// Retrieves the account stored in the vault with the provided accountNumber. If no
        /// such account exists or multiple accounts with that account number exists, an
        /// <see cref="InvalidOperationException"/> will be thrown.
        /// </summary>
        /// <param name="accountNumber">The account number of the account in this vault to retrieve</param>
        /// <returns>The account stored in the vault with the specified account number.</returns>
        /// <exception cref="InvalidOperationException">
        /// If there is not exactly one account identified by the specified account number.
        /// </exception>
        public IBankAccountMultipleCurrency GetAccount(int accountNumber)
        {
            return accounts.Single(account => account.AccountNumber == accountNumber);
        }

        /// <summary>
        /// Stores the provided account in active memory and optionally delays persisting it
        /// in SOAP storage until explicitly flushed.
        /// </summary>
        /// <param name="account">The account to store.</param>
        /// <param name="delayWrite">
        /// If false, the account will immediately be persisted in the SOAP data file; otherwise,
        /// consumers need to explicitly flush data to disk at a later time.
        /// </param>
        public void AddAccount(IBankAccountMultipleCurrency account, bool delayWrite = false)
        {
            accounts.Add(account);
            isFlushed = false;

            if (!delayWrite)
            {
                // TODO: Write accounts to disk
            }
        }

        /// <summary>
        /// Updates the provided account in active memory and optionally delays persisting it
        /// in SOAP storage until explicitly flushed.
        /// </summary>
        /// <param name="account">The account to update.</param>
        /// <param name="delayWrite">
        /// If false, the account will immediately be persisted in the SOAP data file; otherwise,
        /// consumers need to explicitly flush data to disk at a later time.
        /// </param>
        public void UpdateAccount(IBankAccountMultipleCurrency account, bool delayWrite = false)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Deletes the provided account from active memory and optionally delays persisting the
        /// change in SOAP storage until explicitly flushed.
        /// </summary>
        /// <param name="account">The account to remove from the vault.</param>
        /// <param name="delayWrite">
        /// If false, the change will immediately be persisted in the SOAP data file; otherwise,
        /// consumers need to explicitly flush data to disk at a later time.
        /// </param>
        public void DeleteAccount(IBankAccountMultipleCurrency account, bool delayWrite = false)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// Persists any create, update, or delete operations to the SOAP data file if the write
        /// operations were explicitly delayed.
        /// </summary>
        public void FlushAccounts()
        {
            throw new NotImplementedException();
        }
        #endregion IVault Support

        #region Singleton Support
        /// <summary>
        /// The backing instance of the singleton.
        /// </summary>
        private static SoapVault vault;

        /// <summary>
        /// Initializes the vault instance if necessary and returns the singleton instance.
        /// </summary>
        public static SoapVault Instance
        {
            get
            {
                if (vault == null || vault.disposedValue)
                {
                    vault = new SoapVault();
                }
                return vault;
            }
        }
        #endregion Singleton Support

        #region IDisposable Support
        /// <summary>
        /// Detects redundant calls to Dispose.
        /// </summary>
        private bool disposedValue = false;

        /// <summary>
        /// Centralizes all resource cleanup for the SoapVault. The finalizer and public Dispose() method
        /// should not be modified.
        /// </summary>
        /// <param name="disposing"></param>
        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    
                }

                disposedValue = true;
            }
        }

        /// <summary>
        /// Ensures that the vault's resources are cleaned up if not explicitly disposed of by the consumer.
        /// </summary>
        ~SoapVault()
        {
            Dispose(false);
        }

        /// <summary>
        /// Allows for explicit resource cleanup via direct calls or implicit cleanup by putting a vault
        /// instance in a using block.
        /// </summary>
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }
        #endregion IDisposable Support
    }
}
