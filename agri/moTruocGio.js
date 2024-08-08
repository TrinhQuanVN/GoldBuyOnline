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

javascript:(function(){const originalOpen=XMLHttpRequest.prototype.open;XMLHttpRequest.prototype.open=function(method,url){this.addEventListener('readystatechange',function(){if(this.readyState===4&&url.includes('timeZoneId=Asia/Bangkok')){console.log('...');Object.defineProperty(this,'response',{writable:true});Object.defineProperty(this,'responseText',{writable:true});this.response=this.responseText=JSON.stringify({isValidTime:true,nextTime:""});}});return originalOpen.apply(this,arguments);};console.log('...');})();
