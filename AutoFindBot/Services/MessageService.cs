using Microsoft.Extensions.Logging;
using AutoFindBot.Abstractions;
using AutoFindBot.Entities;
using AutoFindBot.Invariants;
using Microsoft.Extensions.Configuration;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace AutoFindBot.Services;

public class MessageService : IMessageService
{
    private readonly IKeyboardService _keyboardService;
    private readonly ILogger<MessageService> _logger;
    private readonly IConfiguration _configuration;
    private readonly IUnitOfWork _unitOfWork;

    public MessageService(
        IKeyboardService keyboardService, 
        ILogger<MessageService> logger,
        IConfiguration configuration,
        IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _keyboardService = keyboardService;
        _configuration = configuration;
        _unitOfWork = unitOfWork;
    }
    
    public async Task SendStartMessage(TelegramBotClient botClient, AppUser user)
    {
        var keyboard = _keyboardService.GetStartMenuKeyboard();
        await botClient.SendTextMessageAsync(user.ChatId, Messages.Start, 
            ParseMode.Markdown, replyMarkup: keyboard);
    }

    public async Task SendErrorMessageAsync(TelegramBotClient botClient, AppUser user, string message)
    {
        await botClient.SendTextMessageAsync(user.ChatId, 
            $"{Invariants.Emoji.Cross} " + message, ParseMode.Markdown);
    }
    
    public async Task SendErrorMessageAsync(TelegramBotClient botClient, AppUser user, int messageId, string message)
    {
        await botClient.EditMessageTextAsync(user.ChatId, 
            messageId, $"{Invariants.Emoji.Cross} " + message, ParseMode.Markdown);
    }

    public async Task SendRequiredSubscriptionsAsync(TelegramBotClient botClient, AppUser user)
    {
        var keyboard = _keyboardService.GetRequiredSubscriptionsKeyboard();
        await botClient.SendTextMessageAsync(user.ChatId, Messages.RequiredSubscriptions, 
            ParseMode.Markdown, replyMarkup: keyboard);
    }

    public async Task SendPopupMessageAsync(TelegramBotClient botClient, AppUser user, Update update, string message)
    {
        await botClient.AnswerCallbackQueryAsync(update.CallbackQuery.Id, message, true);
    }

    public async Task SendNewAutoMessageAsync(
        TelegramBotClient botClient, 
        AppUser user, 
        UserFilter userFilter, 
        List<Car> newCarList)
    {
        if (!newCarList.Any())
        {
            await botClient.SendTextMessageAsync(
                user.ChatId, 
                Messages.NewCarNotFound, 
                ParseMode.Markdown, 
                replyMarkup: new ReplyKeyboardRemove());
            return;
        }
        
        foreach (var newCar in newCarList)
        {
            var message = Messages.NewCarMessage
                .Replace(":filterTitle", userFilter.Title)
                .Replace(":newCarTitle", newCar.Title)
                .Replace(":newCarYear", newCar.Year.ToString())
                .Replace(":newCarPrice", newCar.Price.ToString())
                .Replace(":newCarСity", newCar.Сity)
                .Replace(":newCarUrl", GetUrlBySource(newCar));
            
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
        await botClient.SendTextMessageAsync(user.ChatId, Messages.UserSettingsTitle, 
            ParseMode.Markdown, replyMarkup: keyboard);
    }

    public async Task SendUserFiltersMessageAsync(TelegramBotClient botClient, Update update, AppUser user)
    {
        var userFilters = await _unitOfWork.UserFilters.GetByUserAsync(user);
        var keyboard = _keyboardService.GetUserFiltersKeyboard(userFilters);
        await botClient.SendTextMessageAsync(
            user.ChatId, 
            Messages.UserFiltersTitle.Replace(":filtersCount", userFilters.Count.ToString()), 
            ParseMode.Markdown, 
            replyMarkup: keyboard);
    }

    public async Task SendUserConfirmationMessageAsync(TelegramBotClient botClient, AppUser user)
    {
        var keyboard = _keyboardService.GetStartMenuKeyboard();
        await botClient.SendTextMessageAsync(user.ChatId, Messages.UserConfirmation, 
            ParseMode.Markdown, replyMarkup: keyboard);
    }

    #region Приватные методы

    private string GetUrlBySource(Car car)
    {
        return car.Source switch
        {
            Source.Avito => $"{_configuration[SourceCarBaseUrlPaths.Avito]}{car.Url}",
            Source.TradeDealer => $"{_configuration[SourceCarBaseUrlPaths.TradeDealer]}/{car.Url}",
            Source.AutoRu => $"{_configuration[SourceCarBaseUrlPaths.AutoRu]}{car.Url}",
            Source.KeyAutoProbeg => $"{car.Url}",
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    #endregion
}