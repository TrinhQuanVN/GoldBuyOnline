using BuyGolgBidv.Models;
using Microsoft.Playwright;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace BuyGolgBidv.BankPages
{
    public class PageAgribank : PageBase
    {
        public PageAgribank(IPage page, BankUser bankUser,string bankName,int id) : base(page, bankUser, bankName, id)
        {
        }

        private async Task<bool> GoToAgribankUrl()
        {
            try
            {
                await _page.GotoAsync(_bankUser.UrlAgribankGold);
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                var content = await _page.ContentAsync();
                var pass = content.Contains("Chưa đến giờ", StringComparison.OrdinalIgnoreCase);
                while (pass)
                {
                    _logs.AddLog("access the web fail or web isn't opened. Reload");
                    await _page.ReloadAsync();
                    content = await _page.ContentAsync();
                    pass = content.Contains("Chưa đến giờ", StringComparison.OrdinalIgnoreCase);
                }
                return true;
            }
            catch (Exception)
            {

                return false;
            }
        }
        private async Task SubmitStep1Async(int indexChiNhanh)
        {
            try
            {
                var script = @"(indexChiNhanh)=>{
                    function triggerEvent(el, type) {
                        const event = new Event(type, { bubbles: true });
                        el.dispatchEvent(event);
                    }
                    const inputElement = document.getElementById('input-25');
                    inputElement.click();
                    const listItem = document.querySelectorAll('.v-list-item__content');
                    listItem[indexChiNhanh].click();
                    const button = document.querySelector('button[type=""button""][step=""3""].btn-main.next-step');
                    button.click();
                }";
                await _page.EvaluateAsync(script, indexChiNhanh);
            }
            catch (Exception)
            {

               AddLog("got error on submiting step 1");
            }
        }
        private async Task<bool> SolveCaptcha()
        {
            try
            {
                var img = await _page.WaitForSelectorAsync("img[data-v-772eadec]");
                if (img == null) return false;
                var source = await img.GetAttributeAsync("src");
                var base64 = source.Split(',')[1];
                var captcha = await ApiAntiCaptchaTop.SolveCaptchaIMG(_bankUser.ApiKey, base64, 32, _bankUser.PostUrlAntiCaptchaImg);
                await _page.FillAsync("input[id=input-131]", captcha);
                return true;
            }
            catch (Exception)
            {
                AddLog("got error on solve captcha");
                return false;
            }
        }
        private async Task FillSelectAgri(string id, string value)
        {
            try
            {
                var script = @"([id,value]) =>{
                    function triggerEvent(el, type) {
                        const event = new Event(type, { bubbles: true });
                        el.dispatchEvent(event);
                    }
                    const e = document.querySelector(query);
                    e.value = value;
                    triggerEvent(e, 'input');
                    triggerEvent(e, 'change');
                }";
                await _page.EvaluateAsync(script, new object[] { id, value });
            }
            catch (Exception)
            {
                AddLog("got error on fill select");
            }
        }
        private async Task CheckCheckBoxAsync()
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
                await _page.EvaluateAsync(script);
            }
            catch (Exception)
            {
                AddLog("got error on check the check box");
            }
        }
        private async Task<bool> FillFormAsync()
        {
            try
            {
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                var random = new Random();
                int indexChiNhanh = random.Next(0, 4);

                await SubmitStep1Async(indexChiNhanh);

                await _page.FillAsync("#input-96", _bankUser.FullName);
                await FillSelectAgri("#input-101", _bankUser.Birthday);

                await _page.FillAsync("#input-105", _bankUser.Address);
                await _page.FillAsync("#input-108", _bankUser.Email);
                await _page.FillAsync("#input-111", _bankUser.Phone);
                await _page.FillAsync("#input-114", _bankUser.IDNumber);
                await _page.FillAsync("#input-123", _bankUser.IssuePlace);
                await FillSelectAgri("#input-119", _bankUser.IssueDate);
                await CheckCheckBoxAsync();
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);

                await SolveCaptcha();

                await _page.ClickAsync("button[data-v-5d38e429].btn-main.next-step");
                await _page.WaitForLoadStateAsync(LoadState.NetworkIdle);
                var source = await _page.ContentAsync();

                return source.Contains("không thành công", StringComparison.OrdinalIgnoreCase);

            }
            catch (Exception)
            {

                return false;
            }
        }
        public override async Task RunAsync()
        {
            try
            {
                AddLog("access the web");
                await GoToAgribankUrl();
                AddLog("access successfull");

                AddLog("starting register buying gold");
                while (!await FillFormAsync())
                {
                    AddLog("Register fail. Retry");
                    await GoToAgribankUrl();
                    if (IsStop)
                    {
                        AddLog("stop the run");
                        return;
                    }
                }
                AddLog("REGISTER SUCCESS !!!!!!");
                var screenShotPath = $"agribank{DateTime.Now.ToString("yyyyMMddHHmmss")}.png";
                await _page.ScreenshotAsync(new() { Path = screenShotPath });
                AddLog($"Save screen shot picture to {screenShotPath}");
                
            }
            catch (Exception)
            {
                AddLog("got error on register");
            }
        }
    }
}
