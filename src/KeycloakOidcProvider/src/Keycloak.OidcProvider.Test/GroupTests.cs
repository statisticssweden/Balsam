using System.ComponentModel.DataAnnotations;
using Keycloak.OidcProvider.Client;

namespace Keycloak.OicdProvider.Test
{
    [TestFixture]
    public class GroupTests
    {
        private string? _namePrefix;

        [SetUp]
        public void Setup()
        {
            _namePrefix = "balsam-grp_";
        }

        [Test]
        public void ShortName_IsFilled_With_0s()
        {
            //Arrange 
            var group = new Group("A");

            var expected = $"{_namePrefix}00a";

            Assert.That(expected, Is.EqualTo(group.Name));
        }

        [Test]
        public void LongName_50Chars_ReturnsValid()
        {
            //Arrange 
            var group = new Group("00000000010000000002000000000300000000040000000005");

            //Act
            var expected = $"{_namePrefix}00000000010000000002000000000300000000040000000005";

            //Assert
            Assert.That(expected, Is.EqualTo(group.Name));

        }

        [Test]
        public void LongName_52Chars_ReturnsInvalid()
        {
            //Arrange 
            var group = new Group("123456789012345678901234567890123456789012345678902");

            //Act
            var expected = $"{_namePrefix}123456789012345678901234567890123456789012345678902";

            //Assert
            Assert.That(expected, Is.EqualTo(group.Name));
        }

        [Test]
        public void InvalidCharacters_IsReplacedWith_WithDash()
        {
            var group = new Group("Påäö");

            var expected = $"{_namePrefix}p---";
            Assert.That(expected, Is.EqualTo(group.Name));
        }

        [Test]
        public void CapitalLetter_IsReplacedWith_Lower()
        {
            var group = new Group("TESTAR");

            var expected = $"{_namePrefix}testar";
            Assert.That(expected, Is.EqualTo(group.Name));
        }
    }
}
