using AutoFindBot.Integration.Youla.Models;
using AutoFindBot.Lookups;
using AutoFindBot.Models.Youla;
using AutoMapper;

namespace AutoFindBot.Integration.Youla.Mappings;

public class YoulaHttpApiClientMappingProfile : Profile
{
    public YoulaHttpApiClientMappingProfile()
    {
        CreateMap<YoulaResponse, YoulaResult>()
            .ForMember(x => x.Cars, o => o.MapFrom(x => x.Data!.Feed!.Items));
        
        CreateMap<YoulaResponseDataFeedItem, YoulaResultAuto>()
            .ForMember(x => x.OriginId, o => o.MapFrom(x => x.Product!.Id))
            .ForMember(x => x.Title, o => o.MapFrom(x => x.Product!.Name))
            .ForMember(x => x.Price, o => o.MapFrom(x => x.Product!.Price.RealPrice.Price / 100))
            .ForMember(x => x.Сity, o => o.MapFrom(x => x.Product!.Location.CityName))
            .ForMember(x => x.PublishedAt, o => o.MapFrom(x => DateTime.Now))
            .ForMember(x => x.SourceType, o => o.MapFrom(x => SourceType.Youla))
            .ForMember(x => x.Images, o => o.MapFrom(x => x.Product!.Images.Select(z => z.Url).ToList()))
            .ForMember(x => x.Url, o => o.MapFrom(x => x.Product!.Url))
            .AfterMap((youlaResponseDataFeedItem, youlaResultAuto) =>
            {
                var nameArr = youlaResponseDataFeedItem.Product?.Name
                    .Split(",", StringSplitOptions.TrimEntries);
                if (nameArr?.Length == Decimal.One || nameArr?.Length > 2)
                {
                    return;
                }

                if (int.TryParse(nameArr?.Last(), out var year))
                {
                    youlaResultAuto.Year = year;
                }
            });
    }
}