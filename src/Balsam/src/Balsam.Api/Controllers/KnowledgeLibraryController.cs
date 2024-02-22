using BalsamApi.Server.Models;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace Balsam.Api.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class KnowledgeLibraryController : BalsamApi.Server.Controllers.KnowledgeLibraryApiController
    {
        public override Task<IActionResult> ListKnowledgeLibaries()
        {
            throw new NotImplementedException();
        }

        public async override Task<IActionResult> ListKnowledgeLibaryFileContent([FromRoute(Name = "libraryId"), Required] string libraryId, [FromRoute(Name = "fileId"), Required] string fileId)
        {
            try
            {
                //TODO Ensure the file exists
                string filePath = string.Empty;
                //filePath = KnowledgeLibraryClient.GetRepositoryFilePath(libraryId, fileId);

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

        public override Task<IActionResult> ListKnowledgeLibaryFiles([FromRoute(Name = "libraryId"), Required] string libraryId)
        {
            throw new NotImplementedException();
        }
    }
}
