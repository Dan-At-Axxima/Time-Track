namespace TimeTrackerRepo.Models.Reports
{
    public class TimeStructure
    {
        public TimeStructure()
        {
        }

        public TimeStructure(int employee, string firstName, string lastName, DateTime date, string hours)
        {
            EmployeeNumber = employee;
            FirstName = firstName;
            LastName = lastName;
            Date = date;
            Hours = hours;
        }

        public int EmployeeNumber { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public DateTime Date { get; set; }

        public string Hours { get; set; } = string.Empty;
    }
}