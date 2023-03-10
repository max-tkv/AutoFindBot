using AutoFindBot.Abstractions.HttpClients;
using AutoFindBot.Exceptions;
using AutoFindBot.Integration.Invariants;
using AutoFindBot.Integration.Models;
using AutoFindBot.Integration.Options;
using AutoFindBot.Models.TradeDealer;
using AutoFindBot.Utils.Http;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AutoFindBot.Integration;

public class TradeDealerHttpApiClient : JsonHttpApiClient, ITradeDealerHttpApiClient
{
    private readonly IMapper _mapper;
    private readonly ILogger<TradeDealerHttpApiClient> _logger;
    private readonly TradeDealerHttpApiClientOptions? _options;

    public TradeDealerHttpApiClient(
        HttpClient httpClient,
        IMapper mapper,
        TradeDealerHttpApiClientOptions options,
        ILogger<TradeDealerHttpApiClient> logger) : base(httpClient)
    {
        _options = options;
        _mapper = mapper;
        _logger = logger;
    }

    public async Task<TradeDealerResult> GetAutoByFilterAsync(TradeDealerFilter filter)
    {
        try
        {
            var path = _options.BaseUrl + _options.GetAutoByFilterQuery
                .Replace(TradeDealerHttpApiClientInvariants.PriceMin, filter.PriceMin)
                .Replace(TradeDealerHttpApiClientInvariants.PriceMax, filter.PriceMax);
            var response = await SendAsync(path, HttpMethod.Get);
            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode == false)
            {
                throw new TradeDealerClientException($"Произошла ошибка: {content}");
            }
            
            var tradeDealerResponse = GetObjectFromResponse<TradeDealerResponse>(content);

            ArgumentNullException.ThrowIfNull(tradeDealerResponse.CarInfoResponses);
    
            return _mapper.Map<TradeDealerResult>(tradeDealerResponse);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Ошибка: {e.Message}");
            throw;
        }
    }
}