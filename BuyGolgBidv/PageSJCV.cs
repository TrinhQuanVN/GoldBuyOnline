using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace BuyGolgBidv
{
    public class PageSJCV
    {
        public string QueryNameLogin { get; set; }
        public string QueryCccdLogin { get; set; }
        public string QueryAreaOption { get; set; }
        public string QueryStoreOption { get; set; }
        public string QueryButtonLogin { get; set; }
        public string QueryButtonRegister { get; set; }
        public string UrlLogin { get; set; }
        public string ApiAntiCaptchaTop { get; set; }
        public string UrlRegister { get; set; }
        public string AreaName { get; set; }
        public int StoreIndex { get; set; }
        public async Task<string> GetSourceCode(IPage page)
        {
            var a = await page.ContentAsync();
            return await page.ContentAsync();
        }

        public bool IsLoginSuccess(string sourceCode)
        {
            return sourceCode.Contains("id_store", StringComparison.OrdinalIgnoreCase);
        }
        public string ScreenShotFolder()
        {
            string desktopPath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Desktop), "SJC_ScreenShot"); // Get desktop path
            Directory.CreateDirectory(desktopPath);
            return desktopPath;
        }

        public async Task<bool> ReloadAndCheckPageError404(IPage page)
        {
            await page.ReloadAsync();
            return Is404(await page.ContentAsync());
        }
        public Task<int> StoreCount(IPage page)
        {
            var storeElement = page.Locator(QueryStoreOption);
            return storeElement.CountAsync();
        }
        public async Task ChangeStoreIndex(IPage page)
        {
            var storeCount = await StoreCount(page);
            if (StoreIndex < storeCount)
            {
                StoreIndex++;
            }
            else
            {
                StoreIndex = 1;
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

                PrintMessage.Error($"Error on access login page");
                return false;
            }
        }
        public async Task<bool> Login(IPage page, UserInfo userInfo)
        {
            try
            {
                await page.FillAsync(QueryNameLogin, userInfo.Name);
                await page.FillAsync(QueryCccdLogin, userInfo.CCCD);


                await page.ClickAsync(QueryButtonLogin);
                await page.WaitForURLAsync(UrlRegister);

                return true;

            }
            catch (Exception)
            {

                PrintMessage.Error($"Error on Login");
                return false;
            }
        }
        public string CreateScreenShotPath(string folderPath)
        {
            string fileName = $"SJC_{DateTime.Now.ToString("yyyy-MM-dd_HH-mm-ss")}.png"; // Name format

            return Path.Combine(folderPath, fileName);
        }
        public async Task TakeScreenShot(IPage page, string path)
        {
            try
            {
                
                await Task.Delay(1000);
                await page.ScreenshotAsync(new()
                {
                    Path = path,
                    FullPage = true,
                });
                
            }
            catch (Exception)
            {
                PrintMessage.Error($"Take screen shot fail");
            }
        }
        
        private async Task<bool> SelectArea(IPage page)
        {
            try
            {
                var areaElement = page.Locator(QueryAreaOption, new());
                if (areaElement == null) { return false; }
                await areaElement.SelectOptionAsync(new SelectOptionValue { Label = AreaName },new() { Timeout=2000});
                return true;
            }
            catch (Exception e)
            {
                PrintMessage.Error("Error on select area: ");
                return false;
            }

        }
        private async Task<bool> SelectStore(IPage page)
        {
            try
            {
                var storeElement = page.Locator(QueryStoreOption);
                if (storeElement == null) { return false; }
                await storeElement.SelectOptionAsync(new SelectOptionValue { Index = StoreIndex }, new() { Timeout = 2000 });
                return true;
            }
            catch (Exception)
            {

                PrintMessage.Error("Error on select store: ");
                return false;
            }

        }
        public bool Is404(string sourceCode)
        {
            return sourceCode.Contains("404",StringComparison.OrdinalIgnoreCase) || sourceCode.Contains("401", StringComparison.OrdinalIgnoreCase);
        }
        public bool IsPageNotOpenedForSell(string sourceCode)
        {
            return sourceCode.Contains("chưa mở bán", StringComparison.OrdinalIgnoreCase);
        }
        public bool IsAreaOpen(string sourceCode) => sourceCode.Contains(AreaName, StringComparison.OrdinalIgnoreCase);
        public async Task<bool> Register(IPage page)
        {
            try
            {
                await SelectArea(page);
                if(await SelectStore(page)){

                    Random random = new Random();
                    int[] options = { 1, 2, 3, 4 };
                    int randomNumber = options[random.Next(options.Length)];
                    StoreIndex = randomNumber;

                    var api = new ApiAntiCaptchaTop(ApiAntiCaptchaTop, await GetSourceCode(page), UrlRegister);
                    var token =await api.SolveCaptcha(page);

                    await page.EvaluateAsync(@"(token) => {
                            document.getElementById('g-recaptcha-response').innerHTML = token;}", token);

                    await page.ClickAsync("#register_form_submit");
                    await page.WaitForURLAsync(UrlRegister);

                    return IsRegisterSuccess(await page.ContentAsync());
                }
                return false;

            }
            catch (Exception)
            {
                PrintMessage.Error($"Error on Register");
                return false;
            }
        }
        public bool IsRegisterSuccess(string sourceCode) => sourceCode.Contains("Phiếu đăng ký mua vàng", StringComparison.OrdinalIgnoreCase);
    }
}
