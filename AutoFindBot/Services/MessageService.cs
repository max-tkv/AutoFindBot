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
        if (!newCarList.Any())
        {
            await botClient.SendTextMessageAsync(
                user.ChatId, 
                "Новых объявлений не найдено!", 
                ParseMode.Markdown, 
                replyMarkup: new ReplyKeyboardRemove());
            return;
        }
        
        foreach (var newCar in newCarList)
        {
            var message =
                      $"По вашему фильтру *«{userFilter.Title}»*\n" +
                      $"Найдено новое объявление\n\n" +
                      $"{Emoji.Car.GetValue()}*{newCar.Title}*\n" +
                      $"Год: {newCar.Year}\n" +
                      $"Цена: {newCar.Price} руб.\n" +
                      $"Город: {newCar.Сity}\n" +
                      $"[Открыть объявление]({GetUrlBySource(newCar)})";

            
            if (newCar.ImageUrls.Any())
            {
                var media = GetMediaMessage(newCar.ImageUrls, message);
                await botClient.SendMediaGroupAsync(
                    user.ChatId, 
                    media);
            }
            else
            {
                await botClient.SendTextMessageAsync(
                    user.ChatId, 
                    message, 
                    ParseMode.Markdown, 
                    replyMarkup: new ReplyKeyboardRemove());   
            }
        }
    }

    private IEnumerable<IAlbumInputMedia> GetMediaMessage(List<Image> newCarImageUrls, string message)
    {
        var media = new List<IAlbumInputMedia>();
        var messageFill = false;
        foreach (var imageUrl in newCarImageUrls)
        {
            if (media.Count >= 10)
            {
                continue;    
            }
                    
            var imagesOrderBy = imageUrl.Urls.OrderByDescending(x => long.Parse(x.Key
                .Replace("x", String.Empty)
                .Replace("n", String.Empty)
                .Replace("small", "0")
                .Replace("thumb_m", "0")));
            var imageBestUrl = imagesOrderBy.FirstOrDefault()!.Value;
            
            if (imageBestUrl == null) continue;

            if (imageBestUrl.Substring(0, 2) == "//")
            {
                imageBestUrl = imageBestUrl.Replace("//", "https://");   
            }

            InputMediaPhoto mediaPhoto;
            if (messageFill == false)
            {
                messageFill = true;
                mediaPhoto = new InputMediaPhoto(imageBestUrl)
                {
                    Caption = message,
                    ParseMode = ParseMode.Markdown
                };
            }
            else
            {
                mediaPhoto = new InputMediaPhoto(imageBestUrl);   
            }
            media.Add(mediaPhoto);
        }

        return media;
    }

    public async Task SendSettingsCommands(TelegramBotClient botClient, Update update, AppUser user)
    {
        var keyboard = _keyboardService.GetUserSettingsKeyboard();
        await botClient.SendTextMessageAsync(user.ChatId, Messages.UserSettings.GetDescription(), 
            ParseMode.Markdown, replyMarkup: keyboard);
    }

    #region Приватные методы

    private string GetUrlBySource(Car car)
    {
        return car.Source switch
        {
            Source.Avito => $"{_configuration["Integration:Avito:BaseUrl"]}{car.Url}",
            Source.TradeDealer => $"{_configuration["Integration:TradeDealer:SiteUrl"]}/{car.Url}",
            Source.AutoRu => $"{_configuration["Integration:AutoRu:BaseUrl"]}{car.Url}",
            Source.KeyAutoProbeg => $"{car.Url}"
        };
    }

    #endregion
}