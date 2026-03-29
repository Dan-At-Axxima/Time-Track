using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using TimeTrackerRepo.Models.Reports;
using TimeTrackerRepo.Services.Reports.Legacy;

namespace TimeTrackerRepo.Services.Reports
{
    public class MissingHoursReportService
    {
        private readonly SQLFunctions _sqlFunctions;

        public MissingHoursReportService(SQLFunctions sqlFunctions)
        {
            _sqlFunctions = sqlFunctions;
        }

        public List<TimeStructure> GetMissingHours(DateTime startDate, DateTime endDate)
        {
            DataSet allHours = _sqlFunctions.GetAllHours(startDate, endDate);
            if (allHours == null || allHours.Tables.Count == 0)
            {
                return new List<TimeStructure>();
            }

            DataTable dt = allHours.Tables[0];
            var allHrsList = Helpers.ConvertDataTable<TimeStructure>(dt);

            List<TimeStructure> weekendDays = new List<TimeStructure>();
            foreach (TimeStructure ts in allHrsList)
            {
                if (!Helpers.WorkDays(ts.Date))
                {
                    weekendDays.Add(ts);
                }
            }

            if (weekendDays.Count > 0)
            {
                foreach (TimeStructure ts in weekendDays)
                {
                    allHrsList.Remove(ts);
                }
            }

            var distinctDates = allHrsList
                .GroupBy(p => p.Date.Date)
                .Select(g => g.First())
                .ToList();

            var allEmployeesDs = _sqlFunctions.GetAllActiveEmployees();
            if (allEmployeesDs == null || allEmployeesDs.Tables.Count == 0)
            {
                return new List<TimeStructure>();
            }

            dt = allEmployeesDs.Tables[0];
            var allEmployeesList = Helpers.ConvertDataTable<Employee>(dt);

            var employeesWithHoursPerDayDs = _sqlFunctions.GetAllActiveEmployeesWithHoursPerDay();
            if (employeesWithHoursPerDayDs == null || employeesWithHoursPerDayDs.Tables.Count == 0)
            {
                return new List<TimeStructure>();
            }

            dt = employeesWithHoursPerDayDs.Tables[0];
            var allEmployeesWithHoursPerDayList = Helpers.ConvertDataTable<EmployeeWithHoursPerDay>(dt);

            List<TimeStructure> noHours = new List<TimeStructure>();

            foreach (EmployeeWithHoursPerDay tm in allEmployeesWithHoursPerDayList)
            {
                foreach (TimeStructure ts in distinctDates)
                {
                    var res = allHrsList.Count(p => p.Date.Date == ts.Date.Date && p.EmployeeNumber == tm.EmployeeNumber);
                    if (res == 0)
                    {
                        noHours.Add(new TimeStructure(tm.EmployeeNumber, tm.FirstName, tm.LastName, ts.Date.Date, "0"));
                    }
                }
            }

            allHrsList.AddRange(noHours);

            allHrsList = allHrsList
                .OrderBy(x => x.Date)
                .ThenBy(x => x.EmployeeNumber)
                .ToList();

            if (allHrsList.Count == 0)
            {
                return new List<TimeStructure>();
            }

            int employee = allHrsList[0].EmployeeNumber;
            string firstName = allHrsList[0].FirstName;
            string lastName = allHrsList[0].LastName;
            string thisDate = allHrsList[0].Date.ToString("yyyyMMdd");
            DateTime actualDate = allHrsList[0].Date;
            double seconds = 0;

            List<TimeStructure> missingHours = new List<TimeStructure>();

            foreach (TimeStructure time in allHrsList)
            {
                string date = time.Date.ToString("yyyyMMdd");

                if (thisDate != date || employee != time.EmployeeNumber)
                {
                    var employeeHours = allEmployeesWithHoursPerDayList
                        .FirstOrDefault(x => x.EmployeeNumber == employee);

                    if (employeeHours != null)
                    {
                        var hoursPerDayInSeconds = employeeHours.HoursPerDay * 3600.00;

                        if (seconds < hoursPerDayInSeconds)
                        {
                            var t = Helpers.ConvertSecondsToHoursMinutes(seconds);
                            var rec = new TimeStructure(employee, firstName, lastName, actualDate, t);
                            missingHours.Add(rec);
                        }
                    }

                    thisDate = date;
                    employee = time.EmployeeNumber;
                    seconds = 0;
                    firstName = time.FirstName;
                    lastName = time.LastName;
                    actualDate = time.Date;
                }

                seconds += Helpers.CalcSeconds(time.Hours, 0);

                if (time == allHrsList[allHrsList.Count - 1])
                {
                    var employeeHours = allEmployeesWithHoursPerDayList
                        .FirstOrDefault(x => x.EmployeeNumber == employee);

                    if (employeeHours != null)
                    {
                        var hoursPerDayInSeconds = employeeHours.HoursPerDay * 3600.00;

                        if (seconds < hoursPerDayInSeconds)
                        {
                            var t = Helpers.ConvertSecondsToHoursMinutes(seconds);
                            var rec = new TimeStructure(employee, firstName, lastName, actualDate, t);
                            missingHours.Add(rec);
                        }
                    }
                }
            }

            return missingHours;
        }
    }
}