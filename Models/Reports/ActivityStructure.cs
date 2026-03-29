namespace TimeTrackerRepo.Models.Reports
{
    public class ActivityStructure
    {
        public ActivityStructure(string client, string project, string activity, double multiple, int axximaCompanyCodes)
        {
            Client = client;
            Project = project;
            Activity = activity;
            Multiple = multiple;
            AxximaCompanyCodes = axximaCompanyCodes;
        }

        public string Client { get; }

        public string Project { get; }

        public string Activity { get; }

        public double Multiple { get; }

        public int AxximaCompanyCodes { get; }
    }
}