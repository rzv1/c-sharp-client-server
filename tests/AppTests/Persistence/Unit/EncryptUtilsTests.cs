using FluentAssertions;
using Persistence;
using Services;
using Xunit;

namespace AppTests.Persistence.Unit;

public class EncryptUtilsTests
{
    [Fact]
    public void Encrypt_WithValidInput_ReturnsBase64String()
    {
        // Arrange
        var text = "password123";
        var secret = "mpp";

        // Act
        var result = EncryptUtils.Encrypt(text, secret);

        // Assert
        result.Should().NotBeNullOrEmpty();
        result.Should().NotBe(text);
    }

    [Fact]
    public void Encrypt_WithSameInput_ReturnsSameResult()
    {
        // Arrange
        var text = "admin123";
        var secret = "secretKey";

        // Act
        var result1 = EncryptUtils.Encrypt(text, secret);
        var result2 = EncryptUtils.Encrypt(text, secret);

        // Assert
        result1.Should().Be(result2);
    }

    [Fact]
    public void Encrypt_WithDifferentSecrets_ReturnsDifferentResults()
    {
        // Arrange
        var text = "admin123";

        // Act
        var result1 = EncryptUtils.Encrypt(text, "secret1");
        var result2 = EncryptUtils.Encrypt(text, "secret2");

        // Assert
        result1.Should().NotBe(result2);
    }

    [Fact]
    public void Encrypt_WithDifferentText_ReturnsDifferentResults()
    {
        // Arrange
        var secret = "mpp";

        // Act
        var result1 = EncryptUtils.Encrypt("user1", secret);
        var result2 = EncryptUtils.Encrypt("user2", secret);

        // Assert
        result1.Should().NotBe(result2);
    }
}
