using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;
using System.Threading.Tasks;
using waveweb.ServiceInterface;
using waveweb.ServiceModel;

namespace waveweb.ServerComponents
{
    public class JobProgressProvider : IJobProgressProvider
    {
        private readonly IConfiguration configuration;

        public JobProgressProvider(IConfiguration configuration)
        {
            this.configuration = configuration;
        }
        public async Task SetJobProgressAsync(Guid jobId, JobProgress jobProgress)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            await using var connection = new SqlConnection(connectionString);
            var progressToSet = Math.Max(jobProgress.Progress, 0.05);
            await using var command = new SqlCommand(@"
                INSERT INTO JobProgress (JobId, Progress, DateLastUpdated, IsComplete)
                SELECT @JobId, @Progress, GETDATE(), @IsComplete
                WHERE NOT EXISTS (SELECT * FROM JobProgress WHERE JobId = @JobId)

                UPDATE JobProgress SET Progress = @Progress, DateLastUpdated = GETDATE(), IsComplete = @IsComplete
                WHERE JobId = @JobId",
                connection);
            command.Parameters.AddWithValue("@JobId", jobId);
            command.Parameters.AddWithValue("@Progress", progressToSet);
            command.Parameters.AddWithValue("@IsComplete", jobProgress.IsComplete);
            await connection.OpenAsync();
            await command.ExecuteNonQueryAsync();
        }

        public async Task<JobProgress> GetJobProgressAsync(Guid jobId)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            await using var connection = new SqlConnection(connectionString);
            await using var command = new SqlCommand(
                "SELECT Progress, IsComplete FROM JobProgress WHERE JobId = @JobId", connection);
            command.Parameters.AddWithValue("@JobId", jobId);
            await connection.OpenAsync();
            var result = await command.ExecuteReaderAsync(System.Data.CommandBehavior.SingleRow);
            var jobProgress = new JobProgress();
            if(await result.ReadAsync())
            {
                jobProgress.IsComplete = (bool)Convert.ChangeType(result["IsComplete"], typeof(bool));
                jobProgress.Progress = (double)Convert.ChangeType(result["Progress"], typeof(double));
            }
            return jobProgress;
        }
    }
}
