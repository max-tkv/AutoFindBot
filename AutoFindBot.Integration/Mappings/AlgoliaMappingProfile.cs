using AutoMapper;

namespace AutoFindBot.Integration.Mappings;

public class AlgoliaMappingProfile : Profile
{
    public AlgoliaMappingProfile()
    {
        // CreateMap<AlgoliaSearchReasponse, AlgoliaSearchResult>()
        //     .ForMember(x => x.Results, o => o.MapFrom(x => x.Results));
        //
        // CreateMap<AlgoliaReasponseItem, AlgoliaSearchResultItem>(MemberList.Destination);
        //
        // CreateMap<AlgoliaHitItem, AlgoliaHitResult>()
        //     .ForMember(x => x.Urls, o => o.MapFrom(x => new List<string>() { x.Url! }));
    }
}