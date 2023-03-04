using AutoFindBot.Entities;
using Telegram.Bot.Types;

namespace AutoFindBot.Commands
{
    public abstract class BaseCommand
    {
        public abstract string Name { get; }
        public abstract Task ExecuteAsync(Update update, AppUser user);
    }
}