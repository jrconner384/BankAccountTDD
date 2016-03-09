namespace TellerUI
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(MainForm));
            this.lblCustomerName = new System.Windows.Forms.Label();
            this.txtCustomerName = new System.Windows.Forms.TextBox();
            this.lblStartingAmount = new System.Windows.Forms.Label();
            this.txtStartingAmount = new System.Windows.Forms.TextBox();
            this.btnNewAccount = new System.Windows.Forms.Button();
            this.lblSummary = new System.Windows.Forms.Label();
            this.lblCurrencyType = new System.Windows.Forms.Label();
            this.cmbCurrencyType = new System.Windows.Forms.ComboBox();
            this.radChecking = new System.Windows.Forms.RadioButton();
            this.lblAccountType = new System.Windows.Forms.Label();
            this.radSavings = new System.Windows.Forms.RadioButton();
            this.SuspendLayout();
            // 
            // lblCustomerName
            // 
            this.lblCustomerName.AutoSize = true;
            this.lblCustomerName.Location = new System.Drawing.Point(13, 19);
            this.lblCustomerName.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblCustomerName.Name = "lblCustomerName";
            this.lblCustomerName.Size = new System.Drawing.Size(113, 17);
            this.lblCustomerName.TabIndex = 0;
            this.lblCustomerName.Text = "Customer Name:";
            // 
            // txtCustomerName
            // 
            this.txtCustomerName.Location = new System.Drawing.Point(137, 16);
            this.txtCustomerName.Margin = new System.Windows.Forms.Padding(4);
            this.txtCustomerName.Name = "txtCustomerName";
            this.txtCustomerName.Size = new System.Drawing.Size(171, 23);
            this.txtCustomerName.TabIndex = 1;
            // 
            // lblStartingAmount
            // 
            this.lblStartingAmount.AutoSize = true;
            this.lblStartingAmount.Location = new System.Drawing.Point(13, 61);
            this.lblStartingAmount.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.lblStartingAmount.Name = "lblStartingAmount";
            this.lblStartingAmount.Size = new System.Drawing.Size(113, 17);
            this.lblStartingAmount.TabIndex = 2;
            this.lblStartingAmount.Text = "Starting Amount:";
            // 
            // txtStartingAmount
            // 
            this.txtStartingAmount.Location = new System.Drawing.Point(137, 58);
            this.txtStartingAmount.Margin = new System.Windows.Forms.Padding(4);
            this.txtStartingAmount.Name = "txtStartingAmount";
            this.txtStartingAmount.Size = new System.Drawing.Size(171, 23);
            this.txtStartingAmount.TabIndex = 3;
            // 
            // btnNewAccount
            // 
            this.btnNewAccount.Location = new System.Drawing.Point(208, 190);
            this.btnNewAccount.Margin = new System.Windows.Forms.Padding(4);
            this.btnNewAccount.Name = "btnNewAccount";
            this.btnNewAccount.Size = new System.Drawing.Size(100, 28);
            this.btnNewAccount.TabIndex = 4;
            this.btnNewAccount.Text = "New Account";
            this.btnNewAccount.UseVisualStyleBackColor = true;
            this.btnNewAccount.Click += new System.EventHandler(this.btnNewAccount_Click);
            // 
            // lblSummary
            // 
            this.lblSummary.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.lblSummary.AutoSize = true;
            this.lblSummary.Location = new System.Drawing.Point(13, 402);
            this.lblSummary.Name = "lblSummary";
            this.lblSummary.Size = new System.Drawing.Size(81, 17);
            this.lblSummary.TabIndex = 5;
            this.lblSummary.Text = "lblSummary";
            // 
            // lblCurrencyType
            // 
            this.lblCurrencyType.AutoSize = true;
            this.lblCurrencyType.Location = new System.Drawing.Point(13, 100);
            this.lblCurrencyType.Name = "lblCurrencyType";
            this.lblCurrencyType.Size = new System.Drawing.Size(105, 17);
            this.lblCurrencyType.TabIndex = 6;
            this.lblCurrencyType.Text = "Currency Type:";
            // 
            // cmbCurrencyType
            // 
            this.cmbCurrencyType.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.cmbCurrencyType.FormattingEnabled = true;
            this.cmbCurrencyType.Location = new System.Drawing.Point(137, 97);
            this.cmbCurrencyType.Name = "cmbCurrencyType";
            this.cmbCurrencyType.Size = new System.Drawing.Size(171, 24);
            this.cmbCurrencyType.TabIndex = 7;
            // 
            // radChecking
            // 
            this.radChecking.AutoSize = true;
            this.radChecking.Location = new System.Drawing.Point(137, 135);
            this.radChecking.Name = "radChecking";
            this.radChecking.Size = new System.Drawing.Size(84, 21);
            this.radChecking.TabIndex = 8;
            this.radChecking.Text = "Checking";
            this.radChecking.UseVisualStyleBackColor = true;
            // 
            // lblAccountType
            // 
            this.lblAccountType.AutoSize = true;
            this.lblAccountType.Location = new System.Drawing.Point(16, 137);
            this.lblAccountType.Name = "lblAccountType";
            this.lblAccountType.Size = new System.Drawing.Size(99, 17);
            this.lblAccountType.TabIndex = 9;
            this.lblAccountType.Text = "Account Type:";
            // 
            // radSavings
            // 
            this.radSavings.AutoSize = true;
            this.radSavings.Checked = true;
            this.radSavings.Location = new System.Drawing.Point(137, 163);
            this.radSavings.Name = "radSavings";
            this.radSavings.Size = new System.Drawing.Size(76, 21);
            this.radSavings.TabIndex = 10;
            this.radSavings.TabStop = true;
            this.radSavings.Text = "Savings";
            this.radSavings.UseVisualStyleBackColor = true;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 16F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(600, 435);
            this.Controls.Add(this.radSavings);
            this.Controls.Add(this.lblAccountType);
            this.Controls.Add(this.radChecking);
            this.Controls.Add(this.cmbCurrencyType);
            this.Controls.Add(this.lblCurrencyType);
            this.Controls.Add(this.lblSummary);
            this.Controls.Add(this.btnNewAccount);
            this.Controls.Add(this.txtStartingAmount);
            this.Controls.Add(this.lblStartingAmount);
            this.Controls.Add(this.txtCustomerName);
            this.Controls.Add(this.lblCustomerName);
            this.Font = new System.Drawing.Font("Microsoft Sans Serif", 10F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4);
            this.Name = "MainForm";
            this.Text = "Effingham National Bank Teller";
            this.FormClosed += new System.Windows.Forms.FormClosedEventHandler(this.MainForm_FormClosed);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label lblCustomerName;
        private System.Windows.Forms.TextBox txtCustomerName;
        private System.Windows.Forms.Label lblStartingAmount;
        private System.Windows.Forms.TextBox txtStartingAmount;
        private System.Windows.Forms.Button btnNewAccount;
        private System.Windows.Forms.Label lblSummary;
        private System.Windows.Forms.Label lblCurrencyType;
        private System.Windows.Forms.ComboBox cmbCurrencyType;
        private System.Windows.Forms.RadioButton radChecking;
        private System.Windows.Forms.Label lblAccountType;
        private System.Windows.Forms.RadioButton radSavings;
    }
}

