using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TimeTrackerRepo.Data
{
    public class NewEmployee
    {
        [Key]
        public int Id { get; set; }

        [Required]
        [Column("Employee Number")]
        public int EmployeeNumber { get; set; }

        [Required]
        [Column("First Name")]
        public string FirstName { get; set; }

        [Required]
        [Column("Last Name")]
        public string LastName { get; set; }

        [Required]
        public string EMail { get; set; }

        [Required]
        public bool Active { get; set; }

        [Required]
        public double DDARates { get; set; }

        [Required]
        public double AxximaRates { get; set; }
    }
}
