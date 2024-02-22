using Balsam.Api.Models;
using BalsamApi.Server.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using System.ComponentModel.DataAnnotations;

namespace Balsam.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KnowledgeLibraryController : BalsamApi.Server.Controllers.KnowledgeLibraryApiController
    {
        private readonly HubClient _hubClient;
        private readonly ILogger<ProjectController> _logger;

        public KnowledgeLibraryController(IOptionsSnapshot<CapabilityOptions> capabilityOptions, ILogger<ProjectController> logger, HubClient hubClient)
        {
            _hubClient = hubClient;
            _logger = logger;
            capabilityOptions.Get(Capabilities.Git);
            capabilityOptions.Get(Capabilities.Authentication);
        }
        public async override Task<IActionResult> ListKnowledgeLibaries() //A. Implementera
        {
            _logger.LogInformation("calling endpoint: Listing Knowledgelibraries");
            var knowledgeLibraries = await _hubClient.ListKnowledgelibraries();
            return Ok(knowledgeLibraries);
        }

        public override Task<IActionResult> ListKnowledgeLibaryFileContent([FromRoute(Name = "libraryId"), Required] string libraryId, [FromRoute(Name = "fileId"), Required] string fileId)
        {
            throw new NotImplementedException();
        }

        public override Task<IActionResult> ListKnowledgeLibaryFiles([FromRoute(Name = "libraryId"), Required] string libraryId)
        {
            throw new NotImplementedException();
        }
    }
}
