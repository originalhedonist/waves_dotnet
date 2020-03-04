using ServiceStack;

namespace waveweb.ServiceModel
{
    [Route("/testrequest")]
    public class TestRequest : IReturn<TestResponse>
    {

    }
}
