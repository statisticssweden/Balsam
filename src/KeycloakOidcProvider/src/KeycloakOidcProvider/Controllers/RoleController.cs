using System.ComponentModel.DataAnnotations;
using Keycloak.OidcProvider.Client;
using Microsoft.AspNetCore.Mvc;
using OidcProvider.Controllers;
using OidcProvider.Models;

namespace Keycloak.OidcProvider.Controllers
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

        public override IActionResult AddUserToRole([FromRoute(Name = "roleId"), Required] string roleId, [FromBody] AddUserToRoleRequest? addUserToRoleRequest)
        {
            throw new NotImplementedException();
        }

        public override IActionResult CreateRole([FromBody] CreateRoleRequest? createRoleRequest)
        {
            //TODO: Secure Name
            _keyCloakClient.CreateRole(createRoleRequest.Name);
            return Accepted();
        }
    }
}