using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Options;
using AutoFindBot.Abstractions;
using AutoFindBot.Entities;
using AutoFindBot.Exceptions;
using AutoFindBot.Models.ConfigurationOptions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace AutoFindBot.Services;

public class AppUserService : IAppUserService
{
    private readonly IUnitOfWork _unitOfWork;
    private readonly TelegramBotClient _botClient;
    private readonly IOptions<PaymentsOptions> _paymentsOptions;
    private readonly IOptions<RequiredSubscriptionsOptions> _requiredSubscriptionsOptions;
    private readonly ILogger<AppUserService> _logger;

    public AppUserService(IUnitOfWork unitOfWork, IOptions<PaymentsOptions> paymentsOptions,
        IOptions<RequiredSubscriptionsOptions> requiredSubscriptionsOptions, TelegramBot telegramBot,
        ILogger<AppUserService> logger)
    {
        _logger = logger;
        _unitOfWork = unitOfWork;
        _paymentsOptions = paymentsOptions;
        _botClient = telegramBot.GetBot().Result;
        _requiredSubscriptionsOptions = requiredSubscriptionsOptions;
    }
    
    public async Task<AppUser> GetOrCreateAsync(Update update)
    {
        var newUser = GetByUpdate(update);
        var user = await _unitOfWork.Users.GetByChatIdAsync(newUser.ChatId);
        if (user != null)
        {
            return user;
        }

        await _unitOfWork.Users.AddAsync(newUser);
        await _unitOfWork.SaveChangesAsync();
        return newUser;
    }
    
    public async Task<AppUser> GetOrCreateAsync(AppUser newUser)
    {
        var user = await _unitOfWork.Users.GetByChatIdAsync(newUser.ChatId);
        if (user != null)
        {
            return user;
        }

        await _unitOfWork.Users.AddAsync(newUser);
        await _unitOfWork.SaveChangesAsync();
        return newUser;
    }
    

    public AppUser GetByUpdate(Update update)
    {
        return update.Type switch
        {
            UpdateType.CallbackQuery => new AppUser
            {
                Username = update.CallbackQuery.From.Username,
                ChatId = update.CallbackQuery.Message.Chat.Id,
                FirstName = update.CallbackQuery.Message.From.FirstName,
                LastName = update.CallbackQuery.Message.From.LastName
            },
            UpdateType.Message => new AppUser
            {
                Username = update.Message.From.Username,
                ChatId = update.Message.Chat.Id,
                FirstName = update.Message.Chat.FirstName,
                LastName = update.Message.Chat.LastName
            },
            UpdateType.PreCheckoutQuery => new AppUser
            {
                Username = update.PreCheckoutQuery.From.Username,
                ChatId = update.PreCheckoutQuery.From.Id,
                FirstName = update.PreCheckoutQuery.From.FirstName,
                LastName = update.PreCheckoutQuery.From.LastName
            },
            _ => throw new ArgumentOutOfRangeException()
        };
    }

    public async Task CheckFreeRequestAsync(AppUser user)
    {
        if (user.Tarif == Tarif.Premium)
        {
            return;
        }
        
        if (_paymentsOptions.Value.Active)
        {
            var numberOfRequests = await _unitOfWork.Actions.GetNumberOfRequestsByUserAsync(user);
            if (numberOfRequests >= _paymentsOptions.Value.MaxFreeNumberRequests)
            {
                throw new FreeRequestsDidNotException();
            }
        }
    }
    
    public async Task CheckRequiredSubscriptionsAsync(AppUser user)
    {
        try
        {
            var value = _requiredSubscriptionsOptions.Value;
            if (value.Active && value.Groups.Any())
            {
                foreach (var group in value.Groups)
                {
                    var chatMember = await _botClient.GetChatMemberAsync(group.Id, user.ChatId);
                    if (chatMember.Status != ChatMemberStatus.Left)
                    {
                        continue;
                    }

                    throw new RequiredSubscriptionsException(
                        $"User: {user.Id}. Not included in the group: {group.Id}");
                }
            }
        }
        catch (RequiredSubscriptionsException e)
        {
            _logger.LogInformation(e, e.Message);
            throw;
        }
        catch (Exception e)
        {
            _logger.LogError(e, e.Message);
        }
    }
}