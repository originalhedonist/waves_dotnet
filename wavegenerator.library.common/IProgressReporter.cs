using System.Threading.Tasks;
using wavegenerator.models;

namespace wavegenerator.library
{
    public interface IProgressReporter
    {
        Task ReportProgress(double progress, JobProgressStatus status, string message);
    }
}
