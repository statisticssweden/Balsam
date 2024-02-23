using BalsamApi.Server.Models;
using Microsoft.AspNetCore.Mvc;
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
        private readonly KnowledgeLibraryClient _knowledgeLibraryClient;

        public KnowledgeLibraryController(ILogger<ProjectController> logger, HubClient hubClient, KnowledgeLibraryClient knowledgeLibraryClient)
        {
            _hubClient = hubClient;
            _logger = logger;
            _knowledgeLibraryClient = knowledgeLibraryClient;
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

        public async override Task<IActionResult> ListKnowledgeLibaryFileContent([FromRoute(Name = "libraryId"), Required] string libraryId, [FromRoute(Name = "fileId"), Required] string fileId)
        {
            try
            {
                //TODO Ensure the file exists
                string filePath = string.Empty;
                filePath = _knowledgeLibraryClient.GetRepositoryFilePath(libraryId, fileId);

                // Open the file
                var stream = System.IO.File.OpenRead(filePath);

                // Get the file extension
                var extension = System.IO.Path.GetExtension(filePath);

                // Determine the content type
                var provider = new Microsoft.AspNetCore.StaticFiles.FileExtensionContentTypeProvider();
                if (!provider.TryGetContentType(fileId, out var contentType))
                {
                    contentType = "application/octet-stream";
                }

                Response.Headers.Add("content-disposition", "inline");

                // Return the file stream
                return File(stream, contentType);
            }
            catch (Exception ex)
            {
                // Log the exception here
                return StatusCode(500, "Internal server error");
            }
        }

        public async override Task<IActionResult> ListKnowledgeLibaryFiles([FromRoute(Name = "libraryId"), Required] string libraryId)
        {
            try
            {
                //TODO Ensure the library exists and get the repositoryUrl
                var contents = _knowledgeLibraryClient.GetRepositoryContent(libraryId, "");
                return Ok(contents.ToArray());
            } catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing knowledge library files");
                return BadRequest(new Problem() { Status = 404, Title = "Error fetching knowledge libraries", Detail = "Error fetching knowledge libraries" });
            }
        }
    }
}
