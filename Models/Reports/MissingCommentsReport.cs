namespace TimeTrackerRepo.Models.Reports
{
    public class MissingCommentsReport
    {
        public MissingCommentsReport()
        {
        }

        public MissingCommentsReport(
            int employeeNumber,
            string firstName,
            string lastName,
            string client,
            string project,
            string activity,
            DateTime date,
            string hours,
            string comment)
        {
            EmployeeNumber = employeeNumber;
            FirstName = firstName;
            LastName = lastName;
            Client = client;
            Project = project;
            Activity = activity;
            Date = date;
            Hours = hours;
            Comment = comment;
        }

        public int EmployeeNumber { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string Client { get; set; } = string.Empty;

        public string Project { get; set; } = string.Empty;

        public string Activity { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public string Hours { get; set; } = string.Empty;

        public string Comment { get; set; } = string.Empty;
    }
}