using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTrackerRepo.Models
{
    public class SaveCellValues
    {
        public string Client { get; set; }
        public string Project { get; set; }
        public string Activity { get; set; }
        public string User { get; set; }
        public string Comment { get; set; }
        public string Time { get; set; }
        public string Date { get; set; }
        public SaveCellValues(string client, string project, string activity, string user, string comment, string time, string date)
        {
            Client = client;
            Project = project;
            Activity = activity;
            User = user;
            Comment = comment;
            Time = time;
            Date = date;
        }
    }
}
