using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TimeTrackerRepo.Data;
using TimeTrackerRepo.Functions;
using TimeTrackerRepo.Models;
using Newtonsoft.Json;
using TimeTrackerRepo.Services;
using System.Globalization;
using TimeTrackerRepo.Pages;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.SignalR;

namespace TimeTrackerRepo.Controllers
{
    [Route("api/[Controller]")]
    [ApiController]
    
    public class DataAccessController : ControllerBase
    {
        private ILogger<IndexModel> _logger;
        private TimeTrackerContext _dbContext;
        private IHttpContextAccessor _httpContextAccessor;
        private ICurrentUserService _currentUserService;
        private IUserFunctions _userFunctions;
        private IDataFunctions _dataFunctions;
        private readonly IHubContext<FrozenDateHub> _hub;

        public DataAccessController(ILogger<IndexModel> logger,TimeTrackerContext dbContext, IHttpContextAccessor httpContextAccessor, ICurrentUserService currentUserService,
                                IUserFunctions userFunctions, IDataFunctions dataFunctions,IHubContext<FrozenDateHub> hub)
        {
            _logger = logger;
            _dbContext = dbContext;
            _httpContextAccessor = httpContextAccessor;
            _currentUserService = currentUserService;
            _userFunctions = userFunctions;
            _dataFunctions = dataFunctions;
            _hub = hub;
        }
        [HttpGet("SayHello")]
        public object Get()
        {
            return new { Moniker = "ATL2018", Name = "Atlanta Code Cammp" };
        }

        [HttpGet("GetFrozenDate")]
        public IActionResult GetFrozenDate()
        {
            var date = _dataFunctions.GetFrozenDate();
            return Ok(date);
        }

        [HttpPost("SetFrozenDate")]
        public async Task<IActionResult> SetFrozenDate(FrozenDateUpdate date)
        {
            _dataFunctions.SetFrozenDate(date.Date);

            await _hub.Clients.All.SendAsync("FrozenDateChanged", date.Date);

            return Ok();
        }

        [HttpGet("GetActivityList")]
        public IActionResult GetActivityList()
        {
            string? userName = string.Empty;
            userName = _httpContextAccessor.HttpContext.User.Identity.Name;
            var activityList = _userFunctions.GetActivityList(userName);
            
            var json = JsonConvert.SerializeObject(activityList);
            var result = new JsonResult(json);
            return Ok(json);
        }

        [HttpGet("GetActivityByUser")]
        public IActionResult GetActivityByUser(string id)
        {
            string? userName = string.Empty;
            userName = _httpContextAccessor.HttpContext.User.Identity.Name;
            var activityList =_userFunctions.GetActivityList(userName);

            var json = JsonConvert.SerializeObject(activityList);
            var result = new JsonResult(json);
            return Ok(json);
        }

        [HttpPost("SaveActivity")]
        public ActionResult<UpdateValues> SaveActivity(UpdateValues updateValues)
        {
            var currentUser = _httpContextAccessor.HttpContext.Session.GetString("currentUser");
            updateValues.User = currentUser == null ? String.Empty : currentUser;

            var success = _dataFunctions.SaveTransaction(updateValues);

            return (Ok());
        }

        [HttpPost("SaveComment")]
        public ActionResult<UpdateValues> SaveComment(UpdateValues updateValues)
        {
            var currentUser = _httpContextAccessor.HttpContext.Session.GetString("currentUser");
            updateValues.User = currentUser == null ? String.Empty : currentUser;

            int day;
            Int32.TryParse(updateValues.Day, out day);
            var success = _dataFunctions.SaveCellValueAndComment(updateValues, _currentUserService.AllDates[day]);

            return (Ok());
        }

        [HttpPost("IgnoreThis")]
        public ActionResult<UpdateValues> IgnoreThis(SaveCellValues updateValues)
        {
                return Ok(new { success = true });

        }

        [HttpPost("SaveCellData")]
        public ActionResult<UpdateValues> SaveCellData(SaveCellValues updateValues)
        {
            //var currentUser = _httpContextAccessor.HttpContext.Session.GetString("currentUser");
            //updateValues.User = currentUser == null ? String.Empty : currentUser;
            var message = "we are at the function save cell data " + updateValues.Comment;
            _logger.LogInformation(message);
            var date = DateTime.ParseExact(updateValues.Date, "yyyy-MM-dd", CultureInfo.InvariantCulture);
            var success = _dataFunctions.SaveCellData(_logger,updateValues, date);
            if (success)
            {
                return Ok(new { success = true });
            }
            else
            {
                return BadRequest(new { success = false });
            }

        }

        [HttpPost("GetCellData")]
        public ActionResult<UpdateValues> GetCellData(GetCellValues cellValues)
        {
            var currentUser = _httpContextAccessor.HttpContext.Session.GetString("currentUser");
            currentUser = currentUser == null ? String.Empty : currentUser;

            //var message = "we are at the function save cell data " + cellValues.Comment;
            //_logger.LogInformation(message);
            CellValues values = new  CellValues(_dbContext, cellValues.Client, cellValues.Project, cellValues.Activity, currentUser, cellValues.Date);
            var outPut = values.GetCellValues();
            var json = JsonConvert.SerializeObject(outPut);
            return (Ok(json));
        }

        [HttpPost("DeleteCellContent")]
        public ActionResult<UpdateValues> DeleteCellContent(UpdateValues updateValues)
        {
            var currentUser = _httpContextAccessor.HttpContext.Session.GetString("currentUser");
            updateValues.User = currentUser == null ? String.Empty : currentUser;

            var success = _dataFunctions.DeleteCellContent(updateValues,_logger);

            return (Ok());
        }

        [HttpPost("BatchSave")]
        public ActionResult BatchSave(BatchUpdateValues updateValues)
        {
            var retValue = _dataFunctions.AddBatchRecord(updateValues);
            return Ok(retValue);
        }

        [HttpPost("BatchUpdate")]
        public ActionResult BatchUpdate(BatchUpdateValues updateValues)
        {
            _dataFunctions.UpdateBatchRecord(updateValues);

            return (Ok());
        }

        [HttpPost("BatchRemove")]
        public ActionResult BatchRemove(BatchUpdateValues updateValues)
        {
            _dataFunctions.DeleteBatchRecord(updateValues);

            return (Ok());
        }

        [HttpPost("RemoveAssignment")]
        public ActionResult RemoveAssignment(AssignmentListItem listItem)
        {
           _dataFunctions.RemoveAssignment(listItem);

            return (Ok());
        }

        [HttpPost("AddAssignment")]
        public ActionResult AddAssignment(AssignmentListItem listItem)
        {
           _dataFunctions.AddAssignment(listItem);

            return (Ok());
        }

    }
}
