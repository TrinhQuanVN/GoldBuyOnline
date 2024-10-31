using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Metadata;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BuyGolgBidv
{
    public class ApiAntiCaptchaTop
    {
        private string _apiKey;
        private string _urlApi = "https://anticaptcha.top/in.php";


        public string SourceCode { get; set; }
        public string UrlContainCaptcha { get; set; }

        public ApiAntiCaptchaTop(string apiKey, string sourceCode, string urlContainCaptcha)
        {
            _apiKey = apiKey;
            SourceCode = sourceCode;
            UrlContainCaptcha = urlContainCaptcha;
        }
        private string GetCaptChaToken()
        {
            var match = Regex.Match(SourceCode, "/ 'sitekey':\\s * '([A-Za-z0-9_\\-]+)' /");
            return match.Success ? match.Groups[1].Value : string.Empty;
        }
        private string GetSiteKey()
        {
            var match = Regex.Match(SourceCode, "/ 'sitekey':\\s * '([A-Za-z0-9_\\-]+)' /");
            return match.Success ? match.Groups[1].Value : string.Empty;
        }
        private async Task<string> PostReuqest()
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, _urlApi);

                var jsonContent = $@"{{
                    ""key"": ""{_apiKey}"",
                    ""method"": ""userrecaptcha"",
                    ""googlekey"": ""{GetSiteKey()}"",
                    ""pageurl"": ""{UrlContainCaptcha}""
                }}";
                request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();

                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception)
            {

                PrintMessage.Error("Error on post request captcha");
                return string.Empty;
            }
        }
        private string ReadReponseCaptcha(string reponse)
        {
            try
            {
                return reponse.ToString().Split('|')[1];
            }
            catch (Exception)
            {
                PrintMessage.Error("Error on read reponse captcha, not correct form: ");
                return string.Empty;
            }
        }
        public async Task<string> SolveCaptcha(IPage page)
        {
            try
            {
                var response = await PostReuqest();
                var result = ReadReponseCaptcha(response);

                return result;
            }
            catch (Exception)
            {
                PrintMessage.Error("Error on solve captcha ");
                return string.Empty;
            }

        }
    }
}
