using Microsoft.AspNetCore.Mvc;
using S3Provider.Controllers;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Intrinsics.Arm;
using System.Text.RegularExpressions;
using System.IO.Hashing;
using MinIOS3Provider.Client;
using Microsoft.Extensions.Options;
using S3Provider.Models;

namespace MinIOS3Provider.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BucketController : BucketApiController
    {

        private ILogger<BucketController> _logger;
        private IMinioS3Client _client;

        public BucketController(ILogger<BucketController> logger, IMinioS3Client client)
        {
            _logger = logger;
            _client = client;
        }

        public override IActionResult CreateAccessKey([FromRoute(Name = "id"), Required] string id)
        {
            throw new NotImplementedException();
        }

        public override IActionResult CreateBucket([FromQuery(Name = "preferredName"), Required] string preferredName)
        {
            
            var name = NameUtil.SanitizeBucketName(preferredName);
            
            _client.CreateBucket(name);
            _client.CreatePolicy(name);
            _client.CreateUser(name);

            return Ok(new BucketCreatedResponse() { Name = name, PreferredName = preferredName });

        }

        public override IActionResult CreateFolder([FromRoute(Name = "id"), Required] string id, [FromQuery(Name = "preferredName"), Required] string preferredName)
        {

            if (!NameUtil.CheckObjectName(preferredName))
            {
                return BadRequest(new Problem() { Status = 400, Title = "Invalid directory name" });
            }

            var name = preferredName;
            _client.CreateDirectory(id, name);

            return Ok(new FolderCreatedResponse() { Name = name, PreferredName = preferredName });
        }


    }
}