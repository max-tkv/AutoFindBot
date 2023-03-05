using System.Globalization;
using AutoFindBot.Models.TradeDealer;
using AutoMapper;

namespace AutoFindBot.Mappings;

public class CheckerNewAutoServiceMappingProfile : Profile
{
    public CheckerNewAutoServiceMappingProfile()
    {
        CreateMap<Entities.UserFilter, GetAutoByFilter>()
            .ForMember(x => x.PriceMin, o => o.MapFrom(x => ConvertPrice(x.PriceMin)))
            .ForMember(x => x.PriceMax, o => o.MapFrom(x => ConvertPrice(x.PriceMax)));
    }

    private string ConvertPrice(decimal price)
    {
        return price.ToString(CultureInfo.InvariantCulture).Split('.').First();
    }
}