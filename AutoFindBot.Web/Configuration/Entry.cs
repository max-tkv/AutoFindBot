using System;
using System.IO;
using System.Reflection;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using AutoFindBot.Controllers.Api.V1;
using AutoFindBot.Web.Configuration.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AutoFindBot.Web.Configuration;

public static class Entry
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
        serviceCollection.AddSwaggerGen(c =>
        {
            {   //Добавляем документации для контроллеров
                var xmlFile = $"{Assembly.GetAssembly(typeof(AdminController))?.GetName().Name}.xml";
                var xmlPath = Path.Combine(AppContext.BaseDirectory, xmlFile);
                c.IncludeXmlComments(xmlPath);
            }
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
                    $"Auto Find Bot API {description.GroupName}");
            }
                    
            c.RoutePrefix = "swagger";
        });
    }
}