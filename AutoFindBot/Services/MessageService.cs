using Microsoft.Extensions.Logging;
using AutoFindBot.Abstractions;
using AutoFindBot.Entities;
using AutoFindBot.Lookups;
using AutoFindBot.Extensions;
using AutoFindBot.Utils.Helpers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Emoji = AutoFindBot.Lookups.Emoji;

namespace AutoFindBot.Services;

public class MessageService : IMessageService
{
    private readonly TelegramBotClient _botClient;
    private readonly IKeyboardService _keyboardService;
    private readonly ILogger<MessageService> _logger;

    public MessageService(TelegramBot telegramBot, IKeyboardService keyboardService, ILogger<MessageService> logger)
    {
        _logger = logger;
        _botClient = telegramBot.GetBot().Result;
        _keyboardService = keyboardService;
    }
    
    public async Task SendStartMessage(AppUser user)
    {
        var keyboard = _keyboardService.GetStartMenuKeyboard();
        await _botClient.SendTextMessageAsync(user.ChatId, Messages.Start.GetDescription(), 
            ParseMode.Markdown, replyMarkup: keyboard);
    }

    public async Task SendErrorMessageAsync(AppUser user, string message)
    {
        await _botClient.SendTextMessageAsync(user.ChatId, 
            $"{Emoji.Cross.GetValue()} " + message, ParseMode.Markdown);
    }
    
    public async Task SendErrorMessageAsync(AppUser user, int messageId, string message)
    {
        await _botClient.EditMessageTextAsync(user.ChatId, 
            messageId, $"{Emoji.Cross.GetValue()} " + message, ParseMode.Markdown);
    }

    public async Task SendRequiredSubscriptionsAsync(AppUser user)
    {
        var keyboard = _keyboardService.GetRequiredSubscriptionsKeyboard();
        await _botClient.SendTextMessageAsync(user.ChatId, Messages.RequiredSubscriptions.GetDescription(), 
            ParseMode.Markdown, replyMarkup: keyboard);
    }

    public async Task SendPopupMessageAsync(AppUser user, Update update, string message)
    {
        await _botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, message, true);
    }

    #region Приватные методы
    
    //

    #endregion
}