using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SE1StudentTracker.Services;
using System.Data;

namespace SE1StudentTracker.Pages
{
    public class PrivacyModel : Microsoft.AspNetCore.Mvc.RazorPages.PageModel
    {
        private readonly ILogger<PrivacyModel> _logger;
        private readonly Services.OracleService _oracleService;
        public DataTable QueryResults { get; set; }

        public PrivacyModel(ILogger<PrivacyModel> logger, OracleService oracleServ)
        {
            _logger = logger;
            _oracleService = oracleServ;
        }

        public void OnGet()
        {
            //QueryResults = _oracleService.ExecuteQuery("SELECT * FROM USER_ACCOUNT");
        }
    }

}
