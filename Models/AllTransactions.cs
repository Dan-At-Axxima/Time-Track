using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTrackerRepo.Data;

namespace TimeTrackerRepo.Models
{
    public class AllTransactions
    {
        public List<FullMonth> hours { get; set; }
        public List<FullMonth> comments { get; set; }

        public List<Comments> CommentList { get; set; }
        public string[] RowTitles { get; set; }
        public int NonchargeableRowCount { get; set; }   
        public int RowCount { get; set; }
        public string hoursStringArray { get; set; }
        public string hoursString { get; set; }

        public string commentsString { get; set; }

    }
}
