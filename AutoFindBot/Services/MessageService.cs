using Microsoft.Extensions.Logging;
using AutoFindBot.Abstractions;
using AutoFindBot.Entities;
using AutoFindBot.Invariants;
using AutoFindBot.Lookups;
using AutoFindBot.Repositories;
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
    private readonly IUserFilterRepository _userFilterRepository;

    public MessageService(
        IKeyboardService keyboardService, 
        ILogger<MessageService> logger,
        IConfiguration configuration,
        IUserFilterRepository userFilterRepository)
    {
        _logger = logger;
        _keyboardService = keyboardService;
        _configuration = configuration;
        _userFilterRepository = userFilterRepository;
    }
    
    public async Task SendStartMessageAsync(
        TelegramBotClient botClient, 
        AppUser user, 
        CancellationToken stoppingToken = default)
    {
        var keyboard = _keyboardService.GetStartMenuKeyboard();
        await botClient.SendTextMessageAsync(
            user.ChatId, 
            Messages.Start, 
            ParseMode.Markdown, 
            replyMarkup: keyboard, 
            cancellationToken: stoppingToken);
    }

    public async Task SendErrorMessageAsync(
        TelegramBotClient botClient, 
        AppUser user, 
        string message, 
        CancellationToken stoppingToken = default)
    {
        await botClient.SendTextMessageAsync(user.ChatId, 
            $"{Invariants.Emoji.Cross} " + message, 
            ParseMode.Markdown, 
            cancellationToken: stoppingToken);
    }
    
    public async Task SendErrorMessageAsync(
        TelegramBotClient botClient, 
        AppUser user, 
        int messageId, 
        string message, 
        CancellationToken stoppingToken = default)
    {
        await botClient.EditMessageTextAsync(
            user.ChatId, 
            messageId, $"{Invariants.Emoji.Cross} " + message, 
            ParseMode.Markdown, 
            cancellationToken: stoppingToken);
    }

    public async Task SendRequiredSubscriptionsAsync(
        TelegramBotClient botClient, 
        AppUser user, 
        CancellationToken stoppingToken = default)
    {
        var keyboard = _keyboardService.GetRequiredSubscriptionsKeyboard();
        await botClient.SendTextMessageAsync(
            user.ChatId, 
            Messages.RequiredSubscriptions, 
            ParseMode.Markdown, 
            replyMarkup: keyboard, 
            cancellationToken: stoppingToken);
    }

    public async Task SendPopupMessageAsync(
        TelegramBotClient botClient, 
        AppUser user, 
        Update update, 
        string message, 
        CancellationToken stoppingToken = default)
    {
        await botClient.AnswerCallbackQueryAsync(
            update.CallbackQuery.Id, 
            message, 
            true, 
            cancellationToken: stoppingToken);
    }

    public async Task SendNewAutoMessageAsync(
        TelegramBotClient botClient, 
        AppUser user, 
        UserFilter userFilter, 
        List<Car> newCarList, 
        CancellationToken stoppingToken = default)
    {
        if (!newCarList.Any())
        {
            await botClient.SendTextMessageAsync(
                user.ChatId, 
                Messages.NewCarNotFound
                    .Replace(":filterTitle", userFilter.Title), 
                ParseMode.Markdown, 
                replyMarkup: new ReplyKeyboardRemove(), 
                cancellationToken: stoppingToken);
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
                    media, 
                    cancellationToken: stoppingToken);
            }
            else
            {
                await botClient.SendTextMessageAsync(
                    user.ChatId, 
                    message, 
                    ParseMode.Markdown, 
                    replyMarkup: new ReplyKeyboardRemove(), 
                    cancellationToken: stoppingToken);   
            }
        }
    }

    public async Task SendSettingsCommandsAsync(
        TelegramBotClient botClient, 
        Update update, 
        AppUser user, 
        CancellationToken stoppingToken = default)
    {
        var keyboard = _keyboardService.GetUserSettingsKeyboard();
        await botClient.SendTextMessageAsync(
            user.ChatId, 
            Messages.UserSettingsTitle, 
            ParseMode.Markdown, 
            replyMarkup: keyboard, 
            cancellationToken: stoppingToken);
    }

    public async Task SendUserFiltersMessageAsync(
        TelegramBotClient botClient, 
        Update update, 
        AppUser user, 
        CancellationToken stoppingToken = default)
    {
        var userFilters = await _userFilterRepository.GetByUserAsync(user, stoppingToken);
        var keyboard = _keyboardService.GetUserFiltersKeyboard(userFilters);
        await botClient.SendTextMessageAsync(
            user.ChatId, 
            Messages.UserFiltersTitle.Replace(":filtersCount", userFilters.Count.ToString()), 
            ParseMode.Markdown, 
            replyMarkup: keyboard, 
            cancellationToken: stoppingToken);
    }

    public async Task SendUserConfirmationMessageAsync(
        TelegramBotClient botClient, 
        AppUser user, 
        CancellationToken stoppingToken = default)
    {
        var keyboard = _keyboardService.GetStartMenuKeyboard();
        await botClient.SendTextMessageAsync(
            user.ChatId, 
            Messages.UserConfirmation, 
            ParseMode.Markdown, 
            replyMarkup: keyboard, 
            cancellationToken: stoppingToken);
    }

    #region Приватные методы

    private string GetUrlBySource(Car car)
    {
        return car.SourceType switch
        {
            SourceType.Avito => $"{_configuration[SourceCarBaseUrlPaths.Avito]}{car.Url}",
            SourceType.TradeDealer => $"{_configuration[SourceCarBaseUrlPaths.TradeDealer]}/{car.Url}",
            SourceType.AutoRu => $"{_configuration[SourceCarBaseUrlPaths.AutoRu]}{car.Url}",
            SourceType.KeyAutoProbeg => $"{car.Url}",
            SourceType.Drom => $"{car.Url}",
            SourceType.Youla => $"{_configuration[SourceCarBaseUrlPaths.Youla]}{car.Url}",
            _ => throw new ArgumentOutOfRangeException()
        };
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

    #endregion
}