using AutoFindBot.Abstractions.HttpClients;
using AutoFindBot.Integration.AutoRu.Invariants;
using AutoFindBot.Integration.AutoRu.Models;
using AutoFindBot.Integration.AutoRu.Options;
using AutoFindBot.Models.AutoRu;
using AutoFindBot.Utils.Http;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace AutoFindBot.Integration.AutoRu;

public class AutoRuHttpApiClient : JsonHttpApiClient, IAutoRuHttpApiClient
{
    private readonly ILogger<AutoRuHttpApiClient> _logger;
    private readonly IMapper _mapper;
    private readonly AutoRuHttpApiClientOptions _options;

    public AutoRuHttpApiClient(
        HttpClient httpClient, 
        ILogger<AutoRuHttpApiClient> logger,
        IMapper mapper,
        AutoRuHttpApiClientOptions options) : base(httpClient)
    {
        _logger = logger;
        _mapper = mapper;
        _options = options;
    }
    
    public async Task<List<AutoRuResult>> GetAutoByFilterAsync(AutoRuFilter filter)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(filter);

            var path = _options?.BaseUrl + _options?.GetAutoByFilterQuery
                .Replace(AutoRuHttpApiClientInvariants.PriceMin, filter.PriceMin)
                .Replace(AutoRuHttpApiClientInvariants.PriceMax, filter.PriceMax);
            var response = await SendAsync(path, HttpMethod.Get);
            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode == false)
            {
                throw new Exception($"Произошла ошибка: {content}");
            }
            
            var avitoResponse = GetObjectFromResponse<AutoRuResponse>(content);

            ArgumentNullException.ThrowIfNull(avitoResponse);

            return default;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"{nameof(AutoRuHttpApiClient)}: {e.Message}");
            return new List<AutoRuResult>();
        }
    }
}