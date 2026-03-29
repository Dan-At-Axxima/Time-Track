using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using TimeTrackerRepo.Data;
using TimeTrackerRepo.Models;
using System.Text.Json;

namespace TimeTrackerRepo.Functions
{

    
    public class UserFunctions : IUserFunctions
    { 
        private TimeTrackerContext _context;

        public UserFunctions()
        {

        }
        public UserFunctions(TimeTrackerContext context)
        {
            _context = context;
        }
        public List<FullMonth> returnHoursValues { get; set; }
     
        public List<Comments> returnCommentValues { get; set; }

        public string[] returnHoursStringArray { get; set; }

        public string GetSelectableYears(int startingYear) {
            string ret = string.Empty;
            List<SelectableYears> years = new List<SelectableYears>();
            int firstYear = startingYear - 5;

            for(int i=0; i<6; i++)
            {
                var year = (firstYear +i).ToString();
                var yearToAdd = new SelectableYears
                {
                    id = year,
                    text = year
                };
                years.Add(yearToAdd);

            }
            string jsonYears = JsonSerializer.Serialize(years);

            return jsonYears;
        }

        public List<Users> GetUsers()
        {
            var users = new List<Users>();

            var employees = _context.Employee.Where(x => x.Active==true).ToList();
            foreach(var employee in employees)
            {
                var user = new Users(employee.EmployeeNumber, employee.FirstName + " " + employee.LastName,employee.EMail);
                users.Add(user);
            }
            
            return users;
        }

        public List<Users> GetUsers(string loggedInUser)
        {
            var users = new List<Users>();

            var employees = _context.Employee.Where(x => x.Active == true).ToList();
            if(loggedInUser == "torimastroianni@axxima.ca")
            {
                var test = employees.Where(x => x.EmployeeNumber == 1004 || x.EmployeeNumber == 1041).ToList();
                employees = test;
            }
            foreach (var employee in employees)
            {
                var user = new Users(employee.EmployeeNumber, employee.FirstName + " " + employee.LastName, employee.EMail);
                users.Add(user);
            }

            return users;
        }

        public string GetSelectedUser(string email)
        {
            var user = _context.Employee.Where(e => e.EMail.ToLower() == email.ToLower()).FirstOrDefault();
            return user.EmployeeNumber.ToString();
        }

        public List<ActivityList> GetActivityList(string? userName)
        {
            var user = _context.Employee.Where(e => e.EMail == userName).FirstOrDefault();
            //  return new { Moniker = "ATL2018", Name = "Atlanta Code Cammp" };
            var activities = _context.Assignments.Where(x => x.EmployeeNumber == user.EmployeeNumber).Select(p => p.ClientProjectActivity).ToList();
            var output = System.Text.Json.JsonSerializer.Serialize(activities);
            List<ActivityList> alist = new List<ActivityList>();
            for (int i = 0; i < activities.Count; i++)
            {
                var item = new ActivityList();
                item.ActivityId = i;
                item.ActivityName = activities[i];
                alist.Add(item);

            }
            return alist;

        }

