using System;
using System.Collections.Generic;
using System.Linq;
using ServiceStack;
using ServiceStack.Script;
using waveweb.ServiceModel;

namespace waveweb.ServiceInterface
{
    public class CreateFileService : Service
    {
        //Return index.html for unmatched requests so routing is handled on client
        public object Any(FallbackForClientRoutes request) => Request.GetPageResult("/");

        public object Any(CreateFileRequest request)
        {
            return new CreateFileResponse();
        }

    }
}
