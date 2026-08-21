using System.Net.Sockets;
using Services;

namespace Networking;

public class ConcurrentServer(string ip,  int port, IServices clientProxy) : AbstractConcurrentServer(ip, port)
{
    protected override Thread CreateWorkerThread(Socket client)
    {
        ClientWorker worker = new ClientWorker(clientProxy, client);
        return new Thread(worker.Run);
    }
}