using Telegram.Bot.Types.Enums;

namespace AutoFindBot.Invariants;

public class Messages
{
    public const string Start = "Добро пожаловать!\n" +
                                "Бот заработает только после личного подтверждения администратором.\n" +
                                "Ожидайте подтверждения";
    
    public const string ErrorPayed = "При обработке платежа произошла ошибка, пожалуйста повторите попытку позже.";
    
    public const string RequiredSubscriptions = "❗ Для использования бота, вы должны быть подписаны на каналы:";
    
    public const string RequiredSubscriptionsPopupError = "Вы не подписались на каналы!";
    
    public const string UserSettingsTitle = "Настройки";
    
    public const string UserFiltersTitle = "У вас *:filtersCount* фильтр(ов):";
    
    public const string NewCarMessage = $"По вашему фильтру *«:filterTitle»*\n" +
                                        $"Найдено новое объявление\n\n" +
                                        $"{Emoji.Car}*:newCarTitle*\n" +
                                        $"Год: :newCarYear\n" +
                                        $"Цена: :newCarPrice руб.\n" +
                                        $"Город: :newCarСity\n" +
                                        $"[Открыть объявление](:newCarUrl)";
    
    public const string NewCarNotFound = "Новых объявлений не найдено!";
}