using Microsoft.Extensions.Configuration;
using System;
using System.Data.SqlClient;
using System.Linq;
using System.Threading.Tasks;
using Ultimate.ORM;
using wavegenerator.models;
using waveweb.ServiceInterface;
using waveweb.ServiceModel;

namespace waveweb.ServerComponents
{
    public class JobProgressProvider : IJobProgressProvider
    {
        private readonly IConfiguration configuration;
        private readonly IObjectMapper objectMapper;

        public JobProgressProvider(IConfiguration configuration, IObjectMapper objectMapper)
        {
            this.configuration = configuration;
            this.objectMapper = objectMapper;
        }
        public async Task<int> SetJobProgressAsync(Guid jobId, JobProgress jobProgress)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            await using var connection = new SqlConnection(connectionString);
            var progressToSet = Math.Max(jobProgress.Progress, 0.05);
            await using var command = new SqlCommand(@"                
                INSERT INTO JobProgress (JobId, Progress, DateLastUpdated, Status, Message)
                SELECT @JobId, @Progress, GETDATE(), @Status, @Message
                WHERE NOT EXISTS (SELECT * FROM JobProgress WHERE JobId = @JobId)
                DECLARE @rowsInserted int = @@ROWCOUNT

                UPDATE JobProgress SET Progress = @Progress, DateLastUpdated = GETDATE(), Status = @Status, Message = @Message
                WHERE JobId = @JobId
                
                SELECT @rowsInserted",
                connection);
            command.Parameters.AddWithValue("@JobId", jobId);
            command.Parameters.AddWithValue("@Progress", progressToSet);
            command.Parameters.AddWithValue("@Status", jobProgress.Status.ToString());
            command.Parameters.AddWithValue("@Message", jobProgress.Message);
            await connection.OpenAsync();
            var rowsInserted = await command.ExecuteScalarAsync();
            return Convert.ToInt32(rowsInserted);
        }

        public async Task<JobProgress> GetJobProgressAsync(Guid jobId)
        {
            var connectionString = configuration.GetConnectionString("DefaultConnection");
            await using var connection = new SqlConnection(connectionString);
            await using var command = new SqlCommand(
                "SELECT Progress, Status, Message FROM JobProgress WHERE JobId = @JobId", connection);
            command.Parameters.AddWithValue("@JobId", jobId);
            await connection.OpenAsync();
            var jobProgresses = await objectMapper.ToMultipleObjects<JobProgress>(command);
            var jobProgress = jobProgresses.FirstOrDefault() ?? new JobProgress { Message = "Starting...", Progress = 0, Status = JobProgressStatus.InProgress };
            return jobProgress;
        }
    }
}
