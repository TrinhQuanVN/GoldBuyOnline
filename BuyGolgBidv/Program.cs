using Microsoft.Playwright;
using Microsoft.VisualBasic;
using System.Text;
using System.Text.Json;

namespace BuyGolgBidv
{
    internal class Program
    {
        private static string _dataPath = Path.Combine(AppDomain.CurrentDomain.BaseDirectory,"data.xml");
        private static DataBaseXML _data = new();

        static async Task Main(string[] args)
        {
            Console.InputEncoding = Encoding.UTF8;
            Console.OutputEncoding = Encoding.UTF8;
            if (!Path.Exists(_dataPath))
            {
                PrintMessage.Error("data.xml can not find!");
                return;
            }

            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Chromium.LaunchAsync(new() { Headless=false});
            var content = browser.NewContextAsync();
            var page = await browser.NewPageAsync();

            await _data.LoadFromXmlAsync(_dataPath);
            var pageSjc = _data.PageSJC;
            var userInfo = _data.UserInfo;

            ShowTimeNow();
            PrintMessage.Information($"Hello {userInfo.Name}");
            await Run(page, pageSjc, userInfo);

            Console.ReadKey();

            await browser.CloseAsync();
        }
        private static async Task Run(IPage page, PageSJCV pageSJC,UserInfo userInfo)
        {
            var folderPath = pageSJC.ScreenShotFolder();
            
            if(!await pageSJC.AccessLoginPage(page))
            {
                PrintMessage.Error("Access login page fail!");
                Console.ReadKey();
                return;
            }
            await Task.Delay(2000);
            if(await pageSJC.ReloadAndCheckPageError404(page))
            {
                PrintMessage.Error("Page got error 404");
                return;
            }
            

            await pageSJC.Login(page,userInfo);

            
            if (!pageSJC.IsLoginSuccess(await pageSJC.GetSourceCode(page)))
            {
                PrintMessage.Error("Login fail!");
                return;
            }
            PrintMessage.Success("Login success!");

            var sourceCode = await pageSJC.GetSourceCode(page);
            while (!pageSJC.IsAreaOpen(sourceCode))
            {
               if(await pageSJC.ReloadAndCheckPageError404(page))
                {
                    PrintMessage.Error("Page got error 404");
                    return;
                }
               sourceCode = await pageSJC.GetSourceCode(page);
                
                PrintMessage.Error($"{pageSJC.AreaName} isn't opened");
                await Task.Delay(1000);
            }
            while (!await pageSJC.Register(page))
            {
                PrintMessage.Information("Register fail");

                await Task.Delay(1000);

                if(await pageSJC.ReloadAndCheckPageError404(page))
                {
                    PrintMessage.Error("Error 404");
                    return;
                }
            }
            await pageSJC.TakeScreenShot(page, pageSJC.CreateScreenShotPath(folderPath));
            PrintMessage.Success($"Register succesfull");
        }
        private static void ShowTimeNow()
        {
            PrintMessage.Information($"Time now is {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
        }
    }
}
