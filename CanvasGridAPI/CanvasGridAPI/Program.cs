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

    builder.Services.AddDbContext<GridContext>(options => options.UseSqlServer(builder.Configuration.GetConnectionString("DBConnection")));
    builder.Services.AddCors(options =>
    {
        options.AddPolicy("CorsPolicy",
            builder => builder.WithOrigins("http://localhost:56885")
                .AllowAnyMethod()
                .AllowAnyHeader()
                .AllowCredentials());
    });

    builder.Services.AddControllers();
    builder.Services.AddSpaStaticFiles(configuration =>
    {
        configuration.RootPath = "./CanvasGridUI/dist";
    });

    // NLog: Setup NLog for Dependency injection
    builder.Logging.ClearProviders();
    builder.Logging.SetMinimumLevel(Microsoft.Extensions.Logging.LogLevel.Trace);
    builder.Host.UseNLog();
    WebApplication app = builder.Build();

    if (app.Environment.IsDevelopment()) 
    { 
        app.UseDeveloperExceptionPage(); 
    }

    app.UseCors(CORS_POLICY);
    app.UseRouting();
    app.UseAuthorization();
    app.UseExceptionHandler("/Error");
    app.MapControllers();

    app.UseStaticFiles();
    app.UseSpaStaticFiles();
    app.UseSpa(spa =>
    {
        spa.Options.SourcePath = "./CanvasGridUI/dist";

        if (app.Environment.IsDevelopment())
        {
            spa.Options.StartupTimeout = new TimeSpan(0, 0, 80);
            spa.UseAngularCliServer(npmScript: "start");
        }
    });

    app.Run();
}
catch (Exception exception)
{
    // NLog: catch setup errors
    logger.Error(exception, "Stopped program because of exception");
    throw;
}
finally
{
    // Ensure to flush and stop internal timers/threads before application-exit (Avoid segmentation fault on Linux)
    LogManager.Shutdown();
}