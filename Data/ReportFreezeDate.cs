using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations.Schema;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTrackerRepo.Data
{
    public class ReportFreezeDate
    {
        public DateTime FrozenDate { get; set; }
        public ReportFreezeDate()
        {

        }

        [NotMapped]
        public string FrozenDateString {
            get
            {
                var date = FrozenDate.ToString("yyyy-MM-dd");
                return date;
            }
        }    
    }
}
