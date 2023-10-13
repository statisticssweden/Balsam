/*
 * ChatProvider
 *
 * This a service contract for the ChatProvider in Balsam.
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
using ChatProvider.Attributes;
using ChatProvider.Models;

namespace ChatProvider.Controllers
{ 
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    public abstract class AreaApiController : ControllerBase
    { 
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Creates a new Area</remarks>
        /// <param name="createAreaRequest">Definition of a new area</param>
        /// <response code="200">Success</response>
        /// <response code="400">Error respsone for 400</response>
        [HttpPost]
        [Route("/api/v1/areas")]
        [Consumes("application/json")]
        [ValidateModelState]
        [SwaggerOperation("CreateArea")]
        [SwaggerResponse(statusCode: 200, type: typeof(AreaCreatedResponse), description: "Success")]
        [SwaggerResponse(statusCode: 400, type: typeof(Problem), description: "Error respsone for 400")]
        public abstract Task<IActionResult> CreateArea([FromBody]CreateAreaRequest? createAreaRequest);
    }
}
