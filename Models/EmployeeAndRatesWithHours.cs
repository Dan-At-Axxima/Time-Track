using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TimeTrackerRepo.Models
{
    public class EmployeeAndRatesWithHours
    {
        [Key]
        [Column("Employee Number")]
        [Display(Name = "Employee Number")]
        public int EmployeeNumber { get; set; }
        [Column("First Name")]
        [Display(Name = "First Name")]
        public string FirstName { get; set; }
        [Column("Last Name")]
        [Display(Name = "Last Name")]
        public string LastName { get; set; }
        public string Email { get; set; }
        public bool Active { get; set; }
        [Display(Name = "Rates")]
        public double DDARates { get; set; }
        public double AxximaRates { get; set; }
        [Display(Name = "HoursPerDay")]
        public int HoursPerDay { get; set; }

    }
}
