namespace NetBms
{
    partial class FormResults
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
            label2 = new Label();
            label3 = new Label();
            txtSell = new TextBox();
            label1 = new Label();
            txtBuy = new TextBox();
            SuspendLayout();
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            label2.Location = new Point(12, 9);
            label2.Name = "label2";
            label2.Size = new Size(86, 30);
            label2.TabIndex = 4;
            label2.Text = "Results";
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(439, 54);
            label3.Name = "label3";
            label3.Size = new Size(31, 15);
            label3.TabIndex = 9;
            label3.Text = "SELL";
            // 
            // txtSell
            // 
            txtSell.Location = new Point(439, 72);
            txtSell.Multiline = true;
            txtSell.Name = "txtSell";
            txtSell.ScrollBars = ScrollBars.Vertical;
            txtSell.Size = new Size(343, 324);
            txtSell.TabIndex = 8;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(18, 54);
            label1.Name = "label1";
            label1.Size = new Size(29, 15);
            label1.TabIndex = 7;
            label1.Text = "BUY";
            // 
            // txtBuy
            // 
            txtBuy.Location = new Point(18, 72);
            txtBuy.Multiline = true;
            txtBuy.Name = "txtBuy";
            txtBuy.ScrollBars = ScrollBars.Vertical;
            txtBuy.Size = new Size(343, 324);
            txtBuy.TabIndex = 6;
            // 
            // FormResults
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ActiveCaption;
            ClientSize = new Size(800, 450);
            Controls.Add(label3);
            Controls.Add(txtSell);
            Controls.Add(label1);
            Controls.Add(txtBuy);
            Controls.Add(label2);
            Name = "FormResults";
            Text = "FormResults";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Label label2;
        private Label label3;
        private TextBox txtSell;
        private Label label1;
        private TextBox txtBuy;
    }
}