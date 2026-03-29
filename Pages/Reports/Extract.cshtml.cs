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
    public class ExtractModel : PageModel
    {
        private readonly ExtractReportService _extractReportService;
        private readonly ExtractExcelExporter _extractExcelExporter;

        public ExtractModel(
            ExtractReportService extractReportService,
            ExtractExcelExporter extractExcelExporter)
        {
            _extractReportService = extractReportService;
            _extractExcelExporter = extractExcelExporter;
        }

        [BindProperty(SupportsGet = true)]
        public DateTime StartDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public DateTime EndDate { get; set; }

        [BindProperty(SupportsGet = true)]
        public bool IncludeAllocations { get; set; }

        public List<WipDetailReportData> Rows { get; set; } = new();

        public string? StatusMessage { get; set; }

        public DateTime GeneratedAt { get; set; }

        public void OnGet()
        {
            ApplyDefaults();
            GeneratedAt = DateTime.Now;

            if (EndDate < StartDate)
            {
                StatusMessage = "End Date cannot be before Start Date.";
                Rows = new List<WipDetailReportData>();
                return;
            }

            LoadRows();

            if (Rows.Count == 0)
            {
                StatusMessage = $"There is no extract data for the period: {StartDate:yyyy/MM/dd} To {EndDate:yyyy/MM/dd}";
            }
        }

        public IActionResult OnPostExport()
        {
            ApplyDefaults();

            if (EndDate < StartDate)
            {
                StatusMessage = "End Date cannot be before Start Date.";
                Rows = new List<WipDetailReportData>();
                return Page();
            }

            var rows = _extractReportService.GenerateExtract(StartDate, EndDate);
            var fileBytes = _extractExcelExporter.CreateExtractExcel(rows);
            var fileName = $"Extract_{DateTime.Now:yyyyMMdd_HHmmss}.xlsx";

            return File(
                fileBytes,
                "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
                fileName);
        }

        private void LoadRows()
        {
            Rows = _extractReportService.GenerateExtract(StartDate, EndDate);
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