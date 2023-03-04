using System.Reflection;
using Microsoft.Extensions.DependencyInjection;
using AutoFindBot.Controllers.Api.V1;

namespace AutoFindBot.Controllers;

/// <summary>
/// 
/// </summary>
public static class Entry
{
    /// <summary>
    /// Добавление API приложения
    /// </summary>
    /// <param name="builder">IMvcBuilder</param>
    /// <returns>IMvcBuilder</returns>
    public static IMvcBuilder AddApi(this IMvcBuilder builder) =>
        builder.AddApplicationPart(Assembly.GetAssembly(typeof(AdminController)) ?? throw new AggregateException());

    /// <summary>
    /// Добавление FluentValidation 
    /// </summary>
    /// <param name="builder"></param>
    /// <returns></returns>
    public static IMvcBuilder AddValidators(this IMvcBuilder builder)
    {
        return builder;
    }
}