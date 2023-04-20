using AutoFindBot.Abstractions.HttpClients;
using AutoFindBot.Integration.AutoRu.Exceptions;
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

    public async Task<AutoRuResult> GetAutoByFilterAsync(AutoRuFilter filter)
    {
        try
        {
            if (!IsActive())
            {
                _logger.LogInformation($"{nameof(AutoRuHttpApiClient)} отключен.");
                return new AutoRuResult();
            }
            
            ArgumentNullException.ThrowIfNull(filter);

            var path = _options?.BaseUrl + _options?.GetAutoByFilterQuery;
            var request = new AutoRuRequest()
            {
                Category = "cars",
                Section = "used",
                PriceFrom = Int32.Parse(filter.PriceMin),
                PriceTo = Int32.Parse(filter.PriceMax),
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
        catch (Exception e)
        {
            _logger.LogWarning(e, $"{nameof(AutoRuHttpApiClient)}: {e.Message}");
            return new AutoRuResult();
        }
    }

    public bool IsActive()
    {
        return _options.Active;
    }
}