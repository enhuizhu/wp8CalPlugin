using Microsoft.Phone.Tasks;
using Microsoft.Phone.UserData;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using WPCordovaClassLib.Cordova;
using WPCordovaClassLib.Cordova.Commands;
using WPCordovaClassLib.Cordova.JSON;

namespace Cordova.Extension.Commands
{
    public class Calendar : BaseCommand
    {

        /**
        * variables which will be used to search the similar event 
        **/
        public string location;
        public string eventName;
        public DateTime startTime;


        public void createEvent(string options)
        {

            string[] optVals = JsonHelper.Deserialize<string[]>(options);
            /**
            * get the start time of the event
            **/
            string startTimeStr = optVals[2];

            int[] dateTimeArr = getTimeArr(startTimeStr);


            this.startTime = new DateTime(dateTimeArr[0], dateTimeArr[1], dateTimeArr[2], dateTimeArr[3], dateTimeArr[4], dateTimeArr[5]);

            /**
            * get the name of the match
            **/
            this.eventName = optVals[0];

            /**
            * get the channel info of the match 
            **/
            string eventLocation = optVals[1];

            this.location = eventLocation;


            /**
            * should check if same eent already in the calendar
            **/

            Microsoft.Phone.UserData.Appointments appts = new Appointments();


            /**
            * when search completed should call searchComplete function
            **/
            appts.SearchCompleted += new EventHandler<AppointmentsSearchEventArgs>(AppointmentsSearchCompleted);

            /**
            * start asynchronous search
            **/
            appts.SearchAsync(this.startTime, this.startTime.AddHours(1), 20, "Search Appointments");

        }

        void AppointmentsSearchCompleted(object sender, AppointmentsSearchEventArgs e)
        {

            /**
            * variables to trace if there is duplicate event in the calendar
            **/
            bool foundApp = false;

            foreach (Appointment appt in e.Results)
            {
                Debug.WriteLine(appt.Subject);
                if (appt.Subject == this.eventName && this.location == appt.Location)
                {
                    /*find the application, pop up the appointment already in the calendar*/
                    DispatchCommandResult(new PluginResult(PluginResult.Status.ERROR, "Bu maç daha önce takvime eklendi!"));
                    //MessageBox.Show("Bu maç daha önce takvime eklendi!");
                    foundApp = true;

                    break;
                }

            }


            if (!foundApp)
            {

                SaveAppointmentTask saveAppointmentTask = new SaveAppointmentTask();
                saveAppointmentTask.StartTime = this.startTime;
                saveAppointmentTask.EndTime = this.startTime.AddHours(1);
                saveAppointmentTask.Subject = this.eventName;
                saveAppointmentTask.Location = this.location;
                saveAppointmentTask.IsAllDayEvent = false;
                saveAppointmentTask.Reminder = Reminder.FifteenMinutes;
                saveAppointmentTask.AppointmentStatus = Microsoft.Phone.UserData.AppointmentStatus.Busy;
                saveAppointmentTask.Show();


            }




        }




        /**
        * function to change the start time string into array
        **/
        public int[] getTimeArr(string timeStr)
        {
            int[] timeArr = new int[6];
            string[] dateTimeArr = timeStr.Split(' ');
            string[] dateArr = dateTimeArr[0].Split('-');
            string[] detailTimeArr = dateTimeArr[1].Split(':');
            timeArr[0] = Convert.ToInt32(dateArr[0]);//year
            timeArr[1] = Convert.ToInt32(dateArr[1]);//month
            timeArr[2] = Convert.ToInt32(dateArr[2]);//date
            timeArr[3] = Convert.ToInt32(detailTimeArr[0]);//hour
            timeArr[4] = Convert.ToInt32(detailTimeArr[1]);//mins
            timeArr[5] = Convert.ToInt32(detailTimeArr[2]);//seconds
            return timeArr;
        }






    }
}
