using AutoFindBot.Integration.AutoRu.Models;
using AutoFindBot.Models.AutoRu;
using AutoMapper;

namespace AutoFindBot.Integration.AutoRu.Mappings;

public class AutoRuHttpApiClientMappingProfile : Profile
{
    public AutoRuHttpApiClientMappingProfile()
    {
        CreateMap<AutoRuResponse, AutoRuResult>(MemberList.Destination);
        
        CreateMap<AutoRuResponseOffer, AutoRuResultOffer>(MemberList.Destination);
        
        CreateMap<AutoRuResponseOfferDocuments, AutoRuResultOfferDocuments>(MemberList.Destination);
        
        CreateMap<AutoRuResponseOfferPriceInfo, AutoRuResultOfferPriceInfo>(MemberList.Destination);
        
        CreateMap<AutoRuResponseOfferState, AutoRuResultOfferState>(MemberList.Destination);
        
        CreateMap<AutoRuResponseOfferStateImageUrl, AutoRuResultOfferStateImageUrl>(MemberList.Destination);
        
        CreateMap<AutoRuResponseOfferVehicleInfo, AutoRuResultOfferVehicleInfo>(MemberList.Destination);
        
        CreateMap<AutoRuResponseOfferVehicleInfoMarkInfo, AutoRuResultOfferVehicleInfoMarkInfo>(MemberList.Destination);
        
        CreateMap<AutoRuResponseOfferVehicleInfoModelInfo, AutoRuResultOfferVehicleInfoModelInfo>(MemberList.Destination);
        
        CreateMap<AutoRuResponseOfferVehicleInfoSuperGen, AutoRuResultOfferVehicleInfoSuperGen>(MemberList.Destination);
        
        CreateMap<AutoRuResponseOfferVehicleInfoTechParam, AutoRuResultOfferVehicleInfoTechParam>(MemberList.Destination);
        
        CreateMap<AutoRuResponseOfferSeller, AutoRuResultOfferSeller>(MemberList.Destination);
        
        CreateMap<AutoRuResponseOfferSellerLocation, AutoRuResultOfferSellerLocation>(MemberList.Destination);
        
        CreateMap<AutoRuResponseOfferSellerLocationRegionInfo, AutoRuResultOfferSellerLocationRegionInfo>(MemberList.Destination);
    }
}