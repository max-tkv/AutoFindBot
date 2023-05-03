using AutoFindBot.Abstractions.HttpClients;
using AutoFindBot.Integration.Avito.Invariants;
using AutoFindBot.Integration.Avito.Models;
using AutoFindBot.Integration.Avito.Options;
using AutoFindBot.Models.Avito;
using AutoFindBot.Utils.Http;
using AutoMapper;
using Microsoft.Extensions.Logging;

namespace AutoFindBot.Integration.Avito;

public class AvitoHttpApiClient : JsonHttpApiClient, IAvitoHttpApiClient
{
    private readonly ILogger<AvitoHttpApiClient> _logger;
    private readonly AvitoHttpApiClientOptions? _options;
    private readonly IMapper _mapper;

    public AvitoHttpApiClient(
        IMapper mapper,
        HttpClient httpClient,
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
            if (!IsActive())
            {
                throw new Exception($"{nameof(AvitoHttpApiClient)} отключен.");
            }
            
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
            
            var avitoResponse = GetObjectFromResponse<AvitoRootResponse>(content);

            ArgumentNullException.ThrowIfNull(avitoResponse.Result.Items);
            
            return _mapper.Map<List<AvitoResult>>(avitoResponse.Result.Items);
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"{nameof(AvitoHttpApiClient)}: {e.Message}");
            throw;
        }
    }

    public bool IsActive()
    {
        return _options.Active;
    }
}