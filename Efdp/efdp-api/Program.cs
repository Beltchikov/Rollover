internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        // Add services to the container.
        // Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
        builder.Services.AddEndpointsApiExplorer();
        builder.Services.AddSwaggerGen();
        builder.Services.AddHttpClient(); 
        // builder.Services.AddCors(options =>
        // {
        //     options.AddPolicy("AllowAllOrigins", builder =>
        //     {
        //         builder.AllowAnyOrigin()
        //                .AllowAnyMethod()
        //                .AllowAnyHeader();
        //     });
        // });

        var app = builder.Build();

        // Configure the HTTP request pipeline.
        if (app.Environment.IsDevelopment())
        {
            app.UseSwagger();
            app.UseSwaggerUI();
        }

        app.UseHttpsRedirection();

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

            // Iterate over stock symbols and make API calls
            foreach (var symbol in stockSymbols)
            {
                var apiUrl = $"{baseUrl}{symbol}?period=annual&apikey={apiKey}";
                try
                {
                    // Make the API call and get the response
                    var response = await httpClient.GetStringAsync(apiUrl);

                    // Store the response in the array
                    balanceSheetStatementArray.Add(response);

                    // Log the response (Console.WriteLine in .NET)
                    Console.WriteLine($"Response for {symbol}: {response}");
                }
                catch (Exception ex)
                {
                    // Handle errors (e.g., log them or return a meaningful response)
                    Console.WriteLine($"Error fetching balance sheet for {symbol}: {ex.Message}");
                }
            }

            var retainedEarningsData = new RetainedEarningsResponse(
                Labels: new[]
                {
            "2009-09-26", "2009-12-31", "2010-06-30", "2010-09-25", "2010-12-31"
                    // add more labels as needed...
                },
                Datasets: new[]
                {
            new Dataset(
                Label: "NVDA",
                Data: new[] { 253146000L, 417118000L, 581090000L, 571813000L, 562536000L },
                BorderColor: "rgba(255, 99, 132, 1)",
                BackgroundColor: "rgba(255, 99, 132, 0.2)",
                YAxisID: "y-axis-1",
                Hidden: false,
                BorderWidth: 1
            ),
            new Dataset(
                Label: "GOOG",
                Data: new[] { 16348000000L, 17913000000L, 19478000000L, 16070000000L, 21699000000L },
                BorderColor: "rgba(54, 162, 235, 1)",
                BackgroundColor: "rgba(54, 162, 235, 0.2)",
                YAxisID: "y-axis-1",
                Hidden: false,
                BorderWidth: 1
            )
                }
            );
            return retainedEarningsData;
        })
        .WithName("GetRetainedEarnings")
        .WithOpenApi();  // Ensure this is included to expose the endpoint in Swagger


        app.Run();
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

