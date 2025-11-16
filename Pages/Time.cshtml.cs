using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SE1StudentTracker.Services;
using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.CodeAnalysis.CSharp.Syntax;

namespace SE1StudentTracker.Pages
{
    //[Authorize(Roles = "Student")]
    public class TimeModel : Microsoft.AspNetCore.Mvc.RazorPages.PageModel
    {
        private readonly Services.OracleService _oracleService;
        public DataTable QueryResults { get; set; }
        [BindProperty]
        public string Location { get; set; }


        public DateTime Clock_in_at = DateTime.Now;

        public TimeModel(OracleService oracleServ)
        {
            _oracleService = oracleServ;
        }

        public IActionResult OnPostClockIn()
        {
            string username = this.User.Identity.Name;
            Console.WriteLine(username);
            _oracleService.ExecuteCreateUpdateDelete(
                $"INSERT INTO TIME_SESSION (USER_ID, SESSION_TYPE, LOCATION_TEXT, CLOCK_IN_AT) VALUES ('{username}', 'Clinical', '{Location}', SYSTIMESTAMP)");

            return RedirectToPage();
        }

        public IActionResult OnPostClockOut()
        {
            string username = this.User.Identity.Name;
            _oracleService.ExecuteCreateUpdateDelete(
                $"UPDATE TIME_SESSION SET CLOCK_OUT_AT = SYSTIMESTAMP WHERE USER_ID = '{username}'");

            return RedirectToPage();
        }

        public void OnGet()
        {
            QueryResults = _oracleService.ExecuteQuery("SELECT STUDENT_NUMBER FROM STUDENT_PROFILE"); 
        } 
    }
}
