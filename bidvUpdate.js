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
};


setInterval(()=> {});
  clickButtonWithDelay('#btnYes'); // Click 'Yes' button after a delay
  
  fillInput('input[name=addREss2]', person.address);       // address2
  fillInput('input[name=addREss1]', person.address);       // address1
  fillInput('input[name=amount]', person.bidvAmount);      // amount
  fillInput('input[name=accTNum]', person.bidvAccNum);     // account number
  fillInput('input[name=cellPHone]', person.phone);        // phone number
  fillInput('input[name=idNUmber]', person.idNumber);      // ID number
   fillInput('input[name=fullNAme]', person.fullName);      // full name
   fillInput('input[name=isSUedate]', person.issueDate);    // issue date
   fillInput('input[name=birthDAy]', person.birthday);      // birthday
   fillInput('select[name=braNCh]', person.bidvBranch, 1);   // branch
   fillInput('select[name=caPItal]', person.bidvCapital, 1); // capital
   fillInput('select[name=puRPose]', person.bidvPurpose, 1); // purpose
   fillInput('select[name=isSUeplace]', person.bidvIssuePlace, 1); // issue place

   clickButtonWithDelay('ins'); // Click 'ins' element after a delay

  window.scrollTo(0, document.body.scrollHeight); // scroll to bottom


if(document.getElementById('caPItal')){
    fillInput('select[name=braNCh]', person.bidvBranch, 1);   // branch
   fillInput('select[name=caPItal]', person.bidvCapital, 1); // capital
   fillInput('select[name=puRPose]', person.bidvPurpose, 1); // purpose
   fillInput('select[name=isSUeplace]', person.bidvIssuePlace, 1); // issue place
  
  function fillInput(query, value, trigger = 0) {
  const e = document.querySelector(query);
  e.value = value;
  triggerEvent(e, 'input');
  if (trigger > 0) {
    triggerEvent(e, 'change');
  }
}
}

function triggerEvent(el, type) {
  const event = new Event(type, { bubbles: true });
  el.dispatchEvent(event);
}

function fillInput(query, value, trigger = 0) {
  const e = document.querySelector(query);
  e.value = value;
  triggerEvent(e, 'input');
  if (trigger > 0) {
    triggerEvent(e, 'change');
  }
}

function clickButtonWithDelay(selector, delay = 100) {
  const button = document.querySelector(selector);
  if (button) {
    button.click();
  }
}

