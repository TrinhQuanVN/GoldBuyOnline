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
  e= document.getElementById("addREss2"); e.value = person.address;
  triggerEvent(e,'input');
  e= document.getElementById("addREss1"); e.value = person.address;  //address1
  triggerEvent(e,'input');
  e= document.getElementById("amount"); e.value = person.bidvAmount; //amount
  triggerEvent(e,'input');
  e= document.getElementById("accTNum"); e.value = person.bidvAccNum;  //acctnum
  triggerEvent(e,'input');
  e= document.getElementById("cellPHone"); e.value = person.phone;  //cellphone
  triggerEvent(e,'input');
  e= document.getElementById("idNUmber"); e.value = person.idNumber;  //idnumber
  triggerEvent(e,'input');
  e= document.getElementById("fullNAme"); e.value = person.fullName;  //full name
  triggerEvent(e,'input');
  e= document.getElementById("isSUedate"); e.value = person.issueDate;  //ngay cap issuedate
  triggerEvent(e,'input');
  e= document.getElementById("birthDAy"); e.value = person.birthday;  //birthday
  triggerEvent(e,'input');
  document.getElementById('btnYes').click(); //click dong y co tai khoan
  e= document.getElementById("braNCh"); e.selectedIndex = person.bidvBranch; triggerEvent(e, 'change'); //branch dia diem giao dich
  e= document.getElementById("caPItal"); e.selectedIndex = person.bidvCapital; triggerEvent(e, 'change'); //capital
  e= document.getElementById("puRPose"); e.selectedIndex = person.bidvPurpose; triggerEvent(e, 'change'); //purpose
  e= document.getElementById("isSUeplace"); e.selectedIndex = person.bidvIssuePlace; triggerEvent(e, 'change'); //issueplace
  document.querySelector('ins').click();
  window.scrollTo(0, document.body.scrollHeight);// Cuộn xuống cuối trang
}
main();
