using System.ComponentModel;
using System.Reflection;

namespace AutoFindBot.Utils.Helpers;

public static class EnumHelper
{
    public static string GetDescription<TType>(this TType self) where TType : Enum
        => (self.GetType()
                .GetField(self.ToString()) ?? throw new InvalidOperationException())
                .GetCustomAttribute<DescriptionAttribute>(false)?
                .Description ?? self.ToString();
}