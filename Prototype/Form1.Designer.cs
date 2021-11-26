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
            this.txtSymbolRealTime = new System.Windows.Forms.TextBox();
            this.lblSymbolRealTime = new System.Windows.Forms.Label();
            this.txtCurrencyRealTime = new System.Windows.Forms.TextBox();
            this.lblCurrencyRealTime = new System.Windows.Forms.Label();
            this.txtExchangeRealTime = new System.Windows.Forms.TextBox();
            this.lblExchangeRealTime = new System.Windows.Forms.Label();
            this.txtSecTypeRealTime = new System.Windows.Forms.TextBox();
            this.lblSecTypeRealTime = new System.Windows.Forms.Label();
            this.btReqRealTime = new System.Windows.Forms.Button();
            this.btCancelRealTime = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // txtHost
            // 
            this.txtHost.Location = new System.Drawing.Point(86, 21);
            this.txtHost.Name = "txtHost";
            this.txtHost.Size = new System.Drawing.Size(130, 27);
            this.txtHost.TabIndex = 0;
            this.txtHost.Text = "localhost";
            // 
            // lblHost
            // 
            this.lblHost.AutoSize = true;
            this.lblHost.Location = new System.Drawing.Point(25, 28);
            this.lblHost.Name = "lblHost";
            this.lblHost.Size = new System.Drawing.Size(40, 20);
            this.lblHost.TabIndex = 1;
            this.lblHost.Text = "Host";
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(255, 28);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(35, 20);
            this.lblPort.TabIndex = 3;
            this.lblPort.Text = "Port";
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(302, 21);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(71, 27);
            this.txtPort.TabIndex = 2;
            this.txtPort.Text = "4001";
            // 
            // lblClientId
            // 
            this.lblClientId.AutoSize = true;
            this.lblClientId.Location = new System.Drawing.Point(421, 28);
            this.lblClientId.Name = "lblClientId";
            this.lblClientId.Size = new System.Drawing.Size(66, 20);
            this.lblClientId.TabIndex = 5;
            this.lblClientId.Text = "Client ID";
            // 
            // txtClientId
            // 
            this.txtClientId.Location = new System.Drawing.Point(495, 21);
            this.txtClientId.Name = "txtClientId";
            this.txtClientId.Size = new System.Drawing.Size(73, 27);
            this.txtClientId.TabIndex = 4;
            this.txtClientId.Text = "1";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(609, 20);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(94, 29);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(25, 437);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMessage.Size = new System.Drawing.Size(1012, 225);
            this.txtMessage.TabIndex = 7;
            // 
            // lblSymbol
            // 
            this.lblSymbol.AutoSize = true;
            this.lblSymbol.Location = new System.Drawing.Point(25, 133);
            this.lblSymbol.Name = "lblSymbol";
            this.lblSymbol.Size = new System.Drawing.Size(59, 20);
            this.lblSymbol.TabIndex = 9;
            this.lblSymbol.Text = "Symbol";
            // 
            // txtSymbol
            // 
            this.txtSymbol.Location = new System.Drawing.Point(86, 128);
            this.txtSymbol.Name = "txtSymbol";
            this.txtSymbol.Size = new System.Drawing.Size(61, 27);
            this.txtSymbol.TabIndex = 8;
            this.txtSymbol.Text = "MNQ";
            // 
            // btGetConnId
            // 
            this.btGetConnId.Location = new System.Drawing.Point(165, 128);
            this.btGetConnId.Name = "btGetConnId";
            this.btGetConnId.Size = new System.Drawing.Size(315, 32);
            this.btGetConnId.TabIndex = 10;
            this.btGetConnId.Text = "Match Symbol and get ConnId";
            this.btGetConnId.UseVisualStyleBackColor = true;
            this.btGetConnId.Click += new System.EventHandler(this.btCheckSymbol_Click);
            // 
            // btListPositions
            // 
            this.btListPositions.Location = new System.Drawing.Point(86, 72);
            this.btListPositions.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btListPositions.Name = "btListPositions";
            this.btListPositions.Size = new System.Drawing.Size(411, 33);
            this.btListPositions.TabIndex = 11;
            this.btListPositions.Text = "List positions";
            this.btListPositions.UseVisualStyleBackColor = true;
            this.btListPositions.Click += new System.EventHandler(this.btListPositions_Click);
            // 
            // btStrikes
            // 
            this.btStrikes.Location = new System.Drawing.Point(871, 180);
            this.btStrikes.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btStrikes.Name = "btStrikes";
            this.btStrikes.Size = new System.Drawing.Size(86, 31);
            this.btStrikes.TabIndex = 12;
            this.btStrikes.Text = "Strikes";
            this.btStrikes.UseVisualStyleBackColor = true;
            this.btStrikes.Click += new System.EventHandler(this.btStrikes_Click);
            // 
            // txtReqId
            // 
            this.txtReqId.Location = new System.Drawing.Point(86, 180);
            this.txtReqId.Name = "txtReqId";
            this.txtReqId.Size = new System.Drawing.Size(61, 27);
            this.txtReqId.TabIndex = 8;
            this.txtReqId.Text = "70100001";
            // 
            // lblReqId
            // 
            this.lblReqId.AutoSize = true;
            this.lblReqId.Location = new System.Drawing.Point(25, 185);
            this.lblReqId.Name = "lblReqId";
            this.lblReqId.Size = new System.Drawing.Size(54, 20);
            this.lblReqId.TabIndex = 9;
            this.lblReqId.Text = "Req ID";
            // 
            // txtSymbolStrike
            // 
            this.txtSymbolStrike.Location = new System.Drawing.Point(262, 180);
            this.txtSymbolStrike.Name = "txtSymbolStrike";
            this.txtSymbolStrike.Size = new System.Drawing.Size(61, 27);
            this.txtSymbolStrike.TabIndex = 8;
            this.txtSymbolStrike.Text = "MNQ";
            // 
            // lblSymbolStrike
            // 
            this.lblSymbolStrike.AutoSize = true;
            this.lblSymbolStrike.Location = new System.Drawing.Point(165, 184);
            this.lblSymbolStrike.Name = "lblSymbolStrike";
            this.lblSymbolStrike.Size = new System.Drawing.Size(100, 20);
            this.lblSymbolStrike.TabIndex = 9;
            this.lblSymbolStrike.Text = "Symbol Strike";
            // 
            // txtExchange
            // 
            this.txtExchange.Location = new System.Drawing.Point(431, 180);
            this.txtExchange.Name = "txtExchange";
            this.txtExchange.Size = new System.Drawing.Size(61, 27);
            this.txtExchange.TabIndex = 8;
            this.txtExchange.Text = "GLOBEX";
            // 
            // lblExchange
            // 
            this.lblExchange.AutoSize = true;
            this.lblExchange.Location = new System.Drawing.Point(358, 185);
            this.lblExchange.Name = "lblExchange";
            this.lblExchange.Size = new System.Drawing.Size(72, 20);
            this.lblExchange.TabIndex = 9;
            this.lblExchange.Text = "Exchange";
            // 
            // txtSecType
            // 
            this.txtSecType.Location = new System.Drawing.Point(609, 180);
            this.txtSecType.Name = "txtSecType";
            this.txtSecType.Size = new System.Drawing.Size(61, 27);
            this.txtSecType.TabIndex = 8;
            this.txtSecType.Text = "IND";
            // 
            // lblSecType
            // 
            this.lblSecType.AutoSize = true;
            this.lblSecType.Location = new System.Drawing.Point(538, 185);
            this.lblSecType.Name = "lblSecType";
            this.lblSecType.Size = new System.Drawing.Size(68, 20);
            this.lblSecType.TabIndex = 9;
            this.lblSecType.Text = "Sec. type";
            // 
            // txtConId
            // 
            this.txtConId.Location = new System.Drawing.Point(768, 180);
            this.txtConId.Name = "txtConId";
            this.txtConId.Size = new System.Drawing.Size(61, 27);
            this.txtConId.TabIndex = 8;
            this.txtConId.Text = "362687422";
            // 
            // lblConId
            // 
            this.lblConId.AutoSize = true;
            this.lblConId.Location = new System.Drawing.Point(707, 185);
            this.lblConId.Name = "lblConId";
            this.lblConId.Size = new System.Drawing.Size(54, 20);
            this.lblConId.TabIndex = 9;
            this.lblConId.Text = "Con ID";
            // 
            // txtSymbolRealTime
            // 
            this.txtSymbolRealTime.Location = new System.Drawing.Point(86, 231);
            this.txtSymbolRealTime.Name = "txtSymbolRealTime";
            this.txtSymbolRealTime.Size = new System.Drawing.Size(61, 27);
            this.txtSymbolRealTime.TabIndex = 8;
            this.txtSymbolRealTime.Text = "MNQ";
            // 
            // lblSymbolRealTime
            // 
            this.lblSymbolRealTime.AutoSize = true;
            this.lblSymbolRealTime.Location = new System.Drawing.Point(25, 236);
            this.lblSymbolRealTime.Name = "lblSymbolRealTime";
            this.lblSymbolRealTime.Size = new System.Drawing.Size(59, 20);
            this.lblSymbolRealTime.TabIndex = 9;
            this.lblSymbolRealTime.Text = "Symbol";
            // 
            // txtCurrencyRealTime
            // 
            this.txtCurrencyRealTime.Location = new System.Drawing.Point(242, 231);
            this.txtCurrencyRealTime.Name = "txtCurrencyRealTime";
            this.txtCurrencyRealTime.Size = new System.Drawing.Size(61, 27);
            this.txtCurrencyRealTime.TabIndex = 8;
            this.txtCurrencyRealTime.Text = "USD";
            // 
            // lblCurrencyRealTime
            // 
            this.lblCurrencyRealTime.AutoSize = true;
            this.lblCurrencyRealTime.Location = new System.Drawing.Point(167, 236);
            this.lblCurrencyRealTime.Name = "lblCurrencyRealTime";
            this.lblCurrencyRealTime.Size = new System.Drawing.Size(66, 20);
            this.lblCurrencyRealTime.TabIndex = 9;
            this.lblCurrencyRealTime.Text = "Currency";
            // 
            // txtExchangeRealTime
            // 
            this.txtExchangeRealTime.Location = new System.Drawing.Point(408, 231);
            this.txtExchangeRealTime.Name = "txtExchangeRealTime";
            this.txtExchangeRealTime.Size = new System.Drawing.Size(79, 27);
            this.txtExchangeRealTime.TabIndex = 8;
            this.txtExchangeRealTime.Text = "SMART";
            // 
            // lblExchangeRealTime
            // 
            this.lblExchangeRealTime.AutoSize = true;
            this.lblExchangeRealTime.Location = new System.Drawing.Point(335, 236);
            this.lblExchangeRealTime.Name = "lblExchangeRealTime";
            this.lblExchangeRealTime.Size = new System.Drawing.Size(72, 20);
            this.lblExchangeRealTime.TabIndex = 9;
            this.lblExchangeRealTime.Text = "Exchange";
            // 
            // txtSecTypeRealTime
            // 
            this.txtSecTypeRealTime.Location = new System.Drawing.Point(579, 232);
            this.txtSecTypeRealTime.Name = "txtSecTypeRealTime";
            this.txtSecTypeRealTime.Size = new System.Drawing.Size(61, 27);
            this.txtSecTypeRealTime.TabIndex = 8;
            this.txtSecTypeRealTime.Text = "IND";
            // 
            // lblSecTypeRealTime
            // 
            this.lblSecTypeRealTime.AutoSize = true;
            this.lblSecTypeRealTime.Location = new System.Drawing.Point(509, 237);
            this.lblSecTypeRealTime.Name = "lblSecTypeRealTime";
            this.lblSecTypeRealTime.Size = new System.Drawing.Size(68, 20);
            this.lblSecTypeRealTime.TabIndex = 9;
            this.lblSecTypeRealTime.Text = "Sec. type";
            // 
            // btReqRealTime
            // 
            this.btReqRealTime.Location = new System.Drawing.Point(690, 232);
            this.btReqRealTime.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btReqRealTime.Name = "btReqRealTime";
            this.btReqRealTime.Size = new System.Drawing.Size(117, 31);
            this.btReqRealTime.TabIndex = 12;
            this.btReqRealTime.Text = "Req. Real Time";
            this.btReqRealTime.UseVisualStyleBackColor = true;
            this.btReqRealTime.Click += new System.EventHandler(this.btReqRealTime_Click);
            // 
            // btCancelRealTime
            // 
            this.btCancelRealTime.Location = new System.Drawing.Point(827, 232);
            this.btCancelRealTime.Margin = new System.Windows.Forms.Padding(3, 4, 3, 4);
            this.btCancelRealTime.Name = "btCancelRealTime";
            this.btCancelRealTime.Size = new System.Drawing.Size(129, 31);
            this.btCancelRealTime.TabIndex = 13;
            this.btCancelRealTime.Text = "Cancel Real Time";
            this.btCancelRealTime.UseVisualStyleBackColor = true;
            this.btCancelRealTime.Click += new System.EventHandler(this.btCancelRealTime_Click);
            // 
            // Form1
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(8F, 20F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1056, 699);
            this.Controls.Add(this.btCancelRealTime);
            this.Controls.Add(this.btReqRealTime);
            this.Controls.Add(this.btStrikes);
            this.Controls.Add(this.btListPositions);
            this.Controls.Add(this.btGetConnId);
            this.Controls.Add(this.lblReqId);
            this.Controls.Add(this.lblSymbolStrike);
            this.Controls.Add(this.lblExchangeRealTime);
            this.Controls.Add(this.lblExchange);
            this.Controls.Add(this.lblSecTypeRealTime);
            this.Controls.Add(this.lblSecType);
            this.Controls.Add(this.lblConId);
            this.Controls.Add(this.lblCurrencyRealTime);
            this.Controls.Add(this.lblSymbolRealTime);
            this.Controls.Add(this.lblSymbol);
            this.Controls.Add(this.txtSymbolStrike);
            this.Controls.Add(this.txtExchangeRealTime);
            this.Controls.Add(this.txtReqId);
            this.Controls.Add(this.txtExchange);
            this.Controls.Add(this.txtSecTypeRealTime);
            this.Controls.Add(this.txtSecType);
            this.Controls.Add(this.txtCurrencyRealTime);
            this.Controls.Add(this.txtSymbolRealTime);
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
        private System.Windows.Forms.TextBox txtSymbolRealTime;
        private System.Windows.Forms.Label lblSymbolRealTime;
        private System.Windows.Forms.TextBox txtCurrencyRealTime;
        private System.Windows.Forms.Label lblCurrencyRealTime;
        private System.Windows.Forms.TextBox txtExchangeRealTime;
        private System.Windows.Forms.Label lblExchangeRealTime;
        private System.Windows.Forms.TextBox txtSecTypeRealTime;
        private System.Windows.Forms.Label lblSecTypeRealTime;
        private System.Windows.Forms.Button btReqRealTime;
        private System.Windows.Forms.Button btCancelRealTime;
    }
}
