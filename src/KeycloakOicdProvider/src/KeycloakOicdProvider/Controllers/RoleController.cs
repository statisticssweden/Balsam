using Microsoft.AspNetCore.Mvc;
using OicdProvider.Controllers;
using System.ComponentModel.DataAnnotations;
using Keycloak.OicdProvider.Client;

namespace Keycloak.OicdProvider.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class RoleController : RoleApiController
    {
        private readonly ILogger _logger;
        private readonly IKeyCloakClient _keyCloakClient;

        public RoleController(ILogger<RoleController> logger, IKeyCloakClient keyCloakClient)
        {
            _logger = logger;
            _keyCloakClient = keyCloakClient;
        }
        public override IActionResult CreateRole([FromQuery(Name = "name"), Required] string name)
        {
            //TODO: Secure Name
            _keyCloakClient.CreateRole(name);
            return Accepted();
        }
    }
}