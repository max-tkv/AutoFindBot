using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AutoFindBot.Abstractions;
using AutoFindBot.Entities;
using AutoFindBot.Lookups;
using AutoFindBot.Models.ConfigurationOptions;
using AutoFindBot.Utils.Helpers;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Payments;

namespace AutoFindBot.Services;

public class PaymentService : IPaymentService
{
    private readonly TelegramBotClient _botClient;
    private readonly IOptions<PaymentsOptions> _paymentsOptions;
    private readonly ILogger<PaymentService> _logger;
    private readonly IUnitOfWork _unitOfWork;

    public PaymentService(IOptions<PaymentsOptions> paymentsOptions, TelegramBot telegramBot, 
        ILogger<PaymentService> logger, IUnitOfWork unitOfWork)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _botClient = telegramBot.GetBot().Result;
        _paymentsOptions = paymentsOptions;
    }

    public async Task SendInvoiceAsync(AppUser user)
    {
        await _botClient.SendInvoiceAsync(user.ChatId,
            _paymentsOptions.Value.Title,
            _paymentsOptions.Value.Description,
            "payments",
            _paymentsOptions.Value.Token,
            _paymentsOptions.Value.Currency,
            new List<LabeledPrice>()
            {
                new LabeledPrice()
                {
                    Label = _paymentsOptions.Value.Price.Label,
                    Amount = _paymentsOptions.Value.Price.Amount,
                }
            },
            needEmail: true);
    }

    public async Task SavePaymentAsync(Update update, AppUser user)
    {
        try
        {
            await _unitOfWork.Payments.AddAsync(new Payment()
            {
                UserId = user.Id,
                PayId = update.PreCheckoutQuery.Id,
                Address = update.PreCheckoutQuery.OrderInfo?.ShippingAddress?.ToString(),
                Email = update.PreCheckoutQuery.OrderInfo?.ShippingAddress?.ToString(),
                PhoneNumber = update.PreCheckoutQuery.OrderInfo?.PhoneNumber,
                Payload = update.PreCheckoutQuery.InvoicePayload,
                OptionId = update.PreCheckoutQuery.ShippingOptionId,
                Currency = update.PreCheckoutQuery.Currency,
                Amount = update.PreCheckoutQuery.TotalAmount
            });

            user.Tarif = Tarif.Premium;
            _unitOfWork.Users.Update(user);
            
            await _unitOfWork.SaveChangesAsync();
        }
        catch (Exception e)
        {
            await _botClient.AnswerPreCheckoutQueryAsync(update.PreCheckoutQuery.Id, Messages.ErrorPayed.GetDescription());
            throw;
        }
        
        await _botClient.AnswerPreCheckoutQueryAsync(update.PreCheckoutQuery.Id);
    }
}