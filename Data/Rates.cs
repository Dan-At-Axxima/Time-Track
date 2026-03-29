using System.ComponentModel.DataAnnotations;

namespace TimeTrackerRepo.Data
{
    public class Rates
    {
        [Key]
        public int EmployeeNumber { get; set; }
        public double DDARates { get; set; }
        public double AxximaRates { get; set; }
    }
}
