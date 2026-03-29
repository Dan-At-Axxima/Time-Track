using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTrackerRepo.Data;
using TimeTrackerRepo.Models;
using TimeTrackerRepo.Pages;

namespace TimeTrackerRepo.Functions
{
    public interface IDataFunctions
    {
        public DateTime GetDefaultStartDate(string employeeNumber);
        public DateTime GetDefaultEndDate(string employeeNumber);

        public bool SetDefaultStartDate(string employeeNumber, DateTime startDate);
        public bool SetDefaultEndDate(string employeeNumber, DateTime endDate);
        public bool SaveTransaction(UpdateValues values);
        public UpdateValues GetCellData(CellValues cellValues);
        public bool SaveCellValueAndComment(UpdateValues values, string d1);
        public bool RemoveAssignment(AssignmentListItem listItem);
        public int AddAssignment(AssignmentListItem listItem);
        public bool DeleteCellContent(UpdateValues values, ILogger<IndexModel> logger);
        public bool SaveComment(UpdateValues values);
        public bool SaveCellData(ILogger<IndexModel> logger,SaveCellValues values, DateTime d1);
        public List<AssignmentListItem> GetUserAssignments();
        public List<Assignments> GetUserAssignments2();
        public List<Activities> GetUnAssignedActivities();
        public bool AddBatchRecord(BatchUpdateValues values);
        public bool UpdateBatchRecord(BatchUpdateValues values);
        public bool DeleteBatchRecord(BatchUpdateValues values);
        public bool SetFrozenDate(string date);
        public DateTime GetFrozenDate();
        public Task<string?> GetFrozenDateStringAsync();
    }
}
