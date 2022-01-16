namespace Prototype
{
    partial class FormContractId
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
            this.txtDocumentation = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtDocumentation
            // 
            this.txtDocumentation.Location = new System.Drawing.Point(12, 12);
            this.txtDocumentation.Multiline = true;
            this.txtDocumentation.Name = "txtDocumentation";
            this.txtDocumentation.Size = new System.Drawing.Size(776, 422);
            this.txtDocumentation.TabIndex = 0;
            // 
            // FormContractId
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 450);
            this.Controls.Add(this.txtDocumentation);
            this.Name = "FormContractId";
            this.Text = "FormContractId";
            this.Load += new System.EventHandler(this.FormContractId_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtDocumentation;
    }
}