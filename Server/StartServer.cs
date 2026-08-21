using log4net;
using Networking;
using Persistence;
using Services;

namespace Server;
using System.Configuration;

public class StartServer
{
    private static readonly string Ip = ConfigurationManager.AppSettings["ServerIp"]!;
    private static readonly string Port = ConfigurationManager.AppSettings["ServerPort"]!;
    private static readonly ILog Log = LogManager.GetLogger(typeof(StartServer));
    public static void Main(string[] args)
    {
        log4net.Config.XmlConfigurator.Configure(new System.IO.FileInfo("log4net.config"));
        Log.Info("Initializing services...");
        var props = new SortedList<string, string>();
        var connectionString = ConfigurationManager.ConnectionStrings["SqliteConnection"].ConnectionString;
        props.Add("ConnectionString", connectionString);
        Log.InfoFormat("Connecting with database url {0}...", props["ConnectionString"]);
        var context = new AppDbContext(props["ConnectionString"]);
        var userRepoEf = new UserRepoEf(context);
        var raceRepoEf = new RaceRepoEf(context);
        var participantRepoEf = new ParticipantRepoEf(context);
        var userRepo = new UserRepo(props);
        var participantRepo = new ParticipantRepo(props);
        var raceRepo = new RaceRepo(props);
        var userService = new UserService(userRepoEf);
        var participantService = new ParticipantService(participantRepoEf);
        var raceService = new RaceService(raceRepoEf);
        var services = new ServiceImpl(userService, participantService, raceService);
        var server = new ConcurrentServer(Ip, int.Parse(Port), services);
        server.Start();
    }
}