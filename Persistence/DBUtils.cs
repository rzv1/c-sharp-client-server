using System.Data;

namespace Persistence;

public class DbUtils
{
    private static IDbConnection? _instance;
    
    public static IDbConnection GetConnection(IDictionary<string, string> props)
    {
        if (_instance != null && _instance.State != ConnectionState.Closed)
            return _instance;

        _instance = GetNewConnection(props);
        _instance.Open();
        return _instance;
    }

    private static IDbConnection GetNewConnection(IDictionary<string, string> props)
    {
        return ConnectionFactory.GetInstance().CreateConnection(props);
    }
}