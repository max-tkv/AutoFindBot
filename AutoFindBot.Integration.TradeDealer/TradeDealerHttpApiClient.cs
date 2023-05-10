﻿using AutoFindBot.Abstractions.HttpClients;
using AutoFindBot.Exceptions;
using AutoFindBot.Integration.Invariants;
using AutoFindBot.Integration.Models;
using AutoFindBot.Integration.Options;
using AutoFindBot.Models.ConfigurationOptions;
using AutoFindBot.Models.TradeDealer;
using AutoFindBot.Utils.Http;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AutoFindBot.Integration;

public class TradeDealerHttpApiClient : HttpApiClient, ITradeDealerHttpApiClient
{
    private readonly IMapper _mapper;
    private readonly ILogger<TradeDealerHttpApiClient> _logger;
    private readonly TradeDealerHttpApiClientOptions _options;
    private readonly IOptions<DefaultFilterOptions> _defaultFilterOptions;

    public TradeDealerHttpApiClient(
        HttpClient httpClient,
        IMapper mapper,
        TradeDealerHttpApiClientOptions options,
        ILogger<TradeDealerHttpApiClient> logger,
        IOptions<DefaultFilterOptions> defaultFilterOptions) : base(httpClient)
    {
        _options = options;
        _mapper = mapper;
        _logger = logger;
        _defaultFilterOptions = defaultFilterOptions;
    }

    public async Task<TradeDealerResult> GetAllNewAutoAsync()
    {
        NotActiveSourceException.ThrowIfNotActive(nameof(TradeDealerHttpApiClient), _options.Active);

        var path = _options.BaseUrl + _options.GetAutoByFilterQuery
            .Replace(TradeDealerHttpApiClientInvariants.PriceMin, _defaultFilterOptions.Value.PriceMin.ToString())
            .Replace(TradeDealerHttpApiClientInvariants.PriceMax, _defaultFilterOptions.Value.PriceMax.ToString());
        var response = await SendAsync(path, HttpMethod.Get);
        var content = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode == false)
        {
            throw new TradeDealerClientException($"Произошла ошибка: {content}");
        }
            
        var tradeDealerResponse = JsonConvert.DeserializeObject<TradeDealerResponse>(content);

        ArgumentNullException.ThrowIfNull(tradeDealerResponse?.CarInfoResponses);
    
        return _mapper.Map<TradeDealerResult>(tradeDealerResponse);
    }
}