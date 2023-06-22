using Microsoft.Extensions.Logging;
using MinIOS3Provider.Client;
using MinIOS3Provider.Controllers;
using Moq;

namespace MinIOS3Provider.Test
{
    [TestFixture]
    public class Naming_Tests
    {
        private BucketController _controller;
        private Mock<IMinioS3Client> _client;
        private Mock<ILogger<BucketController>> _logger;

        [SetUp]
        public void Setup()
        {
            _client = new Mock<IMinioS3Client>();
            _logger = new Mock<ILogger<BucketController>>();

            _controller = new BucketController(_logger.Object, _client.Object);
        }

        [Test]
        [TestCase("this-is-a-valid-name")]
        [TestCase("this-is-a-valid-name-and-this")]
        public void Bucket_names_unchanged(string name)
        {
            //var name = "this-is-a-valid-name";
            var newName = _controller.SanitizeBucketName(name);
            Assert.AreEqual(name, newName);
        }

        [Test]
        [TestCase("this-is-NOT-a-valid-name")]
        [TestCase("this_is_ALSO_NOT_a-valid-name-and-this")]
        [TestCase("-666")]
        [TestCase("6")]
        [TestCase("xn--test")]
        public void Bucket_names_changed(string name)
        {
            //var name = "this-is-a-valid-name";
            var newName = _controller.SanitizeBucketName(name);
            Assert.AreNotEqual(name, newName);
        }

        [Test]
        [TestCase("this-is-NOT-a-valid-name")]
        [TestCase("this_is_ALSO_NOT_a-valid-name-and-this")]
        [TestCase("1 this_is_ALSO_NOT_a-valid-name-and-this")]
        [TestCase("-666")]
        [TestCase("6")]
        [TestCase("xn--test")]
        public void Bucket_names_ShouldPassSecondTime(string name)
        {
            //var name = "this-is-a-valid-name";
            name = _controller.SanitizeBucketName(name);
            var newName = _controller.SanitizeBucketName(name);
            Assert.AreEqual(name, newName);
        }
    }
}