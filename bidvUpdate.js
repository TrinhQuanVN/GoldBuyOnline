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

async function main() {
  await clickButtonWithDelay('#btnYes'); // Click 'Yes' button after a delay
  
  await fillInput('input[name=addREss2]', person.address);       // address2
  await fillInput('input[name=addREss1]', person.address);       // address1
  await fillInput('input[name=amount]', person.bidvAmount);      // amount
  await fillInput('input[name=accTNum]', person.bidvAccNum);     // account number
  await fillInput('input[name=cellPHone]', person.phone);        // phone number
  await fillInput('input[name=idNUmber]', person.idNumber);      // ID number
  await fillInput('input[name=fullNAme]', person.fullName);      // full name
  await fillInput('input[name=isSUedate]', person.issueDate);    // issue date
  await fillInput('input[name=birthDAy]', person.birthday);      // birthday
  await fillInput('select[name=braNCh]', person.bidvBranch, 1);   // branch
  await fillInput('select[name=caPItal]', person.bidvCapital, 1); // capital
  await fillInput('select[name=puRPose]', person.bidvPurpose, 1); // purpose
  await fillInput('select[name=isSUeplace]', person.bidvIssuePlace, 1); // issue place

  await clickButtonWithDelay('ins'); // Click 'ins' element after a delay

  window.scrollTo(0, document.body.scrollHeight); // scroll to bottom
}

function triggerEvent(el, type) {
  const event = new Event(type, { bubbles: true });
  el.dispatchEvent(event);
}

function fillInput(query, value, trigger = 0) {
  return new Promise((resolve) => {
    setTimeout(() => {
      const e = document.querySelector(query);
      e.value = value;
      triggerEvent(e, 'input');
      if (trigger > 0) {
        triggerEvent(e, 'change');
      }
      resolve();
    }, 10);
  });
}

function clickButtonWithDelay(selector, delay = 100) {
  return new Promise((resolve) => {
    setTimeout(() => {
      const button = document.querySelector(selector);
      if (button) {
        button.click();
      }
      resolve();
    }, delay);
  });
}

main();
