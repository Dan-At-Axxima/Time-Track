using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTrackerRepo.Services;

namespace TimeTrackerRepo.Models
{
    public class CurrentUserService : ICurrentUserService
    {
        public string CurrentUser { get; set;}
        public string LoggedInUser { get; set; }
        public List<string> AllDates { get; set ; }
    }
}
