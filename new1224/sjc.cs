//dotnet add package Microsoft.Playwright
using Microsoft.Playwright;
using System.Text;
using System.Text.RegularExpressions;

namespace SJCGold
{
    class Program
    {
        static readonly string _sjcPageUrl = "https://tructuyen.sjc.com.vn";
        static string _sjcLoginPageUrl = "https://tructuyen.sjc.com.vn/dang-nhap";
        readonly static string _fullName = "trịnh tiến quân";
        readonly static string _idNumber = "035092013752";
        readonly static string _apiKey = "796b02353453441eb50179e374758059";
        static async Task Main(string[] args)
        {
            var playwright = await Playwright.CreateAsync();
             var browser = await playwright.Firefox.LaunchAsync(new() { Headless = false });
            var content = browser.NewContextAsync();
            var page = await browser.NewPageAsync();

            await GoPage(page, _sjcLoginPageUrl);
            await Login(page, _fullName, _idNumber);
            await Register(page, _apiKey);

        }
        static async Task GoPage(IPage page, string url)
        {
            await page.GotoAsync(url, new()
            {
                WaitUntil = WaitUntilState.Load
            });
            //return true;
        }
        static async Task Login(IPage page, string fullName, string idNumber)
        {
            await page.EvaluateAsync<string>(@"
         (fullname) => {
             function triggerEvent(el, type) {
                 const event = new Event(type, { bubbles: true });
                 el.dispatchEvent(event);
             }

             const tb1 = document.querySelector('#id_name');
             tb1.value = fullname;
             triggerEvent(tb1, 'change');
             triggerEvent(tb1, 'input');
         }", fullName);

            await page.EvaluateAsync<string>(@"
         (idNumber) => {
             function triggerEvent(el, type) {
                 const event = new Event(type, { bubbles: true });
                 el.dispatchEvent(event);
             }
             const tb2 = document.querySelector('#id_cccd');
             tb2.value = idNumber;
             triggerEvent(tb2, 'change');
             triggerEvent(tb2, 'input');
         }", idNumber);
            await page.ClickAsync("#sign_in_submit");
            await page.WaitForURLAsync(_sjcPageUrl, new()
            {
                WaitUntil = WaitUntilState.Load,
            });
        }
        static async Task Register(IPage page, string apiKey)
        {
            try
            {
                while (!IsAreaOpen(await page.ContentAsync()))
                {
                    await page.ReloadAsync();
                }
                while (true)
                {
                    await RegisterSingleAsync(page);
                    await page.ReloadAsync();
                }
            }
            catch (Exception)
            {

                Console.WriteLine("error");
            }
           

        }
        static async Task RegisterSingleAsync(IPage page)
        {
            try
            {
                await SelectArea(page);
                await SelectStore(page);
                //await SelectHinhThuc(page, index);
                await SolveCaptchaAsync(page);
                await page.ClickAsync("#register_form_submit");
                await page.WaitForURLAsync(_sjcPageUrl);

            }
            catch (Exception)
            {

                throw;
            }
        }
        static bool IsAreaOpen(string sourceCode)
        {
            return sourceCode.Contains("hà nội", StringComparison.OrdinalIgnoreCase);
        }
        static async Task SelectArea(IPage page)
        {
            try
            {
                await page.EvaluateAsync<string>(@"
                 () => {
                     function triggerEvent(el, type) {
                         const event = new Event(type, { bubbles: true });
                         el.dispatchEvent(event);
                     }

                     const area = document.querySelectorAll('#id_area')[0];
                     let hanoiArea = Array.from(area).find(option => option.innerText.trim() === 'Thành phố Hà Nội');

                     hanoiArea.selected = true;
                     triggerEvent(hanoiArea, 'change');
                     triggerEvent(hanoiArea, 'input');
                }");
            }
            catch (Exception)
            {
                Console.WriteLine("error on select area");
            }
           
        }
        static async Task SelectStore(IPage page)
        {
            try
            {
                await page.EvaluateAsync<string>(@"
         () => {
             function triggerEvent(el, type) {
                 const event = new Event(type, { bubbles: true });
                 el.dispatchEvent(event);
             }

             const store = document.querySelectorAll('#id_store')[0];
             const selectedStore = store[store.length-1];
             triggerEvent(selectedStore, 'change');
             triggerEvent(selectedStore, 'input');
         }");
            }
            catch (Exception)
            {

                Console.WriteLine("error on select store");
            }
            
        }
        private static async Task SolveCaptchaAsync(IPage page)
        {
            try
            {
                var sourceCode = await page.ContentAsync();
                var token = await GetTokenAsync(sourceCode);
                await page.EvaluateAsync(@"(token) => {
                    document.getElementById('g-recaptcha-response').innerHTML = token;}", token);

                await page.ClickAsync("#register_form_submit");
                await page.WaitForURLAsync(_sjcPageUrl);
            }
            catch (Exception)
            {
                Console.WriteLine("error on solve captcha");
            }
        }

        private static async Task<string> GetTokenAsync(string sourceCode)
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, _urlApi);

                var jsonContent = $@"{{
                    ""key"": ""{_apiKey}"",
                    ""method"": ""userrecaptcha"",
                    ""googlekey"": ""{GetSiteKey(sourceCode)}"",
                    ""pageurl"": ""{_sjcPageUrl}""
                }}";
                request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");

                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                var reponseString = await response.Content.ReadAsStringAsync();
                return reponseString.Split('|')[1];
            }
            catch (Exception)
            {
                Console.WriteLine("error");
                return string.Empty;
            }
        }

        private static object GetSiteKey(string sourceCode)
        {
            var match = Regex.Match(sourceCode, "/ 'sitekey':\\s * '([A-Za-z0-9_\\-]+)' /");
            return match.Success ? match.Groups[1].Value : string.Empty;
        }
    }
}

