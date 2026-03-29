using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTrackerRepo.Models
{
    public class NewSettings
    {
        public string EmployeeNumber { get; set; }
        public DateTime StartDate { get; set; }   
        public DateTime EndDate { get; set; }
        public int HoursPerDay { get; set; }
    }
}
