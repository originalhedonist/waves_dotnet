using ServiceStack;
using waveweb.ServiceModel;

namespace waveweb.ServiceInterface
{
    public class CreateFileService : Service
    {

        public CreateFileResponse Post(CreateFileRequest request)
        {
            return new CreateFileResponse();
        }
    }
}
