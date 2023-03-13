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
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;
using AutoFindBot.Controllers;
using AutoFindBot.HostedServices;
using AutoFindBot.Integration.Avito.Extensions;
using AutoFindBot.Integration.Avito.Mappings;
using AutoFindBot.Integration.Extensions;
using AutoFindBot.Integration.KeyAutoProbeg.Extensions;
using AutoFindBot.Integration.Mappings;
using AutoFindBot.Mappings;
using AutoFindBot.Services;
using AutoFindBot.Storage;
using AutoFindBot.Web.Configuration;
using AutoFindBot.Web.Configuration.Swagger;

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
            }).CreateMapper());

            services.AddDomain();
            services.AddHostedServices();
            services.AddConfig(_configuration);
            
            services.AddTradeDealerHttpApiClient(_configuration);
            services.AddKeyAutoProbegHttpApiClient(_configuration);
            services.AddAvitoHttpApiClient(_configuration);
            
            services.AddMvc()
                .AddApi()
                .AddValidators()
                .AddControllersAsServices()
                .AddNewtonsoftJson(options =>
                {
                    options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
                    options.SerializerSettings.Converters.Add(new StringEnumConverter(new CamelCaseNamingStrategy(),
                        false));
                });
            
            services.AddSwagger(_configuration);
            services.Configure<AppSwaggerOptions>(_configuration);
            
            var serviceProvider = services.BuildServiceProvider();
            ServiceLocator.SetProvider(serviceProvider);

            serviceProvider.GetRequiredService<TelegramBot>().GetBot().Wait();
        }

        public void Configure(IApplicationBuilder app, IWebHostEnvironment env, IServiceProvider serviceProvider, 
            ILogger<Startup> logger, IHostApplicationLifetime lifetime, IOptions<AppSwaggerOptions> swaggerOptions,
            IApiVersionDescriptionProvider apiVersionDescriptionProvider)
        {
            MigrationsRunner.ApplyMigrations(logger, serviceProvider, "AutoFindBot.Web").Wait();
            RegisterLifetimeLogging(lifetime, logger);
            
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
            }

            if (swaggerOptions.Value.UseSwagger)
            {
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