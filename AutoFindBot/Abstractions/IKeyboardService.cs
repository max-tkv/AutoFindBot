using AutoFindBot.Entities;
using Telegram.Bot.Types.ReplyMarkups;

namespace AutoFindBot.Abstractions;

public interface IKeyboardService
{
    IReplyMarkup GetStartMenuKeyboard();
    IReplyMarkup GetRemovedKeyboard();
    IReplyMarkup GetRequiredSubscriptionsKeyboard();
    IReplyMarkup GetUserSettingsKeyboard();
    IReplyMarkup GetUserFiltersKeyboard(List<UserFilter> userFilters);
}