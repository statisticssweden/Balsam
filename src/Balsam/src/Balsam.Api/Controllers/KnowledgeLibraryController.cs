using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Balsam.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KnowledgeLibraryController : BalsamApi.Server.Controllers.KnowledgeLibraryApiController
    {
        private readonly HubClient _hubClient;
        private readonly ILogger<ProjectController> _logger;

        public KnowledgeLibraryController(ILogger<ProjectController> logger, HubClient hubClient)
        {
            _hubClient = hubClient;
            _logger = logger;
        }
        public async override Task<IActionResult> ListKnowledgeLibaries() //A. Implementera
        {
            _logger.LogInformation("calling endpoint: Listing Knowledgelibraries");
            try
            {
                var knowledgeLibraries = await _hubClient.ListKnowledgeLibraries();
                return Ok(knowledgeLibraries);
            }
            catch(Exception ex)
            {
                _logger.LogError(ex, "Error listing knowledgelibraries");
                return BadRequest(ex);
            }
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
