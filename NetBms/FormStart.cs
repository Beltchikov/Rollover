using System.Text.Json;

namespace NetBms
{
    public partial class FormStart : Form
    {
        readonly string BATCH_SEPARATOR = Environment.NewLine + Environment.NewLine;

        public FormStart()
        {
            InitializeComponent();
            //txtChatGptBatchResults.Text = TestData();
            txtChatGptBatchResults.Text = TestData2();
        }

        private void btGo_ClickAsync(object sender, EventArgs e)
        {
            var input = txtChatGptBatchResults.Text;

            // Deserialize text as ChatGptBatchResult JSON
            var batchesAsStringArray = input
                .Split(BATCH_SEPARATOR)
                .Select(s => s.Trim())
                .ToArray();
            var batches = new List<ChatGptBatchResult>();
            var notConvertableStrings = new List<string>();
            foreach (var batch in batchesAsStringArray)
            {
                try
                {
                    ChatGptBatchResult chatGptBatchResult = JsonSerializer.Deserialize<ChatGptBatchResult>(batch)!;
                    batches.Add(chatGptBatchResult);
                }
                catch (System.Text.Json.JsonException)
                {
                    notConvertableStrings.Add(batch);
                }
            }

            // Remove wrong symbols
            var processor = new Processor();
            (batches, List<string> wrongSymbols) = processor.RemoveWrongSymbols(batches);

            // SUM_BUY, SUM_SELL
            (var sumBuySignals, var sumSellSignals) = processor.SumBuySellFromBatches(batches);
            var crossSection = sumBuySignals.Keys.Intersect(sumSellSignals.Keys).ToList(); // Intersection. Evtl. for later

            // NET_BUY, NET_SELL
            (var netBuySignals, var netSellSignals) = processor.NetBuySellFromSum(sumBuySignals, sumSellSignals);

            // Order DESC by value
            var netBuyOrderedBuyValue = processor.OrderDictionaryByValue(netBuySignals, false);
            var netSellOrderedBuyValue = processor.OrderDictionaryByValue(netSellSignals, false);

            // Display results
            txtBuyResults.Text = "";
            txtBuyResults.Text += netBuyOrderedBuyValue
                .Select(r => r.Key + '\t' + r.Value.ToString())
                .Aggregate((r, n) => r + Environment.NewLine + n);

            txtSellResults.Text = "";
            txtSellResults.Text += netSellOrderedBuyValue
                .Select(r => r.Key + '\t' + r.Value.ToString())
                .Aggregate((r, n) => r + Environment.NewLine + n);

            txtErrors.Text = "";
            if(notConvertableStrings.Any())
                txtErrors.Text += notConvertableStrings.Aggregate((r, n) => r + BATCH_SEPARATOR + n);

            txtSymbolErrors.Text = "";
            if (wrongSymbols.Any())
                txtSymbolErrors.Text += wrongSymbols.Aggregate((r, n) => r + Environment.NewLine + n);
        }

