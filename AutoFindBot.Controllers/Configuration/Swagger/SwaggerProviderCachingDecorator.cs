using System.Collections.Concurrent;
using AutoFindBot.Controllers.Extensions;
using Microsoft.AspNetCore.Mvc.ApiExplorer;
using Microsoft.OpenApi.Models;
using Swashbuckle.AspNetCore.Swagger;
using Swashbuckle.AspNetCore.SwaggerGen;

namespace AutoFindBot.Controllers.Configuration.Swagger;

public class SwaggerProviderCachingDecorator : ISwaggerProvider, IAsyncSwaggerProvider
{
    private static readonly ConcurrentDictionary<string, OpenApiDocument> Cache = new();
    private static readonly ConcurrentDictionary<string, OpenApiDocument> CacheAsync = new();
    
    private readonly SwaggerGenerator _swaggerGenerator;

    public SwaggerProviderCachingDecorator(SwaggerGeneratorOptions options,
        IApiDescriptionGroupCollectionProvider apiDescriptionsProvider,
        ISchemaGenerator schemaGenerator)
    {
        _swaggerGenerator = new SwaggerGenerator(options, apiDescriptionsProvider, schemaGenerator);
    }
    
    public OpenApiDocument GetSwagger(string documentName, string host = null, string basePath = null)
    {
        // https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/356
        var cacheKey = $"{documentName}_{host}_{basePath}";
        return Cache.GetOrAdd(cacheKey, _ => _swaggerGenerator.GetSwagger(documentName, host, basePath));
    }

    public async Task<OpenApiDocument> GetSwaggerAsync(string documentName, string host = null, string basePath = null)
    {
        // https://github.com/domaindrivendev/Swashbuckle.AspNetCore/issues/356
        var cacheKey = $"{documentName}_{host}_{basePath}";
        return await CacheAsync.GetOrAddAsync(cacheKey, async _ => await _swaggerGenerator.GetSwaggerAsync(documentName, host, basePath));
    }
}