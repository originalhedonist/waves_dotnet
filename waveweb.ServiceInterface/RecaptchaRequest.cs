namespace waveweb.ServiceInterface
{
    public class RecaptchaRequest
    {
        // https://developers.google.com/recaptcha/docs/verify
        /*
         POST Parameter	Description
secret	Required. The shared key between your site and reCAPTCHA.
response	Required. The user response token provided by the reCAPTCHA client-side integration on your site.
remoteip	Optional. The user's IP address.
*/
        public string secret { get; set; }
        public string response { get; set; }
        public string remoteip { get; set; }
    }
}
