using Microsoft.EntityFrameworkCore;
using Model;

namespace Persistence;

public class UserRepoEf(AppDbContext db) : IUserRepo
{
    public List<User> FindAll()
    {
        return db.User.AsNoTracking().ToList();
    }

    public User? FindOne(long id)
    {
        return db.User.AsNoTracking().Where(u => u.Id == id).FirstOrDefault();
    }

    public User? VerifyLogin(string username, string password)
    {
        return db.User.AsNoTracking().Where(u => (u.Username == username && u.Password == EncryptUtils.Encrypt(password, "mpp"))).FirstOrDefault();
    }
}