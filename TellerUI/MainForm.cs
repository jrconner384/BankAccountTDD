using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Forms;
using EffinghamLibrary;
using EffinghamLibrary.Accounts;
using EffinghamLibrary.Vaults;

namespace TellerUI
{
    public partial class MainForm : Form
    {
        #region Fields and Properties
        /// <summary>
        /// The vault which stores accounts for the bank.
        /// </summary>
        private readonly IVault vault;
        #endregion Fields and Properties

        #region Constructors
        public MainForm(IVault injectedVault)
        {
            InitializeComponent();
            vault = injectedVault;
        }
        #endregion Constructors

        #region Events
        private void btnNewAccount_Click(object sender, EventArgs e)
        {
            IBankAccountMultipleCurrency newAccount = CreateBankAccountBasedOnTypeSelection(
                txtCustomerName.Text,
                decimal.Parse(txtStartingAmount.Text),
                (CurrencyType)cmbCurrencyType.SelectedIndex);

            vault.AddAccount(newAccount);
            SummarizeAccounts();
        }

        /// <summary>
        /// Guarantees that form controls, especially concerning bound data, are correctly set up.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            cmbCurrencyType.Items.AddRange(Enum.GetNames(typeof(CurrencyType)));
            cmbCurrencyType.SelectedIndex = 0;
            cmbSort.SelectedIndex = 0;
            cmbFilter.SelectedIndex = 0;
            SummarizeAccounts();
        }

        /// <summary>
        /// Disposes of resources, such as the vault.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            vault.Dispose();
        }
        #endregion Events

        #region Helpers
        /// <summary>
        /// Returns a concrete derivation of a BankAccount based on the type selection
        /// on the form.
        /// </summary>
        /// <param name="customerName">The name of the account holder.</param>
        /// <param name="startingAmount">The first deposit into the account.</param>
        /// <param name="currency">The currency type of the deposit.</param>
        /// <returns>The concrete derivation of BankAccount corresponding to the selection in the UI.</returns>
        private BankAccount CreateBankAccountBasedOnTypeSelection(string customerName, decimal startingAmount, CurrencyType currency)
        {
            if (radChecking.Checked)
            {
                return new CheckingAccount(customerName, startingAmount, currency);
            }

            if (radSavings.Checked)
            {
                return new SavingsAccount(customerName, startingAmount, currency);
            }

            throw new ApplicationException("Please select a valid account type.");
        }

        /// <summary>
        /// Generates a text summary of bank assets.
        /// </summary>
        private void SummarizeAccounts()
        {
            lblSummary.Text = $"{GetSumOfAllBalances():c} in {GetNumberOfCheckingAccounts()} Checking and {GetNumberOfSavingsAccounts()} Savings Accounts";
            RefreshAccountsList();
        }

        /// <summary>
        /// Calculates the number of savings accounts in the bank's vault.
        /// </summary>
        /// <returns>The total number of savings accounts in the bank's vault.</returns>
        private int GetNumberOfSavingsAccounts()
        {
            return vault.GetAccounts().OfType<SavingsAccount>().Count();
        }

        /// <summary>
        /// Calculates the number of checking accounts in the bank's vault.
        /// </summary>
        /// <returns>The total number of checking accounts in the bank's vault.</returns>
        private int GetNumberOfCheckingAccounts()
        {
            return vault.GetAccounts().OfType<CheckingAccount>().Count();
        }

        /// <summary>
        /// Calculates the sum of all accounts' balances in the bank's vault.
        /// </summary>
        /// <returns>The sum of all accounts' balances in the bank's vault.</returns>
        private decimal GetSumOfAllBalances()
        {
            return vault.GetAccounts().Sum(account => account.Balance);
        }

        private int SortByCustomerNameAndBalanceDescending(IBankAccountMultipleCurrency firstAccount, IBankAccountMultipleCurrency secondAccount)
        {
            int result = string.Compare(firstAccount.CustomerName, secondAccount.CustomerName, StringComparison.Ordinal);

            if (result == 0)
            {
                result = -1 * firstAccount.Balance.CompareTo(secondAccount.Balance);
            }
            return result;
        }

        /// <summary>
        /// Syncs the list of accounts with the in-memory collection in the vault.
        /// </summary>
        private void RefreshAccountsList()
        {
            lstAccounts.Items.Clear();

            foreach (IBankAccountMultipleCurrency account in GetFilteredAccountCollection(SortAccountsByUserSelection()))
            {
                lstAccounts.Items.Add(account);
            }
        }

        private IEnumerable<IBankAccountMultipleCurrency> SortAccountsByUserSelection()
        {
            List<IBankAccountMultipleCurrency> sortedAccounts = vault.GetAccounts().ToList();

            switch (cmbSort.SelectedIndex)
            {
                case 0: // Account number
                    sortedAccounts.Sort((a, b) => a.AccountNumber.CompareTo(b.AccountNumber));
                    break;
                case 1: // Customer name
                    sortedAccounts.Sort(SortByCustomerNameAndBalanceDescending);
                    break;
                case 2: // Account balance descending
                    sortedAccounts.Sort((a, b) => -1 * a.Balance.CompareTo(b.Balance));
                    break;
                default:
                    throw new ApplicationException("An unsupported sort order was attempted.");
            }

            return sortedAccounts;
        } 

        private IEnumerable<IBankAccountMultipleCurrency> GetFilteredAccountCollection(IEnumerable<IBankAccountMultipleCurrency> sortedAccounts)
        {
            IEnumerable<IBankAccountMultipleCurrency> filterQuery = from account in sortedAccounts
                                                                    select account;

            if (cmbFilter.SelectedIndex == 1) // Savings
            {
                filterQuery = from account in filterQuery
                              where account is SavingsAccount
                              select account;
            }
            else if (cmbFilter.SelectedIndex == 2) // Checking
            {
                filterQuery = from account in filterQuery
                              where account is CheckingAccount
                              select account;
            }

            return filterQuery;
        } 
        #endregion Helpers

        private void cmbSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            SummarizeAccounts();
        }

        private void cmbFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            SummarizeAccounts();
        }
    }
}
