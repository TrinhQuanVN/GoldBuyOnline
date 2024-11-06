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


            
            //await Task.Delay(900000);
            while (!await Run(page, pageSjc, userInfo))
            {
                PrintMessage.Information("Register fail");
            }

            Console.ReadKey();

            await browser.CloseAsync();
        }
        private static async Task<bool> Run(IPage page, PageSJCV pageSJC,UserInfo userInfo)
        {
            var folderPath = pageSJC.ScreenShotFolder();
            
            if(!await pageSJC.AccessLoginPage(page))
            {
                PrintMessage.Error("Access login page fail!");
                Console.ReadKey();
                return false;
            }
            await Task.Delay(2000);
            if(await pageSJC.ReloadAndCheckPageError404(page))
            {
                PrintMessage.Error("Page got error 404");
                return false;
            }
            

            await pageSJC.Login(page,userInfo);

            
            if (!pageSJC.IsLoginSuccess(await pageSJC.GetSourceCode(page)))
            {
                PrintMessage.Error("Login fail!");
                return false;
            }
            PrintMessage.Success("Login success!");

            var sourceCode = await pageSJC.GetSourceCode(page);
            while (!pageSJC.IsAreaOpen(sourceCode))
            {
               if(await pageSJC.ReloadAndCheckPageError404(page))
                {
                    PrintMessage.Error("Page got error 404");
                    return false;
                }
               sourceCode = await pageSJC.GetSourceCode(page);
                
                PrintMessage.Error($"{pageSJC.AreaName} isn't opened");
                //await Task.Delay(1000);
            }

            while (!await pageSJC.Register(page))
            {
                if(await pageSJC.ReloadAndCheckPageError404(page))
                {
                    PrintMessage.Error("Page error");
                    return false;
                }
            }

            await pageSJC.TakeScreenShot(page, pageSJC.CreateScreenShotPath(folderPath));
            PrintMessage.Success($"Register succesfull");
            return true;
        }
        private static void ShowTimeNow()
        {
            PrintMessage.Information($"Time now is {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
        }
        public static async Task ScheduleTaskAt9AM(Action task)
        {
            // Calculate the time until the next 9:00 AM
            DateTime now = DateTime.Now;
            DateTime next9AM = now.Date.AddHours(9);

            // If it's already past 9:00 AM, schedule for the next day
            if (now > next9AM)
            {
                next9AM = next9AM.AddDays(1);
            }

            TimeSpan delay = next9AM - now;

            // Wait until 9:00 AM
            await Task.Delay(delay);

            // Execute the task
            task();
        }
    }
}
