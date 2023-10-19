using ChatProvider.Models;
using Microsoft.AspNetCore.Mvc;
using RocketChatChatProvider.Client;

namespace RocketChatChatProvider.Controllers
{
    public class AreaController : ChatProvider.Controllers.AreaApiController
    {
        private readonly ILogger<AreaController> _logger;
        private readonly IRocketChatClient _client;

        public AreaController(ILogger<AreaController> logger, IRocketChatClient client)
        {
            _logger = logger;
            _client = client;
        }

        public override async Task<IActionResult> CreateArea([FromBody] CreateAreaRequest? createAreaRequest)
        {
            if (createAreaRequest == null || string.IsNullOrEmpty(createAreaRequest.Name))
            {
                return BadRequest(new Problem {Type = "404", Title = "Request must have a value"});
            }

            var name = NameUtil.SanitizeName(createAreaRequest.Name);
            try
            {
                var area = await _client.CreateArea(name);
                if (area != null)
                {
                    return Ok(area);
                }

                return BadRequest(new Problem {Type = "404", Title = "Could not create channel"});
            }
            catch (Exception e)
            {
                _logger.LogError(e, "An error occurred when creating area {area}", createAreaRequest.Name);
                return BadRequest(new Problem {Type = "404", Title = "Could not create channel"});
            }
        }
    }
}
