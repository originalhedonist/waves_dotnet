using Microsoft.Extensions.Configuration;
using ServiceStack;
using System.Data.SqlClient;
using System.Threading.Tasks;
using waveweb.ServiceModel;

namespace waveweb.ServiceInterface
{
    public class MiscService : Service
    {
        private readonly IConfiguration config;

        public MiscService(IConfiguration config)
        {
            this.config = config;
        }
        //Return index.html for unmatched requests so routing is handled on client
        public object Any(FallbackForClientRoutes request) => Request.GetPageResult("/");

        public async Task<TestResponse> Post(TestRequest request)
        {
            var connectionString = config.GetConnectionString("DefaultConnection");
            await using var sqlConnection = new SqlConnection(connectionString);
            await using var sqlCommand = new SqlCommand("select @@version", sqlConnection);
            await sqlConnection.OpenAsync();
            var commandResult = await sqlCommand.ExecuteScalarAsync();
            var versionString = commandResult.ToString();
            return new TestResponse { Message = $"Hello from SQL Server: {versionString}" };
        }
    }
}
