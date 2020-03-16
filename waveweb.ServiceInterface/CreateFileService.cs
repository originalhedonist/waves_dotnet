using AutoMapper;
using ServiceStack;
using System;
using System.Threading.Tasks;
using wavegenerator.models;
using waveweb.ServiceModel;

namespace waveweb.ServiceInterface
{
    public class CreateFileService : Service
    {
        private readonly IMapper mapper;
        private readonly IJobScheduler jobScheduler;
        private readonly RecaptchaVerifier recaptchaVerifier;

        public CreateFileService(IMapper mapper, IJobScheduler jobScheduler, RecaptchaVerifier recaptchaVerifier)
        {
            this.mapper = mapper;
            this.jobScheduler = jobScheduler;
            this.recaptchaVerifier = recaptchaVerifier;
        }

        public async Task<CreateFileResponse> Post(CreateFileRequest request)
        {
            if (!await recaptchaVerifier.VerifyAsync(request.RecaptchaToken, Request.RemoteIp))
                throw new InvalidOperationException($"An error occurred validating the parameters.");

            var jobId = Guid.NewGuid();
            var settings = mapper.Map<CreateFileRequest, Settings>(request);
            await jobScheduler.ScheduleJob<FileCreator, Settings>(jobId, settings);
            return new CreateFileResponse { JobId = jobId };
        }
    }
}
