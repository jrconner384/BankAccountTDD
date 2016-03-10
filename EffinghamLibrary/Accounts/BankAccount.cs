using System;
using System.Runtime.Serialization;

namespace EffinghamLibrary.Accounts
{
    /// <summary>
    /// Represents a general account, connecting a customer's name to an account balance.
    /// </summary>
    [Serializable]
    public abstract class BankAccount :
        IBankAccountMultipleCurrency,
        ISerializable
    {
        #region Fields and Properties
        #region Serialization Keys
        /// <summary>
        /// Describes the string value used to serialize and deserialize the AccountNumber property.
        /// </summary>
        private const string AccountNumberKey = "AccountNumber";

        /// <summary>
        /// Describes the string value used to serialize and deserialize the Balance property.
        /// </summary>
        private const string BalanceKey = "Balance";

        /// <summary>
        /// Describes the string value used to serialize and deserialize the CustomerName property.
        /// </summary>
        private const string CustomerNameKey = "CustomerName";
        #endregion Serialization Keys

        private readonly int accountNumber;

        protected decimal balance;

        /// <summary>
        /// Locks critical sections for static members.
        /// </summary>
        protected static readonly object ClassBouncer;

        /// <summary>
        /// Backing field for the account holder's name
        /// </summary>
        private string customerName;

        private const CurrencyType DefaultCurrencyType = CurrencyType.Dollar;

        /// <summary>
        /// Locks critical sections for instance members.
        /// </summary>
        protected readonly object instanceBouncer;

        /// <summary>
        /// Holds the value to assign to the next new account.
        /// </summary>
        private static int nextAccountNumber;

        /// <summary>
        /// The unique number identifying a specific account.
        /// </summary>
        public int AccountNumber
        {
            get
            {
                lock (instanceBouncer)
                {
                    return accountNumber;
                }
            }
        }

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
            ClassBouncer = new object();

            // There's no data store. Doing this to demonstrate functionality and make it testable.
            // This should probably be extracted to a GetNextAccount() method
            nextAccountNumber = 1;
        }

        private BankAccount()
        {
            instanceBouncer = new object();
        }

        public BankAccount(string customerName, decimal startingBalance, CurrencyType currency = DefaultCurrencyType)
            : this()
        {
            // If locking on both the class and the instance, we'll have to stick to the same nesting order
            // (i.e. classBouncer outside, instanceBouncer inside).
            lock (ClassBouncer)
            {
                lock (instanceBouncer)
                {
                    accountNumber = nextAccountNumber++;
                }
            }

            lock (instanceBouncer)
            {
                balance = 0.0m;
            }

            CustomerName = customerName;
            Deposit(startingBalance, currency);
        }

        /// <summary>
        /// Deserialize an account from serialized data.
        /// </summary>
        /// <param name="info">The serialized data corresponding to a BankAccount.</param>
        /// <param name="context"></param>
        /// <remarks>
        /// This is marked internal since there's no reason for external consumers to try to use this.
        /// </remarks>
        protected internal BankAccount(SerializationInfo info, StreamingContext context)
            : this()
        {
            lock (instanceBouncer)
            {
                accountNumber = info.GetInt32(AccountNumberKey);
                balance = info.GetDecimal(BalanceKey);
                customerName = info.GetString(CustomerNameKey);
            }

            // Need to do this in a separate set of nested locks since it locks on both the class and the instance.
            // The established order of the locks needs to be consistent.
            lock (ClassBouncer)
            {
                lock (instanceBouncer)
                {
                    if (nextAccountNumber <= accountNumber)
                    {
                        nextAccountNumber = accountNumber + 1;
                    }
                }
            }
        }
        #endregion Constructors

        #region Methods
        /// <summary>
        /// Deposits the specified amount as the default currency.
        /// </summary>
        /// <param name="amount">The amount to deposit.</param>
        public virtual void Deposit(decimal amount)
        {
            Deposit(amount, DefaultCurrencyType);
        }

        /// <summary>
        /// Deposits the specified amount as the specified currency.
        /// </summary>
        /// <param name="amount">The amount to deposit.</param>
        /// <param name="currency">The currency to perform the deposit as.</param>
        public virtual void Deposit(decimal amount, CurrencyType currency)
        {
            if (amount <= 0)
            {
                throw new ApplicationException($"{amount} is less than or equal to zero. Valid deposits are greater than zero.");
            }

            lock (instanceBouncer)
            {
                balance += ConvertCurrency(amount, currency);
            }
        }

        /// <summary>
        /// Withdraws the specified amount as the default currency.
        /// </summary>
        /// <param name="amount">The amount to withdraw from the account.</param>
        public virtual void Withdraw(decimal amount)
        {
            // TODO: Probably need to add an overload to withdraw a specified currency.
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

        #region ISerializable
        /// <summary>
        /// Serializes the account for storage.
        /// </summary>
        /// <param name="info">The structure to serialize data into.</param>
        /// <param name="context"></param>
        public virtual void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue(AccountNumberKey, AccountNumber);
            info.AddValue(BalanceKey, Balance);
            info.AddValue(CustomerNameKey, CustomerName);
        }
        #endregion ISerializable

        #region Helpers
        /// <summary>
        /// Given an amount of money, this converts it from the default currency to the currency specified.
        /// </summary>
        /// <param name="amount"></param>
        /// <param name="toConvertTo"></param>
        /// <returns></returns>
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

        #region Object Overrides
        /// <summary>
        /// Returns a string that represents the account.
        /// </summary>
        /// <returns>
        /// A string that represents the account.
        /// </returns>
        public override string ToString()
        {
            return $"Account {AccountNumber}: \t{CustomerName}\t balance: {Balance:c}";
        }
        #endregion Object Overrides
    }
}
