using ServiceStack.Web;
using System.Collections.Generic;
using System.IO;
using System.Text;
using System.Threading.Tasks;

namespace waveweb.ServiceInterface
{
    public class NotFoundFileResult : MemoryStream, IHasOptions
    {
        private NotFoundFileResult()
        {

        }

        public static async Task<NotFoundFileResult> Create()
        {
            var r = new NotFoundFileResult();
            var b = Encoding.UTF8.GetBytes("The file is no longer on the server. It could have been deleted as it was too old. Please try again, or if it was not long since created, please raise an issue on the project webpage.");
            await r.WriteAsync(b);
            r.Seek(0, SeekOrigin.Begin);
            return r;
        }
        public IDictionary<string, string> Options => new Dictionary<string, string>
        {
            { "Content-Disposition", $"attachment;filename=NotFound.txt" }
        };
    }
}
