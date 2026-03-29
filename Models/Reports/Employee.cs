namespace TimeTrackerRepo.Models.Reports
{
    public class Employee
    {
        public Employee()
        {
        }

        public Employee(int employeeNumber, string firstName, string lastName, string eMail, bool active)
        {
            EmployeeNumber = employeeNumber;
            FirstName = firstName;
            LastName = lastName;
            EMail = eMail;
            Active = active;
        }

        public int EmployeeNumber { get; set; }

        public string FirstName { get; set; } = string.Empty;

        public string LastName { get; set; } = string.Empty;

        public string EMail { get; set; } = string.Empty;

        public bool Active { get; set; }
    }
}