using System;

namespace wavegenerator
{
    public class ConsoleProgressReporter : IProgressReporter
    {
        private readonly int row;
        private readonly int colMin;
        private readonly int colCount;
        public ConsoleProgressReporter(int row, int colMin, int colCount)
        {
            this.row = row;
            this.colMin = colMin;
            this.colCount = colCount;
        }

        public void ReportProgress(double progress)
        {
            double colsToFill = progress * colCount;
            lock(Program.ConsoleLockObj)
            {
                Console.SetCursorPosition(colMin, row);
                for(int i = 0;i < colsToFill; i++)
                {
                    Console.Write('#');
                }
            }
        }

        public void AddMessage(string message)
        {
            lock(Program.ConsoleLockObj)
            {
                Console.SetCursorPosition(colMin + colCount + 2, row);
                Console.Write(message);
            }
        }
    }

    public interface IProgressReporter
    {
        void ReportProgress(double progress);
        void AddMessage(string message);
    }
}
