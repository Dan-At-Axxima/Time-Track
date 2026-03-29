using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using TimeTrackerRepo.Models;

namespace TimeTrackerRepo.Pages.Shared
{
    public class _ChangeUserModel : PageModel
    {
        [BindProperty]
        public List<Users> Users { get; set; }
        public string selectedUser { get; set; }


        public void OnGet()
        {
        }
    }
}
