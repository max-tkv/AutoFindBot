using System.Runtime.Serialization;

namespace AutoFindBot.Lookups;

public enum Emoji
{
    [EnumMember(Value = "❌")]
    Cross,
    
    [EnumMember(Value = "🚘")]
    Car
}