﻿using AutoFindBot.Integration.Models.TradeDealerClient;
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
         
         CreateMap<CompanyResponse, Company>()
             .ForMember(x => x.Сity, o => o.MapFrom(x => x.City));
         
         CreateMap<СityResponse, Сity>(MemberList.Destination);
         
         CreateMap<BrandResponse, Brand>(MemberList.Destination);
         
         CreateMap<GenerationResponse, Generation>(MemberList.Destination);
         
         CreateMap<ModelResponse, Model>(MemberList.Destination);
    }
}