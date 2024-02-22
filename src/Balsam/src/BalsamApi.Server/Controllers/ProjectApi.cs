/*
 * BalsamApi
 *
 * This is the API for createing Baslam artifcats.
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
using BalsamApi.Server.Attributes;
using BalsamApi.Server.Models;

namespace BalsamApi.Server.Controllers
{ 
    /// <summary>
    /// 
    /// </summary>
    [ApiController]
    public abstract class ProjectApiController : ControllerBase
    { 
        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Copy files from a knowledge library to a branch</remarks>
        /// <param name="projectId">the identity of the project.</param>
        /// <param name="branchId">The identity of the branch.</param>
        /// <param name="libraryId">id for the knowledge library</param>
        /// <param name="fileId">id for the file/drectory</param>
        /// <response code="200">Success</response>
        /// <response code="400">Error respsone for 400</response>
        [HttpPut]
        [Route("/api/v1/projects/{projectId}/branches/{branchId}/files")]
        [ValidateModelState]
        [SwaggerOperation("CopyFromKnowleadgeLibrary")]
        [SwaggerResponse(statusCode: 400, type: typeof(Problem), description: "Error respsone for 400")]
        public abstract Task<IActionResult> CopyFromKnowleadgeLibrary([FromRoute (Name = "projectId")][Required]string projectId, [FromRoute (Name = "branchId")][Required]string branchId, [FromQuery (Name = "libraryId")][Required()]string libraryId, [FromQuery (Name = "fileId")][Required()]string fileId);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Create a new branch for a project</remarks>
        /// <param name="projectId">the identity of the project.</param>
        /// <param name="createBranchRequest">Definition of a new branch</param>
        /// <response code="200">Success</response>
        /// <response code="400">Error respsone for 400</response>
        [HttpPost]
        [Route("/api/v1/projects/{projectId}/branches")]
        [Consumes("application/json")]
        [ValidateModelState]
        [SwaggerOperation("CreateBranch")]
        [SwaggerResponse(statusCode: 200, type: typeof(BranchCreatedResponse), description: "Success")]
        [SwaggerResponse(statusCode: 400, type: typeof(Problem), description: "Error respsone for 400")]
        public abstract Task<IActionResult> CreateBranch([FromRoute (Name = "projectId")][Required]string projectId, [FromBody]CreateBranchRequest? createBranchRequest);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Creates a new project</remarks>
        /// <param name="createProjectRequest">Definition of a new project</param>
        /// <response code="200">Success</response>
        /// <response code="400">Error respsone for 400</response>
        [HttpPost]
        [Route("/api/v1/projects")]
        [Consumes("application/json")]
        [ValidateModelState]
        [SwaggerOperation("CreateProject")]
        [SwaggerResponse(statusCode: 200, type: typeof(ProjectCreatedResponse), description: "Success")]
        [SwaggerResponse(statusCode: 400, type: typeof(Problem), description: "Error respsone for 400")]
        public abstract Task<IActionResult> CreateProject([FromBody]CreateProjectRequest? createProjectRequest);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Deletes branch of project</remarks>
        /// <param name="projectId">the identity of the project.</param>
        /// <param name="branchId">The identity of the branch.</param>
        /// <response code="200">Success</response>
        /// <response code="400">Error respsone for 400</response>
        [HttpDelete]
        [Route("/api/v1/projects/{projectId}/branches/{branchId}")]
        [ValidateModelState]
        [SwaggerOperation("DeleteBranch")]
        [SwaggerResponse(statusCode: 400, type: typeof(Problem), description: "Error respsone for 400")]
        public abstract Task<IActionResult> DeleteBranch([FromRoute (Name = "projectId")][Required]string projectId, [FromRoute (Name = "branchId")][Required]string branchId);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Deletes a project</remarks>
        /// <param name="projectId">the identity of the project.</param>
        /// <response code="200">Success</response>
        /// <response code="400">Error respsone for 400</response>
        [HttpDelete]
        [Route("/api/v1/projects/{projectId}")]
        [ValidateModelState]
        [SwaggerOperation("DeleteProject")]
        [SwaggerResponse(statusCode: 400, type: typeof(Problem), description: "Error respsone for 400")]
        public abstract Task<IActionResult> DeleteProject([FromRoute (Name = "projectId")][Required]string projectId);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Get file contents</remarks>
        /// <param name="projectId">the identity of the project.</param>
        /// <param name="branchId">The identity of the branch.</param>
        /// <param name="fileId">The identity of the file.</param>
        /// <response code="200">Success</response>
        /// <response code="400">Error respsone for 400</response>
        [HttpGet]
        [Route("/api/v1/projects/{projectId}/branches/{branchId}/files/{fileId}")]
        [ValidateModelState]
        [SwaggerOperation("GetFile")]
        [SwaggerResponse(statusCode: 200, type: typeof(string), description: "Success")]
        [SwaggerResponse(statusCode: 400, type: typeof(Problem), description: "Error respsone for 400")]
        public abstract Task<IActionResult> GetFile([FromRoute (Name = "projectId")][Required]string projectId, [FromRoute (Name = "branchId")][Required]string branchId, [FromRoute (Name = "fileId")][Required]string fileId);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Get files for a branch for a project</remarks>
        /// <param name="projectId">the identity of the project.</param>
        /// <param name="branchId">The identity of the branch.</param>
        /// <response code="200">Success</response>
        /// <response code="400">Error respsone for 400</response>
        [HttpGet]
        [Route("/api/v1/projects/{projectId}/branches/{branchId}/files")]
        [ValidateModelState]
        [SwaggerOperation("GetFiles")]
        [SwaggerResponse(statusCode: 200, type: typeof(List<RepoFile>), description: "Success")]
        [SwaggerResponse(statusCode: 400, type: typeof(Problem), description: "Error respsone for 400")]
        public abstract Task<IActionResult> GetFiles([FromRoute (Name = "projectId")][Required]string projectId, [FromRoute (Name = "branchId")][Required]string branchId);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Gets project information</remarks>
        /// <param name="projectId">the identity of the project.</param>
        /// <response code="200">Success</response>
        /// <response code="400">Error respsone for 400</response>
        [HttpGet]
        [Route("/api/v1/projects/{projectId}")]
        [ValidateModelState]
        [SwaggerOperation("GetProject")]
        [SwaggerResponse(statusCode: 200, type: typeof(ProjectResponse), description: "Success")]
        [SwaggerResponse(statusCode: 400, type: typeof(Problem), description: "Error respsone for 400")]
        public abstract Task<IActionResult> GetProject([FromRoute (Name = "projectId")][Required]string projectId);

        /// <summary>
        /// 
        /// </summary>
        /// <remarks>Gets a list of all available projects</remarks>
        /// <param name="all">If all projects should be returened or only projects that the user has assess rights too</param>
        /// <response code="200">Success</response>
        /// <response code="400">Error respsone for 400</response>
        [HttpGet]
        [Route("/api/v1/projects")]
        [ValidateModelState]
        [SwaggerOperation("ListProjects")]
        [SwaggerResponse(statusCode: 200, type: typeof(ProjectListResponse), description: "Success")]
        [SwaggerResponse(statusCode: 400, type: typeof(Problem), description: "Error respsone for 400")]
        public abstract Task<IActionResult> ListProjects([FromQuery (Name = "all")]bool? all);
    }
}
