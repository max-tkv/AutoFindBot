using System.Text.RegularExpressions;
using System.Web;

namespace AutoFindBot.Utils.Helpers;

public static class StringHelper
{
    public static double Compare(string? str1, string? str2)
    {
        if (str1 == null || str2 == null) return 0;
        
        str1 = str1.DeleteAllBetween("(", ")").DeleteAllBetween("[", "]");
        str2 = str2.DeleteAllBetween("(", ")").DeleteAllBetween("[", "]");
        
        List<string> pairs1 = WordLetterPairs(str1.ToUpper());
        List<string> pairs2 = WordLetterPairs(str2.ToUpper());

        int intersection = 0;
        int union = pairs1.Count + pairs2.Count;

        for (int i = 0; i < pairs1.Count; i++)
        {
            for (int j = 0; j < pairs2.Count; j++)
            {
                if (pairs1[i] == pairs2[j])
                {
                    intersection++;
                    pairs2.RemoveAt(j);

                    break;
                }
            }
        }

        var result = (2.0 * intersection) / union;
        
        Console.WriteLine($"{str1}={str2}: {result}%");

        return result;
    }

    public static string DeleteAllBetween(this string input, string first, string last)
    {
        while (true)
        {
            var index1 = input.IndexOf(first, StringComparison.Ordinal);
            var index2 = input.IndexOf(last, index1 + first.Length, StringComparison.Ordinal);
            if (index1 > 0 && index2 > 0)
            {
                input = input.Remove(index1, index2 - index1 + last.Length).Trim();
                continue;
            }
            
            return input;
        }
    }
    
    public static string GetBetween(this string input, string first, string last)
    {
        var index1 = input.IndexOf(first, StringComparison.Ordinal);
        var index2 = input.IndexOf(last, index1 + first.Length, StringComparison.Ordinal);
        if (index1 > -1 && index2 > -1)
        {
            return  input.Substring(
                index1 + (first.Length == 0 ? 1 : first.Length), 
                index2 - index1 - (first.Length == 0 ? 1 : first.Length)).Trim();
        }
            
        return input;
    }

    public static string DeletePartNamesFromMovieTitle(this string value)
    {
        return Regex.Replace(value.ToLower(), "часть\\s\\d", "", RegexOptions.IgnoreCase).Trim();
    }

    public static string? DeleteLastChar(this string? value)
    {
        return value?.Remove(value.Length - 1);
    }
    
    public static string? UrlDecode(this string? value)
    {
        return HttpUtility.UrlDecode(value);
    }
    
    public static string? UrlEncode(this string? value)
    {
        return HttpUtility.UrlEncode(value);
    }
    
    public static string? Join(this string[]? values, string? separator)
    {
        return string.Join(separator, values!);
    }
    
    private static List<string> WordLetterPairs(string str)
    {
        List<string> AllPairs = new List<string>();
        
        string[] Words = Regex.Split(str, @"\s");
        for (int w = 0; w < Words.Length; w++)
        {
            if (!string.IsNullOrEmpty(Words[w]))
            {
                String[] PairsInWord = LetterPairs(Words[w]);

                for (int p = 0; p < PairsInWord.Length; p++)
                {
                    AllPairs.Add(PairsInWord[p]);
                }
            }
        }

        return AllPairs;
    }
    
    private static string[] LetterPairs(string str)
    {
        int numPairs = str.Length - 1;

        string[] pairs = new string[numPairs];
        for (int i = 0; i < numPairs; i++)
        {
            pairs[i] = str.Substring(i, 2);
        }

        return pairs;
    }
}