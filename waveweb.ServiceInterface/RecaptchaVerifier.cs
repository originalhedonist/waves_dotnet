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
            this.recaptchaClient = new HttpClient();
            this.configuration = configuration;
        }

        public async Task<bool> VerifyAsync(string userResponse, string remoteIp)
        {
            var secret = configuration.GetValue<string>("Recaptcha:SecretKey");
            var formData = new MultipartFormDataContent
            {
                { new StringContent(remoteIp), "remoteip" },
                { new StringContent(userResponse), "response" },
                { new StringContent(secret), "secret" }
            };
            var response = await recaptchaClient.PostAsync("https://www.google.com/recaptcha/api/siteverify", formData);
            response.EnsureSuccessStatusCode();
            var recaptchaResponse = await response.Content.ReadAsAsync<RecaptchaResonse>();
            return recaptchaResponse.success && recaptchaResponse.score >= 0.5;
        }
    }
}
