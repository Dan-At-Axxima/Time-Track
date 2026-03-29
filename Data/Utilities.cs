using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TimeTrackerRepo.Data
{
    public class Utilities
    {

        public List<string> AllDates(DateTime startDate, DateTime endDate)
        {
            List<string> dates = Enumerable.Range(0, (endDate - startDate).Days + 1)
                    .Select(i => (startDate.AddDays(i)).Date.ToString("yyyy-MM-dd"))
                    .ToList();

            string daysOfTheMonthString = ",,,";

            int days = dates.Count();
            for (int i = 0; i < days; i++)
            {
                daysOfTheMonthString += ",";
                daysOfTheMonthString += dates[i];     // new DateTime(year, month, i + 1).ToString("dddd") + "\n" + monthString + " " + (i + 1).ToString();

            }
            return dates;

        }

        public string getColumnTitles(DateTime startDate,DateTime endDate)
        {
            var columnTitles = "Client,Project,Activity,";
            // tring formattedDate = date.ToUniversalTime().ToString("yyyy-MM-dd'T'HH:mm:ss.fff'Z'", CultureInfo.InvariantCulture);
            List<string> dates = Enumerable.Range(0, (endDate - startDate).Days + 1)
                  .Select(i => (startDate.AddDays(i)).Date.ToUniversalTime().ToString("yyyy-MM-dd"))
//                  .Select(i => (startDate.AddDays(i)).Date.ToString("ddd\nMMM d"))
                  .ToList();
            foreach(string date in dates)
            {
                columnTitles += ("," + date);
            }
           
            return columnTitles;
        }

        public string getAllDates(DateTime startDate, DateTime endDate)
        {
            // place holders for title columns
            var allDates = ","+","+",";

            List<string> dates = Enumerable.Range(0, (endDate - startDate).Days + 1)
                  .Select(i => (startDate.AddDays(i)).Date.ToString("yyyy-MM-dd"))
                  .ToList();
            foreach (string date in dates)
            {
                allDates += ("," + date);
            }

            return allDates;
        }





    }
}
