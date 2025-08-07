using System.Data;
using Dapper;

namespace GestionConge.Components.Repositories;
public class TestRepository
{
    private readonly IDbConnection _db;

    public TestRepository(IDbConnection db)
    {
        _db = db;
    }

    public async Task<string> TestConnectionAsync()
    {
        var result = await _db.ExecuteScalarAsync<string>("SELECT 'Connexion PostgreSQL OK';");
        return result;
    }
}

