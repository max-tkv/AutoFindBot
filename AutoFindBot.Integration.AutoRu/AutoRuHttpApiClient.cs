using System.Net.Http.Headers;
using System.Text;
using AutoFindBot.Abstractions.HttpClients;
using AutoFindBot.Exceptions;
using AutoFindBot.Integration.AutoRu.Models;
using AutoFindBot.Integration.AutoRu.Options;
using AutoFindBot.Models.AutoRu;
using AutoFindBot.Models.ConfigurationOptions;
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

    public AutoRuHttpApiClient(
        HttpClient httpClient,
        ILogger<AutoRuHttpApiClient> logger,
        IMapper mapper,
        AutoRuHttpApiClientOptions options,
        IOptions<DefaultFilterOptions> defaultFilterOptions) : base(httpClient)
    {
        _logger = logger;
        _mapper = mapper;
        _options = options;
        _defaultFilterOptions = defaultFilterOptions;
    }

    public async Task<AutoRuResult> GetAllNewAutoAsync()
    {
        try
        {
            NotActiveSourceException.ThrowIfNotActive(nameof(AutoRuHttpApiClient), _options.Active);

            var path = _options?.BaseUrl + _options?.GetAutoByFilterQuery;
            var request = new AutoRuRequest()
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

            var response = await HttpClient.SendAsync(new HttpRequestMessage(HttpMethod.Post, path)
            {
                Content = new StringContent(JsonConvert.SerializeObject(request), Encoding.UTF8, "application/json")
            });
            var content = await response.Content.ReadAsStringAsync();
            var autoRuResponse = JsonConvert.DeserializeObject<AutoRuResponse>(content);
            ArgumentNullException.ThrowIfNull(autoRuResponse);

            return _mapper.Map<AutoRuResult>(autoRuResponse);
        }
        catch (SuccessSolutionAutoRuException ex)
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