using System.Reflection;
using AutoFindBot.Controllers.Api.V1;
using AutoFindBot.Controllers.Configuration.Swagger;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Serialization;

namespace AutoFindBot.Controllers.Extensions;

public static class RegisterApiDependenciesExtension
{
    /// <summary>
    /// Добавление API приложения + Swagger
    /// </summary>
    /// <param name="services"></param>
    /// <param name="configuration"></param>
    /// <returns></returns>
    public static IServiceCollection RegisterApiWithSwagger(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddMvc()
            .AddApi()
            .AddControllersAsServices()
            .AddNewtonsoftJson(options =>
            {
                options.SerializerSettings.NullValueHandling = NullValueHandling.Include;
                options.SerializerSettings.Converters.Add(
                    new StringEnumConverter(new CamelCaseNamingStrategy(),
                        false));
            });
            
        services.AddSwagger(configuration);
        services.Configure<AppSwaggerOptions>(configuration);

        return services;
    }
    
    /// <summary>
    /// Добавление API приложения
    /// </summary>
    /// <param name="builder">IMvcBuilder</param>
    /// <returns>IMvcBuilder</returns>
    public static IMvcBuilder AddApi(this IMvcBuilder builder)
    {
        return builder.AddApplicationPart(Assembly.GetAssembly(typeof(AutoFindBotController)));
    }
}