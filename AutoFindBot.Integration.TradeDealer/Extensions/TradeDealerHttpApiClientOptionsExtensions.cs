using AutoFindBot.Integration.Invariants;
using AutoFindBot.Integration.Options;
using Microsoft.Extensions.Options;

namespace AutoFindBot.Integration.Extensions;

public static class TradeDealerHttpApiClientOptionsExtensions
{
    public static void Validate(this TradeDealerHttpApiClientOptions options)
    {
        if (options == default)
        {
            throw new ArgumentNullException(
                nameof(options), 
                RegisterTradeDealerHttpApiClientInvariants.OptionsEmptyValue);
        }

        List<string> errors = new();
        if (string.IsNullOrWhiteSpace(options.BaseUrl))
        {
            errors.Add(string.Format(
                RegisterTradeDealerHttpApiClientInvariants.OptionNotFound, 
                nameof(options.BaseUrl)));
        }
        
        if (string.IsNullOrWhiteSpace(options.SiteUrl))
        {
            errors.Add(string.Format(
                RegisterTradeDealerHttpApiClientInvariants.OptionNotFound, 
                nameof(options.SiteUrl)));
        }
            
        if (string.IsNullOrWhiteSpace(options.GetAutoByFilterQuery))
        {
            errors.Add(string.Format(
                RegisterTradeDealerHttpApiClientInvariants.OptionNotFound, 
                nameof(options.GetAutoByFilterQuery)));
        }

        if (errors.Count != 0)
        {
            throw new OptionsValidationException(
                nameof(TradeDealerHttpApiClientOptions), 
                typeof(TradeDealerHttpApiClientOptions), 
                errors);
        }
    }
}