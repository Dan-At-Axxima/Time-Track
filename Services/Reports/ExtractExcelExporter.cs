using System;
using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;
using TimeTrackerRepo.Models.Reports;

namespace TimeTrackerRepo.Services.Reports
{
    public class ExtractExcelExporter
    {
        public byte[] CreateExtractExcel(List<WipDetailReportData> rows)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("Extract");

            GenerateExtractHeader(worksheet);
            GenerateExtractReport(worksheet, rows);

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }

        private void GenerateExtractHeader(IXLWorksheet worksheet)
        {
            worksheet.Column(1).Width = 20;
            worksheet.Column(2).Width = 20;
            worksheet.Column(3).Width = 50;
            worksheet.Column(4).Width = 50;
            worksheet.Column(5).Width = 25;
            worksheet.Column(6).Width = 20;
            worksheet.Column(7).Width = 20;
            worksheet.Column(8).Width = 50;

            worksheet.Cell(1, 1).Value = "Employee Number";
            worksheet.Cell(1, 2).Value = "Client";
            worksheet.Cell(1, 3).Value = "Project";
            worksheet.Cell(1, 4).Value = "Activity";
            worksheet.Cell(1, 5).Value = "Date";
            worksheet.Cell(1, 6).Value = "Hours";
            worksheet.Cell(1, 7).Value = "Decimal Hours";
            worksheet.Cell(1, 8).Value = "Comment";

            worksheet.Cell(1, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

            GenerateLine(worksheet);
        }

        private void GenerateLine(IXLWorksheet worksheet)
        {
            // Matches legacy behavior: only underline the Hours column header (F1)
            worksheet.Cell(1, 6).Style.Border.BottomBorder = XLBorderStyleValues.Thin;
        }

        private void GenerateExtractReport(IXLWorksheet worksheet, List<WipDetailReportData> wipDetailInfo)
        {
            int row = 2;

            foreach (WipDetailReportData data in wipDetailInfo)
            {
                AddExtractRow(worksheet, data, row);
                row += 1;
            }

            // Optional: add Excel table (nice usability improvement)
            if (wipDetailInfo.Count > 0)
            {
                var tableRange = worksheet.Range($"A1:H{wipDetailInfo.Count + 2}");
                var table = tableRange.CreateTable("Extract");
                table.ShowAutoFilter = true;
            }
        }

        private void AddExtractRow(IXLWorksheet worksheet, WipDetailReportData data, int row)
        {
            // Preserve legacy behavior: first data row starts at row 3
            row += 1;

            worksheet.Cell(row, 1).Value = data.EmployeeNumber;
            worksheet.Cell(row, 2).Value = data.Client;
            worksheet.Cell(row, 3).Value = data.Project;
            worksheet.Cell(row, 4).Value = data.Activity;
            worksheet.Cell(row, 5).Value = data.Date.ToString("d");
            worksheet.Cell(row, 6).Value = data.Hours;
            worksheet.Cell(row, 7).Value = data.DecimalHours;
            worksheet.Cell(row, 8).Value = data.Comment;

            // Formatting (optional but improves Excel usability)
            worksheet.Cell(row, 6).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
            worksheet.Cell(row, 7).Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
        }
    }
}