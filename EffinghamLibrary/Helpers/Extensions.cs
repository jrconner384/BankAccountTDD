using EffinghamDataAccess;
using EffinghamLibrary.Accounts;

namespace EffinghamLibrary.Helpers
{
    internal static class Extensions
    {
        /// <summary>
        /// Prepares an <see cref="AccountData"/> for use in memory.
        /// </summary>
        /// <param name="accountData">The account data as produced by the ORM.</param>
        /// <returns>A business object in a form understood by the business logic layer.</returns>
        public static IBankAccountMultipleCurrency MarshallBankAccount(this AccountData accountData)
        {
            // The method of using single characters to identify an account type is incredibly brittle but it's being used as
            // an expedient means of demonstrating the ORM and programmer's ability to write stuff on top of it.
            switch (accountData.AccountType)
            {
                case "C":
                    return new CheckingAccount(accountData.AccountNumber, accountData.CustomerName, accountData.Balance);
                case "S":
                    return new SavingsAccount(accountData.AccountNumber, accountData.CustomerName, accountData.Balance);
                default:
                    throw new InvalidAccountTypeException(
                        $"The AccountData {accountData.AccountNumber}: {accountData.CustomerName} with type {accountData.AccountType} is not recognized.");
            }
        }

        /// <summary>
        /// Prepares an <see cref="IBankAccountMultipleCurrency"/> for persistence in the database by converting it
        /// to a data type the ORM layer understands.
        /// </summary>
        /// <param name="account">The account to prepare for persistence.</param>
        /// <returns>The unmarshalled IBankAccountMultipleCurrency which the ORM can persist in the database.</returns>
        public static AccountData UnmarshallBankAccount(this IBankAccountMultipleCurrency account)
        {
            return
                new AccountData
                {
                    AccountNumber = account.AccountNumber,
                    CustomerName = account.CustomerName,
                    Balance = account.Balance,
                    AccountType = account is SavingsAccount ? "S" : "C"
                };
        }
    }
}
