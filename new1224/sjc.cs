//dotnet add package Microsoft.Playwright
using Microsoft.Playwright;
using System.Text;
using System.Text.RegularExpressions;

namespace sjcgold
{
    class User
    {
        public User(string fullName, string adress, string email, string phone, string idNumber, string idPlace, string birthday, string idDate)
        {
            FullName = fullName;
            Adress = adress;
            Email = email;
            Phone = phone;
            IdNumber = idNumber;
            IdPlace = idPlace;
            Birthday = birthday;
            IdDate = idDate;
        }

        public string FullName { get; set; }
        public string Adress { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string IdNumber { get; set; }
        public string IdPlace { get; set; }
        public string Birthday { get; set; }
        public string IdDate { get; set; }
    }
    class Program
    {
        static readonly string _sjcPageUrl = "https://tructuyen.sjc.com.vn/";
        static string _sjcLoginPageUrl = "https://tructuyen.sjc.com.vn/dang-nhap";
        readonly static string _apiKey = "796b02353453441eb50179e374758059";
        readonly static string _urlApi = "https://anticaptcha.top/in.php";
        static async Task Main(string[] args)
        {
            Console.OutputEncoding = System.Text.Encoding.UTF8;

            Console.WriteLine("Enter 0 if Headless false or enter anything");

            var input = Console.ReadLine();
            var headless = true;
            if (!string.IsNullOrEmpty(input))
            {
                if (input == "0")
                {
                    headless = false;
                }
            }

            var playwright = await Playwright.CreateAsync();
            var browser = await playwright.Firefox.LaunchAsync(new() { Headless = headless });
            var content = browser.NewContextAsync();
            var page = await browser.NewPageAsync();

            var user = GetUserFromTextFile();
            Console.WriteLine($"Register sjc for {user.Result.FullName} " + $"id : {user.Result.IdNumber}");
            Console.WriteLine("enter any key to run");
            Console.ReadKey();
            await GoPage(page, _sjcLoginPageUrl);
            await Login(page, user.Result.FullName, user.Result.IdNumber);
            await Register(page, _apiKey);
            Console.ReadKey();
        }
        static string ExtractValue(string content, string key)
        {
            // Find the key in the content
            var startIndex = content.IndexOf(key, StringComparison.OrdinalIgnoreCase);
            if (startIndex == -1)
            {
                return string.Empty;
            }

            // Move the index to the value part
            startIndex = content.IndexOf(":", startIndex) + 1;

            // Find the end of the value (either ',' or '}')
            var endIndex = content.IndexOf(',', startIndex);
            if (endIndex == -1)
            {
                endIndex = content.IndexOf('}', startIndex);
            }

            if (endIndex == -1)
            {
                return string.Empty;
            }

            // Extract and clean the value
            var value = content.Substring(startIndex, endIndex - startIndex).Trim();
            return value.Trim('\'', '"', ' ');
        }

        static async Task<User?> GetUserFromTextFile(string txtPath = "user.txt")
        {
            try
            {
                var content = await File.ReadAllTextAsync(txtPath);

                // Extract properties using string comparison
                string fullName = ExtractValue(content, "fullName");
                string address = ExtractValue(content, "address");
                string email = ExtractValue(content, "email");
                string phone = ExtractValue(content, "phone");
                string idNumber = ExtractValue(content, "idNumber");
                string idPlace = ExtractValue(content, "issuePlace");
                string birthday = ExtractValue(content, "birthday");
                string idDate = ExtractValue(content, "issueDate");

                // Create a new User instance
                return new User(fullName, address, email, phone, idNumber, idPlace, birthday, idDate);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return null;

            }
        }
        static async Task GoPage(IPage page, string url)
        {
            await page.GotoAsync(url);
            await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
            Console.WriteLine($"go to {url}");
        }
        static async Task Login(IPage page, string fullName, string idNumber)
        {
            try
            {
                var task1 = page.EvaluateAsync<string>(@"
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

                var task2 = page.EvaluateAsync<string>(@"
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
                await Task.WhenAll([task1, task2]);

                await page.ClickAsync("#sign_in_submit");
                await page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                Console.WriteLine("login sucess");
            }
            catch (Exception)
            {
                Console.WriteLine("error on login");
            }

        }
        static async Task Register(IPage page, string apiKey)
        {
            try
            {
                while (!IsAreaOpen(await page.ContentAsync()))
                {
                    Console.WriteLine("area not open try reload");
                    await Task.Delay(1000);
                    await page.ReloadAsync();
                }
                while (true)
                {
                    await RegisterSingleAsync(page);
                    Console.WriteLine("try register again");
                    await Task.Delay(1000);
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
                //await page.WaitForURLAsync(_sjcPageUrl);

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
                var random = new Random();  
                var index = random.Next(1,4);
                await page.EvaluateAsync<string>(@"
        (index) => {
            function triggerEvent(el, type) {
                const event = new Event(type, { bubbles: true });
                el.dispatchEvent(event);
            }

            const store = document.querySelectorAll('#id_store')[0];
            const selectedStore = store[index];
            selectedStore.selected = true;
            triggerEvent(selectedStore, 'change');
            triggerEvent(selectedStore, 'input');
        }",index);
                Console.WriteLine($"select store {index}");
            }
            catch (Exception)
            {
                Console.WriteLine("error on select store");
            }

        }
        static async Task SolveCaptchaAsync(IPage page)
        {
            try
            {
                var sourceCode = await page.ContentAsync();
                var token = await GetTokenAsync(sourceCode);
                await page.EvaluateAsync(@"(token) => {
                document.getElementById('g-recaptcha-response').innerHTML = token;}", token);
                Console.WriteLine("solve captcha success");

            }
            catch (Exception)
            {
                Console.WriteLine("error on solve captcha");
            }
        }

        static async Task<string> GetTokenAsync(string sourceCode)
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

        static object GetSiteKey(string sourceCode)
        {
            var match = Regex.Match(sourceCode, "/ 'sitekey':\\s * '([A-Za-z0-9_\\-]+)' /");
            return match.Success ? match.Groups[1].Value : string.Empty;
        }
        }
    }
