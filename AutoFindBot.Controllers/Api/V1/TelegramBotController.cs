using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json;
using AutoFindBot.Abstractions;
using Telegram.Bot.Types;

namespace AutoFindBot.Controllers.Api.V1
{
    /// <inheritdoc />
    [ApiVersion("1")]
    [Route("api/v{version:apiVersion}/message/update")]
    [ApiController]
    public class TelegramBotController : ControllerBase
    {
        private readonly ICommandExecutorService _commandExecutorService;

        /// <inheritdoc />
        public TelegramBotController(ICommandExecutorService commandExecutorService)
        {
            _commandExecutorService = commandExecutorService;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="update"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<IActionResult> Update([FromBody]object update)
        {
            // /start => register user
            var upd = JsonConvert.DeserializeObject<Update>(update.ToString());
            if (upd?.Message?.Chat == null && upd?.CallbackQuery == null && upd?.PreCheckoutQuery == null)
            {
                return Ok();
            }
            try
            {
                await _commandExecutorService.ExecuteAsync(upd);
            }
            catch (Exception)
            {
                return Ok();
            }
            return Ok();
        }
    }
}