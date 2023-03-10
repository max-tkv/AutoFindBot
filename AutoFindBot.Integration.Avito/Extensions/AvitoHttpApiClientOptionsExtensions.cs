using AutoFindBot.Integration.Avito.Invariants;
using AutoFindBot.Integration.Avito.Options;
using Microsoft.Extensions.Options;

namespace AutoFindBot.Integration.Avito.Extensions;

public static class AvitoHttpApiClientOptionsExtensions
{
    public static void Validate(this AvitoHttpApiClientOptions options)
    {
        if (options == default)
        {
            throw new ArgumentNullException(
                nameof(options), 
                RegisterAvitoHttpApiClientInvariants.OptionsEmptyValue);
        }

        List<string> errors = new();
        if (string.IsNullOrWhiteSpace(options.BaseUrl))
        {
            errors.Add(string.Format(
                RegisterAvitoHttpApiClientInvariants.OptionNotFoundError, 
                nameof(options.BaseUrl)));
        }
            
        if (string.IsNullOrWhiteSpace(options.GetAutoByFilterQuery))
        {
            errors.Add(string.Format(
                RegisterAvitoHttpApiClientInvariants.OptionNotFoundError, 
                nameof(options.GetAutoByFilterQuery)));
        }
        
        if (options?.ProxyData == null || (options.ProxyData.Active && 
                                          string.IsNullOrWhiteSpace(options.ProxyData.Proxy)))
        {
            errors.Add(string.Format(
                RegisterAvitoHttpApiClientInvariants.OptionNotFoundError, 
                nameof(options.GetAutoByFilterQuery)));
        }

        if (errors.Count != 0)
        {
            throw new OptionsValidationException(
                nameof(AvitoHttpApiClientOptions), 
                typeof(AvitoHttpApiClientOptions), 
                errors);
        }
    }
}