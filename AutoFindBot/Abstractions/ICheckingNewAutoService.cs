using AutoFindBot.Entities;
using Telegram.Bot;

namespace AutoFindBot.Abstractions;

public interface ICheckingNewAutoService
{
    Task CheckAndSendMessageAsync(
        TelegramBotClient botClient, 
        AppUser user,
        bool sendEmptyResultMessage = false);
}