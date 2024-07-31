const APIKEY = '16e27e68938cdb1fc7633a9ebef359f3';
const NUMERIC = 0;
const MINLENGTH = 0,
const MAXLENGTH = 0;
var captchaImage = document.querySelector('img[src*="gold-online"]'); // Adjust the selector based on the actual HTML
//numeric: 0 - không có yêu cầu 1 - chỉ cho phép số 2 - cho phép mọi chữ cái ngoại trừ số
//body: Nội dung tập tin được mã hóa trong base64
//minLength: 0 - không có yêu cầu >0 - xác định độ dài tối thiểu của câu trả lời
//maxLength: 0 - không có yêu cầu >0 - xác định độ dài tối đa của câu trả lời
function createPayloadAntiCaptcha(apiKey,type='ImageToTextTask',body,numeric=0,minLength=0,maxLength=0){
    return {
        clientKey: apiKey,
    task:
        {
            "type":type,
            "body":body,
            "phrase":false,
            "case":false,
            "numeric":numeric,
            "math":false,
            "minLength":minLength,
            "maxLength":maxLength,
            "languagePool":"en"
        },
    softId: 0
    }
}
//will return taskId
function postRequestToAntiCaptcha(payload){
    fetch('https://api.anti-captcha.com/createTask', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(payload)
    }).then(response => response.json())
    .then(data => {console.log(data); return data.taskId;})
    .catch(e => {console.error('error: ',e)});
}
//return captcha
function PostTaskIdToAntiCaptcha(apiKey,taskId){
    var resultPayload = {
        clientKey: apiKey,
        taskId: taskId,
    };

    fetch('https://api.anti-captcha.com/getTaskResult', {
        method: 'POST',
        headers: {
            'Content-Type': 'application/json'
        },
        body: JSON.stringify(resultPayload)
    })
    .then(response => response.json())
    .then(data=> {
        console.log(data);
        if(data.status === 'ready'){
            return data.solution.text;
        }else{
            return null;
        }
    })
}
function imageToBase64(image){
    var canvas = document.createElement('canvas');// Create a canvas element
    canvas.width = image.width;
    canvas.height = image.height;
    var ctx = canvas.getContext('2d'); // Draw the CAPTCHA image onto the canvas
    ctx.drawImage(image, 0, 0);
    return canvas.toDataURL('image/png').split(',')[1];
}

function solveCaptcha(api,image){
    var basedata = imageToBase64(image);
    var payload = createPayloadAntiCaptcha(api,'ImageToTextTask',basedata,NUMERIC,MINLENGTH,MAXLENGTH);
    var taskId = postRequestToAntiCaptcha(payload);
    if(taskId){
        return PostTaskIdToAntiCaptcha(api,taskId);
    }
}



//------------------
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


