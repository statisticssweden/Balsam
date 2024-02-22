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
