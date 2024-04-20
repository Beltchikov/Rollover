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
            txtNetBmsResults = new TextBox();
            label3 = new Label();
            txtErrors = new TextBox();
            label4 = new Label();
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
            // txtNetBmsResults
            // 
            txtNetBmsResults.BackColor = Color.LightCyan;
            txtNetBmsResults.Location = new Point(543, 69);
            txtNetBmsResults.Multiline = true;
            txtNetBmsResults.Name = "txtNetBmsResults";
            txtNetBmsResults.ScrollBars = ScrollBars.Vertical;
            txtNetBmsResults.Size = new Size(444, 324);
            txtNetBmsResults.TabIndex = 4;
            // 
            // label3
            // 
            label3.AutoSize = true;
            label3.Location = new Point(543, 51);
            label3.Name = "label3";
            label3.Size = new Size(97, 15);
            label3.TabIndex = 5;
            label3.Text = "NET_BMS Results";
            // 
            // txtErrors
            // 
            txtErrors.BackColor = Color.MistyRose;
            txtErrors.Location = new Point(543, 442);
            txtErrors.Multiline = true;
            txtErrors.Name = "txtErrors";
            txtErrors.ScrollBars = ScrollBars.Vertical;
            txtErrors.Size = new Size(444, 105);
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
            // FormStart
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(1001, 559);
            Controls.Add(label4);
            Controls.Add(txtErrors);
            Controls.Add(label3);
            Controls.Add(txtNetBmsResults);
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
        private TextBox txtNetBmsResults;
        private Label label3;
        private TextBox txtErrors;
        private Label label4;
    }
}
