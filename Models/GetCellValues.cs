using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTrackerRepo.Models
{
    public class GetCellValues
    {
        public string Client { get; set; }
        public string Project { get; set; }
        public string Activity { get; set; }
        public string Date { get; set; }

        public GetCellValues()
        {

        }

    }
}
