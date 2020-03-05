using AutoMapper;
using ServiceStack;
using System;
using System.Threading;
using System.Threading.Tasks;
using wavegenerator.models;
using waveweb.ServiceModel;

namespace waveweb.ServiceInterface
{
    public class CreateFileService : Service
    {
        private readonly IMapper mapper;
        private readonly IJobScheduler jobScheduler;

        public CreateFileService(IMapper mapper, IJobScheduler jobScheduler)
        {
            this.mapper = mapper;
            this.jobScheduler = jobScheduler;
        }

        public async Task<CreateFileResponse> Post(CreateFileRequest request)
        {
            var settings = mapper.Map<CreateFileRequest, Settings>(request);
            var jobId = Guid.NewGuid();
            await jobScheduler.ScheduleJob<FileCreator, Settings>(jobId, settings);
            return new CreateFileResponse { JobId = jobId };
        }
    }
}
