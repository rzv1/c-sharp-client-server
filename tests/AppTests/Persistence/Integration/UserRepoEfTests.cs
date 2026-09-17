using FluentAssertions;
using Model;
using Persistence;
using Xunit;

namespace AppTests.Persistence.Integration;

public class UserRepoEfTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly UserRepoEf _userRepo;
    private readonly string _dbName;

    public UserRepoEfTests()
    {
        _dbName = $"Data Source=file:mem_{Guid.NewGuid():N}?mode=memory&cache=shared";
        _context = new AppDbContext(_dbName);
        _context.Database.EnsureCreated();

        // Seed initial test data
        _context.User.AddRange(
            new User("admin", EncryptUtils.Encrypt("password", "mpp")) { Id = 1 },
            new User("john", EncryptUtils.Encrypt("pass123", "mpp")) { Id = 2 }
        );
        _context.SaveChanges();

        _userRepo = new UserRepoEf(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public void FindAll_ReturnsAllSeededUsers()
    {
        // Act
        var users = _userRepo.FindAll();

        // Assert
        users.Should().HaveCount(2);
        users.Select(u => u.Username).Should().Contain(new[] { "admin", "john" });
    }

    [Fact]
    public void FindOne_WithExistingId_ReturnsUser()
    {
        // Act
        var user = _userRepo.FindOne(1);

        // Assert
        user.Should().NotBeNull();
        user!.Username.Should().Be("admin");
    }

    [Fact]
    public void FindOne_WithNonExistingId_ReturnsNull()
    {
        // Act
        var user = _userRepo.FindOne(999);

        // Assert
        user.Should().BeNull();
    }

    [Fact]
    public void VerifyLogin_WithCorrectCredentials_ReturnsUser()
    {
        // Act
        var user = _userRepo.VerifyLogin("admin", "password");

        // Assert
        user.Should().NotBeNull();
        user!.Username.Should().Be("admin");
    }

    [Fact]
    public void VerifyLogin_WithWrongPassword_ReturnsNull()
    {
        // Act
        var user = _userRepo.VerifyLogin("admin", "wrong_password");

        // Assert
        user.Should().BeNull();
    }

    [Fact]
    public void VerifyLogin_WithNonExistingUsername_ReturnsNull()
    {
        // Act
        var user = _userRepo.VerifyLogin("nonexistent", "password");

        // Assert
        user.Should().BeNull();
    }
}
