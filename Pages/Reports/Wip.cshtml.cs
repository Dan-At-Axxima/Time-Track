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

    public WipModel(WipDetailReportService service)
    {
        _service = service;
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
        CompanyCode == 1 ? "WIP Company 1" : "WIP Company 2";

    public async Task OnGetAsync()
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
}