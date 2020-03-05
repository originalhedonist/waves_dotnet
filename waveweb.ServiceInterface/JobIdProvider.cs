using System;

namespace waveweb.ServiceInterface
{
    public class JobIdProvider : IJobIdProvider
    {
        public Guid JobId { get; set; }
    }


}
