using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TimeTrackerRepo.Data;
using TimeTrackerRepo.Functions;
using TimeTrackerRepo.Models;
using TimeTrackerRepo.Services;

namespace TimeTrackerRepo.Pages
{
    public class ManageActivitiesModel : PageModel
    {
        public string BaseURL { get; }
        public string RemoveAssignmentURL { get; }
        public string AddAssignmentURL { get; }


        private TimeTrackerContext _dbContext;
        internal ICurrentUserService _currentUserService;
        internal IHttpContextAccessor _httpContextAccessor;
        private IConfiguration _configuration;

        public string ActivityId { get; set; }
        public string ActivityName { get; set; }

        public bool IsAdministrator { get; }

        public List<Users> Users { get; set; }

        public List<Activities> ActivityList { get; set; }
        public List<Assignments> Assignments { get; set; }
        public List<AssignmentListItem> AssignmentListItems { get; set; }
        public List<AssignmentListItem> NonAssignedAssignmentListItems { get; set; }
        public ManageActivitiesModel(TimeTrackerContext dbContext, IHttpContextAccessor httpContextAccessor, IConfiguration configuration, ICurrentUserService currentUserService)
        {
            var test = httpContextAccessor.HttpContext.Session.GetString("loggedinUser");
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _httpContextAccessor = httpContextAccessor;
            _configuration = configuration;
#if DEBUG
            BaseURL = _configuration["URLS:TestBaseURL"];
#else
            BaseURL = _configuration["URLS:BaseURL"];
#endif
            RemoveAssignmentURL = _configuration["URLS:RemoveAssignmentURL"];
            AddAssignmentURL = _configuration["URLS:AddAssignmentURL"];
            IsAdministrator = _httpContextAccessor.HttpContext.User.IsInRole("Administrator");

        }

        public void OnGet()
        {
            InitializeUser();
        }

        private void InitializeUser()
        {
            var userFuncs = new UserFunctions(_dbContext);
            Users = userFuncs.GetUsers(_currentUserService.LoggedInUser);
            var dataFuncs = new DataFunctions(_dbContext, _currentUserService,_httpContextAccessor);
            ActivityList = dataFuncs.GetUnAssignedActivities();
            Assignments = dataFuncs.GetUserAssignments2();
            var assignmentList = new List<AssignmentListItem>();
            int i = 1;
            foreach(var assignment in Assignments)
            {
                var listItem = new AssignmentListItem();
                listItem.Id = i++;
                listItem.Client = assignment.Client;
                listItem.Project = assignment.Project;
                listItem.Activity = assignment.Activity;
                assignmentList.Add(listItem);
            }
            AssignmentListItems = assignmentList;
            var activityList = new List<AssignmentListItem>();
            i = 1;
            foreach(var activity in ActivityList)
            {
                var activityItem = new AssignmentListItem();
                activityItem.Id = i++;
                activityItem.Client = activity.Client;
                activityItem.Project = activity.Project;    
                activityItem.Activity = activity.Activity;
                activityList.Add(activityItem);

            }
            NonAssignedAssignmentListItems = activityList;
        }

        public void OnPostChangeUser(string thisSelectedUser)
        {
            _httpContextAccessor.HttpContext.Session.SetString("currentUser",thisSelectedUser);
            InitializeUser();
        }

    }
}
