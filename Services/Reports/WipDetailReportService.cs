using Microsoft.Data.SqlClient;
using TimeTrackerRepo.Models.Reports;

namespace TimeTrackerRepo.Services.Reports;

public class WipDetailReportService
{
    private readonly IConfiguration _configuration;

    public WipDetailReportService(IConfiguration configuration)
    {
        _configuration = configuration;
    }

    public async Task<List<WipDetailReportData>> GetDetailRowsAsync(
        int companyCode,
        DateTime startDate,
        DateTime endDate,
        bool withAllocation,
        bool ifrsOnly,
        bool oneClientOnly = false,
        string? clientCode = null)
    {
        var data = await GetWipDetailDataAsync(startDate, endDate, oneClientOnly, clientCode);

        if (ifrsOnly)
        {
            data = data.Where(x => x.Client == "33707").ToList();
        }

        if (data.Count == 0)
        {
            return data;
        }

        if (!oneClientOnly)
        {
            if (withAllocation)
            {
                var allocations = await GetAllocationsAsync(companyCode);

                var ifrsData = data.Where(x => x.Client == "33707").ToList();
                var allocatedRows = CreateAllocationsFromWip(allocations, ifrsData);

                data.RemoveAll(x => x.Client == "33707");
                data = data.Concat(allocatedRows).ToList();

                data.RemoveAll(x => x.AxximaCompanyCodes != companyCode);

                data = data
                    .OrderBy(x => x.Client)
                    .ThenBy(x => x.Project)
                    .ThenBy(x => x.Activity)
                    .ThenBy(x => x.EmployeeNumber)
                    .ThenBy(x => x.Date)
                    .ToList();
            }
        }
        else
        {
            data.RemoveAll(x => x.AxximaCompanyCodes != companyCode);
        }

        foreach (var item in data)
        {
            item.AxximaRates = CalculateRate(item.AxximaRates, item.Multiple, item.Seconds);
            item.DdaRates = CalculateRate(item.DdaRates, item.Multiple, item.Seconds);
        }

        return data;
    }

    private async Task<List<WipDetailReportData>> GetWipDetailDataAsync(
        DateTime startDate,
        DateTime endDate,
        bool oneClientOnly,
        string? client)
    {
        var result = new List<WipDetailReportData>();

        var sql = !oneClientOnly
            ? """
              SELECT *
              FROM [dbo].[TransacdtionEmployeeRates]
              WHERE [Date] BETWEEN @date1 AND @date2
              ORDER BY Client, Project, Activity, [Employee Number], [Date] ASC
              """
            : """
              SELECT *
              FROM [dbo].[TransacdtionEmployeeRates]
              WHERE [Date] BETWEEN @date1 AND @date2
                AND Client = @client
              ORDER BY Client, Project, Activity, [Employee Number], [Date] ASC
              """;

        var connectionString = _configuration.GetConnectionString("TimeTrackerContext");
        await using var conn = new SqlConnection(connectionString);
        await conn.OpenAsync();

        await using var cmd = new SqlCommand(sql, conn);
        cmd.Parameters.AddWithValue("@date1", startDate);
        cmd.Parameters.AddWithValue("@date2", endDate);

        if (oneClientOnly)
        {
            cmd.Parameters.AddWithValue("@client", client ?? "");
        }

        await using var reader = await cmd.ExecuteReaderAsync();

        while (await reader.ReadAsync())
        {
            var data = new WipDetailReportData
            {
                DdaRates = reader.GetDouble(0),
                AxximaRates = reader.GetDouble(1),
                FirstName = reader.GetString(2),
                LastName = reader.GetString(3),
                EmployeeNumber = reader.GetInt32(4),
                Client = reader.GetString(5),
                Project = reader.GetString(6),
                Activity = reader.GetString(7),
                Date = reader.GetDateTime(8),
                Hours = reader.IsDBNull(9) ? "" : reader.GetString(9),
                Comment = reader.IsDBNull(10) ? "" : reader.GetString(10),
                SlipId = reader.IsDBNull(11) ? "" : reader.GetString(11),
                Multiple = reader.GetDouble(12),
                AxximaCompanyCodes = reader.GetInt32(13)
            };

            if (!string.IsNullOrWhiteSpace(data.Hours))
            {
                data.Seconds = CalcSeconds(data.Hours, 0);
            }

            result.Add(data);
        }

        return result;
    }

