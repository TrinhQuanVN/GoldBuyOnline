using Microsoft.Playwright;
using System.Text;
using System.Text.RegularExpressions;

namespace Agrigold
{
    class User
    {
        public User(string fullName, string adress, string email, string phone, string idNumber, string birthday, string idDate)
        {
            FullName = fullName;
            Adress = adress;
            Email = email;
            Phone = phone;
            IdNumber = idNumber;
            Birthday = birthday;
            IdDate = idDate;
        }

        public string FullName { get; set; }
        public string Adress { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string IdNumber { get; set; }
        public string IdPlace = "CCSQLHCVTTXH";
        public string Birthday { get; set; }
        public string IdDate { get; set; }

    }
    internal class Program
    {
        static readonly string _agriPageUrl = "https://bookingonline.agribank.com.vn/muavangSJCtructuyen";
        readonly static string _apiKey = "796b02353453441eb50179e374758059";
        readonly static string _urlApi = "https://anticaptcha.top/api/captcha";
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.Unicode;
            Console.InputEncoding = System.Text.Encoding.Unicode;

            Console.Write("type 0 to show browser: ");
            var input = Console.ReadKey();
            var headless = true;
            if (input.Key == ConsoleKey.D0 || input.Key == ConsoleKey.NumPad0)
            {
                headless = false;
            }
            Console.WriteLine();
            var user = GetUserInput() ?? throw new Exception("null user input");
            ShowUserInfoToUser("Register agri for:", user);
            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Firefox.LaunchAsync(new() { Headless = headless });
            var content = browser.NewContextAsync();
            var page = await browser.NewPageAsync();

            
            Console.WriteLine("press any key to run");
            Console.ReadKey();
            await RunAsync(page, user);

        }
        private static void ShowUserInfoToUser(string message, User user)
        {
            Console.WriteLine(message);
            Console.WriteLine($"{user.FullName}");
            Console.WriteLine($"{user.Adress}");
            Console.WriteLine($"{user.Email}");
            Console.WriteLine($"{user.Phone}");
            Console.WriteLine($"{user.IdNumber}");
            Console.WriteLine($"{user.Birthday}");
            Console.WriteLine($"{user.IdDate}");
        }
        private static User? GetUserInput()
        {
            try
            {
                Console.WriteLine("type user info <full name> & <adress> & <email> & <phone> & <id number> & <birth day> & <id date>");
                var input = Console.ReadLine() ?? throw new Exception("input can not empty");
                string[] values = input.Split("&");
                if (values.Length != 7) throw new Exception("your input need contain 7 values");
                return new User(
                    values[0].Trim(),
                    values[1].Trim(),
                    values[2].Trim(),
                    values[3].Trim(),
                    values[4].Trim(),
                    values[5].Trim(),
                    values[6].Trim()
                    );

            }
            catch (Exception e)
            {
                Console.WriteLine(e.Message);
                return null;
            }
        }

        private static async Task RunAsync(IPage page, User user)
        {
            while (true)
            {
                await GotoAgriPage(page, _agriPageUrl);
                await FillFormAsync(page, user);
                Console.WriteLine("try register again!");
                await page.ReloadAsync();
            }
        }

        private static async Task GotoAgriPage(IPage page, string agriPageUrl)
        {
            try
            {
                await page.GotoAsync(agriPageUrl);
                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                var content = await page.ContentAsync();
                var pass = content.Contains("chưa đến giờ", StringComparison.OrdinalIgnoreCase);
                while (pass)
                {
                    Console.WriteLine("chưa đến giờ. reload");
                    await page.ReloadAsync();
                    content = await page.ContentAsync();
                    pass = content.Contains("chưa đến giờ", StringComparison.OrdinalIgnoreCase);
                }
            }
            catch (Exception)
            {

            }
        }

