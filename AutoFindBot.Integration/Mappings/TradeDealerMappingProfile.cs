using AutoFindBot.Integration.Models.TradeDealerClient;
using AutoFindBot.Models.TradeDealer;
using AutoMapper;

namespace AutoFindBot.Integration.Mappings;

public class TradeDealerMappingProfile : Profile
{
    public TradeDealerMappingProfile()
    {
         CreateMap<TradeDealerResponse, TradeDealerResult>()
             .ForMember(x => x.CarInfos, o => o.MapFrom(x => x.CarInfoResponses));
        
         CreateMap<CarInfoResponse, CarInfo>(MemberList.Destination);
    }
}