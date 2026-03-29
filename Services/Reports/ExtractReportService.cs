using System;
using System.Collections.Generic;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TimeTrackerRepo.Models.Reports;

namespace TimeTrackerRepo.Services.Reports
{
    public class ExtractReportService
    {
        private readonly string _connectionString;

        public ExtractReportService(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("TimeTrackerContext")
                ?? throw new InvalidOperationException("Connection string 'TimeTrackerContext' was not found.");
        }

        public List<WipDetailReportData> GenerateExtract(DateTime startDate, DateTime endDate)
        {
            var wipDetail = new List<WipDetailReportData>();

            const string sqlQuery = @"
SELECT *
FROM [dbo].[Transactions]
WHERE [Date] BETWEEN @date1 AND @date2
ORDER BY [Date], [Employee Number], Client, Project, Activity;";

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand(sqlQuery, conn);
            cmd.Parameters.Add(new SqlParameter("@date1", startDate));
            cmd.Parameters.Add(new SqlParameter("@date2", endDate));

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var data = new WipDetailReportData
                {
                    EmployeeNumber = reader.IsDBNull(0) ? 0 : reader.GetInt32(0),
                    Client = reader.IsDBNull(1) ? string.Empty : reader.GetString(1),
                    Project = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    Activity = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    Date = reader.IsDBNull(4) ? DateTime.MinValue : reader.GetDateTime(4),
                    Hours = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                    Comment = reader.IsDBNull(6) ? string.Empty : reader.GetString(6)
                };

                wipDetail.Add(data);
            }

            return wipDetail;
        }
    }
}