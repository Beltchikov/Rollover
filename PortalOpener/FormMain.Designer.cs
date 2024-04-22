namespace PortalOpener
{
    partial class FormMain
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FormMain));
            txtSymbols = new TextBox();
            label1 = new Label();
            btGo = new Button();
            cmbOpener = new ComboBox();
            label2 = new Label();
            SuspendLayout();
            // 
            // txtSymbols
            // 
            txtSymbols.Location = new Point(12, 47);
            txtSymbols.Multiline = true;
            txtSymbols.Name = "txtSymbols";
            txtSymbols.ScrollBars = ScrollBars.Vertical;
            txtSymbols.Size = new Size(201, 234);
            txtSymbols.TabIndex = 0;
            // 
            // label1
            // 
            label1.AutoSize = true;
            label1.Location = new Point(13, 27);
            label1.Name = "label1";
            label1.Size = new Size(52, 15);
            label1.TabIndex = 1;
            label1.Text = "Symbols";
            // 
            // btGo
            // 
            btGo.Location = new Point(513, 258);
            btGo.Name = "btGo";
            btGo.Size = new Size(75, 23);
            btGo.TabIndex = 2;
            btGo.Text = "Go";
            btGo.UseVisualStyleBackColor = true;
            btGo.Click += btGo_Click;
            // 
            // cmbOpener
            // 
            cmbOpener.FormattingEnabled = true;
            cmbOpener.Location = new Point(238, 47);
            cmbOpener.Name = "cmbOpener";
            cmbOpener.Size = new Size(350, 23);
            cmbOpener.TabIndex = 3;
            // 
            // label2
            // 
            label2.AutoSize = true;
            label2.Location = new Point(238, 27);
            label2.Name = "label2";
            label2.Size = new Size(51, 15);
            label2.TabIndex = 4;
            label2.Text = "Openers";
            // 
            // FormMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(600, 292);
            Controls.Add(label2);
            Controls.Add(cmbOpener);
            Controls.Add(btGo);
            Controls.Add(label1);
            Controls.Add(txtSymbols);
            Icon = (Icon)resources.GetObject("$this.Icon");
            Name = "FormMain";
            Text = "Portal Opener";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtSymbols;
        private Label label1;
        private Button btGo;
        private ComboBox cmbOpener;
        private Label label2;
    }
}
