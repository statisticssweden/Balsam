using ChatProvider.Models;
using Microsoft.AspNetCore.Mvc;

namespace RocketChatChatProvider.Controllers
{
    public class AreaController : ChatProvider.Controllers.AreaApiController
    {
        public override Task<IActionResult> CreateArea([FromBody] CreateAreaRequest? createAreaRequest)
        {
            throw new NotImplementedException();
        }
    }
}
