using System.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SE1StudentTracker.Services;

namespace SE1StudentTracker.Pages
{
    // [Authorize(Roles = "Teacher")]
    public class StudentsModel : Microsoft.AspNetCore.Mvc.RazorPages.PageModel
    {
        private readonly Services.OracleService _oracleService;
        public DataTable QueryResults { get; set; }

        public StudentsModel(OracleService oracleServ)
        {
            _oracleService = oracleServ;
        }

        public Task OnPostRecords()
        {
            QueryResults = _oracleService.ExecuteQuery("SELECT USER_ID, LOCATION_TEXT, CLOCK_IN_AT, CLOCK_OUT_AT FROM TIME_SESSION");
            return Task.CompletedTask;
        }
        public void OnGet()
        {
            QueryResults = new DataTable();
        }
    }
}
