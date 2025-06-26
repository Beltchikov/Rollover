namespace OptionHelperUi
{
    partial class Form1
    {
        /// <summary>
        ///  Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        ///  Clean up any resources being used.
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
        ///  Required method for Designer support - do not modify
        ///  the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            label1 = new Label();
            txtInterestRate = new TextBox();
            txtDividendYield = new TextBox();
            label2 = new Label();
            txtVolatility = new TextBox();
            label3 = new Label();
            txtDaysToExpiration = new TextBox();
            label5 = new Label();
            txtStrike = new TextBox();
            label6 = new Label();
            cbIsCall = new CheckBox();
            btCalculatePrice = new Button();
            txtPrice = new TextBox();
            txtUnderlyingPrice = new TextBox();
            label4 = new Label();
            SuspendLayout();
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(38, 32);
            label1.Name = "label1";
            label1.Size = new Size(72, 15);
            label1.TabIndex = 0;
            label1.Text = "Interest Rate";
            label1.Click += label1_Click;
            // 
            // txtInterestRate
            // 
            txtInterestRate.Location = new Point(145, 29);
            txtInterestRate.Name = "txtInterestRate";
            txtInterestRate.Size = new Size(100, 23);
            txtInterestRate.TabIndex = 1;
            // 
            // txtDividendYield
            // 
            txtDividendYield.Location = new Point(145, 58);
            txtDividendYield.Name = "txtDividendYield";
            txtDividendYield.Size = new Size(100, 23);
            txtDividendYield.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(38, 61);
            label2.Name = "label2";
            label2.Size = new Size(83, 15);
            label2.TabIndex = 2;
            label2.Text = "Dividend Yield";
            // 
            // txtVolatility
            // 
            txtVolatility.Location = new Point(145, 87);
            txtVolatility.Name = "txtVolatility";
            txtVolatility.Size = new Size(100, 23);
            txtVolatility.TabIndex = 5;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(38, 90);
            label3.Name = "label3";
            label3.Size = new Size(52, 15);
            label3.TabIndex = 4;
            label3.Text = "Volatility";
            // 
            // txtDaysToExpiration
            // 
            txtDaysToExpiration.Location = new Point(145, 188);
            txtDaysToExpiration.Name = "txtDaysToExpiration";
            txtDaysToExpiration.Size = new Size(100, 23);
            txtDaysToExpiration.TabIndex = 9;
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(38, 191);
            label5.Name = "label5";
            label5.Size = new Size(102, 15);
            label5.TabIndex = 8;
            label5.Text = "Days to Expiration";
            // 
            // txtStrike
            // 
            txtStrike.Location = new Point(145, 159);
            txtStrike.Name = "txtStrike";
            txtStrike.Size = new Size(100, 23);
            txtStrike.TabIndex = 7;
            // 
            // label6
            // 
            label6.AutoSize = true;
            label6.Location = new Point(38, 162);
            label6.Name = "label6";
            label6.Size = new Size(36, 15);
            label6.TabIndex = 6;
            label6.Text = "Strike";
            // 
            // cbIsCall
            // 
            cbIsCall.AutoSize = true;
            cbIsCall.Location = new Point(41, 130);
            cbIsCall.Name = "cbIsCall";
            cbIsCall.Size = new Size(57, 19);
            cbIsCall.TabIndex = 10;
            cbIsCall.Text = "Is Call";
            cbIsCall.UseVisualStyleBackColor = true;
            // 
            // btCalculatePrice
            // 
            btCalculatePrice.Location = new Point(42, 298);
            btCalculatePrice.Name = "btCalculatePrice";
            btCalculatePrice.Size = new Size(203, 23);
            btCalculatePrice.TabIndex = 11;
            btCalculatePrice.Text = "Calculate Price";
            btCalculatePrice.UseVisualStyleBackColor = true;
            btCalculatePrice.Click += btCalculatePrice_Click;
            // 
            // txtPrice
            // 
            txtPrice.BackColor = SystemColors.InactiveCaption;
            txtPrice.Font = new Font("Segoe UI", 14F, FontStyle.Bold);
            txtPrice.Location = new Point(42, 327);
            txtPrice.Name = "txtPrice";
            txtPrice.Size = new Size(203, 32);
            txtPrice.TabIndex = 7;
            // 
            // txtUnderlyingPrice
            // 
            txtUnderlyingPrice.Location = new Point(145, 246);
            txtUnderlyingPrice.Name = "txtUnderlyingPrice";
            txtUnderlyingPrice.Size = new Size(100, 23);
            txtUnderlyingPrice.TabIndex = 13;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(38, 249);
            label4.Name = "label4";
            label4.Size = new Size(94, 15);
            label4.TabIndex = 12;
            label4.Text = "Underlying Price";
            // 
            // Form1
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(287, 399);
            Controls.Add(txtUnderlyingPrice);
            Controls.Add(label4);
            Controls.Add(btCalculatePrice);
            Controls.Add(cbIsCall);
            Controls.Add(txtDaysToExpiration);
            Controls.Add(label5);
            Controls.Add(txtPrice);
            Controls.Add(txtStrike);
            Controls.Add(label6);
            Controls.Add(txtVolatility);
            Controls.Add(label3);
            Controls.Add(txtDividendYield);
            Controls.Add(label2);
            Controls.Add(txtInterestRate);
            Controls.Add(label1);
            Name = "Form1";
            Text = "Option Helper";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label1;
        private TextBox txtInterestRate;
        private TextBox txtDividendYield;
        private Label label2;
        private TextBox txtVolatility;
        private Label label3;
        private TextBox txtDaysToExpiration;
        private Label label5;
        private TextBox txtStrike;
        private Label label6;
        private CheckBox cbIsCall;
        private Button btCalculatePrice;
        private TextBox txtPrice;
        private TextBox txtUnderlyingPrice;
        private Label label4;
    }
}
