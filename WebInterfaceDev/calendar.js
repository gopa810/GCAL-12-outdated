
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

function calendar() {
  // Get the current date parameters.
  date = selectedDate;
  var day = date.getDate();
  var month = date.getMonth();
  var year = date.getFullYear();
  
 
  var months = new Array('January','February','March','April','May','June','July','August','September','October','November','December');
  var weekDay = new Array('Mo','Tu','We','Th','Fr','Sa','Su');
  var days_in_this_month = getMonthDays(year,month);

  // Create the basic Calendar structure.
  var calendar_html = '<center><table class="calendarTable">';
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

