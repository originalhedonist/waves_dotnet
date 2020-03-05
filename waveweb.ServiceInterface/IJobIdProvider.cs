using System;

namespace waveweb.ServiceInterface
{
    public interface IJobIdProvider
    {
        Guid JobId { get; }
    }
}