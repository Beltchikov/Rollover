using System.IO;
using System.Text.Json;

internal class Program
{
    private static void Main(string[] args)
    {
        WebApplication app = Helpers.BuildWebApplication();

        var summaries = new[]
        {
            "Freezing", "Bracing", "Chilly", "Cool", "Mild", "Warm", "Balmy", "Hot", "Sweltering", "Scorching"
        };

        app.MapGet("/weatherforecast", () =>
        {
            var forecast = Enumerable.Range(1, 5).Select(index =>
                new WeatherForecast
                (
                    DateOnly.FromDateTime(DateTime.Now.AddDays(index)),
                    Random.Shared.Next(-20, 55),
                    summaries[Random.Shared.Next(summaries.Length)]
                ))
                .ToArray();
            return forecast;
        })
        .WithName("GetWeatherForecast")
        .WithOpenApi();

        app.MapGet("/balance-sheet-statement", async (HttpClient httpClient, string[] stockSymbols) =>
        {
            string apiKey = "14e7a22ed6110f130afa41af05599bb6";
            string baseUrl = "https://financialmodelingprep.com/api/v3/balance-sheet-statement/";

            // Step 1: Call the API for every symbol and store results in balanceSheetResponseDict
            var balanceSheetResponseDict = await FetchFmpResponses(httpClient, stockSymbols, baseUrl, apiKey);

            // Step 2: Serialize the responses into balanceSheetStatementDict
            var balanceSheetStatementDict = DeserializeFmpResponses<BalanceSheetStatement>(balanceSheetResponseDict);

            // Step 3: Create symbols table
            List<string> symbolsTable = CreateSymbolsTable(balanceSheetStatementDict);

            // Step 4: Interpolate data
            List<string> interpolatedSymbolsTable = InterpolateSymbolsTable(symbolsTable);

            // Step 5:
            ChartData chartData = CreateChartData(interpolatedSymbolsTable);

            //DiagOutput(labels, datasets);
            //DiagOutput(interpolatedSymbolsTable);

            return Results.Ok(balanceSheetStatementDict);
        })
        .WithName("GetBalanceSheetStatement")
        .WithOpenApi();

        app.MapGet("/balance-sheet-statement-mock", () =>
        {
            // Define the path to the MockResponses directory
            string mockDirectory = Path.Combine(Directory.GetCurrentDirectory(), "MockResponses", "balance-sheet-statement");

            // Check if the directory exists
            if (!Directory.Exists(mockDirectory))
            {
                return Results.NotFound("MockResponses directory not found.");
            }

            // Prepare a dictionary to hold the mock responses
            var balanceSheetResponseDict = new Dictionary<string, List<BalanceSheetStatement>>();

            // Get all the files in the directory (assuming .json files)
            var files = Directory.GetFiles(mockDirectory, "*.json");

            foreach (var file in files)
            {
                // Extract the symbol from the file name (assuming the file name follows a certain pattern)
                var fileName = Path.GetFileNameWithoutExtension(file); // e.g., "AAPL.json" -> "AAPL"
                string jsonContent = File.ReadAllText(file); // Read the file content

                // Deserialize the content into a list of BalanceSheetStatement objects
                var balanceSheetStatements = JsonSerializer.Deserialize<List<BalanceSheetStatement>>(jsonContent);

                // Add the deserialized content to the response dictionary
                if (balanceSheetStatements != null)
                {
                    balanceSheetResponseDict[fileName] = balanceSheetStatements;
                }
            }

            // Return the response as JSON
            return Results.Json(balanceSheetResponseDict);
        })
        .WithName("GetBalanceSheetStatementMock")
        .WithOpenApi(); ;

        app.MapGet("/cash-flow-statement", async (HttpClient httpClient, string[] stockSymbols) =>
        {
            string apiKey = "14e7a22ed6110f130afa41af05599bb6";
            string baseUrl = "https://financialmodelingprep.com/api/v3/cash-flow-statement/";

            // Step 1: Call the API for every symbol and store results in balanceSheetResponseDict
            Dictionary<string, string> cashFlowResponseDict = await FetchFmpResponses(httpClient, stockSymbols, baseUrl, apiKey);
        
            // Step 2: Serialize the responses into balanceSheetStatementDict
            var cashFlowStatementDict = DeserializeFmpResponses<CashFlowStatement>(cashFlowResponseDict);

            // TODO
            // // Step 3: Create symbols table
            //List<string> symbolsTable = CreateSymbolsTable(cashFlowStatementDict);

            // // Step 4: Interpolate data
            // List<string> interpolatedSymbolsTable = InterpolateSymbolsTable(symbolsTable);

            // // Step 5:
            // ChartData chartData = CreateChartData(interpolatedSymbolsTable);

            return Results.Ok(cashFlowStatementDict);
        })
        .WithName("GetCashFlowStatement")
        .WithOpenApi();

        app.MapGet("/cash-flow-statement-mock", () =>
        {
           // Define the path to the MockResponses directory
           string mockDirectory = Path.Combine(Directory.GetCurrentDirectory(), "MockResponses", "cash-flow-statement");

           // Check if the directory exists
           if (!Directory.Exists(mockDirectory))
           {
               return Results.NotFound("MockResponses directory not found.");
           }

           // Prepare a dictionary to hold the mock responses
           var cashFlowResponseDict = new Dictionary<string, List<CashFlowStatement>>();

           // Get all the files in the directory (assuming .json files)
           var files = Directory.GetFiles(mockDirectory, "*.json");

           foreach (var file in files)
           {
               // Extract the symbol from the file name (assuming the file name follows a certain pattern)
               var fileName = Path.GetFileNameWithoutExtension(file); // e.g., "AAPL.json" -> "AAPL"
               string jsonContent = File.ReadAllText(file); // Read the file content

               // Deserialize the content into a list of BalanceSheetStatement objects
               var cashFlowStatements = JsonSerializer.Deserialize<List<CashFlowStatement>>(jsonContent);

               // Add the deserialized content to the response dictionary
               if (cashFlowStatements != null)
               {
                   cashFlowResponseDict[fileName] = cashFlowStatements;
               }
           }

           // Return the response as JSON
           return Results.Json(cashFlowResponseDict);
        })
       .WithName("GetCashFlowStatementMock")
       .WithOpenApi(); ;

        app.Run();
    }

