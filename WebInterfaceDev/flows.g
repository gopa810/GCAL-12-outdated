
# definitions of pages

# this is main menu
page mainmenu
  source mainmenu
  action calc
    goto mainmenu-calc
  end action
  action set
    goto mainmenu-set
  end action
  action geo
    goto mainmenu-geo
  end action
  action help
    goto help
  end action
end page

# help page

page help
  source help
end page

# this is menu for calculation

page mainmenu-calc
  source mainmenu-calc
  action today
    goto today
  end action
  action nextfest
    goto nextfest
  end action
  action search
    goto dlg-find
  end action
  action calendar
    set $nextpage dlg-startdate-cal
    goto dlg-enterloc
  end action
  action coreevents
    set $nextpage dlg-startdate-core
    goto dlg-enterloc
  end action
  action appday
    set $nextpage dlg-startdate-app
    goto dlg-enterloc
  end action
  action masalist
    set $nextpage dlg-startyear
    goto dlg-enterloc
  end action
  action cal2cal
    set $nextpage dlg-startdate-cal2cal
    goto dlg-enterloc
  end action
  action cal2core
    set $nextpage dlg-startdate-cal2core
    goto dlg-enterloc
  end action
end page

page today
  source today
end page

page nextfest
  source nextfest
end page

page dlg-find
  source dlg-find
end page

page dlg-enterloc
  source dlg-enterloc
  action mylocation
    goto $nextpage
  end action
  action full
    goto dlg-locfull
  end action
  action select
    goto dlg-selloc
  end action
end page

page dlg-selloc
  source dlg-selloc
  action next
    goto $nextpage
  end action
end page

page dlg-locfull
  source dlg-locfull
  action next
    goto dlg-country
  end action
end page

page dlg-country
  source dlg-country
  action next
    goto dlg-enterlongitude
  end action
end page

page dlg-enterlongitude
  source dlg-enterlongitude
  action next
    goto dlg-enterlatitude
  end action
end page

page dlg-enterlatitude
  source dlg-enterlatitude
  action next
    goto dlg-selcoutz
  end action
end page

page dlg-selcoutz
  source dlg-selcoutz
  action next
    goto $nextpage
  end action
end page

page dlg-startdate-cal
  source dlg-startdate
  action next
    set $nextpage calendar
    goto dlg-endperiod-x
  end action
end page

page dlg-startdate-core
  source dlg-startdate
  action next
    set $nextpage coreevents
    goto dlg-endperiod-x
  end action
end page

page dlg-startdate-app
  source dlg-startdate
  action next
    goto dlg-time-app
  end action
end page

page dlg-startdate-cal2cal
  source dlg-startdate
  action next
    set $nextpage cal2locs
    goto dlg-endperiod-x
  end action
end page

page dlg-startdate-cal2core
  source dlg-startdate
  action next
    set $nextpage calcore
    goto dlg-endperiod-x
  end action
end page

page dlg-endperiod-x
  source dlg-endperiod
  action next
    goto $nextpage
  end
end page

page dlg-startyear
  source dlg-startyear
  action next
    goto dlg-yearcount
  end action
end page

page dlg-yearcount
  source dlg-yearcount
  action next
    goto masalist
  end action
end page

page dlg-time-app
  source dlg-time
  action next
    goto appday
  end action
end page

page calendar
  source calendar
end page

page calcore
  source calcore
end page

page cal2locs
  source cal2locs
end page

page masalist
  source masalist
end page

page coreevents
  source coreevents
end page

page appday
  source appday
end page

page mainmenu-set
  source mainmenu-set
  action disp
    goto set-disp
  end action
  action general
    goto set-general
  end action
  action startpage
    goto set-startpage
  end action
  action mylocation
    set $nextpage mainmenu-set
	goto dlg-enterloc
  end action
  action languages
    goto languages
  end action
end page

page set-disp
  source set-disp
  action cal
    goto set-disp-cal
  end action
  action core
    goto set-disp-core
  end action
  action app
    goto set-disp-app
  end action
  action masa
    goto set-disp-masa
  end action
  action today
    goto set-disp-today
  end action
  action nextfest
    goto set-disp-nextfest
  end action
end page

page set-disp-cal
  source set-disp-cal
  action ann
    goto set-disp-cal-ann
  end action
  action hdt
    goto set-disp-cal-hdt
  end action
  action set-disp
    goto set-disp
  end action
end page

page set-disp-cal-ann
  source set-disp-cal-ann
  action set-disp-cal
    goto set-disp-cal
  end action
end page

page set-disp-cal-hdt
  source set-disp-cal-hdt
  action set-disp-cal
    goto set-disp-cal
  end action
