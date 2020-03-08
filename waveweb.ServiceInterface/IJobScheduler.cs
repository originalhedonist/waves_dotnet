﻿using Hangfire;
using System;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;

namespace waveweb.ServiceInterface
{
    public interface IJobScheduler
    {
        Task ScheduleJob<TService, TData>(Guid jobId, TData data) where TService : ILongJobProcessor<TData>;
    }

    public interface ILongJobProcessor<TData>
    {
        [AutomaticRetry(Attempts = 0)]
        Task Run(TData data, Guid jobId, CancellationToken cancellationToken);
    }
}