using Model;

namespace Persistence;

public interface IRaceRepo : IDbRepo<Race, long>
{
    List<long> FindAllParticipantsByRace(long raceId);
}