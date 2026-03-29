using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTrackerRepo.Data;
using TimeTrackerRepo.Models;
using TimeTrackerRepo.Pages;
using TimeTrackerRepo.Services;

namespace TimeTrackerRepo.Functions
{
    public class DataFunctions : IDataFunctions
    {
        private TimeTrackerContext _dbContext;
        private ICurrentUserService _currentUserService;
        private IHttpContextAccessor _httpContextAccessor;

        public DataFunctions()
        {

        }

        
        public DataFunctions(TimeTrackerContext dbContext)
        {
            _dbContext = dbContext;
        }

        public DataFunctions(TimeTrackerContext dbContext, ICurrentUserService currentUserService, IHttpContextAccessor httpContextAccessor)
        {
            _dbContext = dbContext;
            _currentUserService = currentUserService;
            _httpContextAccessor = httpContextAccessor;
        }
        public bool SaveTransaction(UpdateValues values)
        {
            int employee;
            int year;
            int month;
            int day;
            Int32.TryParse(values.User, out employee);
            Int32.TryParse(values.Year, out year);
            Int32.TryParse(values.Month, out month);
            Int32.TryParse(values.Day, out day);
            var date = new DateTime(year,month,day,0,0,0);
            var tran = _dbContext.Transactions.Where(x => x.EmployeeNumber == employee  && x.Date.Year == year && x.Date.Month == month && x.Date.Day == day && x.Project == values.Project
                                                                    && x.Client == values.Client && x.Activity == values.Activity).FirstOrDefault();
            if(tran != null)
            {
                tran.Hours = values.Time;
                _dbContext.SaveChanges();
                return true;
            }
            else
            {
                var newTran = new Transactions(employee,values.Client,values.Project,values.Activity,date,values.Time,values.Comment,"");
                _dbContext.Transactions.Add(newTran);
                _dbContext.SaveChanges();
                return true;
            }

        }

        public List<AssignmentListItem> GetUserAssignments()
        {
            var currentUser = _httpContextAccessor.HttpContext.Session.GetString("currentUser");
            var result = _dbContext.Assignments.Where(x => x.EmployeeNumber.ToString() == currentUser).ToList();
            List<AssignmentListItem> list = new List<AssignmentListItem>();
            foreach(var assignment in result)
            {
                var newListItem = new AssignmentListItem();
                newListItem.Activity = assignment.Activity;
                newListItem.Client = assignment.Client;
                newListItem.Project =    assignment.Project;
                list.Add(newListItem);
            }
            return list;
        }

        public List<Assignments> GetUserAssignments2()
        {
            var currentUser = _httpContextAccessor.HttpContext.Session.GetString("currentUser");
            var result = _dbContext.Assignments.Where(x => x.EmployeeNumber.ToString() == currentUser).ToList();
            result = result.OrderBy(x => x.ClientProjectActivity).ToList();
            return result;
        }

        public List<Activities> GetUnAssignedActivities()
        {
            var activities = _dbContext.Activity.ToList();
            var currentUser = _httpContextAccessor.HttpContext.Session.GetString("currentUser");
            var assignments = _dbContext.Assignments.Where(x => x.EmployeeNumber.ToString() == currentUser).ToList().OrderBy(o => o.ClientProjectActivity);
            var res = new List<Assignments>();
            int i = 0;
            foreach (var assign in assignments)
            {
                i++;
               
                var newRes = activities.Where(x => x.ActivityHashCode == assign.ActivityHashCode).FirstOrDefault();   
                if(newRes == null)
                {
                    res.Add(assign);
                }
            }



            List<Activities> results = new List<Activities>();
            foreach (var activity in activities)
            {
                var rec = activity.ClientProjectActivity;
                var recInAssignments = assignments.Where(x => x.ClientProjectActivity == rec).FirstOrDefault();
                if(recInAssignments == null)
                {
                    results.Add(activity);
                }
            }
            return results;
        }

       
       public bool AddBatchRecord(BatchUpdateValues values)
        {
            var newDate = DateTime.Parse(values.Date);
            var parsedActivities = ParseActivity(values.ClientProjectActivity);
            var id = GetEmployeeNumberFromID(values.UserName);
            var hours = values.Hours + values.Minutes;
            Transactions transaction = new Transactions();
            transaction.Client = parsedActivities.Client;
            transaction.Project = parsedActivities.Project;
            transaction.Activity = parsedActivities.Activity;
            transaction.EmployeeNumber = id;
            transaction.Date = newDate.Date;
            transaction.Hours = hours;
            transaction.Comment = values.Comment;
            transaction.SlipId=String.Empty;

            if (DoesActivityExist(transaction))
            {
                return false;
            }
            else
            {
                _dbContext.Transactions.Add(transaction);
                _dbContext.SaveChanges();
                return true;
            }

        }

