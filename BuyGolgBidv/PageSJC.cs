using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BuyGolgBidv
{
    public class PageSJC
    {
        public string QueryNameLogin { get; set; }
        public string QueryCccdLogin { get; set; }
        public string QueryAreaOption { get; set; }
        public string QueryStoreOption { get; set; }
        public string QueryButtonLogin { get; set; }
        public string QueryButtonRegister { get; set; }
        public string UrlLogin { get; set; }
        public object ApiAntiCaptchaTop { get; set; }//796b02353453441eb50179e374758059
        public object UrlRegister { get; set; }//https://tructuyen.sjc.com.vn/ 
        public string AreaName { get; set; }
        public int StoreIndex { get; set; }
        public async Task<string> GetSourceCode(IPage page)
        {
            return await page.ContentAsync();
        }

        public async Task<bool> IsLoginSuccess(IPage page)
        {
            var pageUrl = page.Url;
            return !pageUrl.Contains("dang-nhap",StringComparison.OrdinalIgnoreCase);
        }
        public string ScreenShotFolder()
        {
            string desktopPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "SJC_ScreenShot"); // Get desktop path
            Directory.CreateDirectory(desktopPath);
            return desktopPath;
        }

        public Task Reload(IPage page)
        {
            return page.ReloadAsync();
        }
        public Task<int> StoreCount(IPage page)
        {
            var storeElement = page.Locator(QueryStoreOption);
            return storeElement.CountAsync();
        }
        public async Task ChangeStoreIndex(IPage page)
        {
            var storeCount = await StoreCount(page);
            if( StoreIndex < storeCount)
            {
                StoreIndex++;
            }
            else
            {
                StoreIndex = 0;
            }

        }
        public async Task<bool> AccessLoginPage(IPage page)
        {
            try
            {
                await page.GotoAsync(UrlLogin, new()
                {
                    WaitUntil = WaitUntilState.Load // Wait for the load event
                });
                return true;
            }
            catch (Exception e)
            {

                PrintMessage.Error($"Error on GotoUrl login:{e.Message}");
                return false;
            }
        }
        public async Task<bool> Login(IPage page,UserInfo userInfo)
        {
            try
            {
                await page.FillAsync(QueryNameLogin, userInfo.Name);
                await page.FillAsync(QueryCccdLogin, userInfo.CCCD);

                //var respon = await page.RunAndWaitForNavigationAsync(() => page.ClickAsync(QueryButtonRegister));
                //if(respon != null && respon.Ok)
                //{
                //    return true;
                //}
                await page.ClickAsync(QueryButtonLogin);
                return true;

            }
            catch (Exception ex)
            {

                PrintMessage.Error($"Error on Login: {ex.Message}");
                return false;
            }
        }
        private string CreateScreenShotPath()
        {
            string fileName = $"SJC_{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.png"; // Name format
            
            return Path.Combine(ScreenShotFolder(), fileName);
        }
        public async Task<string> TakeScreenShot(IPage page)
        {
            try
            {
                string filePath = CreateScreenShotPath();
                await Task.Delay(1000);
                await page.ScreenshotAsync(new()
                {
                    Path = filePath,
                    FullPage = true,
                });
                return filePath;
            }
            catch (Exception ex)
            {
                PrintMessage.Error($"Take screen shot fail: {ex.Message}");
                return string.Empty;
            }
        }
        public async Task<bool> CheckAreaAvailable(IPage page, string area)
        {
            try
            {
                var areaElement = page.Locator(QueryAreaOption);
                var options = await areaElement.AllInnerTextsAsync();
                return options.Any(option =>
                {
                    // Get the text of the option
                    return option.Contains(area, StringComparison.OrdinalIgnoreCase);
                });

            }
            catch (Exception)
            {
                PrintMessage.Error("Error on Check area");
                return false;
            }
        }
        private async Task<bool> SelectArea(IPage page)
        {
            try
            {
                var areaElement = page.Locator(QueryAreaOption, new());
                if(areaElement == null) { return false; }
                await areaElement.SelectOptionAsync(new SelectOptionValue { Label = AreaName }, new() { Timeout = 1000 });
                return true;
            }
            catch (Exception e)
            {
                PrintMessage.Error("Error on select area: " );
                return false;
            }
            
        }
        private  async Task<bool> SelectStore(IPage page)
        {
            try
            {
                var storeElement = page.Locator(QueryStoreOption);
                if (storeElement == null) { return false; }
                await storeElement.SelectOptionAsync(new SelectOptionValue { Index = StoreIndex }, new() { Timeout = 1000 });
                return true;
            }
            catch (Exception e)
            {

                PrintMessage.Error("Error on select store: " );
                return false;
            }
            
        }
        private Task<string> GetSiteKey(IPage page)
        {
            return page.EvaluateAsync<string>(@"() => {
                const match = document.documentElement.innerHTML.match(/'sitekey':\s*'([A-Za-z0-9_\-]+)'/);
                return match ? match[1] : null;"); // site key
        }
        private async Task<string> PostRequestReCaptchaV2(IPage page)
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, "https://anticaptcha.top/in.php");

                var jsonContent = $@"{{
                    ""key"": ""{ApiAntiCaptchaTop}"",
                    ""method"": ""userrecaptcha"",
                    ""googlekey"": ""{GetSiteKey(page)}"",
                    ""pageurl"": ""{UrlRegister}""
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
                PrintMessage.Error("Error on read reponse captcha, not correct form: " );
                return string.Empty;
            }
        }
        private async Task SolveCaptcha(IPage page,string token)
        {
            try
            {
                await page.EvaluateAsync(@"(token) => {
                    document.getElementById('g-recaptcha-response').innerHTML = token;}", token);
            }
            catch (Exception )
            {

                PrintMessage.Error("Error on set captcha reponse result to html captcha on web: " );
            }
        }
        //public async Task<bool> IsAreaOpen(IPage page)
        //{
        //    var areaElement = page.Locator(QueryAreaOption, new());
        //    var innexTexts =await areaElement.AllInnerTextsAsync();
        //    if (!innexTexts.Any())
        //    {
        //        return false;
        //    }
        //    return innexTexts.First().Contains(AreaName,StringComparison.OrdinalIgnoreCase);
        //}
        public async Task<bool> IsAreaOpen(IPage page)
        {
            try
            {
                var sourceCode =await GetSourceCode(page);
                return sourceCode.Contains(AreaName,StringComparison.OrdinalIgnoreCase);
            }
            catch (Exception)
            {
                PrintMessage.Error("Error on IsAreaOpen");
                return false;
            }
        }
        //public async Task<bool> SendHttpRequestToRegister(IPage page)
        //{
        //    try
        //    {
        //        var sourceCode = await GetSourceCode(page);

        //        var sjcToken = GetSJCToken(sourceCode);
        //        var requestVerfiToken = GetRequestVerificationToken(sourceCode);
        //        var price = GetPrice(sourceCode);
        //        var cookie = await page.Context.CookiesAsync();
        //        //post request to: tructuyen.sjc.com.vn
        //        //body payload:
        //        /***
        //         * __RequestVerificationToken: requestVerfiToken
        //         * Store: 7
        //         * Quantity: 1
        //         * Method: 1
        //         * SJCToken: sjcToken
        //         * Price: price
        //         * **/
        //        //with cookie: cookie
        //        //with user-agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/130.0.0.0 Safari/537.36 Edg/130.0.0.0
        //        await SendPostRequest(requestVerfiToken, sjcToken, price, cookie);
        //    }
        //    catch (Exception)
        //    {
        //        throw;
        //    }
        //}
        public async Task SendPostRequest(string requestVerfiToken, string sjcToken, int price, string cookie)
        {
            using var client = new HttpClient();

            // Set up the request URL
            var url = "https://tructuyen.sjc.com.vn";

            // Prepare the headers
            client.DefaultRequestHeaders.Add("User-Agent", "Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/130.0.0.0 Safari/537.36 Edg/130.0.0.0");
            client.DefaultRequestHeaders.Add("Cookie", cookie);

            // Create the POST data payload
            var postData = new Dictionary<string, string>
            {
                { "__RequestVerificationToken", requestVerfiToken },
                { "Store", "7" },
                { "Quantity", "1" },
                { "Method", "1" },
                { "SJCToken", sjcToken },
                { "Price", price.ToString() }
            };

            // Convert payload to form-urlencoded content
            var content = new FormUrlEncodedContent(postData);

            // Send the POST request
            var response = await client.PostAsync(url, content);
        }

        private string GetPrice(string sourceCode)
        {
            var match = Regex.Match(sourceCode, @"""GiaVang"":(\d+)");
            return match.Success ? match.Groups[1].Value : null;
        }
        public string GetRequestVerificationToken(string sourceCode)
        {
            var match = Regex.Match(sourceCode, @"<input[^>]*name=""__RequestVerificationToken""[^>]*value=""([^""]*)""");
            return match.Success ? match.Groups[1].Value : null;
        }
        public string GetSJCToken(string sourceCode)
        {
            var match = Regex.Match(sourceCode, @"SJCToken:\s*'([^']*)'");
            return match.Success ? match.Groups[1].Value : null;
        }
        public async Task<bool> Register(IPage page)
        {
            try
            {
                if(await SelectArea(page))
                {
                    if(await SelectStore(page))
                    {
                        var reponseBody = await PostRequestReCaptchaV2(page);
                        var token = ReadReponseCaptcha(reponseBody);
                        await SolveCaptcha(page, token);

                        await page.ClickAsync("#register_form_submit"); //#register_form_submit

                        await TakeScreenShot(page);
                        return true;
                    }
                }

                return false;
            }
            catch (Exception ex)
            {
                PrintMessage.Error($"Error on Register: {ex.Message}");
                return false;
            }
        }
    }
}
