using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTrackerRepo.Data
{
    public class DisplayAssignments
    {
        [Column("Employee Number")]
        public int Id { get; set; }
        public int EmployeeNumber { get; set; }
        public string Client { get; set; }
        public string Project { get; set; }
        public string Activity { get; set; }

    }
}
