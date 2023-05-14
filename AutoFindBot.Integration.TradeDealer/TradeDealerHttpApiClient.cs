using AutoFindBot.Abstractions.HttpClients;
using AutoFindBot.Exceptions;
using AutoFindBot.Integration.Invariants;
using AutoFindBot.Integration.Models;
using AutoFindBot.Integration.Options;
using AutoFindBot.Lookups;
using AutoFindBot.Models.ConfigurationOptions;
using AutoFindBot.Models.TradeDealer;
using AutoFindBot.Repositories;
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
    private readonly ISourceRepository _sourceRepository;

    public TradeDealerHttpApiClient(
        HttpClient httpClient,
        IMapper mapper,
        TradeDealerHttpApiClientOptions options,
        ILogger<TradeDealerHttpApiClient> logger,
        IOptions<DefaultFilterOptions> defaultFilterOptions,
        ISourceRepository sourceRepository) : base(httpClient)
    {
        _options = options;
        _mapper = mapper;
        _logger = logger;
        _defaultFilterOptions = defaultFilterOptions;
        _sourceRepository = sourceRepository;
    }

    public async Task<TradeDealerResult> GetAllNewAutoAsync(CancellationToken stoppingToken = default)
    {
        NotActiveSourceException.ThrowIfNotActive(
            await _sourceRepository.GetByTypeAsync(SourceType.TradeDealer, stoppingToken));

        var path = _options.BaseUrl + _options.GetAutoByFilterQuery
            .Replace(TradeDealerHttpApiClientInvariants.PriceMin, _defaultFilterOptions.Value.PriceMin.ToString())
            .Replace(TradeDealerHttpApiClientInvariants.PriceMax, _defaultFilterOptions.Value.PriceMax.ToString());
        var response = await HttpClient.GetAsync(path, stoppingToken);
        var content = await response.Content.ReadAsStringAsync(stoppingToken);
        if (response.IsSuccessStatusCode == false)
        {
            throw new TradeDealerClientException($"Произошла ошибка: {content}");
        }
            
        var tradeDealerResponse = JsonConvert.DeserializeObject<TradeDealerResponse>(content);

        ArgumentNullException.ThrowIfNull(tradeDealerResponse?.CarInfoResponses);
    
        return _mapper.Map<TradeDealerResult>(tradeDealerResponse);
    }
}