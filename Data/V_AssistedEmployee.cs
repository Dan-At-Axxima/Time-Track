using System.ComponentModel.DataAnnotations.Schema;
using System.ComponentModel.DataAnnotations;

namespace TimeTrackerRepo.Data
{
    public class V_AssistedEmployee
    {
        [Key]
        [Column("Employee Number")]
        public int EmployeeNumber { get; set; }
        [Column("First Name")]
        public string FirstName { get; set; }

        [Column("Last Name")]
        public string LastName { get; set; }
        public string EMail { get; set; }
        public bool Active { get; set; }

    }
}

