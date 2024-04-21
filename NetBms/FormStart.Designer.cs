namespace NetBms
{
    partial class FormStart
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
            btProcess = new Button();
            txtChatGptBatchResults = new TextBox();
            label1 = new Label();
            label2 = new Label();
            txtBuyResults = new TextBox();
            label3 = new Label();
            txtErrors = new TextBox();
            label4 = new Label();
            label5 = new Label();
            txtSellResults = new TextBox();
            SuspendLayout();
            // 
            // btProcess
            // 
            btProcess.Location = new Point(462, 202);
            btProcess.Name = "btProcess";
            btProcess.Size = new Size(75, 23);
            btProcess.TabIndex = 0;
            btProcess.Text = "Process >>";
            btProcess.UseVisualStyleBackColor = true;
            btProcess.Click += btGo_ClickAsync;
            // 
            // txtChatGptBatchResults
            // 
            txtChatGptBatchResults.Location = new Point(12, 69);
            txtChatGptBatchResults.Multiline = true;
            txtChatGptBatchResults.Name = "txtChatGptBatchResults";
            txtChatGptBatchResults.ScrollBars = ScrollBars.Vertical;
            txtChatGptBatchResults.Size = new Size(444, 478);
            txtChatGptBatchResults.TabIndex = 1;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(12, 51);
            label1.Name = "label1";
            label1.Size = new Size(174, 15);
            label1.TabIndex = 2;
            label1.Text = "Chat GPT Batch Results as JSON";
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Font = new Font("Segoe UI", 16F, FontStyle.Bold);
            label2.Location = new Point(12, 9);
            label2.Name = "label2";
            label2.Size = new Size(111, 30);
            label2.TabIndex = 3;
            label2.Text = "NET_BMS";
            // 
            // txtBuyResults
            // 
            txtBuyResults.BackColor = Color.LightCyan;
            txtBuyResults.Location = new Point(543, 69);
            txtBuyResults.Multiline = true;
            txtBuyResults.Name = "txtBuyResults";
            txtBuyResults.ScrollBars = ScrollBars.Vertical;
            txtBuyResults.Size = new Size(207, 324);
            txtBuyResults.TabIndex = 4;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(543, 51);
            label3.Name = "label3";
            label3.Size = new Size(95, 15);
            label3.TabIndex = 5;
            label3.Text = "NET_BUY Results";
            // 
            // txtErrors
            // 
            txtErrors.BackColor = Color.Firebrick;
            txtErrors.ForeColor = Color.White;
            txtErrors.Location = new Point(543, 442);
            txtErrors.Multiline = true;
            txtErrors.Name = "txtErrors";
            txtErrors.ScrollBars = ScrollBars.Vertical;
            txtErrors.Size = new Size(446, 105);
            txtErrors.TabIndex = 6;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.Location = new Point(543, 424);
            label4.Name = "label4";
            label4.Size = new Size(99, 15);
            label4.TabIndex = 7;
            label4.Text = "Erroneous Strings";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.Location = new Point(780, 51);
            label5.Name = "label5";
            label5.Size = new Size(97, 15);
            label5.TabIndex = 9;
            label5.Text = "NET_SELL Results";
            // 
            // txtSellResults
            // 
            txtSellResults.BackColor = Color.MistyRose;
            txtSellResults.Location = new Point(780, 69);
            txtSellResults.Multiline = true;
            txtSellResults.Name = "txtSellResults";
            txtSellResults.ScrollBars = ScrollBars.Vertical;
            txtSellResults.Size = new Size(207, 324);
            txtSellResults.TabIndex = 8;
            // 
            // FormStart
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1001, 559);
            Controls.Add(label5);
            Controls.Add(txtSellResults);
            Controls.Add(label4);
            Controls.Add(txtErrors);
            Controls.Add(label3);
            Controls.Add(txtBuyResults);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(txtChatGptBatchResults);
            Controls.Add(btProcess);
            Name = "FormStart";
            Text = "NET_BMS";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btProcess;
        private TextBox txtChatGptBatchResults;
        private Label label1;
        private Label label2;
        private TextBox txtBuyResults;
        private Label label3;
        private TextBox txtErrors;
        private Label label4;
        private Label label5;
        private TextBox txtSellResults;
    }
}
