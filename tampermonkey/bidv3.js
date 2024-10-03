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

        window.scrollTo(0, document.body.scrollHeight); // scroll to bottom

        focusCaptchaInput(); // Focus on CAPTCHA input when the page loads
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

    function selectSelector(query, value) {
        const e = document.querySelector(query);
        e.selectedIndex = value;
        triggerEvent(e, 'input');
        triggerEvent(e, 'change');
    }
    
    function disableBlockAutoFill() {
       const proto = Object.getPrototypeOf(window);
        
        if (proto && typeof proto.blockAutoFill === 'function') {
            proto.blockAutoFill = function() {
                // Overwrite the original function to do nothing
                console.log('blockAutoFill disabled on prototype level.');
            };
        }
    }

    function focusCaptchaInput() {
        const captchaInput = document.getElementById('capTCha');
        if (captchaInput) {
            captchaInput.focus();

            captchaInput.addEventListener('keydown', function(event) {
                if (event.key === 'Enter') {
                    event.preventDefault();
                    const submitButton = document.querySelector('.g-recaptcha.btn.btn-blue.next-step.btn-block');
                    if (submitButton) {
                        submitButton.click();
                    }
                }
            });
        }
        const otpInput = document.getElementById('otpcode');
        if (otpInput) {
            otpInput.addEventListener('keydown', function(event) {
                if (event.key === 'Enter') {
                    event.preventDefault();
                    const submitButton = document.querySelector('.btn.btn-blue.finish-step.btn-block'); //btn btn-blue finish-step btn-block
                    if (submitButton) {
                        submitButton.click();
                    }
                }
            });
        }
    }

    // Run the main function
    disableBlockAutoFill();
    main();

})();
