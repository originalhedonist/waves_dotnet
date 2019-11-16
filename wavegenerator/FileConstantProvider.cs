using System.IO;
using System.Linq;

namespace wavegenerator
{
    public class FileConstantProvider : IConstantProvider
    {
        private readonly ILookup<string, string> keys;

        public FileConstantProvider(string filePath)
        {
            var lines = File.ReadAllLines(filePath);
            keys = lines.Select(l => l.Split(new[] { '=' }, 2).Select(kv => kv.Trim()).ToArray()).ToLookup(s => s[0], s => s[1]);
        }

        public string GetValue(string key)
        {
            return keys[key].FirstOrDefault();
        }
    }
}
