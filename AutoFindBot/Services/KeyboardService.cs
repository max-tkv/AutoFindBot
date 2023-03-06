using Microsoft.Extensions.Options;
using AutoFindBot.Abstractions;
using AutoFindBot.Models.ConfigurationOptions;
using Telegram.Bot.Types.ReplyMarkups;

namespace AutoFindBot.Services;

public class KeyboardService : IKeyboardService
{
    private readonly IOptions<RequiredSubscriptionsOptions> _requiredSubscriptionsOptions;

    public KeyboardService(IOptions<RequiredSubscriptionsOptions> requiredSubscriptionsOptions)
    {
        _requiredSubscriptionsOptions = requiredSubscriptionsOptions;
    }
    
    public IReplyMarkup GetResultKeyboard(IPaginationResult pagination)
    {
        // if (pagination is IPaginationResult<SearchResultItem> paginationResult)
        // {
        //     return GetPageResultKeyboard(paginationResult, false);
        // }

        throw new Exception("Не удалось определить тип ответа");
    }

    public IReplyMarkup GetStartMenuKeyboard()
    {
        return new InlineKeyboardMarkup(new List<IEnumerable<InlineKeyboardButton>>()
        {
            // new []
            // {
            //     InlineKeyboardButton.WithCallbackData(
            //         text: "Из фильма",
            //         callbackData: CommandNames.FilmsCommand
            //     )
            // }
        });
    }

    public IReplyMarkup GetRemovedKeyboard()
    {
        return new ReplyKeyboardRemove() { Selective = true };
    }

    public IReplyMarkup GetRequiredSubscriptionsKeyboard()
    {
        var groups = _requiredSubscriptionsOptions.Value.Groups;
        var keyboardButtons = groups.ToList()
            .Select(x => new[] { new InlineKeyboardButton() 
                { Text = x.Title, Url = $"https://t.me/{x.Id.Replace("@", string.Empty)}" } })
            .ToList();

        if (!keyboardButtons.Any())
        {
            throw new Exception("KeyboardButtons empty");
        }
        
        keyboardButtons.Add(new []
        {
            new InlineKeyboardButton()
            {
                Text = "✅ Открыть доступ | Я подписался (ась)",
                CallbackData = "CheckRequiredSubscriptions"
            }
        });

        var resultInlineKeyboard = new InlineKeyboardMarkup(
            new List<IEnumerable<InlineKeyboardButton>>(keyboardButtons));

        return resultInlineKeyboard;
    }

    public IReplyMarkup GetUserSettingsKeyboard()
    {
        return new InlineKeyboardMarkup(new List<IEnumerable<InlineKeyboardButton>>()
        {
            new []
            {
                InlineKeyboardButton.WithCallbackData(
                    text: "Фильтры",
                    callbackData: CommandNames.ChangeFiltersCommand
                ),
                InlineKeyboardButton.WithCallbackData(
                    text: "Источники",
                    callbackData: CommandNames.ChangeSourcesCommand
                )
            }
        });
    }


    #region Приватные методы
    
    //todo
    
    #endregion
}