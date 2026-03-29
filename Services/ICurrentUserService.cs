using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTrackerRepo.Services
{
    public interface ICurrentUserService
    {

       
        string LoggedInUser { get; set; }
        string CurrentUser { get; set; }    
        List<string> AllDates { get; set; }
    }
}
