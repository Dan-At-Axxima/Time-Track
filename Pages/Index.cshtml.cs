using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TimeTrackerRepo.Data;
using TimeTrackerRepo.Functions;
using TimeTrackerRepo.Models;
using TimeTrackerRepo.Services;
using System.Globalization;
using System.Security.Claims;
namespace TimeTrackerRepo.Pages;

public class IndexModel : PageModel
{
    private readonly ILogger<IndexModel> _logger;

    public string CellInitialValue { get; set; } 
    public string CellInitialComment { get; set; }
    public int CellInitialRow { get; set; } = 0;
    public int CellInitialColumn { get; set; } = 0;
    public string CopiedText { get; set; }
    public string CopiedComment { get; set; }
    public string DefaultStartDate { get; set; }
    public string DefaultEndDate { get; set; }
    public List<string> AllDates { get; set; }
    public string NewAllDates { get; set; }

    public string FrozenDateString { get; set; }
    public DateTime FrozenDate { get; set; }
    public int ExtraFreezeColumns { get; set; } = 0;

    public DateTime StartDate { get; set; }
    public DateTime EndDate { get; set; }

    public int? SelectedColumn { get; set; }
    public int? SelectedRow { get; set; }
    public string originalCellComment { get; set; }

    public string originalCellValue { get; set; }

    public int selectedMonth { get; set; }
    public int selectedYear { get; set; }

    
    public int FirstColumn { get; }
    public int FirstRow { get; }

    
    public Boolean IsLoading { get; set; }
    public int NonChargeableRows { get; set; }
    public int TotalRows { get; set; }

    public String ColumnTitles { get; set; }
    public int NumberOfDaysInMonth { get; set; }

    public int NonBillableAssignmentsCount { get; set; }

    public string ReturnHoursStringArray { get; set; }
    public string ReturnHoursString { get; set; }
    public string ReturnCommentString { get; set; }
    public List<Comments> ReturnCommentList { get; set; }
    public string[] RowTitles { get; set; }

    public string BaseURL { get; set; }
    public string UpdateCellValueURL { get; set; }
    public string UpdateCellCommentURL { get; set; }
    public string DeleteCellContentURL { get; set; }
    public string SaveCellDataURL { get; set; }
    public string GetCellDataURL { get; set; }
    public string ChangeFrozenDateURL { get; set; }

    public string SelectableYears { get; set; }
    public string UserRole { get; set; }

    public bool EditError { get; set; }

    [BindProperty]
    public List<Users> Users { get; set; }

    private readonly TimeTrackerContext _dbContext;
    internal ICurrentUserService _currentUserService;
    internal IHttpContextAccessor _httpContextAccessor;
    private IConfiguration _configuration;
    private IDataFunctions _dataFunctions;
    private IUserFunctions _userFunctions;

