using SE1StudentTracker.Models;
using SE1StudentTracker.Services;
using System.Collections.Generic;
using System.Data;
using System.Threading.Tasks;

namespace SE1StudentTracker.Repositories
{
    public class TimeSessionRepository
    {
        private readonly IOracleService _db;
        public TimeSessionRepository(IOracleService db) => _db = db;

        // Pull from your view v_timesession_full for instructor-facing pages
        public Task<List<TimeSessionFull>> GetRecentSessionsAsync(int take = 100)
        {
            string sql = @"
                SELECT
                  session_id, user_id, first_name, last_name, email, role_id, role_name,
                  session_type, location_text, section_id, section_code, course_code, course_name,
                  clock_in, clock_out
                FROM v_timesession_full
                ORDER BY clock_in DESC
                FETCH FIRST :take ROWS ONLY";

            return _db.QueryAsync(sql,
                map: r => new TimeSessionFull
                {
                    SessionId   = r.GetInt32(r.GetOrdinal("session_id")),
                    UserId      = r.GetInt32(r.GetOrdinal("user_id")),
                    FirstName   = r.GetString(r.GetOrdinal("first_name")),
                    LastName    = r.GetString(r.GetOrdinal("last_name")),
                    Email       = r.GetString(r.GetOrdinal("email")),
                    RoleId      = r.GetInt32(r.GetOrdinal("role_id")),
                    RoleName    = r.GetString(r.GetOrdinal("role_name")),
                    SessionType = r.GetString(r.GetOrdinal("session_type")),
                    LocationText= r.IsDBNull(r.GetOrdinal("location_text")) ? null : r.GetString(r.GetOrdinal("location_text")),
                    SectionId   = r.IsDBNull(r.GetOrdinal("section_id")) ? (int?)null : r.GetInt32(r.GetOrdinal("section_id")),
                    SectionCode = r.IsDBNull(r.GetOrdinal("section_code")) ? null : r.GetString(r.GetOrdinal("section_code")),
                    CourseCode  = r.IsDBNull(r.GetOrdinal("course_code")) ? null : r.GetString(r.GetOrdinal("course_code")),
                    CourseName  = r.IsDBNull(r.GetOrdinal("course_name")) ? null : r.GetString(r.GetOrdinal("course_name")),
                    ClockIn     = r.GetDateTime(r.GetOrdinal("clock_in")),
                    ClockOut    = r.IsDBNull(r.GetOrdinal("clock_out")) ? (System.DateTime?)null : r.GetDateTime(r.GetOrdinal("clock_out"))
                },
                parameters: new[] { (":take", (object)take) });
        }

        public Task<int> ClockInAsync(int userId, string sessionType, string? locationText, int? sectionId)
        {
            string sql = @"
                INSERT INTO time_session (session_id, user_id, session_type, location_text, section_id, clock_in)
                VALUES (time_session_seq.NEXTVAL, :user_id, :session_type, :location_text, :section_id, SYSTIMESTAMP)";

            return _db.ExecuteAsync(sql, new (string, object?)[] {
                (":user_id", userId),
                (":session_type", sessionType),
                (":location_text", (object?)locationText ?? DBNull.Value),
                (":section_id", (object?)sectionId ?? DBNull.Value)
            });
        }

        public Task<int> ClockOutAsync(int sessionId)
        {
            string sql = @"UPDATE time_session SET clock_out = SYSTIMESTAMP WHERE session_id = :session_id";
            return _db.ExecuteAsync(sql, new[] { (":session_id", (object)sessionId) });
        }
    }
}