        public int AddAssignment(AssignmentListItem listItem)
        {
            var newAssignment = new Assignments();
            newAssignment.Activity = listItem.Activity;
            newAssignment.Project = listItem.Project;
            newAssignment.Client = listItem.Client;
            newAssignment.EmployeeNumber = listItem.Id;
            var ret = 0;
            try
            {
                _dbContext.Assignments.Add(newAssignment);
                ret = _dbContext.SaveChanges();

            }
            catch (Exception ex)
            {
                ret = -1;
            }
            return ret;
        }

        public bool RemoveAssignment(AssignmentListItem listItem)
        {
            var thisAssignment = _dbContext.Assignments.Where(x => x.Client == listItem.Client && x.Project == listItem.Project
                                                    && x.Activity == listItem.Activity && x.EmployeeNumber == listItem.Id).FirstOrDefault();
            if (thisAssignment != null)
            {
                _dbContext.Assignments.Remove(thisAssignment);
                _dbContext.SaveChanges();
                return true;
            }
            return false;
        }

        public bool UpdateBatchRecord(BatchUpdateValues values)
        {
            var newDate = DateTime.Parse(values.Date);
            var parsedActivities = ParseActivity(values.ClientProjectActivity);
            var id = GetEmployeeNumberFromID(values.UserName);
            var hours = values.Hours + values.Minutes;
            Transactions transaction = new Transactions();
            transaction.Client = parsedActivities.Client;
            transaction.Project = parsedActivities.Project;
            transaction.Activity = parsedActivities.Activity;
            transaction.EmployeeNumber = id;
            transaction.Date = newDate.Date;
            transaction.Hours = hours;
            transaction.Comment = values.Comment;
            transaction.SlipId = String.Empty;

            if (!DoesActivityExist(transaction))
            {
                return false;
            }
            else
            {
                var original = _dbContext.Transactions.Where(x => x.EmployeeNumber == transaction.EmployeeNumber &&
                    transaction.Project == x.Project && x.Activity == transaction.Activity && x.Date.Date == transaction.Date.Date).FirstOrDefault();
                original.Hours = transaction.Hours;
                original.Comment = transaction.Comment;
                _dbContext.SaveChanges();
                return true;
            }

        }

        public bool DeleteBatchRecord(BatchUpdateValues values)
        {
            var newDate = DateTime.Parse(values.Date);
            var parsedActivities = ParseActivity(values.ClientProjectActivity);
            var id = GetEmployeeNumberFromID(values.UserName);
            var hours = values.Hours + values.Minutes;
            var deleteTransaction = from Trans in _dbContext.Transactions
                                    where Trans.EmployeeNumber == id &&
                                    Trans.Project == parsedActivities.Project && Trans.Client == parsedActivities.Client &&
                                    Trans.Activity == parsedActivities.Activity && Trans.Date.Date == newDate.Date
                                    select Trans;

            foreach(var transaction in deleteTransaction)
            {
                _dbContext.Transactions.Remove(transaction);
            }

                _dbContext.SaveChanges();

            return true;

        }

        private bool DoesActivityExist(Transactions transaction)
        {
            var id = 0;
            var count = _dbContext.Transactions.Where(x => x.EmployeeNumber == transaction.EmployeeNumber &&
                    transaction.Project == x.Project && x.Activity == transaction.Activity && x.Date.Date == transaction.Date.Date).Count();
            if(count > 0)
            {
                return true;
            }
            else
            {

                return false;
            }
        }

        private int GetEmployeeNumberFromID(string employeeName)
        {
            var id = 0;
            id = _dbContext.Employee.Where(x => x.EMail == employeeName).FirstOrDefault().EmployeeNumber;
            return id;
        }

        private ActivityParse ParseActivity(string activity)
        {
            var len = activity.Length;
            var all = activity.Split("~");
            var client = string.Empty;
            var project = string.Empty;
            var act = string.Empty;

            if (all.Length == 3)
            {
                client = all[0];
                project = all[1];
                act = all[2];
            }

            ActivityParse activityParse = new ActivityParse();
            activityParse.Client = client;
            activityParse.Project = project;
            activityParse.Activity = act;
            return activityParse;
        }

