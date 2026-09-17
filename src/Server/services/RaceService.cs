using Model;
using Persistence;
using Protobuf;

namespace Server;

public class RaceService(IRaceRepo repo)
{
    public IEnumerable<Race> GetAll()
    {
        return repo.FindAll();
    }

    public IEnumerable<Race> GetAllById(List<long> ids)
    {
        var races = new List<Race>();
        ids.ForEach(id => races.Add(repo.FindOne(id)!));
        return races;
    }
}