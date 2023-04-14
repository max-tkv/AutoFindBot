using AutoFindBot.Integration.RuCaptcha.Invariants;
using AutoFindBot.Integration.RuCaptcha.Options;
using Microsoft.Extensions.Options;

namespace AutoFindBot.Integration.RuCaptcha.Extensions;

public static class RegisterRuCaptchaDependenciesExtension
{
    public static void Validate(this RuCaptchaHttpApiClientOptions options)
    {
        if (options == default)
        {
            throw new ArgumentNullException(
                nameof(options), 
                RegisterRuCaptchaHttpApiClientInvariants.OptionsEmptyValue);
        }

        List<string> errors = new();
        if (string.IsNullOrWhiteSpace(options.BaseUrl))
        {
            errors.Add(string.Format(
                RegisterRuCaptchaHttpApiClientInvariants.OptionNotFoundError, 
                nameof(options.BaseUrl)));
        }

        if (errors.Count != 0)
        {
            throw new OptionsValidationException(
                nameof(RuCaptchaHttpApiClientOptions), 
                typeof(RuCaptchaHttpApiClientOptions), 
                errors);
        }
    }
}