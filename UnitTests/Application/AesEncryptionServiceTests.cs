using Application.Services;
using Microsoft.Extensions.Options;
using Shared.Configuration;
using System.Security.Cryptography;

namespace UnitTests.Application
{
    public class AesEncryptionServiceTests
    {
        private readonly AesEncryptionService _service;
        private readonly string _validKey;
        private readonly string _validIv;

        public AesEncryptionServiceTests()
        {
            using var aes = Aes.Create();
            aes.GenerateKey();
            aes.GenerateIV();
            _validKey = Convert.ToBase64String(aes.Key);
            _validIv = Convert.ToBase64String(aes.IV);

            var options = Options.Create(new EncryptionConfiguration
            {
                Key = _validKey,
                Iv = _validIv
            });
            _service = new AesEncryptionService(options);
        }

        #region Encrypt Tests

        [Fact]
        public void Encrypt_ShouldReturnNonEmptyString()
        {
            // Act
            var result = _service.Encrypt("Hello World");

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(result));
        }

        [Fact]
        public void Encrypt_ShouldReturnBase64String()
        {
            // Act
            var result = _service.Encrypt("Hello World");

            // Assert
            var bytes = Convert.FromBase64String(result);
            Assert.NotEmpty(bytes);
        }

        [Fact]
        public void Encrypt_ShouldReturnDifferentValueFromPlainText()
        {
            // Arrange
            var plainText = "Hello World";

            // Act
            var result = _service.Encrypt(plainText);

            // Assert
            Assert.NotEqual(plainText, result);
        }

        [Fact]
        public void Encrypt_ShouldReturnSameResult_ForSameInput()
        {
            // Arrange
            var plainText = "Hello World";

            // Act
            var result1 = _service.Encrypt(plainText);
            var result2 = _service.Encrypt(plainText);

            // Assert
            Assert.Equal(result1, result2);
        }

        [Fact]
        public void Encrypt_ShouldReturnDifferentResults_ForDifferentInputs()
        {
            // Act
            var result1 = _service.Encrypt("Hello");
            var result2 = _service.Encrypt("World");

            // Assert
            Assert.NotEqual(result1, result2);
        }

        [Fact]
        public void Encrypt_ShouldHandleEmptyString()
        {
            // Act
            var result = _service.Encrypt("");

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(result));
        }

        [Fact]
        public void Encrypt_ShouldHandleLongString()
        {
            // Arrange
            var longText = new string('A', 10000);

            // Act
            var result = _service.Encrypt(longText);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(result));
        }

        [Fact]
        public void Encrypt_ShouldHandleSpecialCharacters()
        {
            // Arrange
            var specialText = "àéîöü@#$%^&*(){}[]|\\:;'<>,.?/~`";

            // Act
            var result = _service.Encrypt(specialText);

            // Assert
            Assert.False(string.IsNullOrWhiteSpace(result));
        }

        #endregion

        #region Decrypt Tests

        [Fact]
        public void Decrypt_ShouldReturnOriginalPlainText()
        {
            // Arrange
            var plainText = "Hello World";
            var encrypted = _service.Encrypt(plainText);

            // Act
            var result = _service.Decrypt(encrypted);

            // Assert
            Assert.Equal(plainText, result);
        }

        [Fact]
        public void Decrypt_ShouldReturnOriginalEmptyString()
        {
            // Arrange
            var encrypted = _service.Encrypt("");

            // Act
            var result = _service.Decrypt(encrypted);

            // Assert
            Assert.Equal("", result);
        }

        [Fact]
        public void Decrypt_ShouldReturnOriginalLongString()
        {
            // Arrange
            var longText = new string('B', 10000);
            var encrypted = _service.Encrypt(longText);

            // Act
            var result = _service.Decrypt(encrypted);

            // Assert
            Assert.Equal(longText, result);
        }

        [Fact]
        public void Decrypt_ShouldReturnOriginalSpecialCharacters()
        {
            // Arrange
            var specialText = "àéîöü@#$%^&*(){}[]|\\:;'<>,.?/~`";
            var encrypted = _service.Encrypt(specialText);

            // Act
            var result = _service.Decrypt(encrypted);

            // Assert
            Assert.Equal(specialText, result);
        }

        [Fact]
        public void Decrypt_ShouldThrow_WhenCipherTextIsInvalid()
        {
            // Assert
            Assert.ThrowsAny<Exception>(() => _service.Decrypt("not-valid-base64!!!"));
        }

        #endregion

        #region Encrypt + Decrypt Roundtrip Tests

        [Theory]
        [InlineData("simple text")]
        [InlineData("12345")]
        [InlineData("email@test.com")]
        [InlineData("password!@#$%")]
        [InlineData("日本語テスト")]
        public void EncryptDecrypt_Roundtrip_ShouldReturnOriginalText(string plainText)
        {
            // Act
            var encrypted = _service.Encrypt(plainText);
            var decrypted = _service.Decrypt(encrypted);

            // Assert
            Assert.Equal(plainText, decrypted);
        }

        #endregion

        #region Different Keys Tests

        [Fact]
        public void Decrypt_ShouldFail_WithDifferentKey()
        {
            // Arrange
            var encrypted = _service.Encrypt("Hello World");

            using var aes = Aes.Create();
            aes.GenerateKey();
            var differentKeyOptions = Options.Create(new EncryptionConfiguration
            {
                Key = Convert.ToBase64String(aes.Key),
                Iv = _validIv
            });
            var differentService = new AesEncryptionService(differentKeyOptions);

            // Act & Assert
            Assert.ThrowsAny<Exception>(() => differentService.Decrypt(encrypted));
        }

        [Fact]
        public void Decrypt_ShouldFail_WithDifferentIv()
        {
            // Arrange
            var encrypted = _service.Encrypt("Hello World");

            using var aes = Aes.Create();
            aes.GenerateIV();
            var differentIvOptions = Options.Create(new EncryptionConfiguration
            {
                Key = _validKey,
                Iv = Convert.ToBase64String(aes.IV)
            });
            var differentService = new AesEncryptionService(differentIvOptions);

            // Act & Assert - with different IV, decryption should fail or return wrong result
            try
            {
                var decrypted = differentService.Decrypt(encrypted);
                Assert.NotEqual("Hello World", decrypted);
            }
            catch (CryptographicException)
            {
                // Expected: padding error when IV is wrong
            }
        }

        #endregion
    }
}
