using AutoFindBot.Entities;
using Telegram.Bot.Types;

namespace AutoFindBot.Abstractions;

public interface IPaymentService
{
    Task SendInvoiceAsync(
        AppUser user, 
        CancellationToken stoppingToken = default);
    
    Task SavePaymentAsync(
        Update update, 
        AppUser user, 
        CancellationToken stoppingToken = default);
}