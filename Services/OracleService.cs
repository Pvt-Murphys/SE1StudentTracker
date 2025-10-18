using Oracle.ManagedDataAccess.Client;
using System.Data;

namespace SE1StudentTracker.Services
{
    public class OracleService
    {
        private readonly string _connString;

        public OracleService(string connString)
        {
            _connString = connString;
        }

        public DataTable ExecuteQuery(string sql)
        {
            var dt = new DataTable();

            var conn = new OracleConnection(_connString);

            conn.Open();

            var cmd = new OracleCommand(sql, conn);
            dt.Load(cmd.ExecuteReader());
            return dt;
        }
    }
}
