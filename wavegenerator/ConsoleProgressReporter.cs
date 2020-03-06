using System;
using System.Threading.Tasks;
using wavegenerator.library;

namespace wavegenerator
{
    public class ConsoleProgressReporter : IProgressReporter
    {
        private int maxLen;
        public async Task ReportProgress(double progress, bool isComplete, string message)
        {
            var messageToWrite = $"{(progress):0.00%} {message}";
            maxLen = Math.Max(maxLen, messageToWrite.Length);
            await Console.Out.WriteAsync($"\r{messageToWrite.PadRight(maxLen, ' ')}");
            if(isComplete)
            {
                await Console.Out.WriteLineAsync();
            }
        }
    }
}
