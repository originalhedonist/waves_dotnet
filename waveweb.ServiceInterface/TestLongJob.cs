using System;
using System.Threading;
using System.Threading.Tasks;

namespace waveweb.ServiceInterface
{
    public class TestLongJob : ILongJobProcessor<long>
    {
        private readonly IJobProgressProvider jobProgressProvider;

        public TestLongJob(IJobProgressProvider jobProgressProvider)
        {
            this.jobProgressProvider = jobProgressProvider;
        }

        public async Task Run(long data, Guid jobId, CancellationToken cancellationToken)
        {
            for (long i = 0; i < data; i++)
            {
                await Task.Delay(TimeSpan.FromSeconds(10));
                await jobProgressProvider.SetJobProgressAsync(jobId, (double)i / data);
            }
        }
    }

}
