
using Model;

namespace Persistence;

public interface IParticipantRepo : IDbRepo<Participant, long>
{
    Participant Save(Participant participant);
    List<long> FindAllRacesForParticipant(long id);
    void SaveRegistration(long participantId, long raceId);
    void DeleteRegistration(long participantId, long raceId);
}