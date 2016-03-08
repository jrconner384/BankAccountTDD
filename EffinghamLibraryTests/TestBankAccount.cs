using System;
using System.Runtime.Serialization;
using EffinghamLibrary;
using EffinghamLibrary.Accounts;

namespace EffinghamLibraryTests
{
    [Serializable]
    internal class TestBankAccount : BankAccount
    {
        public TestBankAccount(string customerName, decimal startingBalance, CurrencyType currency = CurrencyType.Dollar)
            : base(customerName, startingBalance, currency)
        {
        }

        public TestBankAccount(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
            
        }
    }
}
