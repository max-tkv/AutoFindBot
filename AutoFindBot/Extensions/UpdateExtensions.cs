using Telegram.Bot.Types;

namespace AutoFindBot.Extensions;

public static class UpdateExtensions
{
    public static int GetSelectedIndex(this Update update)
    {
        var selectIndex = update.CallbackQuery?.Data?
            .Split(":").GetValue(1)?.ToString();
        if (selectIndex == null)
        {
            return -1;
        }

        if (int.TryParse(selectIndex, out int value))
        {
            return value;
        }

        return -1;
    }
    
    public static string? GetCommandNameFromCallbackData(this Update update)
    {
        var commandName = update.CallbackQuery?.Data?
            .Split(":").GetValue(0)?.ToString();

        return commandName;
    }
}