using System.Net.Http.Headers;
using System.Text;
using AutoFindBot.Abstractions.HttpClients;
using AutoFindBot.Exceptions;
using AutoFindBot.Integration.AutoRu.Models;
using AutoFindBot.Integration.AutoRu.Options;
using AutoFindBot.Lookups;
using AutoFindBot.Models.AutoRu;
using AutoFindBot.Models.ConfigurationOptions;
using AutoFindBot.Repositories;
using AutoFindBot.Utils.Http;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AutoFindBot.Integration.AutoRu;

public class AutoRuHttpApiClient : HttpApiClient, IAutoRuHttpApiClient
{
    private readonly ILogger<AutoRuHttpApiClient> _logger;
    private readonly IMapper _mapper;
    private readonly AutoRuHttpApiClientOptions _options;
    private readonly IOptions<DefaultFilterOptions> _defaultFilterOptions;
    private readonly ISourceRepository _sourceRepository;

    public AutoRuHttpApiClient(
        HttpClient httpClient,
        ILogger<AutoRuHttpApiClient> logger,
        IMapper mapper,
        AutoRuHttpApiClientOptions options,
        IOptions<DefaultFilterOptions> defaultFilterOptions,
        ISourceRepository sourceRepository) : base(httpClient)
    {
        _logger = logger;
        _mapper = mapper;
        _options = options;
        _defaultFilterOptions = defaultFilterOptions;
        _sourceRepository = sourceRepository;
    }

    public async Task<AutoRuResult> GetAllNewAutoAsync(CancellationToken stoppingToken = default)
    {
        try
        {
            NotActiveSourceException.ThrowIfNotActive(
                await _sourceRepository.GetByTypeAsync(SourceType.AutoRu, stoppingToken));

            var path = _options?.BaseUrl + _options?.GetAutoByFilterQuery;
            var body = new AutoRuRequest()
            {
                Category = "cars",
                Section = "used",
                PriceFrom = _defaultFilterOptions.Value.PriceMin,
                PriceTo = _defaultFilterOptions.Value.PriceMax,
                Sort = "cr_date-desc",
                OutputType = "list",
                GeoId = new List<int>()
                {
                    8
                }
            };
            var request = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
            var response = await HttpClient.PostAsync(path, request, stoppingToken);
            var content = await response.Content.ReadAsStringAsync(stoppingToken);
            var autoRuResponse = JsonConvert.DeserializeObject<AutoRuResponse>(content);
            ArgumentNullException.ThrowIfNull(autoRuResponse);

            return _mapper.Map<AutoRuResult>(autoRuResponse);
        }
        catch (SuccessSolutionException ex)
        {
            SetDefaultHeadersToHttpClient(ex.Headers);
            _logger.LogInformation("Капча успешно решена.");
            return new AutoRuResult();
        }
    }

    private void SetDefaultHeadersToHttpClient(HttpResponseHeaders headers)
    {
        if (HttpClient.DefaultRequestHeaders.Any(x => x.Key == "x-csrf-token"))
        {
            return;
        }
        
        foreach (var header in headers)
        {
            HttpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
        }
    }
}