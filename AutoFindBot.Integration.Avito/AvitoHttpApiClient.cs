using AutoFindBot.Abstractions.HttpClients;
using AutoFindBot.Exceptions;
using AutoFindBot.Integration.Avito.Invariants;
using AutoFindBot.Integration.Avito.Models;
using AutoFindBot.Integration.Avito.Options;
using AutoFindBot.Lookups;
using AutoFindBot.Models.Avito;
using AutoFindBot.Models.ConfigurationOptions;
using AutoFindBot.Repositories;
using AutoFindBot.Utils.Http;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AutoFindBot.Integration.Avito;

public class AvitoHttpApiClient : HttpApiClient, IAvitoHttpApiClient
{
    private readonly ILogger<AvitoHttpApiClient> _logger;
    private readonly AvitoHttpApiClientOptions _options;
    private readonly IMapper _mapper;
    private readonly IOptions<DefaultFilterOptions> _defaultFilterOptions;
    private readonly ISourceRepository _sourceRepository;

    public AvitoHttpApiClient(
        IMapper mapper,
        HttpClient httpClient,
        ILogger<AvitoHttpApiClient> logger,
        AvitoHttpApiClientOptions options,
        IOptions<DefaultFilterOptions> defaultFilterOptions,
        ISourceRepository sourceRepository) : base(httpClient)
    {
        _logger = logger;
        _options = options;
        _mapper = mapper;
        _defaultFilterOptions = defaultFilterOptions;
        _sourceRepository = sourceRepository;
    }

    public async Task<List<AvitoResult>> GetAllNewAutoAsync()
    {
        NotActiveSourceException.ThrowIfNotActive(
            await _sourceRepository.GetByTypeAsync(SourceType.Avito));

        var path = _options.BaseUrl + _options.GetAutoByFilterQuery
            .Replace(AvitoHttpApiClientInvariants.PriceMin, _defaultFilterOptions.Value.PriceMin.ToString())
            .Replace(AvitoHttpApiClientInvariants.PriceMax, _defaultFilterOptions.Value.PriceMax.ToString());
        var response = await SendAsync(path, HttpMethod.Get);
        var content = await response.Content.ReadAsStringAsync();
        if (response.IsSuccessStatusCode == false)
        {
            throw new Exception($"Произошла ошибка: {content}");
        }
            
        var avitoResponse = JsonConvert.DeserializeObject<AvitoRootResponse>(content);

        ArgumentNullException.ThrowIfNull(avitoResponse.Result.Items);

        return _mapper.Map<List<AvitoResult>>(avitoResponse.Result.Items);
    }

    public async Task<List<AvitoResult>> GetAutoByFilterAsync(AvitoFilter filter)
    {
        try
        {
            NotActiveSourceException.ThrowIfNotActive(
                await _sourceRepository.GetByTypeAsync(SourceType.Avito));
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
            
            var avitoResponse = JsonConvert.DeserializeObject<AvitoRootResponse>(content);

            ArgumentNullException.ThrowIfNull(avitoResponse.Result.Items);
            
            return _mapper.Map<List<AvitoResult>>(avitoResponse.Result.Items);
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }
    }
}