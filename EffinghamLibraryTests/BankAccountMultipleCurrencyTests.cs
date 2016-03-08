using System;
using EffinghamLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EffinghamLibraryTests
{
    [TestClass]
    public class BankAccountMultipleCurrencyTests
    {
        private IBankAccountMultipleCurrency account;
        private const string TestName = "Steve";
        private const decimal StartingBalance = 10.0m;

        [TestInitialize]
        public void Setup()
        {
            account = new BankAccount(TestName, StartingBalance);
        }

        [TestMethod]
        public void DepositingPesosInAnAccountCorrectlyConvertsToTheDefaultCurrencyTest()
        {
            const decimal depositAmount = 1000.0m;
            const decimal startingBalanceInDollarsPlusPesoDeposit = StartingBalance + depositAmount / 10;

            account.Deposit(depositAmount, CurrencyType.Peso);

            Assert.AreEqual(startingBalanceInDollarsPlusPesoDeposit, account.Balance);
        }

        [TestMethod]
        public void DepositingYenInAnAccountCorrectlyConvertsToTheDefaultCurrencyTest()
        {
            const decimal depositAmount = 1000.0m;
            const decimal startingBalanceInDollarsPlusYenDeposit = StartingBalance + depositAmount / 100;

            account.Deposit(depositAmount, CurrencyType.Yen);

            Assert.AreEqual(startingBalanceInDollarsPlusYenDeposit, account.Balance);
        }

        [TestMethod]
        public void ConstructorWithPesoSuccessfullyConvertsToDefaultCurrencyTest()
        {
            const string pesoCustomer = "Juan";
            const decimal openingBalanceInPesos = 1000.0m;
            const decimal openingBalanceInDollars = openingBalanceInPesos / 10;
            const CurrencyType pesos = CurrencyType.Peso;
            IBankAccountMultipleCurrency pesoAccount = new BankAccount(pesoCustomer, openingBalanceInPesos, pesos);

            Assert.AreEqual(openingBalanceInDollars, pesoAccount.Balance);
        }

        [TestMethod]
        public void ConstructorWithYenSuccessfullyConvertsToDefaultCurrencyTest()
        {
            const string yenCustomer = "Hiro";
            const decimal openingBalanceInYen = 1000.0m;
            const decimal openingBalanceInDollars = openingBalanceInYen / 100;
            const CurrencyType yen = CurrencyType.Yen;
            IBankAccountMultipleCurrency yenAccount = new BankAccount(yenCustomer, openingBalanceInYen, yen);

            Assert.AreEqual(openingBalanceInDollars, yenAccount.Balance);
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void ConstructorWithUnknownCurrencyThrowsExceptionTest()
        {
            const CurrencyType newType = (CurrencyType)int.MaxValue;
            const decimal depositAmount = 10.0m;

            account.Deposit(depositAmount, newType);
        }
    }
}
