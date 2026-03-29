using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace TimeTrackerRepo.Data
{
    public class Employee
    {
        [Key]
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

    }
}

