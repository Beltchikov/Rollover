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
            var balanceSheetResponseDict = await FetchBalanceSheetResponses(httpClient, stockSymbols, baseUrl, apiKey);

            // Step 2: Serialize the responses into balanceSheetStatementDict
            var balanceSheetStatementDict = DeserializeBalanceSheetResponses(balanceSheetResponseDict);

            // Step 3: Extract all date attributes into the labels variable
            var labelsAsDict = ExtractLabels(balanceSheetStatementDict);

            // Step 4: Fill out the retainedEarningsDict from balanceSheetStatementDict
            var retainedEarningsDict = FillRetainedEarningsDict(balanceSheetStatementDict);

            // Prepare the labels (dates) for the response
            var labels = labelsAsDict.SelectMany(x => x.Value).Distinct().OrderBy(date => date).ToArray();

            // Create symbols table
            List<string> symbolsTable = CreateSymbolsTable(labels, balanceSheetStatementDict);
            List<string> interpolatedSymbolsTable = InterpolateSymbolsTable(symbolsTable);

            //// Prepare the datasets based on the retained earnings data
            //var colors = Helpers.GetRandomRgbColors(stockSymbols.Length);
            //var datasets = retainedEarningsDict.Select((entry, index) => new Dataset(
            //    Label: entry.Key,
            //    Data: entry.Value.ToArray(),
            //    BorderColor: colors[index],
            //    BackgroundColor: colors[index].Replace("1)", "0.2)"),
            //    YAxisID: "y-axis-1",
            //    Hidden: false,
            //    BorderWidth: 1
            //)).ToArray();

            // Extract the header row (which contains the labels/dates)
            var headerRow = interpolatedSymbolsTable[0].Split('\t').Skip(1).ToArray(); // Skip the "Symbol" column

            // Prepare the datasets based on the interpolated symbols table data
            var colors = Helpers.GetRandomRgbColors(interpolatedSymbolsTable.Count - 1); // Skip the header row

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

            // Create the RetainedEarningsResponse
            var retainedEarningsData2 = new RetainedEarningsResponse(
                Labels: labels,
                Datasets: datasets
            );

            //DiagOutput(labels, datasets);
            DiagOutput(interpolatedSymbolsTable);

            return Results.Ok(retainedEarningsData2);
        })
     .WithName("GetBalanceSheetStatement")
     .WithOpenApi();

        app.Run();
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
            //var values = dataColumns.Select(val => val == "NULL" ? (long?)null : long.Parse(val)).ToArray();
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
    static List<string> CreateSymbolsTable(string[] labels, Dictionary<string, List<BalanceSheetStatement>> balanceSheetStatementDict)
    {
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



    /// <summary>
    /// Step 1: Fetch balance sheet responses from the API for each stock symbol.
    /// </summary>
    async static Task<Dictionary<string, string>> FetchBalanceSheetResponses(HttpClient httpClient, string[] stockSymbols, string baseUrl, string apiKey)
    {
        var balanceSheetResponseDict = new Dictionary<string, string>();

        foreach (var symbol in stockSymbols)
        {
            var apiUrl = $"{baseUrl}{symbol}?period=annual&apikey={apiKey}";
            try
            {
                var response = await httpClient.GetStringAsync(apiUrl);
                balanceSheetResponseDict[symbol] = response;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching balance sheet for {symbol}: {ex.Message}");
            }
        }
        return balanceSheetResponseDict;
    }

    /// <summary>
    /// Step 2: Deserialize the API responses into balance sheet statement objects.
    /// </summary>
    static Dictionary<string, List<BalanceSheetStatement>> DeserializeBalanceSheetResponses(Dictionary<string, string> balanceSheetResponseDict)
    {
        var balanceSheetStatementDict = new Dictionary<string, List<BalanceSheetStatement>>();

        foreach (var entry in balanceSheetResponseDict)
        {
            var balanceSheets = System.Text.Json.JsonSerializer.Deserialize<List<BalanceSheetStatement>>(entry.Value);
            if (balanceSheets != null)
            {
                balanceSheetStatementDict[entry.Key] = balanceSheets;
            }
        }
        return balanceSheetStatementDict;
    }

    /// <summary>
    /// Step 3: Extract all date attributes into the labels variable.
    /// </summary>
    static Dictionary<string, List<string>> ExtractLabels(Dictionary<string, List<BalanceSheetStatement>> balanceSheetStatementDict)
    {
        var labels = new Dictionary<string, List<string>>();

        foreach (var entry in balanceSheetStatementDict)
        {
            labels[entry.Key] = entry.Value.Select(bs => bs.date).ToList();
        }
        return labels;
    }

    /// <summary>
    /// Step 4: Fill out the retainedEarningsDict from balanceSheetStatementDict.
    /// </summary>
    static Dictionary<string, List<long>> FillRetainedEarningsDict(Dictionary<string, List<BalanceSheetStatement>> balanceSheetStatementDict)
    {
        var retainedEarningsDict = new Dictionary<string, List<long>>();

        foreach (var entry in balanceSheetStatementDict)
        {
            retainedEarningsDict[entry.Key] = entry.Value.Select(bs => bs.retainedEarnings).ToList();
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

    //private static void DiagOutput(List<string> symbolTable)
    //{
    //    foreach (var row in symbolTable)
    //    {
    //        Console.WriteLine(row);
    //    }
    //}

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

record RetainedEarningsResponse(
    string[] Labels,
    Dataset[] Datasets
);

public class BalanceSheetStatement
{
    public string date { get; set; }
    public string symbol { get; set; }
    public long retainedEarnings { get; set; }
}


