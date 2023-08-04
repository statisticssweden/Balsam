/*
 * S3Provider
 *
 * This a service contract for the OicdProvider in Balsam.
 *
 * The version of the OpenAPI document: 2.0
 * 
 * Generated by: https://openapi-generator.tech
 */

using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Http;
using Swashbuckle.AspNetCore.Annotations;
using Swashbuckle.AspNetCore.SwaggerGen;
using S3Provider.Attributes;
using S3Provider.Models;

namespace S3Provider.Controllers
{ 
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    public abstract class BucketApiController : ControllerBase
    { 
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Creates a new acces key for the bucket.</remarks>
        /// <param name="id">the name of the bucket</param>
        /// <response code="200">Success</response>
        /// <response code="400">Error respsone for 400</response>
        [HttpPost]
        [Route("/api/v1/buckets/{id}/acceskey")]
        [ValidateModelState]
        [SwaggerOperation("CreateAccessKey")]
        [SwaggerResponse(statusCode: 200, type: typeof(AccessKeyCreatedResponse), description: "Success")]
        [SwaggerResponse(statusCode: 400, type: typeof(Problem), description: "Error respsone for 400")]
        public abstract IActionResult CreateAccessKey([FromRoute (Name = "id")][Required]string id);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Creates a new Bucket</remarks>
        /// <param name="preferredName">The preferredName of the bucket</param>
        /// <response code="200">Success</response>
        /// <response code="400">Error respsone for 400</response>
        [HttpPost]
        [Route("/api/v1/buckets")]
        [ValidateModelState]
        [SwaggerOperation("CreateBucket")]
        [SwaggerResponse(statusCode: 200, type: typeof(BucketCreatedResponse), description: "Success")]
        [SwaggerResponse(statusCode: 400, type: typeof(Problem), description: "Error respsone for 400")]
        public abstract IActionResult CreateBucket([FromQuery (Name = "preferredName")][Required()]string preferredName);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Creates a new virtual folder in the bucket.</remarks>
        /// <param name="id">the name of the bucket</param>
        /// <param name="preferredName">The preferredName of the bucket</param>
        /// <response code="200">Success</response>
        /// <response code="400">Error respsone for 400</response>
        [HttpPost]
        [Route("/api/v1/buckets/{id}/folder")]
        [ValidateModelState]
        [SwaggerOperation("CreateFolder")]
        [SwaggerResponse(statusCode: 200, type: typeof(FolderCreatedResponse), description: "Success")]
        [SwaggerResponse(statusCode: 400, type: typeof(Problem), description: "Error respsone for 400")]
        public abstract IActionResult CreateFolder([FromRoute (Name = "id")][Required]string id, [FromQuery (Name = "preferredName")][Required()]string preferredName);
    }
}