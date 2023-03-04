using AutoFindBot.Entities;
using Telegram.Bot.Types;

namespace AutoFindBot.Abstractions;

public interface IMessageService
{
    Task SendStartMessage(AppUser user);
    Task SendErrorMessageAsync(AppUser user, string message);
    Task SendErrorMessageAsync(AppUser user, int messageId, string message);
    Task SendRequiredSubscriptionsAsync(AppUser user);
    Task SendPopupMessageAsync(AppUser user, Update update, string getDescription);
}