using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TimeTrackerRepo.Models.Reports;
using TimeTrackerRepo.Services.Reports;

namespace TimeTrackerRepo.Pages.Reports;

[Authorize(Roles = "Administrator")]
public class WipModel : PageModel
{
    private readonly WipDetailReportService _service;
    private readonly WipDetailExcelExporter _exporter;

    public WipModel(
        WipDetailReportService service,
        WipDetailExcelExporter exporter)
    {
        _service = service;
        _exporter = exporter;
    }

    [BindProperty(SupportsGet = true)]
    public DateTime StartDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public DateTime EndDate { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool WithAllocation { get; set; }

    [BindProperty(SupportsGet = true)]
    public bool IFRSOnly { get; set; }

    [BindProperty(SupportsGet = true)]
    public int CompanyCode { get; set; } = 2;

    public List<WipDetailReportData> Rows { get; set; } = new();

    public string StatusMessage { get; set; } = string.Empty;

    public string ReportTitle =>
        CompanyCode == 1
            ? "AXXIMA WIP DETAIL REPORT"
            : "330 WIP DETAIL REPORT";

    public async Task OnGetAsync()
    {
        SetDefaults();

        Rows = await _service.GetDetailRowsAsync(
            companyCode: CompanyCode,
            startDate: StartDate,
            endDate: EndDate,
            withAllocation: WithAllocation,
            ifrsOnly: IFRSOnly,
            oneClientOnly: false,
            clientCode: null);

        StatusMessage = $"{Rows.Count} detail rows loaded.";
    }

    public async Task<IActionResult> OnPostExportAsync()
    {
        SetDefaults();

        var rows = await _service.GetDetailRowsAsync(
            companyCode: CompanyCode,
            startDate: StartDate,
            endDate: EndDate,
            withAllocation: WithAllocation,
            ifrsOnly: IFRSOnly,
            oneClientOnly: false,
            clientCode: null);

        if (rows.Count == 0)
        {
            Rows = rows;
            StatusMessage = "No records found for the selected criteria.";
            return Page();
        }

        var fileBytes = _exporter.ExportSingleCompanyReport(ReportTitle, rows);

        var fileName = CompanyCode == 1
            ? $"WipDetail_Axxima_{DateTime.Now:yyyyMMddHHmmss}.xlsx"
            : $"WipDetail_330_{DateTime.Now:yyyyMMddHHmmss}.xlsx";

        return File(
            fileBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            fileName);
    }

    private void SetDefaults()
    {
        if (StartDate == default)
        {
            StartDate = new DateTime(DateTime.Today.Year, DateTime.Today.Month, 1);
        }

        if (EndDate == default)
        {
            EndDate = DateTime.Today;
        }

        if (CompanyCode != 1 && CompanyCode != 2)
        {
            CompanyCode = 2;
        }
    }
}