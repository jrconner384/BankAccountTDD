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
            decimal openingDeposit;

            if (!decimal.TryParse(txtStartingAmount.Text, out openingDeposit))
            {
                MessageBox.Show(@"Invalid starting amount");
                txtStartingAmount.Focus();
                txtStartingAmount.SelectAll();
                return;
            }

            try
            {
                vault.AddAccount(
                    CreateBankAccountBasedOnTypeSelection(
                        txtCustomerName.Text,
                        openingDeposit,
                        (CurrencyType)cmbCurrencyType.SelectedIndex));
                SummarizeAccounts();
            }
            catch (ApplicationException ae)
            {
                // TODO: Log exception info once logging is enabled
                MessageBox.Show(
                    ae.Message,
                    @"Business Rule Violation",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }
            catch (Exception)
            {
                // TODO: Log exception details. The user won't get any useful information from this exception.
                MessageBox.Show(@"Please try again. If this continues, contact support.");
            }
        }

        /// <summary>
        /// Guarantees that form controls, especially concerning bound data, are correctly set up.
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void MainForm_Load(object sender, EventArgs e)
        {
            cmbCurrencyType.Items.AddRange(Enum.GetNames(typeof(CurrencyType)));

            UnhookRefreshEvents();
            cmbCurrencyType.SelectedIndex = 0;
            cmbSort.SelectedIndex = 0;
            cmbFilter.SelectedIndex = 0;
            SummarizeAccounts();
            RehookRefreshEvents();
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

        private void cmbSort_SelectedIndexChanged(object sender, EventArgs e)
        {
            SummarizeAccounts();
        }

        private void cmbFilter_SelectedIndexChanged(object sender, EventArgs e)
        {
            SummarizeAccounts();
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
            try
            {
                return vault.GetAccounts().OfType<SavingsAccount>().Count();
            }
            catch (ApplicationException ae)
            {
                MessageBox.Show(ae.Message, @"Couldn't Count Number of Savings Accounts", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return 0;
        }

        /// <summary>
        /// Calculates the number of checking accounts in the bank's vault.
        /// </summary>
        /// <returns>The total number of checking accounts in the bank's vault.</returns>
        private int GetNumberOfCheckingAccounts()
        {
            try
            {
                return vault.GetAccounts().OfType<CheckingAccount>().Count();
            }
            catch (ApplicationException ae)
            {
                MessageBox.Show(ae.Message, @"Couldn't Count Number of Checking Accounts", MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            return 0;
        }

        /// <summary>
        /// Calculates the sum of all accounts' balances in the bank's vault.
        /// </summary>
        /// <returns>The sum of all accounts' balances in the bank's vault.</returns>
        private decimal GetSumOfAllBalances()
        {
            try
            {
                return vault.GetAccounts().Sum(account => account.Balance);
            }
            catch (ApplicationException ae)
            {
                MessageBox.Show(
                    ae.Message,
                    @"Couldn't Sum All Account Balances",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Error);
            }

            return 0.0m;
        }

        private static int SortByCustomerNameAndBalanceDescending(IBankAccountMultipleCurrency firstAccount, IBankAccountMultipleCurrency secondAccount)
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
        private async void RefreshAccountsList()
        {
            lstAccounts.Items.Clear();
            List <IBankAccountMultipleCurrency> accounts = (await vault.GetAccountsAsync()).ToList();

            foreach (IBankAccountMultipleCurrency account in GetFilteredAccountCollection(SortAccountsByUserSelection(accounts)))
            {
                lstAccounts.Items.Add(account);
            }
        }

        private IEnumerable<IBankAccountMultipleCurrency> SortAccountsByUserSelection(List<IBankAccountMultipleCurrency> accounts)
        {
            switch (cmbSort.SelectedIndex)
            {
                case 0: // Account number
                    accounts.Sort(
                        (a, b) =>
                            a.AccountNumber.CompareTo(b.AccountNumber));
                    break;
                case 1: // Customer name
                    accounts.Sort(SortByCustomerNameAndBalanceDescending);
                    break;
                case 2: // Account balance descending
                    accounts.Sort((a, b) => -1 * a.Balance.CompareTo(b.Balance));
                    break;
                default:
                    throw new ApplicationException("An unsupported sort order was attempted.");
            }

            return accounts;
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

        private void UnhookRefreshEvents()
        {
            cmbSort.SelectedIndexChanged -= cmbSort_SelectedIndexChanged;
            cmbFilter.SelectedIndexChanged -= cmbFilter_SelectedIndexChanged;
        }

        private void RehookRefreshEvents()
        {
            cmbSort.SelectedIndexChanged += cmbSort_SelectedIndexChanged;
            cmbFilter.SelectedIndexChanged += cmbFilter_SelectedIndexChanged;
        }
        #endregion Helpers
    }
}
