using AutoMapper;

namespace AutoFindBot.Mappings;

public class TracksServiceMappingProfile : Profile
{
    // public TracksServiceMappingProfile()
    // {
    //     CreateMap<TrackResult, MusicSearchResult>()
    //         .ForMember(x => x.Name, o => o.MapFrom(x => x.Title))
    //         .ForMember(x => x.Url, o => o.MapFrom(x => x.TrackSource))
    //         .ForMember(x => x.ArtistNames, o => o.MapFrom(x => (x.Artists ?? new List<ArtistItem>())
    //             .Select(z => z.Name).ToList()))
    //         .ForMember(x => x.ImageUrl, o => o.MapFrom(x => x.CoverUri!.Replace("%%", "400x400")));
    // }
}