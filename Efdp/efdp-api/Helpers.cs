internal static class Helpers
{

    internal static WebApplication BuildWebApplication()
    {
        WebApplicationBuilder builder = WebApplication.CreateBuilder();

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
            app.UseDeveloperExceptionPage();  
        }

        app.UseHttpsRedirection();

        return app;
    }

    internal static string[] GetRandomRgbColors(int numberOfColors)
    {
        var colors = new string[numberOfColors];
        var rand = new Random();

        // Generate distinct colors (similar to Excel-like charts)
        for (int i = 0; i < numberOfColors; i++)
        {
            // Create random RGB values with high contrast
            var r = rand.Next(0, 256);
            var g = rand.Next(0, 256);
            var b = rand.Next(0, 256);

            colors[i] = $"rgba({r}, {g}, {b}, 1)";
        }

        return colors;
    }
}