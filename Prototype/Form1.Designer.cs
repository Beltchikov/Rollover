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
            this.btStrikes = new System.Windows.Forms.Button();
            this.txtReqId = new System.Windows.Forms.TextBox();
            this.lblReqId = new System.Windows.Forms.Label();
            this.txtSymbolStrike = new System.Windows.Forms.TextBox();
            this.lblSymbolStrike = new System.Windows.Forms.Label();
            this.txtExchange = new System.Windows.Forms.TextBox();
            this.lblExchange = new System.Windows.Forms.Label();
            this.txtSecType = new System.Windows.Forms.TextBox();
            this.lblSecType = new System.Windows.Forms.Label();
            this.txtConId = new System.Windows.Forms.TextBox();
            this.lblConId = new System.Windows.Forms.Label();
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
            this.txtMessage.Location = new System.Drawing.Point(22, 328);
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
            this.btGetConnId.Location = new System.Drawing.Point(144, 96);
            this.btGetConnId.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btGetConnId.Name = "btGetConnId";
            this.btGetConnId.Size = new System.Drawing.Size(276, 24);
            this.btGetConnId.TabIndex = 10;
            this.btGetConnId.Text = "Match Symbol and get ConnId";
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
            // btStrikes
            // 
            this.btStrikes.Location = new System.Drawing.Point(762, 135);
            this.btStrikes.Name = "btStrikes";
            this.btStrikes.Size = new System.Drawing.Size(75, 23);
            this.btStrikes.TabIndex = 12;
            this.btStrikes.Text = "Strikes";
            this.btStrikes.UseVisualStyleBackColor = true;
            this.btStrikes.Click += new System.EventHandler(this.btStrikes_Click);
            // 
            // txtReqId
            // 
            this.txtReqId.Location = new System.Drawing.Point(75, 135);
            this.txtReqId.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtReqId.Name = "txtReqId";
            this.txtReqId.Size = new System.Drawing.Size(54, 23);
            this.txtReqId.TabIndex = 8;
            this.txtReqId.Text = "70100001";
            // 
            // lblReqId
            // 
            this.lblReqId.AutoSize = true;
            this.lblReqId.Location = new System.Drawing.Point(22, 139);
            this.lblReqId.Name = "lblReqId";
            this.lblReqId.Size = new System.Drawing.Size(41, 15);
            this.lblReqId.TabIndex = 9;
            this.lblReqId.Text = "Req ID";
            // 
            // txtSymbolStrike
            // 
            this.txtSymbolStrike.Location = new System.Drawing.Point(229, 135);
            this.txtSymbolStrike.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtSymbolStrike.Name = "txtSymbolStrike";
            this.txtSymbolStrike.Size = new System.Drawing.Size(54, 23);
            this.txtSymbolStrike.TabIndex = 8;
            this.txtSymbolStrike.Text = "MNQ";
            // 
            // lblSymbolStrike
            // 
            this.lblSymbolStrike.AutoSize = true;
            this.lblSymbolStrike.Location = new System.Drawing.Point(144, 138);
            this.lblSymbolStrike.Name = "lblSymbolStrike";
            this.lblSymbolStrike.Size = new System.Drawing.Size(79, 15);
            this.lblSymbolStrike.TabIndex = 9;
            this.lblSymbolStrike.Text = "Symbol Strike";
            // 
            // txtExchange
            // 
            this.txtExchange.Location = new System.Drawing.Point(377, 135);
            this.txtExchange.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtExchange.Name = "txtExchange";
            this.txtExchange.Size = new System.Drawing.Size(54, 23);
            this.txtExchange.TabIndex = 8;
            this.txtExchange.Text = "GLOBEX";
            // 
            // lblExchange
            // 
            this.lblExchange.AutoSize = true;
            this.lblExchange.Location = new System.Drawing.Point(313, 139);
            this.lblExchange.Name = "lblExchange";
            this.lblExchange.Size = new System.Drawing.Size(58, 15);
            this.lblExchange.TabIndex = 9;
            this.lblExchange.Text = "Exchange";
            // 
            // txtSecType
            // 
            this.txtSecType.Location = new System.Drawing.Point(533, 135);
            this.txtSecType.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtSecType.Name = "txtSecType";
            this.txtSecType.Size = new System.Drawing.Size(54, 23);
            this.txtSecType.TabIndex = 8;
            this.txtSecType.Text = "IND";
            // 
            // lblSecType
            // 
            this.lblSecType.AutoSize = true;
            this.lblSecType.Location = new System.Drawing.Point(471, 139);
            this.lblSecType.Name = "lblSecType";
            this.lblSecType.Size = new System.Drawing.Size(54, 15);
            this.lblSecType.TabIndex = 9;
            this.lblSecType.Text = "Sec. type";
            // 
            // txtConId
            // 
            this.txtConId.Location = new System.Drawing.Point(672, 135);
            this.txtConId.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtConId.Name = "txtConId";
            this.txtConId.Size = new System.Drawing.Size(54, 23);
            this.txtConId.TabIndex = 8;
            this.txtConId.Text = "362687422";
            // 
            // lblConId
            // 
            this.lblConId.AutoSize = true;
            this.lblConId.Location = new System.Drawing.Point(619, 139);
            this.lblConId.Name = "lblConId";
            this.lblConId.Size = new System.Drawing.Size(43, 15);
            this.lblConId.TabIndex = 9;
            this.lblConId.Text = "Con ID";
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(924, 524);
            this.Controls.Add(this.btStrikes);
            this.Controls.Add(this.btListPositions);
            this.Controls.Add(this.btGetConnId);
            this.Controls.Add(this.lblReqId);
            this.Controls.Add(this.lblSymbolStrike);
            this.Controls.Add(this.lblExchange);
            this.Controls.Add(this.lblSecType);
            this.Controls.Add(this.lblConId);
            this.Controls.Add(this.lblSymbol);
            this.Controls.Add(this.txtSymbolStrike);
            this.Controls.Add(this.txtReqId);
            this.Controls.Add(this.txtExchange);
            this.Controls.Add(this.txtSecType);
            this.Controls.Add(this.txtConId);
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
        private System.Windows.Forms.Button btStrikes;
        private System.Windows.Forms.TextBox txtReqId;
        private System.Windows.Forms.Label lblReqId;
        private System.Windows.Forms.TextBox txtSymbolStrike;
        private System.Windows.Forms.Label lblSymbolStrike;
        private System.Windows.Forms.TextBox txtExchange;
        private System.Windows.Forms.Label lblExchange;
        private System.Windows.Forms.TextBox txtSecType;
        private System.Windows.Forms.Label lblSecType;
        private System.Windows.Forms.TextBox txtConId;
        private System.Windows.Forms.Label lblConId;
    }
}
