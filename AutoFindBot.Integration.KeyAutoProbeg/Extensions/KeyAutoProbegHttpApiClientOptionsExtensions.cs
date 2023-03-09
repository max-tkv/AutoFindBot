using AutoFindBot.Integration.KeyAutoProbeg.Invariants;
using AutoFindBot.Integration.KeyAutoProbeg.Options;
using Microsoft.Extensions.Options;

namespace AutoFindBot.Integration.KeyAutoProbeg.Extensions;

public static class KeyAutoProbegHttpApiClientOptionsExtensions
{
    public static void Validate(this KeyAutoProbegHttpApiClientOptions options)
    {
        if (options == default)
        {
            throw new ArgumentNullException(
                nameof(options), 
                RegisterKeyAutoProbegHttpApiClientInvariants.OptionsEmptyValue);
        }

        List<string> errors = new();
        if (string.IsNullOrWhiteSpace(options.BaseUrl))
        {
            errors.Add(string.Format(
                RegisterKeyAutoProbegHttpApiClientInvariants.OptionNotFoundError, 
                nameof(options.BaseUrl)));
        }
            
        if (string.IsNullOrWhiteSpace(options.GetAutoByFilterQuery))
        {
            errors.Add(string.Format(
                RegisterKeyAutoProbegHttpApiClientInvariants.OptionNotFoundError, 
                nameof(options.GetAutoByFilterQuery)));
        }

        if (errors.Count != 0)
        {
            throw new OptionsValidationException(
                nameof(KeyAutoProbegHttpApiClientOptions), 
                typeof(KeyAutoProbegHttpApiClientOptions), 
                errors);
        }
    }
}