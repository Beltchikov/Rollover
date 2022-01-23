namespace Prototype
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
            this.txtSymbolStrike = new System.Windows.Forms.TextBox();
            this.lblSymbolStrike = new System.Windows.Forms.Label();
            this.txtExchange = new System.Windows.Forms.TextBox();
            this.lblExchange = new System.Windows.Forms.Label();
            this.txtSecType = new System.Windows.Forms.TextBox();
            this.lblSecType = new System.Windows.Forms.Label();
            this.txtConId = new System.Windows.Forms.TextBox();
            this.lblConId = new System.Windows.Forms.Label();
            this.txtExchangeMarketData = new System.Windows.Forms.TextBox();
            this.lblExchangeRealTime = new System.Windows.Forms.Label();
            this.btReqRealTime = new System.Windows.Forms.Button();
            this.btCancelRealTime = new System.Windows.Forms.Button();
            this.txtGenericTickListMarketData = new System.Windows.Forms.TextBox();
            this.lblGenericTicksList = new System.Windows.Forms.Label();
            this.btnClear = new System.Windows.Forms.Button();
            this.txtSecTypeCheckSymbol = new System.Windows.Forms.TextBox();
            this.txtExchangeCheckSymbol = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.label2 = new System.Windows.Forms.Label();
            this.btPlaceBasicOrder = new System.Windows.Forms.Button();
            this.lblSymbolOrder = new System.Windows.Forms.Label();
            this.txtSymbolComboOrder = new System.Windows.Forms.TextBox();
            this.txtExchangeBasicOrder = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.txtLimitPriceBasicOrder = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.txtQuantityBasicOrder = new System.Windows.Forms.TextBox();
            this.txtOrderQuantity = new System.Windows.Forms.Label();
            this.txtActionBasicOrder = new System.Windows.Forms.TextBox();
            this.label8 = new System.Windows.Forms.Label();
            this.label12 = new System.Windows.Forms.Label();
            this.txtConIdBasicOrder = new System.Windows.Forms.TextBox();
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
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.txtLimitPriceComboOrder = new System.Windows.Forms.TextBox();
            this.label17 = new System.Windows.Forms.Label();
            this.txtQuantityComboOrder = new System.Windows.Forms.TextBox();
            this.label18 = new System.Windows.Forms.Label();
            this.txtActionComboBox = new System.Windows.Forms.TextBox();
            this.label11 = new System.Windows.Forms.Label();
            this.txtBuyLegConId = new System.Windows.Forms.TextBox();
            this.label10 = new System.Windows.Forms.Label();
            this.txtSellLegConId = new System.Windows.Forms.TextBox();
            this.label9 = new System.Windows.Forms.Label();
            this.txtCurrencyComboBox = new System.Windows.Forms.TextBox();
            this.label7 = new System.Windows.Forms.Label();
            this.txtSecTypeComboOrder = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.txtExchageComboOrder = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.btComboOrder = new System.Windows.Forms.Button();
            this.groupBox5 = new System.Windows.Forms.GroupBox();
            this.groupBox6 = new System.Windows.Forms.GroupBox();
            this.label19 = new System.Windows.Forms.Label();
            this.txtConItMarketData = new System.Windows.Forms.TextBox();
            this.groupBox7 = new System.Windows.Forms.GroupBox();
            this.label20 = new System.Windows.Forms.Label();
            this.txtConIdContractDetails = new System.Windows.Forms.TextBox();
            this.label21 = new System.Windows.Forms.Label();
            this.txtExchangeContractDetails = new System.Windows.Forms.TextBox();
            this.btReqContractDetails = new System.Windows.Forms.Button();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.groupBox5.SuspendLayout();
            this.groupBox6.SuspendLayout();
            this.groupBox7.SuspendLayout();
            this.SuspendLayout();
            // 
            // txtHost
            // 
            this.txtHost.Location = new System.Drawing.Point(41, 21);
            this.txtHost.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtHost.Name = "txtHost";
            this.txtHost.Size = new System.Drawing.Size(64, 23);
            this.txtHost.TabIndex = 0;
            this.txtHost.Text = "localhost";
            // 
            // lblHost
            // 
            this.lblHost.AutoSize = true;
            this.lblHost.Location = new System.Drawing.Point(3, 29);
            this.lblHost.Name = "lblHost";
            this.lblHost.Size = new System.Drawing.Size(32, 15);
            this.lblHost.TabIndex = 1;
            this.lblHost.Text = "Host";
            // 
            // lblPort
            // 
            this.lblPort.AutoSize = true;
            this.lblPort.Location = new System.Drawing.Point(119, 29);
            this.lblPort.Name = "lblPort";
            this.lblPort.Size = new System.Drawing.Size(29, 15);
            this.lblPort.TabIndex = 3;
            this.lblPort.Text = "Port";
            // 
            // txtPort
            // 
            this.txtPort.Location = new System.Drawing.Point(158, 21);
            this.txtPort.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtPort.Name = "txtPort";
            this.txtPort.Size = new System.Drawing.Size(54, 23);
            this.txtPort.TabIndex = 2;
            this.txtPort.Text = "4001";
            // 
            // lblClientId
            // 
            this.lblClientId.AutoSize = true;
            this.lblClientId.Location = new System.Drawing.Point(229, 29);
            this.lblClientId.Name = "lblClientId";
            this.lblClientId.Size = new System.Drawing.Size(52, 15);
            this.lblClientId.TabIndex = 5;
            this.lblClientId.Text = "Client ID";
            // 
            // txtClientId
            // 
            this.txtClientId.Location = new System.Drawing.Point(290, 21);
            this.txtClientId.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtClientId.Name = "txtClientId";
            this.txtClientId.Size = new System.Drawing.Size(26, 23);
            this.txtClientId.TabIndex = 4;
            this.txtClientId.Text = "1";
            // 
            // btnConnect
            // 
            this.btnConnect.Location = new System.Drawing.Point(328, 20);
            this.btnConnect.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.btnConnect.Name = "btnConnect";
            this.btnConnect.Size = new System.Drawing.Size(68, 24);
            this.btnConnect.TabIndex = 0;
            this.btnConnect.Text = "Connect";
            this.btnConnect.UseVisualStyleBackColor = true;
            this.btnConnect.Click += new System.EventHandler(this.btnConnect_Click);
            // 
            // txtMessage
            // 
            this.txtMessage.Location = new System.Drawing.Point(5, 366);
            this.txtMessage.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtMessage.Multiline = true;
            this.txtMessage.Name = "txtMessage";
            this.txtMessage.ScrollBars = System.Windows.Forms.ScrollBars.Vertical;
            this.txtMessage.Size = new System.Drawing.Size(1095, 332);
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
            this.btListPositions.Location = new System.Drawing.Point(8, 67);
            this.btListPositions.Name = "btListPositions";
            this.btListPositions.Size = new System.Drawing.Size(402, 25);
            this.btListPositions.TabIndex = 11;
            this.btListPositions.Text = "List positions";
            this.btListPositions.UseVisualStyleBackColor = true;
            this.btListPositions.Click += new System.EventHandler(this.btListPositions_Click);
            // 
            // btStrikes
            // 
            this.btStrikes.Location = new System.Drawing.Point(158, 24);
            this.btStrikes.Name = "btStrikes";
            this.btStrikes.Size = new System.Drawing.Size(75, 23);
            this.btStrikes.TabIndex = 12;
            this.btStrikes.Text = "Strikes";
            this.btStrikes.UseVisualStyleBackColor = true;
            this.btStrikes.Click += new System.EventHandler(this.btStrikes_Click);
            // 
            // txtSymbolStrike
            // 
            this.txtSymbolStrike.Location = new System.Drawing.Point(67, 21);
            this.txtSymbolStrike.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtSymbolStrike.Name = "txtSymbolStrike";
            this.txtSymbolStrike.Size = new System.Drawing.Size(81, 23);
            this.txtSymbolStrike.TabIndex = 8;
            this.txtSymbolStrike.Text = "MNQ";
            // 
            // lblSymbolStrike
            // 
            this.lblSymbolStrike.AutoSize = true;
            this.lblSymbolStrike.Location = new System.Drawing.Point(9, 24);
            this.lblSymbolStrike.Name = "lblSymbolStrike";
            this.lblSymbolStrike.Size = new System.Drawing.Size(47, 15);
            this.lblSymbolStrike.TabIndex = 9;
            this.lblSymbolStrike.Text = "Symbol";
            // 
            // txtExchange
            // 
            this.txtExchange.Location = new System.Drawing.Point(67, 76);
            this.txtExchange.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtExchange.Name = "txtExchange";
            this.txtExchange.Size = new System.Drawing.Size(81, 23);
            this.txtExchange.TabIndex = 8;
            this.txtExchange.Text = "GLOBEX";
            // 
            // lblExchange
            // 
            this.lblExchange.AutoSize = true;
            this.lblExchange.Location = new System.Drawing.Point(3, 80);
            this.lblExchange.Name = "lblExchange";
            this.lblExchange.Size = new System.Drawing.Size(58, 15);
            this.lblExchange.TabIndex = 9;
            this.lblExchange.Text = "Exchange";
            // 
            // txtSecType
            // 
            this.txtSecType.Location = new System.Drawing.Point(67, 48);
            this.txtSecType.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtSecType.Name = "txtSecType";
            this.txtSecType.Size = new System.Drawing.Size(81, 23);
            this.txtSecType.TabIndex = 8;
            this.txtSecType.Text = "IND";
            // 
            // lblSecType
            // 
            this.lblSecType.AutoSize = true;
            this.lblSecType.Location = new System.Drawing.Point(2, 52);
            this.lblSecType.Name = "lblSecType";
            this.lblSecType.Size = new System.Drawing.Size(54, 15);
            this.lblSecType.TabIndex = 9;
            this.lblSecType.Text = "Sec. type";
            // 
            // txtConId
            // 
            this.txtConId.Location = new System.Drawing.Point(67, 106);
            this.txtConId.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtConId.Name = "txtConId";
            this.txtConId.Size = new System.Drawing.Size(81, 23);
            this.txtConId.TabIndex = 8;
            this.txtConId.Text = "362687422";
            // 
            // lblConId
            // 
            this.lblConId.AutoSize = true;
            this.lblConId.Location = new System.Drawing.Point(18, 107);
            this.lblConId.Name = "lblConId";
            this.lblConId.Size = new System.Drawing.Size(43, 15);
            this.lblConId.TabIndex = 9;
            this.lblConId.Text = "Con ID";
            // 
            // txtExchangeMarketData
            // 
            this.txtExchangeMarketData.Location = new System.Drawing.Point(109, 49);
            this.txtExchangeMarketData.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtExchangeMarketData.Name = "txtExchangeMarketData";
            this.txtExchangeMarketData.Size = new System.Drawing.Size(70, 23);
            this.txtExchangeMarketData.TabIndex = 8;
            this.txtExchangeMarketData.Text = "DTB";
            // 
            // lblExchangeRealTime
            // 
            this.lblExchangeRealTime.AutoSize = true;
            this.lblExchangeRealTime.Location = new System.Drawing.Point(6, 54);
            this.lblExchangeRealTime.Name = "lblExchangeRealTime";
            this.lblExchangeRealTime.Size = new System.Drawing.Size(58, 15);
            this.lblExchangeRealTime.TabIndex = 9;
            this.lblExchangeRealTime.Text = "Exchange";
            // 
            // btReqRealTime
            // 
            this.btReqRealTime.Location = new System.Drawing.Point(6, 108);
            this.btReqRealTime.Name = "btReqRealTime";
            this.btReqRealTime.Size = new System.Drawing.Size(97, 23);
            this.btReqRealTime.TabIndex = 12;
            this.btReqRealTime.Text = "Req. Real Time";
            this.btReqRealTime.UseVisualStyleBackColor = true;
            this.btReqRealTime.Click += new System.EventHandler(this.btReqRealTime_Click);
            // 
            // btCancelRealTime
            // 
            this.btCancelRealTime.Location = new System.Drawing.Point(109, 107);
            this.btCancelRealTime.Name = "btCancelRealTime";
            this.btCancelRealTime.Size = new System.Drawing.Size(70, 23);
            this.btCancelRealTime.TabIndex = 13;
            this.btCancelRealTime.Text = "Cancel";
            this.btCancelRealTime.UseVisualStyleBackColor = true;
            this.btCancelRealTime.Click += new System.EventHandler(this.btCancelRealTime_Click);
            // 
            // txtGenericTickListMarketData
            // 
            this.txtGenericTickListMarketData.Location = new System.Drawing.Point(109, 76);
            this.txtGenericTickListMarketData.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtGenericTickListMarketData.Name = "txtGenericTickListMarketData";
            this.txtGenericTickListMarketData.Size = new System.Drawing.Size(70, 23);
            this.txtGenericTickListMarketData.TabIndex = 8;
            // 
            // lblGenericTicksList
            // 
            this.lblGenericTicksList.AutoSize = true;
            this.lblGenericTicksList.Location = new System.Drawing.Point(6, 79);
            this.lblGenericTicksList.Name = "lblGenericTicksList";
            this.lblGenericTicksList.Size = new System.Drawing.Size(92, 15);
            this.lblGenericTicksList.TabIndex = 9;
            this.lblGenericTicksList.Text = "Generic Tick List";
            // 
            // btnClear
            // 
            this.btnClear.Location = new System.Drawing.Point(5, 703);
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
            // btPlaceBasicOrder
            // 
            this.btPlaceBasicOrder.Location = new System.Drawing.Point(281, 21);
            this.btPlaceBasicOrder.Name = "btPlaceBasicOrder";
            this.btPlaceBasicOrder.Size = new System.Drawing.Size(118, 23);
            this.btPlaceBasicOrder.TabIndex = 15;
            this.btPlaceBasicOrder.Text = "Place Order";
            this.btPlaceBasicOrder.UseVisualStyleBackColor = true;
            this.btPlaceBasicOrder.Click += new System.EventHandler(this.btPlaceOrder_Click);
            // 
            // lblSymbolOrder
            // 
            this.lblSymbolOrder.AutoSize = true;
            this.lblSymbolOrder.Location = new System.Drawing.Point(142, 26);
            this.lblSymbolOrder.Name = "lblSymbolOrder";
            this.lblSymbolOrder.Size = new System.Drawing.Size(47, 15);
            this.lblSymbolOrder.TabIndex = 18;
            this.lblSymbolOrder.Text = "Symbol";
            // 
            // txtSymbolComboOrder
            // 
            this.txtSymbolComboOrder.Location = new System.Drawing.Point(192, 22);
            this.txtSymbolComboOrder.Name = "txtSymbolComboOrder";
            this.txtSymbolComboOrder.Size = new System.Drawing.Size(60, 23);
            this.txtSymbolComboOrder.TabIndex = 19;
            this.txtSymbolComboOrder.Text = "DAX";
            // 
            // txtExchangeBasicOrder
            // 
            this.txtExchangeBasicOrder.Location = new System.Drawing.Point(202, 22);
            this.txtExchangeBasicOrder.Name = "txtExchangeBasicOrder";
            this.txtExchangeBasicOrder.Size = new System.Drawing.Size(60, 23);
            this.txtExchangeBasicOrder.TabIndex = 25;
            this.txtExchangeBasicOrder.Text = "DTB";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(142, 25);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(58, 15);
            this.label5.TabIndex = 24;
            this.label5.Text = "Exchange";
            // 
            // txtLimitPriceBasicOrder
            // 
            this.txtLimitPriceBasicOrder.Location = new System.Drawing.Point(339, 52);
            this.txtLimitPriceBasicOrder.Name = "txtLimitPriceBasicOrder";
            this.txtLimitPriceBasicOrder.Size = new System.Drawing.Size(60, 23);
            this.txtLimitPriceBasicOrder.TabIndex = 31;
            this.txtLimitPriceBasicOrder.Text = "16300";
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(275, 55);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(63, 15);
            this.label6.TabIndex = 30;
            this.label6.Text = "Limit Price";
            // 
            // txtQuantityBasicOrder
            // 
            this.txtQuantityBasicOrder.Location = new System.Drawing.Point(202, 51);
            this.txtQuantityBasicOrder.Name = "txtQuantityBasicOrder";
            this.txtQuantityBasicOrder.Size = new System.Drawing.Size(60, 23);
            this.txtQuantityBasicOrder.TabIndex = 29;
            this.txtQuantityBasicOrder.Text = "1";
            // 
            // txtOrderQuantity
            // 
            this.txtOrderQuantity.AutoSize = true;
            this.txtOrderQuantity.Location = new System.Drawing.Point(149, 55);
            this.txtOrderQuantity.Name = "txtOrderQuantity";
            this.txtOrderQuantity.Size = new System.Drawing.Size(53, 15);
            this.txtOrderQuantity.TabIndex = 28;
            this.txtOrderQuantity.Text = "Quantity";
            // 
            // txtActionBasicOrder
            // 
            this.txtActionBasicOrder.Location = new System.Drawing.Point(64, 51);
            this.txtActionBasicOrder.Name = "txtActionBasicOrder";
            this.txtActionBasicOrder.Size = new System.Drawing.Size(60, 23);
            this.txtActionBasicOrder.TabIndex = 27;
            this.txtActionBasicOrder.Text = "BUY";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(18, 54);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(42, 15);
            this.label8.TabIndex = 26;
            this.label8.Text = "Action";
            // 
            // label12
            // 
            this.label12.AutoSize = true;
            this.label12.Location = new System.Drawing.Point(15, 25);
            this.label12.Name = "label12";
            this.label12.Size = new System.Drawing.Size(43, 15);
            this.label12.TabIndex = 22;
            this.label12.Text = "Con ID";
            // 
            // txtConIdBasicOrder
            // 
            this.txtConIdBasicOrder.Location = new System.Drawing.Point(64, 22);
            this.txtConIdBasicOrder.Name = "txtConIdBasicOrder";
            this.txtConIdBasicOrder.Size = new System.Drawing.Size(60, 23);
            this.txtConIdBasicOrder.TabIndex = 23;
            this.txtConIdBasicOrder.Text = "483046924";
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
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.txtPort);
            this.groupBox2.Controls.Add(this.txtHost);
            this.groupBox2.Controls.Add(this.lblHost);
            this.groupBox2.Controls.Add(this.lblPort);
            this.groupBox2.Controls.Add(this.txtClientId);
            this.groupBox2.Controls.Add(this.lblClientId);
            this.groupBox2.Controls.Add(this.btnConnect);
            this.groupBox2.Location = new System.Drawing.Point(8, 6);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(402, 60);
            this.groupBox2.TabIndex = 33;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Connection";
            // 
            // groupBox3
            // 
            this.groupBox3.Controls.Add(this.txtConIdBasicOrder);
            this.groupBox3.Controls.Add(this.label12);
            this.groupBox3.Controls.Add(this.txtExchangeBasicOrder);
            this.groupBox3.Controls.Add(this.txtLimitPriceBasicOrder);
            this.groupBox3.Controls.Add(this.label6);
            this.groupBox3.Controls.Add(this.btPlaceBasicOrder);
            this.groupBox3.Controls.Add(this.label5);
            this.groupBox3.Controls.Add(this.txtActionBasicOrder);
            this.groupBox3.Controls.Add(this.txtQuantityBasicOrder);
            this.groupBox3.Controls.Add(this.txtOrderQuantity);
            this.groupBox3.Controls.Add(this.label8);
            this.groupBox3.Location = new System.Drawing.Point(8, 242);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(422, 100);
            this.groupBox3.TabIndex = 34;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "Basic Order";
            // 
            // groupBox4
            // 
            this.groupBox4.Controls.Add(this.txtLimitPriceComboOrder);
            this.groupBox4.Controls.Add(this.label17);
            this.groupBox4.Controls.Add(this.txtQuantityComboOrder);
            this.groupBox4.Controls.Add(this.label18);
            this.groupBox4.Controls.Add(this.txtActionComboBox);
            this.groupBox4.Controls.Add(this.label11);
            this.groupBox4.Controls.Add(this.txtBuyLegConId);
            this.groupBox4.Controls.Add(this.label10);
            this.groupBox4.Controls.Add(this.txtSellLegConId);
            this.groupBox4.Controls.Add(this.label9);
            this.groupBox4.Controls.Add(this.txtCurrencyComboBox);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.txtSecTypeComboOrder);
            this.groupBox4.Controls.Add(this.label4);
            this.groupBox4.Controls.Add(this.txtExchageComboOrder);
            this.groupBox4.Controls.Add(this.label3);
            this.groupBox4.Controls.Add(this.btComboOrder);
            this.groupBox4.Controls.Add(this.txtSymbolComboOrder);
            this.groupBox4.Controls.Add(this.lblSymbolOrder);
            this.groupBox4.Location = new System.Drawing.Point(442, 242);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(658, 119);
            this.groupBox4.TabIndex = 35;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "Combo Order";
            // 
            // txtLimitPriceComboOrder
            // 
            this.txtLimitPriceComboOrder.Location = new System.Drawing.Point(329, 89);
            this.txtLimitPriceComboOrder.Name = "txtLimitPriceComboOrder";
            this.txtLimitPriceComboOrder.Size = new System.Drawing.Size(60, 23);
            this.txtLimitPriceComboOrder.TabIndex = 41;
            this.txtLimitPriceComboOrder.Text = "16300";
            // 
            // label17
            // 
            this.label17.AutoSize = true;
            this.label17.Location = new System.Drawing.Point(265, 92);
            this.label17.Name = "label17";
            this.label17.Size = new System.Drawing.Size(63, 15);
            this.label17.TabIndex = 40;
            this.label17.Text = "Limit Price";
            // 
            // txtQuantityComboOrder
            // 
            this.txtQuantityComboOrder.Location = new System.Drawing.Point(192, 88);
            this.txtQuantityComboOrder.Name = "txtQuantityComboOrder";
            this.txtQuantityComboOrder.Size = new System.Drawing.Size(60, 23);
            this.txtQuantityComboOrder.TabIndex = 39;
            this.txtQuantityComboOrder.Text = "1";
            // 
            // label18
            // 
            this.label18.AutoSize = true;
            this.label18.Location = new System.Drawing.Point(139, 92);
            this.label18.Name = "label18";
            this.label18.Size = new System.Drawing.Size(53, 15);
            this.label18.TabIndex = 38;
            this.label18.Text = "Quantity";
            // 
            // txtActionComboBox
            // 
            this.txtActionComboBox.Location = new System.Drawing.Point(69, 88);
            this.txtActionComboBox.Name = "txtActionComboBox";
            this.txtActionComboBox.Size = new System.Drawing.Size(60, 23);
            this.txtActionComboBox.TabIndex = 37;
            this.txtActionComboBox.Text = "BUY";
            // 
            // label11
            // 
            this.label11.AutoSize = true;
            this.label11.Location = new System.Drawing.Point(9, 91);
            this.label11.Name = "label11";
            this.label11.Size = new System.Drawing.Size(42, 15);
            this.label11.TabIndex = 36;
            this.label11.Text = "Action";
            // 
            // txtBuyLegConId
            // 
            this.txtBuyLegConId.Location = new System.Drawing.Point(283, 57);
            this.txtBuyLegConId.Name = "txtBuyLegConId";
            this.txtBuyLegConId.Size = new System.Drawing.Size(70, 23);
            this.txtBuyLegConId.TabIndex = 35;
            this.txtBuyLegConId.Text = "534020675";
            // 
            // label10
            // 
            this.label10.AutoSize = true;
            this.label10.Location = new System.Drawing.Point(192, 60);
            this.label10.Name = "label10";
            this.label10.Size = new System.Drawing.Size(87, 15);
            this.label10.TabIndex = 34;
            this.label10.Text = "Buy Leg Con Id";
            // 
            // txtSellLegConId
            // 
            this.txtSellLegConId.Location = new System.Drawing.Point(100, 56);
            this.txtSellLegConId.Name = "txtSellLegConId";
            this.txtSellLegConId.Size = new System.Drawing.Size(70, 23);
            this.txtSellLegConId.TabIndex = 33;
            this.txtSellLegConId.Text = "483046924";
            // 
            // label9
            // 
            this.label9.AutoSize = true;
            this.label9.Location = new System.Drawing.Point(9, 59);
            this.label9.Name = "label9";
            this.label9.Size = new System.Drawing.Size(85, 15);
            this.label9.TabIndex = 32;
            this.label9.Text = "Sell Leg Con Id";
            // 
            // txtCurrencyComboBox
            // 
            this.txtCurrencyComboBox.Location = new System.Drawing.Point(454, 22);
            this.txtCurrencyComboBox.Name = "txtCurrencyComboBox";
            this.txtCurrencyComboBox.Size = new System.Drawing.Size(60, 23);
            this.txtCurrencyComboBox.TabIndex = 31;
            this.txtCurrencyComboBox.Text = "EUR";
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(398, 26);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(55, 15);
            this.label7.TabIndex = 30;
            this.label7.Text = "Currency";
            // 
            // txtSecTypeComboOrder
            // 
            this.txtSecTypeComboOrder.Location = new System.Drawing.Point(322, 22);
            this.txtSecTypeComboOrder.Name = "txtSecTypeComboOrder";
            this.txtSecTypeComboOrder.Size = new System.Drawing.Size(60, 23);
            this.txtSecTypeComboOrder.TabIndex = 29;
            this.txtSecTypeComboOrder.Text = "BAG";
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(266, 26);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(55, 15);
            this.label4.TabIndex = 28;
            this.label4.Text = "Sec. Type";
            // 
            // txtExchageComboOrder
            // 
            this.txtExchageComboOrder.Location = new System.Drawing.Point(69, 22);
            this.txtExchageComboOrder.Name = "txtExchageComboOrder";
            this.txtExchageComboOrder.Size = new System.Drawing.Size(60, 23);
            this.txtExchageComboOrder.TabIndex = 27;
            this.txtExchageComboOrder.Text = "DTB";
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(9, 25);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(58, 15);
            this.label3.TabIndex = 26;
            this.label3.Text = "Exchange";
            // 
            // btComboOrder
            // 
            this.btComboOrder.Location = new System.Drawing.Point(537, 21);
            this.btComboOrder.Name = "btComboOrder";
            this.btComboOrder.Size = new System.Drawing.Size(94, 23);
            this.btComboOrder.TabIndex = 0;
            this.btComboOrder.Text = "Place Order";
            this.btComboOrder.UseVisualStyleBackColor = true;
            this.btComboOrder.Click += new System.EventHandler(this.btComboOrder_Click);
            // 
            // groupBox5
            // 
            this.groupBox5.Controls.Add(this.txtSecType);
            this.groupBox5.Controls.Add(this.lblSecType);
            this.groupBox5.Controls.Add(this.txtSymbolStrike);
            this.groupBox5.Controls.Add(this.txtConId);
            this.groupBox5.Controls.Add(this.txtExchange);
            this.groupBox5.Controls.Add(this.lblConId);
            this.groupBox5.Controls.Add(this.btStrikes);
            this.groupBox5.Controls.Add(this.lblExchange);
            this.groupBox5.Controls.Add(this.lblSymbolStrike);
            this.groupBox5.Location = new System.Drawing.Point(8, 98);
            this.groupBox5.Name = "groupBox5";
            this.groupBox5.Size = new System.Drawing.Size(240, 138);
            this.groupBox5.TabIndex = 36;
            this.groupBox5.TabStop = false;
            this.groupBox5.Text = "Strikes";
            // 
            // groupBox6
            // 
            this.groupBox6.Controls.Add(this.label19);
            this.groupBox6.Controls.Add(this.txtConItMarketData);
            this.groupBox6.Controls.Add(this.lblExchangeRealTime);
            this.groupBox6.Controls.Add(this.txtExchangeMarketData);
            this.groupBox6.Controls.Add(this.btCancelRealTime);
            this.groupBox6.Controls.Add(this.btReqRealTime);
            this.groupBox6.Controls.Add(this.lblGenericTicksList);
            this.groupBox6.Controls.Add(this.txtGenericTickListMarketData);
            this.groupBox6.Location = new System.Drawing.Point(254, 98);
            this.groupBox6.Name = "groupBox6";
            this.groupBox6.Size = new System.Drawing.Size(202, 138);
            this.groupBox6.TabIndex = 37;
            this.groupBox6.TabStop = false;
            this.groupBox6.Text = "Market Data (reqMktData)";
            // 
            // label19
            // 
            this.label19.AutoSize = true;
            this.label19.Location = new System.Drawing.Point(6, 26);
            this.label19.Name = "label19";
            this.label19.Size = new System.Drawing.Size(43, 15);
            this.label19.TabIndex = 15;
            this.label19.Text = "Con ID";
            // 
            // txtConItMarketData
            // 
            this.txtConItMarketData.Location = new System.Drawing.Point(109, 23);
            this.txtConItMarketData.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtConItMarketData.Name = "txtConItMarketData";
            this.txtConItMarketData.Size = new System.Drawing.Size(70, 23);
            this.txtConItMarketData.TabIndex = 14;
            this.txtConItMarketData.Text = "825711 ";
            // 
            // groupBox7
            // 
            this.groupBox7.Controls.Add(this.label20);
            this.groupBox7.Controls.Add(this.txtConIdContractDetails);
            this.groupBox7.Controls.Add(this.label21);
            this.groupBox7.Controls.Add(this.txtExchangeContractDetails);
            this.groupBox7.Controls.Add(this.btReqContractDetails);
            this.groupBox7.Location = new System.Drawing.Point(462, 98);
            this.groupBox7.Name = "groupBox7";
            this.groupBox7.Size = new System.Drawing.Size(216, 138);
            this.groupBox7.TabIndex = 38;
            this.groupBox7.TabStop = false;
            this.groupBox7.Text = "Contract Details (reqContractDetails)";
            // 
            // label20
            // 
            this.label20.AutoSize = true;
            this.label20.Location = new System.Drawing.Point(6, 26);
            this.label20.Name = "label20";
            this.label20.Size = new System.Drawing.Size(43, 15);
            this.label20.TabIndex = 15;
            this.label20.Text = "Con ID";
            // 
            // txtConIdContractDetails
            // 
            this.txtConIdContractDetails.Location = new System.Drawing.Point(109, 23);
            this.txtConIdContractDetails.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtConIdContractDetails.Name = "txtConIdContractDetails";
            this.txtConIdContractDetails.Size = new System.Drawing.Size(70, 23);
            this.txtConIdContractDetails.TabIndex = 14;
            this.txtConIdContractDetails.Text = "825711 ";
            // 
            // label21
            // 
            this.label21.AutoSize = true;
            this.label21.Location = new System.Drawing.Point(6, 54);
            this.label21.Name = "label21";
            this.label21.Size = new System.Drawing.Size(58, 15);
            this.label21.TabIndex = 9;
            this.label21.Text = "Exchange";
            // 
            // txtExchangeContractDetails
            // 
            this.txtExchangeContractDetails.Location = new System.Drawing.Point(109, 49);
            this.txtExchangeContractDetails.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.txtExchangeContractDetails.Name = "txtExchangeContractDetails";
            this.txtExchangeContractDetails.Size = new System.Drawing.Size(70, 23);
            this.txtExchangeContractDetails.TabIndex = 8;
            this.txtExchangeContractDetails.Text = "DTB";
            // 
            // btReqContractDetails
            // 
            this.btReqContractDetails.Location = new System.Drawing.Point(6, 108);
            this.btReqContractDetails.Name = "btReqContractDetails";
            this.btReqContractDetails.Size = new System.Drawing.Size(173, 23);
            this.btReqContractDetails.TabIndex = 12;
            this.btReqContractDetails.Text = "Req. Contract Details";
            this.btReqContractDetails.UseVisualStyleBackColor = true;
            this.btReqContractDetails.Click += new System.EventHandler(this.btReqContractDetails_Click);
            // 
            // FormMain
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1118, 738);
            this.Controls.Add(this.groupBox7);
            this.Controls.Add(this.groupBox6);
            this.Controls.Add(this.groupBox5);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.btnClear);
            this.Controls.Add(this.btListPositions);
            this.Controls.Add(this.txtMessage);
            this.Margin = new System.Windows.Forms.Padding(3, 2, 3, 2);
            this.Name = "FormMain";
            this.Text = "Main";
            this.Load += new System.EventHandler(this.Form1_Load);
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.groupBox5.ResumeLayout(false);
            this.groupBox5.PerformLayout();
            this.groupBox6.ResumeLayout(false);
            this.groupBox6.PerformLayout();
            this.groupBox7.ResumeLayout(false);
            this.groupBox7.PerformLayout();
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
        private System.Windows.Forms.TextBox txtSymbolStrike;
        private System.Windows.Forms.Label lblSymbolStrike;
        private System.Windows.Forms.TextBox txtExchange;
        private System.Windows.Forms.Label lblExchange;
        private System.Windows.Forms.TextBox txtSecType;
        private System.Windows.Forms.Label lblSecType;
        private System.Windows.Forms.TextBox txtConId;
        private System.Windows.Forms.Label lblConId;
        private System.Windows.Forms.TextBox txtExchangeMarketData;
        private System.Windows.Forms.Label lblExchangeRealTime;
        private System.Windows.Forms.Button btReqRealTime;
        private System.Windows.Forms.Button btCancelRealTime;
        private System.Windows.Forms.TextBox txtGenericTickListMarketData;
        private System.Windows.Forms.Label lblGenericTicksList;
        private System.Windows.Forms.Button btnClear;
        private System.Windows.Forms.TextBox txtSecTypeCheckSymbol;
        private System.Windows.Forms.TextBox txtExchangeCheckSymbol;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button btPlaceBasicOrder;
        private System.Windows.Forms.Label lblSymbolOrder;
        private System.Windows.Forms.TextBox txtSymbolComboOrder;
        private System.Windows.Forms.TextBox txtExchangeBasicOrder;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.TextBox txtLimitPriceBasicOrder;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.TextBox txtQuantityBasicOrder;
        private System.Windows.Forms.Label txtOrderQuantity;
        private System.Windows.Forms.TextBox txtActionBasicOrder;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.Label label12;
        private System.Windows.Forms.TextBox txtConIdBasicOrder;
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
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.Button btComboOrder;
        private System.Windows.Forms.TextBox txtExchageComboOrder;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox txtSecTypeComboOrder;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.TextBox txtCurrencyComboBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.TextBox txtSellLegConId;
        private System.Windows.Forms.Label label9;
        private System.Windows.Forms.TextBox txtBuyLegConId;
        private System.Windows.Forms.Label label10;
        private System.Windows.Forms.TextBox txtActionComboBox;
        private System.Windows.Forms.Label label11;
        private System.Windows.Forms.TextBox txtLimitPriceComboOrder;
        private System.Windows.Forms.Label label17;
        private System.Windows.Forms.TextBox txtQuantityComboOrder;
        private System.Windows.Forms.Label label18;
        private System.Windows.Forms.GroupBox groupBox5;
        private System.Windows.Forms.GroupBox groupBox6;
        private System.Windows.Forms.Label label19;
        private System.Windows.Forms.TextBox txtConItMarketData;
        private System.Windows.Forms.GroupBox groupBox7;
        private System.Windows.Forms.Label label20;
        private System.Windows.Forms.TextBox txtConIdContractDetails;
        private System.Windows.Forms.Label label21;
        private System.Windows.Forms.TextBox txtExchangeContractDetails;
        private System.Windows.Forms.Button btReqContractDetails;
    }
}