    private static ChartData CreateChartData(List<string> interpolatedSymbolsTable)
    {
        // Prepare the datasets based on the interpolated symbols table data
        var colors = Helpers.GetRandomRgbColors(interpolatedSymbolsTable.Count - 1); // Skip the header row
        // Extract the header row (which contains the labels/dates)
        var labels = interpolatedSymbolsTable[0].Split('\t').Skip(1).ToArray(); // Skip the "Symbol" column
        var datasets = interpolatedSymbolsTable.Skip(1) // Skip the header row
            .Select((row, index) =>
            {
                var columns = row.Split('\t');
                var symbol = columns[0];  // The first column is the stock symbol
                var data = columns.Skip(1) // Skip the symbol
                    .Select(val => string.IsNullOrWhiteSpace(val) || val == "" ? (long?)null : long.Parse(val))
                    .ToArray();

                return new Dataset(
                    Label: symbol, // Stock symbol is used as the label for the dataset
                    Data: data,    // Interpolated retained earnings values
                    BorderColor: colors[index],
                    BackgroundColor: colors[index].Replace("1)", "0.2)"),
                    YAxisID: "y-axis-1",
                    Hidden: false,
                    BorderWidth: 1
                );
            })
            .ToArray();

        //
        var chartData = new ChartData(
            Labels: labels,
            Datasets: datasets
        );

        return chartData;

    }

