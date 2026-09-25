using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SmartLogApi.Data;
using SmartLogApi.Models;

namespace SmartLogApi.Controllers;

[ApiController]
[Route("api/[controller]")]
public class LogsController : ControllerBase
{
    private readonly LogContext _context;

    public LogsController(LogContext context)
    {
        _context = context;
    }

    // POST: api/logs
    [HttpPost]
    public async Task<ActionResult<LogEntry>> SubmitLog([FromBody] LogEntry log)
    {
        _context.Logs.Add(log);
        await _context.SaveChangesAsync();
        return CreatedAtAction(nameof(GetLogById), new { id = log.Id }, log);
    }

    // GET: api/logs
    [HttpGet]
    public async Task<ActionResult<IEnumerable<LogEntry>>> GetAllLogs()
    {
        return await _context.Logs.ToListAsync();
    }

    // GET: api/logs/5
    [HttpGet("{id}")]
    public async Task<ActionResult<LogEntry>> GetLogById(int id)
    {
        var log = await _context.Logs.FindAsync(id);
        if (log == null) return NotFound();

        return log;
    }
}
