
using Model;

namespace Persistence;

public interface IUserRepo : IDbRepo<User, long>
{
    User? VerifyLogin(string username, string password);
}