using ClosedXML.Excel;
using TimeTrackerRepo.Models.Reports;

namespace TimeTrackerRepo.Services.Reports;

public class WipDetailExcelExporter
{
    public byte[] ExportSingleCompanyReport(string reportTitle, List<WipDetailReportRow> rows)
    {
        using var workbook = new XLWorkbook();

        var worksheetName = GetSafeWorksheetName(reportTitle);
        var worksheet = workbook.Worksheets.Add(worksheetName);

        GenerateHeader(worksheet, reportTitle);
        var lastRow = GenerateDetail(worksheet, rows);
        ConfigurePageSetup(worksheet, lastRow);

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    public byte[] ExportMultiCompanyReport(
        List<WipDetailReportRow> company2Rows,
        List<WipDetailReportRow>? company1Rows = null)
    {
        using var workbook = new XLWorkbook();

        var axximaSheet = workbook.Worksheets.Add("Axxima WIP");
        GenerateHeader(axximaSheet, "Axxima WIP DETAIL REPORT");
        var axximaLastRow = GenerateDetail(axximaSheet, company2Rows);
        ConfigurePageSetup(axximaSheet, axximaLastRow);

        if (company1Rows != null && company1Rows.Count > 0)
        {
            var company330Sheet = workbook.Worksheets.Add("330 WIP");
            GenerateHeader(company330Sheet, "330 WIP DETAIL REPORT");
            var company330LastRow = GenerateDetail(company330Sheet, company1Rows);
            ConfigurePageSetup(company330Sheet, company330LastRow);
        }

        using var stream = new MemoryStream();
        workbook.SaveAs(stream);
        return stream.ToArray();
    }

    private void ConfigurePageSetup(IXLWorksheet worksheet, int lastRow)
    {
        worksheet.PageSetup.PagesWide = 1;
        worksheet.PageSetup.PagesTall = 0;
        worksheet.PageSetup.SetRowsToRepeatAtTop(1, 4);

        worksheet.Range(1, 1, Math.Max(lastRow, 4), 9).Style.Alignment.Vertical =
            XLAlignmentVerticalValues.Top;

        worksheet.Columns().AdjustToContents();
        worksheet.Column(8).Width = Math.Max(worksheet.Column(8).Width, 50);
        worksheet.Column(9).Width = Math.Max(worksheet.Column(9).Width, 15);
    }

    private void GenerateHeader(IXLWorksheet worksheet, string reportTitle)
    {
        worksheet.Column(1).Width = 25;
        worksheet.Column(2).Width = 25;
        worksheet.Column(3).Width = 25;
        worksheet.Column(4).Width = 20;
        worksheet.Column(5).Width = 20;
        worksheet.Column(6).Width = 20;
        worksheet.Column(7).Width = 20;
        worksheet.Column(8).Width = 50;
        worksheet.Column(9).Width = 15;

        worksheet.Range("A1:A2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
        worksheet.Range("A1:A2").Style.Alignment.Indent = 3;
        worksheet.Cell("E1").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

        worksheet.Cell(1, 1).Value = DateTime.Now;
        worksheet.Cell(1, 1).Style.DateFormat.Format = "MM/dd/yyyy";

        worksheet.Cell(1, 6).Value = reportTitle;
        worksheet.Cell(1, 6).Style.Font.Bold = true;

        worksheet.Cell(2, 1).Value = DateTime.Now;
        worksheet.Cell(2, 1).Style.DateFormat.Format = "h:mm AM/PM";

        worksheet.Cell(3, 1).Value = "Client";
        worksheet.Cell(3, 2).Value = "Activity";
        worksheet.Cell(3, 3).Value = "Associate";
        worksheet.Cell(3, 4).Value = "Date";
        worksheet.Cell(3, 5).Value = "Time";
        worksheet.Cell(4, 5).Value = "Spent";
        worksheet.Cell(3, 6).Value = "DDA";
        worksheet.Cell(4, 6).Value = "Rates";
        worksheet.Cell(3, 7).Value = "Axxima";
        worksheet.Cell(4, 7).Value = "Rates";
        worksheet.Cell(3, 8).Value = "Description";
        worksheet.Cell(3, 9).Value = "Slip ID";

        worksheet.Range("A3:I4").Style.Font.Bold = true;
        worksheet.Range("A3:I4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
        worksheet.Range("A3:I4").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

        GenerateLine(worksheet, 4);
    }

    public int GenerateDetail(IXLWorksheet worksheet, List<WipDetailReportRow> wipDetailInfo)
    {
        if (worksheet == null)
        {
            throw new ArgumentNullException(nameof(worksheet));
        }

        if (wipDetailInfo == null || wipDetailInfo.Count == 0)
        {
            return 4;
        }

        wipDetailInfo = wipDetailInfo
            .OrderBy(x => x.Client)
            .ThenBy(x => x.Project)
            .ThenBy(x => x.Activity)
            .ThenBy(x => x.EmployeeNumber)
            .ThenBy(x => x.Date)
            .ToList();

        string client = wipDetailInfo[0].Client;
        string activity = wipDetailInfo[0].Activity;
        int empNo = wipDetailInfo[0].EmployeeNumber;

        TimeSpan t1 = TimeSpan.Zero;
        TimeSpan t2 = TimeSpan.Zero;
        TimeSpan t3 = TimeSpan.Zero;
        TimeSpan t4 = TimeSpan.Zero;

        double totalDda1 = 0;
        double totalDda2 = 0;
        double totalDda3 = 0;
        double totalDda4 = 0;

        double totalAxxima1 = 0;
        double totalAxxima2 = 0;
        double totalAxxima3 = 0;
        double totalAxxima4 = 0;

        int row = 4;

        foreach (var data in wipDetailInfo)
        {
            var ddaAmount = CalculateRate(data.DdaRates, data.Multiple, data.Time, data.Seconds);
            var axximaAmount = CalculateRate(data.AxximaRates, data.Multiple, data.Time, data.Seconds);

            if (client != data.Client)
            {
                row = GenerateLevel1Total(row, t1, worksheet, totalDda1, totalAxxima1);
                row = CreateLevelTotal(row, t2, worksheet, totalDda2, totalAxxima2, 2);
                row = CreateLevelTotal(row, t3, worksheet, totalDda3, totalAxxima3, 3);

                client = data.Client;
                activity = data.Activity;
                empNo = data.EmployeeNumber;

                t1 = TimeSpan.Zero;
                t2 = TimeSpan.Zero;
                t3 = TimeSpan.Zero;

                totalDda1 = 0;
                totalDda2 = 0;
                totalDda3 = 0;

                totalAxxima1 = 0;
                totalAxxima2 = 0;
                totalAxxima3 = 0;
            }
            else if (activity != data.Activity)
            {
                row = GenerateLevel1Total(row, t1, worksheet, totalDda1, totalAxxima1);
                row = CreateLevelTotal(row, t2, worksheet, totalDda2, totalAxxima2, 2);

                activity = data.Activity;
                empNo = data.EmployeeNumber;

                t1 = TimeSpan.Zero;
                t2 = TimeSpan.Zero;

                totalDda1 = 0;
                totalDda2 = 0;

                totalAxxima1 = 0;
                totalAxxima2 = 0;
            }
            else if (empNo != data.EmployeeNumber)
            {
                row = GenerateLevel1Total(row, t1, worksheet, totalDda1, totalAxxima1);

                empNo = data.EmployeeNumber;
                t1 = TimeSpan.Zero;

                totalDda1 = 0;
                totalAxxima1 = 0;
            }

            if (client == data.Client && activity == data.Activity && empNo == data.EmployeeNumber)
            {
                t1 += data.Time;
                t2 += data.Time;
                t3 += data.Time;
                t4 += data.Time;
            }

            totalDda1 += ddaAmount;
            totalDda2 += ddaAmount;
            totalDda3 += ddaAmount;
            totalDda4 += ddaAmount;

            totalAxxima1 += axximaAmount;
            totalAxxima2 += axximaAmount;
            totalAxxima3 += axximaAmount;
            totalAxxima4 += axximaAmount;

            row = AddDetailRow(worksheet, data, row, ddaAmount, axximaAmount);
        }

        row = GenerateLevel1Total(row, t1, worksheet, totalDda1, totalAxxima1);
        row = CreateLevelTotal(row, t2, worksheet, totalDda2, totalAxxima2, 2);
        row = CreateLevelTotal(row, t3, worksheet, totalDda3, totalAxxima3, 3);
        row = CreateLevelTotal(row, t4, worksheet, totalDda4, totalAxxima4, 4);

        worksheet.Column(8).Style.Alignment.WrapText = true;

        return row;
    }

    private int AddDetailRow(
        IXLWorksheet worksheet,
        WipDetailReportRow data,
        int row,
        double ddaAmount,
        double axximaAmount)
    {
        row += 1;

        worksheet.Range(row, 5, row, 7).Style.NumberFormat.Format = "#,##0.00";

        worksheet.Cell(row, 1).Value = $"{data.Project}:{data.Client}";
        worksheet.Cell(row, 2).Value = data.Activity;
        worksheet.Cell(row, 3).Value = data.LastName;
        worksheet.Cell(row, 4).Value = data.Date;
        worksheet.Cell(row, 4).Style.DateFormat.Format = "M/d/yyyy";

        worksheet.Cell(row, 5).Value = string.IsNullOrWhiteSpace(data.DecimalHours)
            ? data.Hours
            : data.DecimalHours;

        worksheet.Cell(row, 6).Value = ddaAmount;
        worksheet.Cell(row, 7).Value = axximaAmount;
        worksheet.Cell(row, 8).Value = data.Comment;
        worksheet.Cell(row, 9).Value = data.SlipId;

        return row;
    }

    private int GenerateLevel1Total(
        int row,
        TimeSpan totalTime,
        IXLWorksheet worksheet,
        double ddaRates,
        double axximaRates)
    {
        GenerateLine(worksheet, row);

        row += 2;
        row = CreateLevelTotal(row, totalTime, worksheet, ddaRates, axximaRates, 1);

        return row;
    }

    private void GenerateLine(IXLWorksheet worksheet, int row)
    {
        worksheet.Range(row, 1, row, 9).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
    }

    private int CreateLevelTotal(
        int row,
        TimeSpan totalTime,
        IXLWorksheet worksheet,
        double ddaRates,
        double axximaRates,
        int type)
    {
        worksheet.Cell(row, 4).Value = type switch
        {
            1 => "Associate Total",
            2 => "Activity Total",
            3 => "Client Total",
            4 => "Grand Total",
            _ => string.Empty
        };

        worksheet.Range(row, 5, row, 7).Style.NumberFormat.Format = "#,##0.00";
        worksheet.Cell(row, 5).Value = totalTime.TotalHours;
        worksheet.Cell(row, 6).Value = ddaRates;
        worksheet.Cell(row, 7).Value = axximaRates;

        row += 2;
        return row;
    }

    private double CalculateRate(double rate, double multiple, TimeSpan time, double seconds)
    {
        var hours = seconds > 0 ? seconds / 3600.0 : time.TotalHours;
        return rate * multiple * hours;
    }

    private static string GetSafeWorksheetName(string name)
    {
        var invalidChars = new[] { '\\', '/', '*', '?', ':', '[', ']' };
        var safe = new string(name.Where(c => !invalidChars.Contains(c)).ToArray());

        if (string.IsNullOrWhiteSpace(safe))
        {
            safe = "Sheet1";
        }

        return safe.Length <= 31 ? safe : safe[..31];
    }
}