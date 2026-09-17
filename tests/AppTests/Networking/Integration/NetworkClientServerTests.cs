using System.Net;
using System.Net.Sockets;
using FluentAssertions;
using Model;
using Moq;
using Networking;
using Services;
using Xunit;

namespace AppTests.Networking.Integration;

public class NetworkClientServerTests : IDisposable
{
    private readonly int _port;
    private readonly ConcurrentServer _server;
    private readonly Thread _serverThread;
    private readonly Mock<IServices> _mockServices;
    private readonly Mock<IObserver> _mockObserver;

    public NetworkClientServerTests()
    {
        _port = GetFreeTcpPort();
        _mockServices = new Mock<IServices>();
        _mockObserver = new Mock<IObserver>();

        _server = new ConcurrentServer("127.0.0.1", _port, _mockServices.Object);
        _serverThread = new Thread(() =>
        {
            try
            {
                _server.Start();
            }
            catch (AppException)
            {
                // Expected when stopping server
            }
            catch (SocketException)
            {
                // Expected when socket is closed on stop
            }
        });
        _serverThread.Start();
        Thread.Sleep(200); // Give server a moment to start listening
    }

    public void Dispose()
    {
        _server.Stop();
    }

    private static int GetFreeTcpPort()
    {
        var listener = new TcpListener(IPAddress.Loopback, 0);
        listener.Start();
        int port = ((IPEndPoint)listener.LocalEndpoint).Port;
        listener.Stop();
        return port;
    }

    [Fact]
    public void Login_SuccessfulCredentials_SendsRequestAndSucceeds()
    {
        // Arrange
        var testUser = new User("admin", "password");
        _mockServices
            .Setup(s => s.Login(It.Is<User>(u => u.Username == "admin" && u.Password == "password"), It.IsAny<IObserver>()));

        var proxy = new ServerProxy("127.0.0.1", _port);

        // Act
        var act = () => proxy.Login(testUser, _mockObserver.Object);

        // Assert
        act.Should().NotThrow();
        _mockServices.Verify(s => s.Login(It.Is<User>(u => u.Username == "admin"), It.IsAny<IObserver>()), Times.Once);
    }

    [Fact]
    public void GetAllRaces_ReturnsRacesFromServiceOverNetwork()
    {
        // Arrange
        var expectedRaces = new List<Race>
        {
            new Race("100m", "Sprint") { Id = 1, Participants = new List<long> { 10 } }
        };
        _mockServices.Setup(s => s.GetAllRaces()).Returns(expectedRaces);

        var proxy = new ServerProxy("127.0.0.1", _port);

        // Act
        var races = proxy.GetAllRaces().ToList();

        // Assert
        races.Should().HaveCount(1);
        races[0].Id.Should().Be(1);
        races[0].Distance.Should().Be("100m");
        races[0].Style.Should().Be("Sprint");
    }

    [Fact]
    public void GetAllParticipants_ReturnsParticipantsFromServiceOverNetwork()
    {
        // Arrange
        var expectedParticipants = new List<Participant>
        {
            new Participant("Charlie", 24) { Id = 5, Races = new List<long> { 1 } }
        };
        _mockServices.Setup(s => s.GetAllParticipants()).Returns(expectedParticipants);

        var proxy = new ServerProxy("127.0.0.1", _port);

        // Act
        var participants = proxy.GetAllParticipants().ToList();

        // Assert
        participants.Should().HaveCount(1);
        participants[0].Id.Should().Be(5);
        participants[0].Name.Should().Be("Charlie");
    }

    [Fact]
    public void SaveParticipant_SendsRequestAndReturnsSavedParticipant()
    {
        // Arrange
        var inputParticipant = new Participant("David", 29) { Races = new List<long> { 1 } };
        var savedParticipant = new Participant("David", 29) { Id = 99, Races = new List<long> { 1 } };

        _mockServices
            .Setup(s => s.SaveParticipant(It.Is<Participant>(p => p.Name == "David")))
            .Returns(savedParticipant);

        var proxy = new ServerProxy("127.0.0.1", _port);

        // Act
        var result = proxy.SaveParticipant(inputParticipant);

        // Assert
        result.Should().NotBeNull();
        result.Id.Should().Be(99);
        result.Name.Should().Be("David");
    }
}
