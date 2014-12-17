
var date = new Date();
var monthDays = new Array(31,28,31,30,31,30,31,31,30,31,30,31);



function getMonthDays(year, month) {
  if (month != 1)
     return monthDays[month];
  if (year % 4 == 0 && (year % 100 != 0 || (year % 100 == 0 && year % 400 == 0)))
  {
     return 29;
  }
  else
  {
     return 28;
  }
}

function setPrevYear() {
   var year = selectedDate.getFullYear();
   selectedDate.setDate(1);
   selectedDate.setMonth(0);
   selectedDate.setFullYear(year - 1);
}

function setNextYear() {
   var year = selectedDate.getFullYear();
   selectedDate.setDate(1);
   selectedDate.setMonth(0);
   selectedDate.setFullYear(year + 1);
}

function setPrevMonth() {
   var day = selectedDate.getDate();
   var month = selectedDate.getMonth();
   var year = selectedDate.getFullYear();
   month --;
   if (month < 0)
   {
      month = 11;
	  year--;
   }
   if (day > getMonthDays(year, month))
   {
      day = getMonthDays(year, month);
   }
   selectedDate.setDate(day);
   selectedDate.setMonth(month);
   selectedDate.setFullYear(year);
}

function setNextMonth() {
   var day = selectedDate.getDate();
   var month = selectedDate.getMonth();
   var year = selectedDate.getFullYear();
   month ++;
   if (month > 11)
   {
      month = 0;
	  year++;
   }
   if (day > getMonthDays(year, month))
   {
      day = getMonthDays(year, month);
   }
   selectedDate.setDate(day);
   selectedDate.setMonth(month);
   selectedDate.setFullYear(year);
}

function setd(d,m,y) {
   selectedDate.setDate(d);
   selectedDate.setMonth(m);
   selectedDate.setFullYear(y);
}

function calendar() {
  // Get the current date parameters.
  date = selectedDate;
  var day = date.getDate();
  var month = date.getMonth();
  var year = date.getFullYear();
  
 
  var months = [];
  
  for (i = 0; i < 12; i++) {
    months.push(scriptObject.gstr(i + 760));
  }
//  new Array('January','February','March','April','May','June','July','August','September','October','November','December');
  var weekDay = [];
  //new Array('Mo','Tu','We','Th','Fr','Sa','Su');
  for (i = 0; i < 6; i++) {
    weekDay.push(scriptObject.gstr(i + 151));
  }
  weekDay.push(scriptObject.gstr(150));
  var days_in_this_month = getMonthDays(year,month);

  // Create the basic Calendar structure.
  var calendar_html = '<center>';
  
    calendar_html += '<table cellspacing=8><tr><td class="calLinks">';
   cmx = selectedDate.getFullYear();
  for(ix = -3; ix <= 3; ix++)
  {
      if (ix > -3) {
		calendar_html += ' | ';
	  }
      if (ix != 0)
	  {
 		calendar_html += '<span onclick="setd(1,0,' + (ix + cmx) + ');calendar();" ';
	    calendar_html += ' class="linka"';
		calendar_html += '>';
	  }
	  else 
	  {
		calendar_html += '<span>';
	  }
	  calendar_html += (ix + cmx);
	  calendar_html += '</span>';
  } 

  calendar_html += '</td></tr><tr><td class="calLinks">';
  cmx = selectedDate.getMonth();
	cmy = selectedDate.getFullYear();
  for(ix = -2; ix <= 2; ix++)
  {
    nm = cmx + ix;
	ny = cmy;
	if (nm < 0) {
		nm += 12;
		ny--;
	} else if (nm > 11) {
		nm -= 12;
		ny++;
	}
      if (ix > -2) {
		calendar_html += ' | ';
	  }
      if (ix != 0)
	  {
		calendar_html += '<span onclick="setd(1, ' + nm + ', ' + ny + ');calendar();" ';
	    calendar_html += ' class="linka"';
		calendar_html += '>';
	  }
	  else 
	  {
		calendar_html += '<span>';
	  }
	  calendar_html += scriptObject.getMasaAbr(nm + 1);
	  calendar_html += '</span>';
  }
  calendar_html += '</td></tr></table>';

  
  calendar_html += '<table class="calendarTable">';
  calendar_html += '<tr>';
  calendar_html += '<td class="monthHead" colspan="7">' + months[month] + ' ' + year + '</td>';
  calendar_html += '</tr>';
  calendar_html += '<tr>';
  var first_week_day = new Date(year, month, 1).getDay();
  for(week_day= 0; week_day < 7; week_day++) {
    calendar_html += '<td class="weekDay">' + weekDay[week_day] + '</td>';
  }
  calendar_html += '</tr><tr>';

  // Fill the first week of the month with the appropriate number of blanks.
  for(week_day = 0; week_day < first_week_day; week_day++) {
    calendar_html += '<td> </td>';
  }
  week_day = first_week_day;
  for(day_counter = 1; day_counter <= days_in_this_month; day_counter++) {
    week_day %= 7;
    if(week_day == 0)
      calendar_html += '</tr><tr>';
    // Do something different for the current day.
    if(selectedDate.getDate() == day_counter && month == selectedDate.getMonth()
	   && year == selectedDate.getFullYear()) {
      calendar_html += '<td class="currentDay">' + day_counter + '</td>';
    } else {
      calendar_html += '<td class="monthDay" onclick="selectedDate.setDate(' + day_counter + ');calendar();"> ' + day_counter + ' </td>';
 }
    week_day++;
  }
  calendar_html += '</tr>';
  calendar_html += '</table>';
  
  
  calendar_html += '</center>';
  
  // Display the calendar.
  var elem = document.getElementById('thiscal');
  elem.innerHTML = calendar_html;
  //document.write(calendar_html);
}

