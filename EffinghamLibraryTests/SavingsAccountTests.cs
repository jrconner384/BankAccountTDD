using EffinghamLibrary;
using EffinghamLibrary.Accounts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EffinghamLibraryTests
{
    [TestClass]
    public class SavingsAccountTests
    {
        private IInterestBearing interestBearingAccount;

        private const string AccountHolderName = "Steve";
        private const decimal DefaultStartingBalance = 100.0m;

        [TestInitialize]
        public void Setup()
        {
            interestBearingAccount = new SavingsAccount(AccountHolderName, DefaultStartingBalance, CurrencyType.Dollar);
        }

        [TestMethod]
        public void MonthlyInterestAccumulationTest()
        {
            interestBearingAccount.AddMonthlyInterest();

            Assert.AreEqual(DefaultStartingBalance * SavingsAccount.MonthlyInterestRate + DefaultStartingBalance, interestBearingAccount.Balance);
        }
    }
}
