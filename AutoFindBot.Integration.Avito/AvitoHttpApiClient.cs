using System.Text.RegularExpressions;
using AutoFindBot.Abstractions.HttpClients;
using AutoFindBot.Entities;
using AutoFindBot.Integration.Avito.Invariants;
using AutoFindBot.Integration.Avito.Models;
using AutoFindBot.Integration.Avito.Options;
using AutoFindBot.Models.Avito;
using AutoFindBot.Models.TradeDealer;
using AutoFindBot.Utils.Http;
using AutoMapper;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AutoFindBot.Integration.Avito;

public class AvitoHttpApiClient : JsonHttpApiClient, IAvitoHttpApiClient
{
    private readonly ILogger<AvitoHttpApiClient> _logger;
    private readonly AvitoHttpApiClientOptions? _options;
    private readonly IMapper _mapper;

    //private const string CarNodePath = "//div[contains(@class, 'iva-item-content-rejJg')]";

    public AvitoHttpApiClient(
        IMapper mapper,
        HttpClient httpClient, 
        IConfiguration configuration, 
        ILogger<AvitoHttpApiClient> logger,
        AvitoHttpApiClientOptions options) : base(httpClient)
    {
        _logger = logger;
        _options = options;
        _mapper = mapper;
    }

    public async Task<List<AvitoResult>> GetAutoByFilterAsync(AvitoFilter filter)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(filter);

            var path = _options?.BaseUrl + _options?.GetAutoByFilterQuery
                .Replace(AvitoHttpApiClientInvariants.PriceMin, filter.PriceMin)
                .Replace(AvitoHttpApiClientInvariants.PriceMax, filter.PriceMax);
            var response = await SendAsync(path, HttpMethod.Get);
            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode == false)
            {
                throw new Exception($"Произошла ошибка: {content}");
            }
            
            var tradeDealerResponse = GetObjectFromResponse<AvitoRootResponse>(content);

            ArgumentNullException.ThrowIfNull(tradeDealerResponse.Result.Items);
            
            return _mapper.Map<List<AvitoResult>>(tradeDealerResponse.Result.Items);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"{nameof(AvitoHttpApiClient)}: {e.Message}");
            return new List<AvitoResult>();
        }
    }
}