using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TimeTrackerRepo.Models.Reports;
using TimeTrackerRepo.Services.Reports;

namespace TimeTrackerRepo.Pages.Reports;
//test comment 
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

    public List<WipDetailReportRow> Rows { get; set; } = new();

    public DateTime GeneratedAt { get; set; }

    public string StatusMessage { get; set; } = "";

    public async Task OnGetAsync()
    {
        GeneratedAt = DateTime.Now;

        Rows = await _service.BuildReportDataAsync(
            companyCode: 2,
            startDate: StartDate,
            endDate: EndDate,
            withAllocation: WithAllocation,
            ifrsOnly: IFRSOnly,
            oneClientOnly: false,
            clientCode: null);

        StatusMessage = $"{Rows.Count} records loaded.";
    }

    public async Task<IActionResult> OnPostExportAsync()
    {
        var rows = await _service.BuildReportDataAsync(
            companyCode: 2,
            startDate: StartDate,
            endDate: EndDate,
            withAllocation: WithAllocation,
            ifrsOnly: IFRSOnly,
            oneClientOnly: false,
            clientCode: null);

        var fileBytes = _exporter.ExportSingleCompanyReport(
            "Axxima WIP DETAIL REPORT",
            rows);

        return File(
            fileBytes,
            "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet",
            $"WipDetail_{DateTime.Now:yyyyMMddHHmmss}.xlsx");
    }
}