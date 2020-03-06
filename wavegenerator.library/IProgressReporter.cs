using System.Threading.Tasks;

namespace wavegenerator.library
{
    public interface IProgressReporter
    {
        Task ReportProgress(double progress, bool isComplete, string message);
    }
}
