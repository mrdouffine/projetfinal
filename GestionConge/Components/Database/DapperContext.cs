using Npgsql;

namespace GestionConge.Components.Database;

public class DapperContext
{
    private readonly IConfiguration _configuration;
    private readonly string _connectionString;

    public DapperContext(IConfiguration configuration)
    {
        _configuration = configuration;
        _connectionString = _configuration.GetConnectionString("DefaultConnection");
    }

    public NpgsqlConnection CreateConnection()
    {
        var connection = new NpgsqlConnection(_connectionString);
        connection.Open(); // Ouvre directement la connexion
        return connection;
    }
}
