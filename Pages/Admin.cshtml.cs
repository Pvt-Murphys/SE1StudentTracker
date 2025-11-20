using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using System.Data;
using SE1StudentTracker.Data;
using Microsoft.Data.Sqlite;

namespace SE1StudentTracker.Pages
{
    [Authorize(Roles = "Admin")]
    [ValidateAntiForgeryToken]
    public class AdminModel : PageModel
    {
        private readonly IConfiguration _config;
        private readonly ILogger<AdminModel> _logger;
        private readonly AppDbContext _dbContext;
        private readonly string _connectionString;

        public AdminModel(IConfiguration config, ILogger<AdminModel> logger, AppDbContext dbContext)
        {
            _config = config;
            _logger = logger;
            _dbContext = dbContext;
            _connectionString = _config.GetConnectionString("DefaultConnection")!;
        }

        // Bound properties for the form
        [BindProperty]
        public string Sql { get; set; } = "";

        [BindProperty]
        public bool AllowChanges { get; set; }

        // Output
        public string? ResultMessage { get; set; }
        public Exception? Exception { get; set; }
        public DataTable? ResultTable { get; set; }

        private const int MaxRows = 1000;
        private const int CommandTimeout = 30;

        public void OnGet()
        {
            // Just render empty form
        }

        public async Task<IActionResult> OnPostAsync(CancellationToken cancellationToken)
        {
            if (string.IsNullOrWhiteSpace(Sql))
            {
                ModelState.AddModelError(string.Empty, "SQL query cannot be empty.");
                return Page();
            }

            try
            {
                await using var conn = new SqliteConnection(_connectionString);
                await conn.OpenAsync(cancellationToken);

                await using SqliteTransaction tx = (SqliteTransaction)await conn.BeginTransactionAsync(cancellationToken);
                await using var cmd = conn.CreateCommand();
                cmd.Transaction = tx;
                cmd.CommandText = Sql;
                cmd.CommandTimeout = CommandTimeout;

                var firstWord = Sql.TrimStart().Split(' ', '\n', '\r', '\t').FirstOrDefault()?.ToUpperInvariant() ?? "";

                if (firstWord == "SELECT" || Sql.TrimStart().StartsWith("WITH", StringComparison.OrdinalIgnoreCase))
                {
                    await using var reader = await cmd.ExecuteReaderAsync(cancellationToken);
                    var dt = new DataTable();
                    dt.Load(reader);

                    if (dt.Rows.Count > MaxRows)
                    {
                        var truncated = dt.Clone();
                        for (int i = 0; i < MaxRows; i++)
                            truncated.ImportRow(dt.Rows[i]);
                        ResultTable = truncated;
                        ResultMessage = $"Result truncated to {MaxRows} rows.";
                    }
                    else
                    {
                        ResultTable = dt;
                        ResultMessage = $"Returned {dt.Rows.Count} rows.";
                    }
                }
                else
                {
                    var affected = await cmd.ExecuteNonQueryAsync(cancellationToken);
                    ResultMessage = $"Rows affected: {affected}.";
                }

                if (AllowChanges)
                {
                    await tx.CommitAsync(cancellationToken);
                    ResultMessage += " (Committed)";
                }
                else
                {
                    await tx.RollbackAsync(cancellationToken);
                    ResultMessage += " (Rolled back)";
                }
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Admin SQL execution failed");
                Exception = ex;
                ResultMessage = "Error executing query: " + ex.Message;
            }

            return Page();
        }
    }
}