using System.Globalization;
using AutoFindBot.Lookups;
using AutoFindBot.Models.AutoRu;
using AutoFindBot.Models.Avito;
using AutoFindBot.Models.Drom;
using AutoFindBot.Models.KeyAutoProbeg;
using AutoFindBot.Models.TradeDealer;
using AutoFindBot.Models.Youla;
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
        
        CreateMap<Entities.UserFilter, DromFilter>()
            .ForMember(x => x.PriceMin, o => o.MapFrom(x => ConvertPrice(x.PriceMin)))
            .ForMember(x => x.PriceMax, o => o.MapFrom(x => ConvertPrice(x.PriceMax)));
        
        CreateMap<CarResult, Entities.Car>()
            .ForMember(x => x.Price, o => o.MapFrom(x => x.Price))
            .ForMember(x => x.SourceType, o => o.MapFrom(x => SourceType.TradeDealer))
            .ForMember(x => x.Title, o => o.MapFrom(x => x.Title))
            .ForMember(x => x.Vin, o => o.MapFrom(x => x.Vin))
            .ForMember(x => x.Year, o => o.MapFrom(x => x.Year))
            .ForMember(x => x.OriginId, o => o.MapFrom(x => x.Id))
            .ForMember(x => x.Id, o => o.MapFrom(x => 0))
            .ForMember(x => x.Сity, o => o.MapFrom(x => x.Company.City.Title))
            .ForMember(x => x.PublishedAt, o => o.MapFrom(x => x.PublishedAt))
            .ForMember(x => x.Url, o => o.MapFrom(x => $"{x.Brand.Alias}/{x.Generation!.Alias ?? x.Model!.Alias}/{x.Id}"))
            .ForMember(x => x.ImageUrls, o => o.MapFrom(x => 
                x.OriginalPhotos
                    .Select(z => new Entities.Image()
                    {
                        Urls = new Dictionary<string, string>()
                        {
                            { "1", z.Path }
                        }
                    })
                    .ToList()));
        
        CreateMap<KeyAutoProbegResult, Entities.Car>()
            .ForMember(x => x.Price, o => o.MapFrom(x => x.Price))
            .ForMember(x => x.SourceType, o => o.MapFrom(x => SourceType.KeyAutoProbeg))
            .ForMember(x => x.Title, o => o.MapFrom(x => x.Title))
            .ForMember(x => x.Vin, o => o.MapFrom(x => x.Vin))
            .ForMember(x => x.Year, o => o.MapFrom(x => x.Year))
            .ForMember(x => x.OriginId, o => o.MapFrom(x => x.OriginId))
            .ForMember(x => x.Id, o => o.MapFrom(x => 0))
            .ForMember(x => x.Сity, o => o.MapFrom(x => x.Сity))
            .ForMember(x => x.PublishedAt, o => o.MapFrom(x => x.PublishedAt))
            .ForMember(x => x.Url, o => o.MapFrom(x => x.Url))
            .ForMember(x => x.ImageUrls, o => o.MapFrom(x => 
                x.ImageUrls
                    .Select(z => new Entities.Image()
                    {
                        Urls = new Dictionary<string, string>()
                        {
                            { "1", z }
                        }
                    })
                    .ToList()));
        
        CreateMap<DromResult, Entities.Car>()
            .ForMember(x => x.Price, o => o.MapFrom(x => x.Price))
            .ForMember(x => x.SourceType, o => o.MapFrom(x => SourceType.Drom))
            .ForMember(x => x.Title, o => o.MapFrom(x => x.Title))
            .ForMember(x => x.Vin, o => o.MapFrom(x => x.Vin))
            .ForMember(x => x.Year, o => o.MapFrom(x => x.Year))
            .ForMember(x => x.OriginId, o => o.MapFrom(x => x.OriginId))
            .ForMember(x => x.Id, o => o.MapFrom(x => 0))
            .ForMember(x => x.Сity, o => o.MapFrom(x => x.Сity))
            .ForMember(x => x.PublishedAt, o => o.MapFrom(x => x.PublishedAt))
            .ForMember(x => x.Url, o => o.MapFrom(x => x.Url))
            .ForMember(x => x.ImageUrls, o => o.MapFrom(x => 
                x.ImageUrls
                    .Select(z => new Entities.Image()
                    {
                        Urls = new Dictionary<string, string>()
                        {
                            { "1", z }
                        }
                    })
                    .ToList()));
        
        CreateMap<AvitoResult, Entities.Car>()
            .ForMember(x => x.Price, o => o.MapFrom(x => x.Price))
            .ForMember(x => x.SourceType, o => o.MapFrom(x => SourceType.Avito))
            .ForMember(x => x.Title, o => o.MapFrom(x => x.Title))
            .ForMember(x => x.Vin, o => o.MapFrom(x => x.Vin))
            .ForMember(x => x.Year, o => o.MapFrom(x => x.Year))
            .ForMember(x => x.OriginId, o => o.MapFrom(x => x.OriginId))
            .ForMember(x => x.Id, o => o.MapFrom(x => 0))
            .ForMember(x => x.Сity, o => o.MapFrom(x => x.Сity))
            .ForMember(x => x.PublishedAt, o => o.MapFrom(x => x.PublishedAt))
            .ForMember(x => x.Url, o => o.MapFrom(x => x.Url))
            .ForMember(x => x.ImageUrls, o => o.MapFrom(x => x.Images));
        
        CreateMap<AvitoImage, Entities.Image>()
            .ForMember(x => x.Urls, o => o.MapFrom(x => x.Urls));
        
        CreateMap<AutoRuResultOffer, Entities.Car>()
            .ForMember(x => x.Price, o => o.MapFrom(x => x.PriceInfo.Price))
            .ForMember(x => x.SourceType, o => o.MapFrom(x => SourceType.AutoRu))
            .ForMember(x => x.Title, o => o.MapFrom(x => $"{x.VehicleInfo.MarkInfo.Name} " +
                                                         $"{x.VehicleInfo.ModelInfo.Name} " +
                                                         $"{x.VehicleInfo.TechParam.HumanName}, " +
                                                         $"{x.Documents.Year}, " +
                                                         $"{x.State.Mileage} км"))
            .ForMember(x => x.Year, o => o.MapFrom(x => x.Documents.Year))
            .ForMember(x => x.OriginId, o => o.MapFrom(x => x.Id))
            .ForMember(x => x.Id, o => o.MapFrom(x => 0))
            .ForMember(x => x.Сity, o => o.MapFrom(x => x.Seller.Location.RegionInfo.Name))
            .ForMember(x => x.PublishedAt, o => o.MapFrom(x => DateTime.Now))
            .ForMember(x => x.Url, o => o.MapFrom(x => $"/cars/used/sale/{x.VehicleInfo.MarkInfo.Code}/{x.VehicleInfo.ModelInfo.Code}/{x.SaleId}/"))
            .ForMember(x => x.ImageUrls, o => o.MapFrom(x => x.State.ImageUrls));
        
        CreateMap<AutoRuResultOfferStateImageUrl, Entities.Image>()
            .ForMember(x => x.Urls, o => o.MapFrom(x => x.Sizes));
        
        CreateMap<YoulaResultAuto, Entities.Car>()
            .ForMember(x => x.Price, o => o.MapFrom(x => x.Price))
            .ForMember(x => x.SourceType, o => o.MapFrom(x => x.SourceType))
            .ForMember(x => x.Title, o => o.MapFrom(x => x.Title))
            .ForMember(x => x.Vin, o => o.MapFrom(x => x.Vin))
            .ForMember(x => x.Year, o => o.MapFrom(x => x.Year))
            .ForMember(x => x.OriginId, o => o.MapFrom(x => x.OriginId))
            .ForMember(x => x.Id, o => o.MapFrom(x => 0))
            .ForMember(x => x.Сity, o => o.MapFrom(x => x.Сity))
            .ForMember(x => x.PublishedAt, o => o.MapFrom(x => x.PublishedAt))
            .ForMember(x => x.Url, o => o.MapFrom(x => x.Url))
            .ForMember(x => x.ImageUrls, o => o.MapFrom(x => x.Images));
        
        CreateMap<string, Entities.Image>()
            .ForMember(x => x.Urls, o => o.MapFrom(x => new Dictionary<string, string>()
            {
                { "1", x }
            }));
    }

    private string ConvertPrice(decimal price)
    {
        return price.ToString(CultureInfo.InvariantCulture).Split('.').First();
    }
}