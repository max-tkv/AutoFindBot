using AutoFindBot.Integration.AutoRu.Invariants;
using AutoFindBot.Integration.AutoRu.Options;
using Microsoft.Extensions.Options;

namespace AutoFindBot.Integration.AutoRu.Extensions;

public static class RegisterAutoRuDependenciesExtension
{
    public static void Validate(this AutoRuHttpApiClientOptions options)
    {
        if (options == default)
        {
            throw new ArgumentNullException(
                nameof(options), 
                RegisterAutoRuHttpApiClientInvariants.OptionsEmptyValue);
        }

        List<string> errors = new();
        if (string.IsNullOrWhiteSpace(options.BaseUrl))
        {
            errors.Add(string.Format(
                RegisterAutoRuHttpApiClientInvariants.OptionNotFoundError, 
                nameof(options.BaseUrl)));
        }

        if (errors.Count != 0)
        {
            throw new OptionsValidationException(
                nameof(AutoRuHttpApiClientOptions), 
                typeof(AutoRuHttpApiClientOptions), 
                errors);
        }
    }
}