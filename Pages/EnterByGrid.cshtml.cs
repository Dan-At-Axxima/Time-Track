using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TimeTrackerRepo.Data;
using TimeTrackerRepo.Functions;

namespace TimeTrackerRepo.Pages;

public class TimeTrackerGrid : PageModel
{
    private readonly ILogger<TimeTrackerGrid> _logger;

    public string[] Activities { get; set; }
    public string SelectedActivity { get; set; }
    public string SelectedActivityDate { get; set; }

    private TimeTrackerContext _dbContext;
    private IHttpContextAccessor _httpContextAccessor;
    private IConfiguration _configuration;

    public string BaseURL { get; }
    public string ActivityListURL { get; }
    public string BatchSaveURL { get; set; }
    public string BatchRemoveURL { get; set; }
    public string BatchUpdateURL { get; set; }
    public string IgnoreChange { get; set; }
    public DateTime Today { get; set; }
    public int Year { get; set; }
    public int Month { get; set; }

    public string? UserName { get; set; }
    public string Mode { get; set; }

    public TimeTrackerGrid(ILogger<TimeTrackerGrid> logger, TimeTrackerContext dbContext, IHttpContextAccessor httpContextAccessor, IConfiguration configuration)
    {
        _logger = logger;
        IgnoreChange = "true";
        SelectedActivityDate = DateTime.Now.Date.ToString("yyyy-MM-dd");
        _dbContext = dbContext;
        _httpContextAccessor = httpContextAccessor;
        _configuration = configuration;
        BaseURL = _configuration["URLS:TestBaseURL"];
        ActivityListURL = _configuration["URLS:ActivityListURL"];
        BatchSaveURL = _configuration["URLS:BatchSaveURL"]; 
        BatchUpdateURL = _configuration["URLS:BatchUpdateURL"];
        BatchRemoveURL = _configuration["URLS:BatchRemoveURL"];
        SetActivityList();
        Today = DateTime.Now;
        Year = Today.Date.Year;
        Month = Today.Date.Month;
        UserName = _httpContextAccessor.HttpContext.User.Identity.Name;
        Mode = "none";


    }


    public void SetActivityList()
    {
        UserFunctions funcs = new UserFunctions(_dbContext);
        var userFuncs = new UserFunctions(_dbContext);
        string? userName = string.Empty;
        userName = _httpContextAccessor.HttpContext.User.Identity.Name;
        var activityList = userFuncs.GetActivityList(userName);
        Activities = new string[activityList.Count];
        for (int i = 0; i < activityList.Count; i++)
        {
            Activities[i] = activityList[i].ActivityName;
        }
        SelectedActivity = Activities[0];
    }


}

