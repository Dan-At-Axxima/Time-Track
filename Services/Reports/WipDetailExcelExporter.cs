using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using ClosedXML.Excel;
using TimeTrackerRepo.Models.Reports;

namespace TimeTrackerRepo.Services.Reports
{
    public class WipDetailExcelExporter
    {
        public byte[] ExportSingleCompanyReport(string reportTitle, List<WipDetailReportData> rows)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("WIP Detail");

            GenerateHeader(ws, reportTitle);
            GenerateDetail(ws, rows);

            ws.PageSetup.PagesWide = 1;
            ws.PageSetup.PageOrientation = XLPageOrientation.Portrait;

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private void GenerateHeader(IXLWorksheet ws, string reportTitle)
        {
            // Match legacy widths as closely as possible
            ws.Column(1).Width = 50.71;
            ws.Column(2).Width = 34.14;
            ws.Column(3).Width = 10.86;
            ws.Column(4).Width = 13.71;
            ws.Column(5).Width = 8.14;
            ws.Column(6).Width = 10.71;
            ws.Column(7).Width = 94.57;

            ws.Cell(1, 1).Value = DateTime.Now.ToString("MM/dd/yyyy").Trim();
            ws.Cell(2, 1).Value = DateTime.Now.ToString("t").Trim();
            ws.Cell(1, 6).Value = reportTitle;

            ws.Cell(3, 1).Value = "Client";
            ws.Cell(3, 2).Value = "Activity";
            ws.Cell(3, 3).Value = "Associate";
            ws.Cell(3, 4).Value = "Date";
            ws.Cell(3, 5).Value = "Time";
            ws.Cell(4, 5).Value = "Spent";
            ws.Cell(3, 6).Value = "Axxima";
            ws.Cell(4, 6).Value = "Rates";
            ws.Cell(3, 7).Value = "Description";

            ws.Range("A1:A2").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Left;
            ws.Range("A1:A2").Style.Alignment.Indent = 3;
            ws.Cell(1, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            ws.Range("A3:G4").Style.Font.Bold = true;
            ws.Range("A3:G4").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;
            ws.Range("A3:G4").Style.Alignment.Vertical = XLAlignmentVerticalValues.Center;

            GenerateLine(ws, 4);
        }

        private void GenerateDetail(IXLWorksheet ws, List<WipDetailReportData> rows)
        {
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

            int row = 5;

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

            for (int i = 0; i < orderedRows.Count; i++)
            {
                var item = orderedRows[i];

                if (currentClient is null)
                {
                    currentClient = item.Client;
                    currentActivity = item.Activity;
                    currentEmployee = item.EmployeeNumber;
                }

                var clientChanged = currentClient != item.Client;
                var activityChanged = !clientChanged && currentActivity != item.Activity;
                var employeeChanged = !clientChanged && !activityChanged && currentEmployee != item.EmployeeNumber;

                if (clientChanged)
                {
                    row = WriteDivider(ws, row);
                    row = WriteTotalRow(ws, row, "Associate Total", assocSeconds, assocAxxima);
                    row = WriteTotalRow(ws, row, "Activity Total", activitySeconds, activityAxxima);
                    row = WriteTotalRow(ws, row, "Client Total", clientSeconds, clientAxxima);

                    ResetAssociateTotals();
                    ResetActivityTotals();
                    ResetClientTotals();

                    currentClient = item.Client;
                    currentActivity = item.Activity;
                    currentEmployee = item.EmployeeNumber;
                }
                else if (activityChanged)
                {
                    row = WriteDivider(ws, row);
                    row = WriteTotalRow(ws, row, "Associate Total", assocSeconds, assocAxxima);
                    row = WriteTotalRow(ws, row, "Activity Total", activitySeconds, activityAxxima);

                    ResetAssociateTotals();
                    ResetActivityTotals();

                    currentActivity = item.Activity;
                    currentEmployee = item.EmployeeNumber;
                }
                else if (employeeChanged)
                {
                    row = WriteDivider(ws, row);
                    row = WriteTotalRow(ws, row, "Associate Total", assocSeconds, assocAxxima);

                    ResetAssociateTotals();

                    currentEmployee = item.EmployeeNumber;
                }

                assocSeconds += item.Seconds;
                activitySeconds += item.Seconds;
                clientSeconds += item.Seconds;
                grandSeconds += item.Seconds;

                assocAxxima += item.AxximaRates;
                activityAxxima += item.AxximaRates;
                clientAxxima += item.AxximaRates;
                grandAxxima += item.AxximaRates;

                ws.Cell(row, 1).Value = $"{item.Project}:{item.Client}";
                ws.Cell(row, 2).Value = item.Activity;
                ws.Cell(row, 3).Value = item.LastName;
                ws.Cell(row, 4).Value = item.Date.ToString("M/d/yyyy");
                ws.Cell(row, 5).Value = CalcDecimalTime(item.Seconds);
                ws.Cell(row, 6).Value = item.AxximaRates;
                ws.Cell(row, 7).Value = item.Comment;

                ws.Cell(row, 5).Style.NumberFormat.Format = "#,##0.00000";
                ws.Cell(row, 6).Style.NumberFormat.Format = "#,##0.00";
                ws.Cell(row, 7).Style.Alignment.WrapText = true;

                row++;
            }

            if (orderedRows.Count > 0)
            {
                row = WriteDivider(ws, row);
                row = WriteTotalRow(ws, row, "Associate Total", assocSeconds, assocAxxima);
                row = WriteTotalRow(ws, row, "Activity Total", activitySeconds, activityAxxima);
                row = WriteTotalRow(ws, row, "Client Total", clientSeconds, clientAxxima);
                WriteTotalRow(ws, row, "Grand Total", grandSeconds, grandAxxima, true);
            }
        }

        private int GenerateLevel1Total(IXLWorksheet ws, int row, double seconds, double axximaRates)
        {
            GenerateLine(ws, row);
            row += 2;
            row = CreateLevelTotal(ws, row, seconds, axximaRates, 1);
            return row;
        }
        private static int WriteDivider(IXLWorksheet ws, int row)
        {
            ws.Range(row, 1, row, 7).Style.Border.TopBorder = XLBorderStyleValues.Thick;
            return row + 1;
        }

        private static int WriteTotalRow(
            IXLWorksheet ws,
            int row,
            string label,
            double seconds,
            double amount,
            bool grandTotal = false)
        {
            ws.Cell(row, 4).Value = label;
            ws.Cell(row, 5).Value = CalcDecimalTime(seconds);
            ws.Cell(row, 6).Value = amount;

            ws.Cell(row, 5).Style.NumberFormat.Format = "#,##0.00000";
            ws.Cell(row, 6).Style.NumberFormat.Format = "#,##0.00";

            var range = ws.Range(row, 1, row, 7);
            range.Style.Font.Bold = true;

            if (grandTotal)
            {
                range.Style.Fill.BackgroundColor = XLColor.LightGray;
            }
            else
            {
                range.Style.Fill.BackgroundColor = XLColor.FromHtml("#F8F9FA");
            }

            return row + 2;
        }
        private void AddDetailRow(IXLWorksheet ws, WipDetailReportData data, int row)
        {
            row += 1;

            ws.Cell(row, 1).Value = $"{data.Project}:{data.Client}";
            ws.Cell(row, 2).Value = data.Activity;
            ws.Cell(row, 3).Value = data.LastName;
            ws.Cell(row, 4).Value = data.Date.ToString("d");
            ws.Cell(row, 5).Value = CalcDecimalTime(data.Seconds);
            ws.Cell(row, 6).Value = data.AxximaRates;
            ws.Cell(row, 7).Value = data.Comment;

            ws.Cell(row, 5).Style.NumberFormat.Format = "#,##0.00000";
            ws.Cell(row, 6).Style.NumberFormat.Format = "#,##0.00";
            ws.Cell(row, 7).Style.Alignment.WrapText = true;
        }

        private void GenerateLine(IXLWorksheet ws, int row)
        {
            ws.Range(row, 1, row, 7).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
        }

        private int CreateLevelTotal(
            IXLWorksheet ws,
            int row,
            double seconds,
            double axximaRates,
            int type)
        {
            bool addPageBreak = false;

            if (type == 1)
            {
                ws.Cell(row, 4).Value = "Associate Total";
            }
            else if (type == 2)
            {
                ws.Cell(row, 4).Value = "Activity Total";
            }
            else if (type == 3)
            {
                ws.Cell(row, 4).Value = "Client Total";
                addPageBreak = true;
            }
            else if (type == 4)
            {
                ws.Cell(row, 4).Value = "Grand Total";
            }

            ws.Cell(row, 5).Value = CalcDecimalTime(seconds);
            ws.Cell(row, 6).Value = axximaRates;

            ws.Cell(row, 5).Style.NumberFormat.Format = "#,##0.00000";
            ws.Cell(row, 6).Style.NumberFormat.Format = "#,##0.00";

            row += 2;

            if (addPageBreak)
            {
                ws.Row(row).InsertRowsAbove(1);
                row += 1;
                ws.PageSetup.AddHorizontalPageBreak(row);
            }

            return row;
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
}