using System;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using NLog.Extensions.Logging;

namespace AutoFindBot.Web
{
    public class Program
    {
        private static readonly ILogger SystemLogger  = CreateLoggerFactory(withConfiguration: false, withConsole: true)
            .CreateLogger<Program>();
        
        public static async Task Main(string[] args)
        {
            try
            {
                AppDomain.CurrentDomain.UnhandledException += CurrentDomainOnUnhandledException;
                TaskScheduler.UnobservedTaskException += OnTaskSchedulerOnUnobservedTaskException;

                await RunWebApplication(args);
            }
            catch (Exception exception)
            {
                SystemLogger.LogCritical(exception, "Critical error in Main");
                throw;
            }
        }

        private static async Task RunWebApplication(string[] args)
        {
            var builder = WebApplication.CreateBuilder(args);
            builder.Configuration
                .AddJsonFile("appsettings.json", optional: true, reloadOnChange: true)
                .AddJsonFile($"appsettings.{builder.Environment.EnvironmentName}.json", optional: true, reloadOnChange: true);
            builder.Logging
                .AddConfiguration(builder.Configuration.GetSection("Logging"))
                .ClearProviders();
            
            var startup = new Startup(builder.Configuration);
            startup.ConfigureServices(builder.Services);

            var app = builder.Build();
            startup.Configure(app, app.Environment, app.Services, app.Logger, app.Lifetime );

            await app.RunAsync();
        }

        private static void OnTaskSchedulerOnUnobservedTaskException(object sender,
            UnobservedTaskExceptionEventArgs eventArgs)
        {
            eventArgs.SetObserved();
            eventArgs.Exception.Flatten().Handle(ex =>
            {
                SystemLogger.LogError(ex, "Unhandled exception in Task Scheduler handler");
                return true;
            });
        }

        private static void CurrentDomainOnUnhandledException(object sender, UnhandledExceptionEventArgs e)
        {
            var ex = e.ExceptionObject as Exception;
            if (e.IsTerminating)
            {
                SystemLogger.LogCritical(ex, "Service terminating with fatal exception");
                return;
            }
            SystemLogger.LogError(ex, "Unhandled exception in global handler");
        }
        
        private static ILoggerFactory CreateLoggerFactory(bool withConfiguration = true, bool withConsole = false)
        {
            try
            {
                var configuration = new ConfigurationBuilder()
                    .AddJsonFile("appsettings.json", optional: false, reloadOnChange: false)
                    .Build();
                
                var logger = new ServiceCollection()
                    .AddLogging(builder =>
                    {
                        builder.ClearProviders();
                        builder.SetMinimumLevel(LogLevel.Trace);
                        builder.AddNLog();

                        if (withConfiguration)
                        {
                            builder.AddConfiguration(configuration.GetSection("Logging"));
                        }
                    })
                    .BuildServiceProvider()
                    .GetRequiredService<ILoggerFactory>();
                
                return logger;
            }
            catch (Exception exception)
            {
                SystemLogger.LogCritical(exception, "Microsoft.Extensions.Logging.ILogger resolving error");
                throw;
            }
        }
    }
}