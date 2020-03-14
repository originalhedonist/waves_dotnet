using ServiceStack.Web;
using System.Collections.Generic;
using System.IO;
using System.Threading.Tasks;

namespace waveweb.ServiceInterface
{
    public class DownloadFileResult : MemoryStream, IHasOptions
    {
        private readonly string downloadName;
        private DownloadFileResult() {}
        private DownloadFileResult(string downloadName)
        {
            this.downloadName = downloadName;
        }

        public static async Task<DownloadFileResult> Create(string downloadName, string fullPath) 
        {
            var r = new DownloadFileResult(downloadName);
            await using var fs = File.OpenRead(fullPath);
            await fs.CopyToAsync(r);
            r.Seek(0, SeekOrigin.Begin);
            return r;
        }

        public IDictionary<string, string> Options => new Dictionary<string, string>
        {
            { "Content-Disposition", $"attachment;filename={downloadName}" }
        };
    }
}
