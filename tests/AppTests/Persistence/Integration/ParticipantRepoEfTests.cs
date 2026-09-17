using FluentAssertions;
using Model;
using Persistence;
using Xunit;

namespace AppTests.Persistence.Integration;

public class ParticipantRepoEfTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly ParticipantRepoEf _participantRepo;

    public ParticipantRepoEfTests()
    {
        var dbName = $"Data Source=file:mem_{Guid.NewGuid():N}?mode=memory&cache=shared";
        _context = new AppDbContext(dbName);
        _context.Database.EnsureCreated();

        // Seed data
        _context.Race.AddRange(
            new Race("Sprint 100m", "100m") { Id = 10 },
            new Race("Marathon", "42km") { Id = 20 }
        );

        _context.Participant.Add(new Participant("Alice", 25) { Id = 1 });
        _context.Registration.Add(new Registration { ParticipantId = 1, RaceId = 10 });
        _context.SaveChanges();

        _participantRepo = new ParticipantRepoEf(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public void FindAll_ReturnsParticipantsWithTheirRaces()
    {
        // Act
        var result = _participantRepo.FindAll();

        // Assert
        result.Should().NotBeEmpty();
        var alice = result.FirstOrDefault(p => p.Id == 1);
        alice.Should().NotBeNull();
        alice!.Name.Should().Be("Alice");
        alice.Races.Should().Contain(10L);
    }

    [Fact]
    public void FindOne_WithExistingId_ReturnsParticipantWithRaces()
    {
        // Act
        var result = _participantRepo.FindOne(1);

        // Assert
        result.Should().NotBeNull();
        result!.Name.Should().Be("Alice");
        result.Age.Should().Be(25);
        result.Races.Should().Contain(10L);
    }

    [Fact]
    public void Save_NewParticipant_PersistsParticipantAndRegistrations()
    {
        // Arrange
        var newParticipant = new Participant("Bob", 30)
        {
            Races = new List<long> { 10, 20 }
        };

        // Act
        var saved = _participantRepo.Save(newParticipant);

        // Assert
        saved.Should().NotBeNull();
        saved.Id.Should().BeGreaterThan(0);

        var retrieved = _participantRepo.FindOne(saved.Id);
        retrieved.Should().NotBeNull();
        retrieved!.Name.Should().Be("Bob");
        retrieved.Races.Should().BeEquivalentTo(new[] { 10L, 20L });
    }

    [Fact]
    public void SaveRegistration_And_DeleteRegistration_ModifiesRegistrations()
    {
        // Act - Add registration to race 20
        _participantRepo.SaveRegistration(1, 20);
        var racesAfterAdd = _participantRepo.FindAllRacesForParticipant(1);

        // Assert addition
        racesAfterAdd.Should().Contain(new[] { 10L, 20L });

        // Act - Delete registration from race 10
        _participantRepo.DeleteRegistration(1, 10);
        var racesAfterDelete = _participantRepo.FindAllRacesForParticipant(1);

        // Assert deletion
        racesAfterDelete.Should().NotContain(10L);
        racesAfterDelete.Should().Contain(20L);
    }
}