    public IndexModel(ILogger<IndexModel> logger, TimeTrackerContext dbContext,IHttpContextAccessor httpContextAccessor, IConfiguration configuration,
                ICurrentUserService currentUserService,IUserFunctions userFunctions,IDataFunctions dataFunctions)
    {

        _configuration = configuration;
        _logger = logger;
        _dbContext = dbContext;
        _currentUserService = currentUserService;
        _httpContextAccessor = httpContextAccessor;
        _dataFunctions = dataFunctions;
        _userFunctions = userFunctions;
        _logger = logger;
        _logger.LogInformation("program is starting");
#if DEBUG
        BaseURL = _configuration["URLS:TestBaseURL"];
#else
            BaseURL = _configuration["URLS:BaseURL"];
#endif
        FirstColumn = 4;
        FirstRow = 2;
        FrozenDateString = dbContext.ReportFreezeDate.ToList().FirstOrDefault().FrozenDateString;
        FrozenDate = DateTime.ParseExact(FrozenDateString, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        SetDefaultDates();
        ExtraFreezeColumns = _userFunctions.CalculateExtraFreezeColumns(FrozenDate, StartDate, EndDate);
        EditError = false;
        originalCellValue = "";
        var emps = GetEmployees();
        if ( selectedMonth == 0)
        {
            selectedMonth = StartDate.Month;
            selectedYear = EndDate.Year;
        }
        var funcs = new UserFunctions();
        SelectableYears = funcs.GetSelectableYears(selectedYear);
        IsLoading = true;

        UpdateCellValueURL = _configuration["URLS:UpdateCellValueURL"];
        UpdateCellCommentURL = _configuration["URLS:UpdateCellCommentURL"];
        DeleteCellContentURL = _configuration["URLS:DeleteCellContentURL"];
        SaveCellDataURL = _configuration["URLS:SaveCellDataURL"];
        ChangeFrozenDateURL = _configuration["URLS:ChangeFrozenDateURL"];
        GetCellDataURL = _configuration["URLS:GetCellDataURL"];
        //DefaultStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("yyyy,MM,dd");
        //DefaultEndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, NumberOfDaysInMonth).ToString("yyyy,MM,dd");

        SetUserInfo();
        var userFuncs = new UserFunctions(_dbContext);
        var id = _httpContextAccessor.HttpContext.User.Identity.Name;
        if (_currentUserService.CurrentUser == null || id != _currentUserService.LoggedInUser)
        {
            _currentUserService.LoggedInUser = httpContextAccessor.HttpContext.User.Identity.Name == null ? String.Empty : httpContextAccessor.HttpContext.User.Identity.Name;
            _currentUserService.CurrentUser = userFuncs.GetSelectedUser(id == null ? "help" : id);
        }
        Utilities utils = new Utilities();
        currentUserService.AllDates = utils.AllDates(StartDate, EndDate);
        var claimsPrincipal = httpContextAccessor.HttpContext.User as ClaimsPrincipal;
        var roleClaim = claimsPrincipal.FindFirst(ClaimTypes.Role);
        UserRole = roleClaim.Value;
    }

    private int GetEmployeeNumber()
    {
        var name = _httpContextAccessor.HttpContext.User.Identity.Name;
        var empNo = _dbContext.Employee.Where(x => x.EMail.ToLower() == name.ToLower()).FirstOrDefault().EmployeeNumber;
        return empNo; 

    }
    private void SetUserInfo()
    {
        var loggedInUser = _httpContextAccessor.HttpContext.Session.GetString("loggedinUser");
        var currentUser = _httpContextAccessor.HttpContext.Session.GetString("currentUser");
        if ((loggedInUser == null || currentUser==null)  && _httpContextAccessor!=null)
        {
             loggedInUser = _httpContextAccessor.HttpContext.User.Identity.Name;
            _httpContextAccessor.HttpContext.Session.SetString("loggedinUser", loggedInUser != null ? loggedInUser : string.Empty);
            var id = _httpContextAccessor.HttpContext.User.Identity.Name;
             currentUser = _userFunctions.GetSelectedUser(id == null ? "help" : id);
            _httpContextAccessor.HttpContext.Session.SetString("currentUser", currentUser !=null ? currentUser : string.Empty);
        }


    }


    public void OnGet()
    {
        _logger.LogInformation("program is starting");
        var currentUser = _httpContextAccessor.HttpContext.Session.GetString("currentUser");

        try
        {
            InitializeGrid();
            InitializeUser(currentUser ?? string.Empty);
        }
        catch (Exception ex)
        {
            _logger.LogInformation(ex.Message);

        }
    }

    public void OnPostChangeDates (string beginDate1,string endDate1,string dateAction1)
    {
        if (dateAction1 == "change") {
            ChangeDates(beginDate1, endDate1);
        }
        else
        {
            var d1 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            var d2 = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.DaysInMonth(DateTime.Now.Year, DateTime.Now.Month));
            DefaultStartDate = d1.ToString("yyyy-MM-dd");
            DefaultEndDate = d2.ToString("yyyy-MM-dd");

            ChangeDates(DefaultStartDate, DefaultEndDate);
        }
    }

