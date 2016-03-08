using System;

namespace EffinghamLibrary
{
    public class CheckingAccount : BankAccount
    {
        public CheckingAccount(string customerName, decimal startingBalance, CurrencyType currency = CurrencyType.Dollar)
            : base(customerName, startingBalance, currency)
        {

        }

        #region Methods

        public override void Withdraw(decimal amount)
        {
            if (amount <= 0)
            {
                throw new ApplicationException("Withdraw amount must be greater than zero.");
            }

            if (Balance - amount <= -50)
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
