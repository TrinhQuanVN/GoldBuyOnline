let apiKey = '16e27e68938cdb1fc7633a9ebef359f3';

var captchaImage = document.querySelector('img[src*="gold-online"]'); // Adjust the selector based on the actual HTML
var payload = {
    
};
//numeric: 0 - không có yêu cầu 1 - chỉ cho phép số 2 - cho phép mọi chữ cái ngoại trừ số
//body: Nội dung tập tin được mã hóa trong base64
//minLength: 0 - không có yêu cầu >0 - xác định độ dài tối thiểu của câu trả lời
//maxLength: 0 - không có yêu cầu >0 - xác định độ dài tối đa của câu trả lời
function createPayloadAntiCaptcha(apiKey,type='ImageToTextTask',body,numeric=0,minLength=0,maxLength=0){
    return {
        clientKey: apiKey,
    "task":
        {
            "type":"ImageToTextTask",
            "body":"BASE64_BODY_HERE__NO_NEWLINES__NO_EXTRA_TAGS__ONLY_CLEAN_BASE64",
            "phrase":false,
            "case":false,
            "numeric":0,
            "math":false,
            "minLength":0,
            "maxLength":0,
            "languagePool":"en"
        },
    "softId": 0
    }
},
function solveCaptcha(api,image){
    var basedata = imageToBase64(image);
    var antiCaptchaSolve(api,basedata,payload);
}
if (captchaImage) {
    var canvas = document.createElement('canvas');// Create a canvas element
    canvas.width = captchaImage.width;
    canvas.height = captchaImage.height;
    var ctx = canvas.getContext('2d'); // Draw the CAPTCHA image onto the canvas
    ctx.drawImage(captchaImage, 0, 0);
    var base64data = canvas.toDataURL('image/png').split(',')[1]; // Get base64 data without prefix
    var payload = { 
        clientKey: apiKey,
        task: {
            type: 'ImageToTextTask',
            body: base64data
        }
    };

    fetch('https://api.anti-captcha.com/createTask', {// Send the Base64 image to the anti-captcha service
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(payload)
    })
    .then(response => response.json())
    .then(data => {
        console.log('Anti-Captcha Task ID:', data.taskId);
        if (data.taskId) {
            pollForResult(data.taskId, apiKey);
        }
    })
    .catch(error => {
        console.error('Error:', error);
    });
} else {
    console.log('CAPTCHA image not found.');
}

function pollForResult(taskId, apiKey) {
    var resultPayload = {
        clientKey: apiKey,
        taskId: taskId
    };

    fetch('https://api.anti-captcha.com/getTaskResult', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(resultPayload)
    })
    .then(response => response.json())
    .then(data => {
        if (data.status === 'ready') {
            console.log('CAPTCHA Text:', data.solution.text);
            document.querySelector('input.bB[name=mxn]').value = data.solution.text;
        } else {
            console.log('Task not ready yet, polling again in 5 seconds...');
            setTimeout(() => pollForResult(taskId, apiKey), 5000);
        }
    })
    .catch(error => {
        console.error('Error:', error);
    });
}


