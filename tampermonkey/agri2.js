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
        const jsonData = await listenAndCaptureRequest(); // Step 1: Capture the request
        const base64Image = handleJsonAndExtractImage(jsonData); // Step 2: Handle JSON and extract base64 image
        const captchaText = await solveCaptcha(base64Image); // Step 3: Solve the CAPTCHA

        await fillInput('input[id=input-96]', person.fullName);
        await fillInput('input[id=input-101]', person.birthday,1);
        await fillInput('input[id=input-105]', person.address);
        await fillInput('input[id=input-108]', person.email);
        await fillInput('input[id=input-111]', person.phone);
        await fillInput('input[id=input-114]', person.idNumber);
        await fillInput('input[id=input-123]', person.issuePlace);
        await fillInput('input[id=input-119]', person.issueDate);
        
        await fillInput('input[id=input-119]', captchaText);//captcha

        await checkCheckbox();

        ClickButtonSumit();

      }
      function triggerEvent(el, type) {
        const event = new Event(type, { bubbles: true });
        el.dispatchEvent(event);
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
async function listenAndCaptureRequest() {
    return new Promise((resolve, reject) => {
        // Intercept fetch or XMLHttpRequest (based on the website's request method)
        const originalFetch = window.fetch;
        window.fetch = async function (...args) {
            const response = await originalFetch.apply(this, args);
            
            const url = args[0];
            if (url.includes('https://j8ki3b991l.execute-api.ap-southeast-1.amazonaws.com/production/generate-captcha')) {
                const clonedResponse = response.clone(); // Clone response to read it multiple times
                clonedResponse.json().then(data => {
                    if (response.status === 200 && data.image && data.id) {
                        resolve(data); // Return the JSON body if status is 200
                    } else {
                        reject('Invalid response data');
                    }
                }).catch(err => reject(err));
            }
            
            return response;
        };
    });
}

// 2. Function to handle the JSON and extract the base64 image
function handleJsonAndExtractImage(jsonData) {
    const base64Image = jsonData.image.split(",")[1]; // Extract only the base64 part
    return base64Image; // Return the base64 string of the image
}

// 3. Function to solve the CAPTCHA using the base64 image
async function solveCaptcha(base64Image) {
    const apiUrl = 'https://anticaptcha.top/api/captcha';
    const apiKey = 'YOUR_API_KEY'; // Replace with your API key

    const body = {
        "apikey": apiKey,
        "img": base64Image,
        "type": 32
    };

    const response = await fetch(apiUrl, {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(body)
    });

    const result = await response.json();

    if (result.success) {
        return result.captcha; // Return the CAPTCHA result (e.g., '764NHK')
    } else {
        throw new Error('Captcha solving failed');
    }
}


  
      main();
      
})();
