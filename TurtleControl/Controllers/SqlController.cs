using Microsoft.AspNetCore.Mvc;
using Microsoft.Data.SqlClient;
using System.Text;
using System.Text.Json;

namespace TurtleControl.Controllers
{
    [Route("Sql")]
    [ApiController]
    public class SqlController : ControllerBase
    {
        private readonly IConfiguration config;
        public SqlController(IConfiguration config)
        {
            this.config = config;
        }

        public record ExecPars(string proc,
            Dictionary<string, JsonElement> pars);

        private void ExecUsingReader(ExecPars request, Action<SqlDataReader> consume)
        {
            using var conn = new SqlConnection(config.GetConnectionString("DefaultConnection"));
            conn.Open();
            using var comm = conn.CreateCommand();
            comm.CommandType = System.Data.CommandType.StoredProcedure;
            comm.CommandText = request.proc;

            foreach (var (name, value) in request.pars)
            {
                object parValue = value.ValueKind == JsonValueKind.Null
                    ? DBNull.Value
                    : value.ToString();
                comm.Parameters.AddWithValue(name, parValue);
            }

            var reader = comm.ExecuteReader();
            consume(reader);
        }

        [HttpPost("Exec")]
        public IActionResult Exec([FromBody] ExecPars request)
        {
            ExecUsingReader(request, (_) =>
            {
            });

            return Ok();
        }

        [HttpPost("ExecJson")]
        public IActionResult ExecJson([FromBody] ExecPars request)
        {
            var json = new StringBuilder();
            ExecUsingReader(request, (reader) =>
            {
                while (reader.Read())
                {
                    json.Append(reader[0]?.ToString() ?? "");
                }
                if (json.Length == 0)
                {
                    json.Append("null");
                }
            });
            return Ok(json.ToString());
        }
    }
}
