javascript:(function() {
    const originalOpen = XMLHttpRequest.prototype.open;

    XMLHttpRequest.prototype.open = function(method, url) {
        this.addEventListener('readystatechange', function() {
            if (this.readyState === 4 && url.includes('timeZoneId=Asia/Bangkok')) {
                console.log('...');

                Object.defineProperty(this, 'response', {writable: true});
                Object.defineProperty(this, 'responseText', {writable: true});
                this.response = this.responseText = JSON.stringify({
                    isValidTime: true,
                    nextTime: "",
                });
            }
        });

        return originalOpen.apply(this, arguments);
    };

    console.log('...');
})();

javascript:(function()%7Bconst%20originalOpen%20%3D%20XMLHttpRequest.prototype.open%3B%0AXMLHttpRequest.prototype.open%20%3D%20function(method%2C%20url)%20%7B%0A%20%20%20%20this.addEventListener('readystatechange'%2C%20function()%20%7B%0A%20%20%20%20%20%20%20%20if%20(this.readyState%20%3D%3D%3D%204%20%26%26%20url.includes('check-booking-time'))%20%7B%0A%20%20%20%20%20%20%20%20%20%20%20%20console.log('...')%3B%0A%0A%20%20%20%20%20%20%20%20%20%20%20%20Object.defineProperty(this%2C%20'response'%2C%20%7Bwritable%3A%20true%7D)%3B%0A%20%20%20%20%20%20%20%20%20%20%20%20Object.defineProperty(this%2C%20'responseText'%2C%20%7Bwritable%3A%20true%7D)%3B%0A%20%20%20%20%20%20%20%20%20%20%20%20this.response%20%3D%20this.responseText%20%3D%20JSON.stringify(%7B%0A%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20code%3A%200%2C%0A%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20desc%3A%20%22Success%22%2C%0A%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20%20data%3A%20true%0A%20%20%20%20%20%20%20%20%7D)%3B%0A%20%20%20%20%20%20%20%20%7D%0A%20%20%20%20%7D)%3B%0A%0A%20%20%20%20return%20originalOpen.apply(this%2C%20arguments)%3B%0A%7D%3B%0Aconsole.log('...')%3B%7D)()%3B
