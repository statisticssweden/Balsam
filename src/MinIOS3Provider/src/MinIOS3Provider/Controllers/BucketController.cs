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
            
            var name = SanitizeBucketName(preferredName);
            
            _client.CreateBucket(name);
            _client.CreatePolicy(name);
            _client.CreateUser(name);

            return Ok(new BucketCreatedResponse() { Name = name, PreferredName = preferredName });

        }

        public override IActionResult CreateFolder([FromRoute(Name = "id"), Required] string id, [FromQuery(Name = "preferredName"), Required] string preferredName)
        {

            if (!CheckDirectoryName(preferredName))
            {
                return BadRequest(new Problem() { Status = 400, Title = "Invalid directory name" });
            }

            var name = preferredName;
            _client.CreateDirectory(id, name);

            return Ok(new FolderCreatedResponse() { Name = name, PreferredName = preferredName });
        }

        //See https://min.io/docs/minio/container/operations/checklists/thresholds.html for naming rules

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public string SanitizeBucketName(string name)
        {
            name = name.ToLower(); //Only lower charachters allowed
            name = name.PadLeft(3, '0'); //At least 3 charachters long otherwise padd with zeros in the begining
            name = Regex.Replace(name,"[^a-z0-9]", "-"); // make sure that only a-z or digit or hypen repleces all other to hypen 
            name = name.StartsWith("-") || name.StartsWith("xn--") ? "x" + name:name; //make sure it starts with a character or a number and not xn--

            if (name.Length > 63) // Make sure that the name is not longer than 63 characters
            {
                System.IO.Hashing.Crc32 crc32 = new System.IO.Hashing.Crc32();

                crc32.Append(System.Text.Encoding.ASCII.GetBytes(name));
                var hash = crc32.GetCurrentHash();
                var crcHash = string.Join("", hash.Select(b => b.ToString("x2").ToLower()).Reverse());
                name = name.Substring(0, 63 - crcHash.Length) + crcHash;
            }
            return name;

        }

        private bool CheckDirectoryName(string name)
        {
            if (name.Length > 1024) return false;
            foreach (var token in name.Split("/", StringSplitOptions.RemoveEmptyEntries))
            {
                if (token.Length > 255) return false;
            }
            
            return true;

        }
    }
}