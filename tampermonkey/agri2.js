// ==UserScript==
// @name         Agribank
// @namespace    http://tampermonkey.net/
// @version      2024-10-23
// @description  try to take over the world!
// @author       You
// @match        https://bookingonline.agribank.com.vn/muavangSJCtructuyen
// @icon         data:image/gif;base64,R0lGODlhAQABAAAAACH5BAEKAAEALAAAAAABAAEAAAICTAEAOw==
// @grant        none
// ==/UserScript==

(function() {
    'use strict';

    let person = {
    fullName: 'trịnh tiến quân',
    idNumber: '035092013752',
    issuePlace: 'cục cảnh sát qlhc về ttxh',
    issueDate: '25/08/2021',
    address: 'Lương Khánh Thiện, Phủ Lý, hà nam',
    phone: '0962130922',
    birthday: '28/10/1992',
    email: 'trinhquanhn1992@gmail.com',
    vietinSexIndex: 2,
    vietinLoaiCC: 1,
    vietinNoiGiaoDich: 1,
    bidvAccNum: 4821813240, //tài khoản bid
    bidvAmount: 1,
    bidvBranch: 1, //dia điểm giao dịch
    bidvCapital: 1,
    bidvPurpose: 2,
    bidvIssuePlace: 0,
  }
      async function main() {
    await performClickActions();

    // Start solving the CAPTCHA and filling the inputs along with checkbox checking in parallel
    const base64Img = getBase64Image();
    
    const solveCaptchaPromise = solveCaptcha(base64Img);  // solving CAPTCHA
    const fillInputsPromise = (async () => {
        await fillInput('input[id=input-96]', person.fullName);
        await fillInput('input[id=input-101]', person.birthday, 1);
        await fillInput('input[id=input-105]', person.address);
        await fillInput('input[id=input-108]', person.email);
        await fillInput('input[id=input-111]', person.phone);
        await fillInput('input[id=input-114]', person.idNumber);
        await fillInput('input[id=input-123]', person.issuePlace);
        await fillInput('input[id=input-119]', person.issueDate);

        // Check the checkbox concurrently with filling inputs
        await checkCheckbox();
    })();

    // Wait for both CAPTCHA solving and input filling with checkbox checking to finish
    const [captchaText] = await Promise.all([solveCaptchaPromise, fillInputsPromise]);

    // Fill the CAPTCHA input
    await fillInput('input[id=input-131]', captchaText);

    // Click the button
    await clickButton('button[data-v-5d38e429].btn-main.next-step');
}
      function triggerEvent(el, type) {
        const event = new Event(type, { bubbles: true });
        el.dispatchEvent(event);
      }
        function clickButton(query) {
            return new Promise(resolve => {
                setTimeout(() => {
                    const button = document.querySelector(query);
                    if (button) {
                        button.click();
                    }
                    resolve();
                }, 100);
            });
        }
      function checkCheckbox() {
        return new Promise((resolve, reject) => {
          const checkbox = document.querySelector('input[type=checkbox]');
          if (checkbox) {
            checkbox.checked = true;
            triggerEvent(checkbox, 'change');
            resolve('Checkbox checked and change event triggered');
          } else {
            reject('Checkbox not found');
          }
        });
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


      function performClickActions(indexChiNhanh=0) {
        return new Promise((resolve, reject) => {
          let inputElement;
          let timer = setInterval(function () {
            inputElement = document.getElementById('input-25');

            if (inputElement) {
              inputElement.click();
              const listItem = document.querySelectorAll('.v-list-item__content');
              if (listItem) {
                  listItem[indexChiNhanh].click();
              } else {
                reject('List item not found');
                clearInterval(timer);
                return;
              }
              let button;
              let timer1 = setInterval(function () {
                button = document.querySelector('button[type="button"][step="3"].btn-main.next-step');
                if (button) {
                  button.click();
                  clearInterval(timer1);
                  resolve('Actions performed successfully');
                }
              }, 100);
              clearInterval(timer);
            }
          }, 100);
        });
      }
function getBase64Image() {
    const img = document.querySelector('img[data-v-772eadec]');
    if (img && img.src.includes(',')) {
        const base64Img = img.src.split(',')[1];
        return base64Img;
    } else {
        throw new Error("Image not found or invalid source.");
    }
}


async function solveCaptcha(base64Img) {
    const apiUrl = "https://anticaptcha.top/api/captcha";
    const payload = {
        apikey: "796b02353453441eb50179e374758059", // Replace with your actual API key
        img: base64Img,
        type: 32
    };

    try {
        const response = await fetch(apiUrl, {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(payload)
        });

        const data = await response.json();
        if (data.success) {
            return data.captcha; // Return the solved captcha text
        } else {
            throw new Error("Captcha solving failed: " + data.message);
        }
    } catch (error) {
        console.error("Error:", error);
    }
}
      main();
      
})();
