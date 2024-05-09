using System.Text.Json;

namespace NetBms
{
    public partial class FormStart : Form
    {
        readonly string BATCH_SEPARATOR = Environment.NewLine + Environment.NewLine;

        public FormStart()
        {
            InitializeComponent();
            txtChatGptBatchResults.MaxLength = 0;


            //txtChatGptBatchResults.Text = TestData();
            //txtChatGptBatchResults.Text = TestData2();
            txtChatGptBatchResults.Text = TestDataBug_24_05_08();
        }

        private void btGo_ClickAsync(object sender, EventArgs e)
        {
            var input = txtChatGptBatchResults.Text;

            // Deserialize text as ChatGptBatchResult JSON
            var batchesAsStringArray = input
                .Split(BATCH_SEPARATOR, StringSplitOptions.RemoveEmptyEntries)
                .Where(x => !string.IsNullOrWhiteSpace(x)) 
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
                catch (JsonException)
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

        private string TestDataBug_24_05_08()
        {
            return @"{
    ""BUY"": [
      ""AAPL"", ""MSFT"", ""TSLA"", ""GOOGL"", ""AMZN"", ""NVDA"", ""INTC"", ""PFE"", ""CVX"", ""AMD"",
      ""JNJ"", ""UNH"", ""META"", ""PG"", ""V"", ""HD"", ""MA"", ""BAC"", ""DIS"", ""XOM"",
      ""KO"", ""PEP"", ""CSCO"", ""ADBE"", ""NFLX"", ""CMCSA"", ""NKE"", ""TMO"", ""ABT"", ""ACN"",
      ""CRM"", ""AVGO"", ""LLY"", ""PYPL"", ""DHR"", ""MCD"", ""WMT"", ""QCOM"", ""TXN"", ""HON"",
      ""NEE"", ""LIN"", ""BMY"", ""COST"", ""SCHW"", ""T"", ""PM"", ""UNP"", ""UPS"", ""RTX"",
      ""IBM"", ""BA"", ""NOW"", ""MDT"", ""ORCL"", ""CAT"", ""MMM"", ""BLK"", ""SBUX"", ""CVS"",
      ""DE"", ""GS"", ""AMGN"", ""MS"", ""GE"", ""COP"", ""LOW"", ""SPGI"", ""BKNG"", ""CHTR"",
      ""LMT"", ""GILD"", ""TGT"", ""AXP"", ""MO"", ""ISRG"", ""ANTM"", ""CI"", ""ZTS"", ""CB"",
      ""DUK"", ""PNC"", ""SYK"", ""SO"", ""CL"", ""USB"", ""PLD"", ""CCI"", ""ADP"", ""MDLZ"",
      ""FIS"", ""NSC"", ""VRTX"", ""BDX"", ""FDX"", ""CSX"", ""WM"", ""ITW"", ""FISV"", ""ECL""
    ],
    ""SELL"": [
      ""QCOM"", ""WFC"", ""KHC"", ""PTON"", ""UBER"", ""HOOD"", ""GPS"", ""CCL"", ""BYND"", ""F"",
      ""GM"", ""UAL"", ""AAL"", ""DAL"", ""MRO"", ""SLB"", ""HPQ"", ""GME"", ""AMC"", ""COF"",
      ""PENN"", ""LVS"", ""WYNN"", ""BKR"", ""OXY"", ""HAL"", ""MGM"", ""Z"", ""LYFT"", ""UAA"",
      ""NCLH"", ""APA"", ""DKNG"", ""ROKU"", ""VIAC"", ""SPLK"", ""CLF"", ""X"", ""AIG"", ""BEN"",
      ""HPE"", ""IVZ"", ""JWN"", ""M"", ""PVH"", ""BBY"", ""JBLU"", ""SAVE"", ""CNK"", ""NWSA"",
      ""GPS"", ""DISCA"", ""VZ"", ""TAP"", ""SJM"", ""KR"", ""CPRI"", ""FTCH"", ""HOG"", ""LUMN"",
      ""VTRS"", ""VLO"", ""MTCH"", ""MOS"", ""WDC"", ""NEM"", ""FCX"", ""ALK"", ""COTY"", ""KSS"",
      ""BBWI"", ""HBI"", ""FL"", ""APA"", ""DVN"", ""FANG"", ""OKE"", ""BXP"", ""MAC"", ""REG"",
      ""SPG"", ""PEAK"", ""KIM"", ""UDR"", ""ESS"", ""AIV"", ""AVB"", ""MRO"", ""DVN"", ""CHK"",
      ""SWN"", ""GPS"", ""BBY"", ""KMX"", ""LRN"", ""S"", ""CHKP"", ""TWTR"", ""SNAP"", ""PINS""
    ]
  }
  

  {
    ""BUY"": [
      ""AAPL"", ""MSFT"", ""GOOGL"", ""TSLA"", ""AMZN"", ""META"", ""NVDA"", ""BRK.B"", ""V"", ""JNJ"",
      ""PG"", ""UNH"", ""HD"", ""MA"", ""DIS"", ""PYPL"", ""ADBE"", ""CRM"", ""NFLX"", ""BAC"",
      ""VZ"", ""CMCSA"", ""PFE"", ""KO"", ""NKE"", ""MRK"", ""PEP"", ""T"", ""ABT"", ""ORCL"",
      ""CSCO"", ""ABBV"", ""ACN"", ""TMO"", ""AVGO"", ""XOM"", ""QCOM"", ""COST"", ""CVX"", ""LLY"",
      ""MCD"", ""WMT"", ""MDT"", ""INTC"", ""DHR"", ""HON"", ""TXN"", ""UNP"", ""LIN"", ""BMY"",
      ""SBUX"", ""NEE"", ""AMGN"", ""IBM"", ""LMT"", ""MMM"", ""BA"", ""UPS"", ""GE"", ""RTX"",
      ""CAT"", ""GS"", ""DE"", ""BLK"", ""MS"", ""NOW"", ""SCHW"", ""AMT"", ""CVS"", ""SPGI"",
      ""FIS"", ""GILD"", ""MO"", ""MDLZ"", ""BDX"", ""C"", ""GM"", ""CI"", ""DUK"", ""PNC"",
      ""SYK"", ""USB"", ""SO"", ""ISRG"", ""MU"", ""CL"", ""ADI"", ""TJX"", ""ZTS"", ""CSX"",
      ""NSC"", ""VRTX"", ""FDX"", ""CCI"", ""ITW"", ""PLD"", ""EW"", ""WM"", ""ECL"", ""REGN""
    ],
    ""SELL"": [
      ""F"", ""GM"", ""UAA"", ""GPRO"", ""SNAP"", ""TWTR"", ""M"", ""KSS"", ""JWN"", ""GPS"",
      ""HPQ"", ""X"", ""AAL"", ""UAL"", ""DAL"", ""LUV"", ""MRO"", ""DVN"", ""HFC"", ""VLO"",
      ""OXY"", ""SLB"", ""BKR"", ""HAL"", ""NEM"", ""FCX"", ""CNX"", ""BTU"", ""CCJ"", ""CLF"",
      ""WDC"", ""STX"", ""LYB"", ""CF"", ""MOS"", ""APC"", ""SWN"", ""CHK"", ""RIG"", ""FTI"",
      ""NOV"", ""HP"", ""COP"", ""BXP"", ""VNO"", ""SLG"", ""MAC"", ""HBI"", ""PVH"", ""BBY"",
      ""SJM"", ""CPB"", ""K"", ""GIS"", ""KR"", ""WBA"", ""CVS"", ""L"", ""TAP"", ""SAM"", ""STZ"",
      ""BF.B"", ""NWL"", ""HAS"", ""MAT"", ""LEG"", ""TPX"", ""MHK"", ""HRB"", ""OKE"", ""ET"",
      ""MPC"", ""PSX"", ""PBF"", ""ANDV"", ""UGI"", ""SRE"", ""ES"", ""PEG"", ""CNP"", ""FE"",
      ""DTE"", ""AEP"", ""ED"", ""PCG"", ""EIX"", ""PPL"", ""AES"", ""NI"", ""LNT"", ""ATO"",
      ""CMS"", ""XEL"", ""PNW"", ""D"", ""WEC"", ""SCG"", ""GAS"", ""SJI"", ""SR"", ""UGI""
    ]
  }
  

  {
    ""BUY"": [
      ""AAPL"", ""MSFT"", ""TSLA"", ""NVDA"", ""AMZN"", ""GOOGL"", ""META"", ""AMD"", ""NFLX"", ""ADBE"",
      ""CRM"", ""INTC"", ""PYPL"", ""CSCO"", ""AVGO"", ""TXN"", ""QCOM"", ""ORCL"", ""IBM"", ""ACN"",
      ""SAP"", ""SHOP"", ""SNOW"", ""ZM"", ""SPOT"", ""UBER"", ""LYFT"", ""SQ"", ""TWTR"", ""DOCU"",
      ""NOW"", ""DDOG"", ""PANW"", ""OKTA"", ""SPLK"", ""NET"", ""FSLY"", ""CRWD"", ""MIME"", ""ZS"",
      ""FEYE"", ""CHKP"", ""JNPR"", ""FTNT"", ""ANET"", ""VMW"", ""RNG"", ""BAND"", ""AKAM"", ""FFIV"",
      ""WORK"", ""BOX"", ""DBX"", ""MRVL"", ""AMD"", ""MU"", ""INTU"", ""CDNS"", ""KLAC"", ""LRCX"",
      ""VRSN"", ""SSNC"", ""NTAP"", ""WDAY"", ""TTD"", ""TEAM"", ""ADSK"", ""CTXS"", ""MCHP"", ""WDC"",
      ""HPE"", ""HPQ"", ""DELL"", ""STX"", ""CREE"", ""ANSS"", ""MXIM"", ""ADI"", ""XLNX"", ""SWKS"",
      ""V"", ""MA"", ""AXP"", ""DFS"", ""COF"", ""SYF"", ""PAYC"", ""FIS"", ""GPN"", ""FISV"",
      ""PYPL"", ""SQ"", ""ETSY"", ""SHOP"", ""ROKU"", ""NFLX"", ""DIS"", ""ATVI"", ""EA"", ""TTWO""
    ],
    ""SELL"": [
      ""AAL"", ""UAL"", ""DAL"", ""LUV"", ""ALK"", ""JBLU"", ""SAVE"", ""HA"", ""RYAAY"", ""AZUL"",
      ""EADSY"", ""BA"", ""RTX"", ""NOC"", ""GD"", ""LMT"", ""TXT"", ""HII"", ""COL"", ""TDG"",
      ""GE"", ""HON"", ""MMM"", ""DHR"", ""OTIS"", ""PCAR"", ""CAT"", ""DE"", ""CMI"", ""AGCO"",
      ""NAV"", ""OSK"", ""MTW"", ""TEX"", ""PH"", ""ROK"", ""DOV"", ""ITW"", ""SWK"", ""SNA"",
      ""TT"", ""PNR"", ""XYL"", ""IEX"", ""FLS"", ""GWW"", ""FAST"", ""MSM"", ""WCC"", ""AXE"",
      ""SITE"", ""HD"", ""LOW"", ""LL"", ""FND"", ""TSCO"", ""GME"", ""BBY"", ""AMZN"", ""WMT"",
      ""COST"", ""BJ"", ""KR"", ""WBA"", ""CVS"", ""DG"", ""DLTR"", ""BIG"", ""ODP"", ""AAP"",
      ""AZO"", ""ORLY"", ""LKQ"", ""GPC"", ""M"", ""JWN"", ""KSS"", ""JCP"", ""DDS"", ""BURL"",
      ""TJX"", ""ROST"", ""GPS"", ""LB"", ""ANF"", ""URBN"", ""AEO"", ""GPS"", ""CRI"", ""PLCE"",
      ""MRO"", ""OXY"", ""VLO"", ""MPC"", ""PSX"", ""HFC"", ""TSO"", ""ANDV"", ""CVX"", ""XOM""
    ]
  }
  

  {
    ""BUY"": [
      ""MSFT"", ""AAPL"", ""GOOGL"", ""AMZN"", ""META"", ""TSLA"", ""NVDA"", ""BRK.B"", ""V"", ""JNJ"",
      ""PG"", ""UNH"", ""MA"", ""HD"", ""DIS"", ""PYPL"", ""ADBE"", ""CRM"", ""NFLX"", ""BAC"",
      ""PFE"", ""VZ"", ""CMCSA"", ""KO"", ""NKE"", ""MRK"", ""PEP"", ""T"", ""ABT"", ""CSCO"",
      ""ABBV"", ""ACN"", ""TMO"", ""AVGO"", ""XOM"", ""QCOM"", ""COST"", ""CVX"", ""LLY"", ""MCD"",
      ""WMT"", ""MDT"", ""INTC"", ""DHR"", ""HON"", ""TXN"", ""LIN"", ""BMY"", ""SBUX"", ""UNP"",
      ""UPS"", ""IBM"", ""RTX"", ""BA"", ""MMM"", ""GE"", ""GS"", ""CAT"", ""DE"", ""BLK"", ""MS"",
      ""NOW"", ""SCHW"", ""AMT"", ""CVS"", ""SPGI"", ""FIS"", ""GILD"", ""MO"", ""MDLZ"", ""BDX"",
      ""C"", ""GM"", ""CI"", ""DUK"", ""PNC"", ""SYK"", ""USB"", ""SO"", ""ISRG"", ""MU"", ""CL"",
      ""ADI"", ""TJX"", ""ZTS"", ""CSX"", ""NSC"", ""VRTX"", ""FDX"", ""CCI"", ""ITW"", ""PLD"",
      ""EW"", ""WM"", ""ECL"", ""REGN"", ""EQIX"", ""ORLY"", ""RMD"", ""ILMN"", ""BIIB"", ""DLR""
    ],
    ""SELL"": [
      ""F"", ""GM"", ""UAA"", ""AA"", ""X"", ""NCLH"", ""CCL"", ""RCL"", ""DAL"", ""UAL"",
      ""AAL"", ""LUV"", ""JBLU"", ""SAVE"", ""ALK"", ""HA"", ""M"", ""KSS"", ""GPS"", ""JWN"",
      ""BBY"", ""HBI"", ""LB"", ""COTY"", ""CPRI"", ""GME"", ""MRO"", ""SLB"", ""HAL"", ""BKR"",
      ""HP"", ""VLO"", ""MPC"", ""PSX"", ""OXY"", ""COG"", ""CXO"", ""DVN"", ""FANG"", ""HES"",
      ""NFX"", ""APA"", ""APC"", ""CHK"", ""SWN"", ""RRC"", ""ET"", ""WMB"", ""KMI"", ""OKE"",
      ""AM"", ""AR"", ""COG"", ""CRK"", ""DNR"", ""GPOR"", ""LPI"", ""MTDR"", ""NBL"", ""OAS"",
      ""PDCE"", ""PE"", ""PXD"", ""QEP"", ""REI"", ""REN"", ""SM"", ""SN"", ""VNOM"", ""WPX"",
      ""WLL"", ""XEC"", ""CLR"", ""CPE"", ""EQT"", ""ESTE"", ""GPOR"", ""HPR"", ""LLEX"", ""LONE"",
      ""MNRL"", ""NOG"", ""OII"", ""PARR"", ""PTEN"", ""PUMP"", ""PVAC"", ""RIG"", ""ROSE"", ""SBOW"",
      ""SJT"", ""SM"", ""SNDE"", ""TALO"", ""TISI"", ""TPLM"", ""UNT"", ""USEG"", ""WTI"", ""YUMA""
    ]
  }
  

  {
    ""BUY"": [
      ""MSFT"", ""AAPL"", ""GOOGL"", ""AMZN"", ""TSLA"", ""META"", ""NVDA"", ""BRK.B"", ""V"", ""JNJ"",
      ""PG"", ""UNH"", ""MA"", ""HD"", ""DIS"", ""PYPL"", ""ADBE"", ""CRM"", ""NFLX"", ""BAC"",
      ""PFE"", ""VZ"", ""CMCSA"", ""KO"", ""NKE"", ""MRK"", ""PEP"", ""T"", ""ABT"", ""CSCO"",
      ""ABBV"", ""ACN"", ""TMO"", ""AVGO"", ""XOM"", ""QCOM"", ""COST"", ""CVX"", ""LLY"", ""MCD"",
      ""WMT"", ""MDT"", ""INTC"", ""DHR"", ""HON"", ""TXN"", ""LIN"", ""BMY"", ""SBUX"", ""UNP"",
      ""UPS"", ""IBM"", ""RTX"", ""BA"", ""MMM"", ""GE"", ""GS"", ""CAT"", ""DE"", ""BLK"", ""MS"",
      ""NOW"", ""SCHW"", ""AMT"", ""CVS"", ""SPGI"", ""FIS"", ""GILD"", ""MO"", ""MDLZ"", ""BDX"",
      ""C"", ""GM"", ""CI"", ""DUK"", ""PNC"", ""SYK"", ""USB"", ""SO"", ""ISRG"", ""MU"", ""CL"",
      ""ADI"", ""TJX"", ""ZTS"", ""CSX"", ""NSC"", ""VRTX"", ""FDX"", ""CCI"", ""ITW"", ""PLD"",
      ""EW"", ""WM"", ""ECL"", ""REGN"", ""EQIX"", ""ORLY"", ""RMD"", ""ILMN"", ""BIIB"", ""DLR""
    ],
    ""SELL"": [
      ""F"", ""GM"", ""UAA"", ""AA"", ""X"", ""NCLH"", ""CCL"", ""RCL"", ""DAL"", ""UAL"",
      ""AAL"", ""LUV"", ""JBLU"", ""SAVE"", ""ALK"", ""HA"", ""M"", ""KSS"", ""GPS"", ""JWN"",
      ""BBY"", ""HBI"", ""LB"", ""COTY"", ""CPRI"", ""GME"", ""MRO"", ""SLB"", ""HAL"", ""BKR"",
      ""HP"", ""VLO"", ""MPC"", ""PSX"", ""OXY"", ""COG"", ""CXO"", ""DVN"", ""FANG"", ""HES"",
      ""NFX"", ""APA"", ""APC"", ""CHK"", ""SWN"", ""RRC"", ""ET"", ""WMB"", ""KMI"", ""OKE"",
      ""AM"", ""AR"", ""COG"", ""CRK"", ""DNR"", ""GPOR"", ""LPI"", ""MTDR"", ""NBL"", ""OAS"",
      ""PDCE"", ""PE"", ""PXD"", ""QEP"", ""REI"", ""REN"", ""SM"", ""SN"", ""VNOM"", ""WPX"",
      ""WLL"", ""XEC"", ""CLR"", ""CPE"", ""EQT"", ""ESTE"", ""GPOR"", ""HPR"", ""LLEX"", ""LONE"",
      ""MNRL"", ""NOG"", ""OII"", ""PARR"", ""PTEN"", ""PUMP"", ""PVAC"", ""RIG"", ""ROSE"", ""SBOW"",
      ""SJT"", ""SM"", ""SNDE"", ""TALO"", ""TISI"", ""TPLM"", ""UNT"", ""USEG"", ""WTI"", ""YUMA""
    ]
  }
  

  {
    ""BUY"": [
      ""MSFT"", ""AAPL"", ""GOOGL"", ""AMZN"", ""TSLA"", ""META"", ""NVDA"", ""BRK.B"", ""V"", ""JNJ"",
      ""UNH"", ""PG"", ""MA"", ""HD"", ""DIS"", ""PYPL"", ""ADBE"", ""CRM"", ""NFLX"", ""BAC"",
      ""PFE"", ""VZ"", ""CMCSA"", ""KO"", ""NKE"", ""MRK"", ""PEP"", ""T"", ""ABT"", ""CSCO"",
      ""ABBV"", ""ACN"", ""TMO"", ""AVGO"", ""XOM"", ""QCOM"", ""COST"", ""CVX"", ""LLY"", ""MCD"",
      ""WMT"", ""MDT"", ""INTC"", ""DHR"", ""HON"", ""TXN"", ""LIN"", ""BMY"", ""SBUX"", ""UNP"",
      ""UPS"", ""IBM"", ""RTX"", ""BA"", ""MMM"", ""GE"", ""GS"", ""CAT"", ""DE"", ""BLK"", ""MS"",
      ""NOW"", ""SCHW"", ""AMT"", ""CVS"", ""SPGI"", ""FIS"", ""GILD"", ""MO"", ""MDLZ"", ""BDX"",
      ""C"", ""GM"", ""CI"", ""DUK"", ""PNC"", ""SYK"", ""USB"", ""SO"", ""ISRG"", ""MU"", ""CL"",
      ""ADI"", ""TJX"", ""ZTS"", ""CSX"", ""NSC"", ""VRTX"", ""FDX"", ""CCI"", ""ITW"", ""PLD"",
      ""EW"", ""WM"", ""ECL"", ""REGN"", ""EQIX"", ""ORLY"", ""RMD"", ""ILMN"", ""BIIB"", ""DLR""
    ],
    ""SELL"": [
      ""F"", ""GME"", ""UAA"", ""X"", ""NCLH"", ""CCL"", ""RCL"", ""DAL"", ""UAL"", ""AAL"",
      ""LUV"", ""JBLU"", ""SAVE"", ""ALK"", ""HA"", ""M"", ""KSS"", ""GPS"", ""JWN"", ""BBY"",
      ""HBI"", ""LB"", ""COTY"", ""CPRI"", ""MRO"", ""SLB"", ""HAL"", ""BKR"", ""HP"", ""VLO"",
      ""MPC"", ""PSX"", ""OXY"", ""COG"", ""CXO"", ""DVN"", ""FANG"", ""HES"", ""NFX"", ""APA"",
      ""APC"", ""CHK"", ""SWN"", ""RRC"", ""ET"", ""WMB"", ""KMI"", ""OKE"", ""AM"", ""AR"",
      ""COG"", ""CRK"", ""DNR"", ""GPOR"", ""LPI"", ""MTDR"", ""NBL"", ""OAS"", ""PDCE"", ""PE"",
      ""PXD"", ""QEP"", ""REI"", ""REN"", ""SM"", ""SN"", ""VNOM"", ""WPX"", ""WLL"", ""XEC"",
      ""CLR"", ""CPE"", ""EQT"", ""ESTE"", ""GPOR"", ""HPR"", ""LLEX"", ""LONE"", ""MNRL"", ""NOG"",
      ""OII"", ""PARR"", ""PTEN"", ""PUMP"", ""PVAC"", ""RIG"", ""ROSE"", ""SBOW"", ""SJT"", ""SM"",
      ""SNDE"", ""TALO"", ""TISI"", ""TPLM"", ""UNT"", ""USEG"", ""WTI"", ""YUMA""
    ]
  }
  

  {
    ""BUY"": [
      ""AAPL"", ""MSFT"", ""GOOGL"", ""AMZN"", ""TSLA"", ""META"", ""NVDA"", ""V"", ""JNJ"", ""WMT"",
      ""PG"", ""MA"", ""DIS"", ""PYPL"", ""ADBE"", ""NFLX"", ""CRM"", ""INTC"", ""TMO"", ""ABT"",
      ""XOM"", ""CSCO"", ""ORCL"", ""ACN"", ""QCOM"", ""LLY"", ""UNH"", ""MCD"", ""BMY"", ""SCHW"",
      ""AVGO"", ""COST"", ""TXN"", ""SBUX"", ""HON"", ""LIN"", ""AMD"", ""FIS"", ""NOW"", ""LRCX"",
      ""MDT"", ""BKNG"", ""DHR"", ""PNC"", ""RTX"", ""SPGI"", ""MMC"", ""CMI"", ""ZTS"", ""NOC"",
      ""CVS"", ""GM"", ""CAT"", ""CB"", ""TGT"", ""DE"", ""ADI"", ""SYK"", ""COP"", ""FDX"", ""ICE"",
      ""NSC"", ""SO"", ""ITW"", ""BDX"", ""ETN"", ""VRTX"", ""ECL"", ""WM"", ""EL"", ""CSX"", ""FRC"",
      ""ROP"", ""EW"", ""EMR"", ""MET"", ""PPG"", ""ISRG"", ""AON"", ""MCK"", ""DG"", ""HUM"", ""ADP"",
      ""CDNS"", ""GD"", ""REGN"", ""SYY"", ""DTE"", ""KLAC"", ""TEL"", ""FISV"", ""PSA"", ""A"",
      ""IQV"", ""HSY"", ""APH"", ""EXC"", ""WBA"", ""CTAS"", ""TT"", ""MTD"", ""AMP"", ""RSG""
    ],
    ""SELL"": [
      ""F"", ""BAC"", ""GE"", ""C"", ""WFC"", ""DAL"", ""UAL"", ""HPQ"", ""GME"", ""SLB"",
      ""HAL"", ""MRO"", ""OXY"", ""VLO"", ""MPC"", ""HFC"", ""KMI"", ""DVN"", ""APA"", ""NEM"",
      ""BKR"", ""CCL"", ""NCLH"", ""RCL"", ""AAL"", ""JBLU"", ""LUV"", ""SAVE"", ""ALK"", ""ZM"",
      ""UBER"", ""LYFT"", ""EXPE"", ""TRIP"", ""MAR"", ""HLT"", ""PENN"", ""FTR"", ""GPS"", ""M"",
      ""KSS"", ""JWN"", ""BBY"", ""LB"", ""AEO"", ""URBN"", ""HBI"", ""PVH"", ""VFC"", ""TJX"",
      ""ROST"", ""COTY"", ""CPRI"", ""FTCH"", ""XRX"", ""CLF"", ""AA"", ""FCX"", ""NUE"", ""STLD"",
      ""MOS"", ""CF"", ""CC"", ""PPL"", ""CNX"", ""BTU"", ""CHK"", ""SWN"", ""RIG"", ""PTEN"",
      ""HP"", ""NE"", ""ESV"", ""DO"", ""ATW"", ""SDRL"", ""FTI"", ""NOV"", ""OII"", ""WHD"",
      ""BAS"", ""RES"", ""PXD"", ""COG"", ""SM"", ""REI"", ""CLR"", ""FANG"", ""OVV"", ""AR"",
      ""RRC"", ""EQNR"", ""MTDR"", ""EQT"", ""RRC"", ""CRK"", ""WPX"", ""CNQ"", ""PBA"", ""TOT""
    ]
  }
  

  {
    ""BUY"": [
      ""AAPL"", ""MSFT"", ""GOOGL"", ""AMZN"", ""META"", ""TSLA"", ""BRK.B"", ""JNJ"", ""V"", ""PG"",
      ""UNH"", ""MA"", ""NVDA"", ""HD"", ""DIS"", ""PYPL"", ""ADBE"", ""NFLX"", ""CRM"", ""ABT"",
      ""TMO"", ""ACN"", ""XOM"", ""MCD"", ""LLY"", ""BMY"", ""DHR"", ""CSCO"", ""PFE"", ""NKE"",
      ""MDT"", ""INTC"", ""TXN"", ""GS"", ""HON"", ""MRK"", ""BA"", ""UNP"", ""ORCL"", ""AMGN"",
      ""BLK"", ""COP"", ""RTX"", ""LIN"", ""CVS"", ""MMM"", ""TGT"", ""DE"", ""SCHW"", ""COST"",
      ""NOW"", ""SPGI"", ""CAT"", ""IBM"", ""LMT"", ""FIS"", ""ANTM"", ""SYK"", ""ADP"", ""MDLZ"",
      ""CI"", ""CB"", ""MO"", ""ISRG"", ""UPS"", ""BDX"", ""SYY"", ""MMC"", ""AXP"", ""TJX"",
      ""ZTS"", ""CMI"", ""WM"", ""NSC"", ""ECL"", ""ITW"", ""EW"", ""FRC"", ""ROP"", ""APH"",
      ""IQV"", ""PGR"", ""VRTX"", ""SHW"", ""ETN"", ""FCX"", ""REGN"", ""EQIX"", ""TT"", ""A"",
      ""MET"", ""DTE"", ""PPG"", ""ICE"", ""ILMN"", ""MU"", ""TEL"", ""FDX"", ""AME"", ""CDNS""
    ],
    ""SELL"": [
      ""F"", ""GM"", ""GE"", ""WFC"", ""DAL"", ""UAA"", ""SLB"", ""HAL"", ""KHC"", ""HPQ"",
      ""BKR"", ""CCL"", ""RCL"", ""OXY"", ""MRO"", ""NCLH"", ""GPS"", ""M"", ""XOM"", ""CVX"",
      ""COG"", ""VLO"", ""MPC"", ""HFC"", ""NOV"", ""DVN"", ""FTI"", ""APA"", ""HES"", ""BHI"",
      ""KMI"", ""ET"", ""CHK"", ""SWN"", ""PXD"", ""CLR"", ""EQT"", ""OKE"", ""WMB"", ""CXO"",
      ""PE"", ""NFX"", ""COP"", ""HOG"", ""LUV"", ""AAL"", ""UAL"", ""ALK"", ""JBLU"", ""SAVE"",
      ""MAR"", ""HLT"", ""WYNN"", ""LVS"", ""F"", ""BBY"", ""TAP"", ""STZ"", ""BF.B"", ""EL"",
      ""CPRI"", ""NWL"", ""RL"", ""GPS"", ""GME"", ""NEM"", ""AEM"", ""FCX"", ""AA"", ""ARNC"",
      ""X"", ""NUE"", ""CLF"", ""STLD"", ""AKS"", ""HBI"", ""UA"", ""LB"", ""PVH"", ""VFC"",
      ""TPR"", ""COTY"", ""JWN"", ""DDS"", ""SKX"", ""KSS"", ""SHLD"", ""JCP"", ""BIG"", ""ANF"",
      ""GPS"", ""FL"", ""DLTR"", ""KR"", ""SPLS"", ""ODP"", ""BBBY"", ""WMT"", ""TGT"", ""DG""
    ]
  }
  
  
  {
    ""BUY"": [
      ""AAPL"", ""MSFT"", ""AMZN"", ""GOOGL"", ""META"", ""TSLA"", ""NVDA"", ""BRKB"", ""JPM"", ""V"",
      ""JNJ"", ""UNH"", ""PG"", ""HD"", ""DIS"", ""MA"", ""PYPL"", ""ADBE"", ""CRM"", ""NFLX"",
      ""BAC"", ""PFE"", ""CSCO"", ""VZ"", ""KO"", ""CMCSA"", ""PEP"", ""ORCL"", ""ABT"", ""ACN"",
      ""TMO"", ""AVGO"", ""TXN"", ""NKE"", ""LLY"", ""QCOM"", ""COST"", ""SCHW"", ""MDT"", ""HON"",
      ""LIN"", ""DHR"", ""RTX"", ""SBUX"", ""MMM"", ""INTC"", ""AMT"", ""GS"", ""CAT"", ""BA"",
      ""IBM"", ""CVS"", ""BLK"", ""F"", ""COP"", ""DUK"", ""GM"", ""SO"", ""CCI"", ""USB"", ""T"",
      ""GILD"", ""D"", ""CSX"", ""NSC"", ""SPGI"", ""PLD"", ""PNC"", ""EW"", ""SYK"", ""ADI"",
      ""NOW"", ""BDX"", ""ISRG"", ""VRTX"", ""REGN"", ""MO"", ""FDX"", ""MMC"", ""MCK"", ""CI"",
      ""PXD"", ""CB"", ""EXC"", ""WM"", ""ECL"", ""BIIB"", ""ZTS"", ""FIS"", ""FISV"", ""ITW"",
      ""TEL"", ""AEP"", ""IQV"", ""CTSH"", ""ROP"", ""LRCX"", ""GLW"", ""MCO"", ""APD"", ""MET""
    ],
    ""SELL"": [
      ""F"", ""XOM"", ""SLB"", ""HAL"", ""OXY"", ""MRO"", ""DVN"", ""CCL"", ""NCLH"", ""UAL"",
      ""AAL"", ""LUV"", ""GPS"", ""M"", ""BBY"", ""KSS"", ""JWN"", ""LB"", ""IVZ"", ""BKR"",
      ""FTI"", ""NOV"", ""HPQ"", ""DVN"", ""BAC"", ""WFC"", ""C"", ""SPG"", ""COF"", ""PFG"",
      ""PRU"", ""MET"", ""LNC"", ""AIG"", ""ALL"", ""HIG"", ""TRV"", ""PGR"", ""CF"", ""MOS"",
      ""NUE"", ""CLF"", ""AKS"", ""X"", ""STLD"", ""RS"", ""MT"", ""PKX"", ""SCHN"", ""WOR"",
      ""ZEUS"", ""STL"", ""SF"", ""KEY"", ""CMA"", ""WBS"", ""FHN"", ""BOKF"", ""PB"", ""SNV"",
      ""VLY"", ""SIVB"", ""PACW"", ""TCBI"", ""HBAN"", ""FITB"", ""MTB"", ""CFG"", ""RF"", ""DAL"",
      ""JBLU"", ""SAVE"", ""ALK"", ""HA"", ""AAL"", ""OXY"", ""VLO"", ""MPC"", ""PSX"", ""HFC"",
      ""ANDV"", ""TSO"", ""PBF"", ""CVRR"", ""DK"", ""UGP"", ""SUN"", ""INT"", ""CLMT"", ""HES"",
      ""COP"", ""CVX"", ""XEC"", ""CLR"", ""FANG"", ""EOG"", ""APA"", ""MUR"", ""NFX"", ""SWN""
    ]
  }
  

  {
    ""BUY"": [
      ""AAPL"", ""MSFT"", ""GOOGL"", ""AMZN"", ""TSLA"", ""META"", ""NVDA"", ""V"", ""JNJ"", ""PG"",
      ""UNH"", ""MA"", ""HD"", ""DIS"", ""PYPL"", ""ADBE"", ""CRM"", ""NFLX"", ""BAC"", ""PFE"",
      ""VZ"", ""CMCSA"", ""KO"", ""NKE"", ""MRK"", ""PEP"", ""T"", ""ABT"", ""CSCO"", ""ABBV"",
      ""ACN"", ""TMO"", ""AVGO"", ""XOM"", ""QCOM"", ""COST"", ""CVX"", ""LLY"", ""MCD"", ""WMT"",
      ""MDT"", ""INTC"", ""DHR"", ""HON"", ""TXN"", ""LIN"", ""BMY"", ""SBUX"", ""UNP"", ""UPS"",
      ""IBM"", ""RTX"", ""BA"", ""MMM"", ""GE"", ""GS"", ""CAT"", ""DE"", ""BLK"", ""MS"", ""NOW"",
      ""SCHW"", ""AMT"", ""CVS"", ""SPGI"", ""FIS"", ""GILD"", ""MO"", ""MDLZ"", ""BDX"", ""C"",
      ""GM"", ""CI"", ""DUK"", ""PNC"", ""SYK"", ""USB"", ""SO"", ""ISRG"", ""MU"", ""CL"",
      ""ADI"", ""TJX"", ""ZTS"", ""CSX"", ""NSC"", ""VRTX"", ""FDX"", ""CCI"", ""ITW"", ""PLD"",
      ""EW"", ""WM"", ""ECL"", ""REGN"", ""EQIX"", ""ORLY"", ""RMD"", ""ILMN"", ""BIIB"", ""DLR""
    ],
    ""SELL"": [
      ""F"", ""GME"", ""UAA"", ""GPS"", ""M"", ""KSS"", ""JWN"", ""BBY"", ""HBI"", ""CPRI"",
      ""LB"", ""X"", ""SLB"", ""HAL"", ""BKR"", ""HP"", ""NBL"", ""MRO"", ""OXY"", ""DVN"",
      ""VLO"", ""MPC"", ""PSX"", ""HFC"", ""COP"", ""CHK"", ""SWN"", ""RRC"", ""COG"", ""CNX"",
      ""AR"", ""GPOR"", ""LPI"", ""SM"", ""REI"", ""CLR"", ""CPE"", ""HES"", ""EQT"", ""OVV"",
      ""WLL"", ""PDCE"", ""MTDR"", ""FANG"", ""WHD"", ""NOV"", ""FTI"", ""RIG"", ""DO"", ""NE"",
      ""SPN"", ""ESV"", ""WTI"", ""SN"", ""ROSE"", ""RES"", ""PKD"", ""PTEN"", ""PDS"", ""HP"",
      ""NBR"", ""UNT"", ""BAS"", ""HLX"", ""FI"", ""WTTR"", ""KEG"", ""USAC"", ""SD"", ""SND"",
      ""PUMP"", ""NINE"", ""AMRC"", ""CKH"", ""TDW"", ""USWS"", ""OIS"", ""NR"", ""NGS"", ""TPLM"",
      ""LONE"", ""PARR"", ""PVAC"", ""CRZO"", ""QEP"", ""SMPL"", ""NGVC"", ""SEB"", ""SFM"", ""KR""
    ]
  }
  

  {
    ""BUY"": [
      ""AAPL"", ""MSFT"", ""GOOGL"", ""AMZN"", ""META"", ""NVDA"", ""BRKB"", ""V"", ""JPM"", ""JNJ"",
      ""UNH"", ""PG"", ""HD"", ""MA"", ""DIS"", ""PYPL"", ""ADBE"", ""CRM"", ""NFLX"", ""INTC"",
      ""TMO"", ""ABT"", ""XOM"", ""CSCO"", ""ORCL"", ""ACN"", ""QCOM"", ""COST"", ""AVGO"", ""TXN"",
      ""LIN"", ""LLY"", ""MCD"", ""DHR"", ""HON"", ""UPS"", ""UNP"", ""MMM"", ""IBM"", ""NEE"",
      ""CVX"", ""BMY"", ""MDT"", ""GE"", ""SCHW"", ""F"", ""COP"", ""RTX"", ""PFE"", ""BKNG"",
      ""AMT"", ""GS"", ""CAT"", ""BA"", ""AMGN"", ""DE"", ""WM"", ""BLK"", ""SPGI"", ""CVS"",
      ""MO"", ""GILD"", ""SBUX"", ""MDLZ"", ""LMT"", ""NSC"", ""SO"", ""ISRG"", ""EW"", ""ANTM"",
      ""DXCM"", ""CMI"", ""SYK"", ""BDX"", ""ITW"", ""ECL"", ""SYY"", ""TGT"", ""GD"", ""ZTS"",
      ""PGR"", ""CI"", ""AEP"", ""TEL"", ""CSX"", ""IQV"", ""CCI"", ""ROP"", ""ADP"", ""MMC"",
      ""TT"", ""APH"", ""RMD"", ""FIS"", ""DTE"", ""FISV"", ""MET"", ""REGN"", ""ILMN"", ""MCO""
    ],
    ""SELL"": [
      ""F"", ""GME"", ""M"", ""KSS"", ""JWN"", ""GPS"", ""X"", ""UAL"", ""AAL"", ""DAL"",
      ""LUV"", ""SAVE"", ""NCLH"", ""CCL"", ""RCL"", ""HLT"", ""MAR"", ""EXPE"", ""WYNN"", ""MGM"",
      ""FTR"", ""BBY"", ""HPQ"", ""GAP"", ""DVN"", ""BKR"", ""HAL"", ""SLB"", ""NEM"", ""FCX"",
      ""AA"", ""MRO"", ""OXY"", ""VLO"", ""MPC"", ""PSX"", ""HFC"", ""BAC"", ""C"", ""WFC"", ""GS"",
      ""PNC"", ""COF"", ""DFS"", ""SIVB"", ""MTB"", ""FITB"", ""HBAN"", ""RF"", ""KEY"", ""CFG"",
      ""CMA"", ""USB"", ""ZION"", ""STT"", ""PBCT"", ""PACW"", ""FHN"", ""SNV"", ""WBS"", ""CIT"",
      ""ETFC"", ""IVZ"", ""BEN"", ""AMP"", ""LM"", ""TROW"", ""AB"", ""ARE"", ""VTR"", ""HST"",
      ""SPG"", ""AMH"", ""ESS"", ""AIV"", ""MAA"", ""ELS"", ""PLD"", ""PEAK"", ""CPT"", ""UDR"",
      ""DLR"", ""O"", ""NNN"", ""PSA"", ""AVB"", ""EQIX"", ""WELL"", ""HCN"", ""BXP"", ""VNO"",
      ""KIM"", ""REG"", ""FRT"", ""MAC"", ""JEC"", ""FLS"", ""JBL"", ""HBI"", ""UA"", ""LW""
    ]
  }
  

  {
    ""BUY"": [
      ""AAPL"", ""MSFT"", ""GOOGL"", ""AMZN"", ""META"", ""NVDA"", ""TSLA"", ""V"", ""JNJ"", ""PG"",
      ""MA"", ""UNH"", ""HD"", ""VZ"", ""ADBE"", ""DIS"", ""NFLX"", ""PYPL"", ""INTC"", ""CMCSA"",
      ""PFE"", ""CSCO"", ""ORCL"", ""TMO"", ""ABT"", ""ACN"", ""TXN"", ""AVGO"", ""QCOM"", ""COST"",
      ""MDT"", ""NKE"", ""LLY"", ""FIS"", ""BMY"", ""AMGN"", ""IBM"", ""GE"", ""DHR"", ""RTX"",
      ""CVX"", ""XOM"", ""HON"", ""UPS"", ""MMM"", ""CAT"", ""BA"", ""SBUX"", ""GS"", ""CVS"",
      ""LMT"", ""WMT"", ""BLK"", ""COP"", ""DUK"", ""SO"", ""BAC"", ""MO"", ""GILD"", ""CCI"",
      ""SPGI"", ""MMC"", ""AMT"", ""GM"", ""CMI"", ""USB"", ""ADI"", ""SYK"", ""ISRG"", ""PNC"",
      ""EW"", ""BDX"", ""SYY"", ""CB"", ""DE"", ""ZTS"", ""CI"", ""NSC"", ""CSX"", ""EMR"", ""ITW"",
      ""ETN"", ""REGN"", ""FDX"", ""PCAR"", ""ECL"", ""WM"", ""DXCM"", ""ROP"", ""TEL"", ""EXC"",
      ""CTAS"", ""RMD"", ""IQV"", ""HUM"", ""A"", ""BAX"", ""TT"", ""SCHW"", ""MCK"", ""CDNS""
    ],
    ""SELL"": [
      ""F"", ""UAL"", ""AAL"", ""DAL"", ""LUV"", ""JBLU"", ""ALK"", ""SAVE"", ""HA"", ""MGM"",
      ""CCL"", ""NCLH"", ""RCL"", ""MAR"", ""HLT"", ""EXPE"", ""WYNN"", ""GPS"", ""M"", ""JWN"",
      ""KSS"", ""BBY"", ""COF"", ""DFS"", ""SIVB"", ""FITB"", ""MTB"", ""KEY"", ""HBAN"", ""CMA"",
      ""STT"", ""CFG"", ""RF"", ""PFG"", ""NTRS"", ""BKR"", ""SLB"", ""HAL"", ""HP"", ""DVN"",
      ""HES"", ""MRO"", ""OXY"", ""VLO"", ""MPC"", ""PSX"", ""HFC"", ""FANG"", ""CLR"", ""COP"",
      ""PE"", ""WMB"", ""ET"", ""KMI"", ""OKE"", ""SRE"", ""NI"", ""EIX"", ""FE"", ""ES"",
      ""PPL"", ""AWK"", ""CNP"", ""ATO"", ""CMS"", ""XEL"", ""PNW"", ""DTE"", ""AEE"", ""LNT"",
      ""ED"", ""WEC"", ""D"", ""EVRG"", ""MGEE"", ""IDA"", ""GNE"", ""POR"", ""SJI"", ""SWX"",
      ""SR"", ""UGI"", ""NFG"", ""OTTR"", ""AVA"", ""BKH"", ""PNM"", ""HE"", ""ALE"", ""PNW"",
      ""OGS"", ""VST"", ""NRG"", ""AES"", ""CPLG"", ""PEG"", ""SCG"", ""DYN"", ""PCG"", ""SRE""
    ]
  }
  

  {
    ""BUY"": [
      ""AAPL"", ""MSFT"", ""AMZN"", ""GOOGL"", ""META"", ""NVDA"", ""TSLA"", ""BRKB"", ""V"", ""JNJ"",
      ""UNH"", ""PG"", ""MA"", ""VZ"", ""HD"", ""DIS"", ""PYPL"", ""ADBE"", ""NFLX"", ""CRM"",
      ""INTC"", ""TMO"", ""ABT"", ""ACN"", ""XOM"", ""CSCO"", ""LLY"", ""MCD"", ""DHR"", ""LIN"",
      ""MRK"", ""TXN"", ""HON"", ""AVGO"", ""COST"", ""QCOM"", ""UPS"", ""UNP"", ""MDT"", ""PFE"",
      ""LOW"", ""AMGN"", ""IBM"", ""BA"", ""MMM"", ""SBUX"", ""RTX"", ""CVX"", ""GS"", ""NEE"",
      ""BLK"", ""BMY"", ""C"", ""SPGI"", ""MDLZ"", ""GILD"", ""SYK"", ""ISRG"", ""F"", ""NOW"",
      ""ZTS"", ""CAT"", ""SCHW"", ""BDX"", ""CVS"", ""MO"", ""GM"", ""CB"", ""CI"", ""NSC"",
      ""SO"", ""CCI"", ""ADP"", ""CSX"", ""BAX"", ""ECL"", ""ITW"", ""WM"", ""MET"", ""PRU"",
      ""ALL"", ""PGR"", ""STZ"", ""MCK"", ""VRTX"", ""REGN"", ""LHX"", ""CDNS"", ""FDX"", ""MMC"",
      ""EW"", ""SYY"", ""EXC"", ""DXCM"", ""EQIX"", ""A"", ""TEL"", ""CTAS"", ""ICE"", ""ROP""
    ],
    ""SELL"": [
      ""F"", ""GME"", ""UAA"", ""X"", ""SLB"", ""HAL"", ""MRO"", ""DVN"", ""COP"", ""OXY"",
      ""VLO"", ""MPC"", ""PSX"", ""HFC"", ""NCLH"", ""RCL"", ""CCL"", ""UAL"", ""AAL"", ""DAL"",
      ""LUV"", ""KHC"", ""KR"", ""GPS"", ""M"", ""JWN"", ""BBY"", ""LB"", ""CPRI"", ""PVH"",
      ""HBI"", ""TJX"", ""ROST"", ""BURL"", ""MAR"", ""HLT"", ""WYNN"", ""FTR"", ""DISCA"", ""VIAC"",
      ""AMC"", ""CNK"", ""DLTR"", ""TGT"", ""SPLK"", ""ORCL"", ""EBAY"", ""HPQ"", ""WDC"", ""INTU"",
      ""ADSK"", ""ANF"", ""GPS"", ""COF"", ""DFS"", ""NTRS"", ""STT"", ""SIVB"", ""KEY"", ""HBAN"",
      ""FITB"", ""CFG"", ""RF"", ""CMA"", ""USB"", ""WFC"", ""PNC"", ""BAC"", ""BK"", ""SBNY"",
      ""PBCT"", ""MTB"", ""WAL"", ""PACW"", ""ZION"", ""FHN"", ""SNV"", ""OFG"", ""TCBI"", ""UMBF"",
      ""IBKC"", ""ASB"", ""CIT"", ""FFIN"", ""BOKF"", ""WTFC"", ""FCNCA"", ""HOMB"", ""HWC"", ""TCF""
    ]
  }
  

  {
    ""BUY"": [
      ""AAPL"", ""MSFT"", ""GOOGL"", ""AMZN"", ""META"", ""TSLA"", ""NVDA"", ""BRK.B"", ""V"", ""JNJ"",
      ""UNH"", ""PG"", ""HD"", ""MA"", ""DIS"", ""PYPL"", ""ADBE"", ""CRM"", ""NFLX"", ""INTC"",
      ""TMO"", ""ABT"", ""ACN"", ""XOM"", ""CSCO"", ""ORCL"", ""ABBV"", ""QCOM"", ""COST"", ""CVX"",
      ""LLY"", ""DHR"", ""MCD"", ""HON"", ""TXN"", ""LIN"", ""BMY"", ""MDT"", ""GS"", ""SBUX"",
      ""MMM"", ""AMGN"", ""IBM"", ""RTX"", ""NEE"", ""BA"", ""BLK"", ""AXP"", ""SPGI"", ""CVS"",
      ""COP"", ""SCHW"", ""NOW"", ""MO"", ""GILD"", ""CI"", ""CAT"", ""DE"", ""ANTM"", ""SYK"",
      ""ISRG"", ""ZTS"", ""PNC"", ""NSC"", ""SO"", ""BDX"", ""EW"", ""VRTX"", ""REGN"", ""WM"",
      ""RMD"", ""LHX"", ""IDXX"", ""CMI"", ""TJX"", ""FIS"", ""ITW"", ""DD"", ""LRCX"", ""IQV"",
      ""TEL"", ""PPG"", ""ECL"", ""FDX"", ""ROP"", ""STZ"", ""EXR"", ""TT"", ""SIVB"", ""PSA"",
      ""BIO"", ""ADI"", ""MTD"", ""AVB"", ""FISV"", ""A"", ""AJG"", ""EXPD"", ""CDNS"", ""WST""
    ],
    ""SELL"": [
      ""F"", ""GME"", ""UAA"", ""X"", ""SLB"", ""HAL"", ""MRO"", ""OXY"", ""VLO"", ""MPC"",
      ""HFC"", ""PSX"", ""NCLH"", ""CCL"", ""RCL"", ""UAL"", ""AAL"", ""DAL"", ""LUV"", ""JBLU"",
      ""SAVE"", ""ALK"", ""HA"", ""MAR"", ""HLT"", ""WYNN"", ""LVS"", ""GPS"", ""M"", ""KSS"",
      ""JWN"", ""BBY"", ""LB"", ""HBI"", ""CPRI"", ""PVH"", ""TAP"", ""VIAC"", ""BKR"", ""HPQ"",
      ""DVN"", ""FTI"", ""NOV"", ""RIG"", ""BAC"", ""WFC"", ""C"", ""COF"", ""PFG"", ""DFS"",
      ""FITB"", ""KEY"", ""HBAN"", ""MTB"", ""RF"", ""CMA"", ""USB"", ""CFG"", ""SBNY"", ""STT"",
      ""PNC"", ""BK"", ""ZION"", ""SNV"", ""FHN"", ""WBS"", ""PBCT"", ""PACW"", ""NTRS"", ""ASB"",
      ""CIT"", ""WAL"", ""FCF"", ""TCF"", ""HOMB"", ""FRC"", ""BOKF"", ""UMPQ"", ""TCBI"", ""IBKC"",
      ""HBNC"", ""BMRC"", ""INDB"", ""FFIN"", ""CATY"", ""COLB"", ""WAFD"", ""CUBI"", ""ONB"", ""PNFP""
    ]
  }
  

  {
    ""BUY"": [
      ""AAPL"", ""MSFT"", ""GOOGL"", ""AMZN"", ""META"", ""TSLA"", ""NVDA"", ""V"", ""JPM"", ""JNJ"",
      ""UNH"", ""PG"", ""MA"", ""HD"", ""DIS"", ""PYPL"", ""ADBE"", ""CRM"", ""NFLX"", ""INTC"",
      ""TMO"", ""ABT"", ""XOM"", ""CSCO"", ""ORCL"", ""ACN"", ""QCOM"", ""COST"", ""AVGO"", ""TXN"",
      ""LIN"", ""LLY"", ""MCD"", ""DHR"", ""HON"", ""UPS"", ""UNP"", ""MMM"", ""IBM"", ""NEE"",
      ""CVX"", ""BMY"", ""MDT"", ""GS"", ""SBUX"", ""RTX"", ""CAT"", ""BA"", ""CVS"", ""BLK"",
      ""SPGI"", ""MMC"", ""MO"", ""GILD"", ""CI"", ""GE"", ""DE"", ""FIS"", ""NOW"", ""SCHW"",
      ""AMT"", ""EW"", ""SYK"", ""ISRG"", ""BDX"", ""VRTX"", ""REGN"", ""WM"", ""RMD"", ""LHX"",
      ""IDXX"", ""CMI"", ""TJX"", ""ITW"", ""DD"", ""LRCX"", ""IQV"", ""PPG"", ""ECL"", ""FDX"",
      ""ROP"", ""STZ"", ""EXR"", ""TT"", ""SIVB"", ""PSA"", ""BIO"", ""ADI"", ""MTD"", ""AVB"",
      ""FISV"", ""A"", ""AJG"", ""EXPD"", ""CDNS"", ""WST"", ""KLAC"", ""APD"", ""TEL"", ""CTAS""
    ],
    ""SELL"": [
      ""F"", ""GME"", ""UAA"", ""AA"", ""X"", ""SLB"", ""HAL"", ""MRO"", ""DVN"", ""COP"",
      ""OXY"", ""VLO"", ""MPC"", ""PSX"", ""HFC"", ""NCLH"", ""CCL"", ""RCL"", ""UAL"", ""AAL"",
      ""DAL"", ""LUV"", ""MAR"", ""HLT"", ""WYNN"", ""GPS"", ""M"", ""KSS"", ""JWN"", ""BBY"",
      ""LB"", ""HBI"", ""CPRI"", ""PVH"", ""GPS"", ""TJX"", ""ROST"", ""BURL"", ""COF"", ""DFS"",
      ""SIVB"", ""FITB"", ""KEY"", ""HBAN"", ""MTB"", ""RF"", ""CMA"", ""USB"", ""CFG"", ""PNC"",
      ""BK"", ""STT"", ""WFC"", ""C"", ""BAC"", ""ZION"", ""FHN"", ""SNV"", ""WBS"", ""PBCT"",
      ""PACW"", ""NTRS"", ""ASB"", ""CIT"", ""WAL"", ""FCF"", ""TCF"", ""HOMB"", ""FRC"", ""BOKF"",
      ""UMPQ"", ""TCBI"", ""IBKC"", ""HBNC"", ""BMRC"", ""INDB"", ""FFIN"", ""CATY"", ""COLB"",
      ""WAFD"", ""CUBI"", ""ONB"", ""PNFP"", ""TROW"", ""VTRS"", ""KR"", ""SJM"", ""MDLZ"", ""CPB""
    ]
  }
  

  {
    ""BUY"": [
      ""AAPL"", ""MSFT"", ""AMZN"", ""GOOGL"", ""TSLA"", ""META"", ""BRK.B"", ""V"", ""JNJ"", ""WMT"",
      ""PG"", ""UNH"", ""MA"", ""NVDA"", ""HD"", ""VZ"", ""DIS"", ""ADBE"", ""PYPL"", ""CMCSA"",
      ""PFE"", ""KO"", ""NKE"", ""MRK"", ""PEP"", ""T"", ""ABT"", ""CRM"", ""ORCL"", ""ACN"",
      ""INTC"", ""NFLX"", ""CSCO"", ""LLY"", ""AVGO"", ""BMY"", ""TXN"", ""LIN"", ""QCOM"", ""COST"",
      ""SBUX"", ""MDT"", ""DHR"", ""MCD"", ""AMD"", ""CVS"", ""HON"", ""MMM"", ""UNP"", ""UPS"",
      ""IBM"", ""BA"", ""NEE"", ""LMT"", ""F"", ""BAC"", ""C"", ""CVX"", ""SCHW"", ""BLK"", ""CAT"",
      ""GS"", ""RTX"", ""AMGN"", ""GE"", ""DE"", ""MS"", ""GILD"", ""MO"", ""ANTM"", ""SYK"",
      ""CI"", ""NOW"", ""ISRG"", ""GM"", ""SYY"", ""BDX"", ""NSC"", ""SO"", ""CCI"", ""ITW"",
      ""WM"", ""ADP"", ""CSX"", ""ZTS"", ""EL"", ""EW"", ""FIS"", ""REGN"", ""EXC"", ""VRTX"",
      ""ECL"", ""MET"", ""PGR"", ""RSG"", ""DG"", ""FDX"", ""AEP"", ""IQV"", ""MCK"", ""AON""
    ],
    ""SELL"": [
      ""UAA"", ""GME"", ""SLB"", ""MRO"", ""HAL"", ""XOM"", ""OXY"", ""DVN"", ""COP"", ""DAL"",
      ""UAL"", ""AAL"", ""LUV"", ""JBLU"", ""ALK"", ""GPS"", ""KSS"", ""M"", ""JWN"", ""BBY"",
      ""HBI"", ""LB"", ""COTY"", ""CPRI"", ""PVH"", ""FTI"", ""NOV"", ""APA"", ""HES"", ""HP"",
      ""BKR"", ""VLO"", ""MPC"", ""PSX"", ""HFC"", ""NCLH"", ""CCL"", ""RCL"", ""WYNN"", ""MAR"",
      ""HLT"", ""EXPE"", ""CVS"", ""KHC"", ""KR"", ""SJM"", ""CPB"", ""MDLZ"", ""F"", ""GPS"",
      ""ANF"", ""URBN"", ""JCP"", ""BBBY"", ""DDS"", ""SKX"", ""FL"", ""NKE"", ""TJX"", ""ROST"",
      ""DG"", ""DLTR"", ""FIVE"", ""BIG"", ""OLLI"", ""WMT"", ""TGT"", ""LOW"", ""HD"", ""MGM"",
      ""BYD"", ""CNK"", ""AMC"", ""PLNT"", ""CLX"", ""CL"", ""PG"", ""KMB"", ""UL"", ""EL"",
      ""IFF"", ""NEM"", ""AA"", ""FCX"", ""CLF"", ""WPM"", ""AG"", ""AUY"", ""PAAS"", ""BTG""
    ]
  }
  

  {
    ""BUY"": [
      ""MSFT"", ""AAPL"", ""AMZN"", ""GOOGL"", ""META"", ""TSLA"", ""BRK.B"", ""V"", ""JNJ"", ""JPM"",
      ""UNH"", ""PG"", ""HD"", ""NVDA"", ""MA"", ""VZ"", ""DIS"", ""ADBE"", ""PYPL"", ""NFLX"",
      ""INTC"", ""CMCSA"", ""PFE"", ""KO"", ""NKE"", ""TMO"", ""CSCO"", ""ORCL"", ""ABT"", ""ABBV"",
      ""ACN"", ""QCOM"", ""LLY"", ""COST"", ""AVGO"", ""TXN"", ""LIN"", ""BMY"", ""MDT"", ""DHR"",
      ""HON"", ""UPS"", ""UNP"", ""MMM"", ""IBM"", ""RTX"", ""BA"", ""CVX"", ""GS"", ""NEE"",
      ""BLK"", ""SBUX"", ""AMGN"", ""GE"", ""CAT"", ""MDLZ"", ""CVS"", ""MO"", ""GILD"", ""BAC"",
      ""C"", ""SCHW"", ""SPGI"", ""MMC"", ""AXP"", ""TGT"", ""ANTM"", ""SYK"", ""ISRG"", ""DE"",
      ""NOW"", ""ZTS"", ""CI"", ""BDX"", ""CSX"", ""NSC"", ""SO"", ""EW"", ""FIS"", ""REGN"",
      ""ITW"", ""LHX"", ""RMD"", ""IDXX"", ""CMI"", ""TJX"", ""WM"", ""ADI"", ""TEL"", ""DD"",
      ""IQV"", ""VRTX"", ""FRC"", ""ETN"", ""ROP"", ""A"", ""CTAS"", ""ECL"", ""CDNS"", ""EXR""
    ],
    ""SELL"": [
      ""F"", ""GME"", ""UAA"", ""M"", ""KSS"", ""JWN"", ""GPS"", ""X"", ""SLB"", ""HAL"",
      ""MRO"", ""DVN"", ""OXY"", ""VLO"", ""MPC"", ""HFC"", ""PSX"", ""NCLH"", ""CCL"", ""RCL"",
      ""UAL"", ""AAL"", ""DAL"", ""LUV"", ""SAVE"", ""ALK"", ""JBLU"", ""HA"", ""MAR"", ""HLT"",
      ""WYNN"", ""LVS"", ""FTR"", ""BBY"", ""HBI"", ""LB"", ""CPRI"", ""PVH"", ""GPS"", ""TJX"",
      ""ROST"", ""BURL"", ""COF"", ""DFS"", ""KEY"", ""FITB"", ""RF"", ""CMA"", ""USB"", ""CFG"",
      ""PNC"", ""WFC"", ""BK"", ""STT"", ""MTB"", ""HBAN"", ""SBNY"", ""ZION"", ""FHN"", ""WBS"",
      ""PBCT"", ""PACW"", ""NTRS"", ""CIT"", ""WAL"", ""FCF"", ""TCF"", ""HOMB"", ""FRC"", ""BOKF"",
      ""UMPQ"", ""TCBI"", ""IBKC"", ""HBNC"", ""BMRC"", ""INDB"", ""FFIN"", ""CATY"", ""COLB"",
      ""WAFD"", ""CUBI"", ""ONB"", ""PNFP"", ""TROW"", ""VTRS"", ""KR"", ""SJM"", ""MDLZ"", ""CPB""
    ]
  }
  

  {
    ""BUY"": [
      ""AAPL"", ""MSFT"", ""AMZN"", ""GOOGL"", ""META"", ""TSLA"", ""NVDA"", ""BRKB"", ""V"", ""JNJ"",
      ""UNH"", ""PG"", ""MA"", ""VZ"", ""HD"", ""DIS"", ""ADBE"", ""PYPL"", ""NFLX"", ""CRM"",
      ""INTC"", ""CSCO"", ""TMO"", ""ABT"", ""ACN"", ""LLY"", ""QCOM"", ""COST"", ""AVGO"", ""TXN"",
      ""LIN"", ""BMY"", ""MDT"", ""DHR"", ""HON"", ""MMM"", ""GS"", ""AMGN"", ""IBM"", ""RTX"",
      ""BA"", ""CVX"", ""NEE"", ""BLK"", ""SBUX"", ""F"", ""COP"", ""NOC"", ""CVS"", ""DE"",
      ""NOW"", ""SCHW"", ""AXP"", ""SPGI"", ""MMC"", ""GILD"", ""CI"", ""GM"", ""MO"", ""CAT"",
      ""BDX"", ""PNC"", ""EW"", ""ISRG"", ""REGN"", ""VRTX"", ""IDXX"", ""ZTS"", ""SYK"", ""RMD"",
      ""ANTM"", ""SNPS"", ""ECL"", ""DXCM"", ""IQV"", ""TGT"", ""TEL"", ""A"", ""ROP"", ""SHW"",
      ""CTAS"", ""TDG"", ""DD"", ""ITW"", ""EXR"", ""LRCX"", ""CDNS"", ""KLAC"", ""ADP"", ""WST"",
      ""APH"", ""CMI"", ""PPG"", ""SYY"", ""MCK"", ""BAX"", ""HUM"", ""BIIB"", ""FISV"", ""EQIX""
    ],
    ""SELL"": [
      ""F"", ""GME"", ""UAA"", ""X"", ""SLB"", ""HAL"", ""MRO"", ""DVN"", ""OXY"", ""VLO"",
      ""MPC"", ""HFC"", ""PSX"", ""NCLH"", ""CCL"", ""RCL"", ""UAL"", ""AAL"", ""DAL"", ""LUV"",
      ""KSS"", ""JWN"", ""GPS"", ""M"", ""BBY"", ""LB"", ""HBI"", ""CPRI"", ""PVH"", ""FTI"",
      ""NOV"", ""HP"", ""BKR"", ""WFC"", ""C"", ""BAC"", ""PFG"", ""DFS"", ""FITB"", ""RF"",
      ""KEY"", ""CFG"", ""CMA"", ""MTB"", ""HBAN"", ""SIVB"", ""STT"", ""ZION"", ""FHN"", ""WBS"",
      ""PBCT"", ""PACW"", ""NTRS"", ""CIT"", ""WAL"", ""FCF"", ""TCF"", ""USB"", ""SNV"", ""ASB"",
      ""SBNY"", ""BOKF"", ""UMPQ"", ""TCBI"", ""IBKC"", ""HBNC"", ""BMRC"", ""INDB"", ""FFIN"", ""CATY"",
      ""COLB"", ""WAFD"", ""CUBI"", ""ONB"", ""PNFP"", ""TROW"", ""VTRS"", ""KR"", ""SJM"", ""CPB"",
      ""GPS"", ""ANF"", ""URBN"", ""DDS"", ""SKX"", ""FL"", ""TJX"", ""ROST"", ""BURL"", ""MAR""
    ]
  }
  

  {
    ""BUY"": [
      ""AAPL"", ""MSFT"", ""AMZN"", ""GOOGL"", ""META"", ""TSLA"", ""NVDA"", ""BRK.B"", ""V"", ""JNJ"",
      ""UNH"", ""PG"", ""MA"", ""VZ"", ""HD"", ""DIS"", ""PYPL"", ""ADBE"", ""NFLX"", ""CRM"",
      ""INTC"", ""TMO"", ""ABT"", ""ACN"", ""XOM"", ""CSCO"", ""LLY"", ""QCOM"", ""COST"", ""AVGO"",
      ""TXN"", ""LIN"", ""BMY"", ""MDT"", ""DHR"", ""HON"", ""MMM"", ""GS"", ""AMGN"", ""IBM"",
      ""RTX"", ""BA"", ""CVX"", ""NEE"", ""BLK"", ""SBUX"", ""F"", ""COP"", ""NOC"", ""CVS"",
      ""DE"", ""NOW"", ""SCHW"", ""AXP"", ""SPGI"", ""MMC"", ""GILD"", ""CI"", ""GM"", ""MO"",
      ""CAT"", ""BDX"", ""PNC"", ""EW"", ""ISRG"", ""REGN"", ""VRTX"", ""IDXX"", ""ZTS"", ""SYK"",
      ""RMD"", ""ANTM"", ""SNPS"", ""ECL"", ""DXCM"", ""IQV"", ""TGT"", ""TEL"", ""A"", ""ROP"",
      ""SHW"", ""CTAS"", ""TDG"", ""DD"", ""ITW"", ""EXR"", ""LRCX"", ""CDNS"", ""KLAC"", ""ADP"",
      ""WST"", ""APH"", ""CMI"", ""PPG"", ""SYY"", ""MCK"", ""BAX"", ""HUM"", ""BIIB"", ""FISV"", ""EQIX""
    ],
    ""SELL"": [
      ""F"", ""GME"", ""UAA"", ""X"", ""SLB"", ""HAL"", ""MRO"", ""DVN"", ""OXY"", ""VLO"",
      ""MPC"", ""HFC"", ""PSX"", ""NCLH"", ""CCL"", ""RCL"", ""UAL"", ""AAL"", ""DAL"", ""LUV"",
      ""KSS"", ""JWN"", ""GPS"", ""M"", ""BBY"", ""LB"", ""HBI"", ""CPRI"", ""PVH"", ""FTI"",
      ""NOV"", ""HP"", ""BKR"", ""WFC"", ""C"", ""BAC"", ""PFG"", ""DFS"", ""FITB"", ""RF"",
      ""KEY"", ""CFG"", ""CMA"", ""MTB"", ""HBAN"", ""SIVB"", ""STT"", ""ZION"", ""FHN"", ""WBS"",
      ""PBCT"", ""PACW"", ""NTRS"", ""CIT"", ""WAL"", ""FCF"", ""TCF"", ""USB"", ""SNV"", ""ASB"",
      ""SBNY"", ""BOKF"", ""UMPQ"", ""TCBI"", ""IBKC"", ""HBNC"", ""BMRC"", ""INDB"", ""FFIN"", ""CATY"",
      ""COLB"", ""WAFD"", ""CUBI"", ""ONB"", ""PNFP"", ""TROW"", ""VTRS"", ""KR"", ""SJM"", ""CPB"",
      ""GPS"", ""ANF"", ""URBN"", ""DDS"", ""SKX"", ""FL"", ""TJX"", ""ROST"", ""BURL"", ""MAR""
    ]
  }
  

  {
    ""BUY"": [
      ""AAPL"", ""MSFT"", ""GOOGL"", ""AMZN"", ""META"", ""TSLA"", ""NVDA"", ""BRKB"", ""V"", ""JPM"",
      ""JNJ"", ""UNH"", ""PG"", ""HD"", ""MA"", ""VZ"", ""DIS"", ""ADBE"", ""PYPL"", ""NFLX"",
      ""INTC"", ""TMO"", ""ABT"", ""ACN"", ""XOM"", ""CSCO"", ""LLY"", ""QCOM"", ""COST"", ""AVGO"",
      ""TXN"", ""LIN"", ""BMY"", ""MDT"", ""DHR"", ""HON"", ""MMM"", ""GS"", ""AMGN"", ""IBM"",
      ""RTX"", ""BA"", ""CVX"", ""NEE"", ""BLK"", ""SBUX"", ""F"", ""COP"", ""NOC"", ""CVS"",
      ""DE"", ""NOW"", ""SCHW"", ""AXP"", ""SPGI"", ""MMC"", ""GILD"", ""CI"", ""GM"", ""MO"",
      ""CAT"", ""BDX"", ""PNC"", ""EW"", ""ISRG"", ""REGN"", ""VRTX"", ""IDXX"", ""ZTS"", ""SYK"",
      ""RMD"", ""ANTM"", ""SNPS"", ""ECL"", ""DXCM"", ""IQV"", ""TGT"", ""TEL"", ""A"", ""ROP"",
      ""SHW"", ""CTAS"", ""TDG"", ""DD"", ""ITW"", ""EXR"", ""LRCX"", ""CDNS"", ""KLAC"", ""ADP"",
      ""WST"", ""APH"", ""CMI"", ""PPG"", ""SYY"", ""MCK"", ""BAX"", ""HUM"", ""BIIB"", ""FISV"", ""EQIX""
    ],
    ""SELL"": [
      ""F"", ""GME"", ""UAA"", ""X"", ""SLB"", ""HAL"", ""MRO"", ""DVN"", ""OXY"", ""VLO"",
      ""MPC"", ""HFC"", ""PSX"", ""NCLH"", ""CCL"", ""RCL"", ""UAL"", ""AAL"", ""DAL"", ""LUV"",
      ""KSS"", ""JWN"", ""GPS"", ""M"", ""BBY"", ""LB"", ""HBI"", ""CPRI"", ""PVH"", ""FTI"",
      ""NOV"", ""HP"", ""BKR"", ""WFC"", ""C"", ""BAC"", ""PFG"", ""DFS"", ""FITB"", ""RF"",
      ""KEY"", ""CFG"", ""CMA"", ""MTB"", ""HBAN"", ""SIVB"", ""STT"", ""ZION"", ""FHN"", ""WBS"",
      ""PBCT"", ""PACW"", ""NTRS"", ""CIT"", ""WAL"", ""FCF"", ""TCF"", ""USB"", ""SNV"", ""ASB"",
      ""SBNY"", ""BOKF"", ""UMPQ"", ""TCBI"", ""IBKC"", ""HBNC"", ""BMRC"", ""INDB"", ""FFIN"", ""CATY"",
      ""COLB"", ""WAFD"", ""CUBI"", ""ONB"", ""PNFP"", ""TROW"", ""VTRS"", ""KR"", ""SJM"", ""CPB"",
      ""GPS"", ""ANF"", ""URBN"", ""DDS"", ""SKX"", ""FL"", ""TJX"", ""ROST"", ""BURL"", ""MAR""
    ]
  }
  

  {
    ""BUY"": [
      ""AAPL"", ""MSFT"", ""AMZN"", ""GOOGL"", ""META"", ""TSLA"", ""BRK.B"", ""V"", ""JNJ"", ""PG"",
      ""UNH"", ""MA"", ""HD"", ""NVDA"", ""DIS"", ""PYPL"", ""ADBE"", ""NFLX"", ""CRM"", ""VZ"",
      ""INTC"", ""CSCO"", ""PFE"", ""TMO"", ""ABT"", ""ABBV"", ""LLY"", ""QCOM"", ""COST"", ""TXN"",
      ""LIN"", ""BMY"", ""MDT"", ""DHR"", ""HON"", ""AMGN"", ""AVGO"", ""IBM"", ""MMM"", ""BA"",
      ""CVX"", ""GS"", ""JPM"", ""AXP"", ""BLK"", ""T"", ""MO"", ""GILD"", ""GE"", ""CAT"", ""CVS"",
      ""DOW"", ""SCHW"", ""SPGI"", ""MMC"", ""UPS"", ""ANTM"", ""BDX"", ""SYK"", ""ISRG"", ""VRTX"",
      ""REGN"", ""GM"", ""ADI"", ""EW"", ""MET"", ""PNC"", ""NSC"", ""SO"", ""WM"", ""CMI"", ""ETN"",
      ""ECL"", ""EMR"", ""SYY"", ""FIS"", ""CDNS"", ""TEL"", ""ITW"", ""ROP"", ""CTAS"", ""TT"",
      ""APH"", ""STZ"", ""NOC"", ""WM"", ""WBA"", ""LRCX"", ""RTX"", ""FDX"", ""CSX"", ""EXC"",
      ""ADP"", ""DD"", ""LHX"", ""NUE"", ""ZTS"", ""ORCL"", ""COP"", ""AIG"", ""BAX"", ""PSA""
    ],
    ""SELL"": [
      ""F"", ""GME"", ""UAA"", ""M"", ""KSS"", ""JWN"", ""GPS"", ""X"", ""SLB"", ""HAL"",
      ""MRO"", ""DVN"", ""OXY"", ""VLO"", ""MPC"", ""HFC"", ""PSX"", ""NCLH"", ""CCL"", ""RCL"",
      ""UAL"", ""AAL"", ""DAL"", ""LUV"", ""BBY"", ""HBI"", ""CPRI"", ""PVH"", ""FTI"", ""NOV"",
      ""BKR"", ""HPQ"", ""APA"", ""COG"", ""CHK"", ""SWN"", ""CNX"", ""CLR"", ""CPE"", ""HES"",
      ""WFC"", ""C"", ""BAC"", ""PFG"", ""FITB"", ""KEY"", ""HBAN"", ""RF"", ""CMA"", ""CFG"",
      ""SIVB"", ""MTB"", ""WBS"", ""STT"", ""ZION"", ""FHN"", ""SNV"", ""ASB"", ""PACW"", ""USB"",
      ""PBCT"", ""CIT"", ""WAL"", ""FCF"", ""TCF"", ""BOKF"", ""UMPQ"", ""TCBI"", ""IBKC"", ""HBNC"",
      ""BMRC"", ""INDB"", ""FFIN"", ""CATY"", ""COLB"", ""WAFD"", ""CUBI"", ""ONB"", ""PNFP"", ""TROW"",
      ""KR"", ""SJM"", ""CPB"", ""MDLZ"", ""FLO"", ""IFF"", ""ADM"", ""BG"", ""HRL"", ""MKC""
    ]
  }
  

  {
    ""BUY"": [
      ""AAPL"", ""MSFT"", ""AMZN"", ""GOOGL"", ""TSLA"", ""META"", ""NVDA"", ""BRK.B"", ""JNJ"", ""V"",
      ""PG"", ""UNH"", ""MA"", ""HD"", ""DIS"", ""ADBE"", ""CRM"", ""NFLX"", ""INTC"", ""PYPL"",
      ""CSCO"", ""ORCL"", ""TMO"", ""ABT"", ""ACN"", ""LLY"", ""QCOM"", ""COST"", ""AVGO"", ""TXN"",
      ""LIN"", ""BMY"", ""MDT"", ""DHR"", ""HON"", ""MMM"", ""GS"", ""AMGN"", ""IBM"", ""RTX"",
      ""BA"", ""CVX"", ""NEE"", ""BLK"", ""SBUX"", ""F"", ""COP"", ""NOC"", ""CVS"", ""DE"",
      ""NOW"", ""SCHW"", ""AXP"", ""SPGI"", ""MMC"", ""GILD"", ""CI"", ""GM"", ""MO"", ""CAT"",
      ""BDX"", ""PNC"", ""EW"", ""ISRG"", ""REGN"", ""VRTX"", ""IDXX"", ""ZTS"", ""SYK"", ""RMD"",
      ""ANTM"", ""SNPS"", ""ECL"", ""DXCM"", ""IQV"", ""TGT"", ""TEL"", ""A"", ""ROP"", ""SHW"",
      ""CTAS"", ""TDG"", ""DD"", ""ITW"", ""EXR"", ""LRCX"", ""CDNS"", ""KLAC"", ""ADP"", ""WST"",
      ""APH"", ""CMI"", ""PPG"", ""SYY"", ""MCK"", ""BAX"", ""HUM"", ""BIIB"", ""FISV"", ""EQIX""
    ],
    ""SELL"": [
      ""F"", ""GME"", ""UAA"", ""X"", ""SLB"", ""HAL"", ""MRO"", ""DVN"", ""OXY"", ""VLO"",
      ""MPC"", ""HFC"", ""PSX"", ""NCLH"", ""CCL"", ""RCL"", ""UAL"", ""AAL"", ""DAL"", ""LUV"",
      ""KSS"", ""JWN"", ""GPS"", ""M"", ""BBY"", ""LB"", ""HBI"", ""CPRI"", ""PVH"", ""FTI"",
      ""NOV"", ""HP"", ""BKR"", ""WFC"", ""C"", ""BAC"", ""PFG"", ""DFS"", ""FITB"", ""RF"",
      ""KEY"", ""CFG"", ""CMA"", ""MTB"", ""HBAN"", ""SIVB"", ""STT"", ""ZION"", ""FHN"", ""WBS"",
      ""PBCT"", ""PACW"", ""NTRS"", ""CIT"", ""WAL"", ""FCF"", ""TCF"", ""USB"", ""SNV"", ""ASB"",
      ""SBNY"", ""BOKF"", ""UMPQ"", ""TCBI"", ""IBKC"", ""HBNC"", ""BMRC"", ""INDB"", ""FFIN"", ""CATY"",
      ""COLB"", ""WAFD"", ""CUBI"", ""ONB"", ""PNFP"", ""TROW"", ""VTRS"", ""KR"", ""SJM"", ""CPB"",
      ""GPS"", ""ANF"", ""URBN"", ""DDS"", ""SKX"", ""FL"", ""TJX"", ""ROST"", ""BURL"", ""MAR""
    ]
  }
  
  
  {
    ""BUY"": [
      ""AAPL"", ""MSFT"", ""GOOGL"", ""AMZN"", ""META"", ""TSLA"", ""NVDA"", ""BRK.B"", ""V"", ""JNJ"",
      ""UNH"", ""PG"", ""MA"", ""HD"", ""DIS"", ""ADBE"", ""NFLX"", ""CRM"", ""PYPL"", ""INTC"",
      ""CSCO"", ""ORCL"", ""TMO"", ""ABT"", ""ACN"", ""LLY"", ""QCOM"", ""COST"", ""TXN"", ""AVGO"",
      ""LIN"", ""BMY"", ""MDT"", ""DHR"", ""HON"", ""MMM"", ""GS"", ""AMGN"", ""IBM"", ""RTX"",
      ""BA"", ""CVX"", ""NEE"", ""BLK"", ""SBUX"", ""F"", ""COP"", ""NOC"", ""CVS"", ""DE"",
      ""NOW"", ""SCHW"", ""AXP"", ""SPGI"", ""MMC"", ""GILD"", ""CI"", ""GM"", ""MO"", ""CAT"",
      ""BDX"", ""PNC"", ""EW"", ""ISRG"", ""REGN"", ""VRTX"", ""IDXX"", ""ZTS"", ""SYK"", ""RMD"",
      ""ANTM"", ""SNPS"", ""ECL"", ""DXCM"", ""IQV"", ""TGT"", ""TEL"", ""A"", ""ROP"", ""SHW"",
      ""CTAS"", ""TDG"", ""DD"", ""ITW"", ""EXR"", ""LRCX"", ""CDNS"", ""KLAC"", ""ADP"", ""WST"",
      ""APH"", ""CMI"", ""PPG"", ""SYY"", ""MCK"", ""BAX"", ""HUM"", ""BIIB"", ""FISV"", ""EQIX""
    ],
    ""SELL"": [
      ""F"", ""GME"", ""UAA"", ""X"", ""SLB"", ""HAL"", ""MRO"", ""DVN"", ""OXY"", ""VLO"",
      ""MPC"", ""HFC"", ""PSX"", ""NCLH"", ""CCL"", ""RCL"", ""UAL"", ""AAL"", ""DAL"", ""LUV"",
      ""KSS"", ""JWN"", ""GPS"", ""M"", ""BBY"", ""LB"", ""HBI"", ""CPRI"", ""PVH"", ""FTI"",
      ""NOV"", ""HP"", ""BKR"", ""WFC"", ""C"", ""BAC"", ""PFG"", ""DFS"", ""FITB"", ""RF"",
      ""KEY"", ""CFG"", ""CMA"", ""MTB"", ""HBAN"", ""SIVB"", ""STT"", ""ZION"", ""FHN"", ""WBS"",
      ""PBCT"", ""PACW"", ""NTRS"", ""CIT"", ""WAL"", ""FCF"", ""TCF"", ""USB"", ""SNV"", ""ASB"",
      ""SBNY"", ""BOKF"", ""UMPQ"", ""TCBI"", ""IBKC"", ""HBNC"", ""BMRC"", ""INDB"", ""FFIN"", ""CATY"",
      ""COLB"", ""WAFD"", ""CUBI"", ""ONB"", ""PNFP"", ""TROW"", ""VTRS"", ""KR"", ""SJM"", ""CPB"",
      ""GPS"", ""ANF"", ""URBN"", ""DDS"", ""SKX"", ""FL"", ""TJX"", ""ROST"", ""BURL"", ""MAR""
    ]
  }
  

  {
    ""BUY"": [
      ""AAPL"", ""MSFT"", ""AMZN"", ""GOOGL"", ""TSLA"", ""META"", ""NVDA"", ""BRKB"", ""JNJ"", ""V"",
      ""PG"", ""UNH"", ""MA"", ""HD"", ""VZ"", ""DIS"", ""ADBE"", ""PYPL"", ""NFLX"", ""CRM"",
      ""INTC"", ""CSCO"", ""TMO"", ""ABT"", ""ACN"", ""LLY"", ""QCOM"", ""COST"", ""AVGO"", ""TXN"",
      ""LIN"", ""BMY"", ""MDT"", ""DHR"", ""HON"", ""MMM"", ""GS"", ""AMGN"", ""IBM"", ""RTX"",
      ""BA"", ""CVX"", ""NEE"", ""BLK"", ""SBUX"", ""F"", ""COP"", ""NOC"", ""CVS"", ""DE"",
      ""NOW"", ""SCHW"", ""AXP"", ""SPGI"", ""MMC"", ""GILD"", ""CI"", ""GM"", ""MO"", ""CAT"",
      ""BDX"", ""PNC"", ""EW"", ""ISRG"", ""REGN"", ""VRTX"", ""IDXX"", ""ZTS"", ""SYK"", ""RMD"",
      ""ANTM"", ""SNPS"", ""ECL"", ""DXCM"", ""IQV"", ""TGT"", ""TEL"", ""A"", ""ROP"", ""SHW"",
      ""CTAS"", ""TDG"", ""DD"", ""ITW"", ""EXR"", ""LRCX"", ""CDNS"", ""KLAC"", ""ADP"", ""WST"",
      ""APH"", ""CMI"", ""PPG"", ""SYY"", ""MCK"", ""BAX"", ""HUM"", ""BIIB"", ""FISV"", ""EQIX""
    ],
    ""SELL"": [
      ""F"", ""GME"", ""UAA"", ""X"", ""SLB"", ""HAL"", ""MRO"", ""DVN"", ""OXY"", ""VLO"",
      ""MPC"", ""HFC"", ""PSX"", ""NCLH"", ""CCL"", ""RCL"", ""UAL"", ""AAL"", ""DAL"", ""LUV"",
      ""KSS"", ""JWN"", ""GPS"", ""M"", ""BBY"", ""LB"", ""HBI"", ""CPRI"", ""PVH"", ""FTI"",
      ""NOV"", ""HP"", ""BKR"", ""WFC"", ""C"", ""BAC"", ""PFG"", ""DFS"", ""FITB"", ""RF"",
      ""KEY"", ""CFG"", ""CMA"", ""MTB"", ""HBAN"", ""SIVB"", ""STT"", ""ZION"", ""FHN"", ""WBS"",
      ""PBCT"", ""PACW"", ""NTRS"", ""CIT"", ""WAL"", ""FCF"", ""TCF"", ""USB"", ""SNV"", ""ASB"",
      ""SBNY"", ""BOKF"", ""UMPQ"", ""TCBI"", ""IBKC"", ""HBNC"", ""BMRC"", ""INDB"", ""FFIN"", ""CATY"",
      ""COLB"", ""WAFD"", ""CUBI"", ""ONB"", ""PNFP"", ""TROW"", ""VTRS"", ""KR"", ""SJM"", ""CPB"",
      ""GPS"", ""ANF"", ""URBN"", ""DDS"", ""SKX"", ""FL"", ""TJX"", ""ROST"", ""BURL"", ""MAR""
    ]
  }
  
  


";
        }

    }
}
