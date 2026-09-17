using System.Collections.Concurrent;
using System.Net.Sockets;
using Google.Protobuf;
using log4net;
using Microsoft.VisualBasic.CompilerServices;
using Model;
using Protobuf;
using Services;

namespace Networking;

public class ServerProxy : IServices
{
    public ServerProxy(string host, int port)
    {
        _client = new TcpClient(host, port);
        _stream =  _client.GetStream();
        StartReader();
    }
    
    private readonly BlockingCollection<Response> _responses = new BlockingCollection<Response>();
    private static readonly ILog Log = LogManager.GetLogger(nameof(ServerProxy));
    private bool _finished = false;
    private IObserver? _clientController;
    private readonly TcpClient _client; 
    private NetworkStream? _stream; 

    private void StartReader()
    {
        _stream = _client.GetStream();
        var tw = new Thread(() =>
        {
            try
            {
                while (!_finished)
                {
                    try
                    {
                        var response = Response.Parser.ParseDelimitedFrom(_stream);
                        if (response == null)
                        {
                            Log.Info("Server connection closed.");
                            break;
                        }
                        Log.Info($"Response: {response}");
                        if (response.Type == ResponseType.UpdateParticipantResp)
                        {
                            _clientController?.Update();
                        }
                        else 
                        {
                            _responses.Add(response);
                        }
                    }
                    catch (Exception e)
                    {
                        if (!_finished)
                        {
                            Log.Error("Error reading response from server: ", e);
                        }
                        break;
                    }
                }
            }
            finally
            {
                _responses.CompleteAdding();
            }
        });
        tw.Start();
    }

    private void CloseConnection()
    {
        _client.Close();
        _stream?.Close();
    }

    private void SendRequest(Request request)
    {
        Log.Info($"Sending request: {request}");
        lock (_stream!)
        {
            request.WriteDelimitedTo(_stream); 
            _stream.Flush();
        }
    }

    private Response ReadResponse()
    {
        try
        {
            return _responses.Take();
        }
        catch (InvalidOperationException)
        {
            throw new AppException("Server connection lost.");
        }
    }

    public void Login(User user, IObserver proxy)
    {
        SendRequest(new Request() { Type = RequestType.Login, User =  DtoUtils.ToDto(user) });
        var response = ReadResponse();
        if (response.Type == ResponseType.Ok)
        {
            _clientController = proxy;
        } else
            throw new AppException(response.ErrorMessage);
    }

    public void Logout(User user, IObserver proxy)
    {
        SendRequest(new Request() { Type = RequestType.Logout, User =  DtoUtils.ToDto(user) });
        var response = ReadResponse();
        if (response.Type == ResponseType.Ok)
        {
            //CloseConnection();
            //finished = true;
        }
        else
            throw new AppException(response.ErrorMessage);
    }

    public IEnumerable<Race> GetAllRaces()
    {
        SendRequest(new Request(){Type = RequestType.GetRaces});
        var response = ReadResponse();
        if (response.Type == ResponseType.GetRacesResp)
            return response.Races.ToList().Select(DtoUtils.FromDto);
        throw new AppException("Get All Races error"); 
    }

    public IEnumerable<Race> GetAllRacesById(List<long> id)
    {
        var p = new Participant("mock", 0){Races = id};
        SendRequest(new Request() { Type = RequestType.GetRacesById, Participant = DtoUtils.ToDto(p)});
        var response = ReadResponse();
        return response.Races.ToList().Select(DtoUtils.FromDto);
    }

    public IEnumerable<Participant> GetAllParticipants()
    {
        SendRequest(new Request() {Type = RequestType.GetParticipants});
        var response = ReadResponse();
        return response.Participants.ToList().Select(DtoUtils.FromDto);
    }

    public IEnumerable<Participant> GetAllParticipantsById(List<long> id)
    {
        var r = new Race("mock", "mock"){Participants = id};
        SendRequest(new Request() {Type = RequestType.GetParticipantsById, Race = DtoUtils.ToDto(r)});
        var response = ReadResponse();
        return response.Participants.ToList().Select(DtoUtils.FromDto);
    }

    public Participant SaveParticipant(Participant participant)
    {
        SendRequest(new Request() { Type = RequestType.SaveParticipant, Participant = DtoUtils.ToDto(participant) });
        var response = ReadResponse();

        return DtoUtils.FromDto(response.Participant);
    }

    public void UpdateParticipant(Participant participant)
    {
        SendRequest(new Request() { Type = RequestType.UpdateParticipant, Participant = DtoUtils.ToDto(participant) });
        var response = ReadResponse();
    }
}