using Microsoft.Playwright;
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

            if(await pageSjc.AccessLoginPage(page))
            {
                PrintMessage.Success("access success");
            }
                
            await Task.Delay(1000);
            await pageSjc.Reload(page);
            await Task.Delay(2000);

            await pageSjc.Login(page, userInfo);
            

            //while(!await pageSjc.IsLoginSuccess(page))
            //{
            //    await Task.Delay(2000);
            //    await pageSjc.Reload(page);
            //    await pageSjc.Login(page, userInfo);
            //}

            PrintMessage.Success("login success");

            var counter = GetTimeRegister();
            //check if area open
            var count = 0;
            while (!await pageSjc.IsAreaOpen(page))
            {
                
                await pageSjc.Reload(page);
                PrintMessage.Information(pageSjc.AreaName + $" is not opened x {count}");

                count++;
            }

            //register gold
            while (counter > 0)
            {
                await pageSjc.Reload(page);
                await pageSjc.ChangeStoreIndex(page);

                await pageSjc.Register(page);

                PrintMessage.Information($"register buying sjc time x {counter}");
                counter--;
            }
            PrintMessage.Information($"Screen shoot is save to {pageSjc.ScreenShotFolder()}");

            Console.ReadKey();

            await browser.CloseAsync();
        }
        private static int GetTimeRegister()
        {
            PrintMessage.Information("enter number of register time: ");
            var counter = 0;
            var input = Console.ReadLine();
            while (!int.TryParse(input, out counter))
            {
                PrintMessage.Error("number not correct!. Try again");
                input = Console.ReadLine();
            }
            return counter;
        }
        private static void ShowTimeNow()
        {
            PrintMessage.Information($"Time now is {DateTime.Now.ToString("dd/MM/yyyy HH:mm:ss")}");
        }
        static void RemoveLastConsoleLine()
        {
            if (Console.CursorTop > 0)
            {
                // Move the cursor up by one line
                Console.SetCursorPosition(0, Console.CursorTop - 1);

                // Overwrite the line with spaces to clear it
                Console.Write(new string(' ', Console.WindowWidth));

                // Move the cursor back to the start of that line
                Console.SetCursorPosition(0, Console.CursorTop - 1);
            }
        }
    }
}
