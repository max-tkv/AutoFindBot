using AutoFindBot.Abstractions.HttpClients;
using AutoFindBot.Entities;
using AutoFindBot.Exceptions;
using AutoFindBot.Integration.KeyAutoProbeg.Invariants;
using AutoFindBot.Integration.KeyAutoProbeg.Options;
using AutoFindBot.Models.ConfigurationOptions;
using AutoFindBot.Models.KeyAutoProbeg;
using AutoFindBot.Utils.Http;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;

namespace AutoFindBot.Integration.KeyAutoProbeg;

public class KeyAutoProbegHttpApiClient : HttpApiClient, IKeyAutoProbegHttpApiClient
{
    private readonly ILogger<KeyAutoProbegHttpApiClient> _logger;
    private readonly KeyAutoProbegHttpApiClientOptions _options;
    private IKeyAutoProbegHttpApiClient _keyAutoProbegHttpApiClientImplementation;
    private readonly IOptions<DefaultFilterOptions> _defaultFilterOptions;

    private const string CarNodePath = "";

    public KeyAutoProbegHttpApiClient(
        HttpClient httpClient,
        KeyAutoProbegHttpApiClientOptions options, 
        ILogger<KeyAutoProbegHttpApiClient> logger,
        IOptions<DefaultFilterOptions> defaultFilterOptions) : base(httpClient)
    {
        _logger = logger;
        _options = options;
        _defaultFilterOptions = defaultFilterOptions;
    }

    public async Task<List<KeyAutoProbegResult>> GetAllNewAutoAsync()
    {
        try
        {
            NotActiveSourceException.ThrowIfNotActive(nameof(KeyAutoProbegHttpApiClient), _options.Active);

            var path = _options?.BaseUrl + _options?.GetAutoByFilterQuery
                .Replace(KeyAutoProbegHttpApiClientInvariants.PriceMin, _defaultFilterOptions.Value.PriceMin.ToString())
                .Replace(KeyAutoProbegHttpApiClientInvariants.PriceMax, _defaultFilterOptions.Value.PriceMax.ToString());
            var response = await SendAsync(path, HttpMethod.Get);
            var content = await response.Content.ReadAsStringAsync();
            if (response.IsSuccessStatusCode == false)
            {
                throw new Exception(
                    $"Произошла ошибка: {content}");
            }
            
            var doc = new HtmlDocument();
            doc.LoadHtml(content);

            var carNodes = doc.DocumentNode.SelectNodes(CarNodePath);
            if (carNodes == null || !carNodes.Any())
            {
                throw new Exception(
                    $"Nodes {CarNodePath}: не удалось получить.");
            }

            var result = new List<KeyAutoProbegResult>();
            foreach (var carNode in carNodes)
            {
                var mineNode = carNode
                    .SelectSingleNode("div[contains(@class, 'w-full space-y-4')]");
                
                 var title = mineNode
                    .SelectSingleNode("div[contains(@class, 'flex flex-col md:flex-row justify-between w-full md:-mb-10')]")
                    .SelectSingleNode("p[contains(@class, 'text-2xl md:text-xl font-medium group-hover:text-current')]")
                    .InnerText.Trim();
                
                var address = mineNode
                    .SelectSingleNode("div[contains(@class, 'md:w-72 lg:w-53 xl:w-80 pt-2 md:pt-5')]")
                    .SelectSingleNode("div[contains(@class, 'space-y-2')]")
                    .SelectSingleNode("p[contains(@class, 'space-x-2 flex items-center')]")
                    .InnerText.Trim();
                
                var price = mineNode
                    .SelectSingleNode("div[contains(@class, 'flex flex-col md:flex-row justify-between w-full md:-mb-10')]")
                    .SelectSingleNode("div[contains(@class, 'flex flex-col md:items-end md:space-y-2')]")
                    .SelectSingleNode("div[contains(@class, 'flex flex-row-reverse md:flex-row justify-end md:justify-start items-center md:space-x-2')]")
                    .InnerText.Trim()
                    .Replace(" ", string.Empty).Replace("₽", string.Empty);

                var href = carNode.Attributes["href"].Value;

                var year = mineNode
                    .SelectSingleNode("div[contains(@class, 'space-y-2')]")
                    .SelectNodes("p[contains(@class, 'text-md font-medium')]")
                    .First()
                    .SelectSingleNode("span[contains(@class, 'text-base font-normal')]")
                    .InnerText.Trim();

                var images = new List<string>();
                var imagesNodes = carNode.SelectNodes(".//div[contains(@class, 'swiper-slide h-full')]");
                foreach (var imagesNode in imagesNodes)
                {
                    var url = imagesNode.SelectSingleNode("img").Attributes["src"].Value
                        .Replace("medium", "large");
                    images.Add(url);
                }
                
                result.Add(new KeyAutoProbegResult()
                {
                    Source = Source.KeyAutoProbeg,
                    OriginId = href.Split("/", StringSplitOptions.RemoveEmptyEntries).Last(),
                    Title = title,
                    Сity = address,
                    Price = Int32.Parse(price),
                    PublishedAt = DateTime.Now,
                    Url = href,
                    Year = Int32.Parse(year),
                    ImageUrls = images
                });
            }

            return result;
        }
        catch (NotActiveSourceException e)
        {
            _logger.LogWarning(e.Message);
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e.Message);
            throw;
        }
    }
}