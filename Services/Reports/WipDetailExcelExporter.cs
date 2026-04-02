using System;
using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;
using TimeTrackerRepo.Models.Reports;

namespace TimeTrackerRepo.Services.Reports
{
    public class WipDetailExcelExporter
    {
        public byte[] ExportSingleCompanyReport(string reportTitle, List<WipDisplayRow> rows)
        {
            using var workbook = new XLWorkbook();
            var ws = workbook.Worksheets.Add("WIP Detail");
            ws.Column(1).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            ws.Column(2).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            ws.Column(2).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            ws.Column(4).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            ws.Column(4).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            ws.Column(5).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            ws.Column(6).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;
            GenerateHeader(ws, reportTitle);
            GenerateBody(ws, rows ?? new List<WipDisplayRow>());
            ws.PageSetup.PagesWide = 1;
            ws.PageSetup.PageOrientation = XLPageOrientation.Portrait;

         //   ws.Rows().AdjustToContents();
            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private void GenerateHeader(IXLWorksheet ws, string reportTitle)
        {
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

            ws.Range("A4:G4").Style.Border.BottomBorder = XLBorderStyleValues.Thin;
        }

        private void GenerateBody(IXLWorksheet ws, List<WipDisplayRow> rows)
        {
            var rowNumber = 5;

            foreach (var row in rows)
            {
                switch (row.RowType)
                {
                    case WipDisplayRowType.Divider:
                        // Render exactly one divider row, then move to the next row.
                        ws.Range(rowNumber, 1, rowNumber, 7).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
                        rowNumber++;
                        break;

                    case WipDisplayRowType.Detail:
                        ws.Cell(rowNumber, 1).Value = row.ClientProject ?? string.Empty;
                        ws.Cell(rowNumber, 2).Value = row.Activity ?? string.Empty;
                        ws.Cell(rowNumber, 3).Value = row.Associate ?? string.Empty;
                        if (DateTime.TryParse(row.DateText, out var dt))
                        {
                            row.DateText = dt.ToString("yyyy-MM-dd");
                        }
                        ws.Cell(rowNumber, 4).Value = row.DateText ?? string.Empty;

                        var txt = row.TimeText;
                        var dot = txt?.IndexOf('.') ?? -1;

                        if (dot >= 0 && txt.Length > dot + 3)
                        {
                            txt = txt.Substring(0, dot + 3);
                        }

                        if (decimal.TryParse(row.TimeText, out var totalTime))
                        {
                            ws.Cell(rowNumber, 5).Value = totalTime;
                            ws.Cell(rowNumber, 5).Style.NumberFormat.Format = "#,##0.00";
                        }
                        else
                        {
                            ws.Cell(rowNumber, 5).Value = row.TimeText ?? string.Empty;
                        }
                        if (decimal.TryParse(row.AmountText, out var totalAmount))
                        {
                            ws.Cell(rowNumber, 6).Value = totalAmount;
                            ws.Cell(rowNumber, 6).Style.NumberFormat.Format = "#,##0.00";
                        }
                        else
                        {
                            ws.Cell(rowNumber, 6).Value = row.AmountText ?? string.Empty;
                        }
                        var comment = (row.Description ?? string.Empty)
    .Replace("\r\n", " ")
    .Replace("\n", " ")
    .Replace("\r", " ")
    .Trim();

                        while (comment.Contains("  "))
                        {
                            comment = comment.Replace("  ", " ");
                        }

                        var text = comment ?? string.Empty;

                        ws.Cell(rowNumber, 7).Value = text;
                        ws.Cell(rowNumber, 7).Style.Alignment.Vertical = XLAlignmentVerticalValues.Top;

                        if (text.Length > 100)
                        {
                            ws.Cell(rowNumber, 7).Style.Alignment.WrapText = true;

                            int lineCount = (int)Math.Ceiling(text.Length / 100.0);
                            ws.Row(rowNumber).Height = Math.Max(15, lineCount * 15);
                        }
                        else
                        {
                            ws.Cell(rowNumber, 7).Style.Alignment.WrapText = false;
                            ws.Row(rowNumber).Height = 15;
                        }
                        //   ws.Row(rowNumber).AdjustToContents();
                        //     ws.Row(rowNumber).AdjustToContents();


                        rowNumber++;
                        break;

                    case WipDisplayRowType.AssociateTotal:
                    case WipDisplayRowType.ActivityTotal:
                    case WipDisplayRowType.ClientTotal:
                    case WipDisplayRowType.GrandTotal:
                        ws.Cell(rowNumber, 4).Value = row.Label ?? string.Empty;

                        if (decimal.TryParse(row.TimeText, out var totalTime1))
                        {
                            ws.Cell(rowNumber, 5).Value = totalTime1;
                            ws.Cell(rowNumber, 5).Style.NumberFormat.Format = "#,##0.00";
                        }
                        else
                        {
                            ws.Cell(rowNumber, 5).Value = row.TimeText ?? string.Empty;
                        }

                        if (decimal.TryParse(row.AmountText, out var amount))
                        {
                            ws.Cell(rowNumber, 6).Value = amount;
                            ws.Cell(rowNumber, 6).Style.NumberFormat.Format = "#,##0.00";
                        }
                        else
                        {
                            ws.Cell(rowNumber, 6).Value = row.AmountText ?? string.Empty;
                        }

                        rowNumber++;          // existing line (move to next row)

                        // 👇 ADD THIS
                        ws.Row(rowNumber).Height = 8;  // optional but nice
                        rowNumber++;          // blank spacer row

                        break;
                }
            }

            ws.Column(7).Style.Alignment.WrapText = true;
        }
    }
}