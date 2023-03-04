using System.ComponentModel;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using AutoFindBot.Abstractions;
using AutoFindBot.Entities;
using AutoFindBot.Exceptions;
using AutoFindBot.Extensions;
using AutoFindBot.Helpers;
using AutoFindBot.Utils.Http;

namespace AutoFindBot.Integration.Clients;

public class AlgoliaClient : JsonHttpApiClient
    //, IAlgoliaClient
{
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    private readonly ILogger<AlgoliaClient> _logger;

    public AlgoliaClient(HttpClient httpClient, IConfiguration configuration, IMapper mapper,
        ILogger<AlgoliaClient> logger) : base(httpClient)
    {
        _configuration = configuration;
        _mapper = mapper;
        _logger = logger;
    }
    
    // private string GetMethodQuery() => $"?x-algolia-agent={_configuration["Algolia:Agent"]}" + 
    //                                   $"&x-algolia-application-id={_configuration["Algolia:AppId"]}" +
    //                                   $"&x-algolia-api-key={_configuration["Algolia:ApiKey"]}";
    //
    // public async Task<AlgoliaSearchResult> FindAsync(string text, Categories category)
    // {
    //     try
    //     {
    //         var query = GetMethodQuery();
    //         var body = new AlgoliaSearchContent()
    //         {
    //             Requests = new List<AlgoliaRequestContent>()
    //             {
    //                 new AlgoliaRequestContent()
    //                 {
    //                     IndexName = "tunefind-prod",
    //                     Params = $"query={text}" +
    //                              $"&facetFilters=(type: {GetTypeForQuery(category)})" +
    //                              $"&attributesToRetrieve=[\"*\"]"
    //                 }
    //             }
    //         };
    //         
    //         var response = await SendAsync(query, HttpMethod.Post, GetContent(body));
    //         var content = await response.Content.ReadAsStringAsync();
    //         if (response.IsSuccessStatusCode == false)
    //         {
    //             throw new TuneFindClientException($"Произошла ошибка: {content}");
    //         }
    //         
    //         var algoliaSearch = GetObjectFromResponse<AlgoliaSearchReasponse>(content);
    //         var result = algoliaSearch.Results;
    //         
    //         ArgumentNullException.ThrowIfNull(result);
    //         Guard.NotNull(result.FirstOrDefault()?.Hits?.FirstOrDefault(), 
    //             $"{category.GetAttribute<DisplayNameAttribute>()?.DisplayName}: не найден");
    //
    //         return _mapper.Map<AlgoliaSearchResult>(algoliaSearch);
    //     }
    //     catch (Exception e)
    //     {
    //         _logger.LogError(e, $"Ошибка: {e.Message}");
    //         throw;
    //     }
    // }
    //
    // #region Приватные методы
    //
    // private string GetTypeForQuery(Categories category)
    // {
    //     return category switch
    //     {
    //         Categories.Films => "movie",
    //         Categories.Serials => "show",
    //         Categories.Games => "game",
    //         _ => throw new ArgumentOutOfRangeException(nameof(category), category, null)
    //     };
    // }
    //
    // #endregion
}