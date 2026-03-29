using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTrackerRepo.Data
{
    public class Activities
    {
        [Key]
        public string Client { get; set; }
        public string Project { get; set; }
        public string Activity { get; set; }
        public double Multiple { get; set; }
        public int AxximaCompanyCodes { get; set; }

        [NotMapped]
        public string ClientProjectActivity { get { return (Client.Trim() + "~" + Project.Trim() + "~" + Activity.Trim()); } }

        [NotMapped]
        public int ActivityHashCode  { get { return HashCode.Combine(Client.Trim().GetHashCode(), Project.Trim().GetHashCode(),Activity.Trim().GetHashCode()) ; } }
    }
}
