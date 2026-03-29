using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System;
using System.Collections.Generic;
using System.Data;
using TimeTrackerRepo.Models.Reports;

namespace TimeTrackerRepo.Services.Reports.Legacy
{
    public class SQLFunctions
    {
        private readonly string _connectionString;

        public SQLFunctions(IConfiguration configuration)
        {
            _connectionString = configuration.GetConnectionString("TimeTrackerContext")
                ?? throw new InvalidOperationException("Connection string 'TimeTrackerContext' was not found.");
        }

        internal List<WipDetailReportData> GetWipDetailData(int companyCode, DateTime startDate, DateTime endDate, bool oneClientOnly, string client)
        {
            var wipDetail = new List<WipDetailReportData>();
            string sqlQuery;

            if (!oneClientOnly)
            {
                sqlQuery = @"
SELECT *
FROM [dbo].[TransacdtionEmployeeRates]
WHERE AxximaCompanyCodes = @companyCode
  AND [Date] BETWEEN @date1 AND @date2
ORDER BY Client, Project, Activity, [Employee Number], [Date] ASC";
            }
            else
            {
                sqlQuery = @"
SELECT *
FROM [dbo].[TransacdtionEmployeeRates]
WHERE AxximaCompanyCodes = @companyCode
  AND [Date] BETWEEN @date1 AND @date2
  AND Client = @client
ORDER BY Client, Project, Activity, [Employee Number], [Date] ASC";
            }

            using var conn = new SqlConnection(_connectionString);
            conn.Open();

            using var cmd = new SqlCommand(sqlQuery, conn);
            cmd.Parameters.Add(new SqlParameter("@companyCode", companyCode));
            cmd.Parameters.Add(new SqlParameter("@date1", startDate));
            cmd.Parameters.Add(new SqlParameter("@date2", endDate));

            if (oneClientOnly)
            {
                cmd.Parameters.Add(new SqlParameter("@client", client));
            }

            using var reader = cmd.ExecuteReader();
            while (reader.Read())
            {
                var data = new WipDetailReportData
                {
                    DdaRates = reader.IsDBNull(0) ? 0 : reader.GetDouble(0),
                    AxximaRates = reader.IsDBNull(1) ? 0 : reader.GetDouble(1),
                    FirstName = reader.IsDBNull(2) ? string.Empty : reader.GetString(2),
                    LastName = reader.IsDBNull(3) ? string.Empty : reader.GetString(3),
                    EmployeeNumber = reader.IsDBNull(4) ? 0 : reader.GetInt32(4),
                    Client = reader.IsDBNull(5) ? string.Empty : reader.GetString(5),
                    Project = reader.IsDBNull(6) ? string.Empty : reader.GetString(6),
                    Activity = reader.IsDBNull(7) ? string.Empty : reader.GetString(7),
                    Date = reader.IsDBNull(8) ? DateTime.MinValue : reader.GetDateTime(8),
                    Hours = reader.IsDBNull(9) ? string.Empty : reader.GetString(9),
                    Comment = reader.IsDBNull(10) ? string.Empty : reader.GetString(10),
                    SlipId = reader.IsDBNull(11) ? string.Empty : reader.GetString(11),
                    Multiple = reader.IsDBNull(12) ? 1.0 : reader.GetDouble(12),
                    AxximaCompanyCodes = reader.IsDBNull(13) ? 0 : reader.GetInt32(13)
                };

                wipDetail.Add(data);
            }

            foreach (WipDetailReportData d in wipDetail)
            {
                if (string.IsNullOrEmpty(d.Hours))
                {
                    continue;
                }

                d.Seconds = Helpers.CalcSeconds(d.Hours, 0);
            }

            return wipDetail;
        }

        internal DataSet GetEmployeesWithNoHours(DateTime date)
        {
            const string sqlQuery = "Select [Employee Number] As EmployeeNumber From Employee where Active = 1 and Employee not in (Select Employee from Transactions where Date = @Date)";
            var ds = new DataSet();

            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();

                using var adapter = new SqlDataAdapter(sqlQuery, conn);
                adapter.SelectCommand!.Parameters.AddWithValue("@Date", date);
                adapter.Fill(ds, "ZeroHoursThisDay");

                return ds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null!;
            }
        }

