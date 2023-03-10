using AutoFindBot.Entities;
using AutoFindBot.Integration.Avito.Models;
using AutoFindBot.Models.Avito;
using AutoFindBot.Utils.Helpers;
using AutoMapper;

namespace AutoFindBot.Integration.Avito.Mappings;

public class AvitoHttpApiClientMappingProfile : Profile
{
    public AvitoHttpApiClientMappingProfile()
    {
        CreateMap<AvitoResultResponseItem, AvitoResult>()
            .ForMember(x => x.Title, o => o.MapFrom(x => x.Value.Title))
            .ForMember(x => x.OriginId, o => o.MapFrom(x => x.Value.Id))
            .ForMember(x => x.Price, o => o.MapFrom(x => 
                int.Parse(x.Value.Price.Replace("₽", string.Empty).Replace(" ", String.Empty))))
            .ForMember(x => x.Year, o => o.MapFrom(x => 
                x.Value.Title.Split(",", StringSplitOptions.TrimEntries)[1]))
            .ForMember(x => x.Сity, o => o.MapFrom(x => x.Value.Location))
            .ForMember(x => x.Source, o => o.MapFrom(x => Source.Avito))
            .ForMember(x => x.PublishedAt, o => o.MapFrom(x => 
                DateTimeHelper.UnixTimeStampToDateTime(x.Value.Time)))
            .ForMember(x => x.Url, o => o.MapFrom(x => x.Value.UriMweb));
    }
}