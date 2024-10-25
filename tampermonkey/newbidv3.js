// ==UserScript==
// @name         BIDV
// @namespace    http://tampermonkey.net/
// @version      1.1
// @description  BIDV autofill and CAPTCHA focus.
// @author       Your Name
// @match        https://ebank.bidv.com.vn/DKNHDT/muavang.htm
// @grant        none
// ==/UserScript==

(function() {
    'use strict';

    let person = {
        fullName: 'Mai Văn Tùng',
        idNumber: '035203003891',
        issuePlace: 'cục cảnh sát qlhc về ttxh',
        issueDate: '31/12/2021',
        address: 'đồng tân, tân sơn, kim bảng, hà nam',
        phone: '0949355950',
        birthday: '30/01/2003',
        email: 'mai.vantung03@proton.me',
        vietinSexIndex: 2,
        vietinLoaiCC: 1,
        vietinNoiGiaoDich: 1,
        bidvAccNum: 4823373892,
        bidvAmount: 1,
        bidvBranch: 1,
        bidvCapital: 1,
        bidvPurpose: 2,
        bidvIssuePlace: 0,
    };

    async function main() {
    // Fill input fields in one promise
    const fillInputsPromise = (async () => {
        await fillInput('input[name=addREss2]', person.address); // address2
        await fillInput('input[name=addREss1]', person.address); // address1
        await fillInput('input[name=aMOunt]', person.bidvAmount); // amount
        await fillInput('input[name=accTNum]', person.bidvAccNum); // account number
        await fillInput('input[name=cellPHone]', person.phone); // phone number
        await fillInput('input[name=idNUmber]', person.idNumber); // ID number
        await fillInput('input[name=fullNAme]', person.fullName); // full name
        await fillInput('input[name=isSUedate]', person.issueDate); // issue date
        await fillInput('input[name=birthDAy]', person.birthday); // birthday

        await clickButtonWithDelay('button[id=btnYes]', 10); // Click 'Yes' button after a delay

        await selectSelector('select[name=braNCh]', person.bidvBranch); // branch
        await selectSelector('select[name=caPItal]', person.bidvCapital); // capital
        await selectSelector('select[name=puRPose]', person.bidvPurpose); // purpose
        await selectSelector('select[name=isSUeplace]', person.bidvIssuePlace); // issue place

        await clickButtonWithDelay('ins', 10); // Click 'ins' element after a delay
    })();

    // Solve captcha in another promise
    const base64Image = await getCaptchaBase64(); // Ensure the captcha is retrieved after the image loads
    const solveCaptchaPromise = solveCaptcha(base64Image); // Solve CAPTCHA

    // Wait for both tasks to complete
    const [_, captchaText] = await Promise.all([fillInputsPromise, solveCaptchaPromise]);

    // Fill the captcha input
    await fillInput('input[name=capTCha]', captchaText); // Fill the captcha input

    // Click the button
    await clickButton('button.g-recaptcha.btn.btn-blue.next-step.btn-block'); // Click the next step button
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

    function selectSelector(query, value) {
        const e = document.querySelector(query);
        e.selectedIndex = value;
        triggerEvent(e, 'input');
        triggerEvent(e, 'change');
    }
    
    // function disableBlockAutoFill() {
    //    const proto = Object.getPrototypeOf(window);
        
    //     if (proto && typeof proto.blockAutoFill === 'function') {
    //         proto.blockAutoFill = function() {
    //             // Overwrite the original function to do nothing
    //             console.log('blockAutoFill disabled on prototype level.');
    //         };
    //     }
    // }

    // function focusCaptchaInput() {
    //     const captchaInput = document.getElementById('capTCha');
    //     if (captchaInput) {
    //         captchaInput.focus();

    //         captchaInput.addEventListener('keydown', function(event) {
    //             if (event.key === 'Enter') {
    //                 event.preventDefault();
    //                 const submitButton = document.querySelector('.g-recaptcha.btn.btn-blue.next-step.btn-block');
    //                 if (submitButton) {
    //                     submitButton.click();
    //                 }
    //             }
    //         });
    //     }
    //     const otpInput = document.getElementById('otpcode');
    //     if (otpInput) {
    //         otpInput.addEventListener('keydown', function(event) {
    //             if (event.key === 'Enter') {
    //                 event.preventDefault();
    //                 const submitButton = document.querySelector('.btn.btn-blue.finish-step.btn-block'); //btn btn-blue finish-step btn-block
    //                 if (submitButton) {
    //                     submitButton.click();
    //                 }
    //             }
    //         });
    //     }
    // }

    async function getCaptchaBase64() {
    // Get the captcha image element
    const img = document.getElementById('captchaImg');

    // Create a canvas element
    const canvas = document.createElement('canvas');
    const ctx = canvas.getContext('2d');

    // Set the canvas dimensions to match the image
    canvas.width = img.width;
    canvas.height = img.height;

    // Draw the image onto the canvas
    ctx.drawImage(img, 0, 0);

    // Get the base64-encoded image data
    const base64Img = canvas.toDataURL('image/png').split(',')[1]; // Remove the data URL prefix

    return base64Img; // Returns the base64 string
}
    
async function solveCaptcha(base64Img) {
    const apiUrl = "https://anticaptcha.top/api/captcha";
    const payload = {
        apikey: "796b02353453441eb50179e374758059", // Replace with your actual API key
        img: base64Img,
        type: 31//bidv
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
    // Run the main function
    //disableBlockAutoFill();
    main();

})();
