using System.Collections.Generic;
using EffinghamLibrary.Accounts;
using EffinghamLibrary.Vaults;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace EffinghamLibraryTests
{
    [TestClass]
    public class SoapVaultTests
    {
        [TestMethod]
        public void GetAccountsReturnsNotNullEnumerableOfMultiCurrencyAccountsTest()
        {
            using (IVault vault = SoapVault.Instance)
            {
                var accounts = vault.GetAccounts();

                Assert.IsNotNull(accounts);
                // TODO: This was included as part of the example test but ReSharper says this statement is always true.
                Assert.IsTrue(accounts is IEnumerable<IBankAccountMultipleCurrency>);
            }
        }

        [TestMethod]
        public void AddAccountCreatesAnAccountInTheVaultUsingTheExpectedValuesFromTheProvidedAccountTest()
        {
            using (IVault vault = SoapVault.Instance)
            {
                IBankAccountMultipleCurrency firstAccount = new TestBankAccount("Test", 100.0m);
                vault.AddAccount(firstAccount);

                IBankAccountMultipleCurrency secondAccount = vault.GetAccount(firstAccount.AccountNumber);
                
                Assert.AreEqual(firstAccount.AccountNumber, secondAccount.AccountNumber);
                Assert.AreEqual(firstAccount.Balance, secondAccount.Balance);
                Assert.AreEqual(firstAccount.CustomerName, secondAccount.CustomerName);
                Assert.AreEqual(firstAccount.GetType(), secondAccount.GetType());
            }
        }
    }
}
