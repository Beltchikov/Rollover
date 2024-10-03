using System.IO;
using System.Text.Json;
using Generated;

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

            var balanceSheetResponseDict = await FetchFmpResponses(httpClient, stockSymbols, baseUrl, apiKey);
            var balanceSheetStatementDict = DeserializeFmpResponses<BalanceSheetStatement>(balanceSheetResponseDict);
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

            Dictionary<string, string> cashFlowResponseDict = await FetchFmpResponses(httpClient, stockSymbols, baseUrl, apiKey);
            var cashFlowStatementDict = DeserializeFmpResponses<CashFlowStatement>(cashFlowResponseDict);

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




