using System.Net.Sockets;
using log4net;

namespace Networking;

public abstract class AbstractConcurrentServer(string ip, int port) : AbstractServer(ip, port)
{
    private static readonly ILog Log = LogManager.GetLogger(nameof(ConcurrentServer));
    
    protected override void ProcessRequest(Socket client)
    {
        Thread tw = CreateWorkerThread(client);
        tw.Start();
        Log.Info($"Client {client.RemoteEndPoint} started");
    }
    
    protected abstract Thread CreateWorkerThread(Socket client);
}