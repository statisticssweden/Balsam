using Microsoft.AspNetCore.Mvc;
using S3Provider.Controllers;
using System.ComponentModel.DataAnnotations;

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
            throw new NotImplementedException();
        }

        public override IActionResult CreateFolder([FromRoute(Name = "id"), Required] string id, [FromQuery(Name = "preferredName"), Required] string preferredName)
        {
            throw new NotImplementedException();
        }
    }
}