using System.Data;
using System.Data.Common;
using Npgsql;

namespace BidService.Data;

public class BidDbContext
{
    public BidDbContext(IConfiguration configuration)
    {
        var connectionString = configuration.GetConnectionString("BidDbConnection");
        DbConnection = new NpgsqlConnection(connectionString);
    }
    
    public IDbConnection DbConnection { get; }
}