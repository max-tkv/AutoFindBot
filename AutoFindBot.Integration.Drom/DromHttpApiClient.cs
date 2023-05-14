using System.Net.Http.Headers;
using Ardalis.GuardClauses;
using AutoFindBot.Abstractions.HttpClients;
using AutoFindBot.Exceptions;
using AutoFindBot.Integration.Drom.Invariants;
using AutoFindBot.Integration.Drom.Options;
using AutoFindBot.Lookups;
using AutoFindBot.Models.ConfigurationOptions;
using AutoFindBot.Models.Drom;
using AutoFindBot.Repositories;
using AutoFindBot.Utils.Http;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AutoFindBot.Integration.Drom;

public class DromHttpApiClient : HttpApiClient, IDromHttpApiClient
{
    private readonly DromHttpApiClientOptions _options;
    private readonly ILogger<DromHttpApiClient> _logger;
    private readonly IOptions<DefaultFilterOptions> _defaultFilterOptions;
    private readonly ISourceRepository _sourceRepository;

    public DromHttpApiClient(
        HttpClient httpClient,
        DromHttpApiClientOptions options,
        ILogger<DromHttpApiClient> logger,
        IOptions<DefaultFilterOptions> defaultFilterOptions,
        ISourceRepository sourceRepository) : base(httpClient)
    {
        _options = Guard.Against.Null(options, nameof(options));
        _logger = Guard.Against.Null(logger, nameof(logger));
        _defaultFilterOptions = Guard.Against.Null(defaultFilterOptions, nameof(defaultFilterOptions));
        _sourceRepository = Guard.Against.Null(sourceRepository, nameof(sourceRepository));
    }
    
    public async Task<List<DromResult>> GetAllNewAutoAsync(CancellationToken stoppingToken = default)
    {
        NotActiveSourceException.ThrowIfNotActive(
            await _sourceRepository.GetByTypeAsync(SourceType.Drom, stoppingToken));

        try
        {
            var path = _options?.BaseUrl + _options?.GetAutoByFilterQuery
                .Replace(DromHttpApiClientInvariants.PriceMin, _defaultFilterOptions.Value.PriceMin.ToString())
                .Replace(DromHttpApiClientInvariants.PriceMax,
                    _defaultFilterOptions.Value.PriceMax.ToString());
            var response = await HttpClient.GetAsync(path, stoppingToken);
            var content = await response.Content.ReadAsStringAsync(stoppingToken);

            var doc = new HtmlDocument();
            doc.LoadHtml(content);

            var carNodes = doc.DocumentNode.SelectNodes("//a[@data-ftid='bulls-list_bull']");
            if (carNodes == null || !carNodes.Any())
            {
                throw new Exception(
                    $"Nodes '//a[contains(@data-ftid, 'bulls-list_bull')]': не удалось получить.");
            }

            var result = new List<DromResult>();
            foreach (var carNode in carNodes)
            {
                var title = carNode
                    .SelectSingleNode(
                        ".//span[@data-ftid='bull_title']").InnerText
                    .Split(", ");
                
                var titleName = title[0];
                var titleYear = title[1];
                
                var address = carNode
                    .SelectSingleNode(
                        ".//span[@data-ftid='bull_location']").InnerText;
                
                var price = carNode
                    .SelectSingleNode(
                        ".//span[@data-ftid='bull_price']").InnerText
                    .Trim().Replace(" ", "");
                
                var href = carNode.Attributes["href"].Value;
                var originId = href
                    .Split("/", StringSplitOptions.RemoveEmptyEntries)
                    .Last()
                    .Replace(".html", string.Empty);
                
                var description = carNode
                    .SelectSingleNode(
                        ".//div[@data-ftid='component_inline-bull-description']").InnerText;

                var image = carNode
                    .SelectSingleNode(
                        ".//img").GetAttributeValue("data-srcset", string.Empty)
                    .Split(", ").OrderBy(x => x).Last().Split(" ").First();

                result.Add(new DromResult()
                {
                    SourceType = SourceType.Drom,
                    OriginId = originId,
                    Title = titleName + ", " + description,
                    Сity = address,
                    Price = Int32.Parse(price),
                    PublishedAt = DateTime.Now,
                    Url = href,
                    Year = Int32.Parse(titleYear),
                    ImageUrls = new List<string>() { image }
                });
            }

            return result;
        
        }
        catch (SuccessSolutionException ex)
        {
            SetDefaultHeadersToHttpClient(ex.Headers);
            _logger.LogInformation("Капча успешно решена.");
            return new List<DromResult>();
        }
    }
    
    private void SetDefaultHeadersToHttpClient(HttpResponseHeaders headers)
    {
        if (HttpClient.DefaultRequestHeaders.Any(x => x.Key == "__hash_"))
        {
            return;
        }

        HttpClient.DefaultRequestHeaders.Clear();
        foreach (var header in headers)
        {
            HttpClient.DefaultRequestHeaders.Add(header.Key, header.Value);
        }
    }
}