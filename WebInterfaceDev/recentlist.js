function updateRecentList()
{
	var i;
	var m = Number(scriptObject.getRecentLocationsCount());
	var str = '<table cellpadding=6 cellspacing=0>';

	for(i = 0; i < m; i++)
	{
		var line = scriptObject.getRecentLocation(i).split('<tr>');
		if (line.length == 4)
		{
			if (i > 0)
			{
				str += "<tr><td colspan=2><hr></td></tr>";
			}
			str += "<tr style='cursor:pointer' id='rp" + i.toString() + "' ";
		    str += ' onmouseover=\'shh(1,"rp' + i.toString() + '")\'';
		    str += ' onmouseout=\'shh(0,"rp' + i.toString() + '")\'';
		    str += ' onclick=\'saveString("recentIndex","' + line[3] + '");runAction("recent");\'';
			str += "><td>";
			iconNo = Number(line[0]);
			icon = "globe.png";
			if (iconNo == 1) {
				icon = "home.png";
			} else if (iconNo == 2) {
				icon = "select_item.png";
			}
			str += "<img src=\"" + scriptObject.getDir() + "/images/" + icon + "\" width=32 height=32>";
			str += "</td>";
			str += "<td><b>" + line[1] + "</b><br>" + line[2] + "</td>";
			str += "</tr>";
		}
	}
	
	str += '</table>';
	
	if (m < 1)
	{
		document.getElementById('recent').style.display = 'none';
	}
	else
	{
		document.getElementById('recent').style.display = 'block';
		document.getElementById('recentList').innerHTML = str;
	}
}