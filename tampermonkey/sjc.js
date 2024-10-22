// ==UserScript==
// @name         SJC
// @namespace    http://tampermonkey.net/
// @version      2024-10-22
// @description  sjc auto select
// @author       You
// @match        https://tructuyen.sjc.com.vn/
// @icon         data:image/gif;base64,R0lGODlhAQABAAAAACH5BAEKAAEALAAAAAABAAEAAAICTAEAOw==
// @grant        none
// ==/UserScript==

(function() {
    'use strict';

    function triggerEvent(el, type) {
const event = new Event(type, { bubbles: true });
el.dispatchEvent(event);
}

function selectSelectorArea(query, index = "Thành phố Hà Nội") {
    return new Promise((resolve) => {
        setTimeout(() => {
            const area = document.querySelectorAll(query)[0];
            if (area.length > 0) {
                let hanoiArea = Array.from(area).find(option => option.innerText.trim() === index);
                if (hanoiArea) {
                    hanoiArea.selected = true;
                    triggerEvent(hanoiArea, 'change');
                    triggerEvent(hanoiArea, 'input');
                    console.log('...');

                }
            }
            resolve();
        }, 10);
    });
}

function selectSelectorStore(query, index) {
    return new Promise((resolve) => {
        setTimeout(() => {
            const store = document.querySelectorAll(query)[0];
            if (store.length > 0 && store[index]) {
                const hanoistore = store[index];
                hanoistore.selected = true;
                triggerEvent(hanoistore, 'change');
                triggerEvent(hanoistore, 'input');
                console.log('...');
            }
            resolve();
        }, 10);
    });
}
async function main(){
    await selectSelectorArea('#id_area');
    await selectSelectorStore('#id_store',1);
    console.log('...');
}

main();
})();
