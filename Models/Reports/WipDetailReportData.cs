using System;
using System.Linq;

namespace TimeTrackerRepo.Models.Reports
{
    public class WipDetailReportData
    {
        public double Multiple { get; set; }

        public double AxximaCompanyCodes { get; set; }

        public double DdaRates { get; set; }

        public double AxximaRates { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public int EmployeeNumber { get; set; }

        public string Client { get; set; } = string.Empty;

        public string Project { get; set; } = string.Empty;

        public string Activity { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public string Hours { get; set; } = string.Empty;

        public string Comment { get; set; } = string.Empty;

        public string SlipId { get; set; } = string.Empty;

        public double Seconds { get; set; }

        public string DecimalHours
        {
            get
            {
                if (!string.IsNullOrEmpty(Hours) && Hours.Contains(":"))
                {
                    return ConvertToDecimalTime(Hours);
                }

                return Hours;
            }
        }

        public TimeSpan OldTime
        {
            get
            {
                if (!string.IsNullOrEmpty(Hours) && Hours.Contains(":"))
                {
                    string tmp = string.Concat(Hours.Trim(), ":00");

                    if (tmp.Substring(0, 1) == ":")
                    {
                        tmp = string.Concat("0", tmp);
                    }

                    if (tmp.Length == 7)
                    {
                        tmp = string.Concat("0:0", tmp);
                    }

                    var tmp2 = TimeSpan.Parse(tmp);
                    return tmp2;
                }

                return new TimeSpan();
            }
        }

        public TimeSpan Time
        {
            get
            {
                if (!string.IsNullOrEmpty(DecimalHours))
                {
                    string tmp = DecimalHours.Trim();

                    if (tmp.Substring(0, 1) == ".")
                    {
                        tmp = string.Concat("0", tmp);
                    }

                    if (tmp.Length == 7)
                    {
                        tmp = string.Concat("0.0", tmp);
                    }

                    var tmp2 = AddTime(tmp, true);
                    var tmp3 = new TimeSpan(0, tmp2, 0);
                    return tmp3;
                }

                return new TimeSpan();
            }
        }

        internal string ConvertToDecimalTime(string hours)
        {
            string[] time = hours.Split(':');
            string minutes = time[1];

            int min1 = 0;
            int.TryParse(minutes, out min1);

            double results = 0;
            if (min1 != 0)
            {
                results = (double)min1 / 60d;
            }

            string decimalValue = results.ToString("N2").Split('.').Last();
            string newValue = time[0] + '.' + decimalValue;

            return newValue;
        }

        private int AddTime(string value, bool decimalTime)
        {
            string hrs;
            string mins;
            int hours;
            int minutes;

            if (decimalTime)
            {
                hrs = value.Split('.').First();
                mins = value.Split('.').Last();
            }
            else
            {
                hrs = value.Split(':').First();
                mins = value.Split(':').Last();
            }

            int.TryParse(hrs, out hours);
            int.TryParse(mins, out minutes);

            if (minutes != 0 && decimalTime)
            {
                double tempMinutes = (double)minutes * 0.60;
                string nString = tempMinutes.ToString("N0");
                int.TryParse(nString, out minutes);
            }

            int totalMinutes = (hours * 60) + minutes;
            return totalMinutes;
        }
    }
}