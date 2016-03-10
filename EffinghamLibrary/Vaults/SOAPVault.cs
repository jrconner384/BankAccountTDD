using System;
using System.Collections;
using System.Collections.Generic;
using System.Configuration;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Runtime.Serialization.Formatters.Soap;
using System.Security.Cryptography;
using System.Text;
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

        private List<IBankAccountMultipleCurrency> accounts;

        private bool isFlushed;

        private readonly ReaderWriterLockSlim activeMemoryLock;

        private readonly object persistenceBouncer;
        #endregion Fields and Properties

        #region Constructors
        static SoapVault()
        {
            vault = null;
        }

        private SoapVault()
        {
            // Need to guarantee that locking mechanisms are the first thing instantiated.
            persistenceBouncer = new object();
            activeMemoryLock = new ReaderWriterLockSlim();

            isFlushed = false;
            ReadAccounts(); // Initializes in-memory list of accounts, sets flags, etc.
        }
        #endregion Constructors

        #region IVault Support
        /// <summary>
        /// Retrieves the collection of accounts stored by the vault.
        /// </summary>
        /// <returns>The enumerable collection of accounts stored in the vault.</returns>
        public IEnumerable<IBankAccountMultipleCurrency> GetAccounts()
        {
            activeMemoryLock.EnterReadLock();
            try
            {
                return accounts.AsEnumerable();
            }
            finally
            {
                ExitReadLock();
            }
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
            activeMemoryLock.EnterReadLock();
            try
            {
                return accounts.Single(account => account.AccountNumber == accountNumber);
            }
            finally
            {
                ExitReadLock();
            }
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
            activeMemoryLock.EnterWriteLock();
            try
            {
                accounts.Add(account);
                isFlushed = false;
            }
            finally
            {
                ExitWriteLock();
            }

            if (!delayWrite)
            {
                WriteAccounts();
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
        /// <exception cref="ApplicationException">No account with the specified account number could be found.</exception>
        public void UpdateAccount(IBankAccountMultipleCurrency account, bool delayWrite = false)
        {
            activeMemoryLock.EnterUpgradeableReadLock();
            int index = accounts.FindIndex(accountInMemory => accountInMemory.AccountNumber == account.AccountNumber);

            try
            {
                if (index >= 0)
                {
                    activeMemoryLock.EnterWriteLock();
                    accounts[index] = account;
                    isFlushed = false;
                }
                else
                {
                    throw new ApplicationException($"Can't update the account. Account {account.AccountNumber} not found.");
                }
            }
            finally
            {
                ExitWriteLock();
                ExitUpgradeableReadLock();
            }

            if (!delayWrite)
            {
                WriteAccounts();
            }
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
        /// <exception cref="ApplicationException">No account with the specified account number could be found.</exception>
        public void DeleteAccount(IBankAccountMultipleCurrency account, bool delayWrite = false)
        {
            activeMemoryLock.EnterUpgradeableReadLock();

            try
            {
                int index = accounts.FindIndex(accountInMemory => accountInMemory.AccountNumber == account.AccountNumber);

                if (index >= 0)
                {
                    activeMemoryLock.EnterWriteLock();
                    accounts.RemoveAt(index);
                    isFlushed = false;
                }
                else
                {
                    throw new ApplicationException($"Can't delete the account. Account {account.AccountNumber} not found.");
                }
            }
            finally
            {
                ExitWriteLock();
                ExitUpgradeableReadLock();
            }

            if (!delayWrite)
            {
                WriteAccounts();
            }
        }

        /// <summary>
        /// Persists any create, update, or delete operations to the SOAP data file if the write
        /// operations were explicitly delayed.
        /// </summary>
        public void FlushAccounts()
        {
            WriteAccounts();
        }
        #endregion IVault Support

        #region Read/Write Methods
        private void ReadAccounts()
        {
            if (!File.Exists(DataFile))
            {
                activeMemoryLock.EnterWriteLock();

                try
                {
                    accounts = new List<IBankAccountMultipleCurrency>();
                    isFlushed = true;
                }
                finally
                {
                    ExitWriteLock();
                }
            }
            else
            {
                string encryptionKey = ConfigurationManager.AppSettings["encryptionKey"];
                byte[] encryptionSalt = Encoding.Unicode.GetBytes(ConfigurationManager.AppSettings["encryptionSalt"]);

                using (Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(encryptionKey, encryptionSalt)) // Need to know about these for the test
                {
                    // Also need to know about symmetric versus assymetric encryption.

                    using (AesManaged algorithm = new AesManaged())
                    {
                        algorithm.Padding = PaddingMode.PKCS7;
                        algorithm.Key = rfc.GetBytes(algorithm.KeySize / 8);
                        algorithm.IV = rfc.GetBytes(algorithm.BlockSize / 8);

                        using (ICryptoTransform decryptor = algorithm.CreateDecryptor())
                        {
                            lock (persistenceBouncer)
                            {
                                using (FileStream stream = new FileStream(DataFile, FileMode.OpenOrCreate))
                                {
                                    if (stream.Length == 0)
                                    {
                                        activeMemoryLock.EnterWriteLock();
                                        try
                                        {
                                            accounts = new List<IBankAccountMultipleCurrency>();
                                            isFlushed = true;
                                        }
                                        finally
                                        {
                                            ExitWriteLock();
                                        }

                                        return;
                                    }

                                    using (CryptoStream cryptoStream = new CryptoStream(stream, decryptor, CryptoStreamMode.Read))
                                    {
                                        using (GZipStream gzipStream = new GZipStream(cryptoStream, CompressionMode.Decompress))
                                        {
                                            SoapFormatter formatter = new SoapFormatter();
                                            ArrayList deserializedAccounts = (ArrayList)formatter.Deserialize(gzipStream);

                                            activeMemoryLock.EnterWriteLock();

                                            try
                                            {
                                                accounts = deserializedAccounts.Cast<IBankAccountMultipleCurrency>().ToList();
                                                isFlushed = true;
                                            }
                                            finally
                                            {
                                                ExitWriteLock();
                                            }
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Converts the generic collection of accounts to a non-generic collection and serializes it to disk.
        /// </summary>
        private void WriteAccounts()
        {
            ArrayList nonGenericAccounts = UngenericAccounts();

            if (nonGenericAccounts != null)
            {
                string encryptionKey = ConfigurationManager.AppSettings["encryptionKey"];
                byte[] encryptionSalt = Encoding.Unicode.GetBytes(ConfigurationManager.AppSettings["encryptionSalt"]);

                using (Rfc2898DeriveBytes rfc = new Rfc2898DeriveBytes(encryptionKey, encryptionSalt))
                {
                    using (AesManaged algorithm = new AesManaged())
                    {
                        algorithm.Padding = PaddingMode.PKCS7;
                        algorithm.Key = rfc.GetBytes(algorithm.KeySize / 8);
                        algorithm.IV = rfc.GetBytes(algorithm.BlockSize / 8);

                        using (ICryptoTransform encryptor = algorithm.CreateEncryptor())
                        {
                            lock (persistenceBouncer)
                            {
                                using (FileStream outFile = File.OpenWrite(DataFile))
                                {
                                    using (CryptoStream cryptoStream = new CryptoStream(outFile, encryptor, CryptoStreamMode.Write))
                                    {
                                        using (GZipStream zipStream = new GZipStream(cryptoStream, CompressionLevel.Optimal, true))
                                        {
                                            SoapFormatter formatter = new SoapFormatter();
                                            formatter.Serialize(zipStream, nonGenericAccounts);
                                            activeMemoryLock.EnterWriteLock();
                                            isFlushed = true;
                                            ExitWriteLock();
                                        }
                                    }
                                }
                            }
                        }
                    }
                }
            }
        }

        private ArrayList UngenericAccounts()
        {
            ArrayList ungenericAccounts = null;
            activeMemoryLock.EnterReadLock();

            try
            {
                if (!isFlushed)
                {
                    ungenericAccounts = new ArrayList(accounts);
                }
            }
            finally
            {
                ExitReadLock();
            }

            return ungenericAccounts;
        }
        #endregion Read/Write Methods

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
        private bool disposedValue;

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
                    WriteAccounts(); // Make sure in-memory data gets flushed to disk before the vault is garbage collected.
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

        #region Safe Lock Management
        /// <summary>
        /// If a read lock is held for the in-memory read/write lock, it will be released.
        /// </summary>
        private void ExitReadLock()
        {
            if (activeMemoryLock.IsReadLockHeld)
            {
                activeMemoryLock.ExitReadLock();
            }
        }

        /// <summary>
        /// If a write lock is held for the in-memory read/write lock, it will be released.
        /// </summary>
        private void ExitWriteLock()
        {
            if (activeMemoryLock.IsWriteLockHeld)
            {
                activeMemoryLock.ExitWriteLock();
            }
        }

        /// <summary>
        /// If an upgradeable read lock is held for the in-memory read/write lock, it will be released.
        /// </summary>
        private void ExitUpgradeableReadLock()
        {
            if (activeMemoryLock.IsUpgradeableReadLockHeld)
            {
                activeMemoryLock.ExitUpgradeableReadLock();
            }
        }
        #endregion Safe Lock Management
    }
}
