using EffinghamLibrary.Accounts;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EffinghamLibraryTests
{
    [TestClass]
    public class CheckingAccountTests
    {
        private IBankAccount checkingAccount;
        private const string DefaultAccountHoldersName = "Steve";
        private const decimal DefaultOpeningBalance = 10.0m;

        [TestInitialize]
        public void Setup()
        {
            checkingAccount = new CheckingAccount(DefaultAccountHoldersName, DefaultOpeningBalance);
        }

        [TestMethod]
        public void OverdraftCorrectlyUpdatesAccountBalanceTest()
        {
            const decimal amountToOverdrawBy = 10.0m;
            const decimal expectedAmount = 0 - amountToOverdrawBy;

            checkingAccount.Withdraw(DefaultOpeningBalance + amountToOverdrawBy);

            Assert.AreEqual(expectedAmount, checkingAccount.Balance);
        }
    }
}
