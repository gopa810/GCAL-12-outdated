var searchLastPart = "{%string findstr}";
var searchNeedUpdate = 0;

function checkName()
{
	var newValue = getElementValue('textField0');
	if (newValue != searchLastPart)
	{
		searchLastPart = newValue;
		searchNeedUpdate = 1;
		scriptObject.searchResultString(searchLastPart);
	}
	
	if (scriptObject.isSearching() && searchNeedUpdate == 1)
	{
		if (scriptObject.isSearchFinished())
		{
			updateFindList();
			searchNeedUpdate = 0;
		}
		else
		{
			document.getElementById('findList').innerHTML = "<p align=center><i>" + scriptObject.gstr(1136) + "</i></p>";
		}
	}
}

setInterval(checkName, 300);

function updateFindList()
{
	var i;
	var m = scriptObject.searchResultsCount();
	var str = '<table>';

	for(i = 0; i < m; i++)
	{
		var line = scriptObject.getSearchResult(i).split('<part>');
		if (line.length == 4)
		{
			if (i > 0)
			{
				str += "<tr><td colspan=2><hr></td></tr>";
			}
			str += "<tr><td>";
			var lines = line[2].split('<tr>');
			str += "<p><span class=head2>" + line[0] + "</span><br><span class=shadowSubtitle>" + line[1] + "</span>";
			for(j = 0; j < lines.length; j++)
			{
				var parts = lines[j].split('<r>');
				if (parts.length == 3)
				{
					str += "<br>";
					str += parts[0] + "<span class=highlightFinding>" + parts[1] + "</span>" + parts[2];
				}
			}
			str += "</td><td>";
			str += "<button type=button class=menuItem onclick=\"onSave();" + line[3] + "\">";
			str += scriptObject.gstr(1135) + " &#10217";
			str += "</button>";
			str += "</td></tr>";
		}
	}
	
	str += '</table>';
	
	if (m < 1)
		str = "<p align=center><i>" + scriptObject.gstr(1137) + "</i></p>";

	document.getElementById('findList').innerHTML = str;
}
