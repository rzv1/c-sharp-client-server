using System.Data;
using log4net;
using Model;

namespace Persistence;

public class RaceRepo(IDictionary<string, string> props) : IRaceRepo
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(RaceRepo));

    public List<Race> FindAll()
    {
        Log.Info("Finding all races");
        var conn = DbUtils.GetConnection(props);
        var races = new List<Race>();
        using var command = conn.CreateCommand();
        command.CommandText = "select * from race";
        
        using var dataR = command.ExecuteReader();
        while (dataR.Read())
        {
            races.Add(GetRace(dataR));
        }
        
        Log.Info("Exiting races find all");
        return races;
    }

    private Race GetRace(IDataReader dataR)
    {
        var id = dataR.GetInt64(0);
        var style = dataR.GetString(1);
        var distance = dataR.GetString(2);
        var participants = FindAllParticipantsByRace(id);
        return new Race(style, distance) { Id = id, Participants = participants }; 
    }

    public Race? FindOne(long id)
    {
        Log.Info("Finding race wiht id = " + id);
        var conn = DbUtils.GetConnection(props);

        using var command = conn.CreateCommand();
        command.CommandText = "select * from race where id = @id";
        
        var paramId = command.CreateParameter();
        paramId.ParameterName = "@id";
        paramId.Value = id;
        command.Parameters.Add(paramId);

        using var dataR = command.ExecuteReader();
        
        return dataR.Read() ? GetRace(dataR) : null;
    }

    public List<long> FindAllParticipantsByRace(long raceId)
    {
        Log.InfoFormat("Finding all participants for race {0}", raceId);
        var conn = DbUtils.GetConnection(props);
        var participants = new List<long>();

        using var command = conn.CreateCommand();
        command.CommandText = "select participant from registration where race = @raceId";
        
        var paramId = command.CreateParameter();
        paramId.ParameterName = "@raceId";
        paramId.Value = raceId;
        command.Parameters.Add(paramId);

        using var dataR = command.ExecuteReader();
        while (dataR.Read())
        {
            participants.Add(dataR.GetInt64(0));
        }
        return participants;
    }
}