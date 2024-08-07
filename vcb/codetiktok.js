//mo binh thuong copy code duoi vao console
//8h:55 moi duoc chon gio
(function() {
  const originalOpen = XMLHttpRequest.prototype.open;

  XMLHttpRequest.prototype.open = function(method, url) {
    this.addEventListener('readystatechange', function() {
      if (this.readyState === 4 && url.includes('check-booking-time')) {
        console.log('...');

        
        Object.defineProperty(this, 'response', {writable: true});
        Object.defineProperty(this, 'responseText', {writable: true});
        this.response = this.responseText = JSON.stringify({
          code: 0,
          desc: "Success",
          data: true
        });
      }
    });

    return originalOpen.apply(this, arguments);
  };

  console.log('...');
})();
