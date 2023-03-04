using Telegram.Bot.Types;

namespace AutoFindBot.Abstractions
{
    public interface ICommandExecutorService
    {
        Task ExecuteAsync(Update update, string? commandName = null);
    }
}