        internal int GetHoursForThisDate(int employeeNumber, DateTime date)
        {
            const string sqlQuery = "Select Count(*) From Transactions where [Employee Number] = @EmployeeNumber and Date = @Date";

            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();

                using var cmd = new SqlCommand(sqlQuery, conn);
                cmd.Parameters.AddWithValue("@EmployeeNumber", employeeNumber);
                cmd.Parameters.AddWithValue("@Date", date);

                var count = (int)cmd.ExecuteScalar()!;
                return count;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return -1;
            }
        }

        internal DataSet GetAllActiveEmployees()
        {
            const string sqlQuery = "Select EmployeeNumber, FirstName, LastName, EMail, Active from NewEmployee where Active = 1 and [EmployeeNumber] !=1019 and [EmployeeNumber]<9000";
            var ds = new DataSet();

            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();

                using var adapter = new SqlDataAdapter(sqlQuery, conn);
                adapter.Fill(ds, "NewEmployee");

                return ds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null!;
            }
        }

        internal DataSet GetAllActiveEmployeesWithHoursPerDay()
        {
            const string sqlQuery = "Select [Employee Number] EmployeeNumber, [First Name] as FirstName, [Last Name] as LastName, EMail, Active, HoursPerDay from vEmployeeWithHoursperDay where Active = 1";
            var ds = new DataSet();

            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();

                using var adapter = new SqlDataAdapter(sqlQuery, conn);
                adapter.Fill(ds, "EmployeeWithHoursPerDay");

                return ds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null!;
            }
        }

        internal DataSet GetAllHours(DateTime startDate, DateTime endDate)
        {
            const string sqlQuery = "Select T1.[Employee Number], T2.[First Name], T2.[Last Name], Date, Hours From Transactions T1, Employee T2 where Date Between @StartDate and @EndDate and T1.[Employee Number]=T2.[Employee Number]";
            var ds = new DataSet();

            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();

                using var adapter = new SqlDataAdapter(sqlQuery, conn);
                adapter.SelectCommand!.Parameters.AddWithValue("@StartDate", startDate);
                adapter.SelectCommand.Parameters.AddWithValue("@EndDate", endDate);
                adapter.Fill(ds, "MissingHours");

                return ds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null!;
            }
        }

        internal bool RemoveActivity(string clientCode, string projectCode, string activityCode)
        {
            try
            {
                using var connection = new SqlConnection(_connectionString);
                connection.Open();

                using var transaction = connection.BeginTransaction();

                string deleteSql = @"
DELETE FROM Activity 
WHERE Client = @ClientCode
AND Project = @ProjectCode 
AND Activity = @ActivityCode

DELETE FROM Assignments
WHERE Client = @ClientCode 
AND Project = @ProjectCode 
AND Activity = @ActivityCode;";

                using var command = new SqlCommand(deleteSql, connection, transaction);
                command.Parameters.AddWithValue("@ClientCode", clientCode);
                command.Parameters.AddWithValue("@ProjectCode", projectCode);
                command.Parameters.AddWithValue("@ActivityCode", activityCode);

                int rowsAffected = command.ExecuteNonQuery();
                Console.WriteLine($"{rowsAffected} rows were deleted.");

                transaction.Commit();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return true;
        }

        internal bool RemoveClient(string client)
        {
            string clientCode = client;

            try
            {
                using var connection = new SqlConnection(_connectionString);
                connection.Open();

                string deleteSql = @"
DELETE FROM Activity WHERE Client = @ClientCode;
DELETE FROM Assignments WHERE Client = @ClientCode;";

                using var command = new SqlCommand(deleteSql, connection);
                command.Parameters.AddWithValue("@ClientCode", clientCode);

                int rowsAffected = command.ExecuteNonQuery();
                Console.WriteLine($"{rowsAffected} rows were deleted.");
            }
            catch (Exception ex)
            {
                Console.WriteLine($"An error occurred: {ex.Message}");
            }

            return true;
        }

        internal DataSet GetMissingComments(DateTime startDate, DateTime endDate)
        {
            var ds = new DataSet();

            const string sqlQuery =
                "SELECT Tr.[Employee Number], Emp.[First Name],Emp.[Last Name], Tr.[Client],Tr.[Project],Tr.[Activity],Tr.[Date],Tr.[Hours],Tr.[Comment]" +
                " FROM [dbo].[Transactions] Tr, [dbo].[Employee] Emp Where Date Between @StartDate and @EndDate and (Comment is null or DATALENGTH(Comment)= 0) and" +
                " Client not in (Select Client from Activity where Activity.AxximaCompanyCodes = 3) and Tr.[Employee Number]= Emp.[Employee Number] order by Tr.Date desc";

            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();

                using var adapter = new SqlDataAdapter(sqlQuery, conn);
                adapter.SelectCommand!.Parameters.AddWithValue("@StartDate", startDate);
                adapter.SelectCommand.Parameters.AddWithValue("@EndDate", endDate);
                adapter.Fill(ds, "MissingComments");

                return ds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null!;
            }
        }

        public static bool DoesActivityRecordExist(string connectionString, string client, string project, string activity)
        {
            string query = @"
SELECT CASE
    WHEN EXISTS (
        SELECT 1
        FROM Activity
        WHERE client = @Client
          AND project = @Project
          AND activity = @Activity
    )
THEN 1 ELSE 0 END";

            try
            {
                using var connection = new SqlConnection(connectionString);
                using var command = new SqlCommand(query, connection);

                command.Parameters.AddWithValue("@Client", client);
                command.Parameters.AddWithValue("@Project", project);
                command.Parameters.AddWithValue("@Activity", activity);

                connection.Open();
                int result = (int)command.ExecuteScalar()!;

                return result == 1 || result == 2;
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error: {ex.Message}");
                return false;
            }
        }

        internal bool InsertActivity(ActivityStructure act)
        {
            const string sqlQuery = "Insert into [dbo].[Activity] values (@Client, @Project, @Activity, @Multiple, @AxximaCompanyCodes)";

            try
            {
                bool exists = DoesActivityRecordExist(_connectionString, act.Client, act.Project, act.Activity);
                if (exists)
                {
                    return false;
                }

                using var conn = new SqlConnection(_connectionString);
                conn.Open();

                using var command = new SqlCommand(sqlQuery, conn);
                command.Parameters.AddWithValue("@Client", act.Client);
                command.Parameters.AddWithValue("@Project", act.Project);
                command.Parameters.AddWithValue("@Activity", act.Activity);
                command.Parameters.AddWithValue("@Multiple", act.Multiple);
                command.Parameters.AddWithValue("@AxximaCompanyCodes", act.AxximaCompanyCodes);

                command.ExecuteNonQuery();
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return false;
            }

            return true;
        }

        internal bool CheckIfExists(string client)
        {
            const string query = "SELECT (SELECT COUNT(1) FROM Activity WHERE Client = @Client) + (SELECT COUNT(1) FROM Transactions WHERE Client = @Client) AS TotalCount;";

            using var connection = new SqlConnection(_connectionString);
            using var command = new SqlCommand(query, connection);
            command.Parameters.AddWithValue("@Client", client);

            try
            {
                connection.Open();
                int recordCount = (int)command.ExecuteScalar()!;

                return recordCount > 0;
            }
            catch (Exception ex)
            {
                Console.WriteLine("An error occurred: " + ex.Message);
                return false;
            }
        }

        internal DataSet GetExistingActivities()
        {
            var ds = new DataSet();

            const string sqlQuery = "SELECT * From [dbo].[Activity]";

            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();

                using var adapter = new SqlDataAdapter(sqlQuery, conn);
                adapter.Fill(ds, "Activities");

                return ds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null!;
            }
        }

        internal DataSet GetClients()
        {
            var ds = new DataSet();

            const string sqlQuery = "SELECT Client as ClientCode,Project,AxximaCompanyCodes as CompanyCode from Activity Group By Client, Project, AxximaCompanyCodes order by Project";

            try
            {
                using var conn = new SqlConnection(_connectionString);
                conn.Open();

                using var adapter = new SqlDataAdapter(sqlQuery, conn);
                adapter.Fill(ds, "Clients");

                return ds;
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
                return null!;
            }
        }
    }
}