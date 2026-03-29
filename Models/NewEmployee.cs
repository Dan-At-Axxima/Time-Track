using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TimeTrackerRepo.Models
{
    public class NewEmployee
    {

        public int EmployeeNumber { get; set; }

        public string FirstName { get; set; }

        public string LastName { get; set; }

        public string EMail { get; set; }

        public bool Active { get; set; }

        public double DDARates { get; set; }

        public double AxximaRates { get; set; }

        public int HoursPerDay { get; set; }
    }
}
