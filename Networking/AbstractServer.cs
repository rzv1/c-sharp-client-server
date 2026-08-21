using System.Net;
using System.Net.Sockets;
using log4net;
using Services;

namespace Networking;

public abstract class AbstractServer(string ip, int port)
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(AbstractServer));
    private TcpListener? _server = null;
    public void Start()
    {
        try
        {
            _server = new TcpListener(IPAddress.Parse(ip), port);
            _server.Start();
            while (true)
            {
                Log.Debug($"Starting server at {ip}:{port}");
                var client = _server.AcceptSocket();
                ProcessRequest(client);
            }
        }
        catch (Exception e)
        {
            throw new AppException(e.Message, e);
        }
        finally
        {
            Stop();
        }
    }

    protected abstract void ProcessRequest(Socket client);

    public void Stop()
    {
        try
        {
            _server?.Stop();
        } catch(Exception e)
        {
            Log.Error(e.Message);
        }
    }
}