        private List<Assignments> SortAssignments(List<Assignments> assignments)
        {
            var l1 = assignments.Where(x => x.Client.StartsWith('0')).OrderBy(y => y.Project).ThenBy(a => a.Activity).ToList();
            var l2 = assignments.Where(x => !x.Client.StartsWith('0')).OrderBy(y => y.Project).ThenBy(a => a.Activity).ToList();
            assignments = l1.Concat(l2).ToList();

            return assignments;
        }
        public AllTransactions GetAssignments(string empNo, DateTime startDate, DateTime endDate,string columns)
        {
            int employee;
            int rowHeaderCount = 4;
            Int32.TryParse(empNo, out employee);
            var assignments = _context.Assignments.Where(x => x.EmployeeNumber == employee).OrderBy(x => x.Client).ThenBy(x => x.Project).ThenBy(x => x.Activity).ToList();
            if (assignments != null && assignments.Count > 0)
            {
                assignments = SortAssignments(assignments);

            }


            TimeSpan diff = endDate - startDate;
            int numberOfDays = diff.Days + 1;
            var transactions = new List<Transactions>();
            var months = new List<FullMonth>();
            List<Comments> comments = new List<Comments>();
            var allDates = columns.Split(",").ToList();
            string[] row = new string[rowHeaderCount+allDates.Count()] ;
            List<string[]> strings = new List<string[]>();
            string[] rowTitles = new string[assignments.Count];
            for(int i = 0; i < assignments.Count; i++)
            {
                var ass = assignments[i];
                rowTitles[i] =  ass.Client + "," + ass.Project + "," + ass.Activity+"," ;
            }
            foreach (var assignment in assignments)
            {
                row = new string[3];
                row[0]= assignment.Client;
                row[1] = assignment.Project;
                row[2] = assignment.Activity;
                strings.Add(row);
            }

            foreach (var assignment in assignments)
            {
                var month = new FullMonth();
                month.Project = assignment.Project;
                month.Client = assignment.Client;
                month.Activity = assignment.Activity;
                months.Add(month);
                var comment = new Comments();
                comment.Project = assignment.Project;
                comment.Client = assignment.Client;
                comment.Activity = assignment.Activity;
                comments.Add(comment);
            }
            var dat = columns.Split(",").ToList();
            // AddHours(List<string[]> rows, string employee, DateTime startDate, DateTime endDate,List<string> columns)
         //   var test = AddHours(strings,empNo, startDate, endDate,columns);
         //   var anotherTest = test.ToArray();
//            returnHoursValues = AddHours(months, empNo, startDate, endDate);
           returnCommentValues = AddComments(comments, empNo, startDate, endDate,dat);
            var allTrans = new AllTransactions();
            allTrans.hours = returnHoursValues;
            //        var newTest = test.ToArray();
            //       string newTest2 = string.Join("#", test);
            allTrans.RowTitles = rowTitles;
            allTrans.RowCount = assignments.Count()+2;
            allTrans.NonchargeableRowCount = assignments.Where(x => x.Client[0] == '0').Count();
//            allTrans.hoursStringArray = rowTitles;
            //     allTrans.hoursString = convertHoursToString(returnHoursValues);
            //     allTrans.commentsString = convertCommentsToString(returnCommentValues);
            allTrans.CommentList = returnCommentValues;
            return allTrans;
            //returnHoursValue

        }

        

        private string[] convertHoursToArray(List<FullMonth> hours)
        {
            string allHoursString = string.Empty;
            string[] allHours = new string[hours.Count];

            for (int i = 0; i < hours.Count; i++)
            {
                string row = string.Empty;
                row += hours[i].Client + "," + hours[i].Project + "," + hours[i].Activity + "," + "," + hours[i].DayOne + "," + hours[i].DayTwo + "," + hours[i].DayThree + "," + hours[i].DayFour + ",";
                //row = hours[i].Client + "," + hours[i].Project + "," + hours[i].Activity +"," + hours[i].DayOne + "," + hours[i].DayTwo + "," + hours[i].DayThree + "," + hours[i].DayFour;
                row += hours[i].DayFive + "," + hours[i].DaySix + "," + hours[i].DaySeven + "," + hours[i].DayEight + "," + hours[i].DayNine + "," + hours[i].DayTen + "," + hours[i].DayEleven + ",";
                row += hours[i].DayTwelve + "," + hours[i].DayThirteen + "," + hours[i].DayFourTeen + "," + hours[i].DayFifteen + "," + hours[i].DaySixteen + "," + hours[i].DaySeventeen + "," + hours[i].DayEighteen + ",";
                row += hours[i].DayNineteen + "," + hours[i].DayTwenty + "," + hours[i].DayTwentyOne + "," + hours[i].DayTwentyTwo + "," + hours[i].DayTwentyThree + "," + hours[i].DayTwentyFour + "," + hours[i].DayTwentyFive + ",";
                row += hours[i].DayTwentySix + "," + hours[i].DayTwentySeven + "," + hours[i].DayTwentyEight + "," + hours[i].DayTwentyNine + "," + hours[i].DayThirty + "," + hours[i].DayThirtyOne;
                row += "#";
                allHours[i] = row;
                allHoursString = row;
            }


            return allHours;
        }

