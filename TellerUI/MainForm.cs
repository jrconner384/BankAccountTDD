﻿using System;
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

        public MainForm()
        {
            InitializeComponent();
            vault = SoapVault.Instance;
        }

        #region Events
        private void btnNewAccount_Click(object sender, EventArgs e)
        {
            IBankAccountMultipleCurrency newAccount = CreateBankAccountBasedOnTypeSelection(
                txtCustomerName.Text,
                decimal.Parse(txtStartingAmount.Text),
                (CurrencyType)cmbCurrencyType.SelectedIndex);

            vault.AddAccount(newAccount);
            SummarizeAccounts();
            //MessageBox.Show($"{newAccount.CustomerName} opened an account with a starting balance of {newAccount.Balance:c}");
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            cmbCurrencyType.Items.AddRange(Enum.GetNames(typeof(CurrencyType)));
            cmbCurrencyType.SelectedIndex = 0;
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

        #endregion Helpers
    }
}
