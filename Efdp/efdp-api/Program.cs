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

            // TODO
            //List<string> symbolsTable = createSymbolsTable(labels, retainedEarningsDict);

            // Prepare the datasets based on the retained earnings data
            var colors = Helpers.GetRandomRgbColors(stockSymbols.Length);
            var datasets = retainedEarningsDict.Select((entry, index) => new Dataset(
                Label: entry.Key,
                Data: entry.Value.ToArray(),
                BorderColor: colors[index],
                BackgroundColor: colors[index].Replace("1)", "0.2)"),
                YAxisID: "y-axis-1",
                Hidden: false,
                BorderWidth: 1
            )).ToArray();

            // Create the RetainedEarningsResponse
            var retainedEarningsData2 = new RetainedEarningsResponse(
                Labels: labels,
                Datasets: datasets
            );

            DiagOutput(labels, datasets);

            return Results.Ok(retainedEarningsData2);
        })
     .WithName("GetBalanceSheetStatement")
     .WithOpenApi();

        app.Run();
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
}

record WeatherForecast(DateOnly Date, int TemperatureC, string? Summary)
{
    public int TemperatureF => 32 + (int)(TemperatureC / 0.5556);
}

record Dataset(
    string Label,
    long[] Data,
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


