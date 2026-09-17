using FluentAssertions;
using Model;
using Networking;
using Protobuf;
using Xunit;

namespace AppTests.Networking.Unit;

public class DtoUtilsTests
{
    [Fact]
    public void User_ToDto_And_FromDto_PreservesFields()
    {
        // Arrange
        var user = new User("alice", "secretPass");

        // Act
        var dto = DtoUtils.ToDto(user);
        var restored = DtoUtils.FromDto(dto);

        // Assert
        dto.Username.Should().Be("alice");
        dto.Password.Should().Be("secretPass");

        restored.Username.Should().Be(user.Username);
        restored.Password.Should().Be(user.Password);
    }

    [Fact]
    public void Participant_ToDto_And_FromDto_PreservesFields()
    {
        // Arrange
        var participant = new Participant("Bob", 28)
        {
            Id = 15,
            Races = new List<long> { 1, 2, 3 }
        };

        // Act
        var dto = DtoUtils.ToDto(participant);
        var restored = DtoUtils.FromDto(dto);

        // Assert
        dto.Id.Should().Be(15);
        dto.Name.Should().Be("Bob");
        dto.Age.Should().Be(28);
        dto.Races.Should().BeEquivalentTo(new[] { 1L, 2L, 3L });

        restored.Id.Should().Be(15);
        restored.Name.Should().Be("Bob");
        restored.Age.Should().Be(28);
        restored.Races.Should().BeEquivalentTo(new[] { 1L, 2L, 3L });
    }

    [Fact]
    public void Race_ToDto_And_FromDto_PreservesFields()
    {
        // Arrange
        var race = new Race("50m Freestyle", "Freestyle")
        {
            Id = 42,
            Participants = new List<long> { 101, 102 }
        };

        // Act
        var dto = DtoUtils.ToDto(race);
        var restored = DtoUtils.FromDto(dto);

        // Assert
        dto.Id.Should().Be(42);
        dto.Distance.Should().Be("50m Freestyle");
        dto.Style.Should().Be("Freestyle");
        dto.Participants.Should().BeEquivalentTo(new[] { 101L, 102L });

        restored.Id.Should().Be(42);
        restored.Distance.Should().Be("50m Freestyle");
        restored.Style.Should().Be("Freestyle");
        restored.Participants.Should().BeEquivalentTo(new[] { 101L, 102L });
    }

    [Fact]
    public void ParticipantCollection_ToDto_And_FromDto_ConvertsAllItems()
    {
        // Arrange
        var participants = new List<Participant>
        {
            new Participant("P1", 20) { Id = 1, Races = new List<long> { 10 } },
            new Participant("P2", 22) { Id = 2, Races = new List<long> { 20 } }
        };

        // Act
        var dtos = DtoUtils.ToDto(participants).ToList();
        var restored = DtoUtils.FromDto(dtos).ToList();

        // Assert
        dtos.Should().HaveCount(2);
        restored.Should().HaveCount(2);
        restored[0].Name.Should().Be("P1");
        restored[1].Name.Should().Be("P2");
    }

    [Fact]
    public void RaceCollection_ToDto_And_FromDto_ConvertsAllItems()
    {
        // Arrange
        var races = new List<Race>
        {
            new Race("Race 1", "Style 1") { Id = 1, Participants = new List<long> { 5 } },
            new Race("Race 2", "Style 2") { Id = 2, Participants = new List<long> { 6 } }
        };

        // Act
        var dtos = DtoUtils.ToDto(races).ToList();
        var restored = DtoUtils.FromDto(dtos).ToList();

        // Assert
        dtos.Should().HaveCount(2);
        restored.Should().HaveCount(2);
        restored[0].Distance.Should().Be("Race 1");
        restored[1].Distance.Should().Be("Race 2");
    }
}
