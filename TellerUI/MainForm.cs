using System;
using System.Windows.Forms;
using EffinghamLibrary;

namespace TellerUI
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void btnNewAccount_Click(object sender, EventArgs e)
        {
            IBankAccount newAccount = CreateBankAccountBasedOnTypeSelection(
                txtCustomerName.Text,
                decimal.Parse(txtStartingAmount.Text),
                (CurrencyType)cmbCurrencyType.SelectedIndex);

            MessageBox.Show($"{newAccount.CustomerName} opened an account with a starting balance of {newAccount.Balance:c}");
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            cmbCurrencyType.Items.AddRange(Enum.GetNames(typeof(CurrencyType)));
            cmbCurrencyType.SelectedIndex = 0;
        }

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
    }
}
