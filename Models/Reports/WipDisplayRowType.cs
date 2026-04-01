namespace TimeTrackerRepo.Models.Reports
{
    public enum WipDisplayRowType
    {
        Detail,
        Divider,
        AssociateTotal,
        ActivityTotal,
        ClientTotal,
        GrandTotal
    }

    public class WipDisplayRow
    {
        public WipDisplayRowType RowType { get; set; }

        public string ClientProject { get; set; } = string.Empty;
        public string Activity { get; set; } = string.Empty;
        public string Associate { get; set; } = string.Empty;
        public string DateText { get; set; } = string.Empty;
        public string TimeText { get; set; } = string.Empty;
        public string AmountText { get; set; } = string.Empty;
        public string Description { get; set; } = string.Empty;

        public string Label { get; set; } = string.Empty;
    }
}