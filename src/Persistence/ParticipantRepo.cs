using System.Data;
using log4net;
using Model;

namespace Persistence;

public class ParticipantRepo(IDictionary<string, string> props) : IParticipantRepo
{
    private static readonly ILog Log = LogManager.GetLogger(typeof(ParticipantRepo));

    public Participant Save(Participant it)
    {
        Log.InfoFormat("Saving participant {0}", it);
        var conn = DbUtils.GetConnection(props);

        using var command = conn.CreateCommand();
        command.CommandText = "insert into participant (name, age) values (@name, @age); select last_insert_rowid();";
        
        var paramName = command.CreateParameter();
        paramName.ParameterName = "@name";
        paramName.Value = it.Name;
        command.Parameters.Add(paramName);

        var paramAge = command.CreateParameter();
        paramAge.ParameterName = "@age";
        paramAge.Value = it.Age;
        command.Parameters.Add(paramAge);

        var generatedId = (long) command.ExecuteScalar()!;
        it.Id = generatedId;
        
        foreach (var raceId in it.Races)
        {
            SaveRegistration(generatedId, raceId);
        }
        Log.InfoFormat("Saved participant with id {0}", it.Id);
        return it;
    }

    private Participant GetParticipant(IDataReader dataR)
    {
        var id = dataR.GetInt64(0);
        var name = dataR.GetString(1);
        var age = dataR.GetInt32(2);
        var races = FindAllRacesForParticipant(id);
            
        return new Participant(name, age) { Id = id, Races = races }; 
    }

    public List<Participant> FindAll()
    {
        Log.Info("Finding all participants");
        var conn = DbUtils.GetConnection(props);
        var participants = new List<Participant>();

        using var command = conn.CreateCommand();
        command.CommandText = "select * from participant";
        using var dataR = command.ExecuteReader();
        while (dataR.Read())
        {
            participants.Add(GetParticipant(dataR));
        }
        Log.Info("Exiting participants find all");
        return participants;
    }

    public Participant? FindOne(long id)
    {
        Log.InfoFormat("Finding participant with id {0}", id);
        var conn = DbUtils.GetConnection(props);
        using var command = conn.CreateCommand();
        command.CommandText = "select * from participant where id = @id";
        
        var paramId = command.CreateParameter();
        paramId.ParameterName = "@id";
        paramId.Value = id;
        command.Parameters.Add(paramId);
        
        using var dataR = command.ExecuteReader();
        return dataR.Read() ? GetParticipant(dataR) : null ; 
    }

    public List<long> FindAllRacesForParticipant(long id)
    {
        Log.InfoFormat("Finding all races for participant {0}", id);
        var conn = DbUtils.GetConnection(props);
        var races = new List<long>();

        using var command = conn.CreateCommand();
        command.CommandText = "select race from registration where participant = @id";
        var paramId = command.CreateParameter();
        paramId.ParameterName = "@id";
        paramId.Value = id;
        command.Parameters.Add(paramId);

        using var dataR = command.ExecuteReader();
        while (dataR.Read())
        {
            races.Add(dataR.GetInt64(0));
        }
        return races;
    }
    
    public void SaveRegistration(long participantId, long raceId)
    {
        Log.InfoFormat("Saving a registration for race id {0} and participant id {1}", raceId, participantId);
        var conn = DbUtils.GetConnection(props);
        using var command = conn.CreateCommand();
        command.CommandText = "insert into registration(race, participant) values (@raceId, @participantId)";
        
        var paramRace = command.CreateParameter();
        paramRace.ParameterName = "@raceId";
        paramRace.Value = raceId;
        command.Parameters.Add(paramRace);
        
        var paramParticipant = command.CreateParameter();
        paramParticipant.ParameterName = "@participantId";
        paramParticipant.Value = participantId;
        command.Parameters.Add(paramParticipant);
        
        command.ExecuteNonQuery();
        Log.InfoFormat("Registration potentially saved");
    }

    public void DeleteRegistration(long participantId, long raceId)
    {
        Log.InfoFormat("Deleting a registration for race id {0} and participant id {1}", raceId, participantId);
        var conn = DbUtils.GetConnection(props);
        using var command = conn.CreateCommand();
        command.CommandText = "delete from registration where race = @raceId and participant = @participantId";
        
        var paramRace = command.CreateParameter();
        paramRace.ParameterName = "@raceId";
        paramRace.Value = raceId;
        command.Parameters.Add(paramRace);
        
        var paramParticipant = command.CreateParameter();
        paramParticipant.ParameterName = "@participantId";
        paramParticipant.Value = participantId;
        command.Parameters.Add(paramParticipant);
        
        command.ExecuteNonQuery();
        Log.InfoFormat("Registration potentially deleted");
    }
}