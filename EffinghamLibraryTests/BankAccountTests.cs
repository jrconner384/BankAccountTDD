﻿using System;
using System.Threading.Tasks;
using EffinghamLibrary;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EffinghamLibraryTests
{
    // nUnit and xUnit are common replacements to MSTest
    [TestClass]
    public class BankAccountTests
    {
        private const string DefaultName = "Steve";
        private const decimal DefaultStartingBalance = 10.0m;

        private IBankAccount account;

        [TestInitialize]
        public void Setup()
        {
            account = new BankAccount(DefaultName, DefaultStartingBalance);
        }

        [TestMethod]
        public void NewAccountsAreAssignedSequentialValuesTest()
        {
            IBankAccount secondAccount = new BankAccount(DefaultName, DefaultStartingBalance);

            Assert.AreEqual(account.AccountNumber + 1, secondAccount.AccountNumber);
        }

        [TestMethod]
        public void AccountCustomerNameCorrectlyUpdatesValueAfterAssignmentTest()
        {
            const string testName = "Jeff";

            account.CustomerName = testName;

            Assert.AreNotEqual(DefaultName, account.CustomerName, $"The customer's name, {account.CustomerName}, was equal to the default name, {DefaultName}, selected by the test class.");
            Assert.AreEqual("Jeff", account.CustomerName, $"{account.CustomerName} should be {testName}");
        }

        [TestMethod]
        public void AccountBalanceCorrectlyUpdatesAfterMakingDepositTest()
        {
            const decimal depositAmount = 100.0m;
            decimal originalBalance = account.Balance;

            account.Deposit(depositAmount);

            Assert.AreEqual(depositAmount + originalBalance, account.Balance, $"{account.Balance} should have been {depositAmount}.");
        }

        [TestMethod]
        public void AccountBalanceCorrectlyUpdatesAfterMakingWithdrawTest()
        {
            decimal withdrawAmount = account.Balance / 2.0m;
            decimal balanceBeforeWithdraw = account.Balance;

            account.Withdraw(withdrawAmount);

            Assert.AreEqual(balanceBeforeWithdraw - withdrawAmount, account.Balance, $"{account.Balance} should have been {balanceBeforeWithdraw - withdrawAmount}");
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException), "This should have received an ApplicationException explaining that the string was too short.")]
        public void CannotAssignAnAccountNameLessThanTwoCharactersLongBizRuleTest()
        {
            const string badName = "a";

            account.CustomerName = badName;
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CannotDepositLargeNegativeAmountBizRuleTest()
        {
            const decimal negativeValue = -10.0m;

            account.Deposit(negativeValue);
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CannotDepositSmallNegativeAmountBizRuleTest()
        {
            const decimal smallNegativeValue = -0.01m;

            account.Deposit(smallNegativeValue);
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CannotDepositZeroBizRuleTest()
        {
            const decimal zero = 0.0m;

            account.Deposit(zero);
        }

        [TestMethod]
        public void CanDepositFractionOfACentBizRuleTest()
        {
            const decimal smallPositiveValue = 0.00001m;

            account.Deposit(smallPositiveValue);
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CannotWithdrawIfBalanceIsLessThanWithdrawAmountBizRuleTest()
        {
            const decimal startingBalance = 100.0m;
            const decimal largeWithdraw = startingBalance * 2;
            account.Deposit(startingBalance);

            account.Withdraw(largeWithdraw);
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void CannotWithdrawZeroBizRuleTest()
        {
            const decimal zero = 0.0m;

            account.Withdraw(zero);
        }

        [TestMethod]
        public void TestSetupInitializesTestAccountTest()
        {
            Assert.IsNotNull(account, $"The BankAccount object, {account}, was not initialized by the [TestInitialize] method.");
        }

        [TestMethod]
        public void TestSetupInstantiatesTestAccountAsBankAccountTypeTest()
        {
            Assert.IsTrue(account is BankAccount, $"The test account is not the expected type: {typeof(BankAccount)}");
        }

        [TestMethod]
        public void TestSetupAssignsCorrectStartingBalanceToTestAccountTest()
        {
            Assert.AreEqual(DefaultStartingBalance, account.Balance, $"The test account should always start with {DefaultStartingBalance}");
        }

        [TestMethod]
        public void TestSetupAssignsCorrectDefaultNameToTestAccountTest()
        {
            Assert.AreEqual(DefaultName, account.CustomerName, $"The test account should start with the customer name {DefaultName}");
        }

        [TestMethod]
        [ExpectedException(typeof(NullReferenceException))]
        public void ConstructorThrowsExceptionIfGivenNullStringForCustomerNameTest()
        {
            IBankAccount badInitialize = new BankAccount(null, DefaultStartingBalance);
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void ConstructorThrowsExceptionIfGivenNegativeStartingBalanceTest()
        {
            const decimal negative = -1.0m;
            IBankAccount badInitialize = new BankAccount(DefaultName, negative);
        }

        [TestMethod]
        [ExpectedException(typeof(ApplicationException))]
        public void ConstructorThrowsExceptionIfGivenOneCharacterStringTest()
        {
            const string oneChar = "a";
            IBankAccount badInitialize = new BankAccount(oneChar, DefaultStartingBalance);
        }

        [TestMethod]
        public void ParallelDepositsAndWithdrawsDontCorruptBalanceTest()
        {
            const string newName = "Test";
            const decimal newStartingBalance = 10000m;
            IBankAccount secondAccount = new BankAccount(newName, newStartingBalance);

            Parallel.Invoke(() =>
                            {
                                for (int i = 0; i < 10000; i++)
                                {
                                    secondAccount.Deposit(0.33m);
                                }
                            },
                            () =>
                            {
                                for (int i = 0; i < 10000; i++)
                                {
                                    secondAccount.Withdraw(0.33m);
                                }
                            },
                            () =>
                            {
                                for (int i = 0; i < 10000; i++)
                                {
                                    secondAccount.Deposit(0.09m);
                                }
                            },
                            () =>
                            {
                                for (int i = 0; i < 10000; i++)
                                {
                                    secondAccount.Withdraw(0.09m);
                                }
                            });

            Assert.AreEqual(newStartingBalance, secondAccount.Balance);
        }
    }
}
