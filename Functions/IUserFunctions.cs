using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTrackerRepo.Models;

namespace TimeTrackerRepo.Functions
{
    public interface IUserFunctions
    {

        public string GetSelectableYears(int startingYear);
        public List<Users> GetUsers();
        public List<Users> GetUsers(string loggedInUser);
        public string GetSelectedUser(string email);
        public List<ActivityList> GetActivityList(string? userName);
        public AllTransactions GetAssignments(string empNo, DateTime startDate, DateTime endDate, string columns);

        public int CalculateExtraFreezeColumns(DateTime frozenDate, DateTime startDate, DateTime endDate);
    }
}
