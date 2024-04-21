namespace PortalOpener
{
    partial class frmMain
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
            txtSymbols = new TextBox();
            label1 = new Label();
            btGo = new Button();
            SuspendLayout();
            // 
            // txtSymbols
            // 
            txtSymbols.Location = new Point(12, 47);
            txtSymbols.Multiline = true;
            txtSymbols.Name = "txtSymbols";
            txtSymbols.ScrollBars = ScrollBars.Vertical;
            txtSymbols.Size = new Size(201, 379);
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
            btGo.Location = new Point(138, 442);
            btGo.Name = "btGo";
            btGo.Size = new Size(75, 23);
            btGo.TabIndex = 2;
            btGo.Text = "Go";
            btGo.UseVisualStyleBackColor = true;
            btGo.Click += btGo_Click;
            // 
            // frmMain
            // 
            AutoScaleDimensions = new SizeF(7F, 15F);
            AutoScaleMode = AutoScaleMode.Font;
            ClientSize = new Size(230, 474);
            Controls.Add(btGo);
            Controls.Add(label1);
            Controls.Add(txtSymbols);
            Name = "frmMain";
            Text = "Portal Opener";
            ResumeLayout(false);
            PerformLayout();
        }

        #endregion

        private TextBox txtSymbols;
        private Label label1;
        private Button btGo;
    }
}
