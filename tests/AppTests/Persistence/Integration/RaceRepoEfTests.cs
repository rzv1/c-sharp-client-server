using FluentAssertions;
using Model;
using Persistence;
using Xunit;

namespace AppTests.Persistence.Integration;

public class RaceRepoEfTests : IDisposable
{
    private readonly AppDbContext _context;
    private readonly RaceRepoEf _raceRepo;

    public RaceRepoEfTests()
    {
        var dbName = $"Data Source=file:mem_{Guid.NewGuid():N}?mode=memory&cache=shared";
        _context = new AppDbContext(dbName);
        _context.Database.EnsureCreated();

        // Seed data
        _context.Race.AddRange(
            new Race("Sprint 100m", "100m") { Id = 1 },
            new Race("Marathon 42k", "42km") { Id = 2 }
        );

        _context.Participant.AddRange(
            new Participant("Runner 1", 22) { Id = 101 },
            new Participant("Runner 2", 28) { Id = 102 }
        );

        _context.Registration.AddRange(
            new Registration { RaceId = 1, ParticipantId = 101 },
            new Registration { RaceId = 1, ParticipantId = 102 }
        );
        _context.SaveChanges();

        _raceRepo = new RaceRepoEf(_context);
    }

    public void Dispose()
    {
        _context.Database.EnsureDeleted();
        _context.Dispose();
    }

    [Fact]
    public void FindAll_ReturnsAllRacesWithParticipants()
    {
        // Act
        var races = _raceRepo.FindAll();

        // Assert
        races.Should().HaveCount(2);
        var sprint = races.FirstOrDefault(r => r.Id == 1);
        sprint.Should().NotBeNull();
        sprint!.Distance.Should().Be("Sprint 100m");
        sprint.Style.Should().Be("100m");
        sprint.Participants.Should().BeEquivalentTo(new[] { 101L, 102L });
    }

    [Fact]
    public void FindOne_WithExistingId_ReturnsRace()
    {
        // Act
        var race = _raceRepo.FindOne(1);

        // Assert
        race.Should().NotBeNull();
        race!.Distance.Should().Be("Sprint 100m");
        race.Style.Should().Be("100m");
        race.Participants.Should().BeEquivalentTo(new[] { 101L, 102L });
    }

    [Fact]
    public void FindOne_WithNonExistingId_ReturnsNull()
    {
        // Act
        var race = _raceRepo.FindOne(999);

        // Assert
        race.Should().BeNull();
    }

    [Fact]
    public void FindAllParticipantsByRace_ReturnsRegisteredParticipantIds()
    {
        // Act
        var participantIds = _raceRepo.FindAllParticipantsByRace(1);

        // Assert
        participantIds.Should().BeEquivalentTo(new[] { 101L, 102L });
    }
}
