using Microsoft.EntityFrameworkCore;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTrackerRepo.Data
{
    [Keyless]
    public class Assignments
    {
        public Assignments()
        {
            hours=new List<Hours>();
        }
        [Column("Employee Number")]
        public int EmployeeNumber { get; set; }
        public string Client { get; set; }
        public string Project { get; set; }
        public string Activity { get; set; }


        [NotMapped]
        public List<Hours> hours { get; set; }
        [NotMapped]
        public List<string> titles { get; set; }

        [NotMapped]
        public string ClientProjectActivity { get { return (Client + "~" + Project + "~" + Activity); } }

        [NotMapped]
        public int Column { get; set; } = 0;

        [NotMapped]
        public string Hours { get; set; }

        [NotMapped]
        public string Comments { get; set; }


        [NotMapped]
        public int ActivityHashCode { get { return HashCode.Combine(Client.GetHashCode(), Project.GetHashCode(), Activity.GetHashCode()); } }

    }
}