        private static async Task Step1Async(IPage page, int indexChiNhanh)
        {
            try
            {
                var script1 = @"(indexChiNhanh)=>{
                    function triggerEvent(el, type) {
                        const event = new Event(type, { bubbles: true });
                        el.dispatchEvent(event);
                    }
                    const inputElement = document.getElementById('input-25');
                    inputElement.click();
                }";//.v-list-item__content
                var script2 = @"(indexChiNhanh)=>{
                    function triggerEvent(el, type) {
                        const event = new Event(type, { bubbles: true });
                        el.dispatchEvent(event);
                    }
                    const listItem = document.querySelectorAll('.v-list-item__content');
                    listItem[1].click();
                }";//
                var script3 = @"(indexChiNhanh)=>{
                    function triggerEvent(el, type) {
                        const event = new Event(type, { bubbles: true });
                        el.dispatchEvent(event);
                    }
                    const button = document.querySelector('button[type=""button""][step=""3""].btn-main.next-step');
                    button.click();
                }";//.v-list-item__content
                await page.EvaluateAsync(script1, indexChiNhanh);
                var a = await page.EvaluateAsync(script2, indexChiNhanh);
                await page.EvaluateAsync(script3, indexChiNhanh);
                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                Console.WriteLine($"selected chi nhanh {indexChiNhanh}");

            }
            catch (Exception)
            {
            }
        }
        private static async Task SolveCaptcha(IPage page)
        {
            try
            {
                var img = await page.WaitForSelectorAsync("img[data-v-772eadec]");
                if (img == null) return;
                var source = await img.GetAttributeAsync("src");
                var base64 = source.Split(',')[1];
                var response = await PostRequestAsync(base64);
                var captcha = ReadResponseCaptcha(response);
                await page.FillAsync("input[id=input-131]", captcha);
                Console.WriteLine($"solve captcha: {captcha}");
            }
            catch (Exception)
            {
            }
        }
        private static async Task FillSelectAgri(IPage page, string id, string value)
        {
            try
            {
                var script = @"([id,value]) =>{
                    function triggerEvent(el, type) {
                        const event = new Event(type, { bubbles: true });
                        el.dispatchEvent(event);
                    }
                    const e = document.querySelector(id);
                    e.value = value;
                    triggerEvent(e, 'input');
                    triggerEvent(e, 'change');
                }";
                await page.EvaluateAsync(script, new object[] { id, value });
            }
            catch (Exception)
            {
            }
        }
        private static async Task CheckCheckBoxAsync(IPage page)
        {
            try
            {
                var script = @"()=>{
                    function triggerEvent(el, type) {
                        const event = new Event(type, { bubbles: true });
                        el.dispatchEvent(event);
                    }
                    const checkbox = document.querySelector('input[type=checkbox]');
                    checkbox.checked = true;
                    triggerEvent(checkbox, 'change');
                }";
                await page.EvaluateAsync(script);
            }
            catch (Exception)
            {
            }
        }
        private static async Task FillFormAsync(IPage page, User user)
        {
            try
            {
                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                var random = new Random();
                int indexChiNhanh = random.Next(0, 4);

                await Step1Async(page, indexChiNhanh);

                await page.FillAsync("#input-96", user.FullName);
                await FillSelectAgri(page, "#input-101", user.Birthday);

                await page.FillAsync("#input-105", user.Adress);
                await page.FillAsync("#input-108", user.Email);
                await page.FillAsync("#input-111", user.Phone);
                await page.FillAsync("#input-114", user.IdNumber);
                await page.FillAsync("#input-123", user.IdPlace);
                await FillSelectAgri(page, "#input-119", user.IdDate);
                await CheckCheckBoxAsync(page);
                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);

                await SolveCaptcha(page);

                await page.ClickAsync("button[data-v-5d38e429].btn-main.next-step");
                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                Console.WriteLine("fill form success");
            }
            catch (Exception)
            {
            }
        }
        private static async Task<string> PostRequestAsync(string imageBase64)
        {
            try
            {
                var client = new HttpClient();
                var request = new HttpRequestMessage(HttpMethod.Post, _urlApi);
                var jsonContent = $@"{{
                ""apikey"": ""{_apiKey}"",
                ""img"" : ""{imageBase64}"",
                ""type"" : ""{32}""
                }}";
                request.Content = new StringContent(jsonContent, Encoding.UTF8, "application/json");
                var response = await client.SendAsync(request);
                response.EnsureSuccessStatusCode();
                return await response.Content.ReadAsStringAsync();
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }
        private static string ReadResponseCaptcha(string response)
        {
            string pattern = "\"captcha\":\"([^\"]+)\"";

            // Use Regex.Match to find the first match
            Match match = Regex.Match(response, pattern);
            return match.Groups[1].Value;
        }
    }
}
