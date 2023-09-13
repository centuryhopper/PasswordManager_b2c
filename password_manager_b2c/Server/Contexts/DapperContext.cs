using System.Data;
using Npgsql;

// Server=suleiman.db.elephantsql.com;Database=bsqwowlw;User Id=bsqwowlw;Password=YjBJGARHBrBsODfom0W-7W4XxxKkaIot;Port=5432

namespace password_manager_b2c.Server.Contexts;

public class DapperContext
{
    private readonly IConfiguration config;
    private readonly string ConnectionString;

    public DapperContext(IConfiguration config)
    {
        this.config = config;
        ConnectionString = GetConnectionString();
        System.Console.WriteLine(ConnectionString);
    }

    private string GetConnectionString()
    {
        // ElephantSQL formatting
        var uriString = config.GetConnectionString("CloudConnectionString")!;
        var uri = new Uri(uriString);
        var db = uri.AbsolutePath.Trim('/');
        var user = uri.UserInfo.Split(':')[0];
        var passwd = uri.UserInfo.Split(':')[1];
        var port = uri.Port > 0 ? uri.Port : 5432;
        var connStr = string.Format("Server={0};Database={1};User Id={2};Password={3};Port={4}",
            uri.Host, db, user, passwd, port);
        return connStr;
    }

    public IDbConnection CreateConnection() => new NpgsqlConnection(ConnectionString);
}

