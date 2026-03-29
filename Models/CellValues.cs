using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTrackerRepo.Data;

namespace TimeTrackerRepo.Models
{
    public class CellValues
    {
        private TimeTrackerContext _dbContext;

        public string Client { get; set; }
        public string Project { get; set; }
        public string Activity { get; set; }
        public string? User { get; set; }
        public string Comment { get; set; }
        public string Time { get; set; }
        public string Date { get; set; }

        public CellValues(TimeTrackerContext dbContext, string client, string project, string activity, string user, string date)
        {
            _dbContext = dbContext;
            Client = client;
            Project = project;
            Activity = activity;
            User = user;
            Date = date;
        }

        public CellValues(string client, string project, string activity, string user, string date)
        {

        }

        public UpdateValues GetCellValues()
        {
            var date = DateTime.ParseExact(Date, "yyyy-MM-dd", CultureInfo.InvariantCulture);

            DateTime.TryParse(Date, out date);
            int user;
            Int32.TryParse(User, out user);
            var result = _dbContext.Transactions.Where(u => u.EmployeeNumber == user && u.Client == Client && u.Project == Project
                                && u.Activity == Activity && u.Date.Year == date.Year && u.Date.Month == date.Month && u.Date.Day == date.Day).FirstOrDefault();
            var values = new UpdateValues();
            if(result != null)
            {
                var time = result.Hours.Trim();
                var comment = result.Comment;
                values = new UpdateValues(Client, Project, Activity, User, comment, time, date.Year.ToString(), date.Month.ToString(), date.Day.ToString());
            }
            return values;

        }
    }
}
