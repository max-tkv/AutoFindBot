using Microsoft.Extensions.Configuration;
using Telegram.Bot;

namespace AutoFindBot.Services
{
    public class TelegramBotService
    {
        private readonly IConfiguration _configuration;
        private TelegramBotClient? _botClient;

        public TelegramBotService(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        public async Task<TelegramBotClient> GetBotAsync(CancellationToken stoppingToken = default)
        {
            if (_botClient != null)
            {
                return _botClient;
            }
            
            _botClient = new TelegramBotClient(_configuration["Token"]);

            var hook = $"{_configuration["Url"]}{_configuration["UpdatePath"]}";
            await _botClient.SetWebhookAsync(hook, cancellationToken: stoppingToken);

            return _botClient;
        }
    }
}