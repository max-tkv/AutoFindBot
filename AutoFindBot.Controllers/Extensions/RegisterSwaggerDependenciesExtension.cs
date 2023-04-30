using System.Reflection;
using AutoFindBot.Controllers.Api.V1;
using AutoFindBot.Controllers.Configuration.Swagger;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Options;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AutoFindBot.Controllers.Extensions;

public static class RegisterSwaggerDependenciesExtension
{
    public static void AddSwagger(this IServiceCollection serviceCollection, IConfiguration configuration)
    {
        serviceCollection.AddApiVersioning();
        serviceCollection.AddVersionedApiExplorer(
            options =>
            {
                options.GroupNameFormat = "'v'VVV";
                options.SubstituteApiVersionInUrl = true;
            });
        serviceCollection.AddTransient<IConfigureOptions<SwaggerGenOptions>, ConfigureSwaggerOptions>();
        serviceCollection.Replace(ServiceDescriptor.Transient<ISwaggerProvider, SwaggerProviderCachingDecorator>());
        serviceCollection.Replace(ServiceDescriptor.Transient<IAsyncSwaggerProvider, SwaggerProviderCachingDecorator>());
        serviceCollection.AddSwaggerGen(options =>
        {
            //Добавляем документации для контроллеров
            var xmlFile = $"{Assembly.GetAssembly(typeof(AutoFindBotController))?.GetName().Name}.xml";
            var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
            options.IncludeXmlComments(xmlPath);
        });
        
        serviceCollection.AddSwaggerGenNewtonsoftSupport();
        serviceCollection.Configure<AppSwaggerOptions>(configuration);
    }

    public static void UseSwaggerWithVersion(this IApplicationBuilder app, IApiVersionDescriptionProvider provider)
    {
        app.UseSwagger();
        app.UseSwaggerUI(c =>
        {
            foreach (var description in provider.ApiVersionDescriptions)
            {
                c.SwaggerEndpoint(
                    $"/swagger/{description.GroupName}/swagger.json",
                    $"{nameof(AutoFindBot)} API {description.GroupName}");
            }
            
            c.RoutePrefix = "swagger";
        });
    }
}