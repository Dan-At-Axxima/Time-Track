using System;
using System.Collections.Generic;
using System.IO;
using ClosedXML.Excel;
using TimeTrackerRepo.Models.Reports;

namespace TimeTrackerRepo.Services.Reports
{
    public class MissingHoursExcelExporter
    {
        public byte[] CreateMissingHoursExcel(List<TimeStructure> missingHoursList)
        {
            using var workbook = new XLWorkbook();
            var worksheet = workbook.Worksheets.Add("MissingHours");

            worksheet.Cell("A1").Value = "Employee";
            worksheet.Cell("B1").Value = "Name";
            worksheet.Cell("C1").Value = "Date";
            worksheet.Cell("D1").Value = "Hours";

            worksheet.Column(1).Width = 10;
            worksheet.Column(2).Width = 20;
            worksheet.Column(3).Width = 12;
            worksheet.Column(4).Width = 12;

            var headerRange = worksheet.Range("A1:D1");
            headerRange.Style.Font.Bold = true;
            headerRange.Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Center;

            int rows = missingHoursList.Count;

            for (int i = 0; i < missingHoursList.Count; i++)
            {
                int row = i + 2;
                var item = missingHoursList[i];

                worksheet.Cell(row, 1).Value = item.EmployeeNumber;
                worksheet.Cell(row, 2).Value = $"{item.FirstName} {item.LastName}";
                worksheet.Cell(row, 3).Value = item.Date;

                var hrs = item.Hours;
                if (!string.IsNullOrEmpty(hrs) && hrs.Length > 8)
                {
                    hrs = hrs.Substring(0, 8);
                }

                if (TimeSpan.TryParse(hrs, out var timeValue))
                {
                    worksheet.Cell(row, 4).Value = timeValue;
                }
                else
                {
                    worksheet.Cell(row, 4).Value = hrs;
                }
            }

            if (rows > 0)
            {
                worksheet.Range($"C2:C{rows + 1}").Style.DateFormat.Format = "yyyy-mm-dd";
                worksheet.Range($"D2:D{rows + 1}").Style.DateFormat.Format = "hh:mm";
                worksheet.Range($"D2:D{rows + 1}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;
                worksheet.Range($"B2:B{rows + 1}").Style.Alignment.Horizontal = XLAlignmentHorizontalValues.Right;

                var tableRange = worksheet.Range($"A1:D{rows + 1}");
                var table = tableRange.CreateTable("MissingHours");
                table.ShowAutoFilter = true;
            }

            using var stream = new MemoryStream();
            workbook.SaveAs(stream);
            return stream.ToArray();
        }
    }
}