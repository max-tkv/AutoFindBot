﻿using System.Reflection;

namespace AutoFindBot.Extensions;

public static class EnumExtension
{
    public static TAttribute? GetAttribute<TAttribute>(this Enum enumValue) 
        where TAttribute : Attribute
    {
        return enumValue.GetType()
            .GetMember(enumValue.ToString())
            .First()
            .GetCustomAttribute<TAttribute>();
    }
}