        internal bool SaveBatchValues(BatchUpdateValues values)
        {
            var client = values.ClientProjectActivity.Split(' ').FirstOrDefault();
            var activitu = values.ClientProjectActivity.Split(' ').LastOrDefault();

            return true;
        }

        public bool SaveCellValueAndComment(UpdateValues values,string d1)
        {
            int employee;
            int day;
            int month;
            int year;
            var dates = d1.Split("-");
            DateTime date;
            Int32.TryParse(values.User, out employee);
            if(dates.Length == 3)
            {
                Int32.TryParse(dates[0], out year);
                Int32.TryParse(dates[1], out month);
                Int32.TryParse(dates[2], out day);
                date = new DateTime(year, month, day,0,0,0);
                var tran = _dbContext.Transactions.Where(x => x.EmployeeNumber == employee && x.Date.Year == year && x.Date.Month == month && x.Date.Day == day && x.Project == values.Project
                                                                  && x.Client == values.Client && x.Activity == values.Activity).FirstOrDefault();
                if (tran != null)
                {
                    tran.Comment = values.Comment;
                    tran.Hours = values.Time;
                    _dbContext.SaveChanges();
                    return true;
                }
                else
                {
                    var newTran = new Transactions(employee, values.Client, values.Project, values.Activity, date, values.Time, values.Comment, "");
                    _dbContext.Transactions.Add(newTran);
                    _dbContext.SaveChanges();
                }
                return true;
            }

            return false;
        }

        public bool SaveCellData(ILogger<IndexModel> logger,SaveCellValues values, DateTime d1)
        {
            int employee;
            Int32.TryParse(values.User,out employee);
            if( employee <1000 || employee > 9999)            {
                return false;
            }
            var tran = _dbContext.Transactions.Where(x => x.EmployeeNumber == employee && x.Date.Date == d1.Date.Date && x.Project == values.Project
                                                              && x.Client == values.Client && x.Activity == values.Activity).FirstOrDefault();
            logger.LogInformation("trying to save data");
            var message = "the tran is " + tran;
            logger.LogInformation(message);
            if (tran != null)
            {
                logger.LogInformation("updating info");
                logger.LogInformation(tran.Comment);
                logger.LogInformation(tran.EmployeeNumber.ToString());
                logger.LogInformation(tran.Client);
                logger.LogInformation(tran.Activity);
                logger.LogInformation(tran.Date.ToString("yyyy-dd-mm"));
                logger.LogInformation(tran.EmployeeNumber.ToString());
                logger.LogInformation(tran.Hours);
                tran.Comment = values.Comment;
                tran.Hours = values.Time;
                try
                {
                    _dbContext.SaveChanges();
                    return true;
                }
                catch (Exception ex)
                {
                    logger.LogInformation(ex.Message);
                    return false;
                }
            }
            else
            {
                var newTran = new Transactions(employee, values.Client, values.Project, values.Activity, d1, values.Time.Trim(), values.Comment.Trim(), "");
                logger.LogInformation("saving info");
                logger.LogInformation(newTran.Comment);
                logger.LogInformation(employee.ToString());
                logger.LogInformation(newTran.Client);
                logger.LogInformation(newTran.Activity);
                logger.LogInformation(newTran.Date.ToString("yyyy-MM-dd"));
                logger.LogInformation(newTran.Hours);
                try
                {
                    _dbContext.Transactions.Add(newTran);
                    _dbContext.SaveChanges();
                    return true;

                }
                catch (Exception ex)
                {
                    return false;
                }


            }

        }
        public bool SaveComment(UpdateValues values)
        {
            int employee;
            int year;
            int month;
            int day;
            Int32.TryParse(values.User, out employee);
            Int32.TryParse(values.Year, out year);
            Int32.TryParse(values.Month, out month);
            Int32.TryParse(values.Day, out day);
            var date = new DateTime(year, month, day, 0, 0, 0);
            var tran = _dbContext.Transactions.Where(x => x.EmployeeNumber == employee && x.Date.Year == year && x.Date.Month == month && x.Date.Day == day && x.Project == values.Project
                                                                    && x.Client == values.Client && x.Activity == values.Activity).FirstOrDefault();
            if (tran != null)
            {
                tran.Comment = values.Comment;
                tran.Hours = values.Time;
                _dbContext.SaveChanges();  
                return true;
            }
            else
            {
                var newTran = new Transactions(employee, values.Client, values.Project, values.Activity, date, values.Time,values.Comment, "");
                _dbContext.Transactions.Add(newTran);
                _dbContext.SaveChanges();
                return true;
            }

        }

