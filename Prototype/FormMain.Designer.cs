﻿namespace Prototype
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
            this.txtHost = new System.Windows.Forms.TextBox();
            this.lblHost = new System.Windows.Forms.Label();
            this.lblPort = new System.Windows.Forms.Label();
            this.txtPort = new System.Windows.Forms.TextBox();
            this.lblClientId = new System.Windows.Forms.Label();
            this.txtClientId = new System.Windows.Forms.TextBox();
            this.btnConnect = new System.Windows.Forms.Button();
            this.txtMessage = new System.Windows.Forms.TextBox();
            this.lblSymbol = new System.Windows.Forms.Label();
            this.txtSymbolCheckSymbol = new System.Windows.Forms.TextBox();
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
            this.txtLocalSymbolRealTime = new System.Windows.Forms.TextBox();
            this.lblLocalSymbol = new System.Windows.Forms.Label();
            this.lblLocalSymbolSample = new System.Windows.Forms.Label();
            this.lblFopSample = new System.Windows.Forms.Label();
            this.txtGenericTickList = new System.Windows.Forms.TextBox();
            this.lblGenericTicksList = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            this.txtSecTypeCheckSymbol = new System.Windows.Forms.TextBox();
            this.txtExchangeCheckSymbol = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btPlaceOrder = new System.Windows.Forms.Button();
            this.lblSymbolOrder = new System.Windows.Forms.Label();
            this.txtSymbolOrder = new System.Windows.Forms.TextBox();
            this.txtExchangeOrder = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtOrderLimitPrice = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtQuantityOrder = new System.Windows.Forms.TextBox();
            this.txtOrderQuantity = new System.Windows.Forms.Label();
            this.txtOrderAction = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.txtConIdOrder = new System.Windows.Forms.TextBox();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.btDocumentationCheckSymbol = new System.Windows.Forms.Button();
            this.txtCurrencyCheckSymbol = new System.Windows.Forms.TextBox();
            this.label16 = new System.Windows.Forms.Label();
            this.txtLastTradeCheckSymbol = new System.Windows.Forms.TextBox();
            this.label13 = new System.Windows.Forms.Label();
            this.txtStrikeCheckSymbol = new System.Windows.Forms.TextBox();
            this.label14 = new System.Windows.Forms.Label();
            this.txtRightCheckSymbol = new System.Windows.Forms.TextBox();
            this.label15 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtHost
            // 
            this.txtHost.Location = new System.Drawing.Point(46, 6);
            this.txtHost.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtHost.Name = "txtHost";
            this.txtHost.Size = new System.Drawing.Size(64, 23);
            this.txtHost.TabIndex = 0;
            this.txtHost.Text = "localhost";
            // 
            // lblHost
            // 
            this.lblHost.AutoSize = true;
            this.lblHost.Location = new System.Drawing.Point(8, 14);
            this.lblHost.Name = "lblHost";
            this.lblHost.Size = new System.Drawing.Size(32, 15);
            this.lblHost.TabIndex = 1;
            this.lblHost.Text = "Host";
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(124, 14);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(29, 15);
            this.lblPort.TabIndex = 3;
            this.lblPort.Text = "Port";
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(163, 6);
            this.txtPort.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(54, 23);
            this.txtPort.TabIndex = 2;
            this.txtPort.Text = "4001";
            // 
            // lblClientId
            // 
            this.lblClientId.AutoSize = true;
            this.lblClientId.Location = new System.Drawing.Point(234, 14);
            this.lblClientId.Name = "lblClientId";
            this.lblClientId.Size = new System.Drawing.Size(52, 15);
            this.lblClientId.TabIndex = 5;
            this.lblClientId.Text = "Client ID";
            // 
            // txtClientId
            // 
            this.txtClientId.Location = new System.Drawing.Point(295, 6);
            this.txtClientId.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtClientId.Name = "txtClientId";
            this.txtClientId.Size = new System.Drawing.Size(26, 23);
            this.txtClientId.TabIndex = 4;
            this.txtClientId.Text = "1";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(333, 6);
            this.btnConnect.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(75, 24);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(22, 337);
            this.txtMessage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMessage.Size = new System.Drawing.Size(1030, 268);
            this.txtMessage.TabIndex = 7;
            // 
            // lblSymbol
            // 
            this.lblSymbol.AutoSize = true;
            this.lblSymbol.Location = new System.Drawing.Point(9, 29);
            this.lblSymbol.Name = "lblSymbol";
            this.lblSymbol.Size = new System.Drawing.Size(47, 15);
            this.lblSymbol.TabIndex = 9;
            this.lblSymbol.Text = "Symbol";
            // 
            // txtSymbolCheckSymbol
            // 
            this.txtSymbolCheckSymbol.Location = new System.Drawing.Point(98, 21);
            this.txtSymbolCheckSymbol.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtSymbolCheckSymbol.Name = "txtSymbolCheckSymbol";
            this.txtSymbolCheckSymbol.Size = new System.Drawing.Size(54, 23);
            this.txtSymbolCheckSymbol.TabIndex = 8;
            this.txtSymbolCheckSymbol.Text = "MNQ";
            // 
            // btGetConnId
            // 
            this.btGetConnId.Location = new System.Drawing.Point(584, 11);
            this.btGetConnId.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btGetConnId.Name = "btGetConnId";
            this.btGetConnId.Size = new System.Drawing.Size(100, 61);
            this.btGetConnId.TabIndex = 10;
            this.btGetConnId.Text = "Match Symbol and get ConId";
            this.btGetConnId.UseVisualStyleBackColor = true;
            this.btGetConnId.Click += new System.EventHandler(this.btCheckSymbol_Click);
            // 
            // btListPositions
            // 
            this.btListPositions.Location = new System.Drawing.Point(9, 35);
            this.btListPositions.Name = "btListPositions";
            this.btListPositions.Size = new System.Drawing.Size(399, 25);
            this.btListPositions.TabIndex = 11;
            this.btListPositions.Text = "List positions";
            this.btListPositions.UseVisualStyleBackColor = true;
            this.btListPositions.Click += new System.EventHandler(this.btListPositions_Click);
            // 
            // btStrikes
            // 
            this.btStrikes.Location = new System.Drawing.Point(819, 133);
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
            this.txtSymbolStrike.Location = new System.Drawing.Point(395, 138);
            this.txtSymbolStrike.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtSymbolStrike.Name = "txtSymbolStrike";
            this.txtSymbolStrike.Size = new System.Drawing.Size(54, 23);
            this.txtSymbolStrike.TabIndex = 8;
            this.txtSymbolStrike.Text = "MNQ";
            // 
            // lblSymbolStrike
            // 
            this.lblSymbolStrike.AutoSize = true;
            this.lblSymbolStrike.Location = new System.Drawing.Point(310, 141);
            this.lblSymbolStrike.Name = "lblSymbolStrike";
            this.lblSymbolStrike.Size = new System.Drawing.Size(79, 15);
            this.lblSymbolStrike.TabIndex = 9;
            this.lblSymbolStrike.Text = "Symbol Strike";
            // 
            // txtExchange
            // 
            this.txtExchange.Location = new System.Drawing.Point(533, 137);
            this.txtExchange.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtExchange.Name = "txtExchange";
            this.txtExchange.Size = new System.Drawing.Size(70, 23);
            this.txtExchange.TabIndex = 8;
            this.txtExchange.Text = "GLOBEX";
            // 
            // lblExchange
            // 
            this.lblExchange.AutoSize = true;
            this.lblExchange.Location = new System.Drawing.Point(469, 141);
            this.lblExchange.Name = "lblExchange";
            this.lblExchange.Size = new System.Drawing.Size(58, 15);
            this.lblExchange.TabIndex = 9;
            this.lblExchange.Text = "Exchange";
            // 
            // txtSecType
            // 
            this.txtSecType.Location = new System.Drawing.Point(229, 136);
            this.txtSecType.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtSecType.Name = "txtSecType";
            this.txtSecType.Size = new System.Drawing.Size(54, 23);
            this.txtSecType.TabIndex = 8;
            this.txtSecType.Text = "IND";
            // 
            // lblSecType
            // 
            this.lblSecType.AutoSize = true;
            this.lblSecType.Location = new System.Drawing.Point(167, 140);
            this.lblSecType.Name = "lblSecType";
            this.lblSecType.Size = new System.Drawing.Size(54, 15);
            this.lblSecType.TabIndex = 9;
            this.lblSecType.Text = "Sec. type";
            // 
            // txtConId
            // 
            this.txtConId.Location = new System.Drawing.Point(701, 136);
            this.txtConId.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtConId.Name = "txtConId";
            this.txtConId.Size = new System.Drawing.Size(100, 23);
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
            // txtSymbolRealTime
            // 
            this.txtSymbolRealTime.Location = new System.Drawing.Point(75, 223);
            this.txtSymbolRealTime.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtSymbolRealTime.Name = "txtSymbolRealTime";
            this.txtSymbolRealTime.Size = new System.Drawing.Size(54, 23);
            this.txtSymbolRealTime.TabIndex = 8;
            this.txtSymbolRealTime.Text = "MNQ";
            // 
            // lblSymbolRealTime
            // 
            this.lblSymbolRealTime.AutoSize = true;
            this.lblSymbolRealTime.Location = new System.Drawing.Point(22, 227);
            this.lblSymbolRealTime.Name = "lblSymbolRealTime";
            this.lblSymbolRealTime.Size = new System.Drawing.Size(47, 15);
            this.lblSymbolRealTime.TabIndex = 9;
            this.lblSymbolRealTime.Text = "Symbol";
            // 
            // txtCurrencyRealTime
            // 
            this.txtCurrencyRealTime.Location = new System.Drawing.Point(395, 223);
            this.txtCurrencyRealTime.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtCurrencyRealTime.Name = "txtCurrencyRealTime";
            this.txtCurrencyRealTime.Size = new System.Drawing.Size(54, 23);
            this.txtCurrencyRealTime.TabIndex = 8;
            this.txtCurrencyRealTime.Text = "USD";
            // 
            // lblCurrencyRealTime
            // 
            this.lblCurrencyRealTime.AutoSize = true;
            this.lblCurrencyRealTime.Location = new System.Drawing.Point(334, 226);
            this.lblCurrencyRealTime.Name = "lblCurrencyRealTime";
            this.lblCurrencyRealTime.Size = new System.Drawing.Size(55, 15);
            this.lblCurrencyRealTime.TabIndex = 9;
            this.lblCurrencyRealTime.Text = "Currency";
            // 
            // txtExchangeRealTime
            // 
            this.txtExchangeRealTime.Location = new System.Drawing.Point(533, 223);
            this.txtExchangeRealTime.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtExchangeRealTime.Name = "txtExchangeRealTime";
            this.txtExchangeRealTime.Size = new System.Drawing.Size(70, 23);
            this.txtExchangeRealTime.TabIndex = 8;
            this.txtExchangeRealTime.Text = "GLOBEX";
            // 
            // lblExchangeRealTime
            // 
            this.lblExchangeRealTime.AutoSize = true;
            this.lblExchangeRealTime.Location = new System.Drawing.Point(467, 228);
            this.lblExchangeRealTime.Name = "lblExchangeRealTime";
            this.lblExchangeRealTime.Size = new System.Drawing.Size(58, 15);
            this.lblExchangeRealTime.TabIndex = 9;
            this.lblExchangeRealTime.Text = "Exchange";
            // 
            // txtSecTypeRealTime
            // 
            this.txtSecTypeRealTime.Location = new System.Drawing.Point(229, 223);
            this.txtSecTypeRealTime.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtSecTypeRealTime.Name = "txtSecTypeRealTime";
            this.txtSecTypeRealTime.Size = new System.Drawing.Size(54, 23);
            this.txtSecTypeRealTime.TabIndex = 8;
            this.txtSecTypeRealTime.Text = "IND";
            // 
            // lblSecTypeRealTime
            // 
            this.lblSecTypeRealTime.AutoSize = true;
            this.lblSecTypeRealTime.Location = new System.Drawing.Point(169, 226);
            this.lblSecTypeRealTime.Name = "lblSecTypeRealTime";
            this.lblSecTypeRealTime.Size = new System.Drawing.Size(54, 15);
            this.lblSecTypeRealTime.TabIndex = 9;
            this.lblSecTypeRealTime.Text = "Sec. type";
            // 
            // btReqRealTime
            // 
            this.btReqRealTime.Location = new System.Drawing.Point(819, 223);
            this.btReqRealTime.Name = "btReqRealTime";
            this.btReqRealTime.Size = new System.Drawing.Size(102, 23);
            this.btReqRealTime.TabIndex = 12;
            this.btReqRealTime.Text = "Req. Real Time";
            this.btReqRealTime.UseVisualStyleBackColor = true;
            this.btReqRealTime.Click += new System.EventHandler(this.btReqRealTime_Click);
            // 
            // btCancelRealTime
            // 
            this.btCancelRealTime.Location = new System.Drawing.Point(939, 223);
            this.btCancelRealTime.Name = "btCancelRealTime";
            this.btCancelRealTime.Size = new System.Drawing.Size(113, 23);
            this.btCancelRealTime.TabIndex = 13;
            this.btCancelRealTime.Text = "Cancel Real Time";
            this.btCancelRealTime.UseVisualStyleBackColor = true;
            this.btCancelRealTime.Click += new System.EventHandler(this.btCancelRealTime_Click);
            // 
            // txtLocalSymbolRealTime
            // 
            this.txtLocalSymbolRealTime.Location = new System.Drawing.Point(701, 224);
            this.txtLocalSymbolRealTime.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtLocalSymbolRealTime.Name = "txtLocalSymbolRealTime";
            this.txtLocalSymbolRealTime.Size = new System.Drawing.Size(100, 23);
            this.txtLocalSymbolRealTime.TabIndex = 8;
            this.txtLocalSymbolRealTime.Text = "MNQ";
            // 
            // lblLocalSymbol
            // 
            this.lblLocalSymbol.AutoSize = true;
            this.lblLocalSymbol.Location = new System.Drawing.Point(619, 229);
            this.lblLocalSymbol.Name = "lblLocalSymbol";
            this.lblLocalSymbol.Size = new System.Drawing.Size(78, 15);
            this.lblLocalSymbol.TabIndex = 9;
            this.lblLocalSymbol.Text = "Local Symbol";
            // 
            // lblLocalSymbolSample
            // 
            this.lblLocalSymbolSample.AutoSize = true;
            this.lblLocalSymbolSample.Location = new System.Drawing.Point(701, 207);
            this.lblLocalSymbolSample.Name = "lblLocalSymbolSample";
            this.lblLocalSymbolSample.Size = new System.Drawing.Size(84, 15);
            this.lblLocalSymbolSample.TabIndex = 9;
            this.lblLocalSymbolSample.Text = "MNQZ1 C1658";
            // 
            // lblFopSample
            // 
            this.lblFopSample.AutoSize = true;
            this.lblFopSample.Location = new System.Drawing.Point(229, 207);
            this.lblFopSample.Name = "lblFopSample";
            this.lblFopSample.Size = new System.Drawing.Size(29, 15);
            this.lblFopSample.TabIndex = 9;
            this.lblFopSample.Text = "FOP";
            // 
            // txtGenericTickList
            // 
            this.txtGenericTickList.Location = new System.Drawing.Point(395, 186);
            this.txtGenericTickList.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtGenericTickList.Name = "txtGenericTickList";
            this.txtGenericTickList.Size = new System.Drawing.Size(54, 23);
            this.txtGenericTickList.TabIndex = 8;
            // 
            // lblGenericTicksList
            // 
            this.lblGenericTicksList.AutoSize = true;
            this.lblGenericTicksList.Location = new System.Drawing.Point(292, 189);
            this.lblGenericTicksList.Name = "lblGenericTicksList";
            this.lblGenericTicksList.Size = new System.Drawing.Size(92, 15);
            this.lblGenericTicksList.TabIndex = 9;
            this.lblGenericTicksList.Text = "Generic Tick List";
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(22, 610);
            this.btnClear.Name = "btnClear";
            this.btnClear.Size = new System.Drawing.Size(75, 23);
            this.btnClear.TabIndex = 14;
            this.btnClear.Text = "Clear";
            this.btnClear.UseVisualStyleBackColor = true;
            this.btnClear.Click += new System.EventHandler(this.btnClear_Click);
            // 
            // txtSecTypeCheckSymbol
            // 
            this.txtSecTypeCheckSymbol.Location = new System.Drawing.Point(360, 21);
            this.txtSecTypeCheckSymbol.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtSecTypeCheckSymbol.Name = "txtSecTypeCheckSymbol";
            this.txtSecTypeCheckSymbol.Size = new System.Drawing.Size(54, 23);
            this.txtSecTypeCheckSymbol.TabIndex = 8;
            this.txtSecTypeCheckSymbol.Text = "IND";
            // 
            // txtExchangeCheckSymbol
            // 
            this.txtExchangeCheckSymbol.Location = new System.Drawing.Point(494, 21);
            this.txtExchangeCheckSymbol.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtExchangeCheckSymbol.Name = "txtExchangeCheckSymbol";
            this.txtExchangeCheckSymbol.Size = new System.Drawing.Size(70, 23);
            this.txtExchangeCheckSymbol.TabIndex = 8;
            this.txtExchangeCheckSymbol.Text = "GLOBEX";
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(295, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(54, 15);
            this.label1.TabIndex = 9;
            this.label1.Text = "Sec. type";
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(430, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(58, 15);
            this.label2.TabIndex = 9;
            this.label2.Text = "Exchange";
            // 
            // btPlaceOrder
            // 
            this.btPlaceOrder.Location = new System.Drawing.Point(962, 271);
            this.btPlaceOrder.Name = "btPlaceOrder";
            this.btPlaceOrder.Size = new System.Drawing.Size(90, 23);
            this.btPlaceOrder.TabIndex = 15;
            this.btPlaceOrder.Text = "Place Order";
            this.btPlaceOrder.UseVisualStyleBackColor = true;
            this.btPlaceOrder.Click += new System.EventHandler(this.btPlaceOrder_Click);
            // 
            // lblSymbolOrder
            // 
            this.lblSymbolOrder.AutoSize = true;
            this.lblSymbolOrder.Location = new System.Drawing.Point(108, 271);
            this.lblSymbolOrder.Name = "lblSymbolOrder";
            this.lblSymbolOrder.Size = new System.Drawing.Size(47, 15);
            this.lblSymbolOrder.TabIndex = 18;
            this.lblSymbolOrder.Text = "Symbol";
            // 
            // txtSymbolOrder
            // 
            this.txtSymbolOrder.Location = new System.Drawing.Point(163, 267);
            this.txtSymbolOrder.Name = "txtSymbolOrder";
            this.txtSymbolOrder.Size = new System.Drawing.Size(60, 23);
            this.txtSymbolOrder.TabIndex = 19;
            this.txtSymbolOrder.Text = "DAX";
            // 
            // txtExchangeOrder
            // 
            this.txtExchangeOrder.Location = new System.Drawing.Point(298, 268);
            this.txtExchangeOrder.Name = "txtExchangeOrder";
            this.txtExchangeOrder.Size = new System.Drawing.Size(60, 23);
            this.txtExchangeOrder.TabIndex = 25;
            this.txtExchangeOrder.Text = "DTB";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(234, 271);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 15);
            this.label5.TabIndex = 24;
            this.label5.Text = "Exchange";
            // 
            // txtOrderLimitPrice
            // 
            this.txtOrderLimitPrice.Location = new System.Drawing.Point(473, 298);
            this.txtOrderLimitPrice.Name = "txtOrderLimitPrice";
            this.txtOrderLimitPrice.Size = new System.Drawing.Size(60, 23);
            this.txtOrderLimitPrice.TabIndex = 31;
            this.txtOrderLimitPrice.Text = "16300";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(409, 301);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(63, 15);
            this.label6.TabIndex = 30;
            this.label6.Text = "Limit Price";
            // 
            // txtQuantityOrder
            // 
            this.txtQuantityOrder.Location = new System.Drawing.Point(310, 297);
            this.txtQuantityOrder.Name = "txtQuantityOrder";
            this.txtQuantityOrder.Size = new System.Drawing.Size(60, 23);
            this.txtQuantityOrder.TabIndex = 29;
            this.txtQuantityOrder.Text = "1";
            // 
            // txtOrderQuantity
            // 
            this.txtOrderQuantity.AutoSize = true;
            this.txtOrderQuantity.Location = new System.Drawing.Point(257, 301);
            this.txtOrderQuantity.Name = "txtOrderQuantity";
            this.txtOrderQuantity.Size = new System.Drawing.Size(53, 15);
            this.txtOrderQuantity.TabIndex = 28;
            this.txtOrderQuantity.Text = "Quantity";
            // 
            // txtOrderAction
            // 
            this.txtOrderAction.Location = new System.Drawing.Point(159, 297);
            this.txtOrderAction.Name = "txtOrderAction";
            this.txtOrderAction.Size = new System.Drawing.Size(60, 23);
            this.txtOrderAction.TabIndex = 27;
            this.txtOrderAction.Text = "BUY";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(78, 300);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(75, 15);
            this.label8.TabIndex = 26;
            this.label8.Text = "Order Action";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(390, 275);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(43, 15);
            this.label12.TabIndex = 22;
            this.label12.Text = "Con ID";
            // 
            // txtConIdOrder
            // 
            this.txtConIdOrder.Location = new System.Drawing.Point(439, 272);
            this.txtConIdOrder.Name = "txtConIdOrder";
            this.txtConIdOrder.Size = new System.Drawing.Size(60, 23);
            this.txtConIdOrder.TabIndex = 23;
            this.txtConIdOrder.Text = "483046924";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.btDocumentationCheckSymbol);
            this.groupBox1.Controls.Add(this.txtCurrencyCheckSymbol);
            this.groupBox1.Controls.Add(this.label16);
            this.groupBox1.Controls.Add(this.txtLastTradeCheckSymbol);
            this.groupBox1.Controls.Add(this.label13);
            this.groupBox1.Controls.Add(this.txtStrikeCheckSymbol);
            this.groupBox1.Controls.Add(this.label14);
            this.groupBox1.Controls.Add(this.txtRightCheckSymbol);
            this.groupBox1.Controls.Add(this.label15);
            this.groupBox1.Controls.Add(this.txtSymbolCheckSymbol);
            this.groupBox1.Controls.Add(this.lblSymbol);
            this.groupBox1.Controls.Add(this.txtSecTypeCheckSymbol);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.txtExchangeCheckSymbol);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.btGetConnId);
            this.groupBox1.Location = new System.Drawing.Point(416, 6);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(690, 86);
            this.groupBox1.TabIndex = 32;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "Contract ID";
            // 
            // btDocumentationCheckSymbol
            // 
            this.btDocumentationCheckSymbol.Location = new System.Drawing.Point(430, 49);
            this.btDocumentationCheckSymbol.Name = "btDocumentationCheckSymbol";
            this.btDocumentationCheckSymbol.Size = new System.Drawing.Size(134, 23);
            this.btDocumentationCheckSymbol.TabIndex = 19;
            this.btDocumentationCheckSymbol.Text = "Documentation";
            this.btDocumentationCheckSymbol.UseVisualStyleBackColor = true;
            this.btDocumentationCheckSymbol.Click += new System.EventHandler(this.btDocumentationCheckSymbol_Click);
            // 
            // txtCurrencyCheckSymbol
            // 
            this.txtCurrencyCheckSymbol.Location = new System.Drawing.Point(225, 21);
            this.txtCurrencyCheckSymbol.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtCurrencyCheckSymbol.Name = "txtCurrencyCheckSymbol";
            this.txtCurrencyCheckSymbol.Size = new System.Drawing.Size(54, 23);
            this.txtCurrencyCheckSymbol.TabIndex = 17;
            this.txtCurrencyCheckSymbol.Text = "USD";
            // 
            // label16
            // 
            this.label16.AutoSize = true;
            this.label16.Location = new System.Drawing.Point(163, 29);
            this.label16.Name = "label16";
            this.label16.Size = new System.Drawing.Size(55, 15);
            this.label16.TabIndex = 18;
            this.label16.Text = "Currency";
            // 
            // txtLastTradeCheckSymbol
            // 
            this.txtLastTradeCheckSymbol.Location = new System.Drawing.Point(98, 48);
            this.txtLastTradeCheckSymbol.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtLastTradeCheckSymbol.Name = "txtLastTradeCheckSymbol";
            this.txtLastTradeCheckSymbol.Size = new System.Drawing.Size(54, 23);
            this.txtLastTradeCheckSymbol.TabIndex = 11;
            // 
            // label13
            // 
            this.label13.AutoSize = true;
            this.label13.Location = new System.Drawing.Point(9, 56);
            this.label13.Name = "label13";
            this.label13.Size = new System.Drawing.Size(86, 15);
            this.label13.TabIndex = 14;
            this.label13.Text = "Last Trade Date";
            // 
            // txtStrikeCheckSymbol
            // 
            this.txtStrikeCheckSymbol.Location = new System.Drawing.Point(225, 48);
            this.txtStrikeCheckSymbol.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtStrikeCheckSymbol.Name = "txtStrikeCheckSymbol";
            this.txtStrikeCheckSymbol.Size = new System.Drawing.Size(54, 23);
            this.txtStrikeCheckSymbol.TabIndex = 12;
            // 
            // label14
            // 
            this.label14.AutoSize = true;
            this.label14.Location = new System.Drawing.Point(163, 56);
            this.label14.Name = "label14";
            this.label14.Size = new System.Drawing.Size(36, 15);
            this.label14.TabIndex = 15;
            this.label14.Text = "Strike";
            // 
            // txtRightCheckSymbol
            // 
            this.txtRightCheckSymbol.Location = new System.Drawing.Point(359, 48);
            this.txtRightCheckSymbol.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtRightCheckSymbol.Name = "txtRightCheckSymbol";
            this.txtRightCheckSymbol.Size = new System.Drawing.Size(55, 23);
            this.txtRightCheckSymbol.TabIndex = 13;
            // 
            // label15
            // 
            this.label15.AutoSize = true;
            this.label15.Location = new System.Drawing.Point(295, 56);
            this.label15.Name = "label15";
            this.label15.Size = new System.Drawing.Size(35, 15);
            this.label15.TabIndex = 16;
            this.label15.Text = "Right";
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1118, 662);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.txtOrderLimitPrice);
            this.Controls.Add(this.label6);
            this.Controls.Add(this.txtQuantityOrder);
            this.Controls.Add(this.txtOrderQuantity);
            this.Controls.Add(this.txtOrderAction);
            this.Controls.Add(this.label8);
            this.Controls.Add(this.txtExchangeOrder);
            this.Controls.Add(this.label5);
            this.Controls.Add(this.txtConIdOrder);
            this.Controls.Add(this.label12);
            this.Controls.Add(this.txtSymbolOrder);
            this.Controls.Add(this.lblSymbolOrder);
            this.Controls.Add(this.btPlaceOrder);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btCancelRealTime);
            this.Controls.Add(this.btReqRealTime);
            this.Controls.Add(this.btStrikes);
            this.Controls.Add(this.btListPositions);
            this.Controls.Add(this.lblReqId);
            this.Controls.Add(this.lblSymbolStrike);
            this.Controls.Add(this.lblExchangeRealTime);
            this.Controls.Add(this.lblExchange);
            this.Controls.Add(this.lblSecTypeRealTime);
            this.Controls.Add(this.lblSecType);
            this.Controls.Add(this.lblConId);
            this.Controls.Add(this.lblCurrencyRealTime);
            this.Controls.Add(this.lblFopSample);
            this.Controls.Add(this.lblLocalSymbolSample);
            this.Controls.Add(this.lblLocalSymbol);
            this.Controls.Add(this.lblGenericTicksList);
            this.Controls.Add(this.lblSymbolRealTime);
            this.Controls.Add(this.txtSymbolStrike);
            this.Controls.Add(this.txtExchangeRealTime);
            this.Controls.Add(this.txtReqId);
            this.Controls.Add(this.txtExchange);
            this.Controls.Add(this.txtSecTypeRealTime);
            this.Controls.Add(this.txtSecType);
            this.Controls.Add(this.txtCurrencyRealTime);
            this.Controls.Add(this.txtLocalSymbolRealTime);
            this.Controls.Add(this.txtGenericTickList);
            this.Controls.Add(this.txtSymbolRealTime);
            this.Controls.Add(this.txtConId);
            this.Controls.Add(this.txtMessage);
            this.Controls.Add(this.btnConnect);
            this.Controls.Add(this.lblClientId);
            this.Controls.Add(this.txtClientId);
            this.Controls.Add(this.lblPort);
            this.Controls.Add(this.txtPort);
            this.Controls.Add(this.lblHost);
            this.Controls.Add(this.txtHost);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "FormMain";
            this.Text = "Form1";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
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
        private System.Windows.Forms.TextBox txtSymbolCheckSymbol;
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
        private System.Windows.Forms.TextBox txtLocalSymbolRealTime;
        private System.Windows.Forms.Label lblLocalSymbol;
        private System.Windows.Forms.Label lblLocalSymbolSample;
        private System.Windows.Forms.Label lblFopSample;
        private System.Windows.Forms.TextBox txtGenericTickList;
        private System.Windows.Forms.Label lblGenericTicksList;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.TextBox txtSecTypeCheckSymbol;
        private System.Windows.Forms.TextBox txtExchangeCheckSymbol;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btPlaceOrder;
        private System.Windows.Forms.Label lblSymbolOrder;
        private System.Windows.Forms.TextBox txtSymbolOrder;
        private System.Windows.Forms.TextBox txtExchangeOrder;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtOrderLimitPrice;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtQuantityOrder;
        private System.Windows.Forms.Label txtOrderQuantity;
        private System.Windows.Forms.TextBox txtOrderAction;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtConIdOrder;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.TextBox txtLastTradeCheckSymbol;
        private System.Windows.Forms.Label label13;
        private System.Windows.Forms.TextBox txtStrikeCheckSymbol;
        private System.Windows.Forms.Label label14;
        private System.Windows.Forms.TextBox txtRightCheckSymbol;
        private System.Windows.Forms.Label label15;
        private System.Windows.Forms.TextBox txtCurrencyCheckSymbol;
        private System.Windows.Forms.Label label16;
        private System.Windows.Forms.Button btDocumentationCheckSymbol;
    }
}
