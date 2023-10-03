using System.ComponentModel.DataAnnotations;
using Keycloak.OidcProvider.Client;
using Microsoft.AspNetCore.Mvc;
using OidcProvider.Controllers;
using OidcProvider.Models;

namespace Keycloak.OidcProvider.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class GroupController : GroupApiController
    {
        private readonly ILogger _logger;
        private readonly IKeyCloakClient _keyCloakClient;

        public GroupController(ILogger<GroupController> logger, IKeyCloakClient keyCloakClient)
        {
            _logger = logger;
            _keyCloakClient = keyCloakClient;
        }

        public override Task<IActionResult> AddUserToGroup(string groupId, AddUserToGroupRequest? addUserToGroupRequest)
        {
            throw new NotImplementedException();
        }

        public override async Task<IActionResult> CreateGroup(CreateGroupRequest? createGroupRequest)
        {
            var group = await _keyCloakClient.CreateGroup(createGroupRequest.Name);

            return Ok(group);
        }
    }
}