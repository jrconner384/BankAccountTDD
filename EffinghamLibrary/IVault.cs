using System.Collections.Generic;

namespace EffinghamLibrary
{
    public interface IVault
    {
        /// <summary>
        /// Retrieves an enumerable collection of IBankAccountMultipleCurrency intended to be read-only.
        /// </summary>
        /// <returns></returns>
        IEnumerable<IBankAccountMultipleCurrency> GetAccounts();

        /// <summary>
        /// Retrieves a single IBankAccountMultipleCurrency using the AccountNumber as the unique identifier.
        /// </summary>
        /// <param name="accountNumber"></param>
        /// <returns></returns>
        IBankAccountMultipleCurrency GetAccount(int accountNumber);

        /// <summary>
        /// Adds the provided account and optionally delays persisting the data.
        /// </summary>
        /// <param name="account">The account to add.</param>
        /// <param name="delayWrite">The flag indicating if the account should immediately be persisted or
        /// stored in active memory for later persistence.</param>
        void AddAccount(IBankAccountMultipleCurrency account, bool delayWrite = false);

        /// <summary>
        /// Updates the provided account and optionally delays persisting the change.
        /// </summary>
        /// <param name="account">The account to update.</param>
        /// <param name="delayWrite">The flag indicating if the account should immediately be updated
        /// in the data store or only in active memory until explicitly flushed later.</param>
        void UpdateAccount(IBankAccountMultipleCurrency account, bool delayWrite = false);

        /// <summary>
        /// Deletes the specified account and optionally delays persisting the change.
        /// </summary>
        /// <param name="account">The account to delete.</param>
        /// <param name="delayWrite">The flag indicating if the account should immediately be
        /// deleted from the data store or only from active memory until explicitly flushed later.</param>
        void DeleteAccount(IBankAccountMultipleCurrency account, bool delayWrite = false);

        /// <summary>
        /// Updates accounts queued in active memory as indicated by delayWrite flags.
        /// </summary>
        void FlushAccounts();
    }
}
