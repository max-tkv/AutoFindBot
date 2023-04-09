using System.Text.Json;
using AutoFindBot.Entities;
using Microsoft.EntityFrameworkCore.Storage.ValueConversion;

namespace AutoFindBot.Storage.Converters;

public class ImageUrlsConverter : ValueConverter<List<Image>, string>
{
    public ImageUrlsConverter(JsonSerializerOptions jsonOptions) : base(
        dict => JsonSerializer.Serialize(dict, jsonOptions),
        str => JsonSerializer.Deserialize<List<Image>>(str, jsonOptions))
    {
    }
}