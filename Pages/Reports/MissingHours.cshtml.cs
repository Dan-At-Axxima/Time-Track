using System;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TimeTrackerRepo.Models.Reports;
using TimeTrackerRepo.Services.Reports;

namespace TimeTrackerRepo.Pages.Reports
{
    [Authorize(Roles = "Administrator")]
    public class MissingHoursModel : PageModel
    {
        private readonly MissingHoursReportService _missingHoursReportService;
        private readonly MissingHoursExcelExporter _missingHoursExcelExporter;

        public MissingHoursModel(
            MissingHoursReportService missingHoursReportService,
            MissingHoursExcelExporter missingHoursExcelExporter)
        {
            _missingHoursReportService = missingHoursReportService;
            _missingHoursExcelExporter = missingHoursExcelExporter;
        }

        [BindProperty(SupportsGet = true)]
        public DateTime StartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime EndDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool IncludeAllocations { get; set; }

        public List<TimeStructure> Rows { get; set; } = new();

        public string? StatusMessage { get; set; }

        public void OnGet()
        {
            ApplyDefaults();

            if (EndDate < StartDate)
            {
                StatusMessage = "End Date cannot be before Start Date.";
                return;
            }

            Rows = _missingHoursReportService.GetMissingHours(StartDate, EndDate);

            if (Rows.Count == 0)
            {
                StatusMessage = $"There are no missing hours for the period: {StartDate:yyyy/MM/dd} To {EndDate:yyyy/MM/dd}";
            }
        }

        public IActionResult OnPostExportToExcel()
        {
            ApplyDefaults();

            if (EndDate < StartDate)
            {
                StatusMessage = "End Date cannot be before Start Date.";
                Rows = new List<TimeStructure>();
                return Page();
            }

            var rows = _missingHoursReportService.GetMissingHours(StartDate, EndDate);
            var fileBytes = _missingHoursExcelExporter.CreateMissingHoursExcel(rows);
            var fileName = $"MissingHours_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            return File(
                fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }

        private void ApplyDefaults()
        {
            if (StartDate == default || EndDate == default)
            {
                var today = DateTime.Today;
                StartDate = new DateTime(today.Year, today.Month, 1);
                EndDate = today;
            }
        }
    }
}