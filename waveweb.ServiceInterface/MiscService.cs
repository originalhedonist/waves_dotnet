using ServiceStack;

namespace waveweb.ServiceInterface
{
    public class MiscService : Service
    {
        //Return index.html for unmatched requests so routing is handled on client
        public object Any(FallbackForClientRoutes request) => Request.GetPageResult("/");
    }
}
