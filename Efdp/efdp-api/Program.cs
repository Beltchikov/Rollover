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
            // API Key and base URL
            string apiKey = "14e7a22ed6110f130afa41af05599bb6";
            string baseUrl = "https://financialmodelingprep.com/api/v3/balance-sheet-statement/";

            // List to store the balance sheet responses
            var balanceSheetStatementArray = new List<string>();

            // Dictionary to store the retained earnings for each stock symbol
            var retainedEarningsDict = new Dictionary<string, List<long>>();
            var labels = new HashSet<string>(); // HashSet to avoid duplicate labels

            // Iterate over stock symbols and make API calls
            foreach (var symbol in stockSymbols)
            {
                var apiUrl = $"{baseUrl}{symbol}?period=annual&apikey={apiKey}";
                try
                {
                    // Make the API call and get the response
                    var response = await httpClient.GetStringAsync(apiUrl);

                    // Deserialize the JSON response
                    var balanceSheets = System.Text.Json.JsonSerializer.Deserialize<List<BalanceSheetStatement>>(response);
                    if (balanceSheets != null)
                    {
                        retainedEarningsDict[symbol] = new List<long>();

                        // Extract retained earnings and labels (dates)
                        foreach (var balanceSheet in balanceSheets)
                        {
                            retainedEarningsDict[symbol].Add(balanceSheet.retainedEarnings);
                            labels.Add(balanceSheet.date); // Store the date as a label
                        }

                        // Store the raw response (optional)
                        balanceSheetStatementArray.Add(response);
                    }
                }
                catch (Exception ex)
                {
                    // Handle errors (e.g., log them or return a meaningful response)
                    Console.WriteLine($"Error fetching balance sheet for {symbol}: {ex.Message}");
                }
            }

            // Prepare the labels (dates) for the response
            //var sortedLabels = labels.OrderBy(date => date).ToArray(); // Sort the dates to maintain order
            var labelsAsArray = labels.ToArray();

            // Prepare the datasets based on the retained earnings data
            var colors = Helpers.GetRandomRgbColors(stockSymbols.Length);
            var datasets = retainedEarningsDict.Select((entry, index) => new Dataset(
                Label: entry.Key, // The stock symbol
                Data: entry.Value.ToArray(), // Retained earnings data
                BorderColor: colors[index], // Assign a random color to each dataset
                BackgroundColor: colors[index].Replace("1)", "0.2)"), // Transparent background color
                YAxisID: "y-axis-1",
                Hidden: false,
                BorderWidth: 1
            )).ToArray();

            //DiagOutput(labelsAsArray, datasets);

            // Create the RetainedEarningsResponse
            var retainedEarningsData2 = new RetainedEarningsResponse(
                Labels: labelsAsArray,
                Datasets: datasets
            );

            return Results.Ok(retainedEarningsData2);
        })
.WithName("GetBalanceSheetStatement")
.WithOpenApi();

        app.Run();
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