    private void ChangeDates(string beginDate1, string endDate1) {
        if(beginDate1 == null || endDate1 == null) return;
        DefaultEndDate = endDate1;
        DefaultStartDate = beginDate1;
        var empNo = GetEmployeeNumber();
        StartDate = DateTime.ParseExact(beginDate1, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        EndDate = DateTime.ParseExact(endDate1, "yyyy-MM-dd", CultureInfo.InvariantCulture);
        _dataFunctions.SetDefaultEndDate(empNo.ToString(), EndDate);
        _dataFunctions.SetDefaultStartDate(empNo.ToString(), StartDate);
        ExtraFreezeColumns = _userFunctions.CalculateExtraFreezeColumns(FrozenDate, StartDate, EndDate);
        InitializeGrid();
        var currentUser = _httpContextAccessor.HttpContext.Session.GetString("currentUser");
        InitializeUser(currentUser ?? string.Empty);


    }


    public void OnPostChangeUser(string beginningDate, string endingDate, string thisSelectedUser)
    {
        var empNo = GetEmployeeNumber();
        StartDate = _dataFunctions.GetDefaultStartDate(empNo.ToString());
        EndDate = _dataFunctions.GetDefaultEndDate(empNo.ToString());
        _currentUserService.CurrentUser = thisSelectedUser;
        _httpContextAccessor.HttpContext.Session.SetString("currentUser", thisSelectedUser);
        DefaultStartDate = StartDate.ToString("yyyy-MM-dd");
        DefaultEndDate = EndDate.ToString("yyyy-MM-dd");

        InitializeGrid();
        InitializeUser(thisSelectedUser);

    }
    //public void OnPostChangeUser(string beginningDate, string endingDate,string monthSelection,string yearSelection,string thisSelectedUser)
    //{
        
    //    DateTime startDate = new DateTime();
    //    DateTime endDate = new DateTime();
    //    var dateValues = beginningDate.Split('-');
    //    int year;
    //    int month;
    //    int day;
    //    if(dateValues!=null && dateValues.Length == 3)
    //    {
    //        year = Convert.ToInt32(dateValues[0]);
    //        month = Convert.ToInt32(dateValues[1]);
    //        day = Convert.ToInt32(dateValues[2]);
    //        DefaultStartDate = new DateTime(year,month,day).ToString("yyyy,MM,dd");
    //        startDate = new DateTime(year, month, day);
    //        DefaultStartDate = startDate.ToString("yyyy,MM,dd");
          
    //    }
    //    dateValues = endingDate.Split('-');
    //    if (dateValues != null && dateValues.Length == 3)
    //    {
    //        year = Convert.ToInt32(dateValues[0]);
    //        month = Convert.ToInt32(dateValues[1]);
    //        day = Convert.ToInt32(dateValues[2]);
    //        endDate = new DateTime(year, month, day);
    //        DefaultEndDate = endDate.ToString("yyyy,MM,dd");

    //    }
    //    //        var startDate = new DateTime(beginningDate);
    //    //        DefaultEndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, NumberOfDaysInMonth).ToString("yyyy,MM,dd");
    //    _currentUserService.CurrentUser = thisSelectedUser;
    //    Int32.TryParse(yearSelection, out year);
    //    Int32.TryParse(monthSelection, out month);
    //    selectedMonth = month;
    //    selectedYear = year;
    //    InitializeUser(_currentUserService.CurrentUser, startDate,endDate);
    //    InitializeUser(_currentUserService.CurrentUser);
    //    InitializeGrid(year, month);
    //}


    private void SetDefaultDates()
    {
        var employeeNumber = GetEmployeeNumber();
        var startDate = _dataFunctions.GetDefaultStartDate(employeeNumber.ToString());
        var endDate = _dataFunctions.GetDefaultEndDate(employeeNumber.ToString());
        StartDate = startDate;
        EndDate = endDate;
        DefaultStartDate = startDate.ToString("yyyy-MM-dd");
        DefaultEndDate = endDate.ToString("yyyy-MM-dd");
        TimeSpan diff = endDate - startDate;
        NumberOfDaysInMonth = diff.Days+1;
    }

    private List<Employee> GetEmployees()
    {
        return _dbContext.Employee.ToList();
    }

    private void GetUserData(string selectedUser)
    {
        var userFuncs = new UserFunctions(_dbContext);
        Users = userFuncs.GetUsers(_currentUserService.LoggedInUser);
    }

    
    private void InitializeUser(string selectedUser)
    {
        var userFuncs = new UserFunctions(_dbContext);

        Users = userFuncs.GetUsers(_currentUserService.LoggedInUser);

        //        var getData = userFuncs.GetAssignments(selectedUser,StartDate.Year,StartDate.Month);
        //        var getData = userFuncs.GetAssignments(selectedUser, StartDate, EndDate);
        var getData = userFuncs.GetAssignments(selectedUser, StartDate, EndDate, NewAllDates);
        NonChargeableRows = getData.NonchargeableRowCount;
        TotalRows = getData.RowCount+2;
        //     var hours = getData.hours.OrderBy(c => c.Client).ThenBy(p => p.Project).ThenBy(a => a.Activity).ToList();
     //   var strings = getData.hoursStringArray;
     //   var stringsCount = strings.Length;
        //  ReturnHoursStringArray = new string[stringsCount];
        RowTitles = getData.RowTitles;
        ReturnHoursStringArray = getData.hoursStringArray;
        ReturnHoursString = getData.hoursString;
        ReturnCommentString = getData.commentsString;
        ReturnCommentList = getData.CommentList;
        //var test = ReturnHoursStringArray.Split('#');
        //int count = 0;
        //if (test.Length > 0)
        //{
        //    for (int i = 0; i < test.Length; i++)
        //    {
        //        int idx = test[i].IndexOf(',');
        //        string client = test[i].Substring(0, idx);
        //        if (client[0] == '0')
        //        {
        //            count++;
        //        }
        //    }
        //}
   //     NonBillableAssignmentsCount = count;

    }

    private void InitializeGrid()
    {
        var utils = new Utilities();
        ColumnTitles = utils.getColumnTitles(StartDate, EndDate);
        AllDates = utils.AllDates(StartDate, EndDate);
        NewAllDates = utils.getAllDates(StartDate, EndDate);    
        TimeSpan diff = EndDate - StartDate;
        NumberOfDaysInMonth = diff.Days+1;
    }
}

