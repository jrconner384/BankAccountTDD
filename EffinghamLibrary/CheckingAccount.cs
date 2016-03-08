using System;
using System.Runtime.Serialization;

namespace EffinghamLibrary
{
    /// <summary>
    /// Represents a checking account which allows overdrafting.
    /// </summary>
    [Serializable]
    public class CheckingAccount : BankAccount
    {
        #region Fields and Properties
        private static readonly decimal AccountMinimumBalance;
        #endregion Fields and Properties

        #region Constructors
        /// <summary>
        /// Initializes constant data common to all CheckingAccounts.
        /// </summary>
        static CheckingAccount()
        {
            AccountMinimumBalance = -50.0m;
        }

        /// <summary>
        /// Instantiates the checking account. If no CurrencyType is specified, the default value is used.
        /// </summary>
        /// <param name="customerName">The name of the account holder.</param>
        /// <param name="startingBalance">The amount the account holder is depositing to open the account.</param>
        /// <param name="currency">Optionally: the CurrencyType being deposited.</param>
        public CheckingAccount(string customerName, decimal startingBalance, CurrencyType currency = CurrencyType.Dollar)
            : base(customerName, startingBalance, currency)
        {

        }

        /// <summary>
        /// Deserializes a CheckingAccount from serialized data.
        /// </summary>
        /// <param name="info">The serialized data corresponding to a CheckingAccount.</param>
        /// <param name="context"></param>
        internal CheckingAccount(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            
        }
        #endregion Constructors

        #region Methods
        /// <summary>
        /// Withraws the specified amount from the checking account. This allows overdrafts to some limit specified
        /// by this class.
        /// </summary>
        /// <param name="amount">The amount to withdraw from the account.</param>
        public override void Withdraw(decimal amount)
        {
            // TODO: Extract method.
            if (amount <= 0)
            {
                throw new ApplicationException("Withdraw amount must be greater than zero.");
            }

            // TODO: Extract method.
            if (Balance - amount <= AccountMinimumBalance)
            {
                throw new ApplicationException("Insufficient funds to perform this overdraw action.");
            }

            lock (instanceBouncer)
            {
                balance -= amount;
            }
        }
        #endregion Methods
    }
}
