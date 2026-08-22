using System.Security.AccessControl;
using Microsoft.EntityFrameworkCore;
using Model;

namespace Persistence;

public class ParticipantRepoEf(AppDbContext db) : IParticipantRepo
{
    public List<Participant> FindAll()
    {
        var participants = db.Participant.AsNoTracking().ToList();
        participants.ForEach(p => p.Races = FindAllRacesForParticipant(p.Id));
        return participants; 
    }

    public Participant? FindOne(long id)
    {
        var part = db.Participant.AsNoTracking().FirstOrDefault(p => p.Id == id);
        if (part != null)
        {
            part.Races = FindAllRacesForParticipant(part.Id);
        }
        return part;
    }

    public Participant Save(Participant participant)
    {
        var newParticipant = db.Participant.Add(participant);
        db.SaveChanges();
        foreach (var raceId in participant.Races!)
        {
            SaveRegistration(newParticipant.Entity.Id, raceId);
        }
        db.SaveChanges();
        return participant;
    }

    public List<long> FindAllRacesForParticipant(long id)
    {
        return db.Registration.AsNoTracking().Where(r => r.ParticipantId == id).ToList().ConvertAll(r => r.RaceId);
    }

    public void SaveRegistration(long participantId, long raceId)
    {
        db.Registration.Add(new Registration
            {
                ParticipantId = participantId, RaceId = raceId
            }
        );
        db.SaveChanges();
    }

    public void DeleteRegistration(long participantId, long raceId)
    {
        var reg = db.Registration.FirstOrDefault(r => r.ParticipantId == participantId && r.RaceId == raceId);
        if (reg != null)
        {
            db.Registration.Remove(reg);
            db.SaveChanges();
        }
    }
}