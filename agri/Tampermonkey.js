// ==UserScript==
// @name         Replace is-valid-time Response
// @namespace    http://tampermonkey.net/
// @version      1.0
// @description  Modify response for requests including "is-valid-time"
// @author       Your Name
// @match        https://bookingonline.agribank.com.vn/muavangSJCtructuyen
// @grant        none
// ==/UserScript==

(function() {
    'use strict';
    
    const originalOpen = XMLHttpRequest.prototype.open;

    XMLHttpRequest.prototype.open = function(method, url) {
        if (url.includes('is-valid-time')) {
            this.addEventListener('readystatechange', function() {
                if (this.readyState === 4) {
                    try {
                        console.log('Intercepting request to: ' + url);

                        Object.defineProperty(this, 'response', { writable: true });
                        Object.defineProperty(this, 'responseText', { writable: true });

                        this.response = this.responseText = JSON.stringify({
                            isValidTime: true
                        });

                        console.log('Response modified to: ' + this.responseText);
                    } catch (e) {
                        console.error('Error modifying response:', e);
                    }
                }
            });
        }

        return originalOpen.apply(this, arguments);
    };

    console.log('Userscript is running...');
})();
