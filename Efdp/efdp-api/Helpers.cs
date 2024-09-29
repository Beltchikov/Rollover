internal static class Helpers
{

    internal static WebApplication Build()
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
        }

        app.UseHttpsRedirection();

        return app;
    }
}