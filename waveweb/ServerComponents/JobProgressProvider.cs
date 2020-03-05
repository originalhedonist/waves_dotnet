using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using waveweb.ServiceInterface;

namespace waveweb.ServerComponents
{
    public class JobProgressProvider : IJobProgressProvider
    {
        private readonly IConfiguration configuration;

        public JobProgressProvider(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task SetJobProgressAsync(Guid jobId, double progress)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            await using var connection = new SqlConnection(connectionString);
            var progressToSet = Math.Max(progress, 0.05);
            await using var command = new SqlCommand(@"
                INSERT INTO JobProgress (JobId, Progress, DateLastUpdated)
                SELECT @JobId, @Progress, GETDATE()
                WHERE NOT EXISTS (SELECT * FROM JobProgress WHERE JobId = @JobId)

                UPDATE JobProgress SET Progress = @Progress, DateLastUpdated = GETDATE()
                WHERE JobId = @JobId",
                connection);
            command.Parameters.AddWithValue("@JobId", jobId);
            command.Parameters.AddWithValue("@Progress", progress);
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<double> GetJobProgressAsync(Guid jobId)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            await using var connection = new SqlConnection(connectionString);
            await using var command = new SqlCommand(
                "SELECT Progress FROM JobProgress WHERE JobId = @JobId", connection);
            command.Parameters.AddWithValue("@JobId", jobId);
            await connection.OpenAsync();
            var result = await command.ExecuteScalarAsync();
            double progress = DBNull.Value.Equals(result) ?
                0 :
                (double)Convert.ChangeType(result, typeof(double));
            return progress;
        }
    }
}
