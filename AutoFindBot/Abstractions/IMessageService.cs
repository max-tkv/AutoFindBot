using AutoFindBot.Entities;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace AutoFindBot.Abstractions;

public interface IMessageService
{
    Task SendStartMessageAsync(
        TelegramBotClient botClient, 
        AppUser user, 
        CancellationToken stoppingToken = default);
    
    Task SendErrorMessageAsync(
        TelegramBotClient botClient, 
        AppUser user, 
        string message, 
        CancellationToken stoppingToken = default);
    
    Task SendErrorMessageAsync(
        TelegramBotClient botClient, 
        AppUser user, 
        int messageId, 
        string message, 
        CancellationToken stoppingToken = default);
    
    Task SendRequiredSubscriptionsAsync(
        TelegramBotClient botClient, 
        AppUser user, 
        CancellationToken stoppingToken = default);
    
    Task SendPopupMessageAsync(
        TelegramBotClient botClient, 
        AppUser user, 
        Update update, 
        string getDescription, 
        CancellationToken stoppingToken = default);
    
    Task SendNewAutoMessageAsync(
        TelegramBotClient botClient, 
        AppUser user, 
        UserFilter userFilter, 
        List<Car> newCarList, 
        CancellationToken stoppingToken = default);
    
    Task SendSettingsCommandsAsync(
        TelegramBotClient botClient, 
        Update update, 
        AppUser user, 
        CancellationToken stoppingToken = default);
    
    Task SendUserFiltersMessageAsync(
        TelegramBotClient botClient, 
        Update update, 
        AppUser user, 
        CancellationToken stoppingToken = default);
    
    Task SendUserConfirmationMessageAsync(
        TelegramBotClient botClient, 
        AppUser user, 
        CancellationToken stoppingToken = default);
}