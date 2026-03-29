using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace TimeTrackerRepo.Pages
{
    public class ReportsModel : PageModel
    {

        public string startDate;
        public string endDate;
        public void OnGet()
        {
            //DefaultStartDate = new DateTime(DateTime.Now.Year, DateTime.Now.Month, 1).ToString("yyyy,MM,dd");
            var date = DateTime.Now;
            var tempStartDate = new DateTime(date.Year, date.Month, 1);
            var tempEndDate = tempStartDate.AddMonths(1).AddDays(-1);
            startDate = tempStartDate.ToString("yyyy-MM-dd"); 
            endDate = tempEndDate.ToString("yyyy-MM-dd");
        }

        public IActionResult OnPost(string report)
        {
            // Code to generate the report based on the value of the 'report' parameter
            // ...

            // Return a view or file containing the report data
            //  return File(reportData, "application/pdf", "Report.pdf");
            return ViewComponent(report);

        }
    }
}
