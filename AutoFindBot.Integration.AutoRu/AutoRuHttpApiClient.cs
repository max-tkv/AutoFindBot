using AutoFindBot.Abstractions.HttpClients;
using AutoFindBot.Exceptions;
using AutoFindBot.Integration.AutoRu.Exceptions;
using AutoFindBot.Integration.AutoRu.Models;
using AutoFindBot.Integration.AutoRu.Options;
using AutoFindBot.Models.AutoRu;
using AutoFindBot.Models.ConfigurationOptions;
using AutoFindBot.Utils.Http;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AutoFindBot.Integration.AutoRu;

public class AutoRuHttpApiClient : JsonHttpApiClient, IAutoRuHttpApiClient
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

    public async Task<AutoRuResult> GetAutoByFilterAsync(AutoRuFilter filter)
    {
        try
        {
            NotActiveSourceException.ThrowIfNotActive(nameof(AutoRuHttpApiClient), _options.Active);
            ArgumentNullException.ThrowIfNull(filter);

            var path = _options?.BaseUrl + _options?.GetAutoByFilterQuery;
            var request = new AutoRuRequest()
            {
                Category = "cars",
                Section = "used",
                PriceFrom = _defaultFilterOptions.Value.PriceMin,
                PriceTo = _defaultFilterOptions.Value.PriceMax,
                YearFrom = 2010,
                Sort = "price-asc",
                OutputType = "list",
                GeoId = new List<int>()
                {
                    8
                }
            };

            var response = await SendAsync(path, HttpMethod.Post, GetContent(request));
            var content = await response.Content.ReadAsStringAsync();

            var autoRuResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<AutoRuResponse>(content);

            ArgumentNullException.ThrowIfNull(autoRuResponse);

            return _mapper.Map<AutoRuResult>(autoRuResponse);
        }
        catch (AutoRuCaptchaException)
        {
            throw;
        }
        catch (NotActiveSourceException e)
        {
            _logger.LogWarning($"{nameof(AutoRuHttpApiClient)}: {e.Message}");
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError($"{nameof(AutoRuHttpApiClient)}: {e.Message}");
            throw;
        }
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
                PriceFrom = 1,
                PriceTo = 100000000,
                YearFrom = 2010,
                Sort = "price-asc",
                OutputType = "list",
                GeoId = new List<int>()
                {
                    8
                }
            };

            var response = await SendAsync(path, HttpMethod.Post, GetContent(request));
            var content = await response.Content.ReadAsStringAsync();

            var autoRuResponse = Newtonsoft.Json.JsonConvert.DeserializeObject<AutoRuResponse>(content);

            ArgumentNullException.ThrowIfNull(autoRuResponse);

            return _mapper.Map<AutoRuResult>(autoRuResponse);
        }
        catch (AutoRuCaptchaException)
        {
            throw;
        }
        catch (NotActiveSourceException e)
        {
            _logger.LogWarning($"{nameof(AutoRuHttpApiClient)}: {e.Message}");
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError($"{nameof(AutoRuHttpApiClient)}: {e.Message}");
            throw;
        }
    }
}