    private static double CalculateRate(double rate, double multiple, double seconds)
    {
        if (rate == 0 || multiple == 0 || seconds == 0)
        {
            return 0;
        }

        return (rate / 60d / 60d) * multiple * seconds;
    }

    private static double CalcSeconds(string hours, double percentage)
    {
        var normalized = hours.Trim();

        if (normalized.Contains('.'))
        {
            normalized = ConvertDecimalTimeToNormal(normalized);
        }

        var parts = normalized.Split(':', StringSplitOptions.TrimEntries);
        if (parts.Length != 2)
        {
            return 0;
        }

        if (!int.TryParse(parts[0], out var hh) || !int.TryParse(parts[1], out var mm))
        {
            return 0;
        }

        var totalMinutes = (hh * 60) + mm;

        if (percentage > 0)
        {
            totalMinutes = (int)Math.Round(totalMinutes * (percentage / 100d), MidpointRounding.AwayFromZero);
        }

        return totalMinutes * 60d;
    }

    private static string ConvertDecimalTimeToNormal(string input)
    {
        if (!decimal.TryParse(input, out var decimalHours))
        {
            return "0:00";
        }

        var hours = (int)Math.Floor(decimalHours);
        var minutes = (int)Math.Round((decimalHours - hours) * 60m, MidpointRounding.AwayFromZero);

        if (minutes == 60)
        {
            hours += 1;
            minutes = 0;
        }

        return $"{hours}:{minutes:00}";
    }

    private async Task<List<IFRSAllocation>> GetAllocationsAsync(int companyCode)
    {
        await Task.CompletedTask;
        return new List<IFRSAllocation>();
    }

    private List<WipDetailReportData> CreateAllocationsFromWip(
        List<IFRSAllocation> allocations,
        List<WipDetailReportData> transactions)
    {
        var newTransactions = new List<WipDetailReportData>();

        if (transactions == null || transactions.Count == 0 || allocations.Count == 0)
        {
            return newTransactions;
        }

        foreach (var trans in transactions)
        {
            var hours = (trans.Hours ?? "").Trim();

            if (hours.Contains('.'))
            {
                hours = ConvertDecimalTimeToNormal(hours);
            }

            var totalMinutesAllocatedSoFar = 0;

            for (var i = 0; i < allocations.Count; i++)
            {
                var alloc = allocations[i];

                var transaction = new WipDetailReportData
                {
                    DdaRates = trans.DdaRates,
                    AxximaRates = trans.AxximaRates,
                    FirstName = trans.FirstName,
                    LastName = trans.LastName,
                    EmployeeNumber = trans.EmployeeNumber,
                    Multiple = trans.Multiple,
                    Project = alloc.Project,
                    Date = trans.Date,
                    Comment = trans.Comment,
                    Client = alloc.Client,
                    Activity = alloc.Activity,
                    AxximaCompanyCodes = alloc.CompanyCode
                };

                if (i == allocations.Count - 1)
                {
                    var totalMinutes = ConvertHoursToMinutes(hours);
                    var lastMinutes = totalMinutes - totalMinutesAllocatedSoFar;
                    transaction.Hours = ConvertMinutesToHours(lastMinutes);
                    transaction.Seconds = CalcSeconds(hours, alloc.Percentage);
                }
                else
                {
                    transaction.Hours = CalcHours(hours, alloc.Percentage);
                    transaction.Seconds = CalcSeconds(hours, alloc.Percentage);
                    totalMinutesAllocatedSoFar += ConvertHoursToMinutes(transaction.Hours);
                }

                newTransactions.Add(transaction);
            }
        }

        return newTransactions;
    }

    private static string CalcHours(string hours, double percentage)
    {
        var totalMinutes = ConvertHoursToMinutes(hours);
        var allocatedMinutes = (int)Math.Round(totalMinutes * (percentage / 100d), MidpointRounding.AwayFromZero);
        return ConvertMinutesToHours(allocatedMinutes);
    }

    private static int ConvertHoursToMinutes(string hours)
    {
        var parts = hours.Split(':', StringSplitOptions.TrimEntries);
        if (parts.Length != 2)
        {
            return 0;
        }

        if (!int.TryParse(parts[0], out var hh) || !int.TryParse(parts[1], out var mm))
        {
            return 0;
        }

        return (hh * 60) + mm;
    }

    private static string ConvertMinutesToHours(int minutes)
    {
        var hh = minutes / 60;
        var mm = minutes % 60;
        return $"{hh}:{mm:00}";
    }
}