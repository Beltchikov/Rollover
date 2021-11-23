namespace Prototype
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
            this.txtHost = new System.Windows.Forms.TextBox();
            this.lblHost = new System.Windows.Forms.Label();
            this.lblPort = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.lblClientId = new System.Windows.Forms.Label();
            this.txtClientId = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.lblSymbol = new System.Windows.Forms.Label();
            this.txtSymbol = new System.Windows.Forms.TextBox();
            this.btGetConnId = new System.Windows.Forms.Button();
            this.btListPositions = new System.Windows.Forms.Button();
            this.lblSecType = new System.Windows.Forms.Label();
            this.tbSecType = new System.Windows.Forms.TextBox();
            this.lblCurrency = new System.Windows.Forms.Label();
            this.txtCurrency = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.tbExchange = new System.Windows.Forms.TextBox();
            this.SuspendLayout();
            // 
            // txtHost
            // 
            this.txtHost.Location = new System.Drawing.Point(75, 16);
            this.txtHost.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtHost.Name = "txtHost";
            this.txtHost.Size = new System.Drawing.Size(114, 23);
            this.txtHost.TabIndex = 0;
            this.txtHost.Text = "localhost";
            // 
            // lblHost
            // 
            this.lblHost.AutoSize = true;
            this.lblHost.Location = new System.Drawing.Point(22, 21);
            this.lblHost.Name = "lblHost";
            this.lblHost.Size = new System.Drawing.Size(32, 15);
            this.lblHost.TabIndex = 1;
            this.lblHost.Text = "Host";
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(223, 21);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(29, 15);
            this.lblPort.TabIndex = 3;
            this.lblPort.Text = "Port";
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(264, 16);
            this.txtPort.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(63, 23);
            this.txtPort.TabIndex = 2;
            this.txtPort.Text = "4001";
            // 
            // lblClientId
            // 
            this.lblClientId.AutoSize = true;
            this.lblClientId.Location = new System.Drawing.Point(368, 21);
            this.lblClientId.Name = "lblClientId";
            this.lblClientId.Size = new System.Drawing.Size(52, 15);
            this.lblClientId.TabIndex = 5;
            this.lblClientId.Text = "Client ID";
            // 
            // txtClientId
            // 
            this.txtClientId.Location = new System.Drawing.Point(433, 16);
            this.txtClientId.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtClientId.Name = "txtClientId";
            this.txtClientId.Size = new System.Drawing.Size(64, 23);
            this.txtClientId.TabIndex = 4;
            this.txtClientId.Text = "1";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(533, 15);
            this.btnConnect.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(82, 22);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(22, 140);
            this.txtMessage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMessage.Size = new System.Drawing.Size(886, 170);
            this.txtMessage.TabIndex = 7;
            // 
            // lblSymbol
            // 
            this.lblSymbol.AutoSize = true;
            this.lblSymbol.Location = new System.Drawing.Point(22, 100);
            this.lblSymbol.Name = "lblSymbol";
            this.lblSymbol.Size = new System.Drawing.Size(47, 15);
            this.lblSymbol.TabIndex = 9;
            this.lblSymbol.Text = "Symbol";
            // 
            // txtSymbol
            // 
            this.txtSymbol.Location = new System.Drawing.Point(75, 96);
            this.txtSymbol.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtSymbol.Name = "txtSymbol";
            this.txtSymbol.Size = new System.Drawing.Size(54, 23);
            this.txtSymbol.TabIndex = 8;
            this.txtSymbol.Text = "MNQ";
            // 
            // btGetConnId
            // 
            this.btGetConnId.Location = new System.Drawing.Point(707, 96);
            this.btGetConnId.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btGetConnId.Name = "btGetConnId";
            this.btGetConnId.Size = new System.Drawing.Size(93, 22);
            this.btGetConnId.TabIndex = 10;
            this.btGetConnId.Text = "Get ConnId";
            this.btGetConnId.UseVisualStyleBackColor = true;
            this.btGetConnId.Click += new System.EventHandler(this.btCheckSymbol_Click);
            // 
            // btListPositions
            // 
            this.btListPositions.Location = new System.Drawing.Point(75, 54);
            this.btListPositions.Name = "btListPositions";
            this.btListPositions.Size = new System.Drawing.Size(360, 25);
            this.btListPositions.TabIndex = 11;
            this.btListPositions.Text = "List positions";
            this.btListPositions.UseVisualStyleBackColor = true;
            this.btListPositions.Click += new System.EventHandler(this.btListPositions_Click);
            // 
            // lblSecType
            // 
            this.lblSecType.AutoSize = true;
            this.lblSecType.Location = new System.Drawing.Point(165, 100);
            this.lblSecType.Name = "lblSecType";
            this.lblSecType.Size = new System.Drawing.Size(52, 15);
            this.lblSecType.TabIndex = 12;
            this.lblSecType.Text = "Sec Type";
            this.lblSecType.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // tbSecType
            // 
            this.tbSecType.Location = new System.Drawing.Point(223, 96);
            this.tbSecType.Name = "tbSecType";
            this.tbSecType.Size = new System.Drawing.Size(60, 23);
            this.tbSecType.TabIndex = 13;
            this.tbSecType.Text = "IND";
            // 
            // lblCurrency
            // 
            this.lblCurrency.AutoSize = true;
            this.lblCurrency.Location = new System.Drawing.Point(326, 100);
            this.lblCurrency.Name = "lblCurrency";
            this.lblCurrency.Size = new System.Drawing.Size(55, 15);
            this.lblCurrency.TabIndex = 14;
            this.lblCurrency.Text = "Currency";
            this.lblCurrency.Click += new System.EventHandler(this.lblCurrency_Click);
            // 
            // txtCurrency
            // 
            this.txtCurrency.Location = new System.Drawing.Point(397, 96);
            this.txtCurrency.Name = "txtCurrency";
            this.txtCurrency.Size = new System.Drawing.Size(63, 23);
            this.txtCurrency.TabIndex = 15;
            this.txtCurrency.Text = "USD";
            this.txtCurrency.TextChanged += new System.EventHandler(this.txtCurrency_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(505, 100);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(58, 15);
            this.label1.TabIndex = 16;
            this.label1.Text = "Exchange";
            // 
            // tbExchange
            // 
            this.tbExchange.Location = new System.Drawing.Point(569, 96);
            this.tbExchange.Name = "tbExchange";
            this.tbExchange.Size = new System.Drawing.Size(100, 23);
            this.tbExchange.TabIndex = 17;
            this.tbExchange.Text = "SMART";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(924, 321);
            this.Controls.Add(this.tbExchange);
            this.Controls.Add(this.label1);
            this.Controls.Add(this.txtCurrency);
            this.Controls.Add(this.lblCurrency);
            this.Controls.Add(this.tbSecType);
            this.Controls.Add(this.lblSecType);
            this.Controls.Add(this.btListPositions);
            this.Controls.Add(this.btGetConnId);
            this.Controls.Add(this.lblSymbol);
            this.Controls.Add(this.txtSymbol);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.lblClientId);
            this.Controls.Add(this.txtClientId);
            this.Controls.Add(this.lblPort);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.lblHost);
            this.Controls.Add(this.txtHost);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "Form1";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.TextBox txtHost;
        private System.Windows.Forms.Label lblHost;
        private System.Windows.Forms.Label lblPort;
        private System.Windows.Forms.TextBox txtPort;
        private System.Windows.Forms.Label lblClientId;
        private System.Windows.Forms.TextBox txtClientId;
        private System.Windows.Forms.Button btnConnect;
        private System.Windows.Forms.TextBox txtMessage;
        private System.Windows.Forms.Label lblSymbol;
        private System.Windows.Forms.TextBox txtSymbol;
        private System.Windows.Forms.Button btGetConnId;
        private System.Windows.Forms.Button btListPositions;
        private System.Windows.Forms.Label lblSecType;
        private System.Windows.Forms.TextBox tbSecType;
        private System.Windows.Forms.Label lblCurrency;
        private System.Windows.Forms.TextBox txtCurrency;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.TextBox tbExchange;
    }
}
