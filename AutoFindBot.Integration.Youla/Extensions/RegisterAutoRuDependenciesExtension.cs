using AutoFindBot.Integration.Youla.Invariants;
using AutoFindBot.Integration.Youla.Options;
using Microsoft.Extensions.Options;

namespace AutoFindBot.Integration.Youla.Extensions;

public static class RegisterRuCaptchaDependenciesExtension
{
    public static void Validate(this YoulaHttpApiClientOptions options)
    {
        if (options == default)
        {
            throw new ArgumentNullException(
                nameof(options), 
                RegisterYoulaHttpApiClientInvariants.OptionsEmptyValue);
        }

        List<string> errors = new();
        if (string.IsNullOrWhiteSpace(options.BaseUrl))
        {
            errors.Add(string.Format(
                RegisterYoulaHttpApiClientInvariants.OptionNotFoundError, 
                nameof(options.BaseUrl)));
        }

        if (errors.Count != 0)
        {
            throw new OptionsValidationException(
                nameof(YoulaHttpApiClientOptions), 
                typeof(YoulaHttpApiClientOptions), 
                errors);
        }
    }
}