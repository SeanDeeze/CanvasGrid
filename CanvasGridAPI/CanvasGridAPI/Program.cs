using CanvasGridAPI.Models;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.SpaServices.AngularCli;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog;
using NLog.Web;
using System;

Logger logger = LogManager.Setup().LoadConfigurationFromAppSettings().GetCurrentClassLogger();
logger.Debug("init main");

try
{
    string CORS_POLICY = "CorsPolicy";

    WebApplicationBuilder builder = WebApplication.CreateBuilder(args);

    // NLog: Setup NLog for Dependency injection
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Debug);
    builder.Host.UseNLog();

    builder.Services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy",
            builder => builder.WithOrigins("http://localhost:56885")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
    });

    string DBConnectionString = builder.Configuration.GetConnectionString("DBConnection");
    builder.Services.AddDbContext<GridContext>(options => options.UseSqlServer(DBConnectionString));

    builder.Services.AddControllers();
    builder.Services.AddEndpointsApiExplorer();
    builder.Services.AddSwaggerGen();
    builder.Services.AddSpaStaticFiles(configuration =>
    {
        configuration.RootPath = "./CanvasGridUI/dist";
    });

    WebApplication app = builder.Build();

    app.UseCors(CORS_POLICY);
    app.UseRouting();
    app.MapControllers();

    if (app.Environment.IsDevelopment())
    {
        app.UseDeveloperExceptionPage();
        app.UseSwagger();
        app.UseSwaggerUI(options =>
        {
            options.SwaggerEndpoint("/swagger/v1/swagger.json", "v1");
            options.RoutePrefix = string.Empty;
        });
    }
    else
    {
        app.UseExceptionHandler("/Error");
        app.UseStaticFiles();
        app.UseSpaStaticFiles();
        app.UseSpa(spa =>
        {
            spa.Options.SourcePath = "./CanvasGridUI/dist";
        });
    }

    app.Run();
}
catch (Exception exception)
{
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    LogManager.Shutdown();
}