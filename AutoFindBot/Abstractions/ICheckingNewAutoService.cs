using AutoFindBot.Entities;
using AutoFindBot.Models.TradeDealer;
using Telegram.Bot;

namespace AutoFindBot.Abstractions;

public interface ICheckingNewAutoService
{
    Task CheckAndSendMessageAsync(TelegramBotClient botClient, AppUser user, bool sendEmptyResultMessage = false);
}