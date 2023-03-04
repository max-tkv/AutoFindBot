using System.ComponentModel;

namespace AutoFindBot.Lookups;

public enum Messages
{
    [Description("Добро пожаловать!")]
    Start,

    [Description("При обработке платежа произошла ошибка, пожалуйста повторите попытку позже.")]
    ErrorPayed,
    
    [Description("❗ Для использования бота, вы должны быть подписаны на каналы:")]
    RequiredSubscriptions,
    
    [Description("Вы не подписались на каналы!")]
    RequiredSubscriptionsPopupError
}