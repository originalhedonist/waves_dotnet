using ServiceStack.Web;
using System.Collections.Generic;
using System.IO;

namespace waveweb.ServiceInterface
{
    public class DownloadFileResult : FileStream, IHasOptions
    {
        private readonly string downloadName;

        public DownloadFileResult(string downloadName, string fullPath) :
            base(fullPath, FileMode.Open, FileAccess.Read)
        {
            this.downloadName = downloadName;
        }

        public IDictionary<string, string> Options => new Dictionary<string, string>
        {
            { "Content-Disposition", $"attachment;filename={downloadName}" }
        };
    }
}
