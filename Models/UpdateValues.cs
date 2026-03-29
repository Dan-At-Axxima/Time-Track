using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTrackerRepo.Models
{
    public class UpdateValues
    {
        public string Client { get; set; }
        public string Project { get; set; }
        public string Activity { get; set; }
        public string? User { get; set; }
        public string Comment { get; set; }
        public string Time { get; set; }
        public string Year { get; set; }
        public string Month { get; set; }
        public string Day { get; set; }

        public UpdateValues()
        {

        }
        public UpdateValues(string client, string project, string activity, string user,  string comment, string time, string year, string month, string day)
        {
            Client = client;
            Project = project;
            Activity = activity;
            User = user;
            Comment = comment;
            Time = time;
            Year = year;
            Month = month;
            Day = day;

        }
    }


}
