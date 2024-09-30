// ==UserScript==
// @name         Intercept RegGGSev Request
// @namespace    http://tampermonkey.net/
// @version      1.0
// @description  Modify the response for RegGGSev requests
// @author       Your Name
// @match        https://ebank.bidv.com.vn/DKNHDT/muavang.htm
// @grant        none
// ==/UserScript==

(function() {
    'use strict';

    const originalOpen = XMLHttpRequest.prototype.open;

    XMLHttpRequest.prototype.open = function(method, url) {
        if (url.includes('RegGGSev')) { // Check if the request is to RegGGSev
            this.addEventListener('readystatechange', function() {
                if (this.readyState === 4) {
                    try {
                        console.log('Intercepting request to: ' + url);

                        Object.defineProperty(this, 'response', { writable: true });
                        Object.defineProperty(this, 'responseText', { writable: true });

                        // Modify the response
                        this.response = this.responseText = JSON.stringify({
                            Code: "SUCCESS",
                            ExInfo: "NO"
                        });

                        console.log('Response modified to: ' + this.responseText);
                    } catch (e) {
                        console.error('Error modifying response:', e);
                    }
                }
            });
        }

        return originalOpen.apply(this, arguments);
    };

    console.log('end part 1');
    //next part
    console.log('part2');
    let person = {
  fullName: 'Hà Minh Anh',
  idNumber: '037301005728',
  issuePlace: 'cục cảnh sát qlhc về ttxh',
  issueDate: '13/08/2021',
  address: 'Tổ 4, Hai Bà Trưng, Phủ Lý, Hà Nam',
  phone: '0912155638',
  birthday: '11/04/2001',
  email: 'haminhanh11042001@gmail.com',// haminhanh2001
  vietinSexIndex: 1,
  vietinLoaiCC: 1,
  vietinNoiGiaoDich: 1,
  bidvAccNum: 8882831699, //tài khoản bid
  bidvAmount: 1,
  bidvBranch: 1, //dia điểm giao dịch
  bidvCapital: 1,
  bidvPurpose: 2,
  bidvIssuePlace: 0,
}

    async function main() {
      await fillInput('input[name=addREss2]', person.address);// address2
      await fillInput('input[name=addREss1]', person.address);// address1
      await fillInput('input[name=aMOunt]', person.bidvAmount);// amount
      await fillInput('input[name=accTNum]', person.bidvAccNum);// account number
      await fillInput('input[name=cellPHone]', person.phone);// phone number
      await fillInput('input[name=idNUmber]', person.idNumber);// ID number
      await fillInput('input[name=fullNAme]', person.fullName);// full name
      await fillInput('input[name=isSUedate]', person.issueDate);// issue date
      await fillInput('input[name=birthDAy]', person.birthday);// birthday

      await clickButtonWithDelay('button[id=btnYes]',10);// Click 'Yes' button after a delay

      await selectSelector('select[name=braNCh]', person.bidvBranch);// branch
      await selectSelector('select[name=caPItal]', person.bidvCapital); // capital
      await selectSelector('select[name=puRPose]', person.bidvPurpose); // purpose
      await selectSelector('select[name=isSUeplace]', person.bidvIssuePlace); // issue place

      await clickButtonWithDelay('ins',10); // Click 'ins' element after a delay

      window.scrollTo(0, document.body.scrollHeight); // scroll to bottom
    }

    function triggerEvent(el, type) {
      const event = new Event(type, { bubbles: true });
      el.dispatchEvent(event);
    }

    function fillInput(query, value, trigger = 0) {
      return new Promise((resolve) => {
        setTimeout(() => {
          const e = document.querySelector(query);
          e.value = value;
          triggerEvent(e, 'input');
          if (trigger > 0) {
            triggerEvent(e, 'change');
          }
          resolve();
        }, 10);
      });
    }

    function clickButtonWithDelay(selector, delay = 100) {
      return new Promise((resolve) => {
        setTimeout(() => {
          const button = document.querySelector(selector);
          if (button) {
            button.click();
          }
          resolve();
        }, delay);
      });
    }
    function selectSelector(query,value){
        const e = document.querySelector(query);
        e.selectedIndex = value;
        triggerEvent(e, 'input');
        triggerEvent(e, 'change');
    }

    main();
    console.log('end part 2');
})();
