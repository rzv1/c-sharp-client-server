
using Model;
using Persistence;

namespace Server;

public class UserService(IUserRepo repo)
{
    public User? VerifyLogin(string username, string password)
    {
        return repo.VerifyLogin(username, password);
    }
}