using SE1StudentTracker.Models;
using SE1StudentTracker.Services;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SE1StudentTracker.Repositories
{
    public class UserRepository
    {
        private readonly IOracleService _db;
        public UserRepository(IOracleService db) => _db = db;

        public Task<List<UserAccount>> SearchAsync(string likeEmail, int take = 50)
        {
            var sql = @"
                SELECT user_id, first_name, last_name, email, role_id
                FROM user_account
                WHERE LOWER(email) LIKE LOWER(:email)
                FETCH FIRST :take ROWS ONLY";

            return _db.QueryAsync(sql,
                r => new UserAccount
                {
                    UserId    = r.GetInt32(r.GetOrdinal("user_id")),
                    FirstName = r.GetString(r.GetOrdinal("first_name")),
                    LastName  = r.GetString(r.GetOrdinal("last_name")),
                    Email     = r.GetString(r.GetOrdinal("email")),
                    RoleId    = r.GetInt32(r.GetOrdinal("role_id"))
                },
                new (string, object?)[] { (":email", $"%{likeEmail}%"), (":take", take) });
        }
    }
}
