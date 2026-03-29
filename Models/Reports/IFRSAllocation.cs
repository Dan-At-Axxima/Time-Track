namespace TimeTrackerRepo.Models.Reports;

public class IFRSAllocation
{
    public string Client { get; set; } = string.Empty;
    public string Project { get; set; } = string.Empty;
    public string Activity { get; set; } = string.Empty;
    public double Percentage { get; set; }
    public int CompanyCode { get; set; }
    public DateTime Date { get; set; }
}