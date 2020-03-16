using Microsoft.Extensions.Configuration;
using System;
using System.Net.Http;
using System.Threading.Tasks;

namespace waveweb.ServiceInterface
{
    public class RecaptchaVerifier
    {
        private readonly HttpClient recaptchaClient;
        private readonly IConfiguration configuration;

        public RecaptchaVerifier(IConfiguration configuration)
        {
            this.recaptchaClient = new HttpClient { BaseAddress = new Uri("https://www.google.com/recaptcha/") };
            this.configuration = configuration;
        }

        public async Task<bool> VerifyAsync(string userResponse, string remoteIp)
        {
            var secret = configuration.GetValue<string>("Recaptcha:SecretKey");
            var request = new RecaptchaRequest
            {
                remoteip = remoteIp,
                response = userResponse,
                secret = secret
            };
            var response = await recaptchaClient.PostAsJsonAsync("/api/siteverify", request);
            response.EnsureSuccessStatusCode();
            var recaptchaResponse = await response.Content.ReadAsAsync<RecaptchaResonse>();
            return recaptchaResponse.success;
        }
    }
}
