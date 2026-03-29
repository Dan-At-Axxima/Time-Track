using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTrackerRepo.Models
{
    public class BatchUpdateValues
    {
        public string UserName { get; set; }
        public string ClientProjectActivity { get; set; }
        public string Date { get; set; }
        public string Comment { get; set; }
        public string Hours { get; set; }
        public string Minutes { get; set; }

        public BatchUpdateValues()
        {

        }
        public BatchUpdateValues(BatchUpdateValues values)
        {
            UserName = values.UserName;
            ClientProjectActivity = values.ClientProjectActivity;
            Comment = values.Comment;
            Date = values.Date;
            Hours = values.Hours;
            Minutes = values.Minutes;
        }
    }
}
