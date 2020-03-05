using ServiceStack;
using System;
using System.IO;

namespace waveweb.ServiceModel
{
    [Route("/downloadfile/{Id}")]
    public class DownloadFileRequest : IReturn<Stream>
    {
        public Guid Id { get; set; }
    }

    [Route("/testdownload")]
    public class TestDownloadRequest : IReturn<Stream>
    {

    }
}
