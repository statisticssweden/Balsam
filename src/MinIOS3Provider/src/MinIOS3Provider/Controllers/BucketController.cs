using Microsoft.AspNetCore.Mvc;
using S3Provider.Controllers;
using System.ComponentModel.DataAnnotations;
using System.Runtime.Intrinsics.Arm;
using System.Text.RegularExpressions;
using System.IO.Hashing;

namespace MinIOS3Provider.Controllers
{
    [ApiController]
    [Route("[controller]")]
    public class BucketController : BucketApiController
    {
        public override IActionResult CreateAccessKey([FromRoute(Name = "id"), Required] string id)
        {
            throw new NotImplementedException();
        }

        public override IActionResult CreateBucket([FromQuery(Name = "preferredName"), Required] string preferredName)
        {
            //TODO
            //1. Sanitize preferredName
            //2. client.CreateBucket()
            //3. client.CreatePolicy()
            //4. client.CreateUser()

            throw new NotImplementedException();
        }

        public override IActionResult CreateFolder([FromRoute(Name = "id"), Required] string id, [FromQuery(Name = "preferredName"), Required] string preferredName)
        {
            throw new NotImplementedException();
        }

        private string SanitizeName(string name)
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
    }
}