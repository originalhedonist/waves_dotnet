using System;
using System.Threading.Tasks;
using wavegenerator.library;
using wavegenerator.models;

namespace wavegenerator
{
    public class ConsoleProgressReporter : IProgressReporter
    {
        private int maxLen;
        public async Task ReportProgress(double progress, JobProgressStatus status, string message)
        {
            var messageToWrite = $"{(progress):0.00%} {message}";
            maxLen = Math.Max(maxLen, messageToWrite.Length);
            await Console.Out.WriteAsync($"\r{messageToWrite.PadRight(maxLen, ' ')}");
            if(status != JobProgressStatus.InProgress)
            {
                await Console.Out.WriteLineAsync();
            }
        }
    }
}
