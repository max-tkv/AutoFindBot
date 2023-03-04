using System.Text.RegularExpressions;

namespace AutoFindBot.Extensions;

public static class StringExtension
{
    public static string? RemoveSpecialChars(this string input)
    {
        return string.IsNullOrWhiteSpace(input) ? null 
            : Regex.Replace(input, @"[^0-9a-zA-Z\._]", " ");
    }

    public static string? RemoveExclamationMarks(this string input)
    {
        return string.IsNullOrWhiteSpace(input) ? null 
                : input.Replace("!", string.Empty)
                    .Replace("?", string.Empty);
        
    }
}