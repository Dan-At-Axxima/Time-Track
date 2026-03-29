using System.ComponentModel.DataAnnotations;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TimeTrackerRepo.Services.Reports;

namespace TimeTrackerRepo.Pages.Reports
{
    [Authorize(Roles = "Administrator")]
    public class IndexModel : PageModel
    {
        private readonly ExtractReportService _extractReportService;
        private readonly ExtractExcelExporter _extractExcelExporter;

        public IndexModel(
            ExtractReportService extractReportService,
            ExtractExcelExporter extractExcelExporter)
        {
            _extractReportService = extractReportService;
            _extractExcelExporter = extractExcelExporter;
        }

        [BindProperty]
        [DataType(DataType.Date)]
        [Display(Name = "Start Date")]
        public DateTime StartDate { get; set; }

        [BindProperty]
        [DataType(DataType.Date)]
        [Display(Name = "End Date")]
        public DateTime EndDate { get; set; }

        [BindProperty]
        [Display(Name = "Include Allocations")]
        public bool IncludeAllocations { get; set; }

        public string? StatusMessage { get; set; }

        public void OnGet()
        {
            SetDefaultDates();
            IncludeAllocations = true;
        }

        public void OnPost()
        {
            if (StartDate == default || EndDate == default)
            {
                SetDefaultDates();
                IncludeAllocations = true;
                StatusMessage = "Default dates were applied.";
                return;
            }

            if (EndDate < StartDate)
            {
                ModelState.AddModelError(string.Empty, "End Date cannot be before Start Date.");
                return;
            }

            StatusMessage = $"Selected: {StartDate:yyyy-MM-dd} to {EndDate:yyyy-MM-dd} | Include Allocations: {IncludeAllocations}";
        }

        public IActionResult OnGetExtract(DateTime startDate, DateTime endDate, bool includeAllocations)
        {
            if (startDate == default || endDate == default)
            {
                var today = DateTime.Today;
                startDate = new DateTime(today.Year, today.Month, 1);
                endDate = today;
            }

            if (endDate < startDate)
            {
                return RedirectToPage("/Reports/Index");
            }

            var rows = _extractReportService.GenerateExtract(startDate, endDate);
            var fileBytes = _extractExcelExporter.CreateExtractExcel(rows);
            var fileName = $"Extract_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            return File(
                fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }

        private void SetDefaultDates()
        {
            var today = DateTime.Today;
            var firstDayOfMonth = new DateTime(today.Year, today.Month, 1);
            var lastDayOfMonth = firstDayOfMonth.AddMonths(1).AddDays(-1);
            var defaultEndDate = today <= lastDayOfMonth ? today : lastDayOfMonth;

            StartDate = firstDayOfMonth;
            EndDate = defaultEndDate;
        }
    }
}