        public bool DeleteCellContent(UpdateValues values, ILogger<IndexModel> logger)
        {
            int employee;
            int year;
            int month;
            int day;
            Int32.TryParse(values.User, out employee);
            Int32.TryParse(values.Year, out year);
            Int32.TryParse(values.Month, out month);
            Int32.TryParse(values.Day, out day);
            var date = new DateTime(year, month, day, 0, 0, 0);
            var tran = _dbContext.Transactions.Where(x => x.EmployeeNumber == employee && x.Date.Year == year && x.Date.Month == month && x.Date.Day == day && x.Project == values.Project
                                                                    && x.Client == values.Client && x.Activity == values.Activity).FirstOrDefault();

            var user = "The New Employee is: " + values.User.ToString();
            var project = "The Project is: " + values.Project;
            var client = "The Client is: " + values.Client;
            var activity = "The Activity is: " + values.Activity;
            logger.LogInformation(user);
            logger.LogInformation( year.ToString());
            logger.LogInformation(month.ToString());
            logger.LogInformation(day.ToString());
            logger.LogInformation(client);
            logger.LogInformation(project);
            logger.LogInformation(activity);
            if (tran != null)
            {
                logger.LogInformation("about to delete");
                var ret = _dbContext.Transactions.Remove(tran);
                logger.LogInformation("finished baby");
                logger.LogInformation(ret.ToString());
                _dbContext.SaveChanges();

                return true;
            }
            else
            {
                logger.LogInformation("The tran value was null");
            }
            return true;

        }

        public bool SetFrozenDate(string date)
        {
            DateTime dateTime = new DateTime();
            DateTime.TryParse(date,out dateTime);
            var existingdate = _dbContext.ReportFreezeDate.ToList();  // there should only ever be one
            _dbContext.ReportFreezeDate.RemoveRange(existingdate);
            _dbContext.SaveChanges();

            var newRecord = new ReportFreezeDate();
            newRecord.FrozenDate = dateTime;
            _dbContext.ReportFreezeDate.Add(newRecord);
            _dbContext.SaveChanges();
            return true;
        }

        public DateTime GetFrozenDate()
        {
            try
            {
                var existingdate = _dbContext.ReportFreezeDate.FirstOrDefault();  // there should only ever be one
                return existingdate.FrozenDate;
            }
            catch (Exception ex)
            {
                return DateTime.MinValue;
            }
        }

        public UpdateValues GetCellData(CellValues cellValues)
        {
            throw new NotImplementedException();
        }

        public DateTime GetDefaultStartDate(string employeeNumber)
        {
            var startDate = _dbContext.NewSettings.Where(x => x.EmployeeNumber == employeeNumber).FirstOrDefault().StartDate;
            return startDate;
        }

        public DateTime GetDefaultEndDate(string employeeNumber)
        {
            var endDate = _dbContext.NewSettings.Where(x => x.EmployeeNumber == employeeNumber).FirstOrDefault().EndDate;
            return endDate;
        }

        public bool SetDefaultStartDate(string employeeNumber,DateTime startDate)
        {
            var settings = _dbContext.NewSettings.Where(x => x.EmployeeNumber == employeeNumber).FirstOrDefault();
            if (settings != null)
            {
                settings.StartDate = startDate;
                _dbContext.SaveChanges();
            }
            return true;
        }

        public bool SetDefaultEndDate(string employeeNumber, DateTime endDate)
        {
            var settings = _dbContext.NewSettings.Where(x => x.EmployeeNumber == employeeNumber).FirstOrDefault();
            if(settings != null)
            {
                settings.EndDate = endDate;
                _dbContext.SaveChanges();
            }
            
            return true;
        }

        public Task<string?> GetFrozenDateStringAsync()
        {
            var frozenDate = _dbContext.ReportFreezeDate.FirstOrDefault();
            if (frozenDate != null)
            {
                return Task.FromResult<string?>(frozenDate.FrozenDate.ToString("yyyy-MM-dd HH:mm:ss.fff"));
            }
            else
            {
                return Task.FromResult<string?>(null);
            }
        }
    }
}
