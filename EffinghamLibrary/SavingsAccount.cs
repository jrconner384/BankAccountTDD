namespace EffinghamLibrary
{
    public class SavingsAccount : BankAccount, IInterestBearing
    {
        #region Fields and Properties
        private static decimal AnnualInterestRate { get; }

        public static decimal MonthlyInterestRate { get; }
        #endregion Fields and Properties

        #region Constructors

        static SavingsAccount()
        {
            AnnualInterestRate = 0.03m;
            MonthlyInterestRate = AnnualInterestRate / 12;
        }

        public SavingsAccount(string customerName, decimal openingBalance, CurrencyType currency)
            : base(customerName, openingBalance, currency)
        {
            
        }
        #endregion Constructors

        #region Methods
        public void AddMonthlyInterest()
        {
            lock (bouncer)
            {
                balance *= 1 + MonthlyInterestRate;
            }
        }
        #endregion Methods
    }
}
