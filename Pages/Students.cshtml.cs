using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using SE1StudentTracker.Data;
using System.Data;
using Microsoft.Data.Sqlite;

namespace SE1StudentTracker.Pages
{
    [Authorize(Roles = "Teacher")]
    public class StudentsModel : PageModel
    {
        private readonly AppDbContext _dbContext;
        private readonly IConfiguration _config;
        private readonly ILogger<StudentsModel> _logger;
        private readonly string _connectionString;

        [BindProperty]
        public string StudentEmail { get; set; } = string.Empty;

        public DataTable QueryResults { get; set; } = new DataTable();

        public StudentsModel(AppDbContext dbContext, IConfiguration config, ILogger<StudentsModel> logger)
        {
            _dbContext = dbContext;
            _config = config;
            _logger = logger;
            _connectionString = _config.GetConnectionString("DefaultConnection")!;
        }

        public void OnGet()
        {
            // Page loads with empty results
        }

        public IActionResult OnPostRecords()
        {
            if (string.IsNullOrWhiteSpace(StudentEmail))
            {
                ModelState.AddModelError(string.Empty, "Please enter a student email.");
                return Page();
            }

            try
            {
                QueryResults = GetStudentTimeRecords(StudentEmail);
                _logger.LogInformation($"Retrieved {QueryResults.Rows.Count} records for student {StudentEmail}");
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Error retrieving student records");
                ModelState.AddModelError(string.Empty, "Error retrieving records: " + ex.Message);
            }

            return Page();
        }

        private DataTable GetStudentTimeRecords(string studentEmail)
        {
            var dt = new DataTable();

            try
            {
                using (var connection = new SqliteConnection(_connectionString))
                {
                    connection.Open();

                    using (var command = connection.CreateCommand())
                    {
                        command.CommandText = @"
                            SELECT 
                                UserId, 
                                LocationText, 
                                ClockInAt, 
                                ClockOutAt,
                                SessionType,
                                Status,
                                DurationMinutes
                            FROM time_session
                            WHERE UserId = @studentEmail
                            ORDER BY ClockInAt DESC";

                        command.Parameters.AddWithValue("@studentEmail", studentEmail);

                        using (var reader = command.ExecuteReader())
                        {
                            dt.Load(reader);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, $"Database error retrieving records for {studentEmail}");
                throw;
            }

            return dt;
        }
    }
}