        private string TestData2()
        {
            return @"{
    ""BUY"": [
      ""GOOGL"",
      ""TSLA"",
      ""MSFT"",
      ""AMZN"",
      ""AAPL"",
      ""FB"",
      ""NVDA"",
      ""PYPL"",
      ""ADBE"",
      ""NFLX"",
      ""CRM"",
      ""ASML"",
      ""INTU"",
      ""NOW"",
      ""SHOP"",
      ""SQ"",
      ""ZM"",
      ""AMD"",
      ""MELI"",
      ""BABA"",
      ""DOCU"",
      ""UBER"",
      ""ESTC"",
      ""NET"",
      ""PINS"",
      ""TWLO"",
      ""SNOW"",
      ""OKTA"",
      ""ROKU"",
      ""CRWD"",
      ""DDOG"",
      ""SE"",
      ""DDOG"",
      ""TEAM"",
      ""ABNB"",
      ""FSLY"",
      ""ROKU"",
      ""DDOG"",
      ""SHOP"",
      ""TWLO"",
      ""ZM"",
      ""NET"",
      ""U"",
      ""CRWD"",
      ""PINS"",
      ""TWTR"",
      ""RBLX"",
      ""ZM"",
      ""DDOG"",
      ""PINS"",
      ""DOCU"",
      ""SQ"",
      ""ROKU"",
      ""TDOC"",
      ""ESTC"",
      ""TWLO"",
      ""NET"",
      ""DDOG"",
      ""U"",
      ""SNOW"",
      ""ABNB"",
      ""FSLY"",
      ""SE"",
      ""CRWD"",
      ""TWTR"",
      ""DDOG"",
      ""PINS"",
      ""ZM"",
      ""ROKU"",
      ""TWLO"",
      ""NET"",
      ""DDOG"",
      ""SNOW"",
      ""TWTR"",
      ""ESTC"",
      ""CRWD"",
      ""PINS"",
      ""SE"",
      ""U"",
      ""TWLO"",
      ""ROKU"",
      ""DDOG"",
      ""NET"",
      ""SNOW"",
      ""ABNB"",
      ""FSLY"",
      ""ZM"",
      ""TWTR"",
      ""PINS"",
      ""ROKU"",
      ""CRWD"",
      ""ESTC"",
      ""TWLO"",
      ""DDOG"",
      ""NET"",
      ""SNOW""
    ],
    ""SELL"": [
      ""PG"",
      ""XOM"",
      ""VLO"",
      ""CVX"",
      ""HAL"",
      ""PSX"",
      ""EOG"",
      ""WMB"",
      ""MPC"",
      ""COP"",
      ""OXY"",
      ""KMI"",
      ""SLB"",
      ""DVN"",
      ""MRO"",
      ""CXO"",
      ""APA"",
      ""HES"",
      ""HFC"",
      ""FTI"",
      ""NOV"",
      ""HP"",
      ""VLO"",
      ""CVX"",
      ""XOM"",
      ""PSX"",
      ""KMI"",
      ""OXY"",
      ""MPC"",
      ""HAL"",
      ""SLB"",
      ""WMB"",
      ""EOG"",
      ""DVN"",
      ""MRO"",
      ""COP"",
      ""APA"",
      ""HES"",
      ""HFC"",
      ""FTI"",
      ""NOV"",
      ""HP"",
      ""VLO"",
      ""CVX"",
      ""XOM"",
      ""PSX"",
      ""KMI"",
      ""OXY"",
      ""MPC"",
      ""HAL"",
      ""SLB"",
      ""WMB"",
      ""EOG"",
      ""DVN"",
      ""MRO"",
      ""COP"",
      ""APA"",
      ""HES"",
      ""HFC"",
      ""FTI"",
      ""NOV"",
      ""HP"",
      ""VLO"",
      ""CVX"",
      ""XOM"",
      ""PSX"",
      ""KMI"",
      ""OXY"",
      ""MPC"",
      ""HAL"",
      ""SLB"",
      ""WMB"",
      ""EOG"",
      ""DVN"",
      ""MRO"",
      ""COP"",
      ""APA"",
      ""HES"",
      ""HFC"",
      ""FTI"",
      ""NOV"",
      ""HP""
    ]
  }

  
  {
    ""BUY"": [""EXXON"", ""NOVONORDISK"", ""ABBVIE"", ""PFIZER"", ""WHIRLPOOL"", ""MARUTISUZUKI"", ""MICROSOFT"", ""ALPHABET"", ""BHP"", ""ELLIOTT"", ""TOTAL"", ""NATWEST"", ""CHEVRON"", ""COPPER"", ""QATAR"", ""NOVALAND"", ""LVMH"", ""NOVARTIS"", ""GOLDMAN SACHS"", ""BARCLAYS"", ""BLACKSTONE"", ""CVC"", ""BANK OF AMERICA"", ""BNP PARIBAS"", ""SWEDEN"", ""UN"", ""JPMORGAN"", ""DARKTRACE""],
    ""SELL"": [""TAYLOR SWIFT"", ""ANA"", ""HSBC"", ""NOVALAND"", ""CHINA LIFE INSURANCE"", ""INDIAN AUTO"", ""APS ASSET MANAGEMENT"", ""BANK OF AMERICA"", ""BNP PARIBAS"", ""SWEDEN""]
  }
  
";
        }

