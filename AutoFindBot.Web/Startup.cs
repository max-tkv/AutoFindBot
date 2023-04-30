using System;
using AutoMapper;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AutoFindBot.Controllers.Configuration.Swagger;
using AutoFindBot.Controllers.Extensions;
using AutoFindBot.Integration.AutoRu.Extensions;
using AutoFindBot.Integration.AutoRu.Mappings;
using AutoFindBot.Integration.Avito.Extensions;
using AutoFindBot.Integration.Avito.Mappings;
using AutoFindBot.Integration.Extensions;
using AutoFindBot.Integration.KeyAutoProbeg.Extensions;
using AutoFindBot.Integration.Mappings;
using AutoFindBot.Integration.RuCaptcha.Extensions;
using AutoFindBot.Mappings;
using AutoFindBot.Services;
using AutoFindBot.Storage;

namespace AutoFindBot.Web
{
    public class Startup
    {
        private readonly IConfiguration _configuration;

        public Startup(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public void ConfigureServices(IServiceCollection services)
        {
            services.AddControllers().AddNewtonsoftJson();
            services.AddSqlStorage(options =>
            {
                options.UseSqlServer(_configuration.GetConnectionString("Db") ?? "");
                options.UseQueryTrackingBehavior(QueryTrackingBehavior.NoTracking);
            });
            
            services.AddSingleton(new MapperConfiguration(mc =>
            {
                mc.AddProfile(new TradeDealerHttpApiClientMappingProfile());
                mc.AddProfile(new TracksServiceMappingProfile());
                mc.AddProfile(new CheckerNewAutoServiceMappingProfile());
                mc.AddProfile(new AvitoHttpApiClientMappingProfile());
                mc.AddProfile(new AutoRuHttpApiClientMappingProfile());
            }).CreateMapper());

            services.AddDomain();
            services.AddHostedServices();
            services.AddConfig(_configuration);
            
            services.AddTradeDealerHttpApiClient(_configuration);
            services.AddKeyAutoProbegHttpApiClient(_configuration);
            services.AddAvitoHttpApiClient(_configuration);
            services.AddAutoRuHttpApiClient(_configuration);
            services.AddRuCaptchaHttpApiClient(_configuration);

            services.RegisterApiWithSwagger(_configuration);
            
            var serviceProvider = services.BuildServiceProvider();
            ServiceLocator.SetProvider(serviceProvider);

            serviceProvider.GetRequiredService<TelegramBot>().GetBot().Wait();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider, 
            ILogger<Startup> logger, IHostApplicationLifetime lifetime)
        {
            MigrationsRunner.ApplyMigrations(logger, serviceProvider, "AutoFindBot.Web").Wait();
            RegisterLifetimeLogging(lifetime, logger);
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            var swaggerOptions = serviceProvider.GetRequiredService<IOptions<AppSwaggerOptions>>();
            if (swaggerOptions.Value.UseSwagger)
            {
                var apiVersionDescriptionProvider = serviceProvider.GetRequiredService<IApiVersionDescriptionProvider>();
                app.UseSwaggerWithVersion(apiVersionDescriptionProvider);
            }

            //serviceProvider.GetRequiredService<TelegramBot>().GetBot().Wait();
            app.UseRouting();

            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });
        }
        
        private static void RegisterLifetimeLogging(IHostApplicationLifetime lifetime, ILogger<Startup> logger)
        {
            lifetime.ApplicationStarted.Register(() => logger.LogInformation("App started"));
            lifetime.ApplicationStopped.Register(() => logger.LogInformation("App stopped"));
        }
    }
}