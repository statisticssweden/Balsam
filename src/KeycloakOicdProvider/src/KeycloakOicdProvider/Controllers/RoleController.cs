using Microsoft.AspNetCore.Mvc;
using OicdProvider.Controllers;
using System.ComponentModel.DataAnnotations;

namespace KeycloakOicdProvider.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoleController : RoleApiController
    {
        public override IActionResult CreateRole([FromQuery(Name = "name"), Required] string name)
        {
            throw new NotImplementedException();
        }
    }
}