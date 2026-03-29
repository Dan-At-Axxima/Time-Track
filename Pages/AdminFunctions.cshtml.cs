using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using TimeTrackerRepo.Data;
using TimeTrackerRepo.Models;

namespace TimeTrackerRepo.Pages
{
    public class AdminFunctionsModel : PageModel
    {
        private readonly TimeTrackerContext _context;
        private string _filter = "Active";
        private List<EmployeeAndRates> _employeeAndRates;
        public AdminFunctionsModel(TimeTrackerContext context)
        {
            _context = context;
            
        }

        public List<EmployeeAndRatesWithHours> EmployeeAndRatesList{ get; set; }
        [BindProperty(SupportsGet = true)]
        public string Filter
        {
            get => _filter;
            set => _filter = value ?? "Active";
        }

        public void OnGet()
        {
            try
            {
                List<EmployeeAndRatesWithHours> query = _context.EmployeeAndRatesWithHours.ToList();

                //        List<EmployeeAndRates> query = _context.EmployeeAndRates
                //.FromSqlRaw("SELECT * FROM EmployeeAndRates")
                //.AsNoTracking().Take(10).Take(10)
                //.ToList();
                switch (_filter)
                {
                    case "Active":
                        query = query.Where(e => e.Active && e.EmployeeNumber<9000).ToList();
                        break;
                    case "NonActive":
                        query = query.Where(e => !e.Active && e.EmployeeNumber < 9000).ToList();
                        break;
                    case "All":
                        query = query.Where(e => e.EmployeeNumber < 9000).ToList();
                        break;
                }

                EmployeeAndRatesList = query.ToList();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.ToString());
            }
        }
    }
}
