var builder = WebApplication.CreateBuilder(args);

// Add services to the container.
// Learn more about configuring Swagger/OpenAPI at https://aka.ms/aspnetcore/swashbuckle
builder.Services.AddEndpointsApiExplorer();
builder.Services.AddSwaggerGen();

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
    var forecast =  Enumerable.Range(1, 5).Select(index =>
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

app.MapGet("/retainedEarnings", (string[] stockSymbols) =>
{
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

