using System.Data;
using log4net;
using Model;

namespace Persistence;

public class UserRepo(IDictionary<string, string> props) : IUserRepo
{
    private static readonly ILog Logger = LogManager.GetLogger(typeof(UserRepo));

    public User? VerifyLogin(string username, string password)
    {
        Logger.InfoFormat("Trying to login with user ({0}) and password ({1})", username, password);
        var conn = DbUtils.GetConnection(props);
        using var command = conn.CreateCommand();
        command.CommandText = "select * from user where username = @User and password = @Pass";
        
        IDbDataParameter userParam = command.CreateParameter();
        userParam.ParameterName = "@User";
        userParam.Value = username;
        command.Parameters.Add(userParam);
        
        IDbDataParameter passParam = command.CreateParameter();
        passParam.ParameterName = "@Pass";
        passParam.Value = EncryptUtils.Encrypt(password, "mpp");
        command.Parameters.Add(passParam);
        
        using var reader = command.ExecuteReader();
        if (reader.Read())
        {
            Logger.Info("Login successful");
            return new User(username, password);
        }
        Logger.Info("Incorrect credentials");
        return null;
    }

    public List<User> FindAll()
    {
        throw new NotImplementedException();
    }

    public User FindOne(long id)
    {
        throw new NotImplementedException();
    }
}