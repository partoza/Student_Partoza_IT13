using Microsoft.Data.SqlClient;

namespace Student_Partoza_IT13.Services
{
    public class AuthService
    {
        public bool IsAuthenticated { get; private set; }
        public string? Username { get; private set; }
        public int? EmployeeId { get; private set; }
        public string? EmployeeFirstName { get; private set; }
        public string? EmployeeLastName { get; private set; }
        public string? EmployeeFullName
        {
            get
            {
                var f = string.IsNullOrWhiteSpace(EmployeeFirstName) ? null : EmployeeFirstName!.Trim();
                var l = string.IsNullOrWhiteSpace(EmployeeLastName) ? null : EmployeeLastName!.Trim();
                if (f == null && l == null) return null;
                return f != null && l != null ? $"{f} {l}" : f ?? l;
            }
        }

        // Connection string is resolved from environment variable if available, otherwise fallback to the provided server string.
        private readonly string _connectionString;

        public AuthService()
        {
            _connectionString =
                Environment.GetEnvironmentVariable("DB_STUDENT_PARTOZA_IT13")
                ?? "Data Source=localhost;Initial Catalog=DB_Student_Partoza_IT13;Integrated Security=True;Encrypt=True;Trust Server Certificate=True";
        }

        public void SignIn(string username, int empId)
        {
            IsAuthenticated = true;
            Username = username;
            EmployeeId = empId;
        }

        public void SignInWithInfo(string username, int empId, string? firstName, string? lastName)
        {
            IsAuthenticated = true;
            Username = username;
            EmployeeId = empId;
            EmployeeFirstName = firstName;
            EmployeeLastName = lastName;
        }

        public void SignOut()
        {
            IsAuthenticated = false;
            Username = null;
            EmployeeId = null;
            EmployeeFirstName = null;
            EmployeeLastName = null;
        }

        // Verify credentials against dbo.tbEmployee using parameterized query to prevent SQL injection
        public async Task<bool> VerifyCredentialsAsync(string username, string password, CancellationToken cancellationToken = default)
        {
            if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
                return false;

            try
            {
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync(cancellationToken).ConfigureAwait(false);

                const string sql = @"SELECT COUNT(1) FROM dbo.tbEmployee WHERE empUsername = @u AND empPassword = @p";
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@u", username);
                cmd.Parameters.AddWithValue("@p", password);

                var resultObj = await cmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
                var count = Convert.ToInt32(resultObj);
                return count > 0;
            }
            catch
            {
                // You might want to log this in a real app
                return false;
            }
        }

        // Fetch employee ID after credentials are verified
        public async Task<int?> GetEmployeeIdAsync(string username, CancellationToken cancellationToken = default)
        {
            try
            {
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync(cancellationToken).ConfigureAwait(false);

                const string sql = @"SELECT empID FROM dbo.tbEmployee WHERE empUsername = @u";
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@u", username);

                var resultObj = await cmd.ExecuteScalarAsync(cancellationToken).ConfigureAwait(false);
                if (resultObj == null || resultObj == DBNull.Value)
                    return null;

                return Convert.ToInt32(resultObj);
            }
            catch
            {
                return null;
            }
        }

        // Fetch employee basic info (ID, first name, last name)
        public async Task<(int? EmpId, string? FirstName, string? LastName)> GetEmployeeInfoAsync(string username, CancellationToken cancellationToken = default)
        {
            try
            {
                await using var conn = new SqlConnection(_connectionString);
                await conn.OpenAsync(cancellationToken).ConfigureAwait(false);

                const string sql = @"SELECT empID, empFirstName, empLastName FROM dbo.tbEmployee WHERE empUsername = @u";
                await using var cmd = new SqlCommand(sql, conn);
                cmd.Parameters.AddWithValue("@u", username);

                await using var rdr = await cmd.ExecuteReaderAsync(cancellationToken).ConfigureAwait(false);
                if (await rdr.ReadAsync(cancellationToken).ConfigureAwait(false))
                {
                    var id = rdr.IsDBNull(0) ? (int?)null : rdr.GetInt32(0);
                    var first = rdr.IsDBNull(1) ? null : rdr.GetString(1);
                    var last = rdr.IsDBNull(2) ? null : rdr.GetString(2);
                    return (id, first, last);
                }

                return (null, null, null);
            }
            catch
            {
                return (null, null, null);
            }
        }

        // Expose connection string for other services needing DB access
        public string GetConnectionString() => _connectionString;
    }
}
