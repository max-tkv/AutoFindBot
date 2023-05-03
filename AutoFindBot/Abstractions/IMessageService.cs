using AutoFindBot.Entities;
using AutoFindBot.Models.TradeDealer;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AutoFindBot.Abstractions;

public interface IMessageService
{
    Task SendStartMessage(TelegramBotClient botClient, AppUser user);
    Task SendErrorMessageAsync(TelegramBotClient botClient, AppUser user, string message);
    Task SendErrorMessageAsync(TelegramBotClient botClient, AppUser user, int messageId, string message);
    Task SendRequiredSubscriptionsAsync(TelegramBotClient botClient, AppUser user);
    Task SendPopupMessageAsync(TelegramBotClient botClient, AppUser user, Update update, string getDescription);
    Task SendNewAutoMessageAsync(TelegramBotClient botClient, AppUser user, UserFilter userFilter, List<Car> newCarList);
    Task SendSettingsCommands(TelegramBotClient botClient, Update update, AppUser user);
    Task SendUserFiltersMessageAsync(TelegramBotClient botClient, Update update, AppUser user);
}