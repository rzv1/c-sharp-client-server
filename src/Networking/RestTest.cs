using System.Net.Http.Json;
using Model;

namespace Networking;

public class RestTest
{
    public static void Main(string[] args)
    {
        var handler = new LoggingHandler(new HttpClientHandler());
        using var client = new HttpClient(handler);
        client.BaseAddress = new Uri("http://localhost:8080/api/races");

        try
        {
            Console.WriteLine("> Test Get All");
            var races = client.GetFromJsonAsync<List<Race>>("").Result;
            Console.WriteLine(races!);

            Console.WriteLine("> Test Get By ID");
            var race = client.GetFromJsonAsync<Race>($"races/2").Result;
            Console.WriteLine(race);

            Console.WriteLine("> Test Get Filtered");
            var race2 = client.GetFromJsonAsync<Race>($"races/2").Result;
            Console.WriteLine(race2);

            Console.WriteLine("> Test Add");
            var newRace = new Race("100m", "backstroke") { Participants = [1, 2] };
            var createdRace = client.PostAsJsonAsync("races", newRace).Result.Content.ReadFromJsonAsync<Race>().Result;
            Console.WriteLine(createdRace);

            Console.WriteLine("> Test Update for id 3");
            createdRace!.Style = "NewStyle";
            var putResponse = client.PutAsJsonAsync($"races/3", createdRace).Result;

            Console.WriteLine("> Test Delete");
            var deleteResponse = client.DeleteAsync("races/3").Result;
            Console.WriteLine($"Delete status: {deleteResponse.StatusCode}");
            
        } catch(Exception e)
        {
            Console.WriteLine(e.Message);
        }
    }
}