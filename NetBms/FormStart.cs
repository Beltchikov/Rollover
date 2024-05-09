using System.Text.Json;

namespace NetBms
{
    public partial class FormStart : Form
    {
        string _currentDirectory = Directory.GetCurrentDirectory();

        readonly string BATCH_SEPARATOR = Environment.NewLine + Environment.NewLine;
        private readonly string TEST_DATA_PATH = "TestData\\";

        public FormStart()
        {
            InitializeComponent();
            txtChatGptBatchResults.MaxLength = 0;


            //txtChatGptBatchResults.Text = TestData1();
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
            string fileName = "TestData2.txt";
            return File.ReadAllText(Path.Combine(_currentDirectory, TEST_DATA_PATH, fileName));
        }

        private string TestData1()
        {
            string fileName = "TestData1.txt";
            return File.ReadAllText(Path.Combine(_currentDirectory, TEST_DATA_PATH, fileName));
        }

        private string TestDataBug_24_05_08()
        {
            string fileName = "TestDataBug_24_05_08.txt";
            return File.ReadAllText(Path.Combine(_currentDirectory, TEST_DATA_PATH, fileName));
        }

    }
}
