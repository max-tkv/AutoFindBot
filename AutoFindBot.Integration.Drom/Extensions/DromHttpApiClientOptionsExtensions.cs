using AutoFindBot.Integration.Drom.Invariants;
using AutoFindBot.Integration.Drom.Options;
using Microsoft.Extensions.Options;

namespace AutoFindBot.Integration.Drom.Extensions;

public static class DromHttpApiClientOptionsExtensions
{
    public static void Validate(this DromHttpApiClientOptions options)
    {
        if (options == default)
        {
            throw new ArgumentNullException(
                nameof(options), 
                RegisterDromHttpApiClientInvariants.OptionsEmptyValue);
        }

        List<string> errors = new();
        if (string.IsNullOrWhiteSpace(options.BaseUrl))
        {
            errors.Add(string.Format(
                RegisterDromHttpApiClientInvariants.OptionNotFoundError, 
                nameof(options.BaseUrl)));
        }
            
        if (string.IsNullOrWhiteSpace(options.GetAutoByFilterQuery))
        {
            errors.Add(string.Format(
                RegisterDromHttpApiClientInvariants.OptionNotFoundError, 
                nameof(options.GetAutoByFilterQuery)));
        }

        if (errors.Count != 0)
        {
            throw new OptionsValidationException(
                nameof(DromHttpApiClientOptions), 
                typeof(DromHttpApiClientOptions), 
                errors);
        }
    }
}