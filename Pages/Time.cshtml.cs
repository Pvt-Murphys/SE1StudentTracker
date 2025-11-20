using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.EntityFrameworkCore;
using SE1StudentTracker.Data;
using System.Security.Claims;
using Microsoft.AspNetCore.Identity;
using System.Data;

namespace SE1StudentTracker.Pages
{
    [Authorize(Roles = "Student")]
    public class TimeModel : PageModel
    {
        private readonly AppDbContext _dbContext;
        private readonly ILogger<TimeModel> _logger;
        private readonly UserManager<IdentityUser> _userManager;

        [BindProperty]
        public string Location { get; set; } = string.Empty;

        public TimeModel(AppDbContext dbContext, ILogger<TimeModel> logger, UserManager<IdentityUser> userManager)
        {
            _dbContext = dbContext;
            _logger = logger;
            _userManager = userManager;
        }
        public DateTime Clock_in_at = DateTime.Now;
        private readonly UserManager<IdentityUser> _user;


        public async Task<IActionResult> OnPostClockInAsync()
        {
            try
            {
                string username = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                //Console.WriteLine(username);
                if (string.IsNullOrEmpty(username))
                {
                    ModelState.AddModelError(string.Empty, "User not authenticated.");
                    return Page();
                }

                var now = DateTime.Now;
                var sessionType = "Clinical";
                var source = "web";
                var status = "open";

                _dbContext.Database.ExecuteSqlInterpolated(
                    $@"INSERT INTO time_session (user_id, session_type, location_text, clock_in_at, source, status, created_at)
                    VALUES ({userId}, {sessionType}, {Location}, {now}, {source}, {status}, {now})"
                );

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clocking in");
                ModelState.AddModelError(string.Empty, "Error clocking in: " + ex.Message);
                return Page();
            }

        }

        public async Task<IActionResult> OnPostClockOutAsync()
        {
            try
            {
                // Get current user ID from claims
                var userId = User.FindFirst(ClaimTypes.NameIdentifier)?.Value;
                if (string.IsNullOrEmpty(userId))
                {
                    ModelState.AddModelError(string.Empty, "User not authenticated.");
                    return Page();
                }

                var now = DateTime.Now;
                var closedStatus = "closed";
                var openStatus = "open";

                // Update the most recent open session for this user
                _dbContext.Database.ExecuteSqlInterpolated(
                    $@"UPDATE time_session 
                       SET clock_out_at = {now}, status = {closedStatus}, updated_at = {now}
                       WHERE user_id = {userId} AND status = {openStatus}
                       ORDER BY clock_in_at DESC
                       LIMIT 1"
                );

                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clocking out");
                ModelState.AddModelError(string.Empty, "Error clocking out: " + ex.Message);
                return Page();
            }
        }

        public void OnGet()
        {
        } 
    }
}