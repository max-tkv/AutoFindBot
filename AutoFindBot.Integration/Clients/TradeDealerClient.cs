using AutoFindBot.Abstractions.HttpClients;
using AutoFindBot.Exceptions;
using AutoFindBot.Integration.Models.TradeDealerClient;
using AutoFindBot.Models.TradeDealer;
using AutoFindBot.Utils.Http;
using AutoMapper;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AutoFindBot.Integration.Clients;

public class TradeDealerClient : JsonHttpApiClient, ITradeDealerClient
{
    private readonly IConfiguration _configuration;
    private readonly IMapper _mapper;
    private readonly ILogger<TradeDealerClient> _logger;

    public TradeDealerClient(HttpClient httpClient, IConfiguration configuration, IMapper mapper,
        ILogger<TradeDealerClient> logger) : base(httpClient)
    {
        _configuration = configuration;
        _mapper = mapper;
        _logger = logger;
    }

    private string GetMethodPath(string key) => _configuration[$"{nameof(TradeDealerClient)}:{key}"];
    
    public async Task<TradeDealerResult> GetAutoByFilterAsync(GetAutoByFilter filter)
    {
        try
        {
            var path = $"{GetMethodPath("GetByFilterMethod")}" +
                       $"?priceMin={filter.PriceMin}" +
                       $"&priceMax={filter.PriceMax}" +
                       $"&order=publishedAt" +
                       $"&reversed=true" +
                       $"&page=1" +
                       $"&filterOrder=price" +
                       $"&carType=used" +
                       $"&_token=Xa94WlCOJAieVotE&_tokenProduct=GxWaWtscgmTRQUeP" +
                       $"&_version=desktop";
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