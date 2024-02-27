/*
 * GitProvider
 *
 * This a service contract for the GitProvider in Balsam.
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
using GitProvider.Attributes;
using GitProvider.Models;

namespace GitProvider.Controllers
{ 
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    public abstract class RepositoryApiController : ControllerBase
    { 
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Adds resource files to a branch in a repository in the resource folder</remarks>
        /// <param name="repositoryId">The id of the repository.</param>
        /// <param name="branchId">The id of the branch.</param>
        /// <param name="uploadFile"></param>
        /// <response code="200">Success</response>
        /// <response code="400">Error respsone for 400</response>
        [HttpPut]
        [Route("/api/v1/repos/{repositoryId}/branches/{branchId}/resources")]
        [Consumes("multipart/form-data")]
        [ValidateModelState]
        [SwaggerOperation("AddResourceFiles")]
        [SwaggerResponse(statusCode: 400, type: typeof(Problem), description: "Error respsone for 400")]
        public abstract Task<IActionResult> AddResourceFiles([FromRoute (Name = "repositoryId")][Required]string repositoryId, [FromRoute (Name = "branchId")][Required]string branchId, IFormFile uploadFile);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Create a branch from main branch in a existing repository</remarks>
        /// <param name="repositoryId">The name of the repository where the branch should be created.</param>
        /// <param name="createBranchRequest">Definition of a new repository</param>
        /// <response code="200">Success</response>
        /// <response code="400">Error respsone for 400</response>
        [HttpPost]
        [Route("/api/v1/repos/{repositoryId}/branches")]
        [Consumes("application/json")]
        [ValidateModelState]
        [SwaggerOperation("CreateBranch")]
        [SwaggerResponse(statusCode: 200, type: typeof(BranchCreatedResponse), description: "Success")]
        [SwaggerResponse(statusCode: 400, type: typeof(Problem), description: "Error respsone for 400")]
        public abstract Task<IActionResult> CreateBranch([FromRoute (Name = "repositoryId")][Required]string repositoryId, [FromBody]CreateBranchRequest? createBranchRequest);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Creates a new repository</remarks>
        /// <param name="createRepositoryRequest">Definition of a new repository</param>
        /// <response code="200">Success</response>
        /// <response code="400">Error respsone for 400</response>
        [HttpPost]
        [Route("/api/v1/repos")]
        [Consumes("application/json")]
        [ValidateModelState]
        [SwaggerOperation("CreateRepository")]
        [SwaggerResponse(statusCode: 200, type: typeof(RepositoryCreatedResponse), description: "Success")]
        [SwaggerResponse(statusCode: 400, type: typeof(Problem), description: "Error respsone for 400")]
        public abstract Task<IActionResult> CreateRepository([FromBody]CreateRepositoryRequest? createRepositoryRequest);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Deletes a repository</remarks>
        /// <param name="repositoryId">The name of the repository where the branch should be created.</param>
        /// <response code="200">Success</response>
        /// <response code="400">Error respsone for 400</response>
        [HttpDelete]
        [Route("/api/v1/repos/{repositoryId}")]
        [ValidateModelState]
        [SwaggerOperation("DeleteRepository")]
        [SwaggerResponse(statusCode: 400, type: typeof(Problem), description: "Error respsone for 400")]
        public abstract Task<IActionResult> DeleteRepository([FromRoute (Name = "repositoryId")][Required]string repositoryId);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Deletes git branch for specified repository</remarks>
        /// <param name="repositoryId">The id of the repository.</param>
        /// <param name="branchId">The id of the branch.</param>
        /// <response code="200">Success</response>
        /// <response code="400">Error respsone for 400</response>
        [HttpDelete]
        [Route("/api/v1/repos/{repositoryId}/branches/{branchId}")]
        [ValidateModelState]
        [SwaggerOperation("DeleteRepositoryBranch")]
        [SwaggerResponse(statusCode: 400, type: typeof(Problem), description: "Error respsone for 400")]
        public abstract Task<IActionResult> DeleteRepositoryBranch([FromRoute (Name = "repositoryId")][Required]string repositoryId, [FromRoute (Name = "branchId")][Required]string branchId);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Get file contents</remarks>
        /// <param name="repositoryId">The id of the repository.</param>
        /// <param name="branchId">The identity of the branch.</param>
        /// <param name="fileId">The identity of the file.</param>
        /// <response code="200">Success</response>
        /// <response code="400">Error respsone for 400</response>
        [HttpGet]
        [Route("/api/v1/repos/{repositoryId}/branches/{branchId}/files/{fileId}")]
        [ValidateModelState]
        [SwaggerOperation("GetFile")]
        [SwaggerResponse(statusCode: 200, type: typeof(string), description: "Success")]
        [SwaggerResponse(statusCode: 400, type: typeof(Problem), description: "Error respsone for 400")]
        public abstract Task<IActionResult> GetFile([FromRoute (Name = "repositoryId")][Required]string repositoryId, [FromRoute (Name = "branchId")][Required]string branchId, [FromRoute (Name = "fileId")][Required]string fileId);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Gets file descriptions of all files in a git branch for specified repository</remarks>
        /// <param name="repositoryId">The id of the repository.</param>
        /// <param name="branchId">The id of the branch.</param>
        /// <response code="200">Success</response>
        /// <response code="400">Error respsone for 400</response>
        [HttpGet]
        [Route("/api/v1/repos/{repositoryId}/branches/{branchId}/files")]
        [ValidateModelState]
        [SwaggerOperation("GetFilesInBranch")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<RepoFile>), description: "Success")]
        [SwaggerResponse(statusCode: 400, type: typeof(Problem), description: "Error respsone for 400")]
        public abstract Task<IActionResult> GetFilesInBranch([FromRoute (Name = "repositoryId")][Required]string repositoryId, [FromRoute (Name = "branchId")][Required]string branchId);
    }
}
