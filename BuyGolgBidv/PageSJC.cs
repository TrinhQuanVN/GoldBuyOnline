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
        public string ApiAntiCaptchaTop { get; set; }//796b02353453441eb50179e374758059
        public string UrlRegister { get; set; }//https://tructuyen.sjc.com.vn/ 
        public string AreaName { get; set; }
        public int StoreIndex { get; set; }
        public async Task<string> GetSourceCode(IPage page)
        {
            return await page.ContentAsync();
        }

        public bool IsLoginSuccess(string soureCode)
        {
            return soureCode.Contains("id_store",StringComparison.OrdinalIgnoreCase);
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
                await Task.WhenAll(
                    page.WaitForNavigationAsync(),
                    page.ClickAsync(QueryButtonLogin)
                    );
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
   
        
        public bool IsAreaOpen(string sourceCode) => sourceCode.Contains(AreaName, StringComparison.OrdinalIgnoreCase);

        public async Task<bool> Register(string sourceCode, string cookie)
        {
            try
            {
                var sjcToken = GetSJCToken(sourceCode);
                var requestVerfiToken = GetRequestVerificationToken(sourceCode);
                var price = GetPrice(sourceCode);
                
                //post request to: tructuyen.sjc.com.vn
                //body payload:
                /***
                 * __RequestVerificationToken: requestVerfiToken
                 * Store: 7
                 * Quantity: 1
                 * Method: 1
                 * SJCToken: sjcToken
                 * Price: price
                 * **/
                //with cookie: cookie
                //with user-agent: Mozilla/5.0 (Windows NT 10.0; Win64; x64) AppleWebKit/537.36 (KHTML, like Gecko) Chrome/130.0.0.0 Safari/537.36 Edg/130.0.0.0
                var response = await SendPostRequest(requestVerfiToken, sjcToken, price, cookie);
                if (response != null && response.IsSuccessStatusCode)
                {
                    return true;
                }
                return false;
               
            }
            catch (Exception)
            {
                PrintMessage.Error("Error on posting request register");
                return false;
            }
        }
        private async Task<HttpResponseMessage> SendPostRequest(string requestVerfiToken, string sjcToken, string price, string cookie)
        {
            try
            {
                using var client = new HttpClient();

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
                { "Price", price }
            };

                // Convert payload to form-urlencoded content
                var content = new FormUrlEncodedContent(postData);

                // Send the POST request
                return await client.PostAsync(UrlRegister, content);
            }
            catch (Exception)
            {
                PrintMessage.Error("Error on send post request");
                return new HttpResponseMessage(new() { });
            }
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

        public async Task<string> GetCookie(IPage page)
        {
            try
            {
                var cookies = await page.Context.CookiesAsync();

                // Combine cookies into a single string in the format "Name1=Value1; Name2=Value2"
                return string.Join("; ", cookies.Select(c => $"{c.Name}={c.Value}"));
            }
            catch (Exception)
            {
                PrintMessage.Error("Error on get cookie");
                return string.Empty;
            }
        }

    }
}
