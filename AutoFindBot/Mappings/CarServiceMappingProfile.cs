using AutoFindBot.Entities;
using AutoFindBot.Models.TradeDealer;
using AutoMapper;

namespace AutoFindBot.Mappings;

public class CarServiceMappingProfile : Profile
{
    public CarServiceMappingProfile()
    {
        CreateMap<CarInfo, Car>()
            .ForMember(x => x.Price, o => o.MapFrom(x => x.Price))
            .ForMember(x => x.Source, o => o.MapFrom(x => x.Source))
            .ForMember(x => x.Title, o => o.MapFrom(x => x.Title))
            .ForMember(x => x.Vin, o => o.MapFrom(x => x.Vin))
            .ForMember(x => x.Year, o => o.MapFrom(x => x.Year))
            .ForMember(x => x.OriginId, o => o.MapFrom(x => x.Id))
            .ForMember(x => x.Id, o => o.MapFrom(x => 0));
    }
}