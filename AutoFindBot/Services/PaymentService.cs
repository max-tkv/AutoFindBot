using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AutoFindBot.Abstractions;
using AutoFindBot.Entities;
using AutoFindBot.Invariants;
using AutoFindBot.Lookups;
using AutoFindBot.Models.ConfigurationOptions;
using AutoFindBot.Repositories;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Payments;

namespace AutoFindBot.Services;

public class PaymentService : IPaymentService
{
    private readonly TelegramBotClient _botClient;
    private readonly IOptions<PaymentsOptions> _paymentsOptions;
    private readonly ILogger<PaymentService> _logger;
    private readonly IPaymentRepository _paymentRepository;
    private readonly IAppUserRepository _appUserRepository;

    public PaymentService(
        IOptions<PaymentsOptions> paymentsOptions, 
        TelegramBotService telegramBotService, 
        ILogger<PaymentService> logger, 
        IPaymentRepository paymentRepository, 
        IAppUserRepository appUserRepository)
    {
        _logger = logger;
        _botClient = telegramBotService.GetBotAsync().Result;
        _paymentsOptions = paymentsOptions;
        _paymentRepository = paymentRepository;
        _appUserRepository = appUserRepository;
    }

    public async Task SendInvoiceAsync(
        AppUser user, 
        CancellationToken stoppingToken = default)
    {
        await _botClient.SendInvoiceAsync(user.ChatId,
            _paymentsOptions.Value.Title,
            _paymentsOptions.Value.Description,
            "payments",
            _paymentsOptions.Value.Token,
            _paymentsOptions.Value.Currency,
            new List<LabeledPrice>()
            {
                new()
                {
                    Label = _paymentsOptions.Value.Price.Label,
                    Amount = _paymentsOptions.Value.Price.Amount,
                }
            },
            needEmail: true, 
            cancellationToken: stoppingToken);
    }

    public async Task SavePaymentAsync(
        Update update, 
        AppUser user, 
        CancellationToken stoppingToken = default)
    {
        try
        {
            await _paymentRepository.AddAsync(new Payment()
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
            }, stoppingToken);

            user.Tarif = Tarif.Premium;
            await _appUserRepository.UpdateAsync(user, stoppingToken);
        }
        catch (Exception e)
        {
            await _botClient.AnswerPreCheckoutQueryAsync(
                update.PreCheckoutQuery.Id, 
                Messages.ErrorPayed, 
                stoppingToken);
            throw;
        }
        
        await _botClient.AnswerPreCheckoutQueryAsync(
            update.PreCheckoutQuery.Id, 
            stoppingToken);
    }
}