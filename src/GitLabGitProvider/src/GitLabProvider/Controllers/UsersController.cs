﻿using GitLabProvider.Client;
using GitProvider.Controllers;
using GitProvider.Models;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using System.ComponentModel.DataAnnotations;

namespace GitLabProvider.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class UsersController : UserApiController
    {
        IGitLabClient _gitLabClient;

        public UsersController(IGitLabClient gitLabClient)
        {
            _gitLabClient = gitLabClient;
        }
        public async override Task<IActionResult> CreatePAT([FromRoute(Name = "id"), Required] string id)
        {
            var token = await _gitLabClient.CreatePAT(id);
            if (token != null)
            {
                return Ok(new UserPATCreatedResponse() { Name = "token", Token = token });
            }
            return BadRequest(new Problem() { Type = "404", Title = "Could not create PAT" });
        }

    }
}
