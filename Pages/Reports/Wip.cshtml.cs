using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System;
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
    public List<WipDisplayRow> DisplayRows { get; set; } = new();

    public string StatusMessage { get; set; } = string.Empty;

    public string ReportTitle =>
        CompanyCode == 1
            ? "AXXIMA WIP DETAIL REPORT"
            : "330 WIP DETAIL REPORT";

    public async Task OnGetAsync()
    {
        SetDefaults();
        Console.WriteLine($"EXPORT CompanyCode = {CompanyCode}");
        Rows = await _service.GetDetailRowsAsync(
            companyCode: CompanyCode,
            startDate: StartDate,
            endDate: EndDate,
            withAllocation: WithAllocation,
            ifrsOnly: IFRSOnly,
            oneClientOnly: false,
            clientCode: null);

        DisplayRows = BuildDisplayRows(Rows);
        StatusMessage = $"{Rows.Count} detail rows loaded.";
    }

    public async Task<IActionResult> OnGetExportAsync()
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
            DisplayRows = BuildDisplayRows(rows);
            StatusMessage = "No records found for the selected criteria.";
            return Page();
        }

        var displayRows = BuildDisplayRows(rows);
        var fileBytes = _exporter.ExportSingleCompanyReport(ReportTitle, displayRows);

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

    private static List<WipDisplayRow> BuildDisplayRows(List<WipDetailReportData> rows)
    {
        var displayRows = new List<WipDisplayRow>();

        var orderedRows = rows
            .OrderBy(x => x.Client)
            .ThenBy(x => x.Project)
            .ThenBy(x => x.Activity)
            .ThenBy(x => x.EmployeeNumber)
            .ThenBy(x => x.Date)
            .ToList();

        string? currentClient = null;
        string? currentActivity = null;
        int? currentEmployee = null;

        double assocSeconds = 0;
        double activitySeconds = 0;
        double clientSeconds = 0;
        double grandSeconds = 0;

        double assocAxxima = 0;
        double activityAxxima = 0;
        double clientAxxima = 0;
        double grandAxxima = 0;

        void ResetAssociateTotals()
        {
            assocSeconds = 0;
            assocAxxima = 0;
        }

        void ResetActivityTotals()
        {
            activitySeconds = 0;
            activityAxxima = 0;
        }

        void ResetClientTotals()
        {
            clientSeconds = 0;
            clientAxxima = 0;
        }

        foreach (var row in orderedRows)
        {
            if (currentClient is null)
            {
                currentClient = row.Client;
                currentActivity = row.Activity;
                currentEmployee = row.EmployeeNumber;
            }

            var clientChanged = currentClient != row.Client;
            var activityChanged = !clientChanged && currentActivity != row.Activity;
            var employeeChanged = !clientChanged && !activityChanged && currentEmployee != row.EmployeeNumber;

            if (clientChanged)
            {
                displayRows.Add(new WipDisplayRow { RowType = WipDisplayRowType.Divider });
                displayRows.Add(CreateTotalRow(WipDisplayRowType.AssociateTotal, "Associate Total", assocSeconds, assocAxxima));
                displayRows.Add(CreateTotalRow(WipDisplayRowType.ActivityTotal, "Activity Total", activitySeconds, activityAxxima));
                displayRows.Add(CreateTotalRow(WipDisplayRowType.ClientTotal, "Client Total", clientSeconds, clientAxxima));

                ResetAssociateTotals();
                ResetActivityTotals();
                ResetClientTotals();

                currentClient = row.Client;
                currentActivity = row.Activity;
                currentEmployee = row.EmployeeNumber;
            }
            else if (activityChanged)
            {
                displayRows.Add(new WipDisplayRow { RowType = WipDisplayRowType.Divider });
                displayRows.Add(CreateTotalRow(WipDisplayRowType.AssociateTotal, "Associate Total", assocSeconds, assocAxxima));
                displayRows.Add(CreateTotalRow(WipDisplayRowType.ActivityTotal, "Activity Total", activitySeconds, activityAxxima));

                ResetAssociateTotals();
                ResetActivityTotals();

                currentActivity = row.Activity;
                currentEmployee = row.EmployeeNumber;
            }
            else if (employeeChanged)
            {
                displayRows.Add(new WipDisplayRow { RowType = WipDisplayRowType.Divider });
                displayRows.Add(CreateTotalRow(WipDisplayRowType.AssociateTotal, "Associate Total", assocSeconds, assocAxxima));

                ResetAssociateTotals();

                currentEmployee = row.EmployeeNumber;
            }

            assocSeconds += row.Seconds;
            activitySeconds += row.Seconds;
            clientSeconds += row.Seconds;
            grandSeconds += row.Seconds;

            assocAxxima += row.AxximaRates;
            activityAxxima += row.AxximaRates;
            clientAxxima += row.AxximaRates;
            grandAxxima += row.AxximaRates;

            displayRows.Add(new WipDisplayRow
            {
                RowType = WipDisplayRowType.Detail,
                ClientProject = $"{row.Project}:{row.Client}",
                Activity = row.Activity,
                Associate = row.LastName,
                DateText = row.Date.ToString("M/d/yyyy"),
                TimeText = CalcDecimalTime(row.Seconds),
                AmountText = row.AxximaRates.ToString("N2"),
                Description = row.Comment
            });
        }

        if (orderedRows.Count > 0)
        {
            displayRows.Add(new WipDisplayRow { RowType = WipDisplayRowType.Divider });
            displayRows.Add(CreateTotalRow(WipDisplayRowType.AssociateTotal, "Associate Total", assocSeconds, assocAxxima));
            displayRows.Add(CreateTotalRow(WipDisplayRowType.ActivityTotal, "Activity Total", activitySeconds, activityAxxima));
            displayRows.Add(CreateTotalRow(WipDisplayRowType.ClientTotal, "Client Total", clientSeconds, clientAxxima));
            displayRows.Add(CreateTotalRow(WipDisplayRowType.GrandTotal, "Grand Total", grandSeconds, grandAxxima));
        }

        return displayRows;
    }

    private static WipDisplayRow CreateTotalRow(
        WipDisplayRowType rowType,
        string label,
        double seconds,
        double amount)
    {
        return new WipDisplayRow
        {
            RowType = rowType,
            Label = label,
            TimeText = CalcDecimalTime(seconds),
            AmountText = amount.ToString("N2")
        };
    }

    private static string CalcDecimalTime(double seconds)
    {
        int h = (int)(seconds / 3600);
        int remainingSeconds = (int)(seconds - (h * 3600));
        double fractionalHours = (double)remainingSeconds / 3600;
        double value = h + fractionalHours;
        return value.ToString("0.000000");
    }
}