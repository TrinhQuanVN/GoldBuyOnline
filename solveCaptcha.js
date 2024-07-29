let apiKey = 16e27e68938cdb1fc7633a9ebef359f3;
// Find the CAPTCHA image element
var captchaImage = document.querySelector('img[src*="gold-online"]'); // Adjust the selector based on the actual HTML

// Get the CAPTCHA image URL
var captchaImageUrl = captchaImage ? captchaImage.src : null;

if (captchaImageUrl) {
    console.log('CAPTCHA Image URL:', captchaImageUrl);
} else {
    console.log('CAPTCHA image not found.');
}
if (captchaImageUrl) {
    fetch(captchaImageUrl)
        .then(response => response.blob())
        .then(blob => {
            var reader = new FileReader();
            reader.onloadend = function() {
                var base64data = reader.result.split(',')[1]; // Get base64 data without prefix
                console.log('CAPTCHA Image Base64:', base64data);

                // Send to anti-captcha service (example for anti-captcha.com)
                var payload = {
                    clientKey: apiKey,
                    task: {
                        type: 'ImageToTextTask',
                        body: base64data
                    }
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
                    console.log('Anti-Captcha Response:', data);
                })
                .catch(error => {
                    console.error('Error:', error);
                });
            };
            reader.readAsDataURL(blob);
        })
        .catch(error => {
            console.error('Error fetching CAPTCHA image:', error);
        });
}
