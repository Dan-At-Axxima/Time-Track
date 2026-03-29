using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace TimeTrackerRepo.Services.Reports.Legacy
{
    public static class Helpers
    {
        public enum ReportTypes
        {
            MissingHours,
            MissingComments
        }

        internal static List<T> ConvertDataTable<T>(DataTable dt)
        {
            List<T> data = new List<T>();

            foreach (DataRow row in dt.Rows)
            {
                T item = GetItem<T>(row);
                data.Add(item);
            }

            return data;
        }

        internal static T GetItem<T>(DataRow dr)
        {
            Type temp = typeof(T);
            T obj = Activator.CreateInstance<T>();

            foreach (DataColumn column in dr.Table.Columns)
            {
                foreach (PropertyInfo pro in temp.GetProperties())
                {
                    if (pro.Name == Regex.Replace(column.ColumnName, @"\s+", ""))
                    {
                        var value = dr[column.ColumnName];

                        if (value == DBNull.Value)
                        {
                            value = null;
                        }

                        pro.SetValue(obj, value, null);
                        break;
                    }
                }
            }

            return obj;
        }

        internal static List<DateTime> WokingDaysOfTheMonth(DateTime startDate, DateTime endDate)
        {
            var dates = Enumerable.Range(0, 1 + endDate.Subtract(startDate).Days)
                .Select(offset => startDate.AddDays(offset))
                .ToArray();

            List<DateTime> workingDays = new List<DateTime>();

            foreach (DateTime dt in dates)
            {
                if (dt.DayOfWeek != DayOfWeek.Saturday && dt.DayOfWeek != DayOfWeek.Sunday)
                {
                    workingDays.Add(dt);
                }
            }

            return workingDays;
        }

        internal static bool WorkDays(DateTime tm)
        {
            if (tm.DayOfWeek != DayOfWeek.Saturday && tm.DayOfWeek != DayOfWeek.Sunday)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        internal static string ConvertSecondsToHoursMinutes(double seconds)
        {
            TimeSpan time = TimeSpan.FromSeconds(seconds);
            string str = time.ToString(@"hh\:mm\:ss\:fff");
            return str;
        }

        internal static double CalcSeconds(string hours, double percentage)
        {
            double seconds = 0;

            if (hours.Contains(':'))
            {
                if (hours[0] == ':')
                {
                    hours = "0" + hours;
                }

                seconds = TimeSpan.Parse(hours).TotalSeconds;
            }
            else
            {
                double secs;
                double hrs;

                double.TryParse(hours.Split('.').Last(), out secs);
                double.TryParse(hours.Split('.').First(), out hrs);

                double first = ((secs / 100) * 60) * 60;
                double third = hrs * 3600;
                double fifth = first + third;
                seconds = fifth;
            }

            if (percentage != 0)
            {
                seconds = seconds * percentage;
            }

            if (seconds == .01)
            {
                seconds = .01;
            }

            return seconds;
        }
    }
}