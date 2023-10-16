using ChatProvider.Models;
using Microsoft.AspNetCore.Mvc;
using RocketChatChatProvider.Client;

namespace RocketChatChatProvider.Controllers
{
    public class AreaController : ChatProvider.Controllers.AreaApiController
    {
        private readonly ILogger<AreaController> _logger;
        private IRocketChatClient _rocketChatClient;

        public AreaController(ILogger<AreaController> logger, IRocketChatClient rocketChatClient)
        {
            _logger = logger;
            _rocketChatClient = rocketChatClient;
        }

        public override async Task<IActionResult> CreateArea([FromBody] CreateAreaRequest? createAreaRequest)
        {
            _rocketChatClient.CreateArea(createAreaRequest);

            //TODO: return channel Id, this can be used from workspace to create messages to a channel with id

            return Ok();
        }
    }
}
