using AutoFindBot.Abstractions.HttpClients;
using AutoFindBot.Exceptions;
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
    private readonly AvitoHttpApiClientOptions _options;
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

    public async Task<List<AvitoResult>> GetAllNewAutoAsync()
    {
        try
        {
            NotActiveSourceException.ThrowIfNotActive(nameof(AvitoHttpApiClient), _options.Active);

            var path = _options?.BaseUrl + _options?.GetAutoByFilterQuery
                .Replace(AvitoHttpApiClientInvariants.PriceMin, "1")
                .Replace(AvitoHttpApiClientInvariants.PriceMax, "100000000");
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
        catch (NotActiveSourceException e)
        {
            _logger.LogWarning(e, $"{nameof(AvitoHttpApiClient)}: {e.Message}");
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"{nameof(AvitoHttpApiClient)}: {e.Message}");
            throw;
        }
    }

    public async Task<List<AvitoResult>> GetAutoByFilterAsync(AvitoFilter filter)
    {
        try
        {
            NotActiveSourceException.ThrowIfNotActive(nameof(AvitoHttpApiClient), _options.Active);
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
        catch (NotActiveSourceException e)
        {
            _logger.LogWarning(e, $"{nameof(AvitoHttpApiClient)}: {e.Message}");
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"{nameof(AvitoHttpApiClient)}: {e.Message}");
            throw;
        }
    }
}