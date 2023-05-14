using Microsoft.Extensions.DependencyInjection;

namespace AutoFindBot.Integration.Youla.Extensions;

public static class RegisterMessageHandlersExtension
{
    internal static IHttpClientBuilder AddHttpMessageHandlers (
        this IHttpClientBuilder builder, 
        List<Func<IServiceProvider, DelegatingHandler>> handlers)
    {
        foreach (var handler in handlers)
        {
            builder.AddHttpMessageHandler(handler);
        }

        return builder;
    }
}