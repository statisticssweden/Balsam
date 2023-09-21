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
            //TODO maybe we should have the userId in the accesskey name which will require us to take it as a input parameter.
            var keyPair = _client.CreateAccessKey(id);
            return Ok(new AccessKeyCreatedResponse(){ AccessKey = keyPair.AccessKey, SecretKey = keyPair.SecretKey}); 
        }

           public override IActionResult CreateBucket([FromBody] CreateBucketRequest? createBucketRequest)
        {
            var name = NameUtil.SanitizeBucketName(createBucketRequest.Name);

            _client.CreateBucket(name);
            _client.CreatePolicy(name);
            _client.CreateUser(name);

            return Ok(new BucketCreatedResponse() { Name = name, RequestedName = createBucketRequest.Name });
        }

        public override IActionResult CreateFolder([FromRoute(Name = "bucketId"), Required] string bucketId, [FromBody] CreateFolderRequest? createFolderRequest)
        {

            if (!NameUtil.CheckObjectName(createFolderRequest.Name))
            {
                return BadRequest(new Problem() { Status = 400, Title = "Invalid directory name" });
            }

            var name = createFolderRequest.Name;
            _client.CreateDirectory(bucketId, name);

            return Ok(new FolderCreatedResponse() { Name = name, RequestedName = createFolderRequest.Name });
        }
    }
}