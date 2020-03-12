using System;

namespace wavegenerator.library
{
    public class CurrentDirectoryProvider : IOutputDirectoryProvider
    {
        public string GetOutputDirectory() => Environment.CurrentDirectory;
    }
}
