using Model;

namespace Persistence;

public interface IDbRepo<T, ID> where T : Entity<ID>
{
     List<T> FindAll();
     T? FindOne(ID id);
}