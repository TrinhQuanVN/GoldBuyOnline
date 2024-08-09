//sửa thông tin ở đây
let person = {
  fullName: 'Mai Văn Tùng',
  idNumber: '035203003891',
  issuePlace: 'cục cảnh sát qlhc về ttxh',
  issueDate: '31/12/2021',
  address: 'đồng tân, tân sơn, kim bảng, hà nam',
  phone: '0949355950',
  birthday: '30/01/2003',
  email: 'mai.vantung03@proton.me',//Maivantung03@
  vietinSexIndex: 2, //2 nam 1 nu
  vietinLoaiCC: 1,
  vietinNoiGiaoDich: 1,
  bidvAccNum: 4823373892, //tài khoản bid
  bidvAmount: 1,
  bidvBranch: 1, //dia điểm giao dịch
  bidvCapital: 1,
  bidvPurpose: 2,
  bidvIssuePlace: 0,
}

function fillInput(query, value, doubleTrigger = 0) {
    return new Promise(resolve => {
        setTimeout(() => {
            const input = document.querySelector(query);
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

function triggerEvent(el, type) {
    const event = new Event(type, { bubbles: true });
    el.dispatchEvent(event);
}

setTimeout(async function() {
  await fillInput('input.bB[name=hvt]', person.fullName);
  await fillInput('input.bB[name=s]', person.idNumber);
  await fillInput('input.bB[name=dcc]', person.issuePlace);
  await fillInput('input.bB[name=dctt]', person.address);
  await fillInput('input.bB[name=sdt]', person.phone);
  await fillInput('input.bB[name=ns]', person.birthday);
  await fillInput('input.bB[name=ht]', person.email);
  await fillInput('input.bB[name=nc]', person.issueDate);

  var input = document.querySelector('select[name=l]');
  input.selectedIndex = person.vietinLoaiCC;
  triggerEvent(input,'input');
  triggerEvent(input,'change');

    var input = document.querySelector('select[name=gt]');
  input.selectedIndex = person.vietinSexIndex;
  triggerEvent(input,'input');
  triggerEvent(input,'change');

    var input = document.querySelector('select[name=dc]');
  input.selectedIndex = person.vietinNoiGiaoDich;
  triggerEvent(input,'input');
  triggerEvent(input,'change');
  

  document.querySelector('input.bB[name="mxn"]').focus(); // focus vao o viet capcha
  window.scrollTo(0, document.body.scrollHeight);  // Cuộn xuống cuối trang
  console.log('fill info scueess');
},10);
//enable button dang ky
let button;
let mxn;
let thongBao;
let inputEmailCode;
let now;
let timer;
console.log('running scrpit');
timer = setInterval(function() {
  now = new Date();
  button = document.querySelector('.pa-btn.gh'); // Nút xác nhận xac nhap trang 1
  mxn = document.querySelector('input.bB[name="mxn"]'); // Trường nhập captcha
  thongBao = document.querySelector('.tB').innerText;//thong bao
  inputEmailCode = document.querySelector('input[name="dh"]'); // Trường nhập mã email

  if (mxn.value.length === 5) {
        if (button.disabled) {
          button.disabled = false;
        }
        button.click();
  }
    if (!inputEmailCode.disabled) {
    console.log('email input found!');
    document.title = 'Email';
    clearInterval(timer);
    timer = setInterval(function() {
        button = document.querySelector('.pa-btn.dh'); // Nút xác nhận trang 2
        inputEmailCode = document.querySelector('input[name="dh"]'); // Trường nhập mã email
        if(inputEmailCode.value.length>0){
          if (button.disabled) {
          button.disabled = false;
        }
        button.click();
        }
    },1);
    }
    
}, 1);

