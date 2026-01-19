using Microsoft.AspNetCore.Mvc;
using Npgsql;
using System.Diagnostics;

namespace TP2_API_Complexe.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class DiagnosticController : ControllerBase
    {
        private readonly IConfiguration _configuration;

        public DiagnosticController(IConfiguration configuration)
        {
            _configuration = configuration;
        }

        [HttpGet]
        public async Task<IActionResult> GetStatus()
        {
            var statusData = new
            {
                SystemTime = DateTime.Now,
                Hostname = Environment.MachineName,
                Timezone = TimeZoneInfo.Local.DisplayName,
                GlobalStatus = "UNKNOWN",
                Checks = new List<object>()
            };

            var checks = new List<dynamic>();
            bool allPassed = true;

            // Check 1: Host Info
            checks.Add(new { Name = "API Host", Status = true, Icon = "🖥️" });

            // Check 2: Database Connection
            bool dbConnected = false;
            try
            {
                var connectionString = _configuration.GetConnectionString("DefaultConnection");
                if (!string.IsNullOrEmpty(connectionString))
                {
                    await using var conn = new NpgsqlConnection(connectionString);
                    await conn.OpenAsync();
                    
                    // Simple query to verify persistence/uptime if we had a table, for now just connection
                    using var cmd = new NpgsqlCommand("SELECT 1", conn);
                    await cmd.ExecuteScalarAsync();
                    
                    dbConnected = true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"DB Connection failed: {ex.Message}");
                dbConnected = false;
            }

            checks.Add(new { Name = "PostgreSQL", Status = dbConnected, Icon = "🗄️" });
            if (!dbConnected) allPassed = false;

            var result = new
            {
                statusData.SystemTime,
                statusData.Hostname,
                statusData.Timezone,
                GlobalStatus = allPassed ? "PASSED" : "FAILED",
                Checks = checks
            };

            return Ok(result);
        }
    }
}
