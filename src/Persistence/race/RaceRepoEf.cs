using Microsoft.EntityFrameworkCore;
using Model;

namespace Persistence;

public class RaceRepoEf(AppDbContext db) : IRaceRepo
{
    public List<Race> FindAll()
    {
        var races = db.Race.AsNoTracking().ToList();
        races.ForEach(r => r.Participants = FindAllParticipantsByRace(r.Id));
        return races;
    }

    public Race? FindOne(long id)
    {
        var race = db.Race.AsNoTracking().FirstOrDefault(r => r.Id == id);
        if (race != null)
        {
            race.Participants = FindAllParticipantsByRace(race.Id);
        }
        return race;
    }

    public List<long> FindAllParticipantsByRace(long raceId)
    {
        return db.Registration.AsNoTracking().Where(reg => reg.RaceId == raceId).ToList().ConvertAll(r => r.ParticipantId);
    }
}