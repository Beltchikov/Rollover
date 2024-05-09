using System;
using System.Text.Json;

namespace NetBms
{
    public partial class FormStart : Form
    {
        string _currentDirectory = Directory.GetCurrentDirectory();
        List<string> _exceptions = new List<string>();

        readonly string BATCH_SEPARATOR = Environment.NewLine + Environment.NewLine;
        private readonly string TEST_DATA_PATH = "TestData\\";
        private readonly char CURLY_BRACKET = '{';

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
                string batchValidated = ValidateBatch(batch);
                
                try
                {
                    ChatGptBatchResult chatGptBatchResult = JsonSerializer.Deserialize<ChatGptBatchResult>(batchValidated)!;
                    batches.Add(chatGptBatchResult);
                }
                catch (JsonException jsonException)
                {
                    _exceptions.Add(jsonException.ToString());
                    notConvertableStrings.Add(batchValidated);
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

            if (!_exceptions.Any()) MessageBox.Show("Done!");
            else MessageBox.Show(_exceptions
                .Aggregate((r,n)=> r + Environment.NewLine + Environment.NewLine + r));
        }

        private string ValidateBatch(string batch)
        {
            if (!batch.StartsWith(CURLY_BRACKET)) throw new Exception($"The following batch dpes not start with {CURLY_BRACKET}. Batch: {batch}");
            return batch;
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
