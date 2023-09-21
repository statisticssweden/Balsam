using System.ComponentModel.DataAnnotations;
using Keycloak.OicdProvider.Client;

namespace Keycloak.OicdProvider.Test
{
    [TestFixture]
    public class RoleTests
    {
        private string? _namePrefix;

        [SetUp]
        public void Setup()
        {
            _namePrefix = "balsam-role_";
        }

        [Test]
        public void ShortName_IsFilled_With_0s()
        {
            //Arrange 
            var role = new Role("A");

            var expected = $"{_namePrefix}00a";

            Assert.That(expected, Is.EqualTo(role.Name));
        }

        [Test]
        public void LongName_50Chars_ReturnsValid()
        {
            //Arrange 
            var role = new Role("00000000010000000002000000000300000000040000000005");

            //Act
            var expected = $"{_namePrefix}00000000010000000002000000000300000000040000000005";

            //Assert
            Assert.That(expected, Is.EqualTo(role.Name));

        }

        [Test]
        public void LongName_52Chars_ReturnsInvalid()
        {
            //Arrange 
            var role = new Role("123456789012345678901234567890123456789012345678902");

            //Act
            var validationContext = new ValidationContext(role, null, null);
            var validationResult = new List<ValidationResult>();
            var isValid = Validator.TryValidateObject(role, validationContext, validationResult, validateAllProperties: true);

            //Assert
            Assert.IsFalse(isValid, "Object should be valid");
            Assert.IsNotEmpty(validationResult);
        }

        [Test]
        public void InvalidCharacters_IsReplacedWith_WithDash()
        {
            var role = new Role("Påäö");

            var expected = $"{_namePrefix}p---";
            Assert.That(expected, Is.EqualTo(role.Name));
        }

        [Test]
        public void CapitalLetter_IsReplacedWith_Lower()
        {
            var role = new Role("TESTAR");

            var expected = $"{_namePrefix}testar";
            Assert.That(expected, Is.EqualTo(role.Name));
        }
    }
}