    /// <summary>
    /// Interpolates missing retained earnings values (marked as "NULL") within the symbol table,
    /// but only for values not at the beginning or end of the row.
    /// </summary>
    static List<string> InterpolateSymbolsTable(List<string> symbolsTable)
    {
        // Initialize the interpolated table with the header row
        var interpolatedTable = new List<string> { symbolsTable[0] };  // Add the header directly

        // Iterate through each row (skip the header, which is at index 0)
        for (int i = 1; i < symbolsTable.Count; i++)
        {
            var row = symbolsTable[i];
            var columns = row.Split('\t');  // Split the row into columns (tab-separated)

            string symbol = columns[0];  // First column is the symbol
            var dataColumns = columns.Skip(1).ToArray();  // The rest are retained earnings values

            // Convert the data columns to a list of nullable longs, interpreting "NULL" as null
            var values = dataColumns.Select(val => string.IsNullOrWhiteSpace(val) || val == "NULL" ? (long?)null : long.Parse(val)).ToArray();

            // Perform interpolation
            for (int j = 1; j < values.Length - 1; j++)  // Don't interpolate at the first and last positions
            {
                if (values[j] == null)  // Only interpolate if the value is "NULL" (null)
                {
                    // Find the previous non-null value
                    int prevIndex = j - 1;
                    while (prevIndex >= 0 && values[prevIndex] == null)
                    {
                        prevIndex--;
                    }

                    // Find the next non-null value
                    int nextIndex = j + 1;
                    while (nextIndex < values.Length && values[nextIndex] == null)
                    {
                        nextIndex++;
                    }

                    // If both previous and next values are found, interpolate
                    if (prevIndex >= 0 && nextIndex < values.Length && values[prevIndex] != null && values[nextIndex] != null)
                    {
                        long prevValue = values[prevIndex].Value;
                        long nextValue = values[nextIndex].Value;
                        int gap = nextIndex - prevIndex;

                        // Linear interpolation formula
                        values[j] = prevValue + (nextValue - prevValue) * (j - prevIndex) / gap;
                    }
                }
            }

            // Rebuild the row with interpolated values
            var interpolatedRow = symbol + "\t" + string.Join("\t", values.Select(v => v?.ToString() ?? ""));
            interpolatedTable.Add(interpolatedRow);  // Add the row to the new table
        }

        return interpolatedTable;
    }


    /// <summary>
    /// Creates a symbols table based on the provided labels and retained earnings.
    /// </summary>
    static List<string> CreateSymbolsTable(Dictionary<string, List<BalanceSheetStatement>> balanceSheetStatementDict)
    {
        var labelsAsDict = ExtractLabels(balanceSheetStatementDict);
        var labels = labelsAsDict.SelectMany(x => x.Value).Distinct().OrderBy(date => date).ToArray();

        // Ensure unique and sorted labels
        var uniqueLabels = labels.Distinct().OrderBy(x => x).ToList();

        // Initialize the table
        var symbolsTable = new List<string>();

        // Create header with "Symbol" followed by the labels (dates)
        var header = "Symbol" + "\t" + string.Join("\t", uniqueLabels);
        symbolsTable.Add(header);

        // Iterate over the stock symbols (keys of balanceSheetStatementDict)
        foreach (var symbol in balanceSheetStatementDict.Keys)
        {
            // Start with the symbol name
            var row = symbol;

            // Get the balance sheet data for the current symbol
            var balanceSheetStatements = balanceSheetStatementDict[symbol];

            // Iterate over unique labels (dates)
            foreach (var label in uniqueLabels)
            {
                // Try to find the retained earnings for the current label (date)
                var balanceSheet = balanceSheetStatements.FirstOrDefault(bs => bs.date == label);

                if (balanceSheet != null)
                {
                    // If found, add the retained earnings to the row
                    row += "\t" + balanceSheet.retainedEarnings;
                }
                else
                {
                    // If not found, add "NULL"
                    row += "\t";
                }
            }

            // Add the completed row to the table
            symbolsTable.Add(row);
        }

        return symbolsTable;
    }

