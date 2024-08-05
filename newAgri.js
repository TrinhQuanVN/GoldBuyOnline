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

const API_KEY = '16e27e68938cdb1fc7633a9ebef359f3';
const NUMERIC = 0; //numeric: 0 - không có yêu cầu 1 - chỉ cho phép số 2 - cho phép mọi chữ cái ngoại trừ số
const MIN_LENGTH = 6;//vietin 5so - agribank 6 chu
const MAX_LENGTH = 6;

const DDGD = 0 //0:'Agribank Chi nhánh Sở giao dịch' - 1:'Agribank Chi nhánh Hà Nội' - 2:'Agribank Chi nhánh Cầu Giấy' - 3:'Agribank Chi nhánh Hà Tây'

document.addEventListener('DOMContentLoaded', main);
window.addEventListener('load', main);

async function main() {
    try {
        // Start step 1
        await clickButton('input[id=input-25]');

        // Select dia diem
        await new Promise(resolve => {
            setTimeout(() => {
                var options = document.querySelectorAll('.vue-recycle-scroller__item-view .v-list-item__title');
                if (options[DDGD]) {
                    options[DDGD].click();
                }
                resolve();
            }, 100);
        });

        // Click button to proceed to step 2
        await clickButton('button[type="button"][step="3"].btn-main.next-step');

        // Start step 2
        await Promise.all([...fillElements(), fillCaptcha()]);
        
        // Click button to confirm
        await clickButton('button[data-v-5d38e429]');
    } catch (error) {
        console.error('Error in main function:', error);
    }
}

function fillElements() {
    const fillInputs = [
        fillInput('input[id=input-96]', person.fullName),
        fillInput('input[id=input-101]', person.birthday, 1),
        fillInput('input[id=input-105]', person.address),
        fillInput('input[id=input-111]', person.phone),
        fillInput('input[id=input-114]', person.idNumber),
        fillInput('input[id=input-123]', person.issuePlace),
        fillInput('input[id=input-119]', person.issueDate, 1)
    ];
    return [...fillInputs, checkCheckBox('input[type=checkbox]', true)];
}

function checkCheckBox(query, checked) {
    return new Promise(resolve => {
        setTimeout(() => {
            var input = document.querySelector(query);
            if (input) {
                input.checked = checked;
                triggerEvent(input, 'change');
            }
            resolve();
        }, 100);
    });
}

function fillInput(query, value, doubleTrigger = 0) {
    return new Promise(resolve => {
        setTimeout(() => {
            var input = document.querySelector(query);
            if (input) {
                input.focus();
                input.value = value;
                triggerEvent(input, 'input');
                if (doubleTrigger !== 0) {
                    triggerEvent(input, 'change');
                }
            }
            resolve();
        }, 100);
    });
}

function fillCaptcha() {
    return new Promise((resolve, reject) => {
        setTimeout(() => {
            var img = document.querySelector('img[data-v-772eadec]');
            if (img) {
                var base64Img = img.src.split(',')[1]; // Ensure this is correct for your case
                var input = document.getElementById('input-273');
                if (input) {
                    solveCaptcha(base64Img).then(captchaText => {
                        input.value = captchaText;
                        triggerEvent(input, 'input');
                        resolve();
                    }).catch(err => reject(err));
                } else {
                    reject('Captcha input element not found');
                }
            } else {
                reject('Captcha image not found');
            }
        }, 100);
    });
}

function clickButton(query) {
    return new Promise(resolve => {
        setTimeout(() => {
            var button = document.querySelector(query);
            if (button) {
                button.click();
            }
            resolve();
        }, 100);
    });
}

function solveCaptcha(base64Img) {
    return new Promise((resolve, reject) => {
        let payload = {
            clientKey: API_KEY,
            task: {
                type: 'ImageToTextTask',
                body: base64Img,
                phrase: false,
                case: false,
                numeric: NUMERIC,
                math: false,
                minLength: MIN_LENGTH,
                maxLength: MAX_LENGTH,
                languagePool: 'en'
            },
            softId: 0
        };

        fetch('https://api.anti-captcha.com/createTask', {
            method: 'POST',
            headers: {
                'Content-Type': 'application/json'
            },
            body: JSON.stringify(payload)
        })
        .then(response => response.json())
        .then(data => {
            if (data.errorId !== 0) {
                return reject(`Error: ${data.errorCode}`);
            }

            const taskId = data.taskId;

            const intervalId = setInterval(() => {
                fetch('https://api.anti-captcha.com/getTaskResult', {
                    method: 'POST',
                    headers: {
                        'Content-Type': 'application/json'
                    },
                    body: JSON.stringify({
                        clientKey: API_KEY,
                        taskId: taskId
                    })
                })
                .then(response => response.json())
                .then(data => {
                    if (data.status === 'ready') {
                        clearInterval(intervalId);
                        resolve(data.solution.text);
                    } else if (data.status === 'processing') {
                        // Continue checking
                    } else {
                        clearInterval(intervalId);
                        reject(`Error: ${data.errorCode}`);
                    }
                })
                .catch(error => {
                    clearInterval(intervalId);
                    reject(error);
                });
            }, 100); // Check every second
        })
        .catch(error => {
            reject(error);
        });
    });
}

function triggerEvent(el, type) {
    const event = new Event(type, { bubbles: true });
    el.dispatchEvent(event);
}
