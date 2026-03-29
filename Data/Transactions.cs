using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTrackerRepo.Data
{
    [Keyless]
    public class Transactions
    {
        public Transactions()
        {

        }
        public Transactions(int employee,string client,string project, string activity,DateTime date,string hours,string comment,string slipid)
        {
            EmployeeNumber = employee;
            Client = client;
            Project = project;
            Activity = activity;
            Date = date;
            Hours = hours;
            Comment = comment;
            SlipId = slipid;


        }
        private TimeSpan _numericHours;

        [Column("Employee Number")]
        public int EmployeeNumber { get; set; }
        public string Client { get; set; }
        public string Project { get; set; }
        public string Activity { get; set; }
        public DateTime Date { get; set; }
        public string Hours { get; set; }
        public string Comment { get; set; }
        public string SlipId { get; set; }



        

        [NotMapped]
        public string ClientProjectActivity { get { return (Client + "~" + Project + "~" + Activity); } }

        [NotMapped]
        public TimeSpan numericHours
        {
            get
            {
                _numericHours = convertHours(Hours);

                return _numericHours;
            }

            set
            {
                _numericHours = value;
                convertBackToHours(_numericHours);
            }
        }

        [NotMapped]
        public int column { get; set; } = 0;


        private void convertBackToHours(TimeSpan timeSpan)
        {
            string hour = timeSpan.Hours.ToString();
            string minutes = timeSpan.Minutes.ToString();
            this.Hours = hour + ":" + minutes;
        }


        private TimeSpan convertHours(string hours)
        {
            TimeSpan timeSpan = new TimeSpan(0, 0, 0);
            string numerichours = string.Empty;
            string minutes = string.Empty;
            string[] hoursandminutes = hours.Split(":");
            if (hoursandminutes != null && hoursandminutes.Length > 0)
            {
                if (hoursandminutes.Length > 1)
                {
                    numerichours = hoursandminutes[0];
                    minutes = hoursandminutes[1];
                }
                else
                {
                    minutes = hoursandminutes[0];
                }
                int hrs;
                int mins;
                Int32.TryParse(numerichours, out hrs);
                Int32.TryParse(minutes, out mins);
                TimeSpan test = new TimeSpan(hrs, mins, 0);
                timeSpan = timeSpan.Add(test);
            }
            return timeSpan;
        }

    }
}
