using Balsam.Api.Extensions;
using Balsam.Interfaces;
using Balsam.Model;
using BalsamApi.Server.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Balsam.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KnowledgeLibraryController : BalsamApi.Server.Controllers.KnowledgeLibraryApiController
    {
        private readonly ILogger<KnowledgeLibraryController> _logger;
        private readonly IKnowledgeLibraryService _knowledgeLibraryService;

        public KnowledgeLibraryController(ILogger<KnowledgeLibraryController> logger, IKnowledgeLibraryService knowledgeLibraryService)
        {
            _logger = logger;
            _knowledgeLibraryService = knowledgeLibraryService;
        }
        public async override Task<IActionResult> ListKnowledgeLibaries() //A. Implementera
        {
            _logger.LogInformation("calling endpoint: Listing Knowledgelibraries");
            try
            {
                var balsamKnowledgeLibraries = await _knowledgeLibraryService.ListKnowledgeLibraries();

                var knowledgeLibraries = balsamKnowledgeLibraries.Select(bkl => bkl.ToKnowledgeLibrary());
                return Ok(knowledgeLibraries);
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing knowledgelibraries");
                return BadRequest(ex);
            }
        }

        public async override Task<IActionResult> GetKnowledgeLibraryFileContent([FromRoute(Name = "libraryId"), Required] string libraryId, [FromRoute(Name = "fileId"), Required] string fileId)
        {
            try
            {
                //TODO: Pull knowledge library repo

                //TODO: Move stream to knowledgeLibraryService?
                string filePath = string.Empty;
                filePath = _knowledgeLibraryService.GetRepositoryFilePath(libraryId, fileId);

                // Open the file
                var stream = System.IO.File.OpenRead(filePath);


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
                _logger.LogError(ex, "Error listing knowledge library file content");
                return StatusCode(500, "Internal server error");
            }
        }

        public async override Task<IActionResult> ListKnowledgeLibraryFiles([FromRoute(Name = "libraryId"), Required] string libraryId)
        {
            try
            {
                var balsamRepofiles = await _knowledgeLibraryService.GetRepositoryFileTree(libraryId);
                var repoFiles = balsamRepofiles.Select(rf => rf.ToRepoFile());

                return Ok(repoFiles.ToArray());
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error listing knowledge library files");
                return BadRequest(new Problem() { Status = 404, Title = "Error fetching knowledge libraries", Detail = "Error fetching knowledge libraries" });
            }
        }
    }
}
