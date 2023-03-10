using System.Text.RegularExpressions;
using AutoFindBot.Abstractions.HttpClients;
using AutoFindBot.Entities;
using AutoFindBot.Integration.Avito.Invariants;
using AutoFindBot.Integration.Avito.Options;
using AutoFindBot.Models.Avito;
using AutoFindBot.Utils.Http;
using HtmlAgilityPack;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;

namespace AutoFindBot.Integration.Avito;

public class AvitoHttpApiClient : JsonHttpApiClient, IAvitoHttpApiClient
{
    private readonly ILogger<AvitoHttpApiClient> _logger;
    private readonly AvitoHttpApiClientOptions? _options;

    private const string CarNodePath = "//div[contains(@class, 'iva-item-content-rejJg')]";

    public AvitoHttpApiClient(
        HttpClient httpClient, 
        IConfiguration configuration, 
        ILogger<AvitoHttpApiClient> logger) : base(httpClient)
    {
        _logger = logger;
        _options = configuration
            .GetSection(RegisterAvitoHttpApiClientInvariants.OptionsSectionPath)
            .Get<AvitoHttpApiClientOptions>();
    }

    public async Task<List<AvitoResult>> GetAutoByFilterAsync(AvitoFilter filter)
    {
        try
        {
            ArgumentNullException.ThrowIfNull(filter);

            var response = await GetApiResponseAsync(filter);

            var doc = new HtmlDocument();
            doc.LoadHtml(response);

            var carNodes = doc.DocumentNode.SelectNodes(CarNodePath);
            if (carNodes == null || !carNodes.Any())
            {
                throw new Exception(
                    $"Nodes {CarNodePath}: не удалось получить.");
            }

            var result = new List<AvitoResult>();
            foreach (var carNode in carNodes)
            {
                var bodyNode = carNode
                    .SelectSingleNode("div[contains(@class, 'iva-item-body-')]");

                var titleFull = bodyNode
                    .SelectSingleNode("div[contains(@class, 'iva-item-titleStep-')]")
                    .InnerText.Trim();

                var title = titleFull.Split(",",
                    StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).First();

                var nodes = bodyNode
                    .SelectNodes("div[contains(@class, '')]");
                var address = nodes[nodes.Count - 2]
                    .InnerText?.Trim();

                var priceOrigin = bodyNode
                    .SelectSingleNode("div[contains(@class, 'iva-item-priceStep-')]")
                    .InnerText.Trim().Replace("₽", string.Empty);
                var price = Regex.Replace(priceOrigin, @"\s+", "");

                var href = bodyNode
                    .SelectSingleNode("div[contains(@class, 'iva-item-titleStep-')]/a")
                    .Attributes["href"].Value;

                var year = titleFull.Split(",",
                    StringSplitOptions.TrimEntries | StringSplitOptions.RemoveEmptyEntries).Last();

                result.Add(new AvitoResult()
                {
                    Source = Source.KeyAutoProbeg,
                    OriginId = href
                        .Split("/", StringSplitOptions.RemoveEmptyEntries).Last()
                        .Split("_", StringSplitOptions.RemoveEmptyEntries).Last(),
                    Title = title,
                    Сity = address ?? "",
                    Price = Int32.Parse(price),
                    PublishedAt = DateTime.Now,
                    Url = href,
                    Year = Int32.Parse(year)
                });
            }

            return result;
        }
        catch (Exception e)
        {
            _logger.LogError(e, $"Ошибка: {e.Message}");
            throw;
        }
    }

    #region Приватные методы

    public async Task<string> GetApiResponseAsync(AvitoFilter filter)
    {
        var path = _options?.BaseUrl + _options?.GetAutoByFilterQuery
            .Replace(AvitoHttpApiClientInvariants.PriceMin, filter.PriceMin)
            .Replace(AvitoHttpApiClientInvariants.PriceMax, filter.PriceMax);

        using var response = await SendAsync(path, HttpMethod.Get);
        response.EnsureSuccessStatusCode();

        return await response.Content.ReadAsStringAsync();
    }

    #endregion
}