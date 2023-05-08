using AutoFindBot.Integration.Models;
using AutoFindBot.Models.TradeDealer;
using AutoMapper;

namespace AutoFindBot.Integration.Mappings;

public class TradeDealerHttpApiClientMappingProfile : Profile
{
    public TradeDealerHttpApiClientMappingProfile()
    {
         CreateMap<TradeDealerResponse, TradeDealerResult>()
             .ForMember(x => x.CarInfos, o => o.MapFrom(x => x.CarInfoResponses));
        
         CreateMap<CarInfoResponse, CarInfo>(MemberList.Destination);
         
         CreateMap<CompanyResponse, Company>()
             .ForMember(x => x.City, o => o.MapFrom(x => x.City));
         
         CreateMap<СityResponse, City>(MemberList.Destination);
         
         CreateMap<BrandResponse, Brand>(MemberList.Destination);
         
         CreateMap<GenerationResponse, Generation>(MemberList.Destination);
         
         CreateMap<ModelResponse, Model>(MemberList.Destination);
         
         CreateMap<OriginalPhotoResponse, OriginalPhoto>(MemberList.Destination);
    }
}