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
            IBankAccount newAccount = new BankAccount(
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
    }
}
