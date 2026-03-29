using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace TimeTrackerRepo.Models
{
    public class EmployeeAndRates
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
        [NotMapped]
        [Display(Name = "HoursPerDay")]
        public int HoursPerDay { get; set; } 

    }
    

}
