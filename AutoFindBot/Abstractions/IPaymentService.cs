using AutoFindBot.Entities;
using Telegram.Bot.Types;

namespace AutoFindBot.Abstractions;

public interface IPaymentService
{
    Task SendInvoiceAsync(AppUser user);
    Task SavePaymentAsync(Update update, AppUser user);
}