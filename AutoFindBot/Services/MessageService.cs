using Microsoft.Extensions.Logging;
using AutoFindBot.Abstractions;
using AutoFindBot.Entities;
using AutoFindBot.Lookups;
using AutoFindBot.Extensions;
using AutoFindBot.Models.TradeDealer;
using AutoFindBot.Utils.Helpers;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Emoji = AutoFindBot.Lookups.Emoji;

namespace AutoFindBot.Services;

public class MessageService : IMessageService
{
    private readonly IKeyboardService _keyboardService;
    private readonly ILogger<MessageService> _logger;
    private readonly IConfiguration _configuration;

    public MessageService(
        IKeyboardService keyboardService, 
        ILogger<MessageService> logger,
        IConfiguration configuration)
    {
        _logger = logger;
        _keyboardService = keyboardService;
        _configuration = configuration;
    }
    
    public async Task SendStartMessage(TelegramBotClient botClient, AppUser user)
    {
        var keyboard = _keyboardService.GetStartMenuKeyboard();
        await botClient.SendTextMessageAsync(user.ChatId, Messages.Start.GetDescription(), 
            ParseMode.Markdown, replyMarkup: keyboard);
    }

    public async Task SendErrorMessageAsync(TelegramBotClient botClient, AppUser user, string message)
    {
        await botClient.SendTextMessageAsync(user.ChatId, 
            $"{Emoji.Cross.GetValue()} " + message, ParseMode.Markdown);
    }
    
    public async Task SendErrorMessageAsync(TelegramBotClient botClient, AppUser user, int messageId, string message)
    {
        await botClient.EditMessageTextAsync(user.ChatId, 
            messageId, $"{Emoji.Cross.GetValue()} " + message, ParseMode.Markdown);
    }

    public async Task SendRequiredSubscriptionsAsync(TelegramBotClient botClient, AppUser user)
    {
        var keyboard = _keyboardService.GetRequiredSubscriptionsKeyboard();
        await botClient.SendTextMessageAsync(user.ChatId, Messages.RequiredSubscriptions.GetDescription(), 
            ParseMode.Markdown, replyMarkup: keyboard);
    }

    public async Task SendPopupMessageAsync(TelegramBotClient botClient, AppUser user, Update update, string message)
    {
        await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, message, true);
    }

    public async Task SendNewAutoMessageAsync(TelegramBotClient botClient, AppUser user, UserFilter userFilter, List<Car> newCarList)
    {
        var message = $"По вашему фильтру \"{userFilter.Title}\" найдено *{newCarList.Count}* новых автомобилей.\n\n\n";
        foreach (var newCar in newCarList)
        {
            message = message +
                      $"*{newCar.Title}*\n" +
                      $"Год: {newCar.Year}\n" +
                      $"Цена: {newCar.Price} руб.\n" +
                      $"Город: {newCar.Сity}\n" +
                      $"Дата добавления: {newCar.PublishedAt}\n";
            switch (newCar.Source)
            {
                case Source.Avito:
                    message += $"Ссылка: [Открыть]({_configuration["Integration:Avito:BaseUrl"]}{newCar.Url})";
                    break;
                case Source.TradeDealer:
                    message += $"Ссылка: [Открыть]({_configuration["Integration:TradeDealer:SiteUrl"]}/{newCar.Url})";
                    break;
                case Source.KeyAutoProbeg:
                    message += $"Ссылка: [Открыть]({newCar.Url})";
                    break;
            }
            message += "\n\n\n";
        }
        
        await botClient.SendTextMessageAsync(user.ChatId, message, ParseMode.Markdown, replyMarkup: new ReplyKeyboardRemove());
    }

    public async Task SendSettingsCommands(TelegramBotClient botClient, Update update, AppUser user)
    {
        var keyboard = _keyboardService.GetUserSettingsKeyboard();
        await botClient.SendTextMessageAsync(user.ChatId, Messages.UserSettings.GetDescription(), 
            ParseMode.Markdown, replyMarkup: keyboard);
    }

    #region Приватные методы
    
    //

    #endregion
}