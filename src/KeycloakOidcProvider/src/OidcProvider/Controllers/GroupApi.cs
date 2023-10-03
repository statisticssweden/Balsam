/*
 * OidcProvider
 *
 * This a service contract for the OidcProvider in Balsam.
 *
 * The version of the OpenAPI document: 2.0
 * 
 * Generated by: https://openapi-generator.tech
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Threading;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using OidcProvider.Attributes;
using OidcProvider.Models;

namespace OidcProvider.Controllers
{ 
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    public abstract class GroupApiController : ControllerBase
    { 
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Adds a user to the group</remarks>
        /// <param name="groupId">The id for the group</param>
        /// <param name="addUserToGroupRequest">Definition of the user to add to the group</param>
        /// <response code="200">Success</response>
        /// <response code="400">Error respsone for 400</response>
        [HttpPost]
        [Route("/api/v1/groups/{groupId}/users")]
        [Consumes("application/json")]
        [ValidateModelState]
        [SwaggerOperation("AddUserToGroup")]
        [SwaggerResponse(statusCode: 400, type: typeof(Problem), description: "Error respsone for 400")]
        public abstract Task<IActionResult> AddUserToGroup([FromRoute (Name = "groupId")][Required]string groupId, [FromBody]AddUserToGroupRequest? addUserToGroupRequest);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Creates a new group</remarks>
        /// <param name="createGroupRequest">Definition of a new group</param>
        /// <response code="200">Success</response>
        /// <response code="400">Error respsone for 400</response>
        [HttpPost]
        [Route("/api/v1/groups")]
        [Consumes("application/json")]
        [ValidateModelState]
        [SwaggerOperation("CreateGroup")]
        [SwaggerResponse(statusCode: 200, type: typeof(GroupCreatedResponse), description: "Success")]
        [SwaggerResponse(statusCode: 400, type: typeof(Problem), description: "Error respsone for 400")]
        public abstract Task<IActionResult> CreateGroup([FromBody]CreateGroupRequest? createGroupRequest);
    }
}