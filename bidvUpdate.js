//------------------------------------------------- phan 1
let person = {
  fullName: 'trịnh tiến quân',
  idNumber: '035092013752',
  issuePlace: 'cục cảnh sát qlhc về ttxh',
  issueDate: '25/08/2021',
  address: 'Lương Khánh Thiện, Phủ Lý, hà nam',
  phone: '0962130922',
  birthday: '28/10/1992',
  email: 'trinhquanhn1992@gmail.com',
  vietinSexIndex: 2,
  vietinLoaiCC: 1,
  vietinNoiGiaoDich: 1,
  bidvAccNum: 4821813240, //tài khoản bid
  bidvAmount: 1,
  bidvBranch: 1, //dia điểm giao dịch
  bidvCapital: 1,
  bidvPurpose: 2,
  bidvIssuePlace: 0,
}
//------------
let timer;
let e;
function main(){
  timer = setInterval(function(){
    if(document.readyState === 'complete'){
      fillInput();
      clearInterval(timer);
    }
  },1)
}
function triggerEvent(el, type) {
    const event = new Event(type, { bubbles: true });
    el.dispatchEvent(event);
}
function fillInput(){
  e= document.getElementById("address2"); e.value = person.address;   //address2
  e= document.getElementById("address1"); e.value = person.address;  //address1
  e= document.getElementById("amount"); e.value = person.bidvAmount; //amount
  e= document.getElementById("acctnum"); e.value = person.bidvAccNum;  //acctnum
  e= document.getElementById("cellphone"); e.value = person.phone;  //cellphone
  e= document.getElementById("idnumber"); e.value = person.idNumber;  //idnumber
  e= document.getElementById("fullname"); e.value = person.fullName;  //full name
  e= document.getElementById("issuedate"); e.value = person.issueDate;  //ngay cap issuedate
  e= document.getElementById("birthday"); e.value = person.birthday;  //birthday
  document.getElementById('btnYes').click(); //click dong y co tai khoan
  e= document.getElementById("branch"); e.selectedIndex = person.bidvBranch; triggerEvent(e, 'change'); //branch dia diem giao dich
  e= document.getElementById("capital"); e.selectedIndex = person.bidvCapital; triggerEvent(e, 'change'); //capital
  e= document.getElementById("purpose"); e.selectedIndex = person.bidvPurpose; triggerEvent(e, 'change'); //purpose
  e= document.getElementById("issueplace"); e.selectedIndex = person.bidvIssuePlace; triggerEvent(e, 'change'); //issueplace
  document.querySelector('ins').click();
  window.scrollTo(0, document.body.scrollHeight);// Cuộn xuống cuối trang
}
main();
