using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TimeTrackerRepo.Data;
using TimeTrackerRepo.Models;

namespace TimeTrackerRepo.Pages
{
    public class CreateModel : PageModel
    {
        private readonly TimeTrackerContext _context;

        public CreateModel(TimeTrackerContext context)
        {
            _context = context;
        }

        [BindProperty]
        public EmployeeAndRatesWithHours EmployeeAndRates { get; set; }

        public IActionResult OnGet()
        {
            var val = GetMaxEmployeeNumberBelow(1145) + 1;
            EmployeeAndRates = new EmployeeAndRatesWithHours()
            {
                Active = true,
                EmployeeNumber = val ?? 0,
                HoursPerDay = 8
                
            };

            return Page();
        }

        public IActionResult OnPostCancel()
        {
            // Redirect to the page you want to navigate to when "Cancel" is clicked
            return RedirectToPage("AdminFunctions"); // Redirect to the Index page, for example
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            Employee employee = new Employee();
            employee.FirstName = EmployeeAndRates.FirstName;
            employee.LastName = EmployeeAndRates.LastName;
            employee.EmployeeNumber = EmployeeAndRates.EmployeeNumber;
            employee.Active = EmployeeAndRates.Active;
            employee.EMail = EmployeeAndRates.Email;
            _context.Employee.Add(employee);
            await _context.SaveChangesAsync();

            Rates rates = new Rates();
            rates.EmployeeNumber = EmployeeAndRates.EmployeeNumber; 
            rates.DDARates = EmployeeAndRates.DDARates;
            rates.AxximaRates = EmployeeAndRates.DDARates; 

            _context.Rates.Add(rates);
            await _context.SaveChangesAsync();

            NewSettings settings = new NewSettings();
            settings.EmployeeNumber = EmployeeAndRates.EmployeeNumber.ToString();
            settings.StartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1);
            settings.EndDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).AddMonths(1).AddDays(-1);
            settings.HoursPerDay = EmployeeAndRates.HoursPerDay;
            _context.NewSettings.Add(settings);
            await _context.SaveChangesAsync();

            Assignments assignments = new Assignments();
            assignments.EmployeeNumber=employee.EmployeeNumber;
            assignments.Client = "00001";
            assignments.Activity = "General";
            assignments.Project = "General";
            _context.Assignments.Add(assignments);
            _context.SaveChanges();

            return RedirectToPage("./AdminFunctions");
        }

        private int? GetMaxEmployeeNumberBelow(int maxNumber)
        {
            return _context.Employee
                .Where(e => e.EmployeeNumber < maxNumber)
                .Max(e => (int?)e.EmployeeNumber);
        }
    }
}