        private string TestData()
        {
            return @"{
  ""BUY"": [""AMZN"", ""AAPL"", ""GOOGL"", ""MSFT"", ""TSLA"", ""NVDA"", ""ADBE"", ""PYPL"", ""SHOP"", ""NFLX"", ""CRM"", ""SQ"", ""MA"", ""INTU"", ""NOW"", ""SNOW"", ""ASML"", ""ATVI"", ""DOCU"", ""ZM"", ""OKTA"", ""NET"", ""FTNT"", ""TWLO"", ""PLTR"", ""DDOG"", ""SE"", ""CRWD"", ""ROKU"", ""DDOG"", ""SPLK"", ""PINS"", ""AYX"", ""COIN"", ""U"", ""SHOP"", ""CRSP"", ""TWTR"", ""PINS"", ""UBER"", ""LYFT"", ""ROKU"", ""PTON"", ""CRSP"", ""FSLY"", ""ETSY"", ""NET"", ""BABA"", ""SNAP"", ""ZM"", ""SPOT"", ""TWLO"", ""NOW"", ""TEAM"", ""ZS"", ""DDOG"", ""DDOG"", ""PLTR"", ""CRWD"", ""DDOG"", ""CRWD"", ""SNOW"", ""ETSY"", ""CRWD"", ""PINS"", ""SNOW"", ""FSLY"", ""COIN"", ""CRWD"", ""ETSY"", ""DDOG"", ""U"", ""DDOG"", ""DOCU"", ""NET"", ""ROKU"", ""CRWD"", ""DDOG"", ""DDOG"", ""CRWD"", ""DDOG"", ""CRSP"", ""DDOG"", ""CRWD"", ""CRWD"", ""ROKU"", ""CRSP"", ""DDOG"", ""CRWD"", ""CRWD"", ""CRWD"", ""CRWD"", ""DDOG"", ""CRWD"", ""CRWD"", ""CRWD"", ""CRWD"", ""DDOG"", ""CRWD"", ""CRWD"", ""DDOG"", ""CRWD"", ""CRWD"", ""DDOG"", ""CRWD"", ""CRWD"", ""CRWD"", ""CRWD"", ""CRWD"", ""CRWD"", ""DDOG"", ""CRWD"", ""CRWD"", ""DDOG"", ""CRWD"", ""DDOG"", ""CRWD"", ""CRWD"", ""DDOG"", ""CRWD"", ""CRWD"", ""CRWD"", ""DDOG"", ""DDOG"", ""DDOG"", ""CRWD"", ""DDOG"", ""DDOG"", ""DDOG"", ""CRWD"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"", ""DDOG"",

{ ""BUY"": [""TRLY"", ""GLW"", ""IBB"", ""GS"", ""BAC"", ""JPM"", ""AAPL"", ""MSFT"", ""GOOGL"", ""TSLA"", ""AMZN"", ""FB"", ""NVDA"", ""AMD"", ""PYPL"", ""V"", ""MA"", ""WMT"", ""HD"", ""CRM"", ""ADBE"", ""NFLX"", ""INTC"", ""CSCO"", ""QCOM"", ""UNH"", ""DIS"", ""ABT"", ""T"", ""VZ"", ""PFE"", ""MRK"", ""ABBV"", ""CVS"", ""BMY"", ""JNJ"", ""KO"", ""PEP"", ""PG"", ""MCD"", ""COST"", ""SBUX"", ""CMCSA"", ""TMO"", ""NOW"", ""ORCL"", ""SAP"", ""ACN"", ""TGT"", ""NKE"", ""UPS"", ""FDX"", ""LOW"", ""JD"", ""BABA"", ""SHOP"", ""ZM"", ""CRM"", ""SQ"", ""UBER"", ""LYFT"", ""DKNG"", ""GM"", ""F"", ""GOEV"", ""RIVN"", ""LCID"", ""NIO"", ""XPEV"", ""FUBO"", ""ROKU"", ""CRWD"", ""ZS"", ""NET"", ""TWLO"", ""DOCU"", ""SNOW"", ""PLTR"", ""UPST"", ""SE"", ""DDOG"", ""TEAM"", ""OKTA"", ""PD"", ""PINS"", ""ROKU"", ""TTD"", ""BAND"", ""ASAN"", ""ZM"", ""U"", ""FSLY"", ""SPLK"", ""TWTR"", ""SNAP""], ""SELL"": [""BTC"", ""ETH"", ""LTC"", ""ADA"", ""SOL"", ""DOT"", ""AVAX"", ""LINK"", ""ATOM"", ""FIL"", ""XLM"", ""UNI"", ""MATIC"", ""VET"", ""AAVE"", ""TRX"", ""FTT"", ""EOS"", ""THETA"", ""SHIB"", ""DOGE"", ""BNB"", ""SUSHI"", ""RUNE"", ""CAKE"", ""AXS"", ""MATIC"", ""COMP"", ""LUNA"", ""NEAR"", ""SAND"", ""1INCH"", ""REN"", ""YFI"", ""SNX"", ""ALGO"", ""XMR"", ""MKR"", ""WAVES"", ""CRO"", ""ICP"", ""EGLD"", ""HT"", ""TFUEL"", ""CHZ"", ""GRT"", ""KSM"", ""IOST"", ""MANA"", ""HNT"", ""CRV"", ""GNO"", ""KNC"", ""ETC"", ""QTUM"", ""ZRX"", ""ANKR"", ""RSR"", ""BAL"", ""YFI"", ""ENJ"", ""BTT"", ""BCH"", ""BTC"", ""ETH"", ""LTC"", ""ADA"", ""SOL"", ""DOT"", ""AVAX"", ""LINK"", ""ATOM"", ""FIL"", ""XLM"", ""UNI"", ""MATIC"", ""VET"", ""AAVE"", ""TRX"", ""FTT"", ""EOS"", ""THETA"", ""SHIB"", ""DOGE"", ""BNB"", ""SUSHI"", ""RUNE"", ""CAKE"", ""AXS"", ""MATIC"", ""COMP"", ""LUNA"", ""NEAR"", ""SAND"", ""1INCH"", ""REN"", ""YFI"", ""SNX"", ""ALGO""] }

{ ""BUY"": [""MSFT"", ""AAPL"", ""GOOGL"", ""AMZN"", ""FB"", ""NVDA"", ""TSLA"", ""BRK-A"", ""BRK-B"", ""JPM"", ""V"", ""MA"", ""JNJ"", ""WMT"", ""DIS"", ""PG"", ""PYPL"", ""BAC"", ""HD"", ""INTC"", ""CMCSA"", ""VZ"", ""NFLX"", ""ADBE"", ""CSCO"", ""NKE"", ""PEP"", ""T"", ""MRK"", ""CRM"", ""XOM"", ""KO"", ""ABT"", ""UNH"", ""CVX"", ""TMUS"", ""COST"", ""ABBV"", ""WFC"", ""MCD"", ""AVGO"", ""QCOM"", ""LLY"", ""MO"", ""NEE"", ""MDT"", ""TMO"", ""ACN"", ""AMGN"", ""IBM"", ""PM"", ""SBUX"", ""HON"", ""TXN"", ""LIN"", ""DHR"", ""LMT"", ""NOW"", ""PYPL"", ""MMM"", ""UNP"", ""LOW"", ""BMY"", ""UPS"", ""RTX"", ""CAT"", ""INTU"", ""AMD"", ""CHTR"", ""ADP"", ""NOC"", ""BKNG"", ""BLK"", ""ORCL"", ""DE"", ""GS"", ""TGT"", ""ANTM"", ""DUK"", ""FDX"", ""MS"", ""PLD"", ""CCI"", ""BDX"", ""SYK"", ""ISRG"", ""ZTS"", ""CI"", ""MMC"", ""D"", ""VRTX"", ""ICE"", ""SO"", ""AGN"", ""MU""], ""SELL"": [""SPCE"", ""TLRY"", ""APHA"", ""PLUG"", ""JMIA"", ""ZM"", ""WORK"", ""NOK"", ""RIOT"", ""FSLY"", ""NKLA"", ""WKHS"", ""PTON"", ""RKT"", ""UBER"", ""DKNG"", ""BBBY"", ""GME"", ""SOS"", ""ROKU"", ""MVIS"", ""EXPR"", ""GSAT"", ""BB"", ""VIAC"", ""IQ"", ""AMC"", ""AAL"", ""GE"", ""OXY"", ""MRO"", ""CCL"", ""NCLH"", ""RCL"", ""UAL"", ""SAVE"", ""DAL"", ""SPG"", ""WYNN"", ""MGM"", ""LVS"", ""VXRT"", ""CLNE"", ""ENPH"", ""WKHS"", ""DKNG"", ""WATT"", ""BLNK"", ""HEXO"", ""RIDE"", ""FUBO"", ""JMIA"", ""PSTH"", ""SOS"", ""SNDL"", ""CCIV"", ""FSR"", ""LAZR"", ""RIDE"", ""FCEL"", ""XPEV"", ""KNDI"", ""RMO"", ""F"", ""LI"", ""NKLA"", ""HYLN"", ""QS"", ""SKLZ"", ""NET"", ""CRSR"", ""GRWG"", ""ACB"", ""BNGO"", ""KHC"", ""SOS"", ""X"", ""CRSP"", ""PDD"", ""PENN"", ""MRNA"", ""CRWD"", ""SNAP"", ""DASH"", ""BYND"", ""JD"", ""HUYA"", ""UWMC"", ""EH"", ""TIGR"", ""FSR"", ""OCGN"", ""WISH"", ""UPST"", ""COIN""] }

{ ""BUY"": [""JNJ"", ""BNY"", ""RIVN"", ""PG"", ""AAPL"", ""MSFT"", ""VZ"", ""UNH"", ""GOOGL"", ""TSLA"", ""AMZN"", ""BRK-A"", ""BRK-B"", ""CSCO"", ""MA"", ""NVDA"", ""HD"", ""WMT"", ""V"", ""DIS"", ""PEP"", ""JPM"", ""CMCSA"", ""BAC"", ""INTC"", ""KO"", ""ADBE"", ""CRM"", ""PYPL"", ""NFLX"", ""MRK"", ""XOM"", ""ABT"", ""ABBV"", ""CVX"", ""T"", ""MDT"", ""MMM"", ""ACN"", ""IBM"", ""SBUX"", ""HON"", ""TXN"", ""MO"", ""LLY"", ""NEE"", ""AMGN"", ""PM"", ""TXN"", ""LMT"", ""DHR"", ""NOW"", ""PYPL"", ""INTU"", ""UNP"", ""CAT"", ""ANTM"", ""CI"", ""UPS"", ""CRM"", ""ICE"", ""NOC"", ""LOW"", ""BMY"", ""DE"", ""ISRG"", ""ADP"", ""MMC"", ""SYK"", ""HUM"", ""CHTR"", ""BDX"", ""TMO"", ""ZTS"", ""RTX"", ""CI"", ""VRTX"", ""SO"", ""LIN"", ""D"", ""MU""], ""SELL"": [""TSLA"", ""RIVN"", ""T"", ""NVDA"", ""AAPL"", ""MSFT"", ""CSCO"", ""INTC"", ""AMZN"", ""GOOGL"", ""DIS"", ""HD"", ""WMT"", ""JPM"", ""BRK-A"", ""BRK-B"", ""CMCSA"", ""V"", ""VZ"", ""BAC"", ""PEP"", ""NFLX"", ""MDT"", ""MRK"", ""UNH"", ""ADBE"", ""CRM"", ""KO"", ""PYPL"", ""ABBV"", ""ACN"", ""MMM"", ""XOM"", ""SBUX"", ""CVX"", ""MDT"", ""LMT"", ""NEE"", ""TXN"", ""LLY"", ""TXN"", ""AMGN"", ""IBM"", ""MO"", ""PM"", ""UNP"", ""HON"", ""TMO"", ""INTU"", ""LOW"", ""CAT"", ""ADP"", ""DE"", ""ANTM"", ""CI"", ""MMC"", ""BDX"", ""UPS"", ""BMY"", ""ISRG"", ""ICE"", ""DHR"", ""LOW"", ""LIN"", ""RTX"", ""SYK"", ""MU"", ""VRTX"", ""HUM"", ""ZTS"", ""SO"", ""CI"", ""D"", ""TMO"", ""CHTR"", ""CRM"", ""MMC""] }
";
        }
    }
}
