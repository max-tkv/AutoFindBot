using Microsoft.Extensions.Logging;
using AutoFindBot.Abstractions;
using AutoFindBot.Entities;
using AutoFindBot.Lookups;
using AutoFindBot.Extensions;
using AutoFindBot.Models.TradeDealer;
using AutoFindBot.Utils.Helpers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Emoji = AutoFindBot.Lookups.Emoji;

namespace AutoFindBot.Services;

public class MessageService : IMessageService
{
    private readonly IKeyboardService _keyboardService;
    private readonly ILogger<MessageService> _logger;

    public MessageService(IKeyboardService keyboardService, ILogger<MessageService> logger)
    {
        _logger = logger;
        _keyboardService = keyboardService;
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

    public async Task SendNewAutoMessageAsync(TelegramBotClient botClient, AppUser user, UserFilter userFilter, TradeDealerResult newAutoList)
    {
        if (!newAutoList.CarInfos.Any())
        {
            return;
        }

        var message = $"По вашему фильтру \"{userFilter.Title}\" найдено *{newAutoList.CarInfos.Count}* новых автомобилей.\n\n\n";
        foreach (var carInfo in newAutoList.CarInfos)
        {
            message = message + 
                      $"*{carInfo.Title}*\n" +
                      $"Год: {carInfo.Year}\n" +
                      $"Цена: {carInfo.Price} руб.\n" +
                      $"Город: {carInfo.Company.Сity.Title}\n" +
                      $"Дата добавления: {carInfo.PublishedAt}";
            message += "\n\n\n";
        }
        
        await botClient.SendTextMessageAsync(user.ChatId, message);
    }

    #region Приватные методы
    
    //

    #endregion
}