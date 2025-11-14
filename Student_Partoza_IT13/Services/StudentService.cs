using Microsoft.Data.SqlClient;

namespace Student_Partoza_IT13.Services
{
 public class StudentService
 {
 private readonly string _connectionString;

 public StudentService(AuthService auth)
 {
 _connectionString = auth.GetConnectionString();
 }

 public record StudentDto(
 int StudID,
 string StudName,
 string StudLastName,
 DateTime StudBirthdate,
 string StudYearLevel,
 string StudProgram,
 int CreatedByEmployee,
 string? Status
 );

 public async Task<List<StudentDto>> GetStudentsAsync(string? status = null, CancellationToken ct = default)
 {
 var list = new List<StudentDto>();
 await using var conn = new SqlConnection(_connectionString);
 await conn.OpenAsync(ct).ConfigureAwait(false);

 var sql = "SELECT studID, studName, studLastName, studBirthdate, studYearLevel, studProgram, createdByEmployee, status FROM dbo.tbStudent";
 if (!string.IsNullOrWhiteSpace(status))
 sql += " WHERE status = @status";

 await using var cmd = new SqlCommand(sql, conn);
 if (!string.IsNullOrWhiteSpace(status))
 cmd.Parameters.AddWithValue("@status", status);

 await using var rdr = await cmd.ExecuteReaderAsync(ct).ConfigureAwait(false);
 while (await rdr.ReadAsync(ct).ConfigureAwait(false))
 {
 var dto = new StudentDto(
 rdr.GetInt32(0),
 rdr.GetString(1),
 rdr.GetString(2),
 rdr.GetDateTime(3),
 rdr.GetString(4),
 rdr.GetString(5),
 rdr.GetInt32(6),
 rdr.IsDBNull(7) ? null : rdr.GetString(7)
 );
 list.Add(dto);
 }
 return list;
 }

 public async Task<int> AddStudentAsync(string firstName, string lastName, DateTime birthdate, string yearLevel, string program, int createdByEmployee, string status = "active", CancellationToken ct = default)
 {
 await using var conn = new SqlConnection(_connectionString);
 await conn.OpenAsync(ct).ConfigureAwait(false);

 const string sql = @"INSERT INTO dbo.tbStudent (studName, studLastName, studBirthdate, studYearLevel, studProgram, createdByEmployee, status)
 VALUES (@n, @ln, @bd, @yl, @pr, @emp, @st);
 SELECT CAST(SCOPE_IDENTITY() AS int);";
 await using var cmd = new SqlCommand(sql, conn);
 cmd.Parameters.AddWithValue("@n", firstName);
 cmd.Parameters.AddWithValue("@ln", lastName);
 cmd.Parameters.AddWithValue("@bd", birthdate.Date);
 cmd.Parameters.AddWithValue("@yl", yearLevel);
 cmd.Parameters.AddWithValue("@pr", program);
 cmd.Parameters.AddWithValue("@emp", createdByEmployee);
 cmd.Parameters.AddWithValue("@st", status);

 var obj = await cmd.ExecuteScalarAsync(ct).ConfigureAwait(false);
 return obj == null ?0 : (int)obj;
 }

 public async Task UpdateStudentAsync(int studId, string firstName, string lastName, DateTime birthdate, string yearLevel, string program, string status, CancellationToken ct = default)
 {
 await using var conn = new SqlConnection(_connectionString);
 await conn.OpenAsync(ct).ConfigureAwait(false);

 const string sql = @"UPDATE dbo.tbStudent 
 SET studName=@n, studLastName=@ln, studBirthdate=@bd, studYearLevel=@yl, studProgram=@pr, status=@st 
 WHERE studID=@id";
 await using var cmd = new SqlCommand(sql, conn);
 cmd.Parameters.AddWithValue("@n", firstName);
 cmd.Parameters.AddWithValue("@ln", lastName);
 cmd.Parameters.AddWithValue("@bd", birthdate.Date);
 cmd.Parameters.AddWithValue("@yl", yearLevel);
 cmd.Parameters.AddWithValue("@pr", program);
 cmd.Parameters.AddWithValue("@st", status);
 cmd.Parameters.AddWithValue("@id", studId);

 await cmd.ExecuteNonQueryAsync(ct).ConfigureAwait(false);
 }

 public async Task SetStatusAsync(int studId, string status, CancellationToken ct = default)
 {
 await using var conn = new SqlConnection(_connectionString);
 await conn.OpenAsync(ct).ConfigureAwait(false);

 const string sql = @"UPDATE dbo.tbStudent SET status=@st WHERE studID=@id";
 await using var cmd = new SqlCommand(sql, conn);
 cmd.Parameters.AddWithValue("@st", status);
 cmd.Parameters.AddWithValue("@id", studId);
 await cmd.ExecuteNonQueryAsync(ct).ConfigureAwait(false);
 }
 }
}
