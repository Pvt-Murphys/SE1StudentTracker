using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SE1StudentTracker.Services;
using System.Data;

namespace SE1StudentTracker.Pages
{
    [Authorize(Roles = "Student")]
    public class TimeModel : PageModel
    {
        private readonly Services.OracleService _oracleService;
        public DataTable QueryResults { get; set; }

        [BindProperty]
        public string Location { get; set; }

        public DateTime Clock_in_at = DateTime.Now;
        private readonly UserManager<IdentityUser> _user;

        public TimeModel(OracleService oracleServ)
        {
            _oracleService = oracleServ;
            //_user = user;
        }

        public IActionResult OnPostClockIn()
        {
            var name = this.User.Identity.Name;

            _oracleService.ExecuteCreateUpdateDelete($"INSERT INTO TIME_SESSION (SESSION_TYPE, LOCATION_TEXT, CLOCK_IN_AT) VALUES ('Clinical', '{Location}', SYSTIMESTAMP)");
            
            return RedirectToPage();
        }

        public IActionResult OnPostClockOut()
        {
            _oracleService.ExecuteCreateUpdateDelete("UPDATE TIME_SESSION SET CLOCK_OUT_AT = SYSTIMESTAMP WHERE USER_ID = 1");

            return RedirectToPage();
        }
        public void OnGet()
        {
            QueryResults = _oracleService.ExecuteQuery("SELECT STUDENT_NUMBER FROM STUDENT_PROFILE");
        }
    }
}
