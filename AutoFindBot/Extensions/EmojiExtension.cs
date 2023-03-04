using System.Runtime.Serialization;
using AutoFindBot.Lookups;

namespace AutoFindBot.Extensions;

public static class EmojiExtension
{
    public static string GetValue(this Emoji emoji)
    {
        var attr = emoji.GetType().GetMember(emoji.ToString()).FirstOrDefault()?
            .GetCustomAttributes(false).OfType<EnumMemberAttribute>().FirstOrDefault();
        
        return attr?.Value ?? emoji.ToString();
    }
}