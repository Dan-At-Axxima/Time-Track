namespace TimeTrackerRepo.Models.Reports;

public class WipDetailReportRow
{
    public string Client { get; set; } = string.Empty;
    public string Project { get; set; } = string.Empty;
    public string Activity { get; set; } = string.Empty;

    public int EmployeeNumber { get; set; }
    public int AxximaCompanyCodes { get; set; }

    public string FirstName { get; set; } = string.Empty;
    public string LastName { get; set; } = string.Empty;

    public DateTime Date { get; set; }

    public string Hours { get; set; } = string.Empty;
    public TimeSpan Time { get; set; } = TimeSpan.Zero;
    public double Seconds { get; set; }

    public double DdaRates { get; set; }
    public double AxximaRates { get; set; }
    public double Multiple { get; set; } = 1.0;

    public string Comment { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;

    public string EmployeeName =>
        $"{FirstName} {LastName}".Trim();
    public string DecimalHours { get; set; } = string.Empty;
    public string SlipId { get; set; } = string.Empty;
}