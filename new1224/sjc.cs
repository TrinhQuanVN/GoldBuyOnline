//dotnet add package Microsoft.Playwright
using Microsoft.Playwright;

namespace BuySJC{
     class Program
 {
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
         }",  fullName);

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
         await page.WaitForNavigationAsync(new()
         {
             Url = "https://tructuyen.sjc.com.vn/",
             WaitUntil = WaitUntilState.Load,
         });
     }
     static async Task Register(IPage page, string apiKey)
     {
         while (IsAreaOpen(await page.ContentAsync()))
         {
             await page.ReloadAsync();
         }
         await SelectArea(page);
         await SelectStore(page);
         //await SelectHinhThuc(page, index);
         //await SolveCaptcha(_apiKey);
         //await page.ClickAsync("#register_form_submit");

     }
     static bool IsAreaOpen(string sourceCode)
     {
         return sourceCode.Contains("hà nội", StringComparison.OrdinalIgnoreCase);
     }
     static async Task SelectArea(IPage page)
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
     static async Task SelectStore(IPage page)
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
 }
}
