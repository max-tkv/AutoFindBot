using Telegram.Bot.Types.ReplyMarkups;

namespace AutoFindBot.Abstractions;

public interface IKeyboardService
{
    IReplyMarkup GetResultKeyboard(IPaginationResult pagination);
    IReplyMarkup GetStartMenuKeyboard();
    IReplyMarkup GetRemovedKeyboard();
    IReplyMarkup GetRequiredSubscriptionsKeyboard();
    IReplyMarkup GetUserSettingsKeyboard();
}