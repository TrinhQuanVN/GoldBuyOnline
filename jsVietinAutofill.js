//sửa thông tin ở đây
setTimeout(function() {
  console.log('run1');
  //ho ten
  document.querySelector('input.bB[name="hvt"]').value = 'trịnh tiến quân'
  //id number
  document.querySelector('input.bB[name="s"]').value = '035092013752'
  //noi cap
  document.querySelector('input.bB[name="dcc"]').value = 'cục cảnh sát qlhc về ttxh'
  //adress
  document.querySelector('input.bB[name="dctt"]').value = 'Lương Khánh Thiện, Phủ Lý, hà nam'
  //phone
  document.querySelector('input.bB[name="sdt"]').value = '0962130922'
  // ngay sinh
  document.querySelector('input.bB[name="ns"]').value='28-10-1992';
  //email
  document.querySelector('input.bB.hT[name="ht"]').value = 'trinhquanhn1992@gmail.com'
  // ngay cap
  document.querySelector('input.bB.cN[name="nc"]').value = '25-08-2021';
  // chon loai can cuoc
  document.querySelector('select[name="l"]').selectedIndex = 1;
  // chon gt
  document.querySelector('select[name="gt"]').selectedIndex = 2;
  // chon noi gd
  document.querySelector('select[name="dc"]').selectedIndex = 1;
  // focus vao o viet capcha
  document.querySelector('input.bB[name="mxn"]').focus();
  // Cuộn xuống cuối trang
  window.scrollTo(0, document.body.scrollHeight);  
},100);
//enable button dang ky
var timer = setInterval(function() {
    let now = new Date();
    console.log('running... ' + now.getHours() + ':' +now.getMinutes());
  var button = document.querySelector('.pa-btn.gh'); // Nút xác nhận xac nhap trang 1
    var button2 = document.querySelector('.pa-btn.dh'); // Nút xác nhận trang 2
  var mxn = document.querySelector('input.bB[name="mxn"]'); // Trường nhập captcha
  var inputEmailCode = document.querySelector('input[name="dh"]'); // Trường nhập mã email
  var thongBao = document.querySelector('.tB').innerText;//thong bao
    if(now.getHours()===11 && now.getMinutes()<11){
      if (mxn.value.length === 5) {
            if (button.disabled || button2.disabled) {
              button.disabled = false;
              button2.disabled = false;
            }
            button.click();
      }
        if (!inputEmailCode.disabled) {
        console.log('email input found!');
        document.title = 'Email';
        clearInterval(timer);
        }
    }

}, 1);

//------------------------------mai anh
setInterval(function() {
    var button = document.querySelector('.pa-btn.gh');
var button2 = document.querySelector('.pa-btn.dh'); // Nút xác nhận trang 2
    if (button.disabled) {
        button.disabled = false;
    }
     if (button2.disabled) {
        button2.disabled = false;
    }
}, 100);

//------------------------------auto click button: copy to console trước đó nhớ allow pasting
var button = document.querySelector('.pa-btn.gh');
var counter = 0;
setInterval(function() {
    if (button.disabled) {
        button.disabled = false;
    }
    button.click();
    counter++;
    document.title = counter;
}, 100);

//----------------------open tabs
// Open 20 tabs with the same URL
for (let i = 0; i < 20; i++) {
  window.open('https://dangkymuavang.vietinbankgold.vn/');
}