        private string convertHoursToString(List<FullMonth> hours)
        {
            string allHoursString = string.Empty;
            string row = string.Empty;
            for (int i = 0; i < hours.Count; i++)
            {
                
                row += hours[i].Client + "," + hours[i].Project + "," + hours[i].Activity + "," + "," + hours[i].DayOne + "," + hours[i].DayTwo + "," + hours[i].DayThree + "," + hours[i].DayFour + ",";
                row += hours[i].DayFive + "," + hours[i].DaySix + "," + hours[i].DaySeven + "," + hours[i].DayEight + "," + hours[i].DayNine + "," + hours[i].DayTen + "," + hours[i].DayEleven + ",";
                row += hours[i].DayTwelve + "," + hours[i].DayThirteen + "," + hours[i].DayFourTeen + "," + hours[i].DayFifteen + "," + hours[i].DaySixteen + "," + hours[i].DaySeventeen + "," + hours[i].DayEighteen + ",";
                row += hours[i].DayNineteen + "," + hours[i].DayTwenty + "," + hours[i].DayTwentyOne + "," + hours[i].DayTwentyTwo + "," + hours[i].DayTwentyThree + "," + hours[i].DayTwentyFour + "," + hours[i].DayTwentyFive + ",";
                row += hours[i].DayTwentySix + "," + hours[i].DayTwentySeven + "," + hours[i].DayTwentyEight + "," + hours[i].DayTwentyNine + "," + hours[i].DayThirty + "," + hours[i].DayThirtyOne;
                row += "#";
                allHoursString = row;
            }

            return allHoursString;
        }

        private string convertCommentsToString(List<Comments> hours)
        {
            string allHoursString = string.Empty;
            string row = string.Empty;
            for (int i = 0; i < hours.Count; i++)
            {

                row += hours[i].Client + "~" + hours[i].Project + "~" + hours[i].Activity + "~" + "~" + hours[i].DayOne + "~" + hours[i].DayTwo + "~" + hours[i].DayThree + "~" + hours[i].DayFour + "~";
                row += hours[i].DayFive + "~" + hours[i].DaySix + "~" + hours[i].DaySeven + "~" + hours[i].DayEight + "~" + hours[i].DayNine + "~" + hours[i].DayTen + "~" + hours[i].DayEleven + "~";
                row += hours[i].DayTwelve + "~" + hours[i].DayThirteen + "~" + hours[i].DayFourTeen + "~" + hours[i].DayFifteen + "~" + hours[i].DaySixteen + "~" + hours[i].DaySeventeen + "~" + hours[i].DayEighteen + "~";
                row += hours[i].DayNineteen + "~" + hours[i].DayTwenty + "~" + hours[i].DayTwentyOne + "~" + hours[i].DayTwentyTwo + "~" + hours[i].DayTwentyThree + "~" + hours[i].DayTwentyFour + "~" + hours[i].DayTwentyFive + "~";
                row += hours[i].DayTwentySix + "~" + hours[i].DayTwentySeven + "~" + hours[i].DayTwentyEight + "~" + hours[i].DayTwentyNine + "~" + hours[i].DayThirty + "~" + hours[i].DayThirtyOne;
                row += "^";
                allHoursString = row;
            }

            return allHoursString;
        }

       
       
        private List<string> AddHours(List<string[]> rows, string employee, DateTime startDate, DateTime endDate,string columns)
        {
            var columnList = columns.Split(",").ToList();
            int empNo;
            Int32.TryParse(employee, out empNo);
            var trans = _context.Transactions.Where(x => x.EmployeeNumber == empNo && (x.Date.Date >= startDate.Date && x.Date.Date <= endDate.Date)).ToList();
            trans = trans.OrderBy(x => x.Client).ThenBy(x => x.Project).ThenBy(x => x.Activity).ThenBy(x => x.Date.Month).ThenBy(x => x.Date.Day).ToList();
            List<string> retValues = new List<string>();
            foreach(string[] row in rows)
            {
                var tran = row[0] + "~" + row[1] + "~" + row[2];
                int idx = trans.FindIndex(i => i.ClientProjectActivity == tran);
                if (idx > -1)
                {
                    do
                    {
                        var data = trans[idx];
                        string d1 = data.Date.Date.ToString("yyyy-MM-dd");
                        var ColumnIndex = columnList.FindIndex(i => i == d1);
                        if (ColumnIndex > -1)
                        {
                            row[ColumnIndex] = data.Hours.Trim();
                        }
                        trans.Remove(data);
                        idx = trans.FindIndex(i => i.ClientProjectActivity == tran);
                    } while (idx > -1);
                }
                retValues.Add(string.Join(",",row));

            }
            return retValues;
        }


