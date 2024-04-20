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
            btGo = new Button();
            txtChatGptBatchResults = new TextBox();
            label1 = new Label();
            label2 = new Label();
            SuspendLayout();
            // 
            // btGo
            // 
            btGo.Location = new Point(714, 410);
            btGo.Name = "btGo";
            btGo.Size = new Size(75, 23);
            btGo.TabIndex = 0;
            btGo.Text = "GO";
            btGo.UseVisualStyleBackColor = true;
            btGo.Click += btGo_Click;
            // 
            // txtChatGptBatchResults
            // 
            txtChatGptBatchResults.Location = new Point(12, 69);
            txtChatGptBatchResults.Multiline = true;
            txtChatGptBatchResults.Name = "txtChatGptBatchResults";
            txtChatGptBatchResults.ScrollBars = ScrollBars.Vertical;
            txtChatGptBatchResults.Size = new Size(776, 324);
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
            // FormStart
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(800, 450);
            Controls.Add(label2);
            Controls.Add(label1);
            Controls.Add(txtChatGptBatchResults);
            Controls.Add(btGo);
            Name = "FormStart";
            Text = "NET_BMS";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private Button btGo;
        private TextBox txtChatGptBatchResults;
        private Label label1;
        private Label label2;
    }
}
