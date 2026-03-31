using System;

namespace TimeTrackerRepo.Models.Reports
{
    public enum WipRowType
    {
        Detail,
        AssociateTotal,
        ActivityTotal,
        ClientTotal,
        GrandTotal
    }

    public class WipDetailReportRow : WipDetailReportData
    {
        public WipRowType RowType { get; set; }

        public string Associate { get; set; } = string.Empty;

        public double AxximaAmount { get; set; }

        public string TimeDecimal { get; set; } = "0.000000";

        public string Label { get; set; } = string.Empty;

        public bool IsTotalRow => RowType != WipRowType.Detail;

        public string DisplayClientProject
        {
            get
            {
                if (string.IsNullOrWhiteSpace(Project) && string.IsNullOrWhiteSpace(Client))
                {
                    return string.Empty;
                }

                return $"{Project}:{Client}";
            }
        }
    }
}