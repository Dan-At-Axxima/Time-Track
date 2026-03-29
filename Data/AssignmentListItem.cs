using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTrackerRepo.Data
{
    public class AssignmentListItem
    {
        public int Id { get; set; }
        public string Client { get; set; }
        public string Project { get; set; }
        public string Activity { get; set; }
    }
}
