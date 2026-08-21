namespace Networking;

public class LoggingHandler : DelegatingHandler
{
    public  LoggingHandler(HttpMessageHandler innerHandler) : base(innerHandler)
    {}

    protected override async Task<HttpResponseMessage> SendAsync(HttpRequestMessage request,
        CancellationToken cancellationToken)
    {
        Console.WriteLine("Request (" + request.Method + "): " + request.RequestUri);
        if (request.Content != null)
        {
            var content = await request.Content.ReadAsStringAsync();
            Console.WriteLine(content);
        }
        var response = await base.SendAsync(request, cancellationToken);

        Console.WriteLine("Response status: " + response.StatusCode);
        Console.WriteLine("-----------------------------");
        return response;
    }
}