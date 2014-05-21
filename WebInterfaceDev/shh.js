var lastId = '';

function shh(highIndex, elemId) {

	var highColor2 = '#bbbbbb';

    var elem = document.getElementById(elemId);
	var isSelectedElem = 0;
	if (elem.hasAttribute('seld') && elem.getAttribute('seld') == 1) {
		isSelectedElem = 1;
	}
	if (highIndex == 2) {
	    if (isSelectedElem == 1) {
			elem.style.backgroundColor = '#dddddd';
			elem.setAttribute('seld', 0);
		} else {
			elem.style.backgroundColor = '#bbbbbb';
			elem.setAttribute('seld', 1);
		}
	}
	else if (highIndex == 1) {
	    if (isSelectedElem == 0) {
			elem.style.backgroundColor = '#dddddd';
		}
	}
	else if (highIndex == 0) {
	    if (isSelectedElem == 0) {
			elem.style.backgroundColor = '';
		}
	}

}


function writeChoice(text,myId,targetUrl)
{
   var str = '<tr height=32px><td style=\'cursor:pointer;border:1px solid black\'';
   str += ' onmouseover=\'shh(1,"' + myId + '")\'';
   str += ' onmouseout=\'shh(0,"' + myId + '")\'';
   str += ' onclick=\'window.location.href="' + targetUrl + '"\'';
   str += ' id=\'' + myId + '\'>';
   str += text;
   str += '</td></tr>';
   
   document.write(str);
}

function writeComboChoice(title, valText, myId, targetUrl)
{
	var str;
	
	str = '<tr height=32px>\n';
	str += '<td style=\'cursor:pointer;\' onclick=\'window.location.href="' + targetUrl + '"\' id=\'' + myId + '\'>\n';
	str += '<p><span style=\'font-weight:bold;font-size:120%\'>' + title + '</span>\n';
	str += '<br><span style=\'color:#aaa\'>' + valText + '</span>\n';
	str += '</td>\n';
	str += '<td  style=\'cursor:pointer;\' onclick=\'window.location.href="' + targetUrl + '"\' id=\'' + myId + '\' width=24pt><img src="sarrow-to-right.png"></td>\n';
	str += '</tr>\n';

	document.write(str);
}

function setComboChoiceSelection(myId, newVal)
{
	var elem = document.getElementById(myId);
	elem.src = (newVal != 0 ? "checked-yes.png" : "checked-no.png");
}

function setNewComboChoice(myId)
{
  if (lastId != '')
	setComboChoiceSelection(lastId, 0);
  setComboChoiceSelection(myId, 1);
  lastId = myId;
}


function writeComboChoiceValue(title, valText, myId, val)
{
	var str;
	
	str = '<tr height=32px id=\'row' + myId + '\'';
    str += ' onmouseover=\'shh(1,"row' + myId + '")\'';
    str += ' onmouseout=\'shh(0,"row' + myId + '")\'';
	str += '>\n';
	str += '<td  style=\'cursor:pointer;\' onclick=\'setNewComboChoice("' + myId + '")\' width=24pt>';
	str += '<img id = "' + myId + '" src="';
	str += (val != 0 ? "checked-yes.png" : "checked-no.png");
	str += '"></td>\n';
	str += '<td style=\'cursor:pointer;\' onclick=\'setNewComboChoice("' + myId + '")\'>\n';
	str += '<p><span style=\'font-weight:bold;font-size:120%\'>' + title + '</span>\n';
	str += '<br><span style=\'color:#aaa\'>' + valText + '</span>\n';
	str += '</td>\n';
	str += '</tr>\n';

	if (val != 0)
	   lastId = myId;
	document.write(str);
}


function incrementCounterChoice(myId, valDif)
{
	var elem = document.getElementById(myId);
	var val = valDif;
	val += Number(elem.innerText);
	if (val < 0)
		val = 0;
	elem.innerText = val;
}

function writeCounterChoice(title, valText, myId)
{
	var str;
	
	str = '<tr height=32px>\n';
	str += '<td style=\'cursor:pointer;\'>\n';
	str += '<p><span style=\'font-weight:bold;font-size:120%\'>' + title + '</span>\n';
	str += '<br><span style=\'color:#aaa\'>' + valText + '</span>\n';
	str += '</td>\n';
	str += '<td width=24pt>';
	
	
	str += '<table>';
	str += '<tr>';
	str += '<td style=\'text-align:right;font-size:24pt\' id=\'' + myId + '\'>1</td>';
	str += '<td width=32pt class=\'noselect\' style=\'text-align:center;cursor:pointer\'';
	str += ' onclick=\'incrementCounterChoice("' + myId + '", 1);\'>+1</td>';
	str += '<td width=32pt class=\'noselect\' style=\'text-align:center;cursor:pointer\'';
	str += ' onclick=\'incrementCounterChoice("' + myId + '", -1);\'>-1</td>';
	str += '</tr></table>';


	str += '</tr>\n';

	document.write(str);
}


function writeCheckBox(title, subtitle, myId)
{
  var str;
  
  str = '<tr height=32px>\n';
  str += '<td style=\'cursor:pointer;\' onclick=\'toogleSwitch("' + myId + '")\'>\n';
  str += '<p><span style=\'font-weight:bold;font-size:120%\'>' + title + '</span>';
  str += '<br><span style=\'color:#aaa\'>' + subtitle + '</span>';
  str += '</td>\n';
  str += '<td style=\'cursor:pointer;\' width=24pt  onclick=\'toogleSwitch("' + myId + '")\'><img id=\'' + myId + '\' src="switch-off.png"></td>\n';
  str += '</tr>\n';
  
  document.write(str);
}



function getSwitchValue(switchId)
{
  var elem = document.getElementById(switchId);
  if (elem.hasAttribute('swval') && elem.getAttribute('swval') == 1)
  {
	return 1;
  }
  return 0;
}

function setSwitchValue(switchId, switchValue)
{
  var elem = document.getElementById(switchId);
  elem.setAttribute('swval', switchValue);
  if (switchValue == 0)
  {
	elem.src = "switch-off.png";
  }
  else
  {
	elem.src = "switch-on.png";
  }

}

function toogleSwitch(switchId)
{
    var v = getSwitchValue(switchId);
	setSwitchValue(switchId, 1 - v);
}



