using System.Net.Sockets;
using Google.Protobuf;
using log4net;
using Protobuf;
using Services;

namespace Networking;

public class ClientWorker(IServices clientProxy, Socket client) : IObserver
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(ClientWorker));
    private readonly NetworkStream _stream = new NetworkStream(client);
    private volatile bool _connected = true;

    public void Run()
    {
        while (_connected)
        {
            try
            {
                var request = Request.Parser.ParseDelimitedFrom(_stream);
                if (request == null)
                {
                    Log.Info("Client disconnected (EOF received).");
                    break;
                }
                Log.InfoFormat("Received request: {0}", request);
                var response = HandleRequest(request);
                Log.InfoFormat("Sending response: {0}", response);
                response.WriteDelimitedTo(_stream);
                _stream.Flush();
            }
            catch (Exception e)
            {
                Log.Error("Error handling client request: ", e);
                break;
            }
        }
        try
        {
            lock (_stream)
                _stream.Close();
            client.Close();
        }
        catch (Exception e)
        {
            Log.Error("Error closing client connection: ", e);
        }
    }

    private Response HandleRequest(Request request)
    {
        try
        {
            if (request.Type == RequestType.GetParticipants)
            {
                var response = new Response { Type = ResponseType.GetParticipantsResp };
                response.Participants.AddRange(DtoUtils.ToDto(clientProxy.GetAllParticipants()));
                return response;
            }

            if (request.Type == RequestType.GetParticipantsById)
            {
                var response = new Response { Type = ResponseType.GetParticipantsByIdResp };
                response.Participants.AddRange(DtoUtils.ToDto(clientProxy.GetAllParticipantsById(request.Race.Participants.ToList())));
                return response;
            }

            if (request.Type == RequestType.GetRaces)
            {
                var response = new Response { Type = ResponseType.GetRacesResp };
                response.Races.AddRange(DtoUtils.ToDto(clientProxy.GetAllRaces()));
                return response;
            }

            if (request.Type == RequestType.GetRacesById)
            {
                var response = new Response { Type = ResponseType.GetRacesByIdResp };
                response.Races.AddRange(DtoUtils.ToDto(clientProxy.GetAllRacesById(request.Participant.Races.ToList())));
                return response;
            }

            if (request.Type == RequestType.Login)
            {
                clientProxy.Login(DtoUtils.FromDto(request.User), this);
            }

            if (request.Type == RequestType.Logout)
            {
                clientProxy.Logout(DtoUtils.FromDto(request.User), this);
                //_connected = false;
            }

            if (request.Type == RequestType.SaveParticipant)
            {
                var p = clientProxy.SaveParticipant(DtoUtils.FromDto(request.Participant));
                return new Response { Type = ResponseType.SaveParticipantResp, Participant = DtoUtils.ToDto(p) };
            }

            if (request.Type == RequestType.UpdateParticipant)
            {
                clientProxy.UpdateParticipant(DtoUtils.FromDto(request.Participant));
            }
        }
        catch (AppException e)
        {
            return new Response { Type = ResponseType.Error, ErrorMessage =  e.Message }; 
        }

        return new Response { Type = ResponseType.Ok };
    }
    
    public void Update()
    {
        var response = new Response { Type = ResponseType.UpdateParticipantResp };
        Log.InfoFormat("Notifying clients: {0}", response);
        lock (_stream)
        {
            response.WriteDelimitedTo(_stream);
            _stream.Flush();
        }
    }
}