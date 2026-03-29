using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using TimeTrackerRepo.Data;
using TimeTrackerRepo.Models;

namespace TimeTrackerRepo.Pages
{
    public class EditModel : PageModel
    {
        private readonly TimeTrackerContext _context;

        public EditModel(TimeTrackerContext context)
        {
            _context = context;
        }

        [BindProperty]
        public EmployeeAndRatesWithHours EmployeeAndRates { get; set; }

        public async Task<IActionResult> OnGetAsync(int id)
        {
            int employeeNumber = id;
            // Fetch the data for the specified employee number
            var employee = await _context.Employee.FirstOrDefaultAsync(e => e.EmployeeNumber == employeeNumber);
            if (employee == null)
            {
                return NotFound();
            }

            // Populate EmployeeAndRates based on the existing data
            EmployeeAndRates = new EmployeeAndRatesWithHours
            {
                FirstName = employee.FirstName,
                LastName = employee.LastName,
                EmployeeNumber = employee.EmployeeNumber,
                Active = employee.Active,
                Email = employee.EMail,
                HoursPerDay = (await _context.NewSettings.FirstOrDefaultAsync(s => s.EmployeeNumber == employee.EmployeeNumber.ToString()))?.HoursPerDay ?? 8,
                DDARates = (await _context.Rates.FirstOrDefaultAsync(r => r.EmployeeNumber == employee.EmployeeNumber))?.DDARates ?? 0,
                AxximaRates = (await _context.Rates.FirstOrDefaultAsync(r => r.EmployeeNumber == employee.EmployeeNumber))?.AxximaRates ?? 0
            };

            return Page();
        }

        public IActionResult OnPostCancel()
        {
            return RedirectToPage("./AdminFunctions");
            // Redirect to the page you want to navigate to when "Cancel" is clicked
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (!ModelState.IsValid)
            {
                return Page();
            }

            // Fetch the employee, rates, and settings records to update
            var employee = await _context.Employee.FirstOrDefaultAsync(e => e.EmployeeNumber == EmployeeAndRates.EmployeeNumber);
            var rates = await _context.Rates.FirstOrDefaultAsync(r => r.EmployeeNumber == EmployeeAndRates.EmployeeNumber);
            var settings = await _context.NewSettings.FirstOrDefaultAsync(s => s.EmployeeNumber == EmployeeAndRates.EmployeeNumber.ToString());

            if (employee == null || rates == null || settings == null)
            {
                return NotFound();
            }

            // Update the employee data
            employee.FirstName = EmployeeAndRates.FirstName;
            employee.LastName = EmployeeAndRates.LastName;
            employee.Active = EmployeeAndRates.Active;
            employee.EMail = EmployeeAndRates.Email;

            // Update the rates data
            rates.DDARates = EmployeeAndRates.AxximaRates;
            rates.AxximaRates = EmployeeAndRates.AxximaRates;

            // Update the settings data
            settings.HoursPerDay = EmployeeAndRates.HoursPerDay;

            await _context.SaveChangesAsync();

            return RedirectToPage("./AdminFunctions");
        }
    }
}
