using Microsoft.Extensions.Logging;
using MinIOS3Provider.Client;
using MinIOS3Provider.Controllers;
using Moq;

namespace MinIOS3Provider.Test
{
    [TestFixture]
    public class Naming_Tests
    {

        [SetUp]
        public void Setup()
        {

        }

        [Test]
        [TestCase("this-is-a-valid-name")]
        [TestCase("this-is-a-valid-name-and-this")]
        public void Bucket_names_unchanged(string name)
        {
            var newName = NameUtil.SanitizeBucketName(name);
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
            var newName = NameUtil.SanitizeBucketName(name);
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
            name = NameUtil.SanitizeBucketName(name);
            var newName = NameUtil.SanitizeBucketName(name);
            Assert.AreEqual(name, newName);
        }
    }
}