end page

page set-disp-core
  source set-disp-core
  action sort
    goto set-disp-core-sort
  end action
end page


page set-disp-core-sort
  source set-disp-core-sort
  action set-disp-core
   goto set-disp-core
  end action
end page

page set-disp-app
  source set-disp-app
end page

page set-disp-masa
  source set-disp-masa
end page

page set-disp-today
  source set-disp-today
end page


page set-disp-nextfest
  source set-disp-nextfest
end page

page set-startpage
  source set-startpage
end page

page set-general
  source set-general
  action catur
    goto set-gen-catur
  end action
  action fdow
    goto set-gen-fdow
  end action
  action mnf
    goto set-gen-mnf
  end action
  action fast
    goto set-gen-fast
  end action
  action tformat
    goto set-gen-tformat
  end action
  action snf
    goto set-gen-snf
  end action
end page


page set-gen-catur
  source set-gen-catur
end page

page set-gen-fdow
  source set-gen-fdow
end page

page set-gen-mnf
  source set-gen-mnf
end page

page set-gen-fast
  source set-gen-fast
end page

page set-gen-tformat
  source set-gen-tformat
end page

page set-gen-snf
  source set-gen-snf
end page

page languages
  source languages
end page

page mainmenu-geo
  source mainmenu-geo
  action cities
    goto geo-cities
  end action
  action countries
    goto geo-countries
  end action
  action events
    goto geo-events
  end action
  action tzones
    goto geo-tzones
  end action
end page

page geo-cities
  source geo-cities
  action add
	set $nextpage dlg-savelocation
    goto dlg-locfull-c
  end action
  action edit
    set $nextpage dlg-editlocation-ce
	goto dlg-selloc-x
  end action
  action remove
    set $nextpage dlg-dellocation-ce
	goto dlg-selloc-x
  end action
end page

page dlg-selloc-x
  source dlg-selloc
  action next
    goto $nextpage
  end action
end page

page dlg-locfull-c
  source dlg-locfull
  action next
    goto dlg-country-c
  end action
end page

page dlg-country-c
  source dlg-country
  action next
    goto dlg-enterlongitude-c
  end action
end page

page dlg-enterlongitude-c
  source dlg-enterlongitude
  action next
    goto dlg-enterlatitude-c
  end action
end page

page dlg-enterlatitude-c
  source dlg-enterlatitude
  action next
    goto dlg-selcoutz-c
  end action
end page

page dlg-selcoutz-c
  source dlg-selcoutz
  action next
    goto dlg-savelocation-c
  end action
end page

page dlg-savelocation-c
  source dlg-savelocation
  action save
  end action
end page

page dlg-editlocation-ce
  source dlg-editlocation
  action save
  end action
end page

page dlg-dellocation-ce
  source dlg-dellocation
  action delete
  end action
end page

page geo-countries
  source geo-countries
  action add
    goto dlg-newcountry
  end action
  action edit
    goto dlg-country-coe
  end action
  action delete
	goto dlg-country-cod
  end action
end page

page dlg-country-coe
  source dlg-country
  action next
    goto dlg-renamecountry
  end action
end page

page dlg-country-cod
  source dlg-country
  action next
    goto dlg-confirm-del-country
  end action
end page

page dlg-newcountry
  source dlg-newcountry
  action next
    goto dlg-editcoutz
  end action
end page

page dlg-editcoutz
  source dlg-editcoutz
end page

page dlg-renamecountry
  source dlg-renamecountry
end page

page dlg-confirm-del-country
  source dlg-confirm-del-country
end page

page geo-events
  source geo-events
  action add
    goto dlg-editevent
  end action
  action edit
    goto dlg-findevent-ee
  end action
  action delete
    goto dlg-findevent-ed
  end action
end page

page dlg-findevent-ee
  source dlg-findevent
  action next
    goto dlg-editevent
  end action
end page

page dlg-findevent-ed
  source dlg-findevent
  action next
    goto dlg-delevent
  end action
end page

page dlg-editevent
  source dlg-editevent
end page

page dlg-delevent
  source dlg-delevent
end page

page geo-tzones
  source geo-tzones
  action add
    goto dlg-edittimezone
  end action
  action edit
    goto dlg-findtz-te
  end action
  action delete
    goto dlg-findtz-td
  end action
end page

page dlg-edittimezone
  source dlg-edittimezone
end page

page dlg-findtz-te
  source dlg-findtz
  action next
    goto dlg-edittimezone
  end action
end page

page dlg-findtz-td
  source dlg-findtz
  action next
    goto dlg-deltzone
  end action
end page

page dlg-deltzone
  source dlg-deltzone
end page

