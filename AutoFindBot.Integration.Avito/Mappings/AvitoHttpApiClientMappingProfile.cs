using AutoFindBot.Integration.Avito.Models;
using AutoFindBot.Lookups;
using AutoFindBot.Models.Avito;
using AutoFindBot.Utils.Helpers;
using AutoMapper;
using Newtonsoft.Json;

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
            .ForMember(x => x.SourceType, o => o.MapFrom(x => SourceType.Avito))
            .ForMember(x => x.PublishedAt, o => o.MapFrom(x => 
                DateTimeHelper.UnixTimeStampToDateTime(x.Value.Time)))
            .ForMember(x => x.Url, o => o.MapFrom(x => x.Value.UriMweb))
            .AfterMap((s, d) =>
            {
                var output = new List<AvitoImage>();
                var images = s.Value.GalleryItem.Where(z => z.Type == "image");
                foreach (var image in images)
                {
                    var urls = new Dictionary<string, string>();
                    try
                    {
                        urls = JsonConvert.DeserializeObject<Dictionary<string, string>>(image.Value.ToString());
                        output.Add(new AvitoImage() { Urls = urls } );
                    }
                    catch (Exception e)
                    {
                        Console.WriteLine("Error Deserialize Object to Dictionary<string, string>. " +
                                          "Object: " + image.Value);
                    }
                }

                d.Images = output;
            });
    }
}