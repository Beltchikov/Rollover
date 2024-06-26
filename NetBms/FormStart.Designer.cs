﻿namespace NetBms
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormStart));
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
            label6 = new Label();
            label7 = new Label();
            txtSymbolErrors = new TextBox();
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
            label1.ForeColor = Color.White;
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
            label2.ForeColor = Color.White;
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
            label3.ForeColor = Color.White;
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
            txtErrors.Size = new Size(207, 105);
            txtErrors.TabIndex = 6;
            // 
            // label4
            // 
            label4.AutoSize = true;
            label4.ForeColor = Color.White;
            label4.Location = new Point(543, 424);
            label4.Name = "label4";
            label4.Size = new Size(130, 15);
            label4.TabIndex = 7;
            label4.Text = "Erroneous JSON Strings";
            // 
            // label5
            // 
            label5.AutoSize = true;
            label5.ForeColor = Color.White;
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
            // label6
            // 
            label6.AutoSize = true;
            label6.ForeColor = Color.LightSalmon;
            label6.Location = new Point(192, 51);
            label6.Name = "label6";
            label6.Size = new Size(157, 15);
            label6.TabIndex = 10;
            label6.Text = "Use $ to separte the batches!";
            // 
            // label7
            // 
            label7.AutoSize = true;
            label7.ForeColor = Color.White;
            label7.Location = new Point(780, 424);
            label7.Name = "label7";
            label7.Size = new Size(108, 15);
            label7.TabIndex = 12;
            label7.Text = "Erroneous Symbols";
            // 
            // txtSymbolErrors
            // 
            txtSymbolErrors.BackColor = Color.Firebrick;
            txtSymbolErrors.ForeColor = Color.White;
            txtSymbolErrors.Location = new Point(780, 442);
            txtSymbolErrors.Multiline = true;
            txtSymbolErrors.Name = "txtSymbolErrors";
            txtSymbolErrors.ScrollBars = ScrollBars.Vertical;
            txtSymbolErrors.Size = new Size(207, 105);
            txtSymbolErrors.TabIndex = 11;
            // 
            // FormStart
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            BackColor = SystemColors.ControlDarkDark;
            ClientSize = new Size(1001, 559);
            Controls.Add(label7);
            Controls.Add(txtSymbolErrors);
            Controls.Add(label6);
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
            Icon = (Icon)resources.GetObject("$this.Icon");
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
        private Label label6;
        private Label label7;
        private TextBox txtSymbolErrors;
    }
}
