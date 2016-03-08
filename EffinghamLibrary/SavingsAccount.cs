using System.Runtime.Serialization;

namespace EffinghamLibrary
{
    /// <summary>
    /// Represents an interest-bearing savings account.
    /// </summary>
    public class SavingsAccount : BankAccount, IInterestBearing
    {
        #region Fields and Properties
        /// <summary>
        /// Describes the annual percent interest gain on this account.
        /// </summary>
        private static decimal AnnualInterestRate { get; }

        /// <summary>
        /// Describes the monthly percent interest gain on this account.
        /// </summary>
        public static decimal MonthlyInterestRate { get; }
        #endregion Fields and Properties

        #region Constructors
        /// <summary>
        /// Sets up the class' interest rate information.
        /// </summary>
        static SavingsAccount()
        {
            AnnualInterestRate = 0.03m;
            MonthlyInterestRate = AnnualInterestRate / 12;
        }

        /// <summary>
        /// Initializes a SavingsAccount with the specified name, opening balance, and currency type.
        /// </summary>
        /// <param name="customerName">The name of the customer opening the account.</param>
        /// <param name="openingBalance">The amount the customer is depositing.</param>
        /// <param name="currency">The currency being deposited.</param>
        public SavingsAccount(string customerName, decimal openingBalance, CurrencyType currency)
            : base(customerName, openingBalance, currency)
        {
            // TODO: Can I make the CurrencyType param default to CurrencyType.Dollar?
        }

        /// <summary>
        /// Deserializes stored data into a SavingsAccount.
        /// </summary>
        /// <param name="info">The serialized data to marshall the SavingsAccount from.</param>
        /// <param name="context"></param>
        internal SavingsAccount(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {

        }
        #endregion Constructors

        #region Methods
        /// <summary>
        /// Adds interest earned to the savings account.
        /// </summary>
        public void AddMonthlyInterest()
        {
            lock (instanceBouncer)
            {
                balance *= 1 + MonthlyInterestRate;
            }
        }
        #endregion Methods
    }
}