        private List<Comments> AddComments(List<Comments> comments, string employee, DateTime startDate, DateTime endDate,List<string>columns)
        {
            int empNo;
            Int32.TryParse(employee, out empNo);

            var trans = _context.Transactions.Where(x => x.EmployeeNumber == empNo && (x.Date.Date >= startDate.Date && x.Date.Date <= endDate.Date)).ToList();
            //trans = trans.OrderBy(x => x.Client).ThenBy(x => x.Project).ThenBy(x => x.Activity).ToList();
            //comments = comments.OrderBy(x => x.Client).ThenBy(x => x.Project).ThenBy(x => x.Activity).ToList();
            var newComments = new List<Comments>();
            for(int i=0;i<comments.Count;i++)
            {
                var com = new Comments(comments[i]);
                var idx = trans.FindIndex(x => x.ClientProjectActivity == com.ClientProjectActivity);
                if (idx > -1)
                {
                    do
                    {
                        var tran = trans[idx];
                        var date = tran.Date.ToString("yyyy-MM-dd");
                        var columnIdx = columns.IndexOf(date);
                        var thisComment = tran.Comment;
                        com.Date = date;
                        com.Column = columnIdx;
                        com.Row = i;
                        com.Comment = thisComment;
                        com.Hours = tran.Hours.Trim();
                        trans.Remove(tran);
                        newComments.Add(com);
                        com = new Comments(comments[i]);
                        idx = trans.FindIndex(x => x.ClientProjectActivity == comments[i].ClientProjectActivity);
                    } while(idx > -1);
                }
            }
            return newComments; // comments.Where(c => c.Comment !=null).ToList();
        }
        private List<Comments> AddComments(List<Comments> comments, List<Transactions> trans)
        {
            foreach (Transactions tran in trans)
            {
                var comment = comments.Where(x => x.Activity == tran.Activity && x.Project == tran.Project && x.Client == tran.Client).FirstOrDefault();
                if (comment != null)
                {
                    comment = addComm(comment, tran);
                }
            }
            return comments;
        }

        private Comments addComm(Comments thisComment, Transactions tran)
        {

            var type = typeof(Comments);
            var members = type.GetMembers();
            var mth = tran.Date.Day.ToString();
            for (int c = 0; c < members.Length; c++)
            {
                var member = members[c];
                Object[] objects = member.GetCustomAttributes(typeof(MyAttribute), true);
                if (objects != null && objects.Length > 0)
                {
                    var fieldAttrib = (MyAttribute)objects[0];
                    if (fieldAttrib.Name == mth)
                    {
                        ((PropertyInfo)member).SetValue(thisComment, tran.Comment.Trim());
                        break;
                    }
                }

            }
            return thisComment;
        }

        private List<FullMonth> Add(List<FullMonth> months,List<Transactions> trans, bool newOne)
        {
            var counter = 0;
            foreach (Transactions tran in trans)
            {
                counter += 1;
                var month = months.Where(x => x.Activity == tran.Activity && x.Project == tran.Project && x.Client == tran.Client).FirstOrDefault();
                if (month != null)
                {
                    month = addValues(month, tran);
                    var newMonth = months.Where(x => x.Activity == tran.Activity && x.Project == tran.Project && x.Client == tran.Client).FirstOrDefault();
                }
            }
            var temp = months.Where(x => x.Project == "Accounting").ToList();

            return months;
        }
        private List<FullMonth> Add(List<FullMonth> months, List<Transactions> trans)
        {
            var counter = 0;
            foreach (Transactions tran in trans)
            {
                counter += 1;
                var month = months.Where(x => x.Activity == tran.Activity && x.Project == tran.Project && x.Client == tran.Client).FirstOrDefault();
                if (month != null)
                {
                    month = addValues(month, tran);
                    var newMonth = months.Where(x => x.Activity == tran.Activity && x.Project == tran.Project && x.Client == tran.Client).FirstOrDefault();
                }
            }
            var temp = months.Where(x => x.Project == "Accounting").ToList();

            return months;
        }

        private FullMonth addValues(FullMonth thisMonth, Transactions tran)
        {

            var type = typeof(FullMonth);
            var members = type.GetMembers();
            var mth = tran.Date.Day.ToString();
            for (int c = 0; c < members.Length; c++)
            {
                var member = members[c];
                Object[] objects = member.GetCustomAttributes(typeof(MyAttribute), true);
                if (objects != null && objects.Length > 0)
                {
                    var fieldAttrib = (MyAttribute)objects[0];
                    if (fieldAttrib.Name == mth)
                    {
                        ((PropertyInfo)member).SetValue(thisMonth, tran.Hours.Trim());
                        break;
                    }
                }

            }
            return thisMonth;
        }

        public int CalculateExtraFreezeColumns(DateTime frozenDate, DateTime startDate, DateTime endDate)
        {
            if( startDate < frozenDate)
            {
                TimeSpan difference = frozenDate - startDate;   
                int numberofDays = difference.Days;
                return numberofDays ;
            }
            return 0;
        }
    }

}
