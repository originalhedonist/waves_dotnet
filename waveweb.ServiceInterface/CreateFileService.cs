using ServiceStack;
using waveweb.ServiceModel;

namespace waveweb.ServiceInterface
{
    public class CreateFileService : Service
    {
        public object Any(CreateFileRequest request)
        {
            return new CreateFileResponse();
        }
    }
}
