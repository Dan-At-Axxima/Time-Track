using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTrackerRepo.Data
{
    public class NewTransaction
    {
        public NewTransaction()
        {

        }

        public string Activity { get; set; }
        public string Comment { get; set; }
        public DateTime Date { get; set; }
        public int UnitsInStock { get; set; }
        public Boolean Discontinued { get; set; }
    }
}
