/**
* Javascript calendar plugin 
**/
function winCalendar(){
}

winCalendar.prototype.createEvent=function(event,location,matchStartTime){
	
		try{
		 cordova.exec(this.success, this.error, "Calendar","createEvent",[event,location,matchStartTime]);
		}catch(e){
		  alert("error message:"+e.message);
		}
}

winCalendar.prototype.success=function(msg){
       alert("success message:"+msg);
}

winCalendar.prototype.error=function(msg){
    alert("error message:"+msg);
}

winCalendar.install=function(){
  if (!window.plugins) {
    window.plugins = {};
  }

  window.plugins.calendar = new winCalendar();
  return window.plugins.calendar;
}

cordova.addConstructor(winCalendar.install);




