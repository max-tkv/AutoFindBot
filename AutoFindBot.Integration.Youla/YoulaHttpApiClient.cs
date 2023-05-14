using System.Text;
using AutoFindBot.Abstractions.HttpClients;
using AutoFindBot.Exceptions;
using AutoFindBot.Integration.Youla.Invariants;
using AutoFindBot.Integration.Youla.Models;
using AutoFindBot.Integration.Youla.Options;
using AutoFindBot.Lookups;
using AutoFindBot.Models.ConfigurationOptions;
using AutoFindBot.Models.Youla;
using AutoFindBot.Repositories;
using AutoFindBot.Utils.Http;
using AutoMapper;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace AutoFindBot.Integration.Youla;

public class YoulaHttpApiClient : HttpApiClient, IYoulaHttpApiClient
{
    private readonly IMapper _mapper;
    private readonly ILogger<YoulaHttpApiClient> _logger;
    private readonly YoulaHttpApiClientOptions _options;
    private readonly IOptions<DefaultFilterOptions> _defaultFilterOptions;
    private readonly ISourceRepository _sourceRepository;

    public YoulaHttpApiClient(
        HttpClient httpClient,
        IMapper mapper,
        YoulaHttpApiClientOptions options,
        ILogger<YoulaHttpApiClient> logger,
        IOptions<DefaultFilterOptions> defaultFilterOptions,
        ISourceRepository sourceRepository) : base(httpClient)
    {
        _options = options;
        _mapper = mapper;
        _logger = logger;
        _defaultFilterOptions = defaultFilterOptions;
        _sourceRepository = sourceRepository;
    }

    public async Task<YoulaResult> GetAllNewAutoAsync(CancellationToken stoppingToken = default)
    {
        NotActiveSourceException.ThrowIfNotActive(
            await _sourceRepository.GetByTypeAsync(SourceType.Youla, stoppingToken));

        var path = _options.BaseUrl + _options.GetAutoByFilterQuery;
        var body = GetRequestBody();
        var request = new StringContent(JsonConvert.SerializeObject(body), Encoding.UTF8, "application/json");
        var response = await HttpClient.PostAsync(path, request, stoppingToken);
        var content = await response.Content.ReadAsStringAsync(stoppingToken);
        var youlaResponse = JsonConvert.DeserializeObject<YoulaResponse>(content);

        ArgumentNullException.ThrowIfNull(youlaResponse?.Data?.Feed);

        youlaResponse.Data.Feed.Items.RemoveAll(x => x.Product == null);

        return _mapper.Map<YoulaResult>(youlaResponse);
    }

    #region Приватные методы

    private YoulaRequest GetRequestBody()
    {
        return new YoulaRequest()
        {
            OperationName = RequestInvariants.OperationName,
            Variables = new YoulaRequestVariables()
            {
                Sort = RequestInvariants.Sort,
                Attributes = new List<YoulaRequestVariablesAttribute>()
                {
                    new()
                    {
                        Slug = RequestInvariants.SlugPrice,
                        From = _defaultFilterOptions.Value.PriceMin,
                        To = _defaultFilterOptions.Value.PriceMax
                    },
                    new()
                    {
                        Slug = RequestInvariants.SlugCategories,
                        Value = new List<string> { RequestInvariants.SProbegom }
                    }
                },
                Location = new YoulaRequestVariablesLocation()
                {
                    City = RequestInvariants.CityKursk
                }
            },
            Extensions = new YoulaRequestExtensions()
            {
                PersistedQuery = new YoulaRequestExtensionsPersistedQuery()
                {
                    Version = 1,
                    Sha256Hash = "6e7275a709ca5eb1df17abfb9d5d68212ad910dd711d55446ed6fa59557e2602"
                }
            }
        };
    }

    #endregion
}