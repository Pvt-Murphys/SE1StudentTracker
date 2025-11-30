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

        public async Task<IActionResult> OnPostClockInAsync()
        {
            try
            {
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

                // Debug logging
                _logger.LogInformation($"Clock In Attempt - UserEmail: {userEmail}, Location: {Location}");

                if (string.IsNullOrEmpty(userEmail))
                {
                    ModelState.AddModelError(string.Empty, "User not authenticated.");
                    return Page();
                }

                if (string.IsNullOrEmpty(Location))
                {
                    ModelState.AddModelError(string.Empty, "Location is required.");
                    return Page();
                }

                var now = DateTime.Now;
                var sessionType = "Clinical";
                var source = "web";
                var status = "open";
                var d = User.Identity.Name;

                _logger.LogInformation($"Inserting time_session: UserId={userEmail}, SessionType={sessionType}, Location={Location}, ClockIn={now}");


                _dbContext.Database.ExecuteSqlInterpolated(
                    $@"INSERT INTO time_session (UserId, SessionType, LocationText, ClockInAt, Source, Status, Notes, CreatedAt)
                       VALUES ({userEmail}, {sessionType}, {Location}, {now}, {source}, {status}, {d}, {now})"
                );

                _logger.LogInformation("Clock in successful");
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clocking in - Full Exception");
                _logger.LogError($"Exception Message: {ex.Message}");
                _logger.LogError($"Exception InnerException: {ex.InnerException?.Message}");
                _logger.LogError($"Exception StackTrace: {ex.StackTrace}");

                ModelState.AddModelError(string.Empty, "Error clocking in: " + ex.Message);
                return Page();
            }
        }

        public async Task<IActionResult> OnPostClockOutAsync()
        {
            try
            {
                var userEmail = User.FindFirst(ClaimTypes.Email)?.Value;

                _logger.LogInformation($"Clock Out Attempt - userEmail: {userEmail}");

                if (string.IsNullOrEmpty(userEmail))
                {
                    ModelState.AddModelError(string.Empty, "User not authenticated.");
                    return Page();
                }

                var now = DateTime.Now;
                var closedStatus = "closed";
                var openStatus = "open";

                _logger.LogInformation($"Updating time_session for UserId={userEmail} with ClockOut={now}");

                _dbContext.Database.ExecuteSqlInterpolated($@"
                    UPDATE time_session
                    SET ClockOutAt = {now},
                        Status = {closedStatus},
                        UpdatedAt = {now}
                    WHERE rowid = (
                        SELECT rowid
                        FROM time_session
                        WHERE UserId = {userEmail}
                          AND Status = {openStatus}
                        ORDER BY ClockInAt DESC
                        LIMIT 1
                    );
                ");

                _logger.LogInformation("Clock out successful");
                return RedirectToPage();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error clocking out - Full Exception");
                _logger.LogError($"Exception Message: {ex.Message}");
                _logger.LogError($"Exception InnerException: {ex.InnerException?.Message}");
                _logger.LogError($"Exception StackTrace: {ex.StackTrace}");

                ModelState.AddModelError(string.Empty, "Error clocking out: " + ex.Message);
                return Page();
            }
        }

        public void OnGet()
        {
        }
    }
}