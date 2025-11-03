using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace SE1StudentTracker.Pages
{
    [Authorize(Roles = "Teacher")]
    public class StudentsModel : PageModel
    {
        public void OnGet()
        {
        }
    }
}
