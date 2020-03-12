using System.IO;
using wavegenerator.library;

namespace waveweb.ServerComponents
{
    public class WebOutputDirectoryProvider : IOutputDirectoryProvider
    {
        internal const string OutputDir = "DownloadableFiles";
        public string GetOutputDirectory()
        {
            if(!Directory.Exists(OutputDir))
            {
                Directory.CreateDirectory(OutputDir);
            }
            return OutputDir;
        }
    }
}
