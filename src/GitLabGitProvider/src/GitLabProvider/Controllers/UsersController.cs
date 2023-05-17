using GitProvider.Controllers;
using GitProvider.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace GitLabProvider.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : UserApiController
    {
        public override IActionResult CreatePAT([FromRoute(Name = "id"), Required] string id)
        {
            return Ok(new UserPATCreatedResponse() { Name = $"{id} - PAT", Token = "XYZ TODO" });
        }

    }
}
