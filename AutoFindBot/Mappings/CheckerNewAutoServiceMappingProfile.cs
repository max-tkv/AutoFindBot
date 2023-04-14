using System.Globalization;
using AutoFindBot.Entities;
using AutoFindBot.Models.AutoRu;
using AutoFindBot.Models.Avito;
using AutoFindBot.Models.KeyAutoProbeg;
using AutoFindBot.Models.TradeDealer;
using AutoMapper;

namespace AutoFindBot.Mappings;

public class CheckerNewAutoServiceMappingProfile : Profile
{
    public CheckerNewAutoServiceMappingProfile()
    {
        CreateMap<Entities.UserFilter, TradeDealerFilter>()
            .ForMember(x => x.PriceMin, o => o.MapFrom(x => ConvertPrice(x.PriceMin)))
            .ForMember(x => x.PriceMax, o => o.MapFrom(x => ConvertPrice(x.PriceMax)));
        
        CreateMap<Entities.UserFilter, KeyAutoProbegFilter>()
            .ForMember(x => x.PriceMin, o => o.MapFrom(x => ConvertPrice(x.PriceMin)))
            .ForMember(x => x.PriceMax, o => o.MapFrom(x => ConvertPrice(x.PriceMax)));
        
        CreateMap<Entities.UserFilter, AvitoFilter>()
            .ForMember(x => x.PriceMin, o => o.MapFrom(x => ConvertPrice(x.PriceMin)))
            .ForMember(x => x.PriceMax, o => o.MapFrom(x => ConvertPrice(x.PriceMax)));
        
        CreateMap<Entities.UserFilter, AutoRuFilter>()
            .ForMember(x => x.PriceMin, o => o.MapFrom(x => ConvertPrice(x.PriceMin)))
            .ForMember(x => x.PriceMax, o => o.MapFrom(x => ConvertPrice(x.PriceMax)));
        
        CreateMap<CarInfo, Entities.Car>()
            .ForMember(x => x.Price, o => o.MapFrom(x => x.Price))
            .ForMember(x => x.Source, o => o.MapFrom(x => Entities.Source.TradeDealer))
            .ForMember(x => x.Title, o => o.MapFrom(x => x.Title))
            .ForMember(x => x.Vin, o => o.MapFrom(x => x.Vin))
            .ForMember(x => x.Year, o => o.MapFrom(x => x.Year))
            .ForMember(x => x.OriginId, o => o.MapFrom(x => x.Id))
            .ForMember(x => x.Id, o => o.MapFrom(x => 0))
            .ForMember(x => x.Сity, o => o.MapFrom(x => x.Company.Сity.Title))
            .ForMember(x => x.PublishedAt, o => o.MapFrom(x => x.PublishedAt))
            .ForMember(x => x.Url, o => o.MapFrom(x => $"{x.Brand.Alias}/{x.Generation!.Alias ?? x.Model!.Alias}/{x.Id}"))
            .ForMember(x => x.ImageUrls, o => o.MapFrom(x => 
                x.OriginalPhotos
                    .Select(z => new Image()
                    {
                        Urls = new Dictionary<string, string>()
                        {
                            { "1", z.Path }
                        }
                    })
                    .ToList()));
        
        CreateMap<KeyAutoProbegResult, Entities.Car>()
            .ForMember(x => x.Price, o => o.MapFrom(x => x.Price))
            .ForMember(x => x.Source, o => o.MapFrom(x => Entities.Source.TradeDealer))
            .ForMember(x => x.Title, o => o.MapFrom(x => x.Title))
            .ForMember(x => x.Vin, o => o.MapFrom(x => x.Vin))
            .ForMember(x => x.Year, o => o.MapFrom(x => x.Year))
            .ForMember(x => x.OriginId, o => o.MapFrom(x => x.OriginId))
            .ForMember(x => x.Id, o => o.MapFrom(x => 0))
            .ForMember(x => x.Сity, o => o.MapFrom(x => x.Сity))
            .ForMember(x => x.PublishedAt, o => o.MapFrom(x => x.PublishedAt))
            .ForMember(x => x.Url, o => o.MapFrom(x => x.Url));
        
        CreateMap<AvitoResult, Entities.Car>()
            .ForMember(x => x.Price, o => o.MapFrom(x => x.Price))
            .ForMember(x => x.Source, o => o.MapFrom(x => Entities.Source.Avito))
            .ForMember(x => x.Title, o => o.MapFrom(x => x.Title))
            .ForMember(x => x.Vin, o => o.MapFrom(x => x.Vin))
            .ForMember(x => x.Year, o => o.MapFrom(x => x.Year))
            .ForMember(x => x.OriginId, o => o.MapFrom(x => x.OriginId))
            .ForMember(x => x.Id, o => o.MapFrom(x => 0))
            .ForMember(x => x.Сity, o => o.MapFrom(x => x.Сity))
            .ForMember(x => x.PublishedAt, o => o.MapFrom(x => x.PublishedAt))
            .ForMember(x => x.Url, o => o.MapFrom(x => x.Url))
            .ForMember(x => x.ImageUrls, o => o.MapFrom(x => x.Images));
        
        CreateMap<AvitoImage, Image>()
            .ForMember(x => x.Urls, o => o.MapFrom(x => x.Urls));
    }

    private string ConvertPrice(decimal price)
    {
        return price.ToString(CultureInfo.InvariantCulture).Split('.').First();
    }
}