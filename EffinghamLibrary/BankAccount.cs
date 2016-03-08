using System;

namespace EffinghamLibrary
{
    public enum CurrencyType
    {
        // Can mark each enum element with any integer value.
        // Might want to set them equal to 2^x, use [Flags] on the enum. Then you can do bitwise operators:
        // [Flags]
        // enum Things
        // {
        //     oneThing = 1,
        //     otherThing = 2,
        //     thisThing = 4
        // }
        // Things thing = Things.oneThing | Things.thisThing;
        Dollar,
        Peso,
        Yen
    }

    /// <summary>
    /// Represents an account, connecting a customer's name to an account balance.
    /// </summary>
    public class BankAccount : IBankAccountMultipleCurrency
    {
        #region Fields and Properties

        protected decimal balance;

        protected static readonly object classBouncer;

        /// <summary>
        /// Backing field for the account holder's name
        /// </summary>
        private string customerName;

        private const CurrencyType DefaultCurrencyType = CurrencyType.Dollar;

        protected readonly object instanceBouncer;

        private static int nextAccountNumber;

        public int AccountNumber { get; }

        /// <summary>
        /// Represents the monetary balance of the account.
        /// </summary>
        public decimal Balance
        {
            get
            {
                lock (instanceBouncer)
                {
                    return balance;
                }
            }
        }

        /// <summary>
        /// Represents the name of the customer.
        /// </summary>
        public string CustomerName
        {
            get
            {
                lock (instanceBouncer)
                {
                    return customerName;
                }
            }

            set
            {
                string cleanName = value.Trim();

                if (cleanName.Length > 1)
                {
                    lock (instanceBouncer)
                    {
                        customerName = cleanName;
                    }
                }
                else
                {
                    throw new ApplicationException($"{cleanName} is too short. The customer's name must be at least two characters long.");
                }
            }
        }
        #endregion Fields and Properties

        #region Constructors
        static BankAccount()
        {
            classBouncer = new object();
            nextAccountNumber = 1; // There's no data store. Doing this to demonstrate functionality and make it testable.
        }

        public BankAccount(string customerName, decimal startingBalance, CurrencyType currency = DefaultCurrencyType)
        {
            instanceBouncer = new object();

            lock (classBouncer)
            {
                lock (instanceBouncer)
                {
                    AccountNumber = nextAccountNumber++;
                }
            }

            lock (instanceBouncer)
            {
                balance = 0.0m;
            }

            CustomerName = customerName;
            Deposit(startingBalance, currency);
        }
        #endregion Constructors

        #region Methods
        public virtual void Deposit(decimal amount)
        {
            Deposit(amount, DefaultCurrencyType);
        }

        public virtual void Deposit(decimal amount, CurrencyType currency)
        {
            if (amount <= 0)
            {
                throw new ApplicationException($"{amount} is less than or equal to zero. Valid deposits are greater than zero.");
            }

            lock (instanceBouncer)
            {
                balance += ConvertCurrency(amount, currency); ;
            }
        }

        public virtual void Withdraw(decimal amount)
        {
            if (amount <= 0)
            {
                throw new ApplicationException($"The requested withdraw amount, {amount}, is a non-positive number.");
            }

            // Might lead to a race condition. Could be better to remove locking from the Balance
            // property and lock around this if AND the assignment to the balance field.
            if (Balance - amount <= 0) // Balance property has locking, don't need to lock here.
            {
                throw new ApplicationException($"The account has insufficient funds. Cannot withdraw {amount} from {Balance}.");
            }

            lock (instanceBouncer) // balance field doesn't have locking. Need to lock.
            {
                balance -= amount;
            }
        }
        #endregion Methods

        #region Helpers
        private static decimal ConvertCurrency(decimal amount, CurrencyType toConvertTo)
        {
            switch (toConvertTo)
            {
                case CurrencyType.Dollar:
                    return amount;
                case CurrencyType.Peso:
                    return amount / 10;
                case CurrencyType.Yen:
                    return amount / 100;
                default:
                    throw new ApplicationException("At least one of the currency types hasn't been included in currency conversion logic. Blame the business logic developer.");
            }
        }
        #endregion Helpers
    }
}
