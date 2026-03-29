using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TimeTrackerRepo.Models.Reports;

namespace TimeTrackerRepo.Services.Reports;

public class WipDetailReportService
{
    private readonly string _connectionString;

    public WipDetailReportService(IConfiguration configuration)
    {
        _connectionString = configuration.GetConnectionString("TimeTrackerContext")
            ?? throw new InvalidOperationException("Connection string 'TimeTrackerContext' was not found.");
    }

    public async Task<List<WipDetailReportRow>> GetWipDetailDataAsync(
        int companyCode,
        DateTime startDate,
        DateTime endDate,
        bool oneClientOnly,
        string? clientCode)
    {
        var rows = new List<WipDetailReportRow>();

        const string sqlQuery = @"
SELECT *
FROM [dbo].[TransacdtionEmployeeRates]
WHERE AxximaCompanyCodes = @companyCode
  AND [Date] BETWEEN @date1 AND @date2
ORDER BY Client, Project, Activity, [Employee Number], [Date] ASC";

        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        await using var cmd = new SqlCommand(sqlQuery, conn);
        cmd.Parameters.Add(new SqlParameter("@companyCode", companyCode));
        cmd.Parameters.Add(new SqlParameter("@date1", startDate));
        cmd.Parameters.Add(new SqlParameter("@date2", endDate));

        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var hours = reader.IsDBNull(9) ? string.Empty : reader.GetString(9);

            rows.Add(new WipDetailReportRow
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
                Hours = hours,
                DecimalHours = hours,
                Time = ParseHoursToTimeSpan(hours),
                Seconds = CalcSeconds(hours, 0),
                Comment = reader.IsDBNull(10) ? string.Empty : reader.GetString(10),
                SlipId = reader.IsDBNull(11) ? string.Empty : reader.GetString(11),
                Multiple = reader.IsDBNull(12) ? 1.0 : reader.GetDouble(12),
                AxximaCompanyCodes = reader.IsDBNull(13) ? 0 : reader.GetInt32(13)
            });
        }

        if (oneClientOnly && !string.IsNullOrWhiteSpace(clientCode))
        {
            rows = rows
                .Where(x => string.Equals(x.Client, clientCode, StringComparison.OrdinalIgnoreCase))
                .ToList();
        }

        return rows;
    }

    private TimeSpan ParseHoursToTimeSpan(string hours)
    {
        if (string.IsNullOrWhiteSpace(hours))
        {
            return TimeSpan.Zero;
        }

        hours = hours.Trim();

        if (hours.Contains(':'))
        {
            if (hours.StartsWith(":"))
            {
                hours = "0" + hours;
            }

            return TimeSpan.TryParse(hours, out var ts)
                ? ts
                : TimeSpan.Zero;
        }

        var normalTime = ConvertDecimalTimeToNormal(hours);

        return TimeSpan.TryParse(normalTime, out var converted)
            ? converted
            : TimeSpan.Zero;
    }

    public async Task<List<WipDetailReportRow>> BuildReportDataAsync(
        int companyCode,
        DateTime startDate,
        DateTime endDate,
        bool withAllocation,
        bool ifrsOnly,
        bool oneClientOnly,
        string? clientCode)
    {
        var rows = await GetWipDetailDataAsync(
            companyCode,
            startDate,
            endDate,
            oneClientOnly,
            clientCode);

        if (ifrsOnly)
        {
            rows = rows
                .Where(x => x.Client == "33707")
                .ToList();
        }

        if (rows.Count == 0)
        {
            return rows;
        }

        if (!oneClientOnly)
        {
            if (withAllocation)
            {
                rows = await DoAllocationsAsync(rows, companyCode, startDate);
            }
        }
        else
        {
            rows.RemoveAll(x => x.AxximaCompanyCodes != companyCode);
        }

        return rows
            .OrderBy(x => x.Client)
            .ThenBy(x => x.Project)
            .ThenBy(x => x.Activity)
            .ThenBy(x => x.EmployeeNumber)
            .ThenBy(x => x.Date)
            .ToList();
    }

    internal async Task<List<WipDetailReportRow>> DoAllocationsAsync(
        IEnumerable<WipDetailReportRow> data,
        int companyCode,
        DateTime startDate)
    {
        var sourceRows = data?.ToList() ?? new List<WipDetailReportRow>();

        if (sourceRows.Count == 0)
        {
            return new List<WipDetailReportRow>();
        }

        var ifrsData = sourceRows
            .Where(x => x.Client == "33707")
            .ToList();

        var nonIfrsData = sourceRows
            .Where(x => x.Client != "33707")
            .ToList();

        var allocations = await GetAllocationsAsync(companyCode, startDate);

        var allocatedRows = CreateAllocationsFromWip(allocations, ifrsData);

        return nonIfrsData
            .Concat(allocatedRows)
            .Where(x => x.AxximaCompanyCodes == companyCode)
            .OrderBy(x => x.Client)
            .ThenBy(x => x.Project)
            .ThenBy(x => x.Activity)
            .ThenBy(x => x.EmployeeNumber)
            .ThenBy(x => x.Date)
            .ToList();
    }

    internal async Task<List<IFRSAllocation>> GetAllocationsAsync(
        int companyCode,
        DateTime startDate)
    {
        var allocations = new List<IFRSAllocation>();

        const string sqlQuery = @"
SELECT
    Client,
    Project,
    Activity,
    Percentage,
    CompanyCode,
    [Date]
FROM IFRSAllocations
WHERE CompanyCode = @CompanyCode
  AND YEAR([Date]) = @Year
  AND MONTH([Date]) = @Month;";

        await using var conn = new SqlConnection(_connectionString);
        await conn.OpenAsync();

        await using var cmd = new SqlCommand(sqlQuery, conn);
        cmd.Parameters.AddWithValue("@CompanyCode", companyCode);
        cmd.Parameters.AddWithValue("@Year", startDate.Year);
        cmd.Parameters.AddWithValue("@Month", startDate.Month);

        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            allocations.Add(new IFRSAllocation
            {
                Client = reader["Client"]?.ToString() ?? string.Empty,
                Project = reader["Project"]?.ToString() ?? string.Empty,
                Activity = reader["Activity"]?.ToString() ?? string.Empty,
                Percentage = reader["Percentage"] == DBNull.Value ? 0 : Convert.ToDouble(reader["Percentage"]),
                CompanyCode = reader["CompanyCode"] == DBNull.Value ? 0 : Convert.ToInt32(reader["CompanyCode"]),
                Date = reader["Date"] == DBNull.Value ? DateTime.MinValue : Convert.ToDateTime(reader["Date"])
            });
        }

        return allocations;
    }

    internal List<WipDetailReportRow> CreateAllocationsFromWip(
        List<IFRSAllocation> allocations,
        List<WipDetailReportRow> transactions)
    {
        var newTransactions = new List<WipDetailReportRow>();

        if (transactions == null || transactions.Count == 0)
        {
            return newTransactions;
        }

        if (allocations == null || allocations.Count == 0)
        {
            return newTransactions;
        }

        foreach (var trans in transactions)
        {
            var normalizedHours = (trans.Hours ?? string.Empty).Trim();

            if (normalizedHours.Contains('.'))
            {
                normalizedHours = ConvertDecimalTimeToNormal(normalizedHours);
            }

            double totalAllocatedMinutes = 0;

            for (int i = 0; i < allocations.Count; i++)
            {
                var allocation = allocations[i];

                var transaction = new WipDetailReportRow
                {
                    DdaRates = trans.DdaRates,
                    EmployeeNumber = trans.EmployeeNumber,
                    LastName = trans.LastName,
                    FirstName = trans.FirstName,
                    AxximaCompanyCodes = allocation.CompanyCode,
                    AxximaRates = trans.AxximaRates,
                    Multiple = trans.Multiple,
                    Project = allocation.Project,
                    Date = trans.Date,
                    Comment = trans.Comment,
                    Client = allocation.Client,
                    Activity = allocation.Activity
                };

                if (i == allocations.Count - 1)
                {
                    var totalMinutesForEntry = ConvertHoursToMinutes(normalizedHours);
                    var lastRecordMinutes = totalMinutesForEntry - totalAllocatedMinutes;

                    transaction.Hours = ConvertMinutesToHours(lastRecordMinutes);
                    transaction.Seconds = CalcSeconds(normalizedHours, allocation.Percentage);
                    transaction.Time = TimeSpan.FromMinutes(lastRecordMinutes);

                    totalAllocatedMinutes += lastRecordMinutes;
                }
                else
                {
                    transaction.Hours = CalcHours(normalizedHours, allocation.Percentage);
                    transaction.Seconds = CalcSeconds(normalizedHours, allocation.Percentage);

                    var allocatedMinutes = ConvertHoursToMinutes(transaction.Hours);
                    transaction.Time = TimeSpan.FromMinutes(allocatedMinutes);

                    totalAllocatedMinutes += allocatedMinutes;
                }

                newTransactions.Add(transaction);
            }
        }

        return newTransactions;
    }

    private string ConvertDecimalTimeToNormal(string value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return "0:00";
        }

        if (!double.TryParse(value, out var decimalHours))
        {
            return value;
        }

        var totalMinutes = (int)Math.Round(decimalHours * 60, MidpointRounding.AwayFromZero);
        return ConvertMinutesToHours(totalMinutes);
    }

    private string CalcHours(string originalHours, double percentage)
    {
        var totalMinutes = ConvertHoursToMinutes(originalHours);
        var allocatedMinutes = totalMinutes * percentage;
        return ConvertMinutesToHours(allocatedMinutes);
    }

    private double CalcSeconds(string originalHours, double percentage)
    {
        var totalMinutes = ConvertHoursToMinutes(originalHours);
        var totalSeconds = totalMinutes * 60.0;
        return totalSeconds * percentage;
    }

    private int ConvertHoursToMinutes(string hours)
    {
        if (string.IsNullOrWhiteSpace(hours))
        {
            return 0;
        }

        hours = hours.Trim();

        if (TimeSpan.TryParse(hours, out var timeSpan))
        {
            return (int)Math.Round(timeSpan.TotalMinutes, MidpointRounding.AwayFromZero);
        }

        if (double.TryParse(hours, out var decimalHours))
        {
            return (int)Math.Round(decimalHours * 60, MidpointRounding.AwayFromZero);
        }

        return 0;
    }

    private string ConvertMinutesToHours(double minutes)
    {
        var roundedMinutes = (int)Math.Round(minutes, MidpointRounding.AwayFromZero);

        if (roundedMinutes < 0)
        {
            roundedMinutes = 0;
        }

        var hoursPart = roundedMinutes / 60;
        var minutesPart = roundedMinutes % 60;

        return $"{hoursPart}:{minutesPart:00}";
    }

    internal double CalculateRate(
        double rate,
        double multiple,
        TimeSpan time,
        double seconds)
    {
        var hours = seconds > 0 ? seconds / 3600.0 : time.TotalHours;
        return rate * multiple * hours;
    }
}