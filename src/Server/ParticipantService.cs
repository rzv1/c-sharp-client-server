using Model;
using Persistence;

namespace Server;

public class ParticipantService(IParticipantRepo repo)
{
    public IEnumerable<Participant> GetAll()
    {
        return repo.FindAll();
    }

    public IEnumerable<Participant> GetAllById(List<long> ids)
    {
        var participants = new List<Participant>();
        ids.ForEach(id => participants.Add(repo.FindOne(id)!));
        return participants;
    }

    public Participant Save(string name, int age, List<long> races)
    {
        return repo.Save(new Participant(name, age) { Races = races });
    }

    public void Update(long id, List<long> races)
    {
        var oldRaces = repo.FindAllRacesForParticipant(id);
        oldRaces.ForEach(r => repo.DeleteRegistration(id, r));
        races.ForEach(r => repo.SaveRegistration(id, r));
    }
}