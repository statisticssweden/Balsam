using ChatProvider.Models;
using Microsoft.AspNetCore.Mvc;
using RocketChatChatProvider.Client;

namespace RocketChatChatProvider.Controllers
{
    public class AreaController : ChatProvider.Controllers.AreaApiController
    {
        private readonly ILogger<AreaController> _logger;
        private IRocketChatClient _client;

        public AreaController(ILogger<AreaController> logger, IRocketChatClient client)
        {
            _logger = logger;
            _client = client;
        }

        public override async Task<IActionResult> CreateArea([FromBody] CreateAreaRequest? createAreaRequest)
        {
            if (createAreaRequest == null || string.IsNullOrEmpty(createAreaRequest.Name))
            {
                return BadRequest(createAreaRequest);
            }

            var name = NameUtil.SanitizeName(createAreaRequest.Name);
            var area = await _client.CreateArea(name);

            return Ok(area);
        }
    }
}
