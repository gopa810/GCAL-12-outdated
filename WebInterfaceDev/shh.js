var lastId = '';

var scriptObject = window.external;

function absm(a)
{
    if (a < 0)
	   return - Number(a);
	return Number(a);
}

function shh(highIndex, elemId) {

	var highColor2 = '#bbbbbb';

    var elem = document.getElementById(elemId);
	var isSelectedElem = 0;
	if (elem.getAttribute('seld') && elem.getAttribute('seld') == 1) {
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
   str += ' onclick=\'goPage("' + targetUrl + '")\'';
   str += ' id=\'' + myId + '\'>';
   str += text;
   str += '</td></tr>';
   
   document.write(str);
}

function writeChoiceWithAction(text,myId,targetAction)
{
   var str = '<tr height=32px><td style=\'cursor:pointer;border:1px solid black\'';
   str += ' onmouseover=\'shh(1,"' + myId + '")\'';
   str += ' onmouseout=\'shh(0,"' + myId + '")\'';
   str += ' onclick=\'' + targetAction + '\'';
   str += ' id=\'' + myId + '\'>';
   str += text;
   str += '</td></tr>';
   
   document.write(str);
}

function writeComboChoice(title, valText, myId, targetUrl)
{
	var str;
	
	str = '<tr height=32px>\n';
	str += '<td style=\'cursor:pointer;\' onclick=\'goPage("' + targetUrl + '")\' id=\'' + myId + '\'>\n';
	str += '<p><span style=\'font-weight:bold;font-size:120%\'>' + title + '</span>\n';
	str += '<br><span style=\'color:#aaa\'>' + valText + '</span>\n';
	str += '</td>\n';
	str += '<td  style=\'cursor:pointer;\' onclick=\'goPage("' + targetUrl + '")\' id=\'' + myId + '\' width=24pt><img src="' + getDir() + '/sarrow-to-right.png"></td>\n';
	str += '</tr>\n';

	document.write(str);
}

function setComboChoiceSelection(myId, newVal)
{
	var elem = document.getElementById(myId);
	if (elem != null)
		elem.src = (newVal != 0 ? (getDir() + "/checked-yes.png") : (getDir() + "/checked-no.png"));
}

function setNewComboChoice(myId)
{
  if (lastId != '')
	setComboChoiceSelection(lastId, 0);
  setComboChoiceSelection(myId, 1);
  lastId = myId;
}

function getStringComboChoiceValue(title, valText, myId, val)
{
	var str;
	
	str = '<tr height=32px id=\'row' + myId + '\'';
    str += ' onmouseover=\'shh(1,"row' + myId + '")\'';
    str += ' onmouseout=\'shh(0,"row' + myId + '")\'';
	str += '>\n';
	str += '<td  style=\'cursor:pointer;\' onclick=\'setNewComboChoice("' + myId + '")\' width=24pt>';
	str += '<img id = "' + myId + '" src="';
	str += getDir() + "/";
	str += (val != 0 ? "checked-yes.png" : "checked-no.png");
	str += '"></td>\n';
	str += '<td style=\'cursor:pointer;\' onclick=\'setNewComboChoice("' + myId + '")\'>\n';
	str += '<p><span style=\'font-weight:bold;font-size:120%\'>' + title + '</span>\n';
	str += '<br><span style=\'color:#aaa\'>' + valText + '</span>\n';
	str += '</td>\n';
	str += '</tr>\n';

	if (val != 0)
	   lastId = myId;
	return str;
}

function setDisplay(elemId, dispValue)
{
	var elem = document.getElementById(elemId);
	if (elem != null)
	{
		elem.style.display = dispValue;
	}
}

function setVisibility(elemId, dispValue)
{
	var elem = document.getElementById(elemId);
	if (elem != null)
	{
		elem.style.visibility = dispValue;
	}
}


function writeComboChoiceValue(title, valText, myId, val)
{
	document.write(getStringComboChoiceValue(title, valText, myId, val));
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
  str += '<p><span style=\'font-size:120%\'>' + title + '</span>';
  str += '<br><span style=\'color:#aaa\'>' + subtitle + '</span>';
  str += '</td>\n';
  str += '<td style=\'cursor:pointer;\' width=24pt  onclick=\'toogleSwitch("' + myId + '")\'><img id=\'' + myId + '\' src="'  + getDir() + '/switch-off.png"></td>\n';
  str += '</tr>\n';
  
  document.write(str);
}

function getElementValue(elemId)
{
    var elc = document.getElementById(elemId);
    return elc.value;
}

function setElementValue(elemId, val)
{
    var elc = document.getElementById(elemId);
	if (val != null)
		elc.value = val;
}

function getElementInnerText(elemId)
{
    var elc = document.getElementById(elemId);
    return elc.innerText;
}

function setElementInnerText(elemId, val)
{
    var elc = document.getElementById(elemId);
	if (val != null)
		elc.innerText = val;
}


function getSwitchValue(switchId)
{
  var elem = document.getElementById(switchId);
  if (elem.getAttribute('swval') == 1)
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
	elem.src = getDir() + "/switch-off.png";
  }
  else
  {
	elem.src = getDir() + "/switch-on.png";
  }

}

function toogleSwitch(switchId)
{
    var v = getSwitchValue(switchId);
	setSwitchValue(switchId, 1 - v);
}

function writeNavigStart()
{
    document.write("<div class=\"menuBarDiv\">" 
	     + "<table class=\"menuBar\" align=center cellspacing=2 cellpadding=0><tr>");
}

function writeNavigItem(name, action)
{
   document.write("<td><button type=\"button\" class=\"menuItem\" onclick='" + action + "'>" + name + "</button></td>");
}

function writeNavigItemWithId(name, action, myid)
{
   document.write("<td id=\"" + myid + "\"><button type=\"button\" class=\"menuItem\" onclick='" + action + "'>" + name + "</button></td>");
}

function writeNavigEnd()
{
    document.write("</tr></table></div>");
}

function goPage(page)
{
    window.external.goPage(page);
}

function goNext()
{
	window.external.goNext();
}

function goBack()
{
	window.external.goBack();
}

function addFutureFile(file)
{
    window.external.addFutureFile(file);
}

function addFutureFile2(file,title)
{
    window.external.addFutureFile2(file,title);
}

function insertFutureFile2(file,title)
{
    window.external.insertFutureFile2(file,title);
}

function clearHistory()
{
	window.external.clearHistory();
}

function setCurrTitle(t)
{
	window.external.setCurrTitle(t);
}

function saveString(key,value)
{
	window.external.saveString(key,value);
}

function loadString(key)
{
	return window.external.getString(key);
}

function saveInt(key, value)
{
	window.external.saveInt(key,value);
}

function getUri(fileName)
{
	window.external.getUri(fileName);
}

function getDir()
{
	return window.external.getDir();
}

function setUserDefaultsInt(key,val)
{
    window.external.setUserDefaultsInt(key,val);
}

function getUserDefaultsInt(key)
{
    return window.external.getUserDefaultsInt(key);
}

function setUserDefaultsBool(key,val)
{
    window.external.setUserDefaultsBool(key,val);
}

function getUserDefaultsBool(key)
{
    return window.external.getUserDefaultsBool(key);
}

function resetToday()
{
	window.external.resetToday();
}

function todayGoPrev()
{
    window.external.todayGoPrev();
}

function todayGoNext()
{
    window.external.todayGoNext();
}

function todayToday()
{
    window.external.todayToday();
}

function runAction(action)
{
    window.external.runAction(action);
}

