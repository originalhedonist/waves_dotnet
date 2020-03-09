using System.Threading.Tasks;
using wavegenerator.library;
using wavegenerator.models;
using Xunit.Abstractions;

namespace wavegenerator.tests
{
    public class XUnitProgressReporter : IProgressReporter
    {
        private readonly ITestOutputHelper testOutputHelper;

        public XUnitProgressReporter(ITestOutputHelper testOutputHelper)
        {
            this.testOutputHelper = testOutputHelper;
        }

        public Task ReportProgress(double progress, JobProgressStatus status, string message)
        {
            testOutputHelper.WriteLine($"{progress:0.00%} {message}");
            return Task.CompletedTask;
        }
    }
}
