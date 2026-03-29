namespace TimeTrackerRepo.Models.Reports
{
    public class EmployeeWithHoursPerDay
    {
        public EmployeeWithHoursPerDay()
        {
        }

        public EmployeeWithHoursPerDay(int employeeNumber, string firstName, string lastName, string eMail, bool active, int hoursPerDay)
        {
            EmployeeNumber = employeeNumber;
            FirstName = firstName;
            LastName = lastName;
            EMail = eMail;
            Active = active;
            HoursPerDay = hoursPerDay;
        }

        public int EmployeeNumber { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string EMail { get; set; } = string.Empty;

        public bool Active { get; set; }

        public int HoursPerDay { get; set; }
    }
}