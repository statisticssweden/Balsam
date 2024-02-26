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
        /// <param name="bucketId">the name of the bucket</param>
        /// <response code="200">Success</response>
        /// <response code="400">Error respsone for 400</response>
        [HttpPost]
        [Route("/api/v1/buckets/{bucketId}/acceskey")]
        [ValidateModelState]
        [SwaggerOperation("CreateAccessKey")]
        [SwaggerResponse(statusCode: 200, type: typeof(AccessKeyCreatedResponse), description: "Success")]
        [SwaggerResponse(statusCode: 400, type: typeof(Problem), description: "Error respsone for 400")]
        public abstract IActionResult CreateAccessKey([FromRoute (Name = "bucketId")][Required]string bucketId);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Creates a new Bucket</remarks>
        /// <param name="createBucketRequest">Definition of a new role</param>
        /// <response code="200">Success</response>
        /// <response code="400">Error respsone for 400</response>
        [HttpPost]
        [Route("/api/v1/buckets")]
        [Consumes("application/json")]
        [ValidateModelState]
        [SwaggerOperation("CreateBucket")]
        [SwaggerResponse(statusCode: 200, type: typeof(BucketCreatedResponse), description: "Success")]
        [SwaggerResponse(statusCode: 400, type: typeof(Problem), description: "Error respsone for 400")]
        public abstract IActionResult CreateBucket([FromBody]CreateBucketRequest? createBucketRequest);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Creates a new virtual folder in the bucket.</remarks>
        /// <param name="bucketId">the name of the bucket</param>
        /// <param name="createFolderRequest">Definition of a new role</param>
        /// <response code="200">Success</response>
        /// <response code="400">Error respsone for 400</response>
        [HttpPost]
        [Route("/api/v1/buckets/{bucketId}/folder")]
        [Consumes("application/json")]
        [ValidateModelState]
        [SwaggerOperation("CreateFolder")]
        [SwaggerResponse(statusCode: 200, type: typeof(FolderCreatedResponse), description: "Success")]
        [SwaggerResponse(statusCode: 400, type: typeof(Problem), description: "Error respsone for 400")]
        public abstract IActionResult CreateFolder([FromRoute (Name = "bucketId")][Required]string bucketId, [FromBody]CreateFolderRequest? createFolderRequest);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Deletes a new Bucket</remarks>
        /// <param name="bucketId">the name of the bucket</param>
        /// <response code="200">Success</response>
        /// <response code="400">Error respsone for 400</response>
        [HttpDelete]
        [Route("/api/v1/buckets/{bucketId}")]
        [ValidateModelState]
        [SwaggerOperation("DeleteBucket")]
        [SwaggerResponse(statusCode: 400, type: typeof(Problem), description: "Error respsone for 400")]
        public abstract IActionResult DeleteBucket([FromRoute (Name = "bucketId")][Required]string bucketId);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Deletes a new virtual folder in the bucket.</remarks>
        /// <param name="bucketId">the name of the bucket</param>
        /// <param name="folderName">the name of the folder</param>
        /// <response code="200">Success</response>
        /// <response code="400">Error respsone for 400</response>
        [HttpDelete]
        [Route("/api/v1/buckets/{bucketId}/folder/{folderName}")]
        [ValidateModelState]
        [SwaggerOperation("DeleteFolder")]
        [SwaggerResponse(statusCode: 400, type: typeof(Problem), description: "Error respsone for 400")]
        public abstract IActionResult DeleteFolder([FromRoute (Name = "bucketId")][Required]string bucketId, [FromRoute (Name = "folderName")][Required]string folderName);
    }
}
