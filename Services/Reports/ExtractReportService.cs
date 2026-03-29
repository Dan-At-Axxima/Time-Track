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
from [dbo].[Transactions]
where Date Between @date1 and @date2
order by Date, [Employee Number], Client, Project, Activity";

            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();

                using var cmdIns = new SqlCommand(sqlQuery, conn);
                cmdIns.Parameters.Add(new SqlParameter("@date1", startDate));
                cmdIns.Parameters.Add(new SqlParameter("@date2", endDate));

                using var reader = cmdIns.ExecuteReader();
                while (reader.Read())
                {
                    var data = new WipDetailReportData
                    {
                        EmployeeNumber = reader.GetInt32(0),
                        Client = reader.GetString(1),
                        Project = reader.GetString(2),
                        Activity = reader.GetString(3),
                        Date = reader.GetDateTime(4),
                        Hours = reader.GetString(5),
                        Comment = reader.GetString(6)
                    };

                    wipDetail.Add(data);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }

            return wipDetail;
        }
    }
}