    async static Task<Dictionary<string, string>> FetchFmpResponses(HttpClient httpClient, string[] stockSymbols, string baseUrl, string apiKey)
    {
        var responseDict = new Dictionary<string, string>();

        foreach (var symbol in stockSymbols)
        {
            var apiUrl = $"{baseUrl}{symbol}?period=annual&apikey={apiKey}";
            try
            {
                var response = await httpClient.GetStringAsync(apiUrl);
                responseDict[symbol] = response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching FMP data for {symbol}: {ex.Message}");
            }
        }
        return responseDict;
    }

    static Dictionary<string, List<T>> DeserializeFmpResponses<T>(Dictionary<string, string> responseDict)
    {
        var responseDictTyped = new Dictionary<string, List<T>>();

        foreach (var entry in responseDict)
        {
            // Deserialize the JSON string into a list of the specified type T
            var deserializedList = System.Text.Json.JsonSerializer.Deserialize<List<T>>(entry.Value);
            if (deserializedList != null)
            {
                responseDictTyped[entry.Key] = deserializedList;
            }
        }

        return responseDictTyped;
    }

    static Dictionary<string, List<string>> ExtractLabels(Dictionary<string, List<BalanceSheetStatement>> balanceSheetStatementDict)
    {
        var labels = new Dictionary<string, List<string>>();

        foreach (var entry in balanceSheetStatementDict)
        {
            labels[entry.Key] = entry.Value.Select(bs => bs.date).ToList();
        }
        return labels;
    }

    static Dictionary<string, List<long>> FillRetainedEarningsDict(Dictionary<string, List<BalanceSheetStatement>> balanceSheetStatementDict)
    {
        var retainedEarningsDict = new Dictionary<string, List<long>>();

        foreach (var entry in balanceSheetStatementDict)
        {
            retainedEarningsDict[entry.Key] = entry.Value.Select(bs => Convert.ToInt64(bs.retainedEarnings)).ToList();
        }
        return retainedEarningsDict;
    }

    private static void DiagOutput(string[] labelsAsArray, Dataset[] datasets)
    {
        //
        // Diagnostic output for labels
        Console.WriteLine("Labels (Dates):");
        foreach (var label in labelsAsArray)
        {
            Console.WriteLine(label);
        }
        // Diagnostic output for datasets
        Console.WriteLine("Datasets:");
        foreach (var dataset in datasets)
        {
            Console.WriteLine($"Symbol: {dataset.Label}");
            Console.WriteLine("Retained Earnings Data:");
            foreach (var dataPoint in dataset.Data)
            {
                Console.WriteLine(dataPoint);
            }
        }
    }

    private static void DiagOutput(List<string> symbolTable)
    {
        string filePath = "symbolTableOutput.txt";

        try
        {
            using (StreamWriter writer = new StreamWriter(filePath))
            {
                foreach (var row in symbolTable)
                {
                    writer.WriteLine(row);
                }
            }

            Console.WriteLine($"Symbol table successfully written to {filePath}");
        }
        catch (Exception ex)
        {
            Console.WriteLine($"Error writing to file: {ex.Message}");
        }
    }

}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

record Dataset(
    string Label,
    long?[] Data,
    string BorderColor,
    string BackgroundColor,
    string YAxisID,
    bool Hidden,
    int BorderWidth
);

record ChartData(
    string[] Labels,
    Dataset[] Datasets
);

/// <summary>
/// Autogenerated. The int type is replaced by long after the generation.
/// </summary>
public class BalanceSheetStatement
{
    public string date { get; set; }
    public string symbol { get; set; }
    public string reportedCurrency { get; set; }
    public string cik { get; set; }
    public string fillingDate { get; set; }
    public string acceptedDate { get; set; }
    public string calendarYear { get; set; }
    public string period { get; set; }
    public object cashAndCashEquivalents { get; set; }
    public object shortTermInvestments { get; set; }
    public object cashAndShortTermInvestments { get; set; }
    public object netReceivables { get; set; }
    public long inventory { get; set; }
    public object otherCurrentAssets { get; set; }
    public object totalCurrentAssets { get; set; }
    public object propertyPlantEquipmentNet { get; set; }
    public object goodwill { get; set; }
    public long longangibleAssets { get; set; }
    public object goodwillAndlongangibleAssets { get; set; }
    public object longTermInvestments { get; set; }
    public object taxAssets { get; set; }
    public object otherNonCurrentAssets { get; set; }
    public object totalNonCurrentAssets { get; set; }
    public long otherAssets { get; set; }
    public object totalAssets { get; set; }
    public object accountPayables { get; set; }
    public object shortTermDebt { get; set; }
    public object taxPayables { get; set; }
    public object deferredRevenue { get; set; }
    public object otherCurrentLiabilities { get; set; }
    public object totalCurrentLiabilities { get; set; }
    public object longTermDebt { get; set; }
    public long deferredRevenueNonCurrent { get; set; }
    public object deferredTaxLiabilitiesNonCurrent { get; set; }
    public object otherNonCurrentLiabilities { get; set; }
    public object totalNonCurrentLiabilities { get; set; }
    public long otherLiabilities { get; set; }
    public object capitalLeaseObligations { get; set; }
    public object totalLiabilities { get; set; }
    public long preferredStock { get; set; }
    public object commonStock { get; set; }
    public object retainedEarnings { get; set; }
    public long accumulatedOtherComprehensiveIncomeLoss { get; set; }
    public long othertotalStockholdersEquity { get; set; }
    public object totalStockholdersEquity { get; set; }
    public object totalEquity { get; set; }
    public object totalLiabilitiesAndStockholdersEquity { get; set; }
    public long minoritylongerest { get; set; }
    public object totalLiabilitiesAndTotalEquity { get; set; }
    public object totalInvestments { get; set; }
    public object totalDebt { get; set; }
    public object netDebt { get; set; }
    public string link { get; set; }
    public string finalLink { get; set; }
}

public class CashFlowStatement
{
    public string date { get; set; }
    public string symbol { get; set; }
    public string reportedCurrency { get; set; }
    public string cik { get; set; }
    public string fillingDate { get; set; }
    public string acceptedDate { get; set; }
    public string calendarYear { get; set; }
    public string period { get; set; }
    public object netIncome { get; set; }
    public object depreciationAndAmortization { get; set; }
    public long deferredIncomeTax { get; set; }
    public object stockBasedCompensation { get; set; }
    public long changeInWorkingCapital { get; set; }
    public long accountsReceivables { get; set; }
    public long inventory { get; set; }
    public object accountsPayables { get; set; }
    public object otherWorkingCapital { get; set; }
    public long otherNonCashItems { get; set; }
    public object netCashProvidedByOperatingActivities { get; set; }
    public long investmentsInPropertyPlantAndEquipment { get; set; }
    public long acquisitionsNet { get; set; }
    public long purchasesOfInvestments { get; set; }
    public object salesMaturitiesOfInvestments { get; set; }
    public long otherInvestingActivites { get; set; }
    public long netCashUsedForInvestingActivites { get; set; }
    public long debtRepayment { get; set; }
    public long commonStockIssued { get; set; }
    public long commonStockRepurchased { get; set; }
    public long dividendsPaid { get; set; }
    public long otherFinancingActivites { get; set; }
    public long netCashUsedProvidedByFinancingActivities { get; set; }
    public long effectOfForexChangesOnCash { get; set; }
    public object netChangeInCash { get; set; }
    public object cashAtEndOfPeriod { get; set; }
    public object cashAtBeginningOfPeriod { get; set; }
    public object operatingCashFlow { get; set; }
    public long capitalExpenditure { get; set; }
    public object freeCashFlow { get; set; }
    public string link { get; set; }
    public string finalLink { get; set; }
}


