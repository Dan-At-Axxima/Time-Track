namespace TimeTrackerRepo.Models.Reports;

public class IFRSAllocation
{
    public int CompanyCode { get; set; }
    public string Client { get; set; } = "";
    public string Project { get; set; } = "";
    public string Activity { get; set; } = "";
    public double Percentage